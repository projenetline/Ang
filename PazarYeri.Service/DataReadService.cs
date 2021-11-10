using System;
using System.ServiceProcess;
using PazarYeri.BusinessLayer.N11;
using PazarYeri.BusinessLayer.Koctas;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.BusinessLayer.Helpers;
using PazarYeri.BusinessLayer.Trendyol;
using PazarYeri.BusinessLayer.HepsiBurada;
using PazarYeri.BusinessLayer.GittiGidiyor;
using System.Threading;

namespace PazarYeri.Service
{
    partial class DataReadService : ServiceBase
    {
        System.Timers.Timer timerForTransfer;

        private readonly ProjectUtil _util;

        private readonly DatabaseLayer _databaseLayer;
        private readonly TrendyolLayer _trendyolLayer;
        private readonly N11Layer _n11Layer;
        private readonly KoctasLayer _koctasLayer;
        private readonly GittiGidiyorLayer _gittiGidiyorLayer;
        private readonly HepsiBuradaLayer _hepsiBuradaLayer;

        public DataReadService()
        {
            InitializeComponent();

            _databaseLayer = new DatabaseLayer();
            _util = new ProjectUtil();

            _trendyolLayer = new TrendyolLayer();
            _n11Layer = new N11Layer();
            _koctasLayer = new KoctasLayer();
            _gittiGidiyorLayer = new GittiGidiyorLayer();
            _gittiGidiyorLayer.getOrders();
            _hepsiBuradaLayer = new HepsiBuradaLayer();
        }

        protected override void OnStart(string[] args)
        {
            var serverSettings = _util.getSetting();
            _util.LogService("Servis Başlatıldı.");

            timerForTransfer = new System.Timers.Timer(1000 * 60 * serverSettings.ControlTime);
            timerForTransfer.Start();
            timerForTransfer.Elapsed += timerForTransfer_Elapsed;
        }

        void timerForTransfer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerForTransfer.Stop();

            try
            {
                var licenceKey = _util.getSetting().LicenceKey;
                
                // ANG
                if (licenceKey.CaseInsensitiveContains("C218AA0A-5352-4EC3-9975-B37350697127"))
                {
                    try
                    {
                        _util.LogService("Hepsi Burada Siparişleri Çekiliyor");
                        _hepsiBuradaLayer.GetHepsiBuradaOrdersFromRestService(DateTime.Today.AddDays(-10), DateTime.Today);
                        _util.LogService("Hepsi Burada Siparişleri Tamamlandı");
                    }
                    catch (Exception ex)
                    {
                        _util.LogService($"Hepsi Burada Siparişleri Kaydı Sırasında Hata Oluştu.\n{ex}");
                    }

                    try
                    {
                        _util.LogService("Trendyol Siparişleri Çekiliyor | Created");
                        _trendyolLayer.getOrders(DateTime.Today.AddDays(-10), DateTime.Today, "created");
                        _util.LogService("Trendyol Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {

                        _util.LogService($"Trendyol Siparişleri Kaydı Sırasında Hata Oluştu.\n{exception}");
                    }

                    Thread.Sleep(1000 * 5);

                    try
                    {
                        _util.LogService("Trendyol Siparişleri Çekiliyor | awaiting");
                        _trendyolLayer.getOrders(DateTime.Today.AddDays(-10), DateTime.Today, "awaiting");
                        _util.LogService("Trendyol Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {
                        _util.LogService($"Trendyol Siparişleri Kaydı Sırasında Hata Oluştu.\n{exception}");
                    }

                    Thread.Sleep(1000 * 5);

                    try
                    {
                        _util.LogService("Trendyol Siparişleri Çekiliyor | string.Empty");
                        _trendyolLayer.getOrders(DateTime.Today.AddDays(-10), DateTime.Today, "");
                        _util.LogService("Trendyol Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {
                        _util.LogService($"Trendyol Siparişleri Kaydı Sırasında Hata Oluştu.\n{exception}");

                    }

                    try
                    {
                        _util.LogService("N11 Siparişleri Çekiliyor");
                        _n11Layer.getOrders(DateTime.Today.AddDays(-15), DateTime.Today);
                        _util.LogService("N11 Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {
                        _util.LogService($"N11 Kaydı Sırasında Hata Oluştu.\n{exception}");

                    }

                    try
                    {
                        _util.LogService("Koçtaş Siparişleri Çekiliyor");
                        _koctasLayer.getOrders();
                        _util.LogService("Koçtaş Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {
                        _util.LogService($"Koçtaş Kaydı Sırasında Hata Oluştu.\n{exception}");

                    }

                    try
                    {
                        _util.LogService("Gitti Gidiyor Siparişleri Çekiliyor");
                        _gittiGidiyorLayer.getOrders();
                        _util.LogService("Gitti Gidiyor Siparişleri Tamamlandı");
                    }
                    catch (Exception exception)
                    {
                        _util.LogService($"Gitti Gidiyor Kaydı Sırasında Hata Oluştu.\n{exception}");

                    }



                }

                // Barakat
                else if (licenceKey.CaseInsensitiveContains("8A605BFE-50BC-4126-B72C-2AF2F6F4A4F3"))
                {
                    _util.LogService("N11 Siparişleri Çekiliyor");
                    _n11Layer.getOrders(DateTime.Today.AddDays(-15), DateTime.Today);
                    _util.LogService("N11 Siparişleri Tamamlandı");
                }
            }
            catch (Exception exception)
            {
                _util.LogService(exception.Message);
            }

            timerForTransfer.Start();
        }

        protected override void OnStop()
        {
            _util.LogService("Servis Durduruldu.");
        }
    }
}