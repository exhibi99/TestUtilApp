namespace TestUtilApp.UI
{
    partial class LabelGeneratorControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.rbShowImages = new System.Windows.Forms.RadioButton();
            this.rbShowLabels = new System.Windows.Forms.RadioButton();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.columnFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numMinConfidence = new System.Windows.Forms.NumericUpDown();
            this.chkSkipExistingJson = new System.Windows.Forms.CheckBox();
            this.labelDetectModel = new System.Windows.Forms.Label();
            this.lblDetectModelInfo = new System.Windows.Forms.Label();
            this.btnStartLabelGen = new System.Windows.Forms.Button();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.panelPreviewInfo = new System.Windows.Forms.Panel();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxProgress.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinConfidence)).BeginInit();
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
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.rbShowImages);
            this.groupBoxResults.Controls.Add(this.rbShowLabels);
            this.groupBoxResults.Controls.Add(this.listViewResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(0, 410);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxResults.Size = new System.Drawing.Size(700, 270);
            this.groupBoxResults.TabIndex = 2;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Generate Results";
            // 
            // rbShowImages
            // 
            this.rbShowImages.AutoSize = true;
            this.rbShowImages.Checked = true;
            this.rbShowImages.Location = new System.Drawing.Point(13, 30);
            this.rbShowImages.Name = "rbShowImages";
            this.rbShowImages.Size = new System.Drawing.Size(232, 24);
            this.rbShowImages.TabIndex = 0;
            this.rbShowImages.TabStop = true;
            this.rbShowImages.Text = "Image + Detection Display Results";
            this.rbShowImages.UseVisualStyleBackColor = true;
            this.rbShowImages.CheckedChanged += new System.EventHandler(this.rbShowMode_CheckedChanged);
            // 
            // rbShowLabels
            // 
            this.rbShowLabels.AutoSize = true;
            this.rbShowLabels.Location = new System.Drawing.Point(260, 30);
            this.rbShowLabels.Name = "rbShowLabels";
            this.rbShowLabels.Size = new System.Drawing.Size(187, 24);
            this.rbShowLabels.TabIndex = 1;
            this.rbShowLabels.Text = "Label + JSON Parsing Display";
            this.rbShowLabels.UseVisualStyleBackColor = true;
            this.rbShowLabels.CheckedChanged += new System.EventHandler(this.rbShowMode_CheckedChanged);
            // 
            // listViewResults
            // 
            this.listViewResults.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFileName,
            this.columnPath});
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.GridLines = true;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(10, 60);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(680, 200);
            this.listViewResults.TabIndex = 2;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = System.Windows.Forms.View.Details;
            this.listViewResults.SelectedIndexChanged += new System.EventHandler(this.listViewResults_SelectedIndexChanged);
            // 
            // columnFileName
            // 
            this.columnFileName.Text = "Filename";
            this.columnFileName.Width = 250;
            // 
            // columnPath
            // 
            this.columnPath.Text = "Path";
            this.columnPath.Width = 400;
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Controls.Add(this.progressBar);
            this.groupBoxProgress.Controls.Add(this.txtLog);
            this.groupBoxProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxProgress.Location = new System.Drawing.Point(0, 230);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxProgress.Size = new System.Drawing.Size(700, 180);
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
            this.txtLog.Location = new System.Drawing.Point(10, 30);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(680, 140);
            this.txtLog.TabIndex = 1;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.txtSourceFolder);
            this.groupBoxSettings.Controls.Add(this.btnBrowseSource);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Controls.Add(this.numMinConfidence);
            this.groupBoxSettings.Controls.Add(this.chkSkipExistingJson);
            this.groupBoxSettings.Controls.Add(this.labelDetectModel);
            this.groupBoxSettings.Controls.Add(this.lblDetectModelInfo);
            this.groupBoxSettings.Controls.Add(this.btnStartLabelGen);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 0);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxSettings.Size = new System.Drawing.Size(700, 230);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Label Gen Settings";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minimum Confidence (0~1)";
            // 
            // numMinConfidence
            // 
            this.numMinConfidence.DecimalPlaces = 2;
            this.numMinConfidence.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numMinConfidence.Location = new System.Drawing.Point(13, 118);
            this.numMinConfidence.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinConfidence.Name = "numMinConfidence";
            this.numMinConfidence.Size = new System.Drawing.Size(150, 27);
            this.numMinConfidence.TabIndex = 4;
            this.numMinConfidence.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // chkSkipExistingJson
            // 
            this.chkSkipExistingJson.AutoSize = true;
            this.chkSkipExistingJson.Checked = true;
            this.chkSkipExistingJson.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkipExistingJson.Location = new System.Drawing.Point(200, 120);
            this.chkSkipExistingJson.Name = "chkSkipExistingJson";
            this.chkSkipExistingJson.Size = new System.Drawing.Size(202, 24);
            this.chkSkipExistingJson.TabIndex = 5;
            this.chkSkipExistingJson.Text = "Existing JSON File Skip";
            this.chkSkipExistingJson.UseVisualStyleBackColor = true;
            // 
            // labelDetectModel
            // 
            this.labelDetectModel.AutoSize = true;
            this.labelDetectModel.Location = new System.Drawing.Point(13, 158);
            this.labelDetectModel.Name = "labelDetectModel";
            this.labelDetectModel.Size = new System.Drawing.Size(110, 20);
            this.labelDetectModel.TabIndex = 6;
            this.labelDetectModel.Text = "Detection Model";
            // 
            // lblDetectModelInfo
            // 
            this.lblDetectModelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDetectModelInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDetectModelInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblDetectModelInfo.Location = new System.Drawing.Point(13, 178);
            this.lblDetectModelInfo.Name = "lblDetectModelInfo";
            this.lblDetectModelInfo.Size = new System.Drawing.Size(520, 23);
            this.lblDetectModelInfo.TabIndex = 7;
            this.lblDetectModelInfo.Text = "Loading...";
            this.lblDetectModelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnStartLabelGen
            // 
            this.btnStartLabelGen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartLabelGen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnStartLabelGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartLabelGen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartLabelGen.ForeColor = System.Drawing.Color.White;
            this.btnStartLabelGen.Location = new System.Drawing.Point(539, 175);
            this.btnStartLabelGen.Name = "btnStartLabelGen";
            this.btnStartLabelGen.Size = new System.Drawing.Size(140, 40);
            this.btnStartLabelGen.TabIndex = 8;
            this.btnStartLabelGen.Text = "Label Generate";
            this.btnStartLabelGen.UseVisualStyleBackColor = false;
            this.btnStartLabelGen.Click += new System.EventHandler(this.btnStartLabelGen_Click);
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
            // LabelGeneratorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "LabelGeneratorControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1200, 700);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxResults.PerformLayout();
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBoxProgress.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinConfidence)).EndInit();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMinConfidence;
        private System.Windows.Forms.CheckBox chkSkipExistingJson;
        private System.Windows.Forms.Label labelDetectModel;
        private System.Windows.Forms.Label lblDetectModelInfo;
        private System.Windows.Forms.Button btnStartLabelGen;
        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.RadioButton rbShowImages;
        private System.Windows.Forms.RadioButton rbShowLabels;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ColumnHeader columnFileName;
        private System.Windows.Forms.ColumnHeader columnPath;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Panel panelPreviewInfo;
        private System.Windows.Forms.Label lblPreviewInfo;
    }
}
