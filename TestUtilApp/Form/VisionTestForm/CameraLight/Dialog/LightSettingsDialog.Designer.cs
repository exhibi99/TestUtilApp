namespace TestUtilApp.CameraLight
{
    partial class LightSettingsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblModel        = new System.Windows.Forms.Label();
            this.cmbModel        = new System.Windows.Forms.ComboBox();
            this.lblChannels     = new System.Windows.Forms.Label();
            this.pnlChannels     = new System.Windows.Forms.Panel();
            this.lblIntensity    = new System.Windows.Forms.Label();
            this.trkIntensity    = new System.Windows.Forms.TrackBar();
            this.lblIntensityVal = new System.Windows.Forms.Label();
            this.btnOn           = new System.Windows.Forms.Button();
            this.btnOff          = new System.Windows.Forms.Button();
            this.btnClose        = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trkIntensity)).BeginInit();
            this.SuspendLayout();

            var c0    = System.Drawing.Color.FromArgb(18, 22, 35);
            var c1    = System.Drawing.Color.FromArgb(25, 42, 75);
            var cCyan = System.Drawing.Color.FromArgb(128, 255, 255);
            var cGray = System.Drawing.Color.FromArgb(160, 169, 181);

            // ── lblModel ──────────────────────────────────────────────
            this.lblModel.AutoSize  = true;
            this.lblModel.ForeColor = cGray;
            this.lblModel.Location  = new System.Drawing.Point(12, 16);
            this.lblModel.Text      = "Model:";

            // ── cmbModel ──────────────────────────────────────────────
            this.cmbModel.BackColor     = c1;
            this.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModel.ForeColor     = cCyan;
            this.cmbModel.Items.AddRange(new object[] {
                "ALT-E8RS-24V  (8ch)",
                "ALT-E16RS-24V  (16ch)"
            });
            this.cmbModel.Location           = new System.Drawing.Point(90, 13);
            this.cmbModel.Size               = new System.Drawing.Size(190, 23);
            this.cmbModel.SelectedIndexChanged += new System.EventHandler(this.cmbModel_SelectedIndexChanged);

            // ── lblChannels ───────────────────────────────────────────
            this.lblChannels.AutoSize  = true;
            this.lblChannels.ForeColor = cGray;
            this.lblChannels.Location  = new System.Drawing.Point(12, 52);
            this.lblChannels.Text      = "Channels:";

            // ── pnlChannels (채널 버튼 컨테이너) ──────────────────────
            this.pnlChannels.BackColor = c0;
            this.pnlChannels.Location  = new System.Drawing.Point(12, 70);
            this.pnlChannels.Size      = new System.Drawing.Size(292, 64);

            // ── lblIntensity ──────────────────────────────────────────
            this.lblIntensity.AutoSize  = true;
            this.lblIntensity.ForeColor = cGray;
            this.lblIntensity.Location  = new System.Drawing.Point(12, 148);
            this.lblIntensity.Text      = "Intensity:";

            // ── trkIntensity ──────────────────────────────────────────
            this.trkIntensity.BackColor     = c0;
            this.trkIntensity.Location      = new System.Drawing.Point(90, 142);
            this.trkIntensity.Maximum       = 255;
            this.trkIntensity.Size          = new System.Drawing.Size(160, 45);
            this.trkIntensity.TickFrequency = 32;
            this.trkIntensity.Value         = 128;
            this.trkIntensity.Scroll       += new System.EventHandler(this.trkIntensity_Scroll);

            // ── lblIntensityVal ───────────────────────────────────────
            this.lblIntensityVal.AutoSize  = true;
            this.lblIntensityVal.ForeColor = cCyan;
            this.lblIntensityVal.Location  = new System.Drawing.Point(258, 148);
            this.lblIntensityVal.Text      = "128";

            // ── btnOn ─────────────────────────────────────────────────
            this.btnOn.BackColor = System.Drawing.Color.FromArgb(22, 58, 42);
            this.btnOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOn.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.btnOn.ForeColor = System.Drawing.Color.White;
            this.btnOn.Location  = new System.Drawing.Point(12, 200);
            this.btnOn.Size      = new System.Drawing.Size(100, 30);
            this.btnOn.Text      = "Light ON";
            this.btnOn.Click    += new System.EventHandler(this.btnOn_Click);

            // ── btnOff ────────────────────────────────────────────────
            this.btnOff.BackColor = System.Drawing.Color.FromArgb(50, 52, 58);
            this.btnOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOff.Font      = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.btnOff.ForeColor = System.Drawing.Color.White;
            this.btnOff.Location  = new System.Drawing.Point(122, 200);
            this.btnOff.Size      = new System.Drawing.Size(100, 30);
            this.btnOff.Text      = "Light OFF";
            this.btnOff.Click    += new System.EventHandler(this.btnOff_Click);

            // ── btnClose ──────────────────────────────────────────────
            this.btnClose.BackColor    = c1;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle    = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor    = System.Drawing.Color.White;
            this.btnClose.Location     = new System.Drawing.Point(232, 200);
            this.btnClose.Size         = new System.Drawing.Size(60, 30);
            this.btnClose.Text         = "Close";

            // ── Form ──────────────────────────────────────────────────
            this.BackColor       = c0;
            this.CancelButton    = this.btnClose;
            this.ClientSize      = new System.Drawing.Size(318, 248);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Light Controller Settings";

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblModel,    this.cmbModel,
                this.lblChannels, this.pnlChannels,
                this.lblIntensity, this.trkIntensity, this.lblIntensityVal,
                this.btnOn, this.btnOff, this.btnClose
            });

            ((System.ComponentModel.ISupportInitialize)(this.trkIntensity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label    lblModel;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.Label    lblChannels;
        private System.Windows.Forms.Panel    pnlChannels;
        private System.Windows.Forms.Label    lblIntensity;
        private System.Windows.Forms.TrackBar trkIntensity;
        private System.Windows.Forms.Label    lblIntensityVal;
        private System.Windows.Forms.Button   btnOn;
        private System.Windows.Forms.Button   btnOff;
        private System.Windows.Forms.Button   btnClose;
    }
}
