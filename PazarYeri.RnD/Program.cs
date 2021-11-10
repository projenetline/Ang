using PazarYeri.BusinessLayer.N11;
using PazarYeri.BusinessLayer.Koctas;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.BusinessLayer.Trendyol;
using PazarYeri.BusinessLayer.HepsiBurada;
using PazarYeri.BusinessLayer.GittiGidiyor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using PazarYeri.Models;

namespace PazarYeri.RnD
{
    class Program
    {
        private readonly ProjectUtil _util;

        private static DatabaseLayer _databaseLayer;
        private static TrendyolLayer _trendyolLayer;
        private static N11Layer _n11Layer;
        private static KoctasLayer _koctasLayer;
        private static GittiGidiyorLayer _gittiGidiyorLayer;
        private static HepsiBuradaLayer _hepsiBuradaLayer;

        static void Main(string[] args)
        {
            //_gittiGidiyorLayer = new GittiGidiyorLayer();
            //_gittiGidiyorLayer.getOrders();



            //_trendyolLayer = new TrendyolLayer();
            //_trendyolLayer.getOrders(new System.DateTime(2020, 3, 25), new System.DateTime(2020, 3, 26));
            var sifre = Crypter.Encrypt("Hb12345!", "*?netline?*-*");
            _hepsiBuradaLayer = new HepsiBuradaLayer();
            //_hepsiBuradaLayer.GetHepsiBuradaOrders();
            _hepsiBuradaLayer.GetHepsiBuradaOrdersFromRestService(DateTime.Today.AddDays(-10), DateTime.Today);

           

           //new DateTime(2020, 10, 10), new DateTime(2020, 10, 12)

           //_koctasLayer = new KoctasLayer();
           //_koctasLayer.getOrders();
        }
    }
}
