using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Services
{
    public class OrderManager
    {
        private const string OrdersFile = "orders.txt";

        public void LogOrder(string username, string productName, int quantity, decimal totalPrice)
        {
            try
            {
                string orderEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{username},{productName},{quantity},{totalPrice:C}";
                File.AppendAllText(OrdersFile, orderEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging order: {ex.Message}");
            }
        }

        public IEnumerable<string> GetOrders()
        {
            if (!File.Exists(OrdersFile)) return Enumerable.Empty<string>();
            return File.ReadLines(OrdersFile).OrderByDescending(line =>
            {
                var parts = line.Split(',');
                return DateTime.TryParse(parts[0], out var dt) ? dt : DateTime.MinValue;
            });
        }
    }
}
