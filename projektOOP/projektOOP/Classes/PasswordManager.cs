using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class PasswordManager
    {
        public bool VerifyPassword(string name, string password)
        {
            foreach (var line in File.ReadAllLines("users.txt"))
            {
                if (line.StartsWith(name) && line.Contains(HashPassword(password)))
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
}
