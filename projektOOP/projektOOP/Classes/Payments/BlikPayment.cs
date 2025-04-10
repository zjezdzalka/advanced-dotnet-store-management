using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class BlikPayment : IPayment
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
}
