using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Services
{
    public class PasswordManager
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string username, string password)
        {
            if (!File.Exists("users.txt")) return false;
            foreach (var line in File.ReadLines("users.txt"))
            {
                var parts = line.Split(',');
                if (parts.Length >= 2 && parts[0] == username)
                {
                    string storedHash = parts[1];
                    string inputHash = HashPassword(password);
                    return storedHash == inputHash;
                }
            }
            return false;
        }
    }

}
