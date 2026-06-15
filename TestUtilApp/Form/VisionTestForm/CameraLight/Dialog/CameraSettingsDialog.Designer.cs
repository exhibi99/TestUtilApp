namespace TestUtilApp.CameraLight
{
    partial class CameraSettingsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCamera      = new System.Windows.Forms.Label();
            this.cmbCamera      = new System.Windows.Forms.ComboBox();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.lblExposure    = new System.Windows.Forms.Label();
            this.numExposure    = new System.Windows.Forms.NumericUpDown();
            this.btnOk          = new System.Windows.Forms.Button();
            this.btnCancel      = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).BeginInit();
            this.SuspendLayout();

            var c0    = System.Drawing.Color.FromArgb(18, 22, 35);
            var c1    = System.Drawing.Color.FromArgb(25, 42, 75);
            var cCyan = System.Drawing.Color.FromArgb(128, 255, 255);
            var cGray = System.Drawing.Color.FromArgb(160, 169, 181);

            // ── lblCamera ──────────────────────────────────────────────
            this.lblCamera.AutoSize  = true;
            this.lblCamera.ForeColor = cGray;
            this.lblCamera.Location  = new System.Drawing.Point(12, 16);
            this.lblCamera.Text      = "Camera:";

            // ── cmbCamera ──────────────────────────────────────────────
            this.cmbCamera.BackColor     = c1;
            this.cmbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCamera.ForeColor     = cCyan;
            this.cmbCamera.Location      = new System.Drawing.Point(12, 36);
            this.cmbCamera.Size          = new System.Drawing.Size(260, 23);

            // ── btnRefresh ─────────────────────────────────────────────
            this.btnRefresh.BackColor = c1;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font      = new System.Drawing.Font("Segoe UI", 11f);
            this.btnRefresh.ForeColor = cCyan;
            this.btnRefresh.Location  = new System.Drawing.Point(280, 34);
            this.btnRefresh.Size      = new System.Drawing.Size(36, 28);
            this.btnRefresh.Text      = "↺";
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            // ── lblExposure ────────────────────────────────────────────
            this.lblExposure.AutoSize  = true;
            this.lblExposure.ForeColor = cGray;
            this.lblExposure.Location  = new System.Drawing.Point(12, 74);
            this.lblExposure.Text      = "Exposure (μs):";

            // ── numExposure ────────────────────────────────────────────
            this.numExposure.BackColor     = c1;
            this.numExposure.ForeColor     = cCyan;
            this.numExposure.Location      = new System.Drawing.Point(12, 94);
            this.numExposure.Maximum       = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numExposure.Minimum       = new decimal(new int[] { 100, 0, 0, 0 });
            this.numExposure.Size          = new System.Drawing.Size(120, 23);
            this.numExposure.Increment     = new decimal(new int[] { 100, 0, 0, 0 });
            this.numExposure.Value         = new decimal(new int[] { 5000, 0, 0, 0 });

            // ── btnOk ─────────────────────────────────────────────────
            this.btnOk.BackColor    = c1;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.ForeColor    = cCyan;
            this.btnOk.Location     = new System.Drawing.Point(140, 140);
            this.btnOk.Size         = new System.Drawing.Size(80, 28);
            this.btnOk.Text         = "OK";
            this.btnOk.Click       += new System.EventHandler(this.btnOk_Click);

            // ── btnCancel ─────────────────────────────────────────────
            this.btnCancel.BackColor    = c1;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor    = cGray;
            this.btnCancel.Location     = new System.Drawing.Point(230, 140);
            this.btnCancel.Size         = new System.Drawing.Size(72, 28);
            this.btnCancel.Text         = "Cancel";

            // ── Form ──────────────────────────────────────────────────
            this.AcceptButton       = this.btnOk;
            this.BackColor          = c0;
            this.CancelButton       = this.btnCancel;
            this.ClientSize         = new System.Drawing.Size(326, 186);
            this.FormBorderStyle    = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox        = false;
            this.MinimizeBox        = false;
            this.StartPosition      = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text               = "Camera Settings";

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblCamera, this.cmbCamera, this.btnRefresh,
                this.lblExposure, this.numExposure,
                this.btnOk, this.btnCancel
            });

            ((System.ComponentModel.ISupportInitialize)(this.numExposure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label         lblCamera;
        private System.Windows.Forms.ComboBox      cmbCamera;
        private System.Windows.Forms.Button        btnRefresh;
        private System.Windows.Forms.Label         lblExposure;
        private System.Windows.Forms.NumericUpDown numExposure;
        private System.Windows.Forms.Button        btnOk;
        private System.Windows.Forms.Button        btnCancel;
    }
}
