using DevExpress.Spreadsheet;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using PazarYeri.BusinessLayer.AltinciCadde;
using PazarYeri.BusinessLayer.ExcelTransfer;
using PazarYeri.BusinessLayer.HepsiBurada;
using PazarYeri.BusinessLayer.Koctas;
using PazarYeri.BusinessLayer.N11;
using PazarYeri.BusinessLayer.Trendyol;
using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using PazarYeri.BusinessLayer;
using PazarYeri.BusinessLayer.Amazon;
using PazarYeri.BusinessLayer.GittiGidiyor;
using PazarYeri.BusinessLayer.Helpers;
using net_HepsiBurada = PazarYeri.Models.net_HepsiBurada;
using Excel = Microsoft.Office.Interop.Excel;

namespace PazarYeri.UI
{
    public partial class frmPazarYeri : Form
    {
        private readonly DatabaseLayer _databaseLayer;
        private readonly net_LogoSettings _logoSettings;
        private readonly ProjectUtil _util;

        private GridView _activeGridView;
        private object _activeType;

        public frmPazarYeri()
        {
            GC.Collect();

            InitializeComponent();
            _databaseLayer = new DatabaseLayer();
            _util = new ProjectUtil();
            _logoSettings = _databaseLayer.GetLogoSettings();
        }

        private void frmPazarYeri_Load(object sender, EventArgs e)
        {
            this.Text = $@"{this.Text} - v.{Application.ProductVersion}";

            CheckLicenceKey();

            // MessageBox.Show(logoSettings.LogoUserName + " - " + logoSettings.LogoPassword + " - " + logoSettings.FirmNr + " - " + logoSettings.PeriodNr);

            dateEditTOBegDate.EditValue = DateTime.Today;
            dateEditTOEndDate.EditValue = DateTime.Today;
            dateEditGittiGidiyorStartDate.EditValue = DateTime.Today;
            dateEditGittiGidiyorEndDate.EditValue = DateTime.Today;
            dateEditGittiGidiyorTransferedStartDate.EditValue = DateTime.Today;
            dateEditGittiGidiyorTransferedEndDate.EditValue = DateTime.Today;
            dETOTransferedBegDate.EditValue = DateTime.Today;
            dETOTransferedEndDate.EditValue = DateTime.Today;

            timeTrendyolBegTime.EditValue = DateTime.Now.AddHours(-3);
            timeTrendyolEndTime.EditValue = DateTime.Now;

            timeEditGittiGidiyorStartTime.EditValue = DateTime.Now.AddHours(-3);
            timeEditGittiGidiyorEndTime.EditValue = DateTime.Now;

            TrendyWaitngOrders();

            dateHBTransferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateHBTransferedEndDate.EditValue = DateTime.Today;
            dateHBWaitingBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateHBWaitingEndDate.EditValue = DateTime.Today;
            HbWaitingOrders();

            dateEditKTBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditKTEndDate.EditValue = DateTime.Today;
            dateEditKTTransferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditKTTransferedEndDate.EditValue = DateTime.Today;
            KoctasWaitngOrders();

            dateEditN11BegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditN11EndDate.EditValue = DateTime.Today;
            dateEditN11TransferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditN11TransferedEndDate.EditValue = DateTime.Today;

            n11WaitingOrders();

            dateEditBoynerTrasnsferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditBoynerTrasnsfereEndDate.EditValue = DateTime.Today;
            BoynerWaitngOrders();

            dateEditEvideaTrasnsferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditEvideaTrasnsferedEndDate.EditValue = DateTime.Today;
            EvideaWaitngOrders();

            dateEditDekTrasnsferedBegDate.EditValue = DateTime.Today.AddDays(-2);
            dateEditDekTrasnsferedEndDate.EditValue = DateTime.Today;
            string LogoStatus = "";

            if (Global.Login(out LogoStatus))
            {
                lblLogoStatus.Text = LogoStatus;
                lblLogoStatus.ForeColor = Color.Green;
            }
            else
            {
                lblLogoStatus.Text = LogoStatus;
                lblLogoStatus.ForeColor = Color.Red;
            }
        }

        private void CheckLicenceKey()
        {
            var licenceKey = _util.getSetting().LicenceKey;

            // ANG
            if (licenceKey.CaseInsensitiveContains("C218AA0A-5352-4EC3-9975-B37350697127"))
            {
            }

            // Barakat
            else if (licenceKey.CaseInsensitiveContains("8A605BFE-50BC-4126-B72C-2AF2F6F4A4F3"))
            {
                xtraTabPageHebsiBurada.PageVisible = false;
                xtraTabPageTrendyol.PageVisible = false;
                xtraTabPageGittiGidiyor.PageVisible = false;
                xtraTabPageBoyner.PageVisible = false;
                xtraTabPageEvidea.PageVisible = false;
                xtraTabPageKoctas.PageVisible = false;
                xtraTabPageDekorazon.PageVisible = false;
                xtraTabPageAltinciCadde.PageVisible = false;
                xtraTabPageAmazon.PageVisible = false;
            }
            else
            {
                xtraTabPageHebsiBurada.PageVisible = false;
                xtraTabPageTrendyol.PageVisible = false;
                xtraTabPageN11.PageVisible = false;
                xtraTabPageGittiGidiyor.PageVisible = false;
                xtraTabPageBoyner.PageVisible = false;
                xtraTabPageEvidea.PageVisible = false;
                xtraTabPageKoctas.PageVisible = false;
                xtraTabPageDekorazon.PageVisible = false;
                xtraTabPageAltinciCadde.PageVisible = false;
                xtraTabPageAmazon.PageVisible = false;
            }
        }

        #region HepsiBurada

        private void gvHepsiBurada_PopupMenuShowing(
            object sender,
            DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.MenuType == GridMenuType.Row)
            {
                int rowHandle = e.HitInfo.RowHandle;
                DXMenuItem productPairin = btnProductPairing(view, rowHandle);
                e.Menu.Items.Add(productPairin);
            }
        }

        DXMenuCheckItem btnProductPairing(GridView view, int rowHandle)
        {
            DXMenuCheckItem checkItem = new DXMenuCheckItem(
                "Ürün Eşleştir",
                view.OptionsView.AllowCellMerge,
                null,
                new EventHandler(btnProductPairing));
            return checkItem;
        }

        void btnProductPairing(object sender, EventArgs e)
        {
            productCode = "";
            string Code =
                gvHepsiBurada.GetRowCellDisplayText(gvHepsiBurada.FocusedRowHandle, gvHepsiBurada.Columns["HBSKU"]);
            string Name = gvHepsiBurada.GetRowCellDisplayText(
                gvHepsiBurada.FocusedRowHandle,
                gvHepsiBurada.Columns["ProductName"]);
            frmProductPairing frm = new frmProductPairing(Code, Name, "HepsiBurada");
            frm.ShowDialog();

            for (int i = 0; i < gvHepsiBurada.RowCount; i++)
            {
                if (Code == gvHepsiBurada.GetRowCellDisplayText(i, gvHepsiBurada.Columns["HBSKU"]))
                {
                    gvHepsiBurada.SetRowCellValue(i, "TedSKU", productCode);
                }
            }
        }

        public static string productCode = "";

