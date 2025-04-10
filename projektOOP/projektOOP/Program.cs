using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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


        private static void Register(PasswordManager passwordManager)
        {
            while (true)
            {
                Console.Clear();
                Console.Write("Enter login: ");
                string login = Console.ReadLine()?.Trim();
                if (login?.ToLower() == "quit") Environment.Exit(0);

                Console.Write("Enter password: ");
                string password1 = Console.ReadLine();
                if (password1?.ToLower() == "quit") Environment.Exit(0);

                Console.Write("Repeat password: ");
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
                    string creationDate = DateTime.Now.ToString("yyyy-MM-dd");
                    File.AppendAllText("users.txt", $"{login} {passwordManager.HashPassword(password1)} {creationDate}\n");
                    Console.WriteLine("Account created successfully!");
                    LogAction($"New user '{login}' registered.");
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

        private static void LogAction(string action)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";
            var logs = File.Exists("log.txt") ? File.ReadAllLines("log.txt").ToList() : new List<string>();
            logs.Insert(0, logEntry); // Insert at the beginning for descending order
            File.WriteAllLines("log.txt", logs);
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
        private static void MakeOrder()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Order Menu:");
                Console.WriteLine("1. View Available Products");
                Console.WriteLine("2. Place an Order");
                Console.WriteLine("3. Back to Products Menu");
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
                        PlaceOrder();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        Thread.Sleep(1500);
                        break;
                }
            }
        }

        private static void PlaceOrder()
        {
            Console.Clear();
            if (!File.Exists("products.txt"))
            {
                Console.WriteLine("No products available.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter the product name you want to order: ");
            string productName = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(productName))
            {
                Console.WriteLine("Product name cannot be empty.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter the quantity you want to order: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                Thread.Sleep(1500);
                return;
            }

            var products = File.ReadAllLines("products.txt").ToList();
            bool productFound = false;

            for (int i = 0; i < products.Count; i++)
            {
                var parts = products[i].Split(',');
                if (parts.Length >= 3 && parts[0].Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    productFound = true;
                    if (int.TryParse(parts[1], out int stock) && stock >= quantity)
                    {
                        Console.WriteLine($"Product found: {parts[0]} - Stock: {stock}");
                        Console.WriteLine("Choose payment method:");
                        Console.WriteLine("1. Card");
                        Console.WriteLine("2. Blik");
                        Console.Write("Enter your choice: ");
                        string paymentChoice = Console.ReadLine();

                        if (paymentChoice == "1")
                        {
                            Console.Write("Enter card number: ");
                            string cardNumber = Console.ReadLine()?.Trim();
                            Console.Write("Enter card expiration date (MM/YY): ");
                            string expirationDate = Console.ReadLine()?.Trim();
                            Console.Write("Enter CVV: ");
                            string cvv = Console.ReadLine()?.Trim();

                            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(expirationDate) || string.IsNullOrEmpty(cvv))
                            {
                                Console.WriteLine("Invalid card details. Payment failed.");
                                LogAction($"Payment failed for product '{productName}' (Quantity: {quantity}) - Invalid card details.");
                                Thread.Sleep(1500);
                                return;
                            }

                            bool paymentSuccess = new Random().Next(0, 100) < 99; // 99% chance of success
                            if (paymentSuccess)
                            {
                                Console.WriteLine("Payment successful! Order placed.");
                                LogAction($"Payment successful for product '{productName}' (Quantity: {quantity}).");
                                parts[1] = (stock - quantity).ToString(); // Update stock
                                products[i] = string.Join(",", parts);
                                File.WriteAllLines("products.txt", products);
                            }
                            else
                            {
                                Console.WriteLine("Payment failed. Please try again.");
                                LogAction($"Payment failed for product '{productName}' (Quantity: {quantity}).");
                            }
                        }
                        else if (paymentChoice == "2")
                        {
                            Console.Write("Enter Blik code: ");
                            string blikCode = Console.ReadLine()?.Trim();

                            if (string.IsNullOrEmpty(blikCode) || blikCode.Length != 6 || !int.TryParse(blikCode, out _))
                            {
                                Console.WriteLine("Invalid Blik code. Payment failed.");
                                LogAction($"Payment failed for product '{productName}' (Quantity: {quantity}) - Invalid Blik code.");
                                Thread.Sleep(1500);
                                return;
                            }

                            bool paymentSuccess = new Random().Next(0, 100) < 99; // 99% chance of success
                            if (paymentSuccess)
                            {
                                Console.WriteLine("Payment successful! Order placed.");
                                LogAction($"Payment successful for product '{productName}' (Quantity: {quantity}).");
                                parts[1] = (stock - quantity).ToString(); // Update stock
                                products[i] = string.Join(",", parts);
                                File.WriteAllLines("products.txt", products);
                            }
                            else
                            {
                                Console.WriteLine("Payment failed. Please try again.");
                                LogAction($"Payment failed for product '{productName}' (Quantity: {quantity}).");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid payment method.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not enough stock available.");
                    }
                    break;
                }
            }

            if (!productFound)
            {
                Console.WriteLine("Product not found.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private static void ViewLogs()
        {
            Console.Clear();
            if (!File.Exists("log.txt"))
            {
                Console.WriteLine("No logs available.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var logs = File.ReadAllLines("log.txt");
            const int pageSize = 10;
            int totalPages = (int)Math.Ceiling(logs.Length / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Logs - Page {currentPage}/{totalPages}");
                Console.WriteLine(new string('-', 30));

                var pageLogs = logs.Skip((currentPage - 1) * pageSize).Take(pageSize);
                foreach (var log in pageLogs)
                {
                    Console.WriteLine(log);
                }

                Console.WriteLine(new string('-', 30));
                Console.WriteLine("n: Next Page | p: Previous Page | q: Quit");

                var input = Console.ReadKey(true).Key;
                if (input == ConsoleKey.N && currentPage < totalPages) currentPage++;
                else if (input == ConsoleKey.P && currentPage > 1) currentPage--;
                else if (input == ConsoleKey.Q) break;
            }
        }

        private static void ViewProfile(string username)
        {
            Console.Clear();
            Console.WriteLine("User Profile:");
            Console.WriteLine($"Username: {username}");

            string creationDate = "Unknown";
            if (File.Exists("users.txt"))
            {
                foreach (var line in File.ReadLines("users.txt"))
                {
                    var parts = line.Split(' ');
                    if (parts.Length >= 3 && parts[0] == username)
                    {
                        creationDate = parts[2];
                        break;
                    }
                }
            }

            Console.WriteLine($"Date of Creation: {creationDate}");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }


        public static void Login(PasswordManager passwordManager, out Role userRole, out string username)
        {
            bool loginStatus;
            int attempts = 0;
            const int maxAttempts = 3;
            const int timeoutSeconds = 10;
            userRole = Role.User;
            username = string.Empty;

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
                    username = login;
                    LogAction($"User '{login}' logged in successfully.");

                    // Assign role based on login (for simplicity, hardcoded roles)
                    if (login == "admin") userRole = Role.Admin;
                    else if (login == "manager") userRole = Role.Manager;
                    else userRole = Role.User;
                }
                else
                {
                    Console.WriteLine("Login failed. Try again");
                    LogAction($"Unsuccessful login attempt for account '{login}'.");
                    attempts++;
                }
            } while (!loginStatus);
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
                        string username;
                        Login(passwordManager, out userRole, out username);
                        showRoleMenu(userRole, passwordManager, username);
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
    
private static void showRoleMenu(Role userRole, PasswordManager passwordManager, string username = "")
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {username}!");
                switch (userRole)
                {
                    case Role.Admin:
                        Console.WriteLine("1. Manage Users");
                        Console.WriteLine("2. View Logs");
                        break;
                    case Role.Manager:
                        Console.WriteLine("1. View Profile");
                        Console.WriteLine("2. Show Products");
                        Console.WriteLine("3. Update Stock");
                        break;
                    case Role.User:
                        Console.WriteLine("1. View Profile");
                        Console.WriteLine("2. View Products");
                        Console.WriteLine("3. Make an Order");
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
                    LogAction($"User '{username}' logged out.");
                    Thread.Sleep(1500);
                    Console.Clear();
                    break; // Logout and return to main menu
                }

                if (userRole == Role.Admin)
                {
                    switch (choice)
                    {
                        case 1:
                            ManageUsers();
                            break;
                        case 2:
                            ViewLogs();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            Thread.Sleep(1500);
                            break;
                    }
                }
                else if (userRole == Role.Manager)
                {
                    switch (choice)
                    {
                        case 1:
                            ViewProfile(username);
                            break;
                        case 2:
                            ShowProductsMenu();
                            break;
                        case 3:
                            UpdateStock();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            Thread.Sleep(1500);
                            break;
                    }
                }
                else if (userRole == Role.User)
                {
                    switch (choice)
                    {
                        case 1:
                            ViewProfile(username);
                            break;
                        case 2:
                            ShowProductsMenu();
                            break;
                        case 3:
                            MakeOrder();
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            Thread.Sleep(1500);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"You selected option {choice}. Feature not implemented yet.");
                    Thread.Sleep(2000);
                }
            }
        }

        private static void ManageUsers()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Manage Users:");
                Console.WriteLine("1. Remove User");
                Console.WriteLine("2. Back to Admin Menu");
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
                        RemoveUser();
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        Thread.Sleep(1500);
                        break;
                }
            }
        }

        private static void RemoveUser()
        {
            Console.Clear();
            Console.Write("Enter the username to remove: ");
            string usernameToRemove = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(usernameToRemove))
            {
                Console.WriteLine("Username cannot be empty.");
                Thread.Sleep(1500);
                return;
            }

            if (!File.Exists("users.txt"))
            {
                Console.WriteLine("No users found.");
                Thread.Sleep(1500);
                return;
            }

            var users = File.ReadAllLines("users.txt").ToList();
            bool userFound = false;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].StartsWith(usernameToRemove + " "))
                {
                    users.RemoveAt(i);
                    userFound = true;
                    break;
                }
            }

            if (userFound)
            {
                File.WriteAllLines("users.txt", users);
                Console.WriteLine("User removed successfully.");
                LogAction($"User '{usernameToRemove}' removed.");

            }
            else
            {
                Console.WriteLine("User not found.");
            }

            Thread.Sleep(1500);
        }

        private static void UpdateStock()
        {
            Console.Clear();
            if (!File.Exists("products.txt"))
            {
                Console.WriteLine("No products available.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter the product name to update stock: ");
            string productName = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(productName))
            {
                Console.WriteLine("Product name cannot be empty.");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter the new stock value: ");
            if (!int.TryParse(Console.ReadLine(), out int newStock) || newStock < 0)
            {
                Console.WriteLine("Invalid stock value.");
                Thread.Sleep(1500);
                return;
            }

            var products = File.ReadAllLines("products.txt").ToList();
            bool productFound = false;

            for (int i = 0; i < products.Count; i++)
            {
                var parts = products[i].Split(',');
                if (parts.Length >= 3 && parts[0].Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    parts[1] = newStock.ToString();
                    products[i] = string.Join(",", parts);
                    productFound = true;
                    break;
                }
            }

            if (productFound)
            {
                File.WriteAllLines("products.txt", products);
                Console.WriteLine("Stock updated successfully.");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }

            Thread.Sleep(1500);
        }
    }
}