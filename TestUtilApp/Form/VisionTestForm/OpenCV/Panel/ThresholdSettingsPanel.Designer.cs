namespace TestUtilApp.UI
{
    partial class ThresholdSettingsPanel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblType        = new System.Windows.Forms.Label();
            this.cmbType        = new System.Windows.Forms.ComboBox();
            this.lblThresh      = new System.Windows.Forms.Label();
            this.nudThreshValue = new System.Windows.Forms.NumericUpDown();
            this.lblMax         = new System.Windows.Forms.Label();
            this.nudMaxValue    = new System.Windows.Forms.NumericUpDown();
            this.btnApply       = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxValue)).BeginInit();
            this.SuspendLayout();

            // lblType
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(4, 6);
            this.lblType.Name     = "lblType";
            this.lblType.Text     = "Type:";

            // cmbType
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.Location      = new System.Drawing.Point(52, 3);
            this.cmbType.Name          = "cmbType";
            this.cmbType.Size          = new System.Drawing.Size(160, 23);
            this.cmbType.TabIndex      = 0;

            // lblThresh
            this.lblThresh.AutoSize = true;
            this.lblThresh.Location = new System.Drawing.Point(4, 35);
            this.lblThresh.Name     = "lblThresh";
            this.lblThresh.Text     = "Thresh:";

            // nudThreshValue
            this.nudThreshValue.DecimalPlaces = 1;
            this.nudThreshValue.Location      = new System.Drawing.Point(52, 32);
            this.nudThreshValue.Maximum       = new decimal(new int[] { 255, 0, 0, 0 });
            this.nudThreshValue.Name          = "nudThreshValue";
            this.nudThreshValue.Size          = new System.Drawing.Size(72, 23);
            this.nudThreshValue.TabIndex      = 1;
            this.nudThreshValue.Value         = new decimal(new int[] { 127, 0, 0, 0 });

            // lblMax
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(132, 35);
            this.lblMax.Name     = "lblMax";
            this.lblMax.Text     = "Max:";

            // nudMaxValue
            this.nudMaxValue.DecimalPlaces = 1;
            this.nudMaxValue.Location      = new System.Drawing.Point(168, 32);
            this.nudMaxValue.Maximum       = new decimal(new int[] { 255, 0, 0, 0 });
            this.nudMaxValue.Name          = "nudMaxValue";
            this.nudMaxValue.Size          = new System.Drawing.Size(72, 23);
            this.nudMaxValue.TabIndex      = 2;
            this.nudMaxValue.Value         = new decimal(new int[] { 255, 0, 0, 0 });

            // btnApply
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApply.Location  = new System.Drawing.Point(168, 61);
            this.btnApply.Name      = "btnApply";
            this.btnApply.Size      = new System.Drawing.Size(72, 26);
            this.btnApply.TabIndex  = 3;
            this.btnApply.Text      = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click    += new System.EventHandler(this.btnApply_Click);

            // ThresholdSettingsPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblThresh);
            this.Controls.Add(this.nudThreshValue);
            this.Controls.Add(this.lblMax);
            this.Controls.Add(this.nudMaxValue);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ThresholdSettingsPanel";
            this.Size = new System.Drawing.Size(260, 95);

            ((System.ComponentModel.ISupportInitialize)(this.nudThreshValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label          lblType;
        private System.Windows.Forms.ComboBox       cmbType;
        private System.Windows.Forms.Label          lblThresh;
        private System.Windows.Forms.NumericUpDown  nudThreshValue;
        private System.Windows.Forms.Label          lblMax;
        private System.Windows.Forms.NumericUpDown  nudMaxValue;
        private System.Windows.Forms.Button         btnApply;
    }
}
