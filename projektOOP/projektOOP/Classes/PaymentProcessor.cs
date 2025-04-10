using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class PaymentProcessor
    {
        public delegate void PayVia();
        public event PayVia OnPay;

        public void PaymentMethod(IPayment paymentMethod)
        {
            if (OnPay != null && OnPay.GetInvocationList().Length > 0)
            {
                foreach (var method in OnPay.GetInvocationList())
                {
                    OnPay -= (PayVia)method;
                }
            }
            OnPay += paymentMethod.Pay;
            Console.WriteLine($"You chose: {paymentMethod.Name}");
        }
    }
}
