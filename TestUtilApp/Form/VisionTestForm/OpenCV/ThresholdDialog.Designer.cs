namespace TestUtilApp.UI
{
    partial class ThresholdDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblThreshValue = new System.Windows.Forms.Label();
            this.nudThreshValue = new System.Windows.Forms.NumericUpDown();
            this.lblMaxValue = new System.Windows.Forms.Label();
            this.nudMaxValue = new System.Windows.Forms.NumericUpDown();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxValue)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // lblType
            //
            this.lblType.AutoSize = false;
            this.lblType.Location = new System.Drawing.Point(14, 16);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(240, 16);
            this.lblType.Text = "Type";
            //
            // cmbType
            //
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Items.AddRange(new object[] {
                "Binary",
                "BinaryInv",
                "Truncate",
                "ToZero",
                "ToZeroInv"});
            this.cmbType.Location = new System.Drawing.Point(14, 34);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(240, 22);
            this.cmbType.TabIndex = 0;
            this.cmbType.SelectedIndex = 0;
            //
            // lblThreshValue
            //
            this.lblThreshValue.AutoSize = false;
            this.lblThreshValue.Location = new System.Drawing.Point(14, 68);
            this.lblThreshValue.Name = "lblThreshValue";
            this.lblThreshValue.Size = new System.Drawing.Size(240, 16);
            this.lblThreshValue.Text = "Threshold Value  (0 – 255)";
            //
            // nudThreshValue
            //
            this.nudThreshValue.Location = new System.Drawing.Point(14, 86);
            this.nudThreshValue.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.nudThreshValue.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.nudThreshValue.Name = "nudThreshValue";
            this.nudThreshValue.Size = new System.Drawing.Size(240, 24);
            this.nudThreshValue.TabIndex = 1;
            this.nudThreshValue.Value = new decimal(new int[] { 127, 0, 0, 0 });
            //
            // lblMaxValue
            //
            this.lblMaxValue.AutoSize = false;
            this.lblMaxValue.Location = new System.Drawing.Point(14, 120);
            this.lblMaxValue.Name = "lblMaxValue";
            this.lblMaxValue.Size = new System.Drawing.Size(240, 16);
            this.lblMaxValue.Text = "Max Value  (0 – 255)";
            //
            // nudMaxValue
            //
            this.nudMaxValue.Location = new System.Drawing.Point(14, 138);
            this.nudMaxValue.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.nudMaxValue.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.nudMaxValue.Name = "nudMaxValue";
            this.nudMaxValue.Size = new System.Drawing.Size(240, 24);
            this.nudMaxValue.TabIndex = 2;
            this.nudMaxValue.Value = new decimal(new int[] { 255, 0, 0, 0 });
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 178);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(270, 44);
            this.pnlButtons.TabIndex = 3;
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(62, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            //
            // btnOK
            //
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(162, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(92, 28);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // ThresholdDialog
            //
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 222);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.nudMaxValue);
            this.Controls.Add(this.lblMaxValue);
            this.Controls.Add(this.nudThreshValue);
            this.Controls.Add(this.lblThreshValue);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblType);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThresholdDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Threshold";
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxValue)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblThreshValue;
        private System.Windows.Forms.NumericUpDown nudThreshValue;
        private System.Windows.Forms.Label lblMaxValue;
        private System.Windows.Forms.NumericUpDown nudMaxValue;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}
