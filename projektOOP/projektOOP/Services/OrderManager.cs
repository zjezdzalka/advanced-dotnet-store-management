using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace projektOOP.Services
{
    /// <summary>  
    /// The OrderManager class provides functionality to log and retrieve orders.  
    /// Orders are stored in a text file and can be retrieved in descending order of their timestamps.  
    /// </summary>  
    public class OrderManager
    {
        /// <summary>  
        /// The name of the file where orders are stored.  
        /// </summary>  
        private const string OrdersFile = "orders.txt";

        /// <summary>  
        /// Logs an order to the orders file.  
        /// </summary>  
        /// <param name="username">The username of the customer placing the order.</param>  
        /// <param name="productName">The name of the product being ordered.</param>  
        /// <param name="quantity">The quantity of the product being ordered.</param>  
        /// <param name="totalPrice">The total price of the order.</param>  
        public void LogOrder(string username, string productName, int quantity, decimal totalPrice)
        {
            try
            {
                // Create a formatted string for the order entry.  
                string orderEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{username},{productName},{quantity},{totalPrice:C}";

                // Append the order entry to the orders file.  
                File.AppendAllText(OrdersFile, orderEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Log an error message if an exception occurs.  
                Console.WriteLine($"Error logging order: {ex.Message}");
            }
        }

        /// <summary>  
        /// Retrieves all logged orders from the orders file.  
        /// </summary>  
        /// <returns>A collection of order entries sorted in descending order of their timestamps.</returns>  
        public IEnumerable<string> GetOrders()
        {
            // Check if the orders file exists. If not, return an empty collection.  
            if (!File.Exists(OrdersFile)) return Enumerable.Empty<string>();

            // Read all lines from the orders file and sort them by timestamp in descending order.  
            return File.ReadLines(OrdersFile).OrderByDescending(line =>
            {
                var parts = line.Split(',');
                return DateTime.TryParse(parts[0], out var dt) ? dt : DateTime.MinValue;
            });
        }
    }
}
