using System;
using System.Linq;
using System.IO;
using projektOOP.Interfaces;
using projektOOP.Classes;
using projektOOP.Enums;
using projektOOP.Utils;

namespace projektOOP.Services
{
    /// <summary>
    /// Manages user-related operations such as login, registration, and user loading.
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Event triggered when a user performs an action (e.g., login, registration).
        /// </summary>
        public event UserActionEventHandler UserAction;

        /// <summary>
        /// Role-Based Access Control (RBAC) instance for managing user permissions.
        /// </summary>
        private readonly RBAC _rbac = new RBAC();

        /// <summary>
        /// Handles user login by verifying credentials and loading the user.
        /// </summary>
        /// <param name="passwordManager">Instance of PasswordManager for password verification.</param>
        /// <param name="logger">Instance of ILogger for logging actions.</param>
        /// <returns>
        /// Returns a <see cref="User"/> object if login is successful; otherwise, returns null.
        /// </returns>
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
                        logger.Log($"Successful login for {username}.");
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

        /// <summary>
        /// Handles user registration by creating a new user and saving their details.
        /// </summary>
        /// <param name="passwordManager">Instance of PasswordManager for password hashing.</param>
        /// <param name="logger">Instance of ILogger for logging actions.</param>
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

        /// <summary>
        /// Checks if a user with the specified username already exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>
        /// Returns true if the user exists; otherwise, false.
        /// </returns>
        private bool UserExists(string username)
        {
            if (!File.Exists("users.txt")) return false;
            return File.ReadLines("users.txt").Any(l => l.Split(',')[0] == username);
        }

        /// <summary>
        /// Loads a user by their username from the user data file.
        /// </summary>
        /// <param name="username">The username of the user to load.</param>
        /// <returns>
        /// Returns a <see cref="User"/> object if the user is found; otherwise, null.
        /// </returns>
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

        /// <summary>
        /// Triggers the <see cref="UserAction"/> event with the specified username and action.
        /// </summary>
        /// <param name="username">The username of the user performing the action.</param>
        /// <param name="action">The action performed by the user.</param>
        protected virtual void OnUserAction(string username, string action)
        {
            UserAction?.Invoke(this, new UserActionEventArgs(username, action));
        }
    }
}
