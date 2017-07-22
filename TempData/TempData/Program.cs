using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempData
{
    class Program
    {

        static readonly Random rnd = new Random();
        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            var range = to - from;

            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }

        static void Main(string[] args)
        {

            List<ProductMap> productMapList = new List<ProductMap>();
            productMapList.Add(new ProductMap() { Name = "Toz Seker", BrandList = new List<string>() { "Torku", "Dogus" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Pastane", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Tuz", BrandList = new List<string>() { "Billur", "Becel" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Pastane", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Pirinc", BrandList = new List<string>() { "Reis", "Tad" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Kup Seker", BrandList = new List<string>() { "Torku", "Dogus" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Pastane", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Siyah Cay", BrandList = new List<string>() { "Lipton", "Dogus" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Yesil Cay", BrandList = new List<string>() { "Lipton", "Dogus" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Un", BrandList = new List<string>() { "Soke", "Reis" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Pastane" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Deterjan", BrandList = new List<string>() { "Kosla", "Ariel", "ABC" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Temizlik Firmasi" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Camasir Suyu", BrandList = new List<string>() { "Ace", "Kosla", "Domestos", "Ariel", "ABC" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Temizlik Firmasi" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Dis Macunu", BrandList = new List<string>() { "Colgate", "Signal" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Su", BrandList = new List<string>() { "Erikli", "Sirma", "Pinar" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran", "Kafe" }, QuantityType = "Adet" });
            productMapList.Add(new ProductMap() { Name = "Ketcap", BrandList = new List<string>() { "Tad", "Heinz" }, KobiTypeList = new List<string>() { "Bakkal", "Mini Market", "Restoran" }, QuantityType = "Adet" });

            List<Kobi> kobiList = new MockKobi().GetList();




            List<ProductData> productList = new List<ProductData>();

            for (int i = 0; i < 1000000; i++)
            {
                int index = rnd.Next(0, productMapList.Count);


                string kobiRandomType = productMapList[index].KobiTypeList[new Random().Next(0, productMapList[index].KobiTypeList.Count)];

                //productMapList[index].BrandList[new Random().Next(0, productMapList[index].BrandList.Count - 1)]
                ProductData productData = new ProductData();

                List<Kobi> kobiSelList = kobiList.Where(x => x.KobiType == kobiRandomType).ToList();
               

                Kobi selectedKobi = kobiSelList[rnd.Next(0,kobiSelList.Count )];

                productData.Id = i + 1;
                productData.Name = productMapList[index].Name;
                productData.Brand = productMapList[index].BrandList[rnd.Next(0, productMapList[index].BrandList.Count)];
                productData.KobiType = kobiRandomType;
                productData.OrderDate = GetRandomDate(new DateTime(2017, 1, 1), new DateTime(2017, 7, 22));
                productData.Quantity = (int)Math.Round((double)(rnd.Next(10, 1000) / 10)) * 10;
                productData.QuantityType = productMapList[index].QuantityType;
                productData.WaitingDayCount = (int)Math.Round((decimal)(rnd.Next(0, 90) / 10)) * 10;
                productData.ZoneID = selectedKobi.ZoneId;
                productData.SmeId = selectedKobi.Id;
                productData.KobiName = selectedKobi.Name;

                productList.Add(productData);
            
            }


            List<int> zoneList = productList.GroupBy(x => x.ZoneID).Select(x => x.Key).ToList();
            List<ZoneView> zoneViewList = new List<ZoneView>();

            foreach (var item in zoneList)
            {
                IList<ProductData> zoneProductList = productList.Where(x => x.ZoneID == item).ToList();

                //string headerString = string.Empty;

                ZoneView zoneview = new ZoneView();
                zoneview.Id = item;

                zoneview.ProductIdHeaderList = new List<string>();

                foreach (var product in productMapList)
                {
                    zoneview.ProductIdHeaderList.Add(product.Name);
                }


                var groupedKobiList = zoneProductList.GroupBy(x => x.SmeId).ToList();

                zoneview.QuantityList = new Dictionary<int, Dictionary<string, int>>();

                foreach (var groupedKobi in groupedKobiList)
                {
                    var orderList = zoneProductList.Where(x => x.SmeId == groupedKobi.Key);

                    var prodTotalDictionary = new Dictionary<string, int>();

                    foreach (var header in zoneview.ProductIdHeaderList)
                    {
                        int cnt = orderList.Where(x => x.Name == header).Sum(y => y.Quantity);

                        int flag = cnt > 0 ? 1 : 0;

                        prodTotalDictionary.Add(header, flag);
                    }

                    zoneview.QuantityList.Add(groupedKobi.Key, prodTotalDictionary);

                }

                zoneViewList.Add(zoneview);
            }



           
            foreach (var zoneView in zoneViewList)
            {
                StringBuilder sb = new StringBuilder();

                string headerString = "KobiId;";

                foreach (var header in zoneView.ProductIdHeaderList)
                {
                    headerString += header + ";";
                }

                sb.AppendLine(headerString);

                foreach (var quantity in zoneView.QuantityList)
                {
                    string data = quantity.Key.ToString() + ";";

                    foreach (var value in quantity.Value)
                    {
                        data += value.Value.ToString() + ";";
                    }

                    sb.AppendLine(data);
                }

                //after your loop
                File.WriteAllText(@"C:\z\OrderDataZone_" + zoneView.Id + ".csv", sb.ToString());
            }




            return;
        }
    }


    public class ProductMap
    {
        public string Name { get; set; }

        public string ProductType { get; set; }

        public IList<string> BrandList { get; set; }

        public string QuantityType { get; set; }

        public IList<string> KobiTypeList { get; set; }
    }


    public class Kobi
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ZoneId { get; set; }

        public string KobiType { get; set; }
    }

    public class ProductData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public string QuantityType { get; set; }

        public string Brand { get; set; }

        public int WaitingDayCount { get; set; }

        public DateTime OrderDate { get; set; }

        public int ZoneID { get; set; }

        public string KobiType { get; set; }

        public int SmeId { get; set; }

        public string KobiName { get; set; }

    }

    public class ZoneView
    {
        public int Id { get; set; }

        public List<string> ProductIdHeaderList { get; set; }

        public Dictionary<int, Dictionary<string, int>> QuantityList { get; set; }


    }

}
