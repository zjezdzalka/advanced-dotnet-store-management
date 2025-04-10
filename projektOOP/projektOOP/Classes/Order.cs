using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class Order
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
}
