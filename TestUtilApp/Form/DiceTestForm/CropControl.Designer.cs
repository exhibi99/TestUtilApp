namespace TestUtilApp.UI
{
    partial class CropControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.lblResultCount = new System.Windows.Forms.Label();
            this.lstCroppedFiles = new System.Windows.Forms.ListBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.lblImageCount = new System.Windows.Forms.Label();
            this.groupBoxCropMode = new System.Windows.Forms.GroupBox();
            this.chkUseManualCrop = new System.Windows.Forms.CheckBox();
            this.groupBoxManualCrop = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numManualX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numManualY = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numManualWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numManualHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBoxDetectModel = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDetModelPath = new System.Windows.Forms.TextBox();
            this.btnBrowseDetModel = new System.Windows.Forms.Button();
            this.lblDetModelStatus = new System.Windows.Forms.Label();
            this.btnLoadCropModel = new System.Windows.Forms.Button();
            this.btnStartCrop = new System.Windows.Forms.Button();
            this.panelRight = new System.Windows.Forms.Panel();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.groupBoxProgress.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxCropMode.SuspendLayout();
            this.groupBoxManualCrop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numManualX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualHeight)).BeginInit();
            this.groupBoxDetectModel.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBoxPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.panelLeft);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panelRight);
            this.splitContainerMain.Size = new System.Drawing.Size(1200, 700);
            this.splitContainerMain.SplitterDistance = 550;
            this.splitContainerMain.TabIndex = 0;
            // 
            // panelLeft
            // 
            this.panelLeft.AutoScroll = true;
            this.panelLeft.Controls.Add(this.groupBoxProgress);
            this.panelLeft.Controls.Add(this.groupBoxResults);
            this.panelLeft.Controls.Add(this.groupBoxSettings);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(5);
            this.panelLeft.Size = new System.Drawing.Size(550, 700);
            this.panelLeft.TabIndex = 0;
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Controls.Add(this.progressBar);
            this.groupBoxProgress.Controls.Add(this.txtLog);
            this.groupBoxProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProgress.Location = new System.Drawing.Point(5, 591);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Padding = new System.Windows.Forms.Padding(5);
            this.groupBoxProgress.Size = new System.Drawing.Size(540, 104);
            this.groupBoxProgress.TabIndex = 2;
            this.groupBoxProgress.TabStop = false;
            this.groupBoxProgress.Text = "Progress";
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(5, 21);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(530, 25);
            this.progressBar.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtLog.Location = new System.Drawing.Point(5, 52);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(530, 47);
            this.txtLog.TabIndex = 1;
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.lblResultCount);
            this.groupBoxResults.Controls.Add(this.lstCroppedFiles);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxResults.Location = new System.Drawing.Point(5, 402);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new System.Drawing.Size(540, 189);
            this.groupBoxResults.TabIndex = 1;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Crop Results";
            // 
            // lblResultCount
            // 
            this.lblResultCount.AutoSize = true;
            this.lblResultCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(199)))), ((int)(((byte)(121)))));
            this.lblResultCount.Location = new System.Drawing.Point(10, 25);
            this.lblResultCount.Name = "lblResultCount";
            this.lblResultCount.Size = new System.Drawing.Size(91, 15);
            this.lblResultCount.TabIndex = 0;
            this.lblResultCount.Text = "Cropped Files: 0";
            // 
            // lstCroppedFiles
            // 
            this.lstCroppedFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCroppedFiles.FormattingEnabled = true;
            this.lstCroppedFiles.ItemHeight = 15;
            this.lstCroppedFiles.Location = new System.Drawing.Point(3, 19);
            this.lstCroppedFiles.Name = "lstCroppedFiles";
            this.lstCroppedFiles.Size = new System.Drawing.Size(534, 167);
            this.lstCroppedFiles.TabIndex = 1;
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.txtSourceFolder);
            this.groupBoxSettings.Controls.Add(this.btnBrowseSource);
            this.groupBoxSettings.Controls.Add(this.lblImageCount);
            this.groupBoxSettings.Controls.Add(this.groupBoxCropMode);
            this.groupBoxSettings.Controls.Add(this.groupBoxDetectModel);
            this.groupBoxSettings.Controls.Add(this.btnLoadCropModel);
            this.groupBoxSettings.Controls.Add(this.btnStartCrop);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSettings.Location = new System.Drawing.Point(5, 5);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(540, 397);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Crop Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Folder";
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Location = new System.Drawing.Point(10, 48);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.ReadOnly = true;
            this.txtSourceFolder.Size = new System.Drawing.Size(420, 23);
            this.txtSourceFolder.TabIndex = 1;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Location = new System.Drawing.Point(436, 46);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(90, 30);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "Browse";
            this.btnBrowseSource.UseVisualStyleBackColor = false;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // lblImageCount
            // 
            this.lblImageCount.AutoSize = true;
            this.lblImageCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(139)))), ((int)(((byte)(236)))));
            this.lblImageCount.Location = new System.Drawing.Point(10, 80);
            this.lblImageCount.Name = "lblImageCount";
            this.lblImageCount.Size = new System.Drawing.Size(94, 15);
            this.lblImageCount.TabIndex = 3;
            this.lblImageCount.Text = "Images Found: 0";
            // 
            // groupBoxCropMode
            // 
            this.groupBoxCropMode.Controls.Add(this.chkUseManualCrop);
            this.groupBoxCropMode.Controls.Add(this.groupBoxManualCrop);
            this.groupBoxCropMode.Location = new System.Drawing.Point(10, 110);
            this.groupBoxCropMode.Name = "groupBoxCropMode";
            this.groupBoxCropMode.Size = new System.Drawing.Size(516, 120);
            this.groupBoxCropMode.TabIndex = 4;
            this.groupBoxCropMode.TabStop = false;
            this.groupBoxCropMode.Text = "Crop Mode";
            // 
            // chkUseManualCrop
            // 
            this.chkUseManualCrop.AutoSize = true;
            this.chkUseManualCrop.Location = new System.Drawing.Point(10, 25);
            this.chkUseManualCrop.Name = "chkUseManualCrop";
            this.chkUseManualCrop.Size = new System.Drawing.Size(210, 19);
            this.chkUseManualCrop.TabIndex = 0;
            this.chkUseManualCrop.Text = "Manual Crop (Disable Auto Detect)";
            this.chkUseManualCrop.UseVisualStyleBackColor = false;
            // 
            // groupBoxManualCrop
            // 
            this.groupBoxManualCrop.Controls.Add(this.label2);
            this.groupBoxManualCrop.Controls.Add(this.numManualX);
            this.groupBoxManualCrop.Controls.Add(this.label3);
            this.groupBoxManualCrop.Controls.Add(this.numManualY);
            this.groupBoxManualCrop.Controls.Add(this.label4);
            this.groupBoxManualCrop.Controls.Add(this.numManualWidth);
            this.groupBoxManualCrop.Controls.Add(this.label5);
            this.groupBoxManualCrop.Controls.Add(this.numManualHeight);
            this.groupBoxManualCrop.Location = new System.Drawing.Point(10, 55);
            this.groupBoxManualCrop.Name = "groupBoxManualCrop";
            this.groupBoxManualCrop.Size = new System.Drawing.Size(495, 55);
            this.groupBoxManualCrop.TabIndex = 1;
            this.groupBoxManualCrop.TabStop = false;
            this.groupBoxManualCrop.Text = "Area";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "X";
            // 
            // numManualX
            // 
            this.numManualX.Location = new System.Drawing.Point(32, 23);
            this.numManualX.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numManualX.Name = "numManualX";
            this.numManualX.Size = new System.Drawing.Size(80, 23);
            this.numManualX.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y";
            // 
            // numManualY
            // 
            this.numManualY.Location = new System.Drawing.Point(144, 23);
            this.numManualY.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numManualY.Name = "numManualY";
            this.numManualY.Size = new System.Drawing.Size(80, 23);
            this.numManualY.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(234, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "W";
            // 
            // numManualWidth
            // 
            this.numManualWidth.Location = new System.Drawing.Point(256, 23);
            this.numManualWidth.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numManualWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numManualWidth.Name = "numManualWidth";
            this.numManualWidth.Size = new System.Drawing.Size(80, 23);
            this.numManualWidth.TabIndex = 5;
            this.numManualWidth.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(346, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "H";
            // 
            // numManualHeight
            // 
            this.numManualHeight.Location = new System.Drawing.Point(368, 23);
            this.numManualHeight.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numManualHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numManualHeight.Name = "numManualHeight";
            this.numManualHeight.Size = new System.Drawing.Size(80, 23);
            this.numManualHeight.TabIndex = 7;
            this.numManualHeight.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // groupBoxDetectModel
            // 
            this.groupBoxDetectModel.Controls.Add(this.label6);
            this.groupBoxDetectModel.Controls.Add(this.txtDetModelPath);
            this.groupBoxDetectModel.Controls.Add(this.btnBrowseDetModel);
            this.groupBoxDetectModel.Controls.Add(this.lblDetModelStatus);
            this.groupBoxDetectModel.Location = new System.Drawing.Point(10, 240);
            this.groupBoxDetectModel.Name = "groupBoxDetectModel";
            this.groupBoxDetectModel.Size = new System.Drawing.Size(516, 114);
            this.groupBoxDetectModel.TabIndex = 5;
            this.groupBoxDetectModel.TabStop = false;
            this.groupBoxDetectModel.Text = "Detection Model (Auto Detect)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Model Path";
            // 
            // txtDetModelPath
            // 
            this.txtDetModelPath.Location = new System.Drawing.Point(10, 48);
            this.txtDetModelPath.Name = "txtDetModelPath";
            this.txtDetModelPath.Size = new System.Drawing.Size(390, 23);
            this.txtDetModelPath.TabIndex = 1;
            this.txtDetModelPath.Leave += new System.EventHandler(this.txtDetModelPath_Leave);
            // 
            // btnBrowseDetModel
            // 
            this.btnBrowseDetModel.Location = new System.Drawing.Point(410, 46);
            this.btnBrowseDetModel.Name = "btnBrowseDetModel";
            this.btnBrowseDetModel.Size = new System.Drawing.Size(90, 30);
            this.btnBrowseDetModel.TabIndex = 2;
            this.btnBrowseDetModel.Text = "Browse";
            this.btnBrowseDetModel.UseVisualStyleBackColor = false;
            this.btnBrowseDetModel.Click += new System.EventHandler(this.btnBrowseDetModel_Click);
            // 
            // lblDetModelStatus
            // 
            this.lblDetModelStatus.AutoSize = true;
            this.lblDetModelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDetModelStatus.Location = new System.Drawing.Point(10, 85);
            this.lblDetModelStatus.Name = "lblDetModelStatus";
            this.lblDetModelStatus.Size = new System.Drawing.Size(59, 15);
            this.lblDetModelStatus.TabIndex = 3;
            this.lblDetModelStatus.Text = "Loading...";
            // 
            // btnLoadCropModel
            // 
            this.btnLoadCropModel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadCropModel.Location = new System.Drawing.Point(250, 359);
            this.btnLoadCropModel.Name = "btnLoadCropModel";
            this.btnLoadCropModel.Size = new System.Drawing.Size(130, 32);
            this.btnLoadCropModel.TabIndex = 6;
            this.btnLoadCropModel.Text = "Load Model";
            this.btnLoadCropModel.UseVisualStyleBackColor = false;
            this.btnLoadCropModel.Click += new System.EventHandler(this.btnLoadCropModel_Click);
            // 
            // btnStartCrop
            // 
            this.btnStartCrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnStartCrop.Enabled = false;
            this.btnStartCrop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartCrop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartCrop.ForeColor = System.Drawing.Color.White;
            this.btnStartCrop.Location = new System.Drawing.Point(396, 359);
            this.btnStartCrop.Name = "btnStartCrop";
            this.btnStartCrop.Size = new System.Drawing.Size(130, 32);
            this.btnStartCrop.TabIndex = 7;
            this.btnStartCrop.Text = "Start Crop";
            this.btnStartCrop.UseVisualStyleBackColor = false;
            this.btnStartCrop.Click += new System.EventHandler(this.btnStartCrop_Click);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.groupBoxPreview);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(5);
            this.panelRight.Size = new System.Drawing.Size(646, 700);
            this.panelRight.TabIndex = 0;
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Controls.Add(this.pictureBoxPreview);
            this.groupBoxPreview.Controls.Add(this.lblPreviewInfo);
            this.groupBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPreview.Location = new System.Drawing.Point(5, 5);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Padding = new System.Windows.Forms.Padding(5);
            this.groupBoxPreview.Size = new System.Drawing.Size(636, 690);
            this.groupBoxPreview.TabIndex = 0;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Preview (Display Detection Area)";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.Black;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Location = new System.Drawing.Point(5, 21);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(626, 634);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // lblPreviewInfo
            // 
            this.lblPreviewInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPreviewInfo.Location = new System.Drawing.Point(5, 655);
            this.lblPreviewInfo.Name = "lblPreviewInfo";
            this.lblPreviewInfo.Padding = new System.Windows.Forms.Padding(5);
            this.lblPreviewInfo.Size = new System.Drawing.Size(626, 30);
            this.lblPreviewInfo.TabIndex = 1;
            this.lblPreviewInfo.Text = "Select a file.";
            this.lblPreviewInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CropControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Name = "CropControl";
            this.Size = new System.Drawing.Size(1200, 700);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBoxProgress.PerformLayout();
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxResults.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxCropMode.ResumeLayout(false);
            this.groupBoxCropMode.PerformLayout();
            this.groupBoxManualCrop.ResumeLayout(false);
            this.groupBoxManualCrop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numManualX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numManualHeight)).EndInit();
            this.groupBoxDetectModel.ResumeLayout(false);
            this.groupBoxDetectModel.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.groupBoxPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Label lblImageCount;
        private System.Windows.Forms.GroupBox groupBoxCropMode;
        private System.Windows.Forms.CheckBox chkUseManualCrop;
        private System.Windows.Forms.GroupBox groupBoxManualCrop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numManualX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numManualY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numManualWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numManualHeight;
        private System.Windows.Forms.GroupBox groupBoxDetectModel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDetModelPath;
        private System.Windows.Forms.Button btnBrowseDetModel;
        private System.Windows.Forms.Label lblDetModelStatus;
        private System.Windows.Forms.Button btnLoadCropModel;
        private System.Windows.Forms.Button btnStartCrop;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.Label lblResultCount;
        private System.Windows.Forms.ListBox lstCroppedFiles;
        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label lblPreviewInfo;
    }
}
