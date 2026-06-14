namespace TestUtilApp.UI
{
    partial class AcquireDialog
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
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtImagePath = new System.Windows.Forms.TextBox();
            this.lblFile = new System.Windows.Forms.Label();
            this.grpColor = new System.Windows.Forms.GroupBox();
            this.radioGray = new System.Windows.Forms.RadioButton();
            this.radioOriginal = new System.Windows.Forms.RadioButton();
            this.lblPreview = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSource.SuspendLayout();
            this.grpColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            //
            // grpSource
            //
            this.grpSource.Controls.Add(this.btnBrowse);
            this.grpSource.Controls.Add(this.txtImagePath);
            this.grpSource.Controls.Add(this.lblFile);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(596, 65);
            this.grpSource.TabIndex = 0;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Image Source";
            //
            // btnBrowse
            //
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Location = new System.Drawing.Point(510, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // txtImagePath
            //
            this.txtImagePath.Location = new System.Drawing.Point(48, 29);
            this.txtImagePath.Name = "txtImagePath";
            this.txtImagePath.ReadOnly = true;
            this.txtImagePath.Size = new System.Drawing.Size(452, 23);
            this.txtImagePath.TabIndex = 1;
            //
            // lblFile
            //
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(10, 32);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(30, 20);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "File:";
            //
            // grpColor
            //
            this.grpColor.Controls.Add(this.radioGray);
            this.grpColor.Controls.Add(this.radioOriginal);
            this.grpColor.Location = new System.Drawing.Point(12, 87);
            this.grpColor.Name = "grpColor";
            this.grpColor.Size = new System.Drawing.Size(200, 80);
            this.grpColor.TabIndex = 1;
            this.grpColor.TabStop = false;
            this.grpColor.Text = "Color Mode";
            //
            // radioGray
            //
            this.radioGray.AutoSize = true;
            this.radioGray.Location = new System.Drawing.Point(20, 50);
            this.radioGray.Name = "radioGray";
            this.radioGray.Size = new System.Drawing.Size(50, 20);
            this.radioGray.TabIndex = 1;
            this.radioGray.Text = "Gray";
            this.radioGray.UseVisualStyleBackColor = true;
            this.radioGray.CheckedChanged += new System.EventHandler(this.radioColor_CheckedChanged);
            //
            // radioOriginal
            //
            this.radioOriginal.AutoSize = true;
            this.radioOriginal.Checked = true;
            this.radioOriginal.Location = new System.Drawing.Point(20, 24);
            this.radioOriginal.Name = "radioOriginal";
            this.radioOriginal.Size = new System.Drawing.Size(67, 20);
            this.radioOriginal.TabIndex = 0;
            this.radioOriginal.TabStop = true;
            this.radioOriginal.Text = "Original";
            this.radioOriginal.UseVisualStyleBackColor = true;
            this.radioOriginal.CheckedChanged += new System.EventHandler(this.radioColor_CheckedChanged);
            //
            // lblPreview
            //
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(225, 90);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(57, 20);
            this.lblPreview.TabIndex = 2;
            this.lblPreview.Text = "Preview:";
            //
            // pictureBoxPreview
            //
            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(10)))), ((int)(((byte)(14)))));
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Location = new System.Drawing.Point(225, 113);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(383, 172);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            //
            // btnOK
            //
            this.btnOK.Enabled = false;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(451, 300);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(539, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            //
            // AcquireDialog
            //
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(620, 342);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.grpColor);
            this.Controls.Add(this.grpSource);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AcquireDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Acquire Image";
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpColor.ResumeLayout(false);
            this.grpColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtImagePath;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.GroupBox grpColor;
        private System.Windows.Forms.RadioButton radioGray;
        private System.Windows.Forms.RadioButton radioOriginal;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
