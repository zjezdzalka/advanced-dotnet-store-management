using System;
using System.IO;
using projektOOP.Services;
using projektOOP.Utils;


namespace ProjektOOP
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method that starts the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        static void Main(string[] args)
        {
            // Initializes required files for the application.
            InitializeFiles();

            // Creates instances of core services and managers.
            var passwordManager = new PasswordManager();
            var logger = new FileLogger();
            var userManager = new UserManager();
            var rbac = new RBAC();
            var orderManager = new OrderManager();

            // Subscribes to user action events for logging.
            userManager.UserAction += (s, e) => logger.Log($"User action: {e.Username} - {e.Action}");

            // Main application loop for user interaction.
            while (true)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Choose option: ");

                // Validates user input and processes menu options.
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input!");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        // Handles user login and displays user-specific menu.
                        var user = userManager.Login(passwordManager, logger);
                        if (user != null) user.ShowMenu(passwordManager, logger, rbac, orderManager);
                        break;
                    case 2:
                        // Handles user registration.
                        userManager.Register(passwordManager, logger);
                        break;
                    case 3:
                        // Exits the application.
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes required files for the application if they do not already exist.
        /// </summary>
        static void InitializeFiles()
        {
            // Creates a default users file with predefined users if it does not exist.
            if (!File.Exists("users.txt"))
            {
                var passwordManager = new PasswordManager();
                var pwdAdmin = "123";
                var pwdManager = "1234";
                var pwdUser = "12345";

                var hash = passwordManager.HashPassword(pwdAdmin);
                File.WriteAllText("users.txt", $"admin,{hash},Administrator,{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

                hash = passwordManager.HashPassword(pwdManager);
                File.AppendAllText("users.txt", $"manager,{hash},Manager,{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

                hash = passwordManager.HashPassword(pwdUser);
                File.AppendAllText("users.txt", $"user,{hash},User,{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
            }

            // Creates a default products file with predefined products if it does not exist.
            if (!File.Exists("products.txt"))
            {
                File.WriteAllText("products.txt",
                    "Laptop,10,1200\n" +
                    "Phone,15,800\n" +
                    "Tablet,8,400");
            }

            // Creates an empty logs file if it does not exist.
            if (!File.Exists("logs.txt")) File.WriteAllText("logs.txt", "");
        }
    }
}