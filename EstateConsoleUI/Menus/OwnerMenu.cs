using EstateConsoleUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateConsoleUI.Menus
{
    public static class OwnerMenu
    {
        public static void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Manage Owners ===");
                Console.WriteLine("1. View All Owners");
                Console.WriteLine("2. Add Owner");
                Console.WriteLine("3. Update Owner");
                Console.WriteLine("4. Delete Owner");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        OwnerService.ViewAllOwners();
                        break;
                    case "2":
                        OwnerService.AddOwner();
                        break;
                    case "3":
                        OwnerService.UpdateOwner();
                        break;
                    case "4":
                        OwnerService.DeleteOwner();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}