namespace TestUtilApp.UI
{
    partial class FilterControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseTarget = new System.Windows.Forms.Button();
            this.chkAllSubfolders = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIncludeKeywords = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExcludeKeywords = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExcludeExtensions = new System.Windows.Forms.TextBox();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbCopy = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOutputPostfix = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxOptions.SuspendLayout();
            this.groupBoxProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.txtTargetFolder);
            this.groupBoxSettings.Controls.Add(this.btnBrowseTarget);
            this.groupBoxSettings.Controls.Add(this.chkAllSubfolders);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Controls.Add(this.txtIncludeKeywords);
            this.groupBoxSettings.Controls.Add(this.label3);
            this.groupBoxSettings.Controls.Add(this.txtExcludeKeywords);
            this.groupBoxSettings.Controls.Add(this.label5);
            this.groupBoxSettings.Controls.Add(this.txtExcludeExtensions);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSettings.Location = new System.Drawing.Point(10, 10);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxSettings.Size = new System.Drawing.Size(1180, 300);
            this.groupBoxSettings.TabIndex = 0;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Filtering Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target Folder";
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFolder.Location = new System.Drawing.Point(13, 53);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.ReadOnly = true;
            this.txtTargetFolder.Size = new System.Drawing.Size(1050, 27);
            this.txtTargetFolder.TabIndex = 1;
            // 
            // btnBrowseTarget
            // 
            this.btnBrowseTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseTarget.Location = new System.Drawing.Point(1069, 51);
            this.btnBrowseTarget.Name = "btnBrowseTarget";
            this.btnBrowseTarget.Size = new System.Drawing.Size(90, 30);
            this.btnBrowseTarget.TabIndex = 2;
            this.btnBrowseTarget.Text = "Browse";
            this.btnBrowseTarget.UseVisualStyleBackColor = true;
            this.btnBrowseTarget.Click += new System.EventHandler(this.btnBrowseTarget_Click);
            // 
            // chkAllSubfolders
            // 
            this.chkAllSubfolders.AutoSize = true;
            this.chkAllSubfolders.Checked = true;
            this.chkAllSubfolders.Location = new System.Drawing.Point(13, 95);
            this.chkAllSubfolders.Name = "chkAllSubfolders";
            this.chkAllSubfolders.Size = new System.Drawing.Size(186, 24);
            this.chkAllSubfolders.TabIndex = 3;
            this.chkAllSubfolders.Text = "Include all subfolders";
            this.chkAllSubfolders.UseVisualStyleBackColor = true;
            this.chkAllSubfolders.CheckedChanged += new System.EventHandler(this.chkAllSubfolders_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Folder keywords to include (comma-separated)";
            // 
            // txtIncludeKeywords
            // 
            this.txtIncludeKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIncludeKeywords.Location = new System.Drawing.Point(13, 153);
            this.txtIncludeKeywords.Name = "txtIncludeKeywords";
            this.txtIncludeKeywords.Size = new System.Drawing.Size(1146, 27);
            this.txtIncludeKeywords.TabIndex = 5;
            this.txtIncludeKeywords.Text = "LED, Blade";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(305, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "File keywords to exclude (comma-separated)";
            // 
            // txtExcludeKeywords
            // 
            this.txtExcludeKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExcludeKeywords.Location = new System.Drawing.Point(13, 213);
            this.txtExcludeKeywords.Name = "txtExcludeKeywords";
            this.txtExcludeKeywords.Size = new System.Drawing.Size(1146, 27);
            this.txtExcludeKeywords.TabIndex = 7;
            this.txtExcludeKeywords.Text = "_temp, backup";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 250);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(372, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Extensions to exclude (no dot, comma-separated, e.g. jpg, png)";
            // 
            // txtExcludeExtensions
            // 
            this.txtExcludeExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExcludeExtensions.Location = new System.Drawing.Point(13, 273);
            this.txtExcludeExtensions.Name = "txtExcludeExtensions";
            this.txtExcludeExtensions.Size = new System.Drawing.Size(1146, 27);
            this.txtExcludeExtensions.TabIndex = 9;
            this.txtExcludeExtensions.Text = "tmp, bak";
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.rbDelete);
            this.groupBoxOptions.Controls.Add(this.rbCopy);
            this.groupBoxOptions.Controls.Add(this.label4);
            this.groupBoxOptions.Controls.Add(this.txtOutputPostfix);
            this.groupBoxOptions.Controls.Add(this.btnExecute);
            this.groupBoxOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxOptions.Location = new System.Drawing.Point(10, 310);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxOptions.Size = new System.Drawing.Size(1180, 110);
            this.groupBoxOptions.TabIndex = 1;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "Execution Options";
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(13, 35);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(264, 24);
            this.rbDelete.TabIndex = 0;
            this.rbDelete.Text = "Delete files that match the exclude conditions";
            this.rbDelete.UseVisualStyleBackColor = true;
            // 
            // rbCopy
            // 
            this.rbCopy.AutoSize = true;
            this.rbCopy.Checked = true;
            this.rbCopy.Location = new System.Drawing.Point(300, 35);
            this.rbCopy.Name = "rbCopy";
            this.rbCopy.Size = new System.Drawing.Size(295, 24);
            this.rbCopy.TabIndex = 1;
            this.rbCopy.TabStop = true;
            this.rbCopy.Text = "Copy only allowed files to a new folder";
            this.rbCopy.UseVisualStyleBackColor = true;
            this.rbCopy.CheckedChanged += new System.EventHandler(this.rbCopy_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Output folder postfix (copy mode)";
            // 
            // txtOutputPostfix
            // 
            this.txtOutputPostfix.Location = new System.Drawing.Point(200, 67);
            this.txtOutputPostfix.Name = "txtOutputPostfix";
            this.txtOutputPostfix.Size = new System.Drawing.Size(200, 27);
            this.txtOutputPostfix.TabIndex = 3;
            this.txtOutputPostfix.Text = "_filtered";
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecute.ForeColor = System.Drawing.Color.White;
            this.btnExecute.Location = new System.Drawing.Point(1019, 55);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(140, 40);
            this.btnExecute.TabIndex = 4;
            this.btnExecute.Text = "Start Filtering";
            this.btnExecute.UseVisualStyleBackColor = false;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Controls.Add(this.progressBar);
            this.groupBoxProgress.Controls.Add(this.txtLog);
            this.groupBoxProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProgress.Location = new System.Drawing.Point(10, 420);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxProgress.Size = new System.Drawing.Size(1180, 270);
            this.groupBoxProgress.TabIndex = 2;
            this.groupBoxProgress.TabStop = false;
            this.groupBoxProgress.Text = "Progress";
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(10, 30);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1160, 23);
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
            this.txtLog.Size = new System.Drawing.Size(1160, 237);
            this.txtLog.TabIndex = 1;
            // 
            // FilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxProgress);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.groupBoxSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FilterControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1200, 700);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBoxProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.Button btnBrowseTarget;
        private System.Windows.Forms.CheckBox chkAllSubfolders;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIncludeKeywords;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExcludeKeywords;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtExcludeExtensions;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbCopy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOutputPostfix;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;
    }
}
