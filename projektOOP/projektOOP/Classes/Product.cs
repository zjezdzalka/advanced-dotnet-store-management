using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class Product
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
}
