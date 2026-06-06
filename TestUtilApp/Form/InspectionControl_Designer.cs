namespace TestUtilApp.UI
{
    partial class InspectionControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBoxSource = new System.Windows.Forms.GroupBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.lblSourceFolder = new System.Windows.Forms.Label();
            this.txtFileNameFilter = new System.Windows.Forms.TextBox();
            this.lblFileNameFilter = new System.Windows.Forms.Label();
            this.groupBoxSpecFilter = new System.Windows.Forms.GroupBox();
            this.btnBrowseSpecJson = new System.Windows.Forms.Button();
            this.txtSpecJsonPath = new System.Windows.Forms.TextBox();
            this.lblSpecJsonPath = new System.Windows.Forms.Label();
            this.chkUseSpecFilter = new System.Windows.Forms.CheckBox();
            this.btnStartInspection = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.bottomLeftPanel = new System.Windows.Forms.Panel();
            this.btnSaveResults = new System.Windows.Forms.Button();
            this.btnClearResults = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.topPanel.SuspendLayout();
            this.groupBoxSource.SuspendLayout();
            this.groupBoxSpecFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.bottomLeftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Controls.Add(this.groupBoxSource);
            this.topPanel.Controls.Add(this.groupBoxSpecFilter);
            this.topPanel.Controls.Add(this.btnStartInspection);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1180, 190);
            this.topPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(13, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(313, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Auto Inspection (Detection + Classify)";
            // 
            // groupBoxSource
            // 
            this.groupBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSource.Controls.Add(this.lblFileNameFilter);
            this.groupBoxSource.Controls.Add(this.txtFileNameFilter);
            this.groupBoxSource.Controls.Add(this.btnBrowseSource);
            this.groupBoxSource.Controls.Add(this.txtSourceFolder);
            this.groupBoxSource.Controls.Add(this.lblSourceFolder);
            this.groupBoxSource.Location = new System.Drawing.Point(13, 45);
            this.groupBoxSource.Name = "groupBoxSource";
            this.groupBoxSource.Size = new System.Drawing.Size(900, 90);
            this.groupBoxSource.TabIndex = 1;
            this.groupBoxSource.TabStop = false;
            this.groupBoxSource.Text = "Inspection Target";
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSource.Location = new System.Drawing.Point(810, 22);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "Browse";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFolder.Location = new System.Drawing.Point(100, 23);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.ReadOnly = true;
            this.txtSourceFolder.Size = new System.Drawing.Size(704, 27);
            this.txtSourceFolder.TabIndex = 1;
            // 
            // lblSourceFolder
            // 
            this.lblSourceFolder.AutoSize = true;
            this.lblSourceFolder.Location = new System.Drawing.Point(15, 26);
            this.lblSourceFolder.Name = "lblSourceFolder";
            this.lblSourceFolder.Size = new System.Drawing.Size(79, 20);
            this.lblSourceFolder.TabIndex = 0;
            this.lblSourceFolder.Text = "Source Folder:";
            // 
            // lblFileNameFilter
            // 
            this.lblFileNameFilter.AutoSize = true;
            this.lblFileNameFilter.Location = new System.Drawing.Point(15, 59);
            this.lblFileNameFilter.Name = "lblFileNameFilter";
            this.lblFileNameFilter.Size = new System.Drawing.Size(122, 20);
            this.lblFileNameFilter.TabIndex = 3;
            this.lblFileNameFilter.Text = "Filename Keyword:";
            // 
            // txtFileNameFilter
            // 
            this.txtFileNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileNameFilter.Location = new System.Drawing.Point(143, 56);
            this.txtFileNameFilter.Name = "txtFileNameFilter";
            this.txtFileNameFilter.Size = new System.Drawing.Size(661, 27);
            this.txtFileNameFilter.TabIndex = 4;
            this.txtFileNameFilter.TextChanged += new System.EventHandler(this.txtFileNameFilter_TextChanged);
            // 
            // groupBoxSpecFilter
            // 
            this.groupBoxSpecFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSpecFilter.Controls.Add(this.chkUseSpecFilter);
            this.groupBoxSpecFilter.Controls.Add(this.btnBrowseSpecJson);
            this.groupBoxSpecFilter.Controls.Add(this.txtSpecJsonPath);
            this.groupBoxSpecFilter.Controls.Add(this.lblSpecJsonPath);
            this.groupBoxSpecFilter.Location = new System.Drawing.Point(13, 141);
            this.groupBoxSpecFilter.Name = "groupBoxSpecFilter";
            this.groupBoxSpecFilter.Size = new System.Drawing.Size(900, 35);
            this.groupBoxSpecFilter.TabIndex = 3;
            this.groupBoxSpecFilter.TabStop = false;
            // 
            // chkUseSpecFilter
            // 
            this.chkUseSpecFilter.AutoSize = true;
            this.chkUseSpecFilter.Location = new System.Drawing.Point(15, 9);
            this.chkUseSpecFilter.Name = "chkUseSpecFilter";
            this.chkUseSpecFilter.Size = new System.Drawing.Size(122, 24);
            this.chkUseSpecFilter.TabIndex = 0;
            this.chkUseSpecFilter.Text = "Spec Filter Use";
            this.chkUseSpecFilter.UseVisualStyleBackColor = true;
            this.chkUseSpecFilter.CheckedChanged += new System.EventHandler(this.chkUseSpecFilter_CheckedChanged);
            // 
            // lblSpecJsonPath
            // 
            this.lblSpecJsonPath.AutoSize = true;
            this.lblSpecJsonPath.Location = new System.Drawing.Point(143, 11);
            this.lblSpecJsonPath.Name = "lblSpecJsonPath";
            this.lblSpecJsonPath.Size = new System.Drawing.Size(79, 20);
            this.lblSpecJsonPath.TabIndex = 1;
            this.lblSpecJsonPath.Text = "JSON File:";
            // 
            // txtSpecJsonPath
            // 
            this.txtSpecJsonPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpecJsonPath.Enabled = false;
            this.txtSpecJsonPath.Location = new System.Drawing.Point(228, 7);
            this.txtSpecJsonPath.Name = "txtSpecJsonPath";
            this.txtSpecJsonPath.ReadOnly = true;
            this.txtSpecJsonPath.Size = new System.Drawing.Size(576, 27);
            this.txtSpecJsonPath.TabIndex = 2;
            // 
            // btnBrowseSpecJson
            // 
            this.btnBrowseSpecJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSpecJson.Enabled = false;
            this.btnBrowseSpecJson.Location = new System.Drawing.Point(810, 6);
            this.btnBrowseSpecJson.Name = "btnBrowseSpecJson";
            this.btnBrowseSpecJson.Size = new System.Drawing.Size(75, 27);
            this.btnBrowseSpecJson.TabIndex = 3;
            this.btnBrowseSpecJson.Text = "Browse";
            this.btnBrowseSpecJson.UseVisualStyleBackColor = true;
            this.btnBrowseSpecJson.Click += new System.EventHandler(this.btnBrowseSpecJson_Click);
            // 
            // btnStartInspection
            // 
            this.btnStartInspection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartInspection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnStartInspection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartInspection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartInspection.ForeColor = System.Drawing.Color.White;
            this.btnStartInspection.Location = new System.Drawing.Point(919, 85);
            this.btnStartInspection.Name = "btnStartInspection";
            this.btnStartInspection.Size = new System.Drawing.Size(248, 91);
            this.btnStartInspection.TabIndex = 2;
            this.btnStartInspection.Text = "▶ Start Inspection";
            this.btnStartInspection.UseVisualStyleBackColor = false;
            this.btnStartInspection.Click += new System.EventHandler(this.btnStartInspection_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 190);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.leftPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(1180, 510);
            this.splitContainer.SplitterDistance = 500;
            this.splitContainer.TabIndex = 1;
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.groupBoxResults);
            this.leftPanel.Controls.Add(this.bottomLeftPanel);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(10, 0, 5, 10);
            this.leftPanel.Size = new System.Drawing.Size(500, 580);
            this.leftPanel.TabIndex = 0;
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.listViewResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(10, 0);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxResults.Size = new System.Drawing.Size(485, 490);
            this.groupBoxResults.TabIndex = 0;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Inspection Results List";
            // 
            // listViewResults
            // 
            this.listViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(10, 30);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(465, 450);
            this.listViewResults.TabIndex = 0;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            // 
            // bottomLeftPanel
            // 
            this.bottomLeftPanel.Controls.Add(this.btnSaveResults);
            this.bottomLeftPanel.Controls.Add(this.btnClearResults);
            this.bottomLeftPanel.Controls.Add(this.lblProgress);
            this.bottomLeftPanel.Controls.Add(this.progressBar);
            this.bottomLeftPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomLeftPanel.Location = new System.Drawing.Point(10, 490);
            this.bottomLeftPanel.Name = "bottomLeftPanel";
            this.bottomLeftPanel.Size = new System.Drawing.Size(485, 80);
            this.bottomLeftPanel.TabIndex = 1;
            // 
            // btnSaveResults
            // 
            this.btnSaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveResults.Location = new System.Drawing.Point(255, 45);
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.Size = new System.Drawing.Size(110, 30);
            this.btnSaveResults.TabIndex = 2;
            this.btnSaveResults.Text = "Results Save";
            this.btnSaveResults.UseVisualStyleBackColor = true;
            this.btnSaveResults.Click += new System.EventHandler(this.btnSaveResults_Click);
            // 
            // btnClearResults
            // 
            this.btnClearResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearResults.Location = new System.Drawing.Point(371, 45);
            this.btnClearResults.Name = "btnClearResults";
            this.btnClearResults.Size = new System.Drawing.Size(110, 30);
            this.btnClearResults.TabIndex = 3;
            this.btnClearResults.Text = "Results Reset";
            this.btnClearResults.UseVisualStyleBackColor = true;
            this.btnClearResults.Click += new System.EventHandler(this.btnClearResults_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(5, 10);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(39, 20);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = "0 / 0";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(5, 35);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(244, 23);
            this.progressBar.TabIndex = 1;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.groupBoxPreview);
            this.rightPanel.Controls.Add(this.groupBoxLog);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(5, 0, 10, 10);
            this.rightPanel.Size = new System.Drawing.Size(676, 580);
            this.rightPanel.TabIndex = 0;
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Controls.Add(this.pictureBoxPreview);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(5, 0);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxPreview.Size = new System.Drawing.Size(661, 430);
            this.groupBoxPreview.TabIndex = 0;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Inspection Results Preview";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(10, 30);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(641, 390);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Controls.Add(this.txtLog);
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxLog.Location = new System.Drawing.Point(5, 430);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxLog.Size = new System.Drawing.Size(661, 140);
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
            this.txtLog.Size = new System.Drawing.Size(641, 100);
            this.txtLog.TabIndex = 0;
            // 
            // InspectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "InspectionControl";
            this.Size = new System.Drawing.Size(1180, 700);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.groupBoxSource.ResumeLayout(false);
            this.groupBoxSource.PerformLayout();
            this.groupBoxSpecFilter.ResumeLayout(false);
            this.groupBoxSpecFilter.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.groupBoxResults.ResumeLayout(false);
            this.bottomLeftPanel.ResumeLayout(false);
            this.bottomLeftPanel.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBoxSource;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Label lblSourceFolder;
        private System.Windows.Forms.TextBox txtFileNameFilter;
        private System.Windows.Forms.Label lblFileNameFilter;
        private System.Windows.Forms.GroupBox groupBoxSpecFilter;
        private System.Windows.Forms.CheckBox chkUseSpecFilter;
        private System.Windows.Forms.Button btnBrowseSpecJson;
        private System.Windows.Forms.TextBox txtSpecJsonPath;
        private System.Windows.Forms.Label lblSpecJsonPath;
        private System.Windows.Forms.Button btnStartInspection;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.Panel bottomLeftPanel;
        private System.Windows.Forms.Button btnSaveResults;
        private System.Windows.Forms.Button btnClearResults;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox txtLog;
    }
}


