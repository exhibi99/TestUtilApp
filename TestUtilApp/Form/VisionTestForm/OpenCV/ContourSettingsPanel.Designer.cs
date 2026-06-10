namespace TestUtilApp.UI
{
    partial class ContourSettingsPanel
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
            this.lblRetr      = new System.Windows.Forms.Label();
            this.cmbRetr      = new System.Windows.Forms.ComboBox();
            this.lblApprox    = new System.Windows.Forms.Label();
            this.cmbApprox    = new System.Windows.Forms.ComboBox();
            this.lblMinArea   = new System.Windows.Forms.Label();
            this.nudMinArea   = new System.Windows.Forms.NumericUpDown();
            this.lblMaxArea   = new System.Windows.Forms.Label();
            this.nudMaxArea   = new System.Windows.Forms.NumericUpDown();
            this.lblThickness = new System.Windows.Forms.Label();
            this.nudThickness = new System.Windows.Forms.NumericUpDown();
            this.lblColor      = new System.Windows.Forms.Label();
            this.cmbColor      = new System.Windows.Forms.ComboBox();
            this.chkAutoInvert = new System.Windows.Forms.CheckBox();
            this.btnApply      = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThickness)).BeginInit();
            this.SuspendLayout();

            // ── Row 1: Retr / Approx ──────────────────────────────
            this.lblRetr.AutoSize = true;
            this.lblRetr.Location = new System.Drawing.Point(4, 6);
            this.lblRetr.Name     = "lblRetr";
            this.lblRetr.Text     = "Retr:";

            this.cmbRetr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRetr.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRetr.Location      = new System.Drawing.Point(42, 3);
            this.cmbRetr.Name          = "cmbRetr";
            this.cmbRetr.Size          = new System.Drawing.Size(90, 23);
            this.cmbRetr.TabIndex      = 0;

            this.lblApprox.AutoSize = true;
            this.lblApprox.Location = new System.Drawing.Point(138, 6);
            this.lblApprox.Name     = "lblApprox";
            this.lblApprox.Text     = "Approx:";

            this.cmbApprox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbApprox.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            this.cmbApprox.Location      = new System.Drawing.Point(188, 3);
            this.cmbApprox.Name          = "cmbApprox";
            this.cmbApprox.Size          = new System.Drawing.Size(64, 23);
            this.cmbApprox.TabIndex      = 1;

            // ── Row 2: MinArea / MaxArea / Thickness ──────────────
            this.lblMinArea.AutoSize = true;
            this.lblMinArea.Location = new System.Drawing.Point(4, 34);
            this.lblMinArea.Name     = "lblMinArea";
            this.lblMinArea.Text     = "Min:";

            this.nudMinArea.DecimalPlaces = 0;
            this.nudMinArea.Location      = new System.Drawing.Point(34, 31);
            this.nudMinArea.Maximum       = new decimal(new int[] { 9999999, 0, 0, 0 });
            this.nudMinArea.Name          = "nudMinArea";
            this.nudMinArea.Size          = new System.Drawing.Size(72, 23);
            this.nudMinArea.TabIndex      = 2;

            this.lblMaxArea.AutoSize = true;
            this.lblMaxArea.Location = new System.Drawing.Point(112, 34);
            this.lblMaxArea.Name     = "lblMaxArea";
            this.lblMaxArea.Text     = "Max:";

            this.nudMaxArea.DecimalPlaces = 0;
            this.nudMaxArea.Location      = new System.Drawing.Point(142, 31);
            this.nudMaxArea.Maximum       = new decimal(new int[] { 9999999, 0, 0, 0 });
            this.nudMaxArea.Name          = "nudMaxArea";
            this.nudMaxArea.Size          = new System.Drawing.Size(72, 23);
            this.nudMaxArea.TabIndex      = 3;

            this.lblThickness.AutoSize = true;
            this.lblThickness.Location = new System.Drawing.Point(220, 34);
            this.lblThickness.Name     = "lblThickness";
            this.lblThickness.Text     = "Px:";

            this.nudThickness.Location = new System.Drawing.Point(238, 31);
            this.nudThickness.Minimum  = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudThickness.Maximum  = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudThickness.Value    = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudThickness.Name     = "nudThickness";
            this.nudThickness.Size     = new System.Drawing.Size(48, 23);
            this.nudThickness.TabIndex = 4;

            // ── Row 3: Color / AutoInvert / Apply ────────────────
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(4, 62);
            this.lblColor.Name     = "lblColor";
            this.lblColor.Text     = "Color:";

            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            this.cmbColor.Location      = new System.Drawing.Point(46, 59);
            this.cmbColor.Name          = "cmbColor";
            this.cmbColor.Size          = new System.Drawing.Size(80, 23);
            this.cmbColor.TabIndex      = 5;

            this.chkAutoInvert.AutoSize = true;
            this.chkAutoInvert.Location = new System.Drawing.Point(134, 61);
            this.chkAutoInvert.Name     = "chkAutoInvert";
            this.chkAutoInvert.Text     = "AutoInvert";
            this.chkAutoInvert.TabIndex = 6;
            this.chkAutoInvert.Checked  = true;

            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApply.Location  = new System.Drawing.Point(218, 59);
            this.btnApply.Name      = "btnApply";
            this.btnApply.Size      = new System.Drawing.Size(68, 26);
            this.btnApply.TabIndex  = 7;
            this.btnApply.Text      = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click    += new System.EventHandler(this.btnApply_Click);

            // ── ContourSettingsPanel ──────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblRetr);
            this.Controls.Add(this.cmbRetr);
            this.Controls.Add(this.lblApprox);
            this.Controls.Add(this.cmbApprox);
            this.Controls.Add(this.lblMinArea);
            this.Controls.Add(this.nudMinArea);
            this.Controls.Add(this.lblMaxArea);
            this.Controls.Add(this.nudMaxArea);
            this.Controls.Add(this.lblThickness);
            this.Controls.Add(this.nudThickness);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.cmbColor);
            this.Controls.Add(this.chkAutoInvert);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "ContourSettingsPanel";
            this.Size = new System.Drawing.Size(290, 92);

            ((System.ComponentModel.ISupportInitialize)(this.nudMinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThickness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label          lblRetr;
        private System.Windows.Forms.ComboBox       cmbRetr;
        private System.Windows.Forms.Label          lblApprox;
        private System.Windows.Forms.ComboBox       cmbApprox;
        private System.Windows.Forms.Label          lblMinArea;
        private System.Windows.Forms.NumericUpDown  nudMinArea;
        private System.Windows.Forms.Label          lblMaxArea;
        private System.Windows.Forms.NumericUpDown  nudMaxArea;
        private System.Windows.Forms.Label          lblThickness;
        private System.Windows.Forms.NumericUpDown  nudThickness;
        private System.Windows.Forms.Label          lblColor;
        private System.Windows.Forms.ComboBox       cmbColor;
        private System.Windows.Forms.CheckBox       chkAutoInvert;
        private System.Windows.Forms.Button         btnApply;
    }
}
