using EstateConsoleUI.Services;
using System;

namespace EstateConsoleUI.Menus
{
    public static class EstateMenu
    {
        public static void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Estate Menu ===");
                Console.WriteLine("1. View All Estates");
                Console.WriteLine("2. Add New Estate");
                Console.WriteLine("3. Update Estate");
                Console.WriteLine("4. Delete Estate");
                Console.WriteLine("5. Search and Sort Estates");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EstateService.ViewAllEstates();
                        break;
                    case "2":
                        EstateService.AddEstate();
                        break;
                    case "3":
                        EstateService.UpdateEstate();
                        break;
                    case "4":
                        EstateService.DeleteEstate();
                        break;
                    case "5":
                        EstateService.SearchAndDisplayEstates();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

    }
}