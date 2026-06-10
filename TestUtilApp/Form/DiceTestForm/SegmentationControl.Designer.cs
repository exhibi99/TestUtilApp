namespace TestUtilApp.UI
{
    partial class SegmentationControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeResults();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBoxSource = new System.Windows.Forms.GroupBox();
            this.lblSourceFolder = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.groupBoxModel = new System.Windows.Forms.GroupBox();
            this.lblModelPath = new System.Windows.Forms.Label();
            this.txtModelPath = new System.Windows.Forms.TextBox();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.btnBrowseModel = new System.Windows.Forms.Button();
            this.btnStartSegmentation = new System.Windows.Forms.Button();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnClearResults = new System.Windows.Forms.Button();
            this.btnSaveResults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.groupBoxSource.SuspendLayout();
            this.groupBoxModel.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.groupBoxLog.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.Controls.Add(this.leftPanel);
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(1180, 700);
            this.splitContainer.SplitterDistance = 650;
            this.splitContainer.TabIndex = 0;
            //
            // leftPanel
            //
            this.leftPanel.Controls.Add(this.groupBoxResults);
            this.leftPanel.Controls.Add(this.topPanel);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(10, 0, 5, 10);
            this.leftPanel.Size = new System.Drawing.Size(650, 700);
            this.leftPanel.TabIndex = 0;
            //
            // topPanel
            //
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(25)))), ((int)(((byte)(31)))));
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Controls.Add(this.groupBoxSource);
            this.topPanel.Controls.Add(this.groupBoxModel);
            this.topPanel.Controls.Add(this.btnStartSegmentation);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(10, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(635, 210);
            this.topPanel.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(13, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Segmentation Test";
            //
            // groupBoxSource
            //
            this.groupBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSource.Controls.Add(this.lblSourceFolder);
            this.groupBoxSource.Controls.Add(this.txtSourceFolder);
            this.groupBoxSource.Controls.Add(this.btnBrowseSource);
            this.groupBoxSource.Location = new System.Drawing.Point(10, 44);
            this.groupBoxSource.Name = "groupBoxSource";
            this.groupBoxSource.Size = new System.Drawing.Size(615, 58);
            this.groupBoxSource.TabIndex = 1;
            this.groupBoxSource.TabStop = false;
            this.groupBoxSource.Text = "Source Folder";
            //
            // lblSourceFolder
            //
            this.lblSourceFolder.AutoSize = true;
            this.lblSourceFolder.Location = new System.Drawing.Point(15, 26);
            this.lblSourceFolder.Name = "lblSourceFolder";
            this.lblSourceFolder.Size = new System.Drawing.Size(79, 20);
            this.lblSourceFolder.TabIndex = 0;
            this.lblSourceFolder.Text = "Image Folder:";
            //
            // txtSourceFolder
            //
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFolder.Location = new System.Drawing.Point(105, 23);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.ReadOnly = true;
            this.txtSourceFolder.Size = new System.Drawing.Size(419, 27);
            this.txtSourceFolder.TabIndex = 1;
            //
            // btnBrowseSource
            //
            this.btnBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSource.Location = new System.Drawing.Point(530, 22);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "Browse";
            this.btnBrowseSource.UseVisualStyleBackColor = false;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            //
            // groupBoxModel
            //
            this.groupBoxModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxModel.Controls.Add(this.lblModelPath);
            this.groupBoxModel.Controls.Add(this.txtModelPath);
            this.groupBoxModel.Controls.Add(this.btnLoadModel);
            this.groupBoxModel.Controls.Add(this.btnBrowseModel);
            this.groupBoxModel.Location = new System.Drawing.Point(10, 108);
            this.groupBoxModel.Name = "groupBoxModel";
            this.groupBoxModel.Size = new System.Drawing.Size(615, 58);
            this.groupBoxModel.TabIndex = 2;
            this.groupBoxModel.TabStop = false;
            this.groupBoxModel.Text = "Segmentation Model";
            //
            // lblModelPath
            //
            this.lblModelPath.AutoSize = true;
            this.lblModelPath.Location = new System.Drawing.Point(15, 26);
            this.lblModelPath.Name = "lblModelPath";
            this.lblModelPath.Size = new System.Drawing.Size(86, 20);
            this.lblModelPath.TabIndex = 0;
            this.lblModelPath.Text = "Model Folder:";
            //
            // txtModelPath
            //
            this.txtModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModelPath.Location = new System.Drawing.Point(105, 23);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.ReadOnly = true;
            this.txtModelPath.Size = new System.Drawing.Size(337, 27);
            this.txtModelPath.TabIndex = 1;
            //
            // btnLoadModel
            //
            this.btnLoadModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadModel.Location = new System.Drawing.Point(448, 22);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(75, 27);
            this.btnLoadModel.TabIndex = 2;
            this.btnLoadModel.Text = "Load";
            this.btnLoadModel.UseVisualStyleBackColor = false;
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            //
            // btnBrowseModel
            //
            this.btnBrowseModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseModel.Location = new System.Drawing.Point(530, 22);
            this.btnBrowseModel.Name = "btnBrowseModel";
            this.btnBrowseModel.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseModel.TabIndex = 3;
            this.btnBrowseModel.Text = "Browse";
            this.btnBrowseModel.UseVisualStyleBackColor = false;
            this.btnBrowseModel.Click += new System.EventHandler(this.btnBrowseModel_Click);
            //
            // btnStartSegmentation
            //
            this.btnStartSegmentation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartSegmentation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnStartSegmentation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartSegmentation.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartSegmentation.ForeColor = System.Drawing.Color.White;
            this.btnStartSegmentation.Location = new System.Drawing.Point(10, 170);
            this.btnStartSegmentation.Name = "btnStartSegmentation";
            this.btnStartSegmentation.Size = new System.Drawing.Size(615, 35);
            this.btnStartSegmentation.TabIndex = 3;
            this.btnStartSegmentation.Text = "▶ Start Segmentation";
            this.btnStartSegmentation.UseVisualStyleBackColor = false;
            this.btnStartSegmentation.Click += new System.EventHandler(this.btnStartSegmentation_Click);
            //
            // groupBoxResults
            //
            this.groupBoxResults.Controls.Add(this.listViewResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(10, 210);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxResults.Size = new System.Drawing.Size(635, 480);
            this.groupBoxResults.TabIndex = 1;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Segmentation Results";
            //
            // listViewResults
            //
            this.listViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(10, 30);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(615, 440);
            this.listViewResults.TabIndex = 0;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            //
            // rightPanel
            //
            this.rightPanel.Controls.Add(this.groupBoxPreview);
            this.rightPanel.Controls.Add(this.groupBoxLog);
            this.rightPanel.Controls.Add(this.bottomPanel);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(5, 0, 10, 10);
            this.rightPanel.Size = new System.Drawing.Size(526, 700);
            this.rightPanel.TabIndex = 0;
            //
            // groupBoxPreview
            //
            this.groupBoxPreview.Controls.Add(this.pictureBoxPreview);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(5, 0);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxPreview.Size = new System.Drawing.Size(511, 540);
            this.groupBoxPreview.TabIndex = 0;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Segmentation Preview";
            //
            // pictureBoxPreview
            //
            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(10, 30);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(491, 500);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            //
            // groupBoxLog
            //
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxLog.Location = new System.Drawing.Point(5, 540);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxLog.Size = new System.Drawing.Size(511, 100);
            this.groupBoxLog.TabIndex = 1;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "Log";
            //
            // txtLog
            //
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(10, 30);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(491, 60);
            this.txtLog.TabIndex = 0;
            //
            // bottomPanel
            //
            this.bottomPanel.Controls.Add(this.progressBar);
            this.bottomPanel.Controls.Add(this.lblProgress);
            this.bottomPanel.Controls.Add(this.btnClearResults);
            this.bottomPanel.Controls.Add(this.btnSaveResults);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(5, 640);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(511, 50);
            this.bottomPanel.TabIndex = 2;
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(85, 15);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(190, 23);
            this.progressBar.TabIndex = 1;
            //
            // lblProgress
            //
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(5, 15);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(39, 20);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = "0 / 0";
            //
            // btnClearResults
            //
            this.btnClearResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearResults.Location = new System.Drawing.Point(397, 10);
            this.btnClearResults.Name = "btnClearResults";
            this.btnClearResults.Size = new System.Drawing.Size(110, 30);
            this.btnClearResults.TabIndex = 3;
            this.btnClearResults.Text = "Results Reset";
            this.btnClearResults.UseVisualStyleBackColor = false;
            this.btnClearResults.Click += new System.EventHandler(this.btnClearResults_Click);
            //
            // btnSaveResults
            //
            this.btnSaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveResults.Location = new System.Drawing.Point(281, 10);
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.Size = new System.Drawing.Size(110, 30);
            this.btnSaveResults.TabIndex = 2;
            this.btnSaveResults.Text = "Results Save";
            this.btnSaveResults.UseVisualStyleBackColor = false;
            this.btnSaveResults.Click += new System.EventHandler(this.btnSaveResults_Click);
            //
            // SegmentationControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "SegmentationControl";
            this.Size = new System.Drawing.Size(1180, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.groupBoxSource.ResumeLayout(false);
            this.groupBoxSource.PerformLayout();
            this.groupBoxModel.ResumeLayout(false);
            this.groupBoxModel.PerformLayout();
            this.groupBoxResults.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBoxSource;
        private System.Windows.Forms.Label lblSourceFolder;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.GroupBox groupBoxModel;
        private System.Windows.Forms.Label lblModelPath;
        private System.Windows.Forms.TextBox txtModelPath;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.Button btnBrowseModel;
        private System.Windows.Forms.Button btnStartSegmentation;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnClearResults;
        private System.Windows.Forms.Button btnSaveResults;
    }
}
