using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ProductsPrice
{
    //класс кассы
    public class Cashier
    {
        private static Cashier instance = new Cashier();

        //конструктор
        static Cashier()
        {
            try
            {
                path = "products.xml";
                path_log = "log.txt";
                products=LoadProducts(path);
            }
            catch { }
        }
        public static Cashier GetInstance()
        {
            return instance;
        }

        //default path to file .xml
        public static string path;

        //default path to file .txt
        public static string path_log;
 
        //продукты
        public static List<Product> products = new List<Product>();
        public static Product GetProduct(string product)
        {
            return products?.Find(a => a.Equals(product));
        }

        public static string Purchase(string purchase)
        {
            //удаление разделителей
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ |\t]{1,}", options);
            purchase = regex.Replace(purchase, string.Empty);

            //нахождение покупок
            var productsNamePurchase =
                purchase
                //группировка по товарам
                .GroupBy(b => b)
                //проверка на наличие в продуктах
                .Where(b => GetProduct(b.Key.ToString()) != null)
                //подчет кол-ва и цены
                .Select(product => new
                {
                    //имя продукта
                    NameProduct = product.Key.ToString(),
                    //цена
                    FPrice = GetProduct(product.Key.ToString())?.GetUnits(product.Count())
                    .Sum(u=>u.Price)??0,
                    //цена
                    FSize = GetProduct(product.Key.ToString())?.GetUnits(product.Count()).Count??0,
                    //подходящие скидки по товару
                    discounts = GetProduct(product.Key.ToString())?.discounts
                     //подходящие скидки
                     .Where(
                         discount => (discount.TypeDiscount == Discount.Type.summ ?
                         (discount.Target <= (GetProduct(product.Key.ToString())?.GetUnits(product.Count()).Sum(u => u.Price)??0)) :
                         (discount.Target <= (GetProduct(product.Key.ToString())?.GetUnits(product.Count()).Count ?? 0)))
                     )
                     //группировка по приоритету
                     .GroupBy(discount => discount.Priority)
                     //сортировка по приоритету, сначала меньший
                     .OrderBy(discount => discount.Key)
                     //сортировка скидок, сначала наименьшая
                     .Select(discount => new
                     {
                         Discounts = discount
                         //создание скидки с продуктом и сортировка
                         .Select(_d => new
                         {
                             //сортировка скидок, сначала наименьшая
                             NameProduct = product.Key.ToString(),
                             discount = _d
                         }).OrderBy(d => d.discount.ValueDiscount)
                     })
                     //объеденение скидок по товару
                     .SelectMany(discount => discount.Discounts)
                });
                /*с учетом разделителя
                //получение пакетов
                purhcase.Split().
                Select(p => new
                {
                    products =
                    //группировка по товарам
                    p.GroupBy(b => b)
                    //проверка на наличие в продуктах
                    .Where(b => GetProduct(b.Key.ToString()) != null)
                    //подчет кол-ва и цены
                    .Select(product => new
                    {
                        //имя продукта
                        NameProduct = product.Key.ToString(),
                        //кол-во
                        Size = GetProduct(product.Key.ToString())?.GetUnit(product.Count()) != null ? 1 : product.Count(),
                        //цена
                        Price = (GetProduct(product.Key.ToString())?.GetUnit(product.Count())?.Price
                            ?? (GetProduct(product.Key.ToString())?.GetUnit(1)?.Price * product.Count() ?? 0))
                    })
                })
                //соеденение всех пакетов
                .SelectMany(p => p.products)
                //группировка по товарам
                .GroupBy(p => p.NameProduct)
                //суммирование цены и кол-ва по товарам
                .Select(p => new
                {
                    //имя продукта
                    NameProduct = p.Key,
                    //общее кол-во
                    FSize = p.Sum(u => u.Size),
                    //общая цена
                    FPrice = p.Sum(u => u.Price),
                    //подходящие скидки по товару
                    discounts = GetProduct(p.Key.ToString()).discounts
                     //подходящие скидки
                     .Where(
                         discount => (discount.TypeDiscount == Discount.Type.count?
                         (discount.Target <= p.Sum(u => u.Size)) :
                         (discount.Target <= p.Sum(u => u.Price)))
                     )
                     //группировка по приоритету
                     .GroupBy(discount => discount.Priority)
                     //сортировка по приоритету, сначала меньший
                     .OrderBy(discount => discount.Key)
                     //сортировка скидок, сначала наименьшая
                     .Select(discount => new
                     {
                         Discounts = discount
                         //создание скидки с продуктом и сортировка
                         .Select(_d => new
                         {
                                NameProduct=p.Key,
                                discount=_d
                         }).OrderBy(d=>d.discount.ValueDiscount)
                     })
                     //объеденение скидок по товару
                     .SelectMany(discount => discount.Discounts)
                     });*/

            //вычисление итоговой цены
            int full_price = productsNamePurchase?.Sum(p => p.FPrice)??0;

            //LOG вычисления скидки
            string discounts="";


            //нахождение итоговой скидки покупки
            var _discounts = productsNamePurchase
                //объеденение всех скидок
                .SelectMany(p => p.discounts)
                //группировка всех скидок по приоритету
                .GroupBy(p => p.discount.Priority)
                //сортировка группированных скидок, сначала ниаменьшая
                .OrderBy(discount => discount.Key)
                //сортировка скидок, сначала наименьшая
                .Select(discount => new
                {
                    discount = discount.OrderBy(d=>d.discount.ValueDiscount)
                });


            //логирование вычисления скидки
            discounts += $"Вычисление скидки для покупки: {purchase}" + Environment.NewLine;
            foreach (var product in productsNamePurchase)
            {
                    discounts += $"Товар {product.NameProduct}" +
                    $" с общей ценой {product.FPrice}р. и общим кол-ом {product.FSize}шт. " +
                    $"покупки  с доступными скидками (по приоритету):" + Environment.NewLine;

                    if (product.discounts != null && product.discounts.Count()>0)
                    {
                        foreach (var d in product.discounts)
                                discounts += "\t" + d.discount + Environment.NewLine;

                        discounts += "\tСкидки с наивысшим приоритетом и наименьшим значением скидки:" + Environment.NewLine;

                        discounts += "\t\t" + product.discounts.First().discount + Environment.NewLine;

                        discounts += "\t\tИтоговая скидка:" + Environment.NewLine;
                        discounts += "\t\t\t" + product.discounts.First().discount + Environment.NewLine;
                    }
                    else discounts += "\tСкидок нет" + Environment.NewLine;
            }

            discounts += $"Итоговые скидки покупки: {purchase}" + Environment.NewLine;

            //для вычисленной скидки
            Discount _dis = null;
            string _product = null;

            if (_discounts!=null && _discounts.Count() > 0)
            {
                foreach (var d in _discounts)
                    foreach (var _d in d.discount)
                        discounts += "\t" + _d.discount+$" товар {_d.NameProduct}" + Environment.NewLine;

                discounts += "\tСкидки с наивысшим приоритетом и наименьшим значением скидки:" + Environment.NewLine;

                foreach (var d in _discounts.First().discount)
                    discounts += "\t\t" + d.discount+$" товар {d.NameProduct}" + Environment.NewLine;

                discounts += "\t\tИтоговая скидка покупки:" + Environment.NewLine;
                discounts += "\t\t\t" + 
                    _discounts.First().discount.First().discount
                    + $" товар {_discounts.First().discount.First().NameProduct}" + Environment.NewLine;

                _dis = _discounts.First().discount.First().discount;
                _product = _discounts.First().discount.First().NameProduct;
                discounts += $"Покупка: {purchase} " +
                    $"с полной ценой {full_price}р. и " +
                    $"скидка: ({(_dis?.ToString() ?? "Скидки нет")}) " +
                    $"товар {_discounts.First().discount.First().NameProduct}" + Environment.NewLine;
            }
            else discounts += "\t" + "Скидок нет" + Environment.NewLine;

            discounts += Environment.NewLine;
            //запись лога вычисления скидки в файл
            System.IO.File.AppendAllText(path_log, discounts);

            //вычисление цены после скидки
            float price_after_discount = full_price;
            if ((_dis?.ValueDiscount ?? 0) > 0 && productsNamePurchase!=null)
                price_after_discount = full_price - (float)(productsNamePurchase?.First(a => a.NameProduct == _product).FPrice ?? 0) / 100 * _dis.ValueDiscount;

            return $"Покупка: {purchase} - {full_price}р. " +
                    $"({(_dis?.ToString() ?? "Скидки нет")}" +
                    (_dis != null ? $" Товар {_product}":string.Empty)+
                    $") = {price_after_discount}р.";
        }

        //сохранить кассу
        public static void SaveProducts(string pathFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>),new XmlRootAttribute("products"));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);
            using (StreamWriter writer = new StreamWriter(pathFile))
            {
                serializer.Serialize(writer, products, xmlSerializerNamespaces);
            }
        }

        //загрузить кассу
        public static List<Product> LoadProducts(string pathFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("products"));
            using (Stream writer = new FileStream(pathFile, FileMode.Open))
            {
                return serializer.Deserialize(writer) as List<Product>;
            }
        }

        ~Cashier()
        {
            try
            {
                SaveProducts(path);
            }
            catch { }
        }
    }
}
