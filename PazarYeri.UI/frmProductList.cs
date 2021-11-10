using PazarYeri.BusinessLayer.Utility;
using System;
using System.Windows.Forms;

namespace PazarYeri.UI
{
    public partial class frmProductList : DevExpress.XtraEditors.XtraForm
    {
        public frmProductList()
        {
            InitializeComponent();
        }

        private void gridProducts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string Code = gvProducts.GetRowCellDisplayText(gvProducts.FocusedRowHandle, gvProducts.Columns["ItemCode"]);
        }
        DatabaseLayer DatabaseLayer = new DatabaseLayer();
        private void frmProductList_Load(object sender, EventArgs e)
        {

        }
    }
}