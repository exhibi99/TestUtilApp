namespace TestUtilApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabDiceTest = new System.Windows.Forms.TabPage();
            this.tabVisionTest = new System.Windows.Forms.TabPage();
            this.lblGpuInfo = new System.Windows.Forms.Label();
            this.btnDiceVersion = new System.Windows.Forms.Button();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabDiceTest);
            this.tabControlMain.Controls.Add(this.tabVisionTest);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.tabControlMain.ItemSize = new System.Drawing.Size(140, 34);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1731, 1203);
            this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlMain.TabIndex = 0;
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
            // 
            // tabDiceTest
            // 
            this.tabDiceTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(25)))), ((int)(((byte)(31)))));
            this.tabDiceTest.Location = new System.Drawing.Point(4, 38);
            this.tabDiceTest.Name = "tabDiceTest";
            this.tabDiceTest.Size = new System.Drawing.Size(1723, 1161);
            this.tabDiceTest.TabIndex = 0;
            this.tabDiceTest.Text = "DICE Test";
            // 
            // tabVisionTest
            // 
            this.tabVisionTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(25)))), ((int)(((byte)(31)))));
            this.tabVisionTest.Location = new System.Drawing.Point(4, 38);
            this.tabVisionTest.Name = "tabVisionTest";
            this.tabVisionTest.Size = new System.Drawing.Size(1192, 738);
            this.tabVisionTest.TabIndex = 1;
            this.tabVisionTest.Text = "Vision Test";
            //
            // btnDiceVersion
            //
            this.btnDiceVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDiceVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDiceVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDiceVersion.Location = new System.Drawing.Point(900, 4);
            this.btnDiceVersion.Name = "btnDiceVersion";
            this.btnDiceVersion.Size = new System.Drawing.Size(110, 26);
            this.btnDiceVersion.TabIndex = 2;
            this.btnDiceVersion.Text = "DICE v2";
            this.btnDiceVersion.UseVisualStyleBackColor = false;
            this.btnDiceVersion.Click += new System.EventHandler(this.btnDiceVersion_Click);
            //
            // lblGpuInfo
            //
            this.lblGpuInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGpuInfo.AutoSize = true;
            this.lblGpuInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGpuInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(169)))), ((int)(((byte)(181)))));
            this.lblGpuInfo.Location = new System.Drawing.Point(1050, 10);
            this.lblGpuInfo.Name = "lblGpuInfo";
            this.lblGpuInfo.Size = new System.Drawing.Size(100, 20);
            this.lblGpuInfo.TabIndex = 1;
            this.lblGpuInfo.Text = "GPU: detecting...";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(1385, 962);
            this.Controls.Add(this.btnDiceVersion);
            this.Controls.Add(this.lblGpuInfo);
            this.Controls.Add(this.tabControlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test Utility";
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabDiceTest;
        private System.Windows.Forms.TabPage tabVisionTest;
        private System.Windows.Forms.Label lblGpuInfo;
        private System.Windows.Forms.Button btnDiceVersion;
    }
}
