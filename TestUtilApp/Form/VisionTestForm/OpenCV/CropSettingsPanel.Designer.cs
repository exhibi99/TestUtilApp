namespace TestUtilApp.UI
{
    partial class CropSettingsPanel
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
            this.lblX       = new System.Windows.Forms.Label();
            this.nudX       = new System.Windows.Forms.NumericUpDown();
            this.lblY       = new System.Windows.Forms.Label();
            this.nudY       = new System.Windows.Forms.NumericUpDown();
            this.lblWidth   = new System.Windows.Forms.Label();
            this.nudWidth   = new System.Windows.Forms.NumericUpDown();
            this.lblHeight  = new System.Windows.Forms.Label();
            this.nudHeight  = new System.Windows.Forms.NumericUpDown();
            this.btnApply   = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            this.SuspendLayout();

            // lblX
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(4, 6);
            this.lblX.Name     = "lblX";
            this.lblX.Text     = "X:";

            // nudX
            this.nudX.Location = new System.Drawing.Point(28, 3);
            this.nudX.Maximum  = new decimal(new int[] { 99999, 0, 0, 0 });
            this.nudX.Name     = "nudX";
            this.nudX.Size     = new System.Drawing.Size(72, 23);
            this.nudX.TabIndex = 0;

            // lblY
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(108, 6);
            this.lblY.Name     = "lblY";
            this.lblY.Text     = "Y:";

            // nudY
            this.nudY.Location = new System.Drawing.Point(124, 3);
            this.nudY.Maximum  = new decimal(new int[] { 99999, 0, 0, 0 });
            this.nudY.Name     = "nudY";
            this.nudY.Size     = new System.Drawing.Size(72, 23);
            this.nudY.TabIndex = 1;

            // lblWidth
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(4, 35);
            this.lblWidth.Name     = "lblWidth";
            this.lblWidth.Text     = "W:";

            // nudWidth
            this.nudWidth.Location = new System.Drawing.Point(28, 32);
            this.nudWidth.Minimum  = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudWidth.Maximum  = new decimal(new int[] { 99999, 0, 0, 0 });
            this.nudWidth.Value    = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudWidth.Name     = "nudWidth";
            this.nudWidth.Size     = new System.Drawing.Size(72, 23);
            this.nudWidth.TabIndex = 2;

            // lblHeight
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(108, 35);
            this.lblHeight.Name     = "lblHeight";
            this.lblHeight.Text     = "H:";

            // nudHeight
            this.nudHeight.Location = new System.Drawing.Point(124, 32);
            this.nudHeight.Minimum  = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudHeight.Maximum  = new decimal(new int[] { 99999, 0, 0, 0 });
            this.nudHeight.Value    = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudHeight.Name     = "nudHeight";
            this.nudHeight.Size     = new System.Drawing.Size(72, 23);
            this.nudHeight.TabIndex = 3;

            // btnApply
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApply.Location  = new System.Drawing.Point(124, 61);
            this.btnApply.Name      = "btnApply";
            this.btnApply.Size      = new System.Drawing.Size(72, 26);
            this.btnApply.TabIndex  = 4;
            this.btnApply.Text      = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click    += new System.EventHandler(this.btnApply_Click);

            // CropSettingsPanel
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblX);
            this.Controls.Add(this.nudX);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.nudY);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "CropSettingsPanel";
            this.Size = new System.Drawing.Size(260, 95);

            ((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label          lblX;
        private System.Windows.Forms.NumericUpDown  nudX;
        private System.Windows.Forms.Label          lblY;
        private System.Windows.Forms.NumericUpDown  nudY;
        private System.Windows.Forms.Label          lblWidth;
        private System.Windows.Forms.NumericUpDown  nudWidth;
        private System.Windows.Forms.Label          lblHeight;
        private System.Windows.Forms.NumericUpDown  nudHeight;
        private System.Windows.Forms.Button         btnApply;
    }
}
