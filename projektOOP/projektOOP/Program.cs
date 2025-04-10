using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static void Login(PasswordManager passwordManager)
        {
            bool loginStatus;
            do
            {
                Thread.Sleep(1500);
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
            while (true)
            {
                Console.Clear();
                Console.Write("Enter login: ");
                string login = Console.ReadLine()?.Trim();
                Console.Write("Enter password: ");
                string password1 = Console.ReadLine();
                Console.Write("Repeat password: ");
                string password2 = Console.ReadLine();

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
            showMenu(passwordManager);
            while (true)
            {
                Console.WriteLine("nigger");
                break;
            }
        }

        private static void showMenu(PasswordManager passwordManager)
        {
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
                    Login(passwordManager);
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
}