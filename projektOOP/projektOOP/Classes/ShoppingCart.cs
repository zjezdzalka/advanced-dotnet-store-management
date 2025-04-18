using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class ShoppingCart
    {
        public Dictionary<Product, int> ShoppingCartContent { get; set; }
        public ShoppingCart()
        {
            ShoppingCartContent = new Dictionary<Product, int>();
        }

    }
}
