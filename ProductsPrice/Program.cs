using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductsPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            /*List<Product> products = new List<Product>();
            Product a = new Product("A");
            a.AddUnit(new Unit(5));
            a.AddUnit(new Unit(14, 3));
            a.AddUnit(new Unit(40, 10));
            a.AddDiscount(new Discount(Discount.Type.summ,30, 7, 1));
            a.AddDiscount(new Discount(Discount.Type.count, 10, 5, 1));
            a.AddDiscount(new Discount(Discount.Type.count, 5, 3, 2));
            a.AddDiscount(new Discount(Discount.Type.summ, 12, 2, 3));
            products.Add(a);
            Product b = new Product("B");
            b.AddUnit(new Unit(1));
            b.AddUnit(new Unit(4, 5));
            b.AddUnit(new Unit(6, 8));
            b.AddDiscount(new Discount(Discount.Type.count, 20, 7, 1));
            b.AddDiscount(new Discount(Discount.Type.summ, 10, 2, 2));
            b.AddDiscount(new Discount(Discount.Type.count, 10, 3, 2));
            b.AddDiscount(new Discount(Discount.Type.summ, 7, 1, 3));
            products.Add(b);
            Product c = new Product("C");
            c.AddUnit(new Unit(3));
            c.AddUnit(new Unit(5, 2));
            c.AddUnit(new Unit(11, 5));
            c.AddDiscount(new Discount(Discount.Type.summ, 30, 7, 1));
            c.AddDiscount(new Discount(Discount.Type.count, 20, 5, 1));
            c.AddDiscount(new Discount(Discount.Type.count, 6, 2, 2));
            c.AddDiscount(new Discount(Discount.Type.summ, 15, 3, 3));
            products.Add(c);

            Cashier.products = products;
            //Cashier.Save...*/

            while (true)
            {
                string line = Console.ReadLine();
                if (line.Equals("."))
                    break;
                Console.WriteLine(Cashier.Purchase(line));
            }
        }
    }
}
