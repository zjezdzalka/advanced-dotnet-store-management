using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class CardPayment : IPayment
    {
        public string Name { get; set; }
        public CardPayment()
        {
            Name = "Card payment";
        }
        public void Pay()
        {

        }
    }
}
