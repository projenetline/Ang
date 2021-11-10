namespace PazarYeri.UI
{
    partial class frmProductList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridProducts = new DevExpress.XtraGrid.GridControl();
            this.gvProducts = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridProducts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvProducts)).BeginInit();
            this.SuspendLayout();
            // 
            // gridProducts
            // 
            this.gridProducts.Location = new System.Drawing.Point(12, 12);
            this.gridProducts.MainView = this.gvProducts;
            this.gridProducts.Name = "gridProducts";
            this.gridProducts.Size = new System.Drawing.Size(620, 237);
            this.gridProducts.TabIndex = 0;
            this.gridProducts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvProducts});
            this.gridProducts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gridProducts_MouseDoubleClick);
            // 
            // gvProducts
            // 
            this.gvProducts.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gvProducts.GridControl = this.gridProducts;
            this.gvProducts.Name = "gvProducts";
            this.gvProducts.OptionsView.ShowAutoFilterRow = true;
            this.gvProducts.OptionsView.ShowGroupExpandCollapseButtons = false;
            this.gvProducts.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Malzeme Kodu";
            this.gridColumn1.FieldName = "ItemCode";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 322;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Malzeme Açıklması";
            this.gridColumn2.FieldName = "ItemName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 655;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Barkod";
            this.gridColumn3.FieldName = "Barcode";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 655;
            // 
            // frmProductList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 261);
            this.Controls.Add(this.gridProducts);
            this.Name = "frmProductList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmProductList";
            this.Load += new System.EventHandler(this.frmProductList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridProducts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvProducts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridProducts;
        private DevExpress.XtraGrid.Views.Grid.GridView gvProducts;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
    }
}