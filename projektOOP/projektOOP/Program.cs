using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using projektOOP.Classes;

namespace projektOOP
{
    public enum Role
    {
        Admin,
        Manager,
        User
    }
    public interface IPayment
    {
        string Name { get; set; }
        void Pay();
    }

    internal class Program
    {
        public static void Login(PasswordManager passwordManager, out Role userRole)
        {
            bool loginStatus;
            int attempts = 0;
            const int maxAttempts = 3;
            const int timeoutSeconds = 10;
            userRole = Role.User;

            do
            {
                if (attempts >= maxAttempts)
                {
                    Console.WriteLine($"Too many failed attempts. Please wait {timeoutSeconds} seconds before trying again.");
                    Thread.Sleep(timeoutSeconds * 1000);
                    attempts = 0;
                }

                Thread.Sleep(750);
                Console.Clear();
                Console.Write("Enter login: ");
                string login = Console.ReadLine();
                if (login?.ToLower() == "quit") Environment.Exit(0);

                Console.Write("Enter password: ");
                string password = Console.ReadLine();
                if (password?.ToLower() == "quit") Environment.Exit(0);

                loginStatus = passwordManager.VerifyPassword(login, password);

                if (loginStatus)
                {
                    Console.WriteLine("Login successful");

                    // Assign role based on login (for simplicity, hardcoded roles)
                    if (login == "admin") userRole = Role.Admin;
                    else if (login == "manager") userRole = Role.Manager;
                    else userRole = Role.User;
                }
                else
                {
                    Console.WriteLine("Login failed. Try again");
                    attempts++;
                }
            } while (!loginStatus);
        }

        public static void Register(PasswordManager passwordManager)
        {
            while (true)
            {
                Console.Clear();
                Console.Write("Enter login (or type 'quit' to exit): ");
                string login = Console.ReadLine()?.Trim();
                if (login?.ToLower() == "quit") Environment.Exit(0);

                Console.Write("Enter password (or type 'quit' to exit): ");
                string password1 = Console.ReadLine();
                if (password1?.ToLower() == "quit") Environment.Exit(0);

                Console.Write("Repeat password (or type 'quit' to exit): ");
                string password2 = Console.ReadLine();
                if (password2?.ToLower() == "quit") Environment.Exit(0);

                if (string.IsNullOrEmpty(login))
                {
                    Console.WriteLine("Login cannot be empty");
                    Thread.Sleep(1500);
                    continue;
                }

                if (password1 != password2)
                {
                    Console.WriteLine("Passwords do not match");
                    Thread.Sleep(1500);
                    continue;
                }

                bool userExists = false;
                if (File.Exists("users.txt"))
                {
                    foreach (var line in File.ReadLines("users.txt"))
                    {
                        if (line.StartsWith(login + " "))
                        {
                            userExists = true;
                            break;
                        }
                    }
                }

                if (userExists)
                {
                    Console.WriteLine("Username already exists");
                    Thread.Sleep(1500);
                    continue;
                }

                try
                {
                    File.AppendAllText("users.txt", $"{login} {passwordManager.HashPassword(password1)}\n");
                    Console.WriteLine("Account created successfully!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating account: {ex.Message}");
                    Thread.Sleep(1500);
                }
            }
        }

        static void Main(string[] args)
        {
            PasswordManager passwordManager = new PasswordManager();
            showMenu(passwordManager);
        }

        private static void showMenu(PasswordManager passwordManager)
        {
            while (true)
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Choose option (or type 'quit' in any moment of the program to exit): ");
                string input = Console.ReadLine();
                if (input?.ToLower() == "quit") Environment.Exit(0);

                int choice;
                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Wrong input");
                    Thread.Sleep(1000);
                    Console.Clear();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Role userRole;
                        Login(passwordManager, out userRole);
                        showRoleMenu(userRole, passwordManager);
                        break;
                    case 2:
                        Register(passwordManager);
                        break;
                    default:
                        Environment.Exit(0);
                        break;
                }
            }
        }


        private static void showRoleMenu(Role userRole, PasswordManager passwordManager)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {userRole}!");
                switch (userRole)
                {
                    case Role.Admin:
                        Console.WriteLine("1. Manage Users");
                        Console.WriteLine("2. View Logs");
                        break;
                    case Role.Manager:
                        Console.WriteLine("1. View Reports");
                        Console.WriteLine("2. Manage Projects");
                        break;
                    case Role.User:
                        Console.WriteLine("1. View Profile");
                        Console.WriteLine("2. Update Settings");
                        Console.WriteLine("3. View Products");
                        break;
                }
                Console.WriteLine("4. Logout");
                Console.Write("Choose option: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Wrong input");
                    Thread.Sleep(2000);
                    continue;
                }

                if (choice == 4)
                {
                    Console.WriteLine("Successfully logged out.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    break; // Logout and return to main menu
                }

                if (userRole == Role.User && choice == 3)
                {
                    ShowProductsMenu();
                    continue;
                }

                Console.WriteLine($"You selected option {choice}. Feature not implemented yet.");
                Thread.Sleep(2000);
            }
        }


        private static void ShowProductsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Products Menu:");
                Console.WriteLine("1. View All Products");
                Console.WriteLine("2. Search by Name");
                Console.WriteLine("3. Filter by Price");
                Console.WriteLine("4. Back to Role Menu");
                Console.Write("Choose an option: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input. Try again.");
                    Thread.Sleep(1500);
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        DisplayAllProducts();
                        break;
                    case 2:
                        SearchProductByName();
                        break;
                    case 3:
                        FilterProductsByPrice();
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        Thread.Sleep(1500);
                        break;
                }
            }
        }

        private static void DisplayAllProducts()
        {
            Console.Clear();
            if (!File.Exists("products.txt"))
            {
                Console.WriteLine("No products available.");
            }
            else
            {
                foreach (var line in File.ReadLines("products.txt"))
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private static void SearchProductByName()
        {
            Console.Clear();
            Console.Write("Enter product name to search: ");
            string name = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty.");
                Thread.Sleep(1500);
                return;
            }

            bool found = false;
            if (File.Exists("products.txt"))
            {
                foreach (var line in File.ReadLines("products.txt"))
                {
                    if (line.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Console.WriteLine(line);
                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("No products found with the given name.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private static void FilterProductsByPrice()
        {
            Console.Clear();
            Console.Write("Enter minimum price: ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal minPrice))
            {
                Console.WriteLine("Invalid price format.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter maximum price: ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal maxPrice))
            {
                Console.WriteLine("Invalid price format.");
                Thread.Sleep(1500);
                return;
            }

            bool found = false;
            if (File.Exists("products.txt"))
            {
                foreach (var line in File.ReadLines("products.txt"))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 3 && decimal.TryParse(parts[2], out decimal price) && price >= minPrice && price <= maxPrice)
                    {
                        Console.WriteLine(line);
                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("No products found in the given price range.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // Renam

    }
}