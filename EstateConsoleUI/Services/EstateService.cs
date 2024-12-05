using EstateDataAccess.Repository;
using EstateDataAccess.SqlRepository;
using EstateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace EstateConsoleUI.Services
{
    public static class EstateService
    {
        private static readonly IRepository<Estate> _estateRepository = new EstateRepository();

        public static void ViewAllEstates()
        {
            var estates = _estateRepository.GetAll();
            Console.Clear();
            Console.WriteLine("=== List of Estates ===");
            foreach (var estate in estates)
            {
                if (Enum.IsDefined(typeof(EstateType), estate.Type))
                {
                    Console.WriteLine($"ID: {estate.Id}, Name: {estate.Name}, Address: {estate.Address}, Price: {estate.Price:C}, Type: {(EstateType)estate.Type}");
                }
                else
                {
                    Console.WriteLine($"ID: {estate.Id}, Name: {estate.Name}, Address: {estate.Address}, Price: {estate.Price:C}, Type: Unknown");
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }


        public static void AddEstate()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Estate ===");
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Address: ");
            var address = Console.ReadLine();
            Console.Write("Owner ID: ");
            if (!int.TryParse(Console.ReadLine(), out int ownerId))
            {
                Console.WriteLine("Invalid Owner ID! Press any key to return...");
                Console.ReadKey();
                return;
            }

            if (!OwnerExists(ownerId))
            {
                Console.WriteLine($"Owner with ID {ownerId} does not exist! Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.Write("Price: ");
            if (!double.TryParse(Console.ReadLine(), out double price))
            {
                Console.WriteLine("Invalid price! Press any key to return...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Type (1: Apartment, 2: House, 3: Ground, 4: Office): ");
            if (!int.TryParse(Console.ReadLine(), out int type) || type < 1 || type > 4)
            {
                Console.WriteLine("Invalid type! Press any key to return...");
                Console.ReadKey();
                return;
            }

            var estate = new Estate
            {
                Name = name,
                Address = address,
                OwnerId = ownerId,
                Price = price,
                Type = (EstateType)type,
                CreateDate = DateTime.Now
            };
            _estateRepository.Create(estate);

            Console.WriteLine("Do you want to add a picture for this estate? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("Enter the picture file name (e.g., picture.jpg): ");
                string pictureFileName = Console.ReadLine();

                string picturesFolder = ConfigurationManager.AppSettings["PicturesFolder"];
                string fullPicturePath = Path.Combine(picturesFolder, pictureFileName);

                if (File.Exists(fullPicturePath))
                {
            
                    FileInfo fileInfo = new FileInfo(fullPicturePath);
                    long fileSize = fileInfo.Length;

                  
                    PictureService.AddPicture(estate.Id, pictureFileName, fileSize);
                }
                else
                {
                    Console.WriteLine("The picture file does not exist!");
                }
            }

            Console.WriteLine("Estate added successfully! Press any key to return...");
            Console.ReadKey();
        }

      
        public static bool OwnerExists(int ownerId)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EstateManagementDb"].ConnectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Owner WHERE OwnerId = @OwnerId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OwnerId", ownerId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }



        public static void UpdateEstate()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Update Estate ===");
                Console.Write("Enter Estate ID: ");

             
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var estate = _estateRepository.GetById(id);
                    if (estate != null)
                    {
                        Console.Write($"Name ({estate.Name}): ");
                        var name = Console.ReadLine();
                        Console.Write($"Address ({estate.Address}): ");
                        var address = Console.ReadLine();

                        Console.Write($"Price ({estate.Price}): ");
                        var priceInput = Console.ReadLine();
                        double price = estate.Price; 

                        if (!string.IsNullOrWhiteSpace(priceInput) && double.TryParse(priceInput, out double parsedPrice))
                        {
                            price = parsedPrice;
                        }

                        Console.Write($"Type ({(EstateType)estate.Type}): ");
                        var typeInput = Console.ReadLine();
                        EstateType type = estate.Type; 

                        if (!string.IsNullOrWhiteSpace(typeInput) && Enum.TryParse(typeInput, out EstateType parsedType))
                        {
                            type = parsedType;
                        }

                        estate.Name = string.IsNullOrWhiteSpace(name) ? estate.Name : name;
                        estate.Address = string.IsNullOrWhiteSpace(address) ? estate.Address : address;
                        estate.Price = price;
                        estate.Type = type;

                        _estateRepository.Update(estate);
                        Console.WriteLine("Estate updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Estate not found!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID!");
                }
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"InvalidCastException: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }


        public static void DeleteEstate()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Delete Estate ===");
                Console.Write("Enter Estate ID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    _estateRepository.Delete(id);
                    Console.WriteLine("Estate deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid ID!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        public static void SearchAndDisplayEstates()
        {
            Console.Clear();
            Console.WriteLine("=== Search and Sort Estates ===");
            Console.Write("Enter a search term for estate name (or leave blank to skip): ");
            string searchTerm = Console.ReadLine();

            Console.Write("Sort by (1: Name, 2: Price): ");
            if (!int.TryParse(Console.ReadLine(), out int sortOption) || (sortOption != 1 && sortOption != 2))
            {
                Console.WriteLine("Invalid sort option! Press any key to return...");
                Console.ReadKey();
                return;
            }

            string sortBy = sortOption == 1 ? "Name" : "Price";

            Console.Write("Sort in descending order? (Y/N): ");
            bool descending = Console.ReadLine().Trim().ToUpper() == "Y";

            try
            {
                var results = _estateRepository.SearchAndSort(searchTerm, sortBy, descending);

                Console.WriteLine("\n=== Results ===");
                foreach (var estate in results)
                {
                    Console.WriteLine($"ID: {estate.Id}, Name: {estate.Name}, Price: {estate.Price:C}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }



    }
}