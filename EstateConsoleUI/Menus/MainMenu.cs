using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstateConsoleUI.Menus
{
    public static class MainMenu
    {
        public static void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Estate Management Application ===");
                Console.WriteLine("1. Manage Owners");
                Console.WriteLine("2. Manage Estates");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        OwnerMenu.Display();
                        break;
                    case "2":
                        EstateMenu.Display();
                        break;
                    case "3":
                        Console.WriteLine("Exiting application. Goodbye!");
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