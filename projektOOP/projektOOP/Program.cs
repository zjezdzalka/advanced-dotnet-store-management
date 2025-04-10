using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
    class RBAC
    {
        private readonly Dictionary<Role, List<string>> _rolePermissions;
        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<string>>
            {
                {Role.Admin, new List<string>{"Read","Write","Delete"}},
                {Role.Manager, new List<string>{"Read","Write"}},
                {Role.User, new List<string>{"Read"}}
            };
        }
        public bool HasPermission(User user, string permission)
        {
            if (_rolePermissions.ContainsKey(user.Role) && _rolePermissions[user.Role].Contains(permission))
            {
                return true;
            }
            return false;
        }
    }
    class Product
    {
        public int ID;
        private static int counter = 1;
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public Product(string name, string description, string category)
        {
            ID = counter++;
            Name = name;
            Description = description;
            Category = category;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"ID: {ID}");
        }
    }
    class PasswordManager
    {
        public bool VerifyPassword(string name, string password)
        {
            foreach (var line in File.ReadAllLines("users.txt"))
            {
                if(line.StartsWith(name) && line.Contains(HashPassword(password)))
                {
                    return true;
                }
            }
            return false;
        }
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
    class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }

    }
    class Order
    {
        public int OrderID { get; set; }
        private static int counter = 1;
        public Order() 
        { 
            OrderID = counter++;
        }
        public void ShowOrderDetails()
        {
            Console.WriteLine($"Id: {OrderID}");
        }
    }
    class ShoppingCart
    {
        public Dictionary<Product, int> ShoppingCartContent { get; set; }
        public ShoppingCart()
        {
            ShoppingCartContent = new Dictionary<Product, int>();
        }
        
    }
    class PaymentProcessor
    {
        public delegate void PayVia();
        public event PayVia OnPay;

        public void PaymentMethod(IPayment paymentMethod)
        {
            if(OnPay != null && OnPay.GetInvocationList().Length > 0)
            {
                foreach (var method in OnPay.GetInvocationList())
                {
                    OnPay -= (PayVia)method;
                }
            }
            OnPay += paymentMethod.Pay;
            Console.WriteLine($"You chose: {paymentMethod.Name}");
        }
    }
    class BlikPayment : IPayment
    {
        public string Name { get; set; }
        public BlikPayment()
        {
            Name = "Blik payment";
        }
        public void Pay()
        {

        }
    }
    class CardPayment : IPayment
    {
        public string Name { get; set; }
        public CardPayment()
        {
            Name = "Card payment";
        }
        public void Pay()
        {

        }
    }
    internal class Program
    {
        public static void Login(PasswordManager passwordManager)
        {
            bool loginStatus;
            do
            {
                System.Threading.Thread.Sleep(1500);
                Console.Clear();
                Console.Write("Enter login: ");
                string login = Console.ReadLine();
                Console.Write("Enter password: ");
                string password = Console.ReadLine();
                loginStatus = passwordManager.VerifyPassword(login, password);
                Console.WriteLine(loginStatus ? "Login successful" : "Login failed. Try again");
            } while (!loginStatus);
            
            
        }
        public static void Register(PasswordManager passwordManager)
        {
            bool registerStatus = false;
            registerLoop:
            do
            {
                System.Threading.Thread.Sleep(1500);
                Console.Clear();
                Console.Write("Enter login: ");
                string login = Console.ReadLine();
                Console.Write("Enter password: ");
                string password1 = Console.ReadLine();
                Console.Write("Repeat password: ");
                string password2 = Console.ReadLine();
                if (password1 != password2)
                {
                    Console.WriteLine("Passwords are not the same");
                    continue;
                }
                foreach (var line in File.ReadAllLines("users.txt"))
                {
                    if (line.StartsWith(login))
                    {
                        Console.WriteLine("There is an account with the same login");
                        goto registerLoop;
                    }
                }
                File.AppendAllText("users.txt", $"{login} {passwordManager.HashPassword(password1)}\n");
                Console.WriteLine("Account created");
                registerStatus = true;
            }while(!registerStatus);
            
        }
        static void Main(string[] args)
        {
            /*
            Product product1 = new Product("cos", "cos", "cos");
            Product product2 = new Product("cos", "cos", "cos");
            Product product3 = new Product("cos", "cos", "cos");
            Product product4 = new Product("cos", "cos", "cos");

            product1.ShowInfo();
            product2.ShowInfo();
            product3.ShowInfo();
            product4.ShowInfo();
            */
            PasswordManager passwordManager = new PasswordManager();
            BlikPayment blik = new BlikPayment();
            CardPayment card = new CardPayment();
            /*
            string password = "niger";
            File.AppendAllText("users.txt", $"marek {passwordManager.HashPassword(password)}");
            if (passwordManager.VerifyPassword("marek", "niger"))
            {
                Console.WriteLine("nigger");
            }*/
            string password = "niger";
            File.AppendAllText("users.txt", $"marek {passwordManager.HashPassword(password)}\n");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.Write("Choose option: ");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Wrong input");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
            }
            switch (choice)
            {
                case 1:
                    Program.Login(passwordManager);
                    break;
                case 2:
                    Program.Register(passwordManager);
                    Program.Login(passwordManager);
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
            while (true)
            {
                Console.WriteLine("nigger");
                break;
            }
        }
    }
}