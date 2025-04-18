using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektOOP.Interfaces;
using projektOOP.Services;
using projektOOP.Utils;
using projektOOP.Enums;


namespace projektOOP.Classes
{
    public class User
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public Role Role { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();
        public DateTime CreationDate { get; set; }

        private void DisplayMenu(Dictionary<Permission, (string, Action)> menuOptions, RBAC rbac)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{Role.ToString().ToUpper()} PANEL - {Username}");

                // Display menu options based on permissions
                int optionNumber = 1;
                var availableOptions = new Dictionary<int, Action>();
                foreach (var option in menuOptions)
                {
                    if (rbac.HasPermission(this, option.Key))
                    {
                        Console.WriteLine($"{optionNumber}. {option.Value.Item1}");
                        availableOptions[optionNumber] = option.Value.Item2;
                        optionNumber++;
                    }
                }
                Console.WriteLine("0. Logout");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice) ||
                    (choice != 0 && !availableOptions.ContainsKey(choice)))
                {
                    Console.WriteLine("Invalid input!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    continue;
                }

                if (choice == 0) return;
                availableOptions[choice].Invoke();
            }
        }
        private void ViewOrders(ILogger logger, OrderManager orderManager)
        {
            Console.Clear();
            Console.WriteLine("ORDER LOGS:");
            if (!File.Exists("orders.txt"))
            {
                Console.WriteLine("No orders found!");
            }
            else
            {
                var orders = orderManager.GetOrders().ToList();
                const int pageSize = 15;
                int currentPage = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"ORDERS - Page {currentPage + 1}/{(orders.Count + pageSize - 1) / pageSize}");
                    foreach (var order in orders.Skip(currentPage * pageSize).Take(pageSize))
                    {
                        Console.WriteLine(order);
                    }

                    Console.WriteLine("\nN - Next Page | P - Previous Page | Q - Quit");
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.N && (currentPage + 1) * pageSize < orders.Count)
                        currentPage++;
                    else if (input == ConsoleKey.P && currentPage > 0)
                        currentPage--;
                    else if (input == ConsoleKey.Q)
                        break;
                }
            }
            logger.Log($"User {Username} viewed order logs");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public void ShowMenu(PasswordManager passwordManager, ILogger logger, RBAC rbac, OrderManager orderManager)
        {
            // Define menu options with their required permissions and corresponding actions
            var menuOptions = new Dictionary<Permission, (string, Action)>
            {
                { Permission.ViewUsers, ("List Users", () => ListUsers(logger)) },
                { Permission.ManageUsers, ("Remove User", () => RemoveUser(logger)) },
                { Permission.ViewLogs, ("View Logs", () => ViewLogs(logger)) },
                { Permission.ViewProducts, ("View Products", () => ViewProducts(logger)) },
                { Permission.UpdateStock, ("Update Stock", () => UpdateStock(logger)) },
                { Permission.PlaceOrder, ("Place Order", () => PlaceOrder(logger, orderManager)) },
                { Permission.ViewProfile, ("View Profile", () => ViewProfile(logger)) },
                { Permission.ViewOrderLogs, ("View Order Logs", () => ViewOrders(logger, orderManager)) }
            };

            // Filter menu options based on the user's permissions
            var filteredMenuOptions = menuOptions
                .Where(option => rbac.HasPermission(this, option.Key)) // Check if the user has the required permission
                .ToDictionary(option => option.Key, option => option.Value);

            // Pass the filtered menu options to the DisplayMenu method
            DisplayMenu(filteredMenuOptions, rbac);
        }

        private void PlaceOrder(ILogger logger, OrderManager orderManager)
        {
            Console.Clear();
            if (!File.Exists("products.txt")) { Console.WriteLine("No products available!"); Console.ReadKey(); return; }
            Console.Write("Enter product name: "); var name = Console.ReadLine();
            Console.Write("Enter quantity: "); if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) { Console.WriteLine("Invalid quantity!"); Console.ReadKey(); return; }
            var products = File.ReadAllLines("products.txt").ToList();
            bool found = false;
            for (int i = 0; i < products.Count; i++)
            {
                var parts = products[i].Split(',');
                if (parts[0].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    int stock = int.TryParse(parts[1], out int s) ? s : 0;
                    if (stock >= qty)
                    {
                        decimal totalPrice = decimal.Parse(parts[2]) * qty;
                        if (MakePayment(totalPrice, logger) == false)
                        {
                            Console.Clear();
                            Console.WriteLine("Payment failed! Order not placed.");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Order placed successfully!");
                            logger.Log($"User {Username} ordered {qty}x {name}");
                            orderManager.LogOrder(Username, name, qty, totalPrice);
                            Console.WriteLine("Press any key to continue...");
                            parts[1] = (stock - qty).ToString();
                            products[i] = string.Join(",", parts);
                            File.WriteAllLines("products.txt", products);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not enough stock available!");
                        Console.WriteLine($"Available stock: {stock}");
                        Console.WriteLine($"Requested quantity: {qty}");
                        Console.WriteLine("Press any key to continue...");
                    }
                    found = true; break;
                }
            }
            if (!found) Console.WriteLine("Product not found!");
            Console.ReadKey();
        }
        private bool MakePayment(decimal orderPrice, ILogger logger)
        {
            Console.Clear();
            // First line - time remaining will be updated here
            Console.WriteLine($"Time remaining: 10:00");
            Console.WriteLine($"Order Price: {orderPrice:C}");
            Console.WriteLine("Choose payment method:");
            Console.WriteLine("1. BLIK");
            Console.WriteLine("2. Card");
            Console.WriteLine("0. Cancel");
            Console.Write("Your choice: ");

            DateTime startTime = DateTime.Now;
            string userInput = string.Empty;
            bool inPaymentProcess = false;
            int inputLine = Console.CursorTop; // Remember where input is being taken

            var timer = new System.Timers.Timer(1000); // Update every second
            timer.Elapsed += (sender, e) =>
            {
                if (inPaymentProcess) return;

                TimeSpan remainingTime = TimeSpan.FromMinutes(10) - (DateTime.Now - startTime);
                if (remainingTime.TotalSeconds <= 0)
                {
                    timer.Stop();
                    Console.Clear();
                    Console.WriteLine("Transaction timed out!");
                    logger.Log($"User {Username} failed to complete payment of {orderPrice:C} within the 10-minute limit.");
                    Environment.Exit(0);
                    return;
                }

                // Save current cursor position
                int currentLeft = Console.CursorLeft;
                int currentTop = Console.CursorTop;

                // Update only the time remaining line
                Console.SetCursorPosition(0, 0);
                Console.Write($"Time remaining: {remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}".PadRight(Console.WindowWidth));

                // Restore cursor position
                Console.SetCursorPosition(currentLeft, currentTop);
            };
            timer.Start();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (inPaymentProcess)
                {
                    // Let the payment process handle the input
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (!int.TryParse(userInput, out int choice) || choice < 0 || choice > 2)
                    {
                        Console.SetCursorPosition(0, inputLine + 1);
                        Console.WriteLine("Invalid input! Please enter 0, 1, or 2.".PadRight(Console.WindowWidth));
                        Console.SetCursorPosition("Your choice: ".Length, inputLine);
                        userInput = string.Empty;
                        continue;
                    }

                    if (choice == 0)
                    {
                        Console.WriteLine("\nPayment canceled.");
                        Console.ReadKey();
                        timer.Stop();
                        return false;
                    }

                    inPaymentProcess = true;
                    timer.Stop();

                    Random random = new Random();
                    bool paymentSuccess = random.Next(0, 100) >= 0.5; // 0,5% chance of failure

                    switch (choice)
                    {
                        case 1:
                            Console.SetCursorPosition(0, inputLine + 1);
                            Console.Write("Enter 6-digit BLIK code: ".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition("Enter 6-digit BLIK code: ".Length, inputLine + 1);
                            var blikCode = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(blikCode) || blikCode.Length != 6 || !int.TryParse(blikCode, out _))
                            {
                                Console.SetCursorPosition(0, inputLine + 2);
                                Console.WriteLine("Invalid BLIK code!".PadRight(Console.WindowWidth));
                                Console.ReadKey();
                                inPaymentProcess = false;
                                timer.Start();
                                // Clear the payment method input lines
                                Console.SetCursorPosition(0, inputLine + 1);
                                Console.WriteLine(new string(' ', Console.WindowWidth));
                                Console.WriteLine(new string(' ', Console.WindowWidth));
                                Console.SetCursorPosition("Your choice: ".Length, inputLine);
                                userInput = string.Empty;
                                continue;
                            }
                            break;

                        case 2:
                            Console.SetCursorPosition(0, inputLine + 1);
                            Console.Write("Enter card number (16 digits): ".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition("Enter card number (16 digits): ".Length, inputLine + 1);
                            var cardNumber = Console.ReadLine();
                            Console.SetCursorPosition(0, inputLine + 2);
                            Console.Write("Enter expiration date (MM/YY): ".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition("Enter expiration date (MM/YY): ".Length, inputLine + 2);
                            var expirationDate = Console.ReadLine();
                            Console.SetCursorPosition(0, inputLine + 3);
                            Console.Write("Enter CVV (3 digits): ".PadRight(Console.WindowWidth));
                            Console.SetCursorPosition("Enter CVV (3 digits): ".Length, inputLine + 3);
                            var cvv = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16 || !long.TryParse(cardNumber, out _) ||
                                string.IsNullOrWhiteSpace(expirationDate) || !expirationDate.Contains("/") ||
                                string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3 || !int.TryParse(cvv, out _))
                            {
                                Console.SetCursorPosition(0, inputLine + 4);
                                Console.WriteLine("Invalid card information!".PadRight(Console.WindowWidth));
                                Console.ReadKey();
                                inPaymentProcess = false;
                                timer.Start();
                                // Clear the payment method input lines
                                for (int i = 1; i <= 4; i++)
                                {
                                    Console.SetCursorPosition(0, inputLine + i);
                                    Console.WriteLine(new string(' ', Console.WindowWidth));
                                }
                                Console.SetCursorPosition("Your choice: ".Length, inputLine);
                                userInput = string.Empty;
                                continue;
                            }
                            break;
                    }

                    if (paymentSuccess)
                    {
                        Console.SetCursorPosition(0, inputLine + (choice == 1 ? 2 : 4));
                        Console.WriteLine("Payment successful!".PadRight(Console.WindowWidth));
                        logger.Log($"User {Username} completed payment of {orderPrice:C} using {(choice == 1 ? "BLIK" : "Card")}");
                        return true;
                    }
                    else
                    {
                        Console.SetCursorPosition(0, inputLine + (choice == 1 ? 2 : 4));
                        Console.WriteLine("Payment failed!".PadRight(Console.WindowWidth));
                        Console.ReadKey();
                        inPaymentProcess = false;
                        timer.Start();
                        // Clear the payment method input lines
                        for (int i = 1; i <= (choice == 1 ? 2 : 4); i++)
                        {
                            Console.SetCursorPosition(0, inputLine + i);
                            Console.WriteLine(new string(' ', Console.WindowWidth));
                        }
                        Console.SetCursorPosition("Your choice: ".Length, inputLine);
                        userInput = string.Empty;
                        continue;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace && userInput.Length > 0)
                {
                    userInput = userInput.Substring(0, userInput.Length - 1);
                    Console.Write("\b \b");
                }
                else if (char.IsDigit(key.KeyChar))
                {
                    userInput += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        private void RemoveUser(ILogger logger)
        {
            Console.Clear();
            Console.Write("Enter username to remove: ");
            var username = Console.ReadLine();

            if (!File.Exists("users.txt"))
            {
                Console.WriteLine("No users found!");
                Console.ReadKey();
                return;
            }

            var users = File.ReadAllLines("users.txt").ToList();
            var userToRemove = users.FirstOrDefault(u => u.StartsWith(username + ","));

            if (userToRemove != null && userToRemove != Username)
            {
                users.Remove(userToRemove);
                File.WriteAllLines("users.txt", users);
                Console.WriteLine("User removed successfully!");
                logger.Log($"User {Username} removed user {username}");
            }
            else if(userToRemove == null)
            {
                Console.WriteLine("User not found!");
            }
            else
            {
                Console.WriteLine("You cannot remove yourself!");
            }

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }

        private void ListUsers(ILogger logger)
        {
            Console.Clear();
            Console.WriteLine("LIST OF USERS:");
            if (!File.Exists("users.txt"))
            {
                Console.WriteLine("No users found!");
            }
            else
            {
                var users = File.ReadLines("users.txt").ToList();
                const int pageSize = 15;
                int currentPage = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"USERS - Page {currentPage + 1}/{(users.Count + pageSize - 1) / pageSize}");
                    foreach (var user in users.Skip(currentPage * pageSize).Take(pageSize))
                    {
                        Console.WriteLine(user);
                    }

                    Console.WriteLine("\nN - Next Page | P - Previous Page | Q - Quit");
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.N && (currentPage + 1) * pageSize < users.Count)
                        currentPage++;
                    else if (input == ConsoleKey.P && currentPage > 0)
                        currentPage--;
                    else if (input == ConsoleKey.Q)
                        break;
                }
            }
            logger.Log($"User {Username} viewed user list");
        }

        private void UpdateStock(ILogger logger)
        {
            Console.Clear();
            if (!File.Exists("products.txt")) { Console.WriteLine("No products available!"); Console.ReadKey(); return; }
            Console.Write("Enter product name: "); var name = Console.ReadLine();
            Console.Write("Enter new quantity: "); if (!int.TryParse(Console.ReadLine(), out int qty)) { Console.WriteLine("Invalid quantity!"); Console.ReadKey(); return; }
            var products = File.ReadAllLines("products.txt").ToList();
            bool found = false;
            for (int i = 0; i < products.Count; i++)
            {
                var parts = products[i].Split(',');
                if (parts[0].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    parts[1] = qty.ToString(); products[i] = string.Join(",", parts); found = true; break;
                }
            }
            if (found)
            {
                File.WriteAllLines("products.txt", products);
                Console.WriteLine("Stock updated successfully!");
                logger.Log($"User {Username} updated stock for {name}");
            }
            else Console.WriteLine("Product not found!");
            Console.ReadKey();
        }

        private void ViewLogs(ILogger logger)
        {
            Console.Clear();
            Console.WriteLine("SYSTEM LOGS:");
            FileLogger fileLogger = (FileLogger)logger;
            if (fileLogger == null || !File.Exists("logs.txt"))
            {
                Console.WriteLine("No logs found!");
            }
            else
            {
                var logs = fileLogger.GetLogs().ToList();
                const int pageSize = 15;
                int currentPage = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"LOGS - Page {currentPage + 1}/{(logs.Count + pageSize - 1) / pageSize}");
                    foreach (var log in logs.Skip(currentPage * pageSize).Take(pageSize))
                    {
                        Console.WriteLine(log);
                    }

                    Console.WriteLine("\nN - Next Page | P - Previous Page | Q - Quit");
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.N && (currentPage + 1) * pageSize < logs.Count)
                        currentPage++;
                    else if (input == ConsoleKey.P && currentPage > 0)
                        currentPage--;
                    else if (input == ConsoleKey.Q)
                        break;
                }
            }
            logger.Log($"User {Username} viewed logs");
        }

        private void ViewProducts(ILogger logger)
        {
            Console.Clear();
            Console.WriteLine("PRODUCT LIST:");
            if (!File.Exists("products.txt"))
            {
                Console.WriteLine("No products found!");
            }
            else
            {
                var products = File.ReadLines("products.txt").ToList();
                const int pageSize = 15;
                int currentPage = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"PRODUCTS - Page {currentPage + 1}/{(products.Count + pageSize - 1) / pageSize}");
                    foreach (var product in products.Skip(currentPage * pageSize).Take(pageSize))
                    {
                        Console.WriteLine(product);
                    }

                    Console.WriteLine("\nN - Next Page | P - Previous Page | Q - Quit");
                    var input = Console.ReadKey(true).Key;
                    if (input == ConsoleKey.N && (currentPage + 1) * pageSize < products.Count)
                        currentPage++;
                    else if (input == ConsoleKey.P && currentPage > 0)
                        currentPage--;
                    else if (input == ConsoleKey.Q)
                        break;
                }
            }
            logger.Log($"User {Username} viewed products");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewProfile(ILogger logger)
        {
            Console.Clear();
            Console.WriteLine($"USER PROFILE - {Username}");
            Console.WriteLine($"Role: {Role}");
            Console.WriteLine($"Account created: {CreationDate:yyyy-MM-dd HH:mm:ss}");
            logger.Log($"User {Username} viewed profile");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
