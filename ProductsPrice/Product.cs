using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductsPrice
{
    //Базовый класс продукта
    [Serializable]
    [XmlType(TypeName = "product")]
    public class Product
    {
        //конструктор
        public Product(string name)
        {
            ProductName = name;
        }
        public Product()
        {
            ProductName = Guid.NewGuid().ToString();
        }

        //код товара
        private string productname;
        [XmlAttribute(AttributeName = "name",DataType = "string")]
        public string ProductName
        {
            set
            {
                if (!String.IsNullOrEmpty(value))
                    this.productname = value;
                else throw new ArgumentException("Product should have a name.");
            }
            get
            {
                return this.productname;
            }
        }


        [XmlArray(ElementName = "units")]
        public List<Unit> units
        {
            set; get;
        }
        //добавление юнита
        public void AddUnit(Unit unit)
        {
            if (units == null)
                units = new List<Unit>();

            if (units.IndexOf(unit) >= 0)
                units[units.IndexOf(unit)] = unit;
            else units.Add(unit);
        }
        //удаление юнита
        public void RemoveUnit(Unit unit)
        {
            units?.Remove(unit);
        }
        //получение юнита по кол-ву
        public Unit GetUnit(int count)
        {
            return units?.Find(a => a.Count == count);
        }
        //получение юнитов по кол-ву
        public List<Unit> GetUnits(int count)
        {
            int cur_count = count;
            List<Unit> _units = new List<Unit>();
            foreach(var unit in units?.
                Where(u=>u.Count<=count).
                OrderByDescending(u=>u.Count))
            {
                while (cur_count >= unit.Count)
                {
                    _units.Add(unit);
                    cur_count -= unit.Count;
                }
            }
            //if (cur_count > 0 && _units!=null)
            //    _units.AddRange(GetUnits(cur_count));

            return _units;
        }


        [XmlArray("discounts")]
        public List<Discount> discounts
        {
            set; get;
        }
        //добавление скидки
        public void AddDiscount(Discount discount)
        {
            if (discounts == null)
                discounts = new List<Discount>();

            if (discounts.IndexOf(discount) >= 0)
                discounts[discounts.IndexOf(discount)] = discount;
            else discounts.Add(discount);
        }
        //удаление скидки
        public void RemoveDiscount(Discount discount)
        {
            discounts?.Remove(discount);
        }

        //переопределение сравнения
        public override bool Equals(object obj)
        {
            return Equals(obj as Product);
        }
        public bool Equals(Product product)
        {
            return product != null && product.ProductName.Equals(this.ProductName);
        }
        public bool Equals(string product)
        {
            return !string.IsNullOrEmpty(product) && product.Equals(this.ProductName);
        }

        //переопределение hashCode
        public override int GetHashCode()
        {
            return this.productname.GetHashCode();
        }

        //переопределение ToString()
        public override string ToString()
        {
            return $"Товар {ProductName}";
        }
    }
}
