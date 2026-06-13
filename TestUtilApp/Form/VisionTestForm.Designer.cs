namespace TestUtilApp.UI
{
    partial class VisionTestForm
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnOpenCv = new System.Windows.Forms.Button();
            this.btnCameraCalc = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.topPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // topPanel
            //
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(28)))), ((int)(((byte)(55)))));
            this.topPanel.Controls.Add(this.btnOpenCv);
            this.topPanel.Controls.Add(this.btnCameraCalc);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1200, 83);
            this.topPanel.TabIndex = 0;
            //
            // btnOpenCv
            //
            this.btnOpenCv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnOpenCv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenCv.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOpenCv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnOpenCv.Location = new System.Drawing.Point(13, 12);
            this.btnOpenCv.Name = "btnOpenCv";
            this.btnOpenCv.Size = new System.Drawing.Size(130, 69);
            this.btnOpenCv.TabIndex = 0;
            this.btnOpenCv.Text = "OpenCV";
            this.btnOpenCv.UseVisualStyleBackColor = false;
            this.btnOpenCv.Click += new System.EventHandler(this.btnOpenCv_Click);
            //
            // btnCameraCalc
            //
            this.btnCameraCalc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnCameraCalc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCameraCalc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCameraCalc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnCameraCalc.Location = new System.Drawing.Point(153, 12);
            this.btnCameraCalc.Name = "btnCameraCalc";
            this.btnCameraCalc.Size = new System.Drawing.Size(130, 69);
            this.btnCameraCalc.TabIndex = 1;
            this.btnCameraCalc.Text = "Camera Calc";
            this.btnCameraCalc.UseVisualStyleBackColor = false;
            this.btnCameraCalc.Click += new System.EventHandler(this.btnCameraCalc_Click);
            //
            // contentPanel
            //
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 83);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1200, 667);
            this.contentPanel.TabIndex = 1;
            //
            // bottomPanel
            //
            this.bottomPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(25)))), ((int)(((byte)(31)))));
            this.bottomPanel.Controls.Add(this.statusStrip);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 750);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1200, 30);
            this.bottomPanel.TabIndex = 2;
            //
            // statusStrip
            //
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.statusStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(169)))), ((int)(((byte)(181)))));
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 4);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1200, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            //
            // toolStripStatusLabel
            //
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(50, 20);
            this.toolStripStatusLabel.Text = "Ready";
            //
            // VisionTestForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "VisionTestForm";
            this.Size = new System.Drawing.Size(1200, 780);
            this.topPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Button btnOpenCv;
        private System.Windows.Forms.Button btnCameraCalc;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
