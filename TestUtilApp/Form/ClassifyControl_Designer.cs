namespace TestUtilApp.UI
{
    partial class ClassifyControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.groupBoxModel = new System.Windows.Forms.GroupBox();
            this.rbModelA = new System.Windows.Forms.RadioButton();
            this.rbModelB = new System.Windows.Forms.RadioButton();
            this.lblModelAInfo = new System.Windows.Forms.Label();
            this.lblModelBInfo = new System.Windows.Forms.Label();
            this.groupBoxModelInfo = new System.Windows.Forms.GroupBox();
            this.btnStartClassify = new System.Windows.Forms.Button();
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.columnCategory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.panelPreviewInfo = new System.Windows.Forms.Panel();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxModel.SuspendLayout();
            this.groupBoxModelInfo.SuspendLayout();
            this.groupBoxProgress.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelPreviewInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(10, 10);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBoxResults);
            this.splitContainer.Panel1.Controls.Add(this.groupBoxProgress);
            this.splitContainer.Panel1.Controls.Add(this.groupBoxSettings);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBoxPreview);
            this.splitContainer.Size = new System.Drawing.Size(1180, 680);
            this.splitContainer.SplitterDistance = 700;
            this.splitContainer.TabIndex = 0;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.txtSourceFolder);
            this.groupBoxSettings.Controls.Add(this.btnBrowseSource);
            this.groupBoxSettings.Controls.Add(this.groupBoxModel);
            this.groupBoxSettings.Controls.Add(this.groupBoxModelInfo);
            this.groupBoxSettings.Controls.Add(this.btnStartClassify);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxSettings.Size = new System.Drawing.Size(700, 330);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Classification Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Folder";
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFolder.Location = new System.Drawing.Point(13, 53);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.ReadOnly = true;
            this.txtSourceFolder.Size = new System.Drawing.Size(570, 27);
            this.txtSourceFolder.TabIndex = 1;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSource.Location = new System.Drawing.Point(589, 51);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(90, 30);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "Browse";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // groupBoxModel
            // 
            this.groupBoxModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxModel.Controls.Add(this.rbModelA);
            this.groupBoxModel.Controls.Add(this.lblModelAInfo);
            this.groupBoxModel.Controls.Add(this.rbModelB);
            this.groupBoxModel.Controls.Add(this.lblModelBInfo);
            this.groupBoxModel.Location = new System.Drawing.Point(13, 95);
            this.groupBoxModel.Name = "groupBoxModel";
            this.groupBoxModel.Size = new System.Drawing.Size(666, 70);
            this.groupBoxModel.TabIndex = 3;
            this.groupBoxModel.TabStop = false;
            this.groupBoxModel.Text = "Classification Model Selection";
            // 
            // rbModelA
            // 
            this.rbModelA.AutoSize = true;
            this.rbModelA.Checked = true;
            this.rbModelA.Location = new System.Drawing.Point(15, 30);
            this.rbModelA.Name = "rbModelA";
            this.rbModelA.Size = new System.Drawing.Size(145, 24);
            this.rbModelA.TabIndex = 0;
            this.rbModelA.TabStop = true;
            this.rbModelA.Text = "ClassifyModel_A";
            this.rbModelA.UseVisualStyleBackColor = true;
            // 
            // lblModelAInfo
            // 
            this.lblModelAInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModelAInfo.AutoSize = false;
            this.lblModelAInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblModelAInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblModelAInfo.Location = new System.Drawing.Point(180, 28);
            this.lblModelAInfo.Name = "lblModelAInfo";
            this.lblModelAInfo.Size = new System.Drawing.Size(475, 20);
            this.lblModelAInfo.TabIndex = 1;
            this.lblModelAInfo.Text = "Loading...";
            this.lblModelAInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbModelB
            // 
            this.rbModelB.AutoSize = true;
            this.rbModelB.Location = new System.Drawing.Point(15, 48);
            this.rbModelB.Name = "rbModelB";
            this.rbModelB.Size = new System.Drawing.Size(145, 24);
            this.rbModelB.TabIndex = 2;
            this.rbModelB.Text = "ClassifyModel_B";
            this.rbModelB.UseVisualStyleBackColor = true;
            // 
            // lblModelBInfo
            // 
            this.lblModelBInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModelBInfo.AutoSize = false;
            this.lblModelBInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblModelBInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblModelBInfo.Location = new System.Drawing.Point(180, 46);
            this.lblModelBInfo.Name = "lblModelBInfo";
            this.lblModelBInfo.Size = new System.Drawing.Size(475, 20);
            this.lblModelBInfo.TabIndex = 3;
            this.lblModelBInfo.Text = "Loading...";
            this.lblModelBInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnStartClassify
            // 
            this.btnStartClassify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartClassify.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnStartClassify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartClassify.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartClassify.ForeColor = System.Drawing.Color.White;
            this.btnStartClassify.Location = new System.Drawing.Point(539, 278);
            this.btnStartClassify.Name = "btnStartClassify";
            this.btnStartClassify.Size = new System.Drawing.Size(140, 40);
            this.btnStartClassify.TabIndex = 6;
            this.btnStartClassify.Text = "Start Classification";
            this.btnStartClassify.UseVisualStyleBackColor = false;
            this.btnStartClassify.Click += new System.EventHandler(this.btnStartClassify_Click);
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Controls.Add(this.progressBar);
            this.groupBoxProgress.Controls.Add(this.txtLog);
            this.groupBoxProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxProgress.Location = new System.Drawing.Point(0, 330);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxProgress.Size = new System.Drawing.Size(700, 150);
            this.groupBoxProgress.TabIndex = 1;
            this.groupBoxProgress.TabStop = false;
            this.groupBoxProgress.Text = "Progress";
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(10, 30);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(680, 23);
            this.progressBar.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(10, 53);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(680, 87);
            this.txtLog.TabIndex = 1;
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.listViewResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(0, 480);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxResults.Size = new System.Drawing.Size(700, 200);
            this.groupBoxResults.TabIndex = 2;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Classification Results";
            // 
            // listViewResults
            // 
            this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCategory,
            this.columnFileName,
            this.columnPath});
            this.listViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.GridLines = true;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(10, 30);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(680, 160);
            this.listViewResults.TabIndex = 0;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = System.Windows.Forms.View.Details;
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            // 
            // columnCategory
            // 
            this.columnCategory.Text = "Category";
            this.columnCategory.Width = 150;
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "Filename";
            this.columnFileName.Width = 200;
            // 
            // columnPath
            // 
            this.columnPath.Text = "Path";
            this.columnPath.Width = 300;
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Controls.Add(this.pictureBoxPreview);
            this.groupBoxPreview.Controls.Add(this.panelPreviewInfo);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxPreview.Size = new System.Drawing.Size(476, 680);
            this.groupBoxPreview.TabIndex = 0;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Image Preview";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.Black;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(10, 30);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(456, 590);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // panelPreviewInfo
            // 
            this.panelPreviewInfo.Controls.Add(this.lblPreviewInfo);
            this.panelPreviewInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPreviewInfo.Location = new System.Drawing.Point(10, 620);
            this.panelPreviewInfo.Name = "panelPreviewInfo";
            this.panelPreviewInfo.Size = new System.Drawing.Size(456, 50);
            this.panelPreviewInfo.TabIndex = 1;
            // 
            // lblPreviewInfo
            // 
            this.lblPreviewInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviewInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreviewInfo.Location = new System.Drawing.Point(0, 0);
            this.lblPreviewInfo.Name = "lblPreviewInfo";
            this.lblPreviewInfo.Size = new System.Drawing.Size(456, 50);
            this.lblPreviewInfo.TabIndex = 0;
            this.lblPreviewInfo.Text = "Select an image to display preview.";
            this.lblPreviewInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ClassifyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ClassifyControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1200, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxModel.ResumeLayout(false);
            this.groupBoxModel.PerformLayout();
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBoxProgress.PerformLayout();
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelPreviewInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.GroupBox groupBoxModel;
        private System.Windows.Forms.GroupBox groupBoxModelInfo;
        private System.Windows.Forms.RadioButton rbModelA;
        private System.Windows.Forms.RadioButton rbModelB;
        private System.Windows.Forms.Label lblModelAInfo;
        private System.Windows.Forms.Label lblModelBInfo;
        private System.Windows.Forms.Button btnStartClassify;
        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ColumnHeader columnCategory;
        private System.Windows.Forms.ColumnHeader columnFileName;
        private System.Windows.Forms.ColumnHeader columnPath;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Panel panelPreviewInfo;
        private System.Windows.Forms.Label lblPreviewInfo;
    }
}