using EstateDataAccess.Repository;
using EstateDataAccess.SqlRepository;
using EstateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateConsoleUI.Services
{
    public static class OwnerService
    {
        private static readonly IRepository<Owner> _ownerRepository = new OwnerRepository();

        public static void ViewAllOwners()
        {
            var owners = _ownerRepository.GetAll();
            Console.Clear();
            Console.WriteLine("=== List of Owners ===");
            foreach (var owner in owners)
            {
                Console.WriteLine($"ID: {owner.Id}, Name: {owner.Name}, Email: {owner.Email}, Phone: {owner.Phone}");
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        public static void AddOwner()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Owner ===");
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Email: ");
            var email = Console.ReadLine();
            Console.Write("Phone: ");
            var phone = Console.ReadLine();
            Console.Write("CNP: ");
            var cnp = Console.ReadLine();

            var owner = new Owner { Name = name, Email = email, Phone = phone, Cnp = cnp };
            _ownerRepository.Create(owner);

            Console.WriteLine("Owner added successfully! Press any key to return...");
            Console.ReadKey();
        }

        public static void UpdateOwner()
        {
            Console.Clear();
            Console.WriteLine("=== Update Owner ===");
            Console.Write("Enter Owner ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var owner = _ownerRepository.GetById(id);
                if (owner != null)
                {
                    Console.Write($"Name ({owner.Name}): ");
                    var name = Console.ReadLine();
                    Console.Write($"Email ({owner.Email}): ");
                    var email = Console.ReadLine();
                    Console.Write($"Phone ({owner.Phone}): ");
                    var phone = Console.ReadLine();
                    Console.Write($"CNP ({owner.Cnp}): ");
                    var cnp = Console.ReadLine();

                    owner.Name = string.IsNullOrWhiteSpace(name) ? owner.Name : name;
                    owner.Email = string.IsNullOrWhiteSpace(email) ? owner.Email : email;
                    owner.Phone = string.IsNullOrWhiteSpace(phone) ? owner.Phone : phone;
                    owner.Cnp = string.IsNullOrWhiteSpace(cnp) ? owner.Cnp : cnp;

                    _ownerRepository.Update(owner);
                    Console.WriteLine("Owner updated successfully!");
                }
                else
                {
                    Console.WriteLine("Owner not found!");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID!");
            }
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        public static void DeleteOwner()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Owner ===");
            Console.Write("Enter Owner ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _ownerRepository.Delete(id);
                Console.WriteLine("Owner deleted successfully!");
            }
            else
            {
                Console.WriteLine("Invalid ID!");
            }
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}