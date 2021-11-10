using System;
using PazarYeri.BusinessLayer.GittiGidiyor;
using PazarYeri.BusinessLayer.HepsiBurada;
using PazarYeri.BusinessLayer.Koctas;
using PazarYeri.BusinessLayer.N11;
using PazarYeri.BusinessLayer.Trendyol;
using PazarYeri.BusinessLayer.Utility;

namespace PazarYeri.Test
{
    class Program
    {
        private static ProjectUtil util = new ProjectUtil();

        static void Main(string[] args)
        {

            //util.LogService("HepsiBurada Siparişleri Çekiliyor");
            //var hepsiburada = new HepsiBuradaLayer();
            //hepsiburada.GetHepsiBuradaOrdersFromRestService(DateTime.Today.AddDays(-10), DateTime.Today);
            //util.LogService("HepsiBurada Siparişleri Tamamlandı");


            util.LogService("Trendyol Siparişleri Çekiliyor");
            var trendyol = new TrendyolLayer();
            trendyol.getOrders(DateTime.Today.AddDays(-10), DateTime.Today, "created");
            util.LogService("Trendyol Siparişleri Tamamlandı");


            //    util.LogService("N11 Siparişleri Çekiliyor");
            //    var n11 = new N11Layer();
            //    n11.getOrders(DateTime.Today.AddDays(-15), DateTime.Today);
            //    util.LogService("N11 Siparişleri Tamamlandı");

            //    util.LogService("Koçtaş Siparişleri Çekiliyor");
            //    var koctas = new KoctasLayer();
            //    koctas.getOrders();
            //    util.LogService("Koçtaş Siparişleri Tamamlandı");

            //    util.LogService("Hepsi Burada Siparişleri Çekiliyor");
            //    var hepsiBurada = new HepsiBuradaLayer();
            //    hepsiBurada.GetHepsiBuradaOrders();
            //    util.LogService("Hepsi Burada Siparişleri Tamamlandı");

            //    util.LogService("Gitti Gidiyor Siparişleri Çekiliyor");
            //    var gittiGidiyor = new GittiGidiyorLayer();
            //    gittiGidiyor.getOrders();
            //    util.LogService("Gitti Gidiyor Siparişleri Tamamlandı");
            //}
        }
    }
}