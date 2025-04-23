using System;
using System.IO;
using projektOOP.Services;
using projektOOP.Utils;


namespace ProjektOOP
{
    class Program
    {

        static void Main(string[] args)
        {
            InitializeFiles();
            var passwordManager = new PasswordManager();
            var logger = new FileLogger();
            var userManager = new UserManager();
            var rbac = new RBAC();
            var orderManager = new OrderManager();
            userManager.UserAction += (s, e) => logger.Log($"User action: {e.Username} - {e.Action}");

            while (true)
            {
                Console.Clear();
                Console.WriteLine("MAIN MENU");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Choose option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input!");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        var user = userManager.Login(passwordManager, logger);
                        if (user != null) user.ShowMenu(passwordManager, logger, rbac, orderManager);
                        break;
                    case 2:
                        userManager.Register(passwordManager, logger);
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static void InitializeFiles()
        {
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
            if (!File.Exists("products.txt"))
            {
                File.WriteAllText("products.txt",
                    "Laptop,10,1200\n" +
                    "Phone,15,800\n" +
                    "Tablet,8,400");
            }
            if (!File.Exists("logs.txt")) File.WriteAllText("logs.txt", "");
        }
    }
}