        private void btnHepsiBuradaTransfer_Click(object sender, EventArgs e)
        {
            var hepsiBuradaLayer = new HepsiBuradaLayer();

            var selectedRows = gvHepsiBurada.GetSelectedRows();

            var list = new List<net_HepsiBurada>();

            foreach (var rowHandle in selectedRows)
            {
                list.Add((net_HepsiBurada) gvHepsiBurada.GetRow(rowHandle));
            }

            foreach (var i in list.GroupBy(x => x.SasNo))
            {
                var orderLines = i.ToList();

                var entegratedOrder = new net_EntegratedOrders();

                if (orderLines.Any(x => string.IsNullOrEmpty(x.TedSKU)))
                {
                    var hepsiBurada = orderLines.First(x => string.IsNullOrEmpty(x.TedSKU));

                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                        serializer.Serialize(stringwriter, hepsiBurada);
                        var returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "HepsiBurada";
                    entegratedOrder.OrderDate = hepsiBurada.CreatedDate;
                    entegratedOrder.OrderNo = hepsiBurada.SasNo;
                    entegratedOrder.LineNr = hepsiBurada.SasItemNr.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = "Tanımlı stok kodu bulunamadı..";
                    _databaseLayer.saveOrders(entegratedOrder);

                    var rowHandle = gvHepsiBurada.LocateByValue("HBSKU", hepsiBurada.SasNo);

                    gvHepsiBurada.SetRowCellValue(rowHandle, "Status", "Tanımlı stok kodu bulunamadı..");

                    continue;
                }

                if (hepsiBuradaLayer.TransferOrders(orderLines, out var result))
                {
                    foreach (var netHepsiBurada in orderLines)
                    {
                        using (var stringwriter = new StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                            serializer.Serialize(stringwriter, netHepsiBurada);
                            var returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "HepsiBurada";
                        entegratedOrder.OrderDate = netHepsiBurada.CreatedDate;
                        entegratedOrder.OrderNo = netHepsiBurada.SasNo;
                        entegratedOrder.LineNr = netHepsiBurada.SasItemNr.ToString();
                        entegratedOrder.Transfered = 1;

                        _databaseLayer.UpdateOrders(entegratedOrder);
                    }

                    for (var j = selectedRows.Length - 1; j >= 0; j--)
                    {
                        var netHepsiBurada = (net_HepsiBurada) gvHepsiBurada.GetRow(j);

                        if (netHepsiBurada == null)
                            continue;

                        if (netHepsiBurada.SasNo == orderLines.First().SasNo)
                        {
                            gvHepsiBurada.DeleteRow(j);
                        }
                    }
                }
                else
                {
                    var hepsiBurada = orderLines.First();

                    hepsiBurada.ResultMsg = result;

                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                        serializer.Serialize(stringwriter, hepsiBurada);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "HepsiBurada";
                    entegratedOrder.OrderDate = hepsiBurada.CreatedDate;
                    entegratedOrder.OrderNo = hepsiBurada.SasNo;
                    entegratedOrder.LineNr = hepsiBurada.LineNr.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = result;

                    var rowHandle = gvHepsiBurada.LocateByValue("HBSKU", hepsiBurada.SasNo);

                    gvHepsiBurada.SetRowCellValue(rowHandle, "Status", result);
                }
            }

            gvHepsiBurada.ClearSelection();

            btnGetHBOrders.PerformClick();
        }

        private void btnGetHBOrders_Click(object sender, EventArgs e)
        {
            HbWaitingOrders();
        }

        private void btnGetHBTransfered_Click(object sender, EventArgs e)
        {
            HBTransferedOrders();
        }

        private void tabHepsiBurada_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabHepsiBurada.SelectedTabPage == pageHBTransfered)
            {
                HBTransferedOrders();
            }
            else if (tabHepsiBurada.SelectedTabPage == pageHBWaiting)
            {
                HbWaitingOrders();
            }
        }

        private void HBTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateHBTransferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateHBTransferedEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("HepsiBurada", begdate, enddate);
            List<net_HepsiBurada> hepsiBuradas = new List<net_HepsiBurada>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                    net_HepsiBurada hepsiBurada = (net_HepsiBurada) serializer.Deserialize(stringReader);
                    hepsiBurada.LineNr = lineCount;
                    hepsiBuradas.Add(hepsiBurada);
                }

                lineCount++;
            }

            gridHBSended.DataSource = hepsiBuradas;
        }

        private void HbWaitingOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateHBWaitingBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateHBWaitingEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetWaitingOrders("HepsiBurada", begdate, enddate);
            List<net_HepsiBurada> hepsiBuradas = new List<net_HepsiBurada>();
            int lineCount = 1;

            foreach (var item in list)
            {
                if (item.OrderXml.Length > 0)
                {
                    var xmlString = Encoding.UTF8.GetString(item.OrderXml);
                    _util.LogService(xmlString);
                    using (var stringReader = new StringReader(xmlString))
                    {
                        var serializerrest = new XmlSerializer(typeof(HepsiBuradaRestModel));
                        var serializer = new XmlSerializer(typeof(net_HepsiBurada));
                        var hepsiBurada = (net_HepsiBurada) serializer.Deserialize(stringReader);
                        hepsiBurada.LineNr = lineCount;
                        hepsiBurada.ResultMsg = item.ResultMsg;
                        hepsiBuradas.Add(hepsiBurada);
                    }
                }

                lineCount++;
            }

            foreach (var netHepsiBurada in hepsiBuradas)
            {
                var pairing = new net_ProductPairing
                    {EntegrationCode = netHepsiBurada.HBSKU, EntegrationName = "HepsiBurada"};
                pairing = _databaseLayer.GetProductPairing(pairing);

                if (pairing != null && (string.IsNullOrEmpty(netHepsiBurada.TedSKU) || netHepsiBurada.TedSKU == "-" ||
                                        netHepsiBurada.TedSKU == netHepsiBurada.HBSKU))
                {
                    netHepsiBurada.TedSKU = pairing.LogoCode;
                }
            }

            gridHepsiBurada.DataSource = hepsiBuradas;
        }

        #endregion

        #region trendyOl

        private void btnGetTredyolOrders_Click(object sender, EventArgs e)
        {
            TrendyWaitngOrders();
        }

        private void btnTrendyTransfer_Click(object sender, EventArgs e)
        {
            TrendyolLayer trendyolLayer = new TrendyolLayer();
            int[] RowHandles = gvTrendyol.GetSelectedRows();
            List<int> deletingRows = new List<int>();
            foreach (var i in RowHandles)
            {
                string Statu = gvTrendyol.GetRowCellDisplayText(i, gvTrendyol.Columns["Status"]);
                net_Trendyol trendyol = (net_Trendyol) gvTrendyol.GetRow(i);
                if (Statu != "Cancelled")
                {
                    string result = "";
                    net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                    if (trendyolLayer.transferOrder(trendyol, out result))
                    {
                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_Trendyol));
                            serializer.Serialize(stringwriter, trendyol);
                            string returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "Trendyol";
                        entegratedOrder.OrderDate = trendyol.orderDate;
                        entegratedOrder.OrderNo = trendyol.orderNumber;
                        entegratedOrder.LineNr = trendyol.LineId.ToString();
                        entegratedOrder.Transfered = 1;
                        deletingRows.Add(i);
                    }
                    else
                    {
                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_Trendyol));
                            serializer.Serialize(stringwriter, trendyol);
                            string returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "Trendyol";
                        entegratedOrder.OrderDate = trendyol.orderDate;
                        entegratedOrder.OrderNo = trendyol.orderNumber;
                        entegratedOrder.LineNr = trendyol.LineId.ToString();
                        entegratedOrder.Transfered = 0;
                        entegratedOrder.ResultMsg = result;
                        gvTrendyol.SetRowCellValue(i, "Status", result);
                    }

                    _databaseLayer.UpdateOrders(entegratedOrder);
                }
                else
                {
                    net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Trendyol));
                        serializer.Serialize(stringwriter, trendyol);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "Trendyol";
                    entegratedOrder.OrderDate = trendyol.orderDate;
                    entegratedOrder.OrderNo = trendyol.orderNumber;
                    entegratedOrder.LineNr = trendyol.LineId.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = "İptal olduğu için Logoya aktarılamaz.";
                    gvTrendyol.SetRowCellValue(i, "Status", "İptal olduğu için Logoya aktarılamaz.");
                    _databaseLayer.UpdateOrders(entegratedOrder);
                }
            }

            foreach (var i in deletingRows.OrderByDescending(x => x))
            {
                gvTrendyol.DeleteRow(i);
            }

            gvTrendyol.ClearSelection();
        }

        private void btnGetTredyolTransferedOrders_Click(object sender, EventArgs e)
        {
            TrendyTransferedOrders();
        }

        private void TrendyTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dETOTransferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dETOTransferedEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("Trendyol", begdate, enddate);
            List<net_Trendyol> trendyols = new List<net_Trendyol>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Trendyol));
                    net_Trendyol trendyol = (net_Trendyol) serializer.Deserialize(stringReader);
                    trendyol.LineNr = lineCount;
                    trendyols.Add(trendyol);
                }

                lineCount++;
            }

            gridTrendyolTransfered.DataSource = trendyols;
        }

        private void TrendyWaitngOrders()
        {
            string begTime = Convert.ToDateTime(dateEditTOBegDate.EditValue).ToString("dd.MM.yyyy") +
                             " " +
                             Convert.ToDateTime(timeTrendyolBegTime.EditValue).ToString("HH:mm");
            string endTime = Convert.ToDateTime(dateEditTOEndDate.EditValue).ToString("dd.MM.yyyy") +
                             " " +
                             Convert.ToDateTime(timeTrendyolEndTime.EditValue).ToString("HH:mm");

            DateTime begdate = Convert.ToDateTime(begTime);
            DateTime enddate = Convert.ToDateTime(endTime);
            var list = _databaseLayer.GetWaitingOrders("Trendyol", begdate, enddate);
            List<net_Trendyol> trendyols = new List<net_Trendyol>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Trendyol));
                    net_Trendyol trendyol = (net_Trendyol) serializer.Deserialize(stringReader);
                    trendyol.LineNr = lineCount;
                    trendyols.Add(trendyol);
                }

                lineCount++;
            }

            gridTrendyol.DataSource = trendyols;
        }

        private void tabTrendyol_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabTrendyol.SelectedTabPage == pageTrendyWaiting)
            {
                TrendyWaitngOrders();
            }
            else if (tabTrendyol.SelectedTabPage == pageTrendyTransfered)
            {
                TrendyTransferedOrders();
            }
        }

        #endregion

        #region Koçtaş

        private void tabKoctas_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabKoctas.SelectedTabPage == pageKoctasWaiting)
            {
                KoctasWaitngOrders();
            }
            else if (tabKoctas.SelectedTabPage == pageKoctasTransfered)
            {
                KoctasTransferedOrders();
            }
        }

        private void KoctasWaitngOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditKTBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditKTEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetWaitingOrders("Koçtaş", begdate, enddate);
            List<net_Koctas> koctasList = new List<net_Koctas>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Koctas));
                    net_Koctas koctas = (net_Koctas) serializer.Deserialize(stringReader);
                    koctas.LineNr = lineCount;
                    net_ProductPairing pairing = new net_ProductPairing();
                    pairing.EntegrationCode = koctas.ItemCode;
                    pairing.EntegrationName = "Koçtaş";
                    pairing = _databaseLayer.GetProductPairing(pairing);
                    if (pairing != null && string.IsNullOrEmpty(koctas.LogoCode))
                    {
                        koctas.LogoCode = pairing.LogoCode;
                        koctas.Barcode = pairing.Barcode;
                    }
                    else
                    {
                        string itemcode = _databaseLayer.GetItemCode(koctas.LogoCode);

                        if (!string.IsNullOrEmpty(itemcode))
                        {
                            koctas.Barcode = koctas.LogoCode;
                            koctas.LogoCode = itemcode;
                        }
                    }

                    koctasList.Add(koctas);
                }

                lineCount++;
            }

            gridKoctasWaiting.DataSource = koctasList;
        }

        private void KoctasTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditKTBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditKTEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("Koçtaş", begdate, enddate);
            List<net_Koctas> koctasList = new List<net_Koctas>();
            int lineCount = 1;
            foreach (var item in list.OrderBy(x => x.OrderNo))
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Koctas));
                    net_Koctas koctas = (net_Koctas) serializer.Deserialize(stringReader);
                    koctas.LineNr = lineCount;
                    net_ProductPairing pairing = new net_ProductPairing();
                    pairing.EntegrationCode = koctas.ItemCode;
                    pairing.EntegrationName = "Koçtaş";
                    pairing = _databaseLayer.GetProductPairing(pairing);
                    if (pairing != null)
                    {
                        koctas.LogoCode = pairing.LogoCode;
                        koctas.Barcode = pairing.Barcode;
                    }
                    else
                    {
                        string itemcode = _databaseLayer.GetItemCode(koctas.LogoCode);

                        if (!string.IsNullOrEmpty(itemcode))
                        {
                            koctas.Barcode = koctas.LogoCode;
                            koctas.LogoCode = itemcode;
                        }
                    }

                    koctasList.Add(koctas);
                }

                lineCount++;
            }

            gridKoctasTransfered.DataSource = koctasList;
        }

        private void btnGetKTOrders_Click(object sender, EventArgs e)
        {
            KoctasWaitngOrders();
        }

        private void btnKoctasTransfer_Click(object sender, EventArgs e)
        {
            KoctasLayer koctasLayer = new KoctasLayer();
            int[] RowHandles = gvKoctasWaiting.GetSelectedRows();
            List<int> deletingRows = new List<int>();
            foreach (var i in RowHandles)
            {
                string result = "";
                net_Koctas koctas = (net_Koctas) gvKoctasWaiting.GetRow(i);
                net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                if (koctasLayer.OrderTransfer(new List<net_Koctas>() {koctas}, out result))
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Koctas));
                        serializer.Serialize(stringwriter, koctas);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "Koçtaş";
                    entegratedOrder.OrderDate = koctas.Date_;
                    entegratedOrder.OrderNo = koctas.SiparisNo;
                    entegratedOrder.LineNr = koctas.OrderLineNr.ToString();
                    entegratedOrder.Transfered = 1;
                    deletingRows.Add(i);
                }
                else
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Koctas));
                        serializer.Serialize(stringwriter, koctas);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "Koçtaş";
                    entegratedOrder.OrderDate = koctas.Date_;
                    entegratedOrder.OrderNo = koctas.SiparisNo;
                    entegratedOrder.LineNr = koctas.OrderLineNr.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = result;
                    gvKoctasWaiting.SetRowCellValue(i, "Status", result);
                }

                _databaseLayer.UpdateOrders(entegratedOrder);
            }

            foreach (var i in deletingRows.OrderByDescending(x => x))
            {
                gvKoctasWaiting.DeleteRow(i);
            }

            gvKoctasWaiting.ClearSelection();
        }

        private void gvKoctasWaiting_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.MenuType == GridMenuType.Row)
            {
                int rowHandle = e.HitInfo.RowHandle;
                DXMenuItem productPairin = btnKTProductPairing(view, rowHandle);
                e.Menu.Items.Add(productPairin);
            }
        }

        DXMenuCheckItem btnKTProductPairing(GridView view, int rowHandle)
        {
            DXMenuCheckItem checkItem = new DXMenuCheckItem(
                "Ürün Eşleştir",
                view.OptionsView.AllowCellMerge,
                null,
                new EventHandler(btnKTProductPairing));
            return checkItem;
        }

        DXMenuCheckItem btnKTProductPairingExcel(GridView view, int rowHandle)
        {
            DXMenuCheckItem checkItem = new DXMenuCheckItem(
                "Ürün Eşleştir",
                view.OptionsView.AllowCellMerge,
                null,
                new EventHandler(btnKTProductPairingExcel));
            return checkItem;
        }

        void btnKTProductPairingExcel(object sender, EventArgs e)
        {
            productCode = "";
            string Code = gridViewKoctasExcelTransfer.GetRowCellDisplayText(
                gridViewKoctasExcelTransfer.FocusedRowHandle,
                gridViewKoctasExcelTransfer.Columns["ItemCode"]);
            string Name = gridViewKoctasExcelTransfer.GetRowCellDisplayText(
                gridViewKoctasExcelTransfer.FocusedRowHandle,
                gridViewKoctasExcelTransfer.Columns["ItemName"]);
            string barCode = gridViewKoctasExcelTransfer.GetRowCellDisplayText(
                gridViewKoctasExcelTransfer.FocusedRowHandle,
                gridViewKoctasExcelTransfer.Columns["Barcode"]);
            frmProductPairing frm = new frmProductPairing(Code, Name, "Koçtaş");
            frm.ShowDialog();

            for (int i = 0; i < gvKoctasWaiting.RowCount; i++)
            {
                if (Code == gvKoctasWaiting.GetRowCellDisplayText(i, gvKoctasWaiting.Columns["ItemCode"]))
                {
                    gvKoctasWaiting.SetRowCellValue(i, "LogoCode", productCode);
                }
            }
        }

        void btnKTProductPairing(object sender, EventArgs e)
        {
            productCode = "";
            string Code = gvKoctasWaiting.GetRowCellDisplayText(
                gvKoctasWaiting.FocusedRowHandle,
                gvKoctasWaiting.Columns["ItemCode"]);
            string Name = gvKoctasWaiting.GetRowCellDisplayText(
                gvKoctasWaiting.FocusedRowHandle,
                gvKoctasWaiting.Columns["ItemName"]);
            string barCode = gvKoctasWaiting.GetRowCellDisplayText(
                gvKoctasWaiting.FocusedRowHandle,
                gvKoctasWaiting.Columns["Barcode"]);
            frmProductPairing frm = new frmProductPairing(Code, Name, "Koçtaş");
            frm.ShowDialog();

            for (int i = 0; i < gvKoctasWaiting.RowCount; i++)
            {
                if (Code == gvKoctasWaiting.GetRowCellDisplayText(i, gvKoctasWaiting.Columns["ItemCode"]))
                {
                    gvKoctasWaiting.SetRowCellValue(i, "LogoCode", productCode);
                }
            }
        }

        #endregion

        #region N11

        private void n11WaitingOrders()
        {
            var begdate = Convert.ToDateTime(dateEditN11BegDate.EditValue);
            var enddate = Convert.ToDateTime(dateEditN11EndDate.EditValue).AddDays(1);

            var list = _databaseLayer.GetWaitingOrders("N11", begdate, enddate);
            var n11List = new List<net_N11>();
            var lineCount = 1;

            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_N11));

                    var n11 = (net_N11) serializer.Deserialize(stringReader);
                    n11.LineNr = lineCount;
                    n11.Id = item.Id;
                    n11.status = item.ResultMsg;

                    n11List.Add(n11);
                }

                lineCount++;
            }

            gridN11Waitings.DataSource = n11List;

            gvN11Waitings.BestFitColumns();
        }

        private void n11TransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditN11TransferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditN11TransferedEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("N11", begdate, enddate);
            List<net_N11> n11List = new List<net_N11>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_N11));
                    net_N11 n11 = (net_N11) serializer.Deserialize(stringReader);
                    n11.LineNr = lineCount;
                    n11.Id = item.Id;
                    n11List.Add(n11);
                }

                lineCount++;
            }

            gridN11Transfered.DataSource = n11List;
        }

        private void btnGetN11Orders_Click(object sender, EventArgs e)
        {
            n11WaitingOrders();
        }

        private void btnN11Transfer_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show(
                "Seçili olan siparişler Logoya aktarılcaktır. Emin misiniz?",
                "Logoya Aktarım",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                N11Layer n11layer = new N11Layer();
                int[] RowHandles = gvN11Waitings.GetSelectedRows();
                List<int> deletingRows = new List<int>();
                foreach (var i in RowHandles)
                {
                    string result = "";
                    net_N11 n11 = (net_N11) gvN11Waitings.GetRow(i);
                    net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                    if (n11layer.OrderTransfer(n11, out result))
                    {
                        n11.status = "";

                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_N11));
                            serializer.Serialize(stringwriter, n11);
                            string returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "N11";
                        entegratedOrder.OrderDate = n11.createDate;
                        entegratedOrder.OrderNo = n11.orderNumber;
                        entegratedOrder.LineNr = n11.lineId.ToString();
                        entegratedOrder.Transfered = 1;
                        deletingRows.Add(i);
                    }
                    else
                    {
                        n11.status = result;

                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_N11));
                            serializer.Serialize(stringwriter, n11);
                            string returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "N11";
                        entegratedOrder.OrderDate = n11.createDate;
                        entegratedOrder.OrderNo = n11.orderNumber;
                        entegratedOrder.LineNr = n11.lineId.ToString();
                        entegratedOrder.Transfered = 0;
                        entegratedOrder.ResultMsg = result;
                        gvN11Waitings.SetRowCellValue(i, "Status", result);
                    }

                    _databaseLayer.UpdateOrders(entegratedOrder);
                }

                foreach (var i in deletingRows.OrderByDescending(x => x))
                {
                    gvN11Waitings.DeleteRow(i);
                }

                gvN11Waitings.ClearSelection();

                gvN11Waitings.BestFitColumns();
            }
        }

        private void btnGetN11TransferedOrders_Click(object sender, EventArgs e)
        {
            n11TransferedOrders();
        }

        private void tabN11_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabN11.SelectedTabPage == pageN11Orders)
            {
                n11WaitingOrders();
            }
            else if (tabN11.SelectedTabPage == pageN11TransferedOrders)
            {
                n11TransferedOrders();
            }
        }

        //private void gvN11Waitings_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        //{
        //    GridView view = sender as GridView;
        //    if (e.MenuType == GridMenuType.Row)
        //    {
        //        int rowHandle = e.HitInfo.RowHandle;
        //        DXMenuItem setTransfered = btnN11SetTransferd(view, rowHandle);
        //        e.Menu.Items.Add(setTransfered);
        //    }
        //}

        //DXMenuCheckItem btnN11SetTransferd(GridView view, int rowHandle)
        //{
        //    DXMenuCheckItem checkItem = new DXMenuCheckItem(
        //        "Aktarıldı Olarak İşaretle",
        //        view.OptionsView.AllowCellMerge,
        //        null,
        //        new EventHandler(btnN11SetTransferd));
        //    return checkItem;
        //}

        //void btnN11SetTransferd(object sender, EventArgs e)
        //{
        //    DialogResult dialog =
        //        MessageBox.Show(
        //            "Seçili olan siparişler Logoya aktarıldı olarak işaretlenecektir. Emin misiniz?",
        //            "Logoya Aktarım",
        //            MessageBoxButtons.YesNo,
        //            MessageBoxIcon.Question);

        //    if (dialog == DialogResult.Yes)
        //    {
        //        int[] RowHandles = gvN11Waitings.GetSelectedRows();
        //        List<int> selectedRows = new List<int>();
        //        foreach (var rowNr in RowHandles)
        //        {
        //            string Id = gvN11Waitings.GetRowCellDisplayText(rowNr, gvN11Waitings.Columns["Id"]);
        //            _databaseLayer.UpdateOrderStatus(Id, 1);
        //            selectedRows.Add(rowNr);
        //        }

        //        foreach (var i in selectedRows.OrderByDescending(x => x))
        //        {
        //            gvN11Waitings.DeleteRow(i);
        //        }

        //        gvN11Waitings.ClearSelection();
        //    }
        //}

        private void SetPopUp(object sender, PopupMenuShowingEventArgs e)
        {
        }

        //private void gvN11Transfered_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        //{
        //    var view = sender as GridView;

        //    _activeGridView = gvN11Transfered;

        //    if (e.MenuType == GridMenuType.Row)
        //    {
        //        int rowHandle = e.HitInfo.RowHandle;
        //        DXMenuItem setTransfered = AddPopUpMenuOptionToGridView(view, rowHandle);
        //        e.Menu.Items.Add(setTransfered);
        //    }
        //}

        DXMenuCheckItem AddPopUpMenuOptionToGridView(GridView view, int rowHandle)
        {
            DXMenuCheckItem checkItem = new DXMenuCheckItem(
                "Aktarılmadı Olarak İşaretle",
                view.OptionsView.AllowCellMerge,
                null,
                new EventHandler(ChangeTransferedSign));

            return checkItem;
        }

        private void ChangeTransferedSign(object sender, EventArgs e)
        {
            DialogResult dialog =
                MessageBox.Show(
                    "Seçili olan siparişler Logoya aktarılmadı olarak işaretlenecektir. Emin misiniz?",
                    "Logoya Aktarım",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                int[] RowHandles = _activeGridView.GetSelectedRows();
                List<int> selectedRows = new List<int>();
                foreach (var rowNr in RowHandles)
                {
                    string Id = _activeGridView.GetRowCellDisplayText(rowNr, _activeGridView.Columns["Id"]);
                    _databaseLayer.UpdateOrderStatus(Id, 0);
                    selectedRows.Add(rowNr);
                }

                foreach (var i in selectedRows.OrderByDescending(x => x))
                {
                    _activeGridView.DeleteRow(i);
                }

                _activeGridView.ClearSelection();
            }
        }

        #endregion

        #region Boyner

        private void btnSelectBoynerExcel_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm";
                openFileDialog1.ShowDialog();
                btnSelectBoynerExcel.Text = openFileDialog1.FileName;
                if (!string.IsNullOrEmpty(btnSelectBoynerExcel.Text))
                {
                    ssExcel.Document.LoadDocument(btnSelectBoynerExcel.Text);
                    Worksheet sheet = ssExcel.Document.Worksheets[0];
                    List<net_Boyner> boyners = new List<net_Boyner>();
                    int lastRow = sheet.GetUsedRange().BottomRowIndex + 1;
                    for (int i = 1; i < lastRow; i++)
                    {
                        int orderlineCode = 0;
                        if (int.TryParse(sheet.Rows[i][0].Value.ToString(), out orderlineCode))
                        {
                            string Barcode = sheet.Rows[i][3].Value.ToString().Trim();
                            string itemcode = _databaseLayer.GetItemCode(Barcode);
                            net_Boyner boyner = new net_Boyner()
                            {
                                Barcode = sheet.Rows[i][3].Value.ToString().Trim(),
                                BoynerProductCode = sheet.Rows[i][1].Value.ToString().Trim(),
                                CargoNo = sheet.Rows[i][11].Value.ToString(),
                                Date_ = DateTime.Today,
                                LineNr = i,
                                OrderCode = sheet.Rows[i][10].Value.ToString(),
                                OrderLineCode = Convert.ToInt32(sheet.Rows[i][0].Value.ToString()),
                                Price = Convert.ToDouble(sheet.Rows[i][5].Value.ToString().Replace(".", ",")) * 1.18,
                                ProductCode = _databaseLayer.GetItemCode(sheet.Rows[i][3].Value.ToString().Trim()),
                                ProductName = sheet.Rows[i][4].Value.ToString(),
                                Quantity = Convert.ToDouble(sheet.Rows[i][2].Value.ToString().Replace(".", ","))
                            };

                            if (_databaseLayer.GetOrderStatus("Boyner", boyner.OrderCode, orderlineCode.ToString()))
                            {
                                boyner.Statu = "Logoya Aktarılmış";
                            }

                            boyners.Add(boyner);
                        }
                    }

                    gridBoynerWaiting.DataSource = boyners;
                }
                else
                {
                    MessageBox.Show("Excel Dosyası Seçiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void BoynerWaitngOrders()
        {
            DateTime begdate = DateTime.Today.AddDays(-100);

            DateTime enddate = DateTime.Today;
            var list = _databaseLayer.GetWaitingOrders("Boyner", begdate, enddate);
            List<net_Boyner> n11List = new List<net_Boyner>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Boyner));
                    net_Boyner n11 = (net_Boyner) serializer.Deserialize(stringReader);
                    n11.LineNr = lineCount;

                    n11List.Add(n11);
                }

                lineCount++;
            }

            gridBoynerWaiting.DataSource = n11List;
        }

        private void BoynerTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditBoynerTrasnsferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditBoynerTrasnsfereEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("Boyner", begdate, enddate);
            List<net_Boyner> boynerList = new List<net_Boyner>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Boyner));
                    net_Boyner boyner = (net_Boyner) serializer.Deserialize(stringReader);
                    boyner.LineNr = lineCount;

                    boynerList.Add(boyner);
                }

                lineCount++;
            }

            gridBoynerTransfered.DataSource = boynerList;
        }

        private void btnBoynerTransfer_Click(object sender, EventArgs e)
        {
            BoynerLayer boynerLayer = new BoynerLayer();
            DialogResult dialog = MessageBox.Show(
                "Seçili olan siparişler Logoya aktarılcaktır. Emin misiniz?",
                "Logoya Aktarım",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                int[] RowHandles = gvBoynerWaiting.GetSelectedRows();
                List<int> deletingRows = new List<int>();
                foreach (var i in RowHandles)
                {
                    string result = "";
                    net_Boyner boynerOrder = (net_Boyner) gvBoynerWaiting.GetRow(i);
                    net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                    if (boynerOrder.Statu != "Logoya Aktarılmış")
                    {
                        if (boynerLayer.transferOrder(boynerOrder, out result))
                        {
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_Boyner));
                                serializer.Serialize(stringwriter, boynerOrder);
                                string returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = "Boyner";
                            entegratedOrder.OrderDate = boynerOrder.Date_;
                            entegratedOrder.OrderNo = boynerOrder.OrderCode;
                            entegratedOrder.LineNr = boynerOrder.OrderLineCode.ToString();
                            entegratedOrder.LogoFicheNo = result;
                            entegratedOrder.Transfered = 1;
                            deletingRows.Add(i);
                        }
                        else
                        {
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_Boyner));
                                serializer.Serialize(stringwriter, boynerOrder);
                                string returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = "Boyner";
                            entegratedOrder.OrderDate = boynerOrder.Date_;
                            entegratedOrder.OrderNo = boynerOrder.OrderCode;
                            entegratedOrder.LineNr = boynerOrder.OrderLineCode.ToString();
                            entegratedOrder.Transfered = 0;
                            entegratedOrder.ResultMsg = result;
                            gvBoynerWaiting.SetRowCellValue(i, "Status", result);
                        }

                        _databaseLayer.UpdateOrders(entegratedOrder);
                    }
                }

                foreach (var i in deletingRows.OrderByDescending(x => x))
                {
                    gvBoynerWaiting.DeleteRow(i);
                }

                gvBoynerWaiting.ClearSelection();
            }
        }

        private void tabBoyner_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabBoyner.SelectedTabPage == pageBoynerWaiting)
            {
                BoynerWaitngOrders();
            }
            else if (tabBoyner.SelectedTabPage == pageBoynerTransfered)
            {
                BoynerTransferedOrders();
            }
        }

        private void gvBoynerWaiting_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            string Statu =
                gvBoynerWaiting.GetRowCellDisplayText(
                    gvBoynerWaiting.FocusedRowHandle,
                    gvBoynerWaiting.Columns["Status"]);
            if (Statu == "Logoya Aktarılmış")
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else
            {
                e.Appearance.ForeColor = Color.Green;
            }
        }

        #endregion

        #region Evidea

        private void btnSelectEvideaExcel_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm";
                openFileDialog1.ShowDialog();
                btnSelectEvideaExcel.Text = openFileDialog1.FileName;
                if (!string.IsNullOrEmpty(btnSelectEvideaExcel.Text))
                {
                    ssExcel.Document.LoadDocument(btnSelectEvideaExcel.Text);
                    Worksheet sheet = ssExcel.Document.Worksheets[0];
                    List<net_Evidea> evideaList = new List<net_Evidea>();
                    int lastRow = sheet.GetUsedRange().BottomRowIndex + 1;
                    for (int i = 1; i < lastRow; i++)
                    {
                        int orderlineCode = 0;
                        if (int.TryParse(sheet.Rows[i][0].Value.ToString(), out orderlineCode))
                        {
                            string Barcode = sheet.Rows[i][3].Value.ToString().Trim();
                            string itemcode = _databaseLayer.GetItemCode(Barcode);
                            net_Evidea evidea = new net_Evidea()
                            {
                                Barcode = sheet.Rows[i][6].Value.ToString().Trim(),
                                EvideaProductCode = sheet.Rows[i][7].Value.ToString().Trim(),

                                Date_ = DateTime.Today,
                                LineNr = i,
                                OrderNumber = sheet.Rows[i][18].Value.ToString(),
                                OrderLineNr = Convert.ToInt32(sheet.Rows[i][0].Value.ToString()),
                                Price = Convert.ToDouble(sheet.Rows[i][16].Value.ToString().Replace(".", ",")) * 1.18,
                                ProductCode = _databaseLayer.GetItemCode(sheet.Rows[i][6].Value.ToString().Trim()),
                                ProductName = sheet.Rows[i][10].Value.ToString(),
                                Quantity = Convert.ToDouble(sheet.Rows[i][13].Value.ToString().Replace(".", ","))
                            };

                            if (_databaseLayer.GetOrderStatus(
                                "Boyner",
                                evidea.OrderNumber,
                                evidea.OrderLineNr.ToString()))
                            {
                                evidea.Statu = "Logoya Aktarılmış";
                            }

                            evideaList.Add(evidea);
                        }
                    }

                    gridEvideaWaiting.DataSource = evideaList;
                }
                else
                {
                    MessageBox.Show("Excel Dosyası Seçiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void gvEvideaWaiting_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            string Statu =
                gvEvideaWaiting.GetRowCellDisplayText(
                    gvEvideaWaiting.FocusedRowHandle,
                    gvEvideaWaiting.Columns["Status"]);
            if (Statu == "Logoya Aktarılmış")
            {
                e.Appearance.ForeColor = Color.Red;
            }
            else
            {
                e.Appearance.ForeColor = Color.Green;
            }
        }

        private void btnEvideaTransfer_Click(object sender, EventArgs e)
        {
            EvideaLayer evideaLayer = new EvideaLayer();
            DialogResult dialog = MessageBox.Show(
                "Seçili olan siparişler Logoya aktarılcaktır. Emin misiniz?",
                "Logoya Aktarım",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                int[] RowHandles = gvEvideaWaiting.GetSelectedRows();
                List<int> deletingRows = new List<int>();
                foreach (var i in RowHandles)
                {
                    string result = "";
                    net_Evidea evideOrder = (net_Evidea) gvEvideaWaiting.GetRow(i);
                    net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                    if (evideOrder.Statu != "Logoya Aktarılmış" && !string.IsNullOrEmpty(evideOrder.ProductCode))
                    {
                        if (evideaLayer.transferOrder(evideOrder, out result))
                        {
                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_Evidea));
                                serializer.Serialize(stringwriter, evideOrder);
                                string returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = "Evidea";
                            entegratedOrder.OrderDate = evideOrder.Date_;
                            entegratedOrder.OrderNo = evideOrder.OrderNumber;
                            entegratedOrder.LineNr = evideOrder.OrderLineNr.ToString();
                            entegratedOrder.LogoFicheNo = result;
                            entegratedOrder.Transfered = 1;
                            deletingRows.Add(i);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(evideOrder.ProductCode))
                            {
                                result = "Ürün Kodu bulunamadı..";
                            }

                            using (var stringwriter = new System.IO.StringWriter())
                            {
                                var serializer = new XmlSerializer(typeof(net_Boyner));
                                serializer.Serialize(stringwriter, evideOrder);
                                string returnStr = stringwriter.ToString();
                                entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                            }

                            entegratedOrder.EntegrationName = "Evidea";
                            entegratedOrder.OrderDate = evideOrder.Date_;
                            entegratedOrder.OrderNo = evideOrder.OrderNumber;
                            entegratedOrder.LineNr = evideOrder.OrderLineNr.ToString();
                            entegratedOrder.Transfered = 0;
                            entegratedOrder.ResultMsg = result;
                            gvEvideaWaiting.SetRowCellValue(i, "Status", result);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(evideOrder.ProductCode))
                        {
                            result = "Ürün Kodu bulunamadı..";
                        }

                        using (var stringwriter = new System.IO.StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_Evidea));
                            serializer.Serialize(stringwriter, evideOrder);
                            string returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        entegratedOrder.EntegrationName = "Evidea";
                        entegratedOrder.OrderDate = evideOrder.Date_;
                        entegratedOrder.OrderNo = evideOrder.OrderNumber;
                        entegratedOrder.LineNr = evideOrder.OrderLineNr.ToString();
                        entegratedOrder.Transfered = 0;
                        entegratedOrder.ResultMsg = result;
                        gvEvideaWaiting.SetRowCellValue(i, "Status", result);
                    }

                    _databaseLayer.UpdateOrders(entegratedOrder);
                }

                foreach (var i in deletingRows.OrderByDescending(x => x))
                {
                    gvEvideaWaiting.DeleteRow(i);
                }

                gvEvideaWaiting.ClearSelection();
            }
        }

        private void EvideaWaitngOrders()
        {
            DateTime begdate = DateTime.Today.AddDays(-100);
            DateTime enddate = DateTime.Today;
            var list = _databaseLayer.GetWaitingOrders("Evidea", begdate, enddate);
            List<net_Evidea> evideaList = new List<net_Evidea>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Evidea));
                    net_Evidea evidea = (net_Evidea) serializer.Deserialize(stringReader);
                    evidea.LineNr = lineCount;
                    evideaList.Add(evidea);
                }

                lineCount++;
            }

            gridEvideaWaiting.DataSource = evideaList;
        }

        private void EvideaTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditEvideaTrasnsferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditEvideaTrasnsferedEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("Evidea", begdate, enddate);
            List<net_Evidea> evideaList = new List<net_Evidea>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Evidea));
                    net_Evidea evidea = (net_Evidea) serializer.Deserialize(stringReader);
                    evidea.LineNr = lineCount;
                    evideaList.Add(evidea);
                }

                lineCount++;
            }

            gridEvideaTransfered.DataSource = evideaList;
        }

        private void btnGetEvideaTransferedOrders_Click(object sender, EventArgs e)
        {
            EvideaTransferedOrders();
        }

        private void tabEvidea_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabEvidea.SelectedTabPage == pageEvideaWaiting)
            {
                EvideaWaitngOrders();
            }
            else if (tabEvidea.SelectedTabPage == pageEvideaTransfered)
            {
                EvideaTransferedOrders();
            }
        }

        #endregion

        #region Dekorazon

        private void btnSelectDekExcel_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm";
                openFileDialog1.ShowDialog();
                btnSelectDekExcel.Text = openFileDialog1.FileName;
                if (!string.IsNullOrEmpty(btnSelectDekExcel.Text))
                {
                    ssExcel.Document.LoadDocument(btnSelectDekExcel.Text);
                    Worksheet sheet = ssExcel.Document.Worksheets[0];
                    List<net_Dekorazon> dekList = new List<net_Dekorazon>();
                    int lastRow = sheet.GetUsedRange().BottomRowIndex + 1;
                    for (int i = 1; i < lastRow; i++)
                    {
                        DateTime dateTest = DateTime.Today;
                        if (DateTime.TryParse(sheet.Rows[i][8].Value.ToString(), out dateTest))
                        {
                            string Barcode = sheet.Rows[i][3].Value.ToString().Trim();
                            string itemcode = _databaseLayer.GetItemCode(Barcode);
                            long startDate =
                                (long) DateTimeToTimestamp(Convert.ToDateTime(sheet.Rows[i][8].Value.ToString()));
                            string Orderlinenr = startDate.ToString();
                            net_Dekorazon dekorazon = new net_Dekorazon()
                            {
                                Addr = sheet.Rows[i][11].Value.ToString(),
                                Amount = Convert.ToDouble(sheet.Rows[i][17].Value.ToString().Replace(".", ",")),
                                Date_ = DateTime.Today,
                                ItemCode = sheet.Rows[i][1].Value.ToString(),
                                Phone = sheet.Rows[i][10].Value.ToString(),
                                Price = Convert.ToDouble(sheet.Rows[i][19].Value.ToString().Replace(".", ",")),
                                SiparisNo = sheet.Rows[i][0].Value.ToString(),
                                ItemName = sheet.Rows[i][15].Value.ToString(),
                                Barcode = sheet.Rows[i][14].Value.ToString(),
                                OrderLineNr = startDate,
                                ShipInfoName = sheet.Rows[i][9].Value.ToString().ToUpper(),
                                LogoCode = _databaseLayer.GetItemCode(sheet.Rows[i][14].Value.ToString()),
                                LineNr = i
                            };

                            if (_databaseLayer.GetOrderStatus(
                                "Evidea",
                                dekorazon.SiparisNo,
                                dekorazon.OrderLineNr.ToString()))
                            {
                                dekorazon.Statu = "Logoya Aktarılmış";
                            }

                            dekList.Add(dekorazon);
                        }
                    }

                    gridDekorazonWaiting.DataSource = dekList;
                }
                else
                {
                    MessageBox.Show("Excel Dosyası Seçiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public double DateTimeToTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMinutes;
        }

        private void btnDekorazonTransfer_Click(object sender, EventArgs e)
        {
            DekorazonLayer dekorazonLayer = new DekorazonLayer();
            int[] RowHandles = gvDekorazonWaiting.GetSelectedRows();
            List<int> deletingRows = new List<int>();
            foreach (var i in RowHandles)
            {
                string result = "";
                net_Dekorazon dekorazon = (net_Dekorazon) gvDekorazonWaiting.GetRow(i);
                net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                if (dekorazonLayer.transferOrders(dekorazon, out result))
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Dekorazon));
                        serializer.Serialize(stringwriter, dekorazon);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "Dekorazon";
                    entegratedOrder.OrderDate = dekorazon.Date_;
                    entegratedOrder.OrderNo = dekorazon.SiparisNo;
                    entegratedOrder.LineNr = dekorazon.OrderLineNr.ToString();
                    entegratedOrder.LogoFicheNo = result;
                    entegratedOrder.Transfered = 1;
                    deletingRows.Add(i);
                }
                else
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Dekorazon));
                        serializer.Serialize(stringwriter, dekorazon);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "Dekorazon";
                    entegratedOrder.OrderDate = dekorazon.Date_;
                    entegratedOrder.OrderNo = dekorazon.SiparisNo;
                    entegratedOrder.LineNr = dekorazon.OrderLineNr.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = result;
                    gvDekorazonWaiting.SetRowCellValue(i, "Status", result);
                }

                _databaseLayer.UpdateOrders(entegratedOrder);
            }

            foreach (var i in deletingRows.OrderByDescending(x => x))
            {
                gvDekorazonWaiting.DeleteRow(i);
            }

            gvDekorazonWaiting.ClearSelection();
        }

        private void DekorazonWaitngOrders()
        {
            DateTime begdate = DateTime.Today.AddDays(-100);
            DateTime enddate = DateTime.Today;
            var list = _databaseLayer.GetWaitingOrders("Dekorazon", begdate, enddate);
            List<net_Dekorazon> dekorazonList = new List<net_Dekorazon>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Dekorazon));
                    net_Dekorazon dekorazon = (net_Dekorazon) serializer.Deserialize(stringReader);
                    dekorazon.LineNr = lineCount;
                    dekorazonList.Add(dekorazon);
                }

                lineCount++;
            }

            gridDekorazonWaiting.DataSource = dekorazonList;
        }

        private void DekorazonTransferedOrders()
        {
            DateTime begdate = Convert.ToDateTime(dateEditDekTrasnsferedBegDate.EditValue);
            DateTime enddate = Convert.ToDateTime(dateEditDekTrasnsferedEndDate.EditValue).AddDays(1);
            var list = _databaseLayer.GetTransferedOrders("Dekorazon", begdate, enddate);
            List<net_Dekorazon> dekorazonList = new List<net_Dekorazon>();
            int lineCount = 1;
            foreach (var item in list)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_Dekorazon));
                    net_Dekorazon dekorazon = (net_Dekorazon) serializer.Deserialize(stringReader);
                    dekorazon.LineNr = lineCount;
                    dekorazonList.Add(dekorazon);
                }

                lineCount++;
            }

            gridDekorazonTransfered.DataSource = dekorazonList;
        }

        private void btnGetDekTransferedOrders_Click(object sender, EventArgs e)
        {
            DekorazonTransferedOrders();
        }

        private void tabDekorazon_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tabDekorazon.SelectedTabPage == pageDekWaiting)
            {
                DekorazonWaitngOrders();
            }

            if (tabDekorazon.SelectedTabPage == pageDekTransfered)
            {
                DekorazonTransferedOrders();
            }
        }

        #endregion

        #region Altıncı Cadde

        private void btnSelectCaddeExcel_ButtonClick(
            object sender,
            DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm";
                openFileDialog1.ShowDialog();
                btnSelectCaddeExcel.Text = openFileDialog1.FileName;
                if (!string.IsNullOrEmpty(btnSelectCaddeExcel.Text))
                {
                    ssExcel.Document.LoadDocument(btnSelectCaddeExcel.Text);
                    Worksheet sheet = ssExcel.Document.Worksheets[0];
                    List<net_AltinciCadde> caddeList = new List<net_AltinciCadde>();
                    int lastRow = sheet.GetUsedRange().BottomRowIndex + 1;
                    for (int i = 1; i < lastRow; i++)
                    {
                        DateTime dateTest = DateTime.Today;
                        if (DateTime.TryParse(sheet.Rows[i][4].Value.ToString(), out dateTest))
                        {
                            string Barcode = sheet.Rows[i][12].Value.ToString().Trim();
                            string itemcode = _databaseLayer.GetItemCode(Barcode);
                            long startDate =
                                (long) DateTimeToTimestamp(Convert.ToDateTime(sheet.Rows[i][4].Value.ToString()));
                            string Orderlinenr = startDate.ToString();
                            net_AltinciCadde altinciCadde = new net_AltinciCadde()
                            {
                                AltinciCaddeCode = sheet.Rows[i][0].Value.ToString(),
                                Barcode = Barcode,
                                Date_ = dateTest,
                                DueDate = Convert.ToDateTime(sheet.Rows[i][16].Value.ToString()),
                                LineNr = i,
                                OrderLineNr = startDate,
                                OrderNumber = sheet.Rows[i][1].Value.ToString(),
                                Price = Convert.ToDouble(sheet.Rows[i][9].Value.ToString().Replace(".", ",")) /
                                        Convert.ToDouble(sheet.Rows[i][7].Value.ToString().Replace(".", ",")),
                                ProductCode = itemcode,
                                ProductName = sheet.Rows[i][6].Value.ToString(),
                                Quantity = Convert.ToDouble(sheet.Rows[i][7].Value.ToString().Replace(".", ",")),
                                SasNo = sheet.Rows[i][3].Value.ToString()
                            };

                            if (_databaseLayer.GetOrderStatus(
                                "AltinciCadde",
                                altinciCadde.OrderNumber,
                                altinciCadde.OrderLineNr.ToString()))
                            {
                                altinciCadde.Statu = "Logoya Aktarılmış";
                            }

                            caddeList.Add(altinciCadde);
                        }
                    }

                    gridAltinciCaddeWaiting.DataSource = caddeList;
                }
                else
                {
                    MessageBox.Show("Excel Dosyası Seçiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnAltinciCaddeTransfer_Click(object sender, EventArgs e)
        {
            AltinciCaddeLayer dekorazonLayer = new AltinciCaddeLayer();
            int[] RowHandles = gvAltinciCaddeWaiting.GetSelectedRows();
            List<int> deletingRows = new List<int>();
            foreach (var i in RowHandles)
            {
                string result = "";
                net_AltinciCadde cadde = (net_AltinciCadde) gvAltinciCaddeWaiting.GetRow(i);
                net_EntegratedOrders entegratedOrder = new net_EntegratedOrders();
                if (dekorazonLayer.TransferOrders(cadde, out result))
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_AltinciCadde));
                        serializer.Serialize(stringwriter, cadde);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "AltinciCadde";
                    entegratedOrder.OrderDate = cadde.Date_;
                    entegratedOrder.OrderNo = cadde.OrderNumber;
                    entegratedOrder.LineNr = cadde.OrderLineNr.ToString();
                    entegratedOrder.LogoFicheNo = result;
                    entegratedOrder.Transfered = 1;
                    deletingRows.Add(i);
                }
                else
                {
                    using (var stringwriter = new System.IO.StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_AltinciCadde));
                        serializer.Serialize(stringwriter, cadde);
                        string returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    entegratedOrder.EntegrationName = "AltinciCadde";
                    entegratedOrder.OrderDate = cadde.Date_;
                    entegratedOrder.OrderNo = cadde.OrderNumber;
                    entegratedOrder.LineNr = cadde.OrderLineNr.ToString();
                    entegratedOrder.Transfered = 0;
                    entegratedOrder.ResultMsg = result;
                    gvAltinciCaddeWaiting.SetRowCellValue(i, "Status", result);
                }

                _databaseLayer.UpdateOrders(entegratedOrder);
            }

            foreach (var i in deletingRows.OrderByDescending(x => x))
            {
                gvAltinciCaddeWaiting.DeleteRow(i);
            }

            gvAltinciCaddeWaiting.ClearSelection();
        }

        #endregion

        private string GetFolderPath()
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                Environment.SpecialFolder root = folderDlg.RootFolder;
                return folderDlg.SelectedPath;
            }

            return "";
        }

        private void ExportGrid(GridControl grid, string firm)
        {
            var folderPath = $"{GetFolderPath()}";

            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            var time = DateTime.Now;

            var filePath =
                $"{folderPath}\\{firm} {time.Day:00}.{time.Month:00}.{time.Year} {time.Hour}.{time.Minute}.{time.Second}.xls";

            grid.ExportToXls(filePath);
        }

        private void btnExportTrendyol_Click(object sender, EventArgs e)
        {
            ExportGrid(gridTrendyol, "Trendyol");
        }

        private void btnExportHepsiBurada_Click(object sender, EventArgs e)
        {
            ExportGrid(gridHepsiBurada, "Hepsi Burada");
        }

        private void btnExportKoctas_Click(object sender, EventArgs e)
        {
            ExportGrid(gridKoctasWaiting, "Koçtaş");
        }

        private void btnExportN11_Click(object sender, EventArgs e)
        {
            ExportGrid(gridN11Waitings, "N11");
        }

        private void btnExportBoyner_Click(object sender, EventArgs e)
        {
            ExportGrid(gridBoynerWaiting, "Boyner");
        }

        private void btnExportGittiGidiyorWaitingOrders_Click(object sender, EventArgs e)
        {
            ExportGrid(gridControlGittiGidiyor, "GittiGidiyor");
        }

        private void btnExportEvidea_Click(object sender, EventArgs e)
        {
            ExportGrid(gridEvideaWaiting, "Evidea");
        }

        private void btnExportDekorazon_Click(object sender, EventArgs e)
        {
            ExportGrid(gridDekorazonWaiting, "Dekorazon");
        }

        private void btnExportAltinciCadde_Click(object sender, EventArgs e)
        {
            ExportGrid(gridAltinciCaddeWaiting, "Altıncı Cadde");
        }

        private void btnEditChooseExcelAmazon_DoubleClick(object sender, EventArgs e)
        {
            var file = new OpenFileDialog
            {
                Filter = "Excel Dosyası |*.xlsx| Excel Dosyası|*.xls",
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Excel Dosyası Seçiniz.."
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                var filePath = file.FileName;
                var fileName = file.SafeFileName;

                btnEditChooseExcelAmazon.Text = fileName;

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                Excel.Range range;

                int rowCount = 0;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(
                    filePath,
                    0,
                    true,
                    5,
                    "",
                    "",
                    true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
                    "\t",
                    false,
                    false,
                    0,
                    true,
                    1,
                    0);
                xlWorkSheet = (Excel.Worksheet) xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                rowCount = range.Rows.Count;

                var list = new List<net_Amazon>();

                for (var i = 5; i <= rowCount; i++)
                {
                    var amazon = new net_Amazon();

                    amazon.PONumber = (string) (range.Cells[i, 1] as Excel.Range).Value2;
                    amazon.Barcode = ((double) (range.Cells[i, 2] as Excel.Range).Value2).ToString();
                    amazon.ModelNumber = (string) (range.Cells[i, 3] as Excel.Range).Value2;
                    amazon.ProductCode = (string) (range.Cells[i, 4] as Excel.Range).Value2;
                    amazon.ProductName = ((string) (range.Cells[i, 5] as Excel.Range).Value2).Trim();
                    amazon.Price = (float) (range.Cells[i, 10] as Excel.Range).Value2;
                    amazon.Quantity = (float) (range.Cells[i, 11] as Excel.Range).Value2;
                    amazon.OrderDate = DateTime.FromOADate((double) (range.Cells[i, 20] as Excel.Range).Value2);

                    amazon.LogoCode = _databaseLayer.GetLogoItemCodeByBarcode(amazon.Barcode);

                    list.Add(amazon);
                }

                xlWorkBook.Close(false, filePath);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                gridControlAmazon.DataSource = list;

                gridViewAmazon.BestFitColumns();
            }
        }

        private void BtnAmazonTransfer_Click(object sender, EventArgs e)
        {
            var list = (List<net_Amazon>) gridControlAmazon.DataSource;

            var nonReferencedItems = new List<net_Amazon>();

            for (var i = 0; i < list.Count; i++)
            {
                if (string.IsNullOrEmpty(list[i].LogoCode))
                {
                    nonReferencedItems.Add(list[i]);
                }
            }

            if (nonReferencedItems.Any())
            {
                var message =
                    "Aşağıda Belirtilen Malzemelerin Logo'da Karşılıkları Bulunamadı. Yine de Aktarmak İstiyor musunuz?\n\n";

                for (var i = 0; i < nonReferencedItems.Count; i++)
                {
                    var item = nonReferencedItems[i];

                    message = message + $"{item.Barcode} [{item.ProductCode}] {item.ProductName}\n";
                }

                if (MessageBox.Show(
                        message,
                        $"Barkod Bulunamadı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) !=
                    DialogResult.Yes)
                {
                    return;
                }
            }

            var amazonLayer = new AmazonLayer();

            if (amazonLayer.TransferOrder(list, out var resultMessage))
            {
                MessageBox.Show(
                    @"Aktarım Başarı ile Tamamlandı.",
                    @"Aktarım Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Sipariş Aktarılırken Hata Oluştu.\n\n{resultMessage}",
                    @"Aktarım Hatası",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnGittiGidiyorGetOrders_Click(object sender, EventArgs e)
        {
            GittiGidiyorWaitingOrders();
        }

        private void GittiGidiyorWaitingOrders()
        {
            var gittiGidiyorLayer = new GittiGidiyorLayer();

            var begTime = Convert.ToDateTime(dateEditGittiGidiyorStartDate.EditValue).ToString("dd.MM.yyyy") +
                          " " +
                          Convert.ToDateTime(timeEditGittiGidiyorStartTime.EditValue).ToString("HH:mm");
            var endTime = Convert.ToDateTime(dateEditGittiGidiyorEndDate.EditValue).ToString("dd.MM.yyyy") +
                          " " +
                          Convert.ToDateTime(timeEditGittiGidiyorEndTime.EditValue).ToString("HH:mm");

            var begdate = Convert.ToDateTime(begTime);
            var enddate = Convert.ToDateTime(endTime);

            var list = _databaseLayer.GetWaitingOrders("GittiGidiyor", begdate, enddate);

            var gittiGidiyors = new List<net_GittiGidiyor>();

            foreach (var item in list)
            {
                var xmlString = Encoding.UTF8.GetString(item.OrderXml);
                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_GittiGidiyor));
                    var gittiGidiyor = (net_GittiGidiyor) serializer.Deserialize(stringReader);

                    //gittiGidiyor.productBarcode = gittiGidiyorLayer.GetItemBarcodeFromGittiGidiyor(gittiGidiyor.productId);

                    gittiGidiyors.Add(gittiGidiyor);
                }
            }

            gridControlGittiGidiyor.DataSource = gittiGidiyors;
        }

        private byte[] GetXmlString(object item)
        {
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(item.GetType());
                serializer.Serialize(stringwriter, item);
                var returnStr = stringwriter.ToString();
                return Encoding.UTF8.GetBytes(returnStr);
            }
        }

        private void btnTransferGittiGidiyor_Click(object sender, EventArgs e)
        {
            var gittiGidiyorLayer = new GittiGidiyorLayer();

            var selectedRows = gridViewGittiGidiyor.GetSelectedRows();

            var list = new List<net_GittiGidiyor>();

            foreach (var rowHandle in selectedRows)
            {
                list.Add((net_GittiGidiyor) gridViewGittiGidiyor.GetRow(rowHandle));
            }

            foreach (var i in list.GroupBy(x => x.saleCode))
            {
                var orderLines = i.ToList();

                var entegratedOrder = new net_EntegratedOrders();

                var count = 1;

                if (gittiGidiyorLayer.transferOrder(orderLines, out var result))
                {
                    foreach (var gittiGidiyor in orderLines)
                    {
                        entegratedOrder.OrderXml = GetXmlString(gittiGidiyor);

                        entegratedOrder.EntegrationName = "GittiGidiyor";

                        var date = gittiGidiyor.lastActionDate;

                        entegratedOrder.OrderDate = Convert.ToDateTime(
                            $"" +
                            $"{date.Substring(0, 2)}." +
                            $"{date.Substring(3, 2)}." +
                            $"{date.Substring(6, 4)}");

                        entegratedOrder.OrderNo = gittiGidiyor.saleCode;
                        entegratedOrder.Transfered = 0;
                        entegratedOrder.ResultMsg = result;

                        entegratedOrder.LineNr = count.ToString();
                        entegratedOrder.Transfered = 1;

                        _databaseLayer.UpdateOrders(entegratedOrder);

                        count++;
                    }

                    for (var j = selectedRows.Length - 1; j >= 0; j--)
                    {
                        var gittiGidiyor = (net_GittiGidiyor) gridViewGittiGidiyor.GetRow(j);

                        if (gittiGidiyor == null)
                            continue;

                        if (gittiGidiyor.saleCode == orderLines.First().saleCode)
                        {
                            gridViewGittiGidiyor.DeleteRow(j);
                        }
                    }
                }
            }

            gridViewGittiGidiyor.ClearSelection();

            btnGittiGidiyorGetOrders.PerformClick();

            /////////////////////

            //var gittiGidiyorLayer = new GittiGidiyorLayer();
            //var selectedRows = gridViewGittiGidiyor.GetSelectedRows();
            //var deletingRows = new List<int>();

            //foreach (var i in selectedRows)
            //{
            //    var gittiGidiyor = (net_GittiGidiyor) gridViewGittiGidiyor.GetRow(i);

            //    var result = "";

            //    var entegratedOrder = new net_EntegratedOrders();

            //    if (gittiGidiyorLayer.transferOrder(gittiGidiyor, out result))
            //    {
            //        gittiGidiyor.status = "";

            //        entegratedOrder.OrderXml = GetXmlString(gittiGidiyor);

            //        var date = gittiGidiyor.lastActionDate;

            //        entegratedOrder.EntegrationName = "GittiGidiyor";

            //        entegratedOrder.OrderDate = Convert.ToDateTime(
            //            $"" +
            //            $"{date.Substring(0, 2)}." +
            //            $"{date.Substring(3, 2)}." +
            //            $"{date.Substring(6, 4)}");

            //        entegratedOrder.OrderNo = gittiGidiyor.saleCode;
            //        entegratedOrder.LineNr = "1";
            //        entegratedOrder.Transfered = 1;

            //        deletingRows.Add(i);
            //    }
            //    else
            //    {
            //        gittiGidiyor.status = result;

            //        using (var stringwriter = new System.IO.StringWriter())
            //        {
            //            var serializer = new XmlSerializer(typeof(net_GittiGidiyor));
            //            serializer.Serialize(stringwriter, gittiGidiyor);
            //            var returnStr = stringwriter.ToString();
            //            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
            //        }

            //        var date = gittiGidiyor.lastActionDate;

            //        entegratedOrder.EntegrationName = "GittiGidiyor";
            //        entegratedOrder.OrderDate = Convert.ToDateTime(
            //            $"" +
            //            $"{date.Substring(0, 2)}." +
            //            $"{date.Substring(3, 2)}." +
            //            $"{date.Substring(6, 4)}");
            //        entegratedOrder.OrderNo = gittiGidiyor.saleCode;
            //        entegratedOrder.LineNr = "1";
            //        entegratedOrder.Transfered = 0;
            //        entegratedOrder.ResultMsg = result;

            //        gridViewGittiGidiyor.SetRowCellValue(i, "Status", result);
            //    }

            //    _databaseLayer.UpdateOrders(entegratedOrder);
            //}

            //foreach (var i in deletingRows.OrderByDescending(x => x))
            //{
            //    gridViewGittiGidiyor.DeleteRow(i);
            //}

            //gridViewGittiGidiyor.ClearSelection();

            //gridViewGittiGidiyor.BestFitColumns();
        }

        private void btnGittiGidiyorGetTransfered_Click(object sender, EventArgs e)
        {
            var begdate = Convert.ToDateTime(dateEditGittiGidiyorTransferedStartDate.EditValue);
            var enddate = Convert.ToDateTime(dateEditGittiGidiyorTransferedEndDate.EditValue).AddDays(1);

            var list = _databaseLayer.GetTransferedOrders("GittiGidiyor", begdate, enddate);

            var gittiGidiyors = new List<net_GittiGidiyor>();

            foreach (var item in list)
            {
                var xmlString = Encoding.UTF8.GetString(item.OrderXml);

                using (var stringReader = new StringReader(xmlString))
                {
                    var serializer = new XmlSerializer(typeof(net_GittiGidiyor));
                    var gittiGidiyor = (net_GittiGidiyor) serializer.Deserialize(stringReader);
                    gittiGidiyors.Add(gittiGidiyor);
                }
            }

            gridControlGittiGidiyorTransfered.DataSource = gittiGidiyors;
        }

        private void CheckOrderTransferedSign_Click(object sender, EventArgs e)
        {
            var control = ActiveControl.Name;

            switch (control)
            {
                case "gridAltinciCaddeWaiting":
                    var altinciCadde =
                        (net_AltinciCadde) gvAltinciCaddeWaiting.GetRow(gvAltinciCaddeWaiting.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("AltinciCadde", altinciCadde.OrderNumber, true);
                    btnGetTredyolOrders.PerformClick();
                    break;
                case "gridTrendyol":
                    var trendyol = (net_Trendyol) gvTrendyol.GetRow(gvTrendyol.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("Trendyol", trendyol.orderNumber, true);
                    btnGetTredyolOrders.PerformClick();
                    break;
                case "gridTrendyolTransfered":
                    var trendyolTransfered =
                        (net_Trendyol) gvTrendyolTransfered.GetRow(gvTrendyolTransfered.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("Trendyol", trendyolTransfered.orderNumber, false);
                    btnGetTredyolTransferedOrders.PerformClick();
                    break;
                case "gridKoctasWaiting":
                    var koctas = (net_Koctas) gvKoctasWaiting.GetRow(gvKoctasWaiting.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("Koçtaş", koctas.SiparisNo, true);
                    btnGetKTOrders.PerformClick();
                    break;
                case "gridKoctasTransfered":
                    var koctasTransfered = (net_Koctas) gvKoctasTransfered.GetRow(gvKoctasTransfered.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("Koçtaş", koctasTransfered.SiparisNo, false);
                    btnGetKTTransfered.PerformClick();
                    break;
                case "gridN11Waitings":
                    var n11 = (net_N11) gvN11Waitings.GetRow(gvN11Waitings.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("N11", n11.orderNumber, true);
                    btnGetN11Orders.PerformClick();
                    break;
                case "gridN11Transfered":
                    var n11Transfered = (net_N11) gvN11Transfered.GetRow(gvN11Transfered.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("N11", n11Transfered.orderNumber, false);
                    btnGetN11TransferedOrders.PerformClick();
                    break;
                case "gridHepsiBurada":
                    var hepsiBurada = (net_HepsiBurada) gvHepsiBurada.GetRow(gvHepsiBurada.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("HepsiBurada", hepsiBurada.SasNo, true);
                    btnGetHBOrders.PerformClick();
                    break;
                case "gridHBSended":
                    var hepsiBuradaTransfered = (net_HepsiBurada) gvHBSended.GetRow(gvHBSended.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("HepsiBurada", hepsiBuradaTransfered.SasNo, false);
                    btnGetHBTransfered.PerformClick();
                    break;
                case "gridControlGittiGidiyor":
                    var gittiGidiyor =
                        (net_GittiGidiyor) gridViewGittiGidiyor.GetRow(gridViewGittiGidiyor.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("GittiGidiyor", gittiGidiyor.saleCode, true);
                    btnGittiGidiyorGetOrders.PerformClick();
                    break;
                case "gridControlGittiGidiyorTransfered":
                    var gittiGidiyorTransfered =
                        (net_GittiGidiyor) gridViewGittiGidiyorTransfered.GetRow(gridViewGittiGidiyorTransfered
                            .FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("GittiGidiyor", gittiGidiyorTransfered.saleCode, false);
                    btnGittiGidiyorGetTransfered.PerformClick();
                    break;
                case "gridBoynerTransfered":
                    var boynerTransfered =
                        (net_Boyner) gvBoynerTransfered.GetRow(gvBoynerTransfered.FocusedRowHandle);
                    _databaseLayer.UpdateTransferedSign("Boyner", boynerTransfered.OrderCode, false);
                    btnGetBoynerTransferedOrders.PerformClick();
                    break;
            }
        }

        private void BtnEditChooseExcelKoctas_DoubleClick(object sender, EventArgs e)
        {
            var file = new OpenFileDialog
            {
                Filter = "Excel Dosyası |*.xlsx|Excel Dosyası|*.xls",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = "Excel Dosyası Seçiniz.."
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                var filePath = file.FileName;
                var fileName = file.SafeFileName;

                btnEditChooseExcelKoctas.Text = fileName;

                ssExcel.Document.LoadDocument(filePath);
                var sheet = ssExcel.Document.Worksheets[0];

                var rowCount = sheet.GetUsedRange().BottomRowIndex + 1;

                var list = new List<net_Koctas>();

                for (var i = 2; i <= rowCount; i++)
                {
                    var koctas = new net_Koctas();

                    koctas.SiparisNo = sheet.Rows[i][0].Value.ToString();

                    if (string.IsNullOrEmpty(koctas.SiparisNo))
                        continue;

                    koctas.Date_ = Convert.ToDateTime((sheet.Rows[i][1].Value).ToString());
                    koctas.LineNr = Convert.ToInt32((sheet.Rows[i][2].Value).ToString());
                    koctas.ItemCode = ((sheet.Rows[i][4].Value).ToString());
                    koctas.Barcode = ((sheet.Rows[i][5].Value).ToString());
                    koctas.ItemName = ((sheet.Rows[i][6].Value).ToString());
                    koctas.DueDate = Convert.ToDateTime(((sheet.Rows[i][8].Value).ToString()));
                    koctas.Amount = Convert.ToDouble(((sheet.Rows[i][10].Value).ToString()));
                    koctas.ShipInfoName = ((sheet.Rows[i][25].Value).ToString());
                    koctas.Addr = ((sheet.Rows[i][27].Value).ToString());
                    koctas.Price = Convert.ToDouble(((sheet.Rows[i][21].Value).ToString()));
                    koctas.Phone = ((sheet.Rows[i][26].Value).ToString());

                    koctas.LogoCode = _databaseLayer.GetLogoItemCodeByBarcode(koctas.Barcode);

                    list.Add(koctas);
                }

                gridControlKoctasExcelTransfer.DataSource = list;

                gridViewKoctasExcelTransfer.BestFitColumns();
            }
        }

        private void BtnKoctacExcelTransfer_Click(object sender, EventArgs e)
        {
            var list = (List<net_Koctas>) gridControlKoctasExcelTransfer.DataSource;

            var nonReferencedItems = new List<net_Koctas>();

            foreach (var t in list)
                if (string.IsNullOrEmpty(t.LogoCode))
                    nonReferencedItems.Add(t);

            if (nonReferencedItems.Any())
            {
                var message =
                    "Aşağıda Belirtilen Malzemelerin Logo'da Karşılıkları Bulunamadı. Yine de Aktarmak İstiyor musunuz?\n\n";

                foreach (var item in nonReferencedItems)
                    message += $"{item.Barcode} [{item.ItemCode}] {item.ItemName}\n";

                if (MessageBox.Show(
                        message,
                        $"Barkod Bulunamadı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) !=
                    DialogResult.Yes)
                {
                    return;
                }
            }

            var koctasLayer = new KoctasLayer();

            foreach (var i in list.GroupBy(x => x.SiparisNo))
            {
                var orderLines = i.ToList();

                if (!koctasLayer.OrderTransfer(orderLines, out var result))
                {
                    foreach (var koctas in orderLines)
                    {
                        var entegratedOrder = new net_EntegratedOrders
                        {
                            EntegrationName = "Koçtaş",
                            OrderDate = koctas.Date_,
                            OrderNo = koctas.SiparisNo,
                            LineNr = koctas.LineNr.ToString(),
                            Transfered = 0,
                            ResultMsg = result
                        };

                        using (var stringwriter = new StringWriter())
                        {
                            var serializer = new XmlSerializer(typeof(net_Koctas));
                            serializer.Serialize(stringwriter, koctas);
                            var returnStr = stringwriter.ToString();
                            entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                        }

                        _databaseLayer.UpdateOrders(entegratedOrder);
                    }
                }

                foreach (var koctas in orderLines)
                {
                    var entegratedOrder = new net_EntegratedOrders
                    {
                        EntegrationName = "Koçtaş",
                        OrderDate = koctas.Date_,
                        OrderNo = koctas.SiparisNo,
                        LineNr = koctas.LineNr.ToString(),
                        Transfered = 1,
                        ResultMsg = ""
                    };

                    using (var stringwriter = new StringWriter())
                    {
                        var serializer = new XmlSerializer(typeof(net_Koctas));
                        serializer.Serialize(stringwriter, koctas);
                        var returnStr = stringwriter.ToString();
                        entegratedOrder.OrderXml = Encoding.UTF8.GetBytes(returnStr);
                    }

                    _databaseLayer.UpdateOrders(entegratedOrder);
                }
            }

            gridControlKoctasExcelTransfer.DataSource = null;
        }

        private void GridViewKoctasExcelTransfer_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.MenuType == GridMenuType.Row)
            {
                int rowHandle = e.HitInfo.RowHandle;
                DXMenuItem productPairin = btnKTProductPairingExcel(view, rowHandle);
                e.Menu.Items.Add(productPairin);
            }
        }

        private void btnSelectBoynerExcel_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void updateTransferSignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckOrderTransferedSign_Click(sender, e);
        }

        private void GetSelectedCellValue(GridView gridView, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (gridView.GetRowCellValue(gridView.FocusedRowHandle, gridView.FocusedColumn) != null &&
                    gridView.GetRowCellValue(gridView.FocusedRowHandle, gridView.FocusedColumn).ToString() !=
                    String.Empty)
                {
                    var selectedCellValue = gridView.GetRowCellValue(
                        gridView.FocusedRowHandle,
                        gridView.FocusedColumn).ToString();
                    Clipboard.SetText(selectedCellValue);
                }
            }
        }

        private void gridN11Waitings_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvN11Waitings, e);
        }

        private void gridN11Transfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvN11Transfered, e);
        }

        private void gridHepsiBurada_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvHepsiBurada, e);
        }

        private void gridHBSended_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvHBSended, e);
        }

        private void gridTrendyol_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvTrendyol, e);
        }

        private void gridTrendyolTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvTrendyolTransfered, e);
        }

        private void gridKoctasWaiting_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvKoctasWaiting, e);
        }

        private void gridKoctasTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvKoctasTransfered, e);
        }

        private void gridControlKoctasExcelTransfer_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gridViewKoctasExcelTransfer, e);
        }

        private void gridControlGittiGidiyor_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gridViewGittiGidiyor, e);
        }

        private void gridControlGittiGidiyorTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gridViewGittiGidiyorTransfered, e);
        }

        private void gridBoynerWaiting_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvBoynerWaiting, e);
        }

        private void gridBoynerTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvBoynerTransfered, e);
        }

        private void gridEvideaWaiting_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvEvideaWaiting, e);
        }

        private void gridEvideaTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvEvideaTransfered, e);
        }

        private void gridDekorazonWaiting_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvDekorazonWaiting, e);
        }

        private void gridDekorazonTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvDekorazonTransfered, e);
        }

        private void gridAltinciCaddeWaiting_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvAltinciCaddeWaiting, e);
        }

        private void gridAltinciCaddeTransfered_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gvAltinciCaddeTransfered, e);
        }

        private void gridControlAmazon_KeyUp(object sender, KeyEventArgs e)
        {
            GetSelectedCellValue(gridViewAmazon, e);
        }
    }
}