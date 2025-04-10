using System;

namespace projektOOP
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VAT { get; set; }
        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {UnitPrice:C}, VAT: {VAT:P}, Quantity: {Quantity}";
        }
    }
}