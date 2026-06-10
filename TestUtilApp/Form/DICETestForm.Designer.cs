namespace TestUtilApp.UI
{
    partial class DICETestForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeLoadedControls();

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnConfigEdit = new System.Windows.Forms.Button();
            this.btnSegmentation = new System.Windows.Forms.Button();
            this.btnInspection = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnCrop = new System.Windows.Forms.Button();
            this.btnClassify = new System.Windows.Forms.Button();
            this.btnLabelGen = new System.Windows.Forms.Button();
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
            this.topPanel.Controls.Add(this.btnConfigEdit);
            this.topPanel.Controls.Add(this.btnSegmentation);
            this.topPanel.Controls.Add(this.btnInspection);
            this.topPanel.Controls.Add(this.btnFilter);
            this.topPanel.Controls.Add(this.btnCrop);
            this.topPanel.Controls.Add(this.btnClassify);
            this.topPanel.Controls.Add(this.btnLabelGen);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1200, 83);
            this.topPanel.TabIndex = 0;
            // 
            // btnConfigEdit
            // 
            this.btnConfigEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnConfigEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfigEdit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfigEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnConfigEdit.Location = new System.Drawing.Point(1057, 13);
            this.btnConfigEdit.Name = "btnConfigEdit";
            this.btnConfigEdit.Size = new System.Drawing.Size(120, 64);
            this.btnConfigEdit.TabIndex = 4;
            this.btnConfigEdit.Text = "Settings";
            this.btnConfigEdit.UseVisualStyleBackColor = false;
            this.btnConfigEdit.Click += new System.EventHandler(this.btnConfigEdit_Click);
            // 
            // btnInspection
            //
            this.btnInspection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnInspection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInspection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnInspection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnInspection.Location = new System.Drawing.Point(561, 12);
            this.btnInspection.Name = "btnInspection";
            this.btnInspection.Size = new System.Drawing.Size(150, 69);
            this.btnInspection.TabIndex = 5;
            this.btnInspection.Text = "Inspect";
            this.btnInspection.UseVisualStyleBackColor = false;
            this.btnInspection.Click += new System.EventHandler(this.btnInspection_Click);
            //
            // btnSegmentation
            //
            this.btnSegmentation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnSegmentation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSegmentation.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSegmentation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnSegmentation.Location = new System.Drawing.Point(717, 12);
            this.btnSegmentation.Name = "btnSegmentation";
            this.btnSegmentation.Size = new System.Drawing.Size(150, 69);
            this.btnSegmentation.TabIndex = 6;
            this.btnSegmentation.Text = "Segmentation";
            this.btnSegmentation.UseVisualStyleBackColor = false;
            this.btnSegmentation.Click += new System.EventHandler(this.btnSegmentation_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnFilter.Location = new System.Drawing.Point(13, 13);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 68);
            this.btnFilter.TabIndex = 3;
            this.btnFilter.Text = "File Filtering";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnCrop
            // 
            this.btnCrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnCrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnCrop.Location = new System.Drawing.Point(276, 12);
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(129, 69);
            this.btnCrop.TabIndex = 0;
            this.btnCrop.Text = "Image Crop";
            this.btnCrop.UseVisualStyleBackColor = false;
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // btnClassify
            // 
            this.btnClassify.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnClassify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClassify.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClassify.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnClassify.Location = new System.Drawing.Point(411, 13);
            this.btnClassify.Name = "btnClassify";
            this.btnClassify.Size = new System.Drawing.Size(144, 68);
            this.btnClassify.TabIndex = 1;
            this.btnClassify.Text = "Image Classification";
            this.btnClassify.UseVisualStyleBackColor = false;
            this.btnClassify.Click += new System.EventHandler(this.btnClassify_Click);
            // 
            // btnLabelGen
            // 
            this.btnLabelGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(42)))), ((int)(((byte)(75)))));
            this.btnLabelGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLabelGen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLabelGen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnLabelGen.Location = new System.Drawing.Point(139, 13);
            this.btnLabelGen.Name = "btnLabelGen";
            this.btnLabelGen.Size = new System.Drawing.Size(131, 68);
            this.btnLabelGen.TabIndex = 2;
            this.btnLabelGen.Text = "Label Generate";
            this.btnLabelGen.UseVisualStyleBackColor = false;
            this.btnLabelGen.Click += new System.EventHandler(this.btnLabelGen_Click);
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
            // DICETestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "DICETestForm";
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
        private System.Windows.Forms.Button btnCrop;
        private System.Windows.Forms.Button btnClassify;
        private System.Windows.Forms.Button btnLabelGen;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnInspection;
        private System.Windows.Forms.Button btnSegmentation;
        private System.Windows.Forms.Button btnConfigEdit;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
