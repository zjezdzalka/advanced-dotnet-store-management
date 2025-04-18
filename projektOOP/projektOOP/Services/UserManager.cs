using System;
using System.Linq;
using System.IO;
using projektOOP.Interfaces;
using projektOOP.Classes;
using projektOOP.Enums;
using projektOOP.Utils;

namespace projektOOP.Services
{
    public class UserManager
    {
        public event UserActionEventHandler UserAction;
        private readonly RBAC _rbac = new RBAC();

        public User Login(PasswordManager passwordManager, ILogger logger)
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                Console.Clear();
                Console.Write("Username: ");
                var username = Console.ReadLine();
                Console.Write("Password: ");
                var password = Console.ReadLine();
                if (passwordManager.VerifyPassword(username, password))
                {
                    var user = LoadUser(username);
                    if (user != null)
                    {
                        OnUserAction(username, "logged in");
                        logger.Log($"Successful login for {username}");
                        return user;
                    }
                }
                attempts++;
                Console.WriteLine($"Invalid credentials! Attempts left: {maxAttempts - attempts}");
                logger.Log($"Failed login attempt for {username}");
                if (attempts != maxAttempts) Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            Console.WriteLine("Too many failed attempts!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        public void Register(PasswordManager passwordManager, ILogger logger)
        {
            Console.Clear();
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var pwd = Console.ReadLine();
            Console.Write("Repeat password: ");
            var confirm = Console.ReadLine();
            if (pwd != confirm)
            {
                Console.WriteLine("Passwords don't match!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            if (UserExists(username))
            {
                Console.WriteLine("Username already exists!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            string hash = passwordManager.HashPassword(pwd);
            File.AppendAllText("users.txt", $"{username},{hash},User,{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
            Console.WriteLine("Registration successful!");
            OnUserAction(username, "registered");
            logger.Log($"New user registered: {username}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private bool UserExists(string username)
        {
            if (!File.Exists("users.txt")) return false;
            return File.ReadLines("users.txt").Any(l => l.Split(',')[0] == username);
        }

        private User LoadUser(string username)
        {
            if (!File.Exists("users.txt")) return null;
            foreach (var line in File.ReadLines("users.txt"))
            {
                var parts = line.Split(',');
                if (parts[0] != username) continue;

                var user = new User
                {
                    Username = parts[0],
                    HashedPassword = parts[1],
                    Role = Enum.TryParse<Role>(parts[2], out var r) ? r : Role.User,
                    CreationDate = DateTime.TryParse(parts[3], out var dt) ? dt : DateTime.Now
                };

                _rbac.ApplyPermissions(user);

                return user;
            }
            return null;
        }

        protected virtual void OnUserAction(string username, string action)
        {
            UserAction?.Invoke(this, new UserActionEventArgs(username, action));
        }
    }
}
