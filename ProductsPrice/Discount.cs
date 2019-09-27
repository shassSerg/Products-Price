using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductsPrice
{
    //Базовый класс скидки
    [Serializable]
    [XmlType(TypeName  = "discount")]
    public class Discount
    {
        //конструктор
        public Discount(Type type, int target, int discount, int priority)
        {
            TypeDiscount = type;
            Target = target;
            Priority = priority;
            ValueDiscount = discount;
        }
        public Discount()
        {
            TypeDiscount = Type.count;
            Target = 0;
            Priority = 0;
            ValueDiscount = 0;
        }

        //тип скидки
        public enum Type
        {
            summ,
            count
        }
        private Type type_discount;
        [XmlAttribute(AttributeName = "type")]
        public Type TypeDiscount
        {
            set
            {
                type_discount = value;
            }
            get
            {
                return this.type_discount;
            }
        }

        //приоритет скидки
        private int priority;
        [XmlAttribute(AttributeName = "priority", DataType = "int")]
        public int Priority
        {
            set
            {
                priority = value;
            }
            get
            {
                return this.priority;
            }
        }

        //цель скидки
        private int target;
        [XmlAttribute(AttributeName = "target", DataType = "int")]
        public int Target
        {
            set
            {
                target = value;
            }
            get
            {
                return this.target;
            }
        }

        //значение скидки в процентах
        private int value_discount;
        [XmlAttribute(AttributeName = "value", DataType = "int")]
        public int ValueDiscount
        {
            set
            {
                value_discount = value;
            }
            get
            {
                return this.value_discount;
            }
        }

        //переопределение сравнения
        public override bool Equals(object obj)
        {
            return Equals(obj as Discount);
        }
        public bool Equals(Discount discount)
        {
            return discount != null && discount.target == this.target
                && discount.type_discount == this.type_discount
                && discount.priority == this.priority
                && discount.value_discount == this.value_discount;
        }

        //можно ли получить скидку
        public bool CanDiscount(int value)
        {
            switch (TypeDiscount)
            {
                case Type.count:
                    return value >= Target;
                case Type.summ:
                    return value >= Target;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //переопределение ToString()
        public override string ToString()
        {
            return $"Скидка {ValueDiscount}% на "
                + (TypeDiscount == Type.count ? "количество " : "сумму ") 
                + $"более {Target}" + 
                (TypeDiscount == Type.count ? "шт. " : "р. ")+
                $"с приоритетом {Priority}";
        }
    }
}
