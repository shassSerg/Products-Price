using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductsPrice
{
    //Базовый класс юнита
    [Serializable]
    [XmlType(TypeName = "unit")]
    public class Unit
    {
        //конструктор
        public Unit(int price)
        {
            Price = price;
            Count = 1;
        }
        //Конструктор PackUnit
        public Unit(int price, int count)
        {
            Price = price;
            Count = count;
        }
        public Unit()
        {
            Count = 1;
            Price = 0;
        }



        //цена
        private int price;
        [XmlAttribute(AttributeName = "price", DataType = "int")]
        public int Price
        {
            set
            {
                if (value >= 0)
                    this.price = value;
                else throw new ArgumentException("Price should be more than zero.");
            }
            get
            {
                return this.price;
            }
        }


        //кол-во товара
        //Кол-во
        private int count;
        [XmlAttribute(AttributeName = "size", DataType = "int")]
        public int Count
        {
            get
            {
                return this.count;
            }

            set
            {
                if (value > 0)
                    this.count = value;
                else throw new ArgumentException("Count should be more than zero.");
            }
        }


        //переопределение сравнения
        public override bool Equals(object obj)
        {
            return Equals(obj as Unit);
        }
        public bool Equals(Unit unit)
        {
            return unit != null && unit.count == this.count
                && this.price == unit.price;
        }

        //переопределение hashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Кол-во {Count} по цене {Price}";
        }
    }
}
