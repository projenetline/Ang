using PazarYeri.BusinessLayer.Utility;
using PazarYeri.Models.Settings;
using System;

namespace PazarYeri.UI
{
    public partial class frmProductPairing : DevExpress.XtraEditors.XtraForm
    {


        net_ProductPairing productPairing = new net_ProductPairing();
        DatabaseLayer dbLayer = new DatabaseLayer();
        public frmProductPairing(string productCode, string productName, string entegrationName)
        {
            InitializeComponent();
            txtHBProductCode.Text = productCode;
            txtProductName.Text = productName;
            productPairing.EntegrationName = entegrationName;
            productPairing.EntegrationCode = productCode;

            this.Text = entegrationName + " " + this.Text;
        }

        private void frmProductPairing_Load(object sender, EventArgs e)
        {
            var pairing = dbLayer.GetProductPairing(productPairing);

            if (pairing != null)
            {
                productPairing.Id = pairing.Id;
                productPairing.LogoCode = pairing.LogoCode;
                productPairing.Barcode = pairing.Barcode;
            }

            txtBarcode.Text = productPairing.Barcode;
            txtLogoCode.Text = productPairing.LogoCode;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtLogoCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            frmProductList frm = new frmProductList();
            frm.ShowDialog();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            productPairing.Barcode = txtBarcode.Text;
            productPairing.LogoCode = txtLogoCode.Text;
            dbLayer.saveProductPairing(productPairing);
            frmPazarYeri.productCode = txtLogoCode.Text;
            this.Close();
        }
    }
}