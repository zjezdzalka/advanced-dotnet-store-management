using System;
using System.IO;
using System.Security.Cryptography;

namespace projektOOP.Services
{
    /// <summary>
    /// Provides functionality for hashing passwords and verifying user credentials.
    /// </summary>
    public class PasswordManager
    {
        /// <summary>
        /// Hashes a plain text password using the SHA-256 algorithm.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>A Base64-encoded string representing the hashed password.</returns>
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifies if the provided username and password match the stored credentials.
        /// </summary>
        /// <param name="username">The username to verify.</param>
        /// <param name="password">The plain text password to verify.</param>
        /// <returns>
        /// True if the username exists and the hashed password matches the stored hash; otherwise, false.
        /// </returns>
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
