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
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnConfigEdit = new System.Windows.Forms.Button();
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
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.topPanel.Controls.Add(this.btnConfigEdit);
            this.topPanel.Controls.Add(this.btnInspection);
            this.topPanel.Controls.Add(this.btnFilter);
            this.topPanel.Controls.Add(this.btnCrop);
            this.topPanel.Controls.Add(this.btnClassify);
            this.topPanel.Controls.Add(this.btnLabelGen);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1200, 60);
            this.topPanel.TabIndex = 0;
            // 
            // btnConfigEdit
            // 
            this.btnConfigEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfigEdit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnConfigEdit.Location = new System.Drawing.Point(1057, 13);
            this.btnConfigEdit.Name = "btnConfigEdit";
            this.btnConfigEdit.Size = new System.Drawing.Size(120, 34);
            this.btnConfigEdit.TabIndex = 4;
            this.btnConfigEdit.Text = "⚙ Settings";
            this.btnConfigEdit.UseVisualStyleBackColor = true;
            this.btnConfigEdit.Click += new System.EventHandler(this.btnConfigEdit_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnFilter.Location = new System.Drawing.Point(391, 13);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 34);
            this.btnFilter.TabIndex = 3;
            this.btnFilter.Text = "File Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnInspection
            // 
            this.btnInspection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInspection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnInspection.Location = new System.Drawing.Point(517, 13);
            this.btnInspection.Name = "btnInspection";
            this.btnInspection.Size = new System.Drawing.Size(120, 34);
            this.btnInspection.TabIndex = 5;
            this.btnInspection.Text = "🔍 Inspect";
            this.btnInspection.UseVisualStyleBackColor = true;
            this.btnInspection.Click += new System.EventHandler(this.btnInspection_Click);
            // 
            // btnCrop
            // 
            this.btnCrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCrop.Location = new System.Drawing.Point(13, 13);
            this.btnCrop.Name = "btnCrop";
            this.btnCrop.Size = new System.Drawing.Size(120, 34);
            this.btnCrop.TabIndex = 0;
            this.btnCrop.Text = "Image Crop";
            this.btnCrop.UseVisualStyleBackColor = true;
            this.btnCrop.Click += new System.EventHandler(this.btnCrop_Click);
            // 
            // btnClassify
            // 
            this.btnClassify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClassify.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClassify.Location = new System.Drawing.Point(139, 13);
            this.btnClassify.Name = "btnClassify";
            this.btnClassify.Size = new System.Drawing.Size(120, 34);
            this.btnClassify.TabIndex = 1;
            this.btnClassify.Text = "Image Classification";
            this.btnClassify.UseVisualStyleBackColor = true;
            this.btnClassify.Click += new System.EventHandler(this.btnClassify_Click);
            // 
            // btnLabelGen
            // 
            this.btnLabelGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLabelGen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLabelGen.Location = new System.Drawing.Point(265, 13);
            this.btnLabelGen.Name = "btnLabelGen";
            this.btnLabelGen.Size = new System.Drawing.Size(120, 34);
            this.btnLabelGen.TabIndex = 2;
            this.btnLabelGen.Text = "Label Gen";
            this.btnLabelGen.UseVisualStyleBackColor = true;
            this.btnLabelGen.Click += new System.EventHandler(this.btnLabelGen_Click);
            // 
            // contentPanel
            // 
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 60);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1200, 690);
            this.contentPanel.TabIndex = 1;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.statusStrip);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 750);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(1200, 30);
            this.bottomPanel.TabIndex = 2;
            // 
            // statusStrip
            // 
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
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 20);
            this.toolStripStatusLabel.Text = "Ready";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 780);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Processing Utility";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
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
        private System.Windows.Forms.Button btnConfigEdit;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}


