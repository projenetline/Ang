namespace PazarYeri.UI
{
    partial class frmProductPairing
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
            this.txtHBProductCode = new DevExpress.XtraEditors.TextEdit();
            this.txtProductName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.lblBarcode = new DevExpress.XtraEditors.LabelControl();
            this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
            this.txtLogoCode = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHBProductCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogoCode.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtHBProductCode
            // 
            this.txtHBProductCode.Location = new System.Drawing.Point(119, 12);
            this.txtHBProductCode.Name = "txtHBProductCode";
            this.txtHBProductCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtHBProductCode.Properties.Appearance.Options.UseFont = true;
            this.txtHBProductCode.Size = new System.Drawing.Size(177, 20);
            this.txtHBProductCode.TabIndex = 0;
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(119, 38);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtProductName.Properties.Appearance.Options.UseFont = true;
            this.txtProductName.Size = new System.Drawing.Size(323, 20);
            this.txtProductName.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(12, 15);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(58, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Ürün Kodu";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(12, 41);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Ürün Adı";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(12, 67);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(88, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Logo Ürün Kodu";
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(381, 123);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Kaydet";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnExit.Appearance.Options.UseFont = true;
            this.btnExit.Location = new System.Drawing.Point(300, 123);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Vazgeç";
            this.btnExit.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // lblBarcode
            // 
            this.lblBarcode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBarcode.Appearance.Options.UseFont = true;
            this.lblBarcode.Location = new System.Drawing.Point(12, 93);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(40, 13);
            this.lblBarcode.TabIndex = 9;
            this.lblBarcode.Text = "Barkod";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(119, 90);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtBarcode.Properties.Appearance.Options.UseFont = true;
            this.txtBarcode.Size = new System.Drawing.Size(177, 20);
            this.txtBarcode.TabIndex = 8;
            // 
            // txtLogoCode
            // 
            this.txtLogoCode.Location = new System.Drawing.Point(119, 64);
            this.txtLogoCode.Name = "txtLogoCode";
            this.txtLogoCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtLogoCode.Properties.Appearance.Options.UseFont = true;
            this.txtLogoCode.Size = new System.Drawing.Size(177, 20);
            this.txtLogoCode.TabIndex = 10;
            // 
            // frmProductPairing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 156);
            this.Controls.Add(this.txtLogoCode);
            this.Controls.Add(this.lblBarcode);
            this.Controls.Add(this.txtBarcode);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(this.txtHBProductCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProductPairing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ürün Eşleştirme";
            this.Load += new System.EventHandler(this.frmProductPairing_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtHBProductCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProductName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogoCode.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtHBProductCode;
        private DevExpress.XtraEditors.TextEdit txtProductName;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.LabelControl lblBarcode;
        private DevExpress.XtraEditors.TextEdit txtBarcode;
        private DevExpress.XtraEditors.TextEdit txtLogoCode;
    }
}