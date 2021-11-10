using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using PazarYeri.Models;

namespace PazarYeri.Sys
{
    public partial class frmSYS : Form
    {
        private readonly ProjectUtil _util;
        private readonly DatabaseLayer _databaseLayer;

        public frmSYS()
        {
            InitializeComponent();

            _util = new ProjectUtil();
            _databaseLayer = new DatabaseLayer();
        }

        private void frmSYS_Load(object sender, EventArgs e)
        {
            this.Text = $@"{this.Text} - v.{Application.ProductVersion}";

            #region Sunucu Ayarları

            var settings = _util.getSetting();

            txtServerName.Text = settings.ServerName;
            txtDBName.Text = settings.DatabaseName;
            txtSqlUser.Text = settings.UserName;
            txtSqlPassword.Text = settings.Password;
            numControlTime.EditValue = settings.ControlTime;
            teLicenceKey.Text = settings.LicenceKey;

            var logoSettings = _databaseLayer.GetLogoSettings();

            if (logoSettings != null)
            {
                txtLogoServer.Text = logoSettings.LogoServerName;
                txtLogoDatabase.Text = logoSettings.LogoDatabase;
                txtLogoUserName.Text = logoSettings.LogoUserName;
                txtLogoPassword.Text = logoSettings.LogoPassword;
                numFirmNr.EditValue = logoSettings.FirmNr;
                numPeriodNr.EditValue = logoSettings.PeriodNr;
                chkboxAutoTransfer.Checked = logoSettings.AutoTransfer;
            }

            #endregion

            #region Aktarım Ayarları

            var items = new List<ComboboxItem>()
            {
                new ComboboxItem()
                {
                    Text = "Öneri",
                    Value = 1,
                },
                new ComboboxItem()
                {
                    Text = "Sevk Edilebilir",
                    Value = 4,
                }
            };
            comboBoxOrderTransferStatus.Properties.Items.AddRange(items);

            var entegrationNames = new[]
                {"N11", "HepsiBurada", "Koçtaş", "Trendyol", "GittiGidiyor", "Boyner", "Evidea", "Dekorazon", "AltinciCadde", "Amazon"};

            //string[] entegrationNames = new[] { "N11"};
            foreach (var entegration in entegrationNames.OrderBy(x => x))
            {
                cboxEntegrationName.Properties.Items.Add(entegration);
            }

            cboxEntegrationName.SelectedIndex = 0;

            var entegrationSettings = _databaseLayer.GetEntegrationSettings(cboxEntegrationName.Text);

            if (entegrationSettings != null)
            {
                num_Id.Text = entegrationSettings.Id.ToString();
                txtSupplierCode.Text = entegrationSettings.FirmCode;
                txtUserName.Text = entegrationSettings.UserName;
                txtPassword.Text = entegrationSettings.PassWord;
                chkBox_Excel.Checked = entegrationSettings.Excel;
                chkBox_WS.Checked = entegrationSettings.WebService;
            }
            else
            {
                num_Id.Text = "0";
            }

            if (entegrationSettings == null)
            {
                num_Id.Text = "0";
            }

            #endregion
        }

        private void btnHB_Save_Click(object sender, EventArgs e)
        {
            var entegrationName = cboxEntegrationName.Text;

            var net_Entegration = new net_EntegrationSettings()
            {
                EntegrationName = cboxEntegrationName.Text,
                Excel = chkBox_Excel.Checked,
                WebService = chkBox_WS.Checked,
                FirmCode = txtSupplierCode.Text,
                Id = Convert.ToInt32(num_Id.Text),
                PassWord = txtPassword.Text,
                UserName = txtUserName.Text
            };

            if (!_databaseLayer.saveEntegrationSettings(net_Entegration))
            {
                MessageBox.Show(@"Ayarlarınız Kayıt Edilemedi.");
            }
            else
            {
                var transferSettings = new net_LogoTransferSettings()
                {
                    ClientTransferPrefixCode = teClientTransferPrefix.Text,
                    ClientTransferSpeCode1 = teClientTransferSpeCode1.Text,
                    ClientTransferSpeCode2 = teClientTransferSpeCode2.Text,
                    ClientTransferSpeCode3 = teClientTransferSpeCode3.Text,
                    ClientTransferSpeCode4 = teClientTransferSpeCode4.Text,
                    ClientTransferSpeCode5 = teClientTransferSpeCode5.Text,
                    ClientTransferAccountCode = teClientTransferAccountCode.Text,
                    ClientTransferPaymentCode = teClientTransferPaymentCode.Text,
                    ClientTransferTradingGroup = teClientTransferTradingGroup.Text,
                    ClientTransferProjectCode = teClientTransferProjectCode.Text,
                    ClientTransferAuthCode = teClientTransferAuthCode.Text,

                    TransferFicheProjectCode = teTransferFicheProjectCode.Text,

                    OrderTransferWareHouseNr = Convert.ToInt16(neOrderTransferWareHouseNr.EditValue),
                    OrderTransferDivisionNr = Convert.ToInt16(neOrderTransferDivisionNr.EditValue),
                    OrderTransferUnitPriceRoundingNumberOfDigits = Convert.ToInt16(neOrderTransferUnitPriceRoundingNumberOfDigits.EditValue),

                    OrderTransferStatus = (comboBoxOrderTransferStatus.SelectedItem as ComboboxItem).Value,

                    OrderTransferRetransferTransferedOrder = checkEditOrderTransferRetransferTransferedOrder.Checked,
                    OrderTransferAddRowsIfOrder = checkEditOrderTransferAddRowsIfOrder.Checked,
                    OrderTransferGroupByOrderNumber = checkEditOrderTransferGroupByOrderNumber.Checked,
                    OrderTransferTransferToShippingAddress = checkEditOrderTransferTransferToShippingAddress.Checked,

                    OrderTransferTradingGroup = teOrderTransferTradingGroup.Text,
                    OrderTransferProjectCode = teOrderTransferProjectCode.Text,
                    OrderTransferDiscountCouponCode = teOrderTransferDiscountCouponCode.Text,
                    OrderTransferServiceChargeCode = teOrderTransferServiceChargeCode.Text,
                    OrderTransferLateChargeCode = teOrderTransferLateChargeCode.Text,
                    OrderTransferSalesManCode = teOrderTransferSalesManCode.Text,
                    OrderTransferDoCode = teOrderTransferDoCode.Text,
                    OrderTransferSpeCode = teOrderTransferSpeCode.Text,
                    OrderTransferPaymentCode = teOrderTransferPaymentCode.Text,
                    OrderTransferArpShippmentCode = teOrderTransferArpShippmentCode.Text,
                    OrderTransferArpCode = teOrderTransferArpCode.Text,
                    OrderTransferAuthCode = teOrderTransferAuthCode.Text,

                    EntegrationName = entegrationName
                };

                MessageBox.Show(
                    _databaseLayer.saveLogoTransferSettings(transferSettings)
                        ? @"Ayarlarınız Kayıt Edildi."
                        : @"Logo Aktarım Ayarlarınız Kayıt Edilemedi.");
            }
        }

        private void cboxEntegrationName_SelectedIndexChanged(object sender, EventArgs e)
        {
            void ClearFields()
            {
                txtSupplierCode.Text = "";
                txtUserName.Text = "";
                txtPassword.Text = "";
                num_Id.Text = "";
                chkBox_Excel.Checked = false;
                chkBox_WS.Checked = false;

                teClientTransferPrefix.Text = "";
                teClientTransferSpeCode1.Text = "";
                teClientTransferSpeCode2.Text = "";
                teClientTransferSpeCode3.Text = "";
                teClientTransferSpeCode4.Text = "";
                teClientTransferSpeCode5.Text = "";
                teClientTransferAccountCode.Text = "";
                teClientTransferPaymentCode.Text = "";
                teClientTransferTradingGroup.Text = "";
                teClientTransferProjectCode.Text = "";

                teTransferFicheProjectCode.Text = "";

                neOrderTransferWareHouseNr.EditValue = 0;
                neOrderTransferDivisionNr.EditValue = 0;
                neOrderTransferUnitPriceRoundingNumberOfDigits.EditValue = 2;

                comboBoxOrderTransferStatus.SelectedIndex = 0;

                checkEditOrderTransferRetransferTransferedOrder.Checked = false;
                checkEditOrderTransferAddRowsIfOrder.Checked = false;
                checkEditOrderTransferGroupByOrderNumber.Checked = false;
                checkEditOrderTransferTransferToShippingAddress.Checked = false;

                teOrderTransferTradingGroup.Text = "";
                teOrderTransferProjectCode.Text = "";
                teOrderTransferDiscountCouponCode.Text = "";
                teOrderTransferServiceChargeCode.Text = "";
                teOrderTransferLateChargeCode.Text = "";
                teOrderTransferSalesManCode.Text = "";
                teOrderTransferDoCode.Text = "";
                teOrderTransferSpeCode.Text = "";
                teOrderTransferPaymentCode.Text = "";
                teOrderTransferArpShippmentCode.Text = "";
                teOrderTransferArpCode.Text = "";
            }

            ClearFields();

            var entegrationSettings = _databaseLayer.GetEntegrationSettings(cboxEntegrationName.Text);

            if (entegrationSettings != null)
            {
                txtSupplierCode.Text = entegrationSettings.FirmCode;
                txtUserName.Text = entegrationSettings.UserName;
                txtPassword.Text = entegrationSettings.PassWord;
                chkBox_Excel.Checked = entegrationSettings.Excel;
                chkBox_WS.Checked = entegrationSettings.WebService;
                num_Id.Text = entegrationSettings.Id.ToString();
            }
            else
            {
                num_Id.Text = "0";
            }

            var logoTransferSettings = _databaseLayer.GetLogoTransferSettings(cboxEntegrationName.Text);

            if (logoTransferSettings != null)
            {
                teClientTransferPrefix.Text = logoTransferSettings.ClientTransferPrefixCode;
                teClientTransferSpeCode1.Text = logoTransferSettings.ClientTransferSpeCode1;
                teClientTransferSpeCode2.Text = logoTransferSettings.ClientTransferSpeCode2;
                teClientTransferSpeCode3.Text = logoTransferSettings.ClientTransferSpeCode3;
                teClientTransferSpeCode4.Text = logoTransferSettings.ClientTransferSpeCode4;
                teClientTransferSpeCode5.Text = logoTransferSettings.ClientTransferSpeCode5;
                teClientTransferAccountCode.Text = logoTransferSettings.ClientTransferAccountCode;
                teClientTransferPaymentCode.Text = logoTransferSettings.ClientTransferPaymentCode;
                teClientTransferTradingGroup.Text = logoTransferSettings.ClientTransferTradingGroup;
                teClientTransferProjectCode.Text = logoTransferSettings.ClientTransferProjectCode;
                teClientTransferAuthCode.Text = logoTransferSettings.ClientTransferAuthCode;

                teTransferFicheProjectCode.Text = logoTransferSettings.TransferFicheProjectCode;

                neOrderTransferWareHouseNr.EditValue = logoTransferSettings.OrderTransferWareHouseNr;
                neOrderTransferDivisionNr.EditValue = logoTransferSettings.OrderTransferDivisionNr;
                neOrderTransferUnitPriceRoundingNumberOfDigits.EditValue = logoTransferSettings.OrderTransferUnitPriceRoundingNumberOfDigits;

                comboBoxOrderTransferStatus.SelectedIndex = logoTransferSettings.OrderTransferStatus == 1 ? 0 : 1;

                checkEditOrderTransferAddRowsIfOrder.Checked = logoTransferSettings.OrderTransferAddRowsIfOrder;
                checkEditOrderTransferRetransferTransferedOrder.Checked = logoTransferSettings.OrderTransferRetransferTransferedOrder;
                checkEditOrderTransferGroupByOrderNumber.Checked = logoTransferSettings.OrderTransferGroupByOrderNumber;
                checkEditOrderTransferTransferToShippingAddress.Checked = logoTransferSettings.OrderTransferTransferToShippingAddress;

                teOrderTransferTradingGroup.Text = logoTransferSettings.OrderTransferTradingGroup;
                teOrderTransferProjectCode.Text = logoTransferSettings.OrderTransferProjectCode;
                teOrderTransferDiscountCouponCode.Text = logoTransferSettings.OrderTransferDiscountCouponCode;
                teOrderTransferServiceChargeCode.Text = logoTransferSettings.OrderTransferServiceChargeCode;
                teOrderTransferLateChargeCode.Text = logoTransferSettings.OrderTransferLateChargeCode;
                teOrderTransferSalesManCode.Text = logoTransferSettings.OrderTransferSalesManCode;
                teOrderTransferDoCode.Text = logoTransferSettings.OrderTransferDoCode;
                teOrderTransferSpeCode.Text = logoTransferSettings.OrderTransferSpeCode;
                teOrderTransferPaymentCode.Text = logoTransferSettings.OrderTransferPaymentCode;
                teOrderTransferArpShippmentCode.Text = logoTransferSettings.OrderTransferArpShippmentCode;
                teOrderTransferArpCode.Text = logoTransferSettings.OrderTransferArpCode;
                teOrderTransferAuthCode.Text = logoTransferSettings.OrderTransferAuthCode;
            }
        }

        private void btnServerSave_Click(object sender, EventArgs e)
        {
            var serverSettings = new net_ServerSettings()
            {
                ControlTime = Convert.ToInt16(numControlTime.EditValue),
                DatabaseName = txtDBName.Text,
                Password = txtSqlPassword.Text,
                ServerName = txtServerName.Text,
                UserName = txtSqlUser.Text,
                LicenceKey = teLicenceKey.Text
            };

            _util.SaveSettings(serverSettings);

            if (!_databaseLayer.checkDatabase(serverSettings))
            {
                MessageBox.Show("Veri tabanı oluşturulacaktır..");
                _databaseLayer.createDataBase(serverSettings);
                if (_databaseLayer.checkDatabase(serverSettings))
                {
                    MessageBox.Show("Tablolar oluşturulacaktır..");
                    _databaseLayer.createTables();
                }
            }

            var logoSettings = new net_LogoSettings()
            {
                AutoTransfer = chkboxAutoTransfer.Checked,
                FirmNr = Convert.ToInt16(numFirmNr.EditValue),
                LogoDatabase = txtLogoDatabase.Text,
                LogoPassword = txtLogoPassword.Text,
                LogoServerName = txtLogoServer.Text,
                LogoUserName = txtLogoUserName.Text,
                PeriodNr = Convert.ToInt16(numPeriodNr.EditValue),
            };

            if (_databaseLayer.saveLogoSettings(logoSettings))
            {
                MessageBox.Show("Ayarlarınız Kayıt Edildi.");
            }
            else
            {
                MessageBox.Show("Ayarlarınız Kayıt Edilemedi.");
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)
            {
                var abs = Convert.ToInt16(txtPassword.UseSystemPasswordChar) - 1;

                txtPassword.UseSystemPasswordChar = Convert.ToBoolean(Math.Abs(abs));
            }
        }

        private void txtSqlPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)
            {
                var abs = Convert.ToInt16(txtSqlPassword.Properties.UseSystemPasswordChar) - 1;

                txtSqlPassword.Properties.UseSystemPasswordChar = Convert.ToBoolean(Math.Abs(abs));
            }
        }

        private void txtLogoPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)
            {
                var abs = Convert.ToInt16(txtLogoPassword.Properties.UseSystemPasswordChar) - 1;

                txtLogoPassword.Properties.UseSystemPasswordChar = Convert.ToBoolean(Math.Abs(abs));
            }
        }

        private void teLicenceKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F1)
            {
                var abs = Convert.ToInt16(teLicenceKey.Properties.UseSystemPasswordChar) - 1;

                teLicenceKey.Properties.UseSystemPasswordChar = Convert.ToBoolean(Math.Abs(abs));
            }
        }
    }
}