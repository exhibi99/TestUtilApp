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
            this.lblModel = new System.Windows.Forms.Label();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.lblChannels = new System.Windows.Forms.Label();
            this.pnlChannels = new System.Windows.Forms.Panel();
            this.lblIntensity = new System.Windows.Forms.Label();
            this.trkIntensity = new System.Windows.Forms.TrackBar();
            this.lblIntensityVal = new System.Windows.Forms.Label();
            this.btnOn = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trkIntensity)).BeginInit();
            this.SuspendLayout();
            // 
            // lblModel
            // 
            this.lblModel.AutoSize = true;
            this.lblModel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(169)))), ((int)(((byte)(181)))));
            this.lblModel.Location = new System.Drawing.Point(12, 16);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(44, 12);
            this.lblModel.TabIndex = 0;
            this.lblModel.Text = "Model:";
            // 
            // cmbModel
            // 
            this.cmbModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbModel.Items.AddRange(new object[] {
            "ALT-E8RS-24V  (8ch)",
            "ALT-E16RS-24V  (16ch)"});
            this.cmbModel.Location = new System.Drawing.Point(90, 13);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(190, 20);
            this.cmbModel.TabIndex = 1;
            this.cmbModel.SelectedIndexChanged += new System.EventHandler(this.cmbModel_SelectedIndexChanged);
            // 
            // lblChannels
            // 
            this.lblChannels.AutoSize = true;
            this.lblChannels.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(169)))), ((int)(((byte)(181)))));
            this.lblChannels.Location = new System.Drawing.Point(12, 52);
            this.lblChannels.Name = "lblChannels";
            this.lblChannels.Size = new System.Drawing.Size(63, 12);
            this.lblChannels.TabIndex = 2;
            this.lblChannels.Text = "Channels:";
            // 
            // pnlChannels
            // 
            this.pnlChannels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(22)))), ((int)(((byte)(35)))));
            this.pnlChannels.Location = new System.Drawing.Point(12, 70);
            this.pnlChannels.Name = "pnlChannels";
            this.pnlChannels.Size = new System.Drawing.Size(292, 64);
            this.pnlChannels.TabIndex = 3;
            // 
            // lblIntensity
            // 
            this.lblIntensity.AutoSize = true;
            this.lblIntensity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(169)))), ((int)(((byte)(181)))));
            this.lblIntensity.Location = new System.Drawing.Point(12, 148);
            this.lblIntensity.Name = "lblIntensity";
            this.lblIntensity.Size = new System.Drawing.Size(56, 12);
            this.lblIntensity.TabIndex = 4;
            this.lblIntensity.Text = "Intensity:";
            // 
            // trkIntensity
            // 
            this.trkIntensity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(22)))), ((int)(((byte)(35)))));
            this.trkIntensity.Location = new System.Drawing.Point(90, 142);
            this.trkIntensity.Maximum = 255;
            this.trkIntensity.Name = "trkIntensity";
            this.trkIntensity.Size = new System.Drawing.Size(160, 45);
            this.trkIntensity.TabIndex = 5;
            this.trkIntensity.TickFrequency = 32;
            this.trkIntensity.Value = 128;
            this.trkIntensity.Scroll += new System.EventHandler(this.trkIntensity_Scroll);
            // 
            // lblIntensityVal
            // 
            this.lblIntensityVal.AutoSize = true;
            this.lblIntensityVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lblIntensityVal.Location = new System.Drawing.Point(258, 148);
            this.lblIntensityVal.Name = "lblIntensityVal";
            this.lblIntensityVal.Size = new System.Drawing.Size(23, 12);
            this.lblIntensityVal.TabIndex = 6;
            this.lblIntensityVal.Text = "128";
            // 
            // btnOn
            // 
            this.btnOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOn.Location = new System.Drawing.Point(12, 200);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(100, 30);
            this.btnOn.TabIndex = 7;
            this.btnOn.Text = "Light ON";
            this.btnOn.UseVisualStyleBackColor = false;
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnOff
            // 
            this.btnOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(52)))), ((int)(((byte)(58)))));
            this.btnOff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOff.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOff.ForeColor = System.Drawing.Color.White;
            this.btnOff.Location = new System.Drawing.Point(122, 200);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(100, 30);
            this.btnOff.TabIndex = 8;
            this.btnOff.Text = "Light OFF";
            this.btnOff.UseVisualStyleBackColor = false;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(232, 200);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 30);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // LightSettingsDialog
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(22)))), ((int)(((byte)(35)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(318, 248);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.cmbModel);
            this.Controls.Add(this.lblChannels);
            this.Controls.Add(this.pnlChannels);
            this.Controls.Add(this.lblIntensity);
            this.Controls.Add(this.trkIntensity);
            this.Controls.Add(this.lblIntensityVal);
            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.btnOff);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LightSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Light Controller Settings";
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
