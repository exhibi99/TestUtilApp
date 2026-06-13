namespace TestUtilApp.UI
{
    partial class CameraCalcControl
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
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.grpFov = new System.Windows.Forms.GroupBox();
            this.lblFovWD = new System.Windows.Forms.Label();
            this.txtFovWD = new System.Windows.Forms.TextBox();
            this.lblFovWDUnit = new System.Windows.Forms.Label();
            this.lblFovFL = new System.Windows.Forms.Label();
            this.txtFovFL = new System.Windows.Forms.TextBox();
            this.lblFovFLUnit = new System.Windows.Forms.Label();
            this.lblFovSensorX = new System.Windows.Forms.Label();
            this.txtFovSensorX = new System.Windows.Forms.TextBox();
            this.lblFovSensorXUnit = new System.Windows.Forms.Label();
            this.lblFovSensorY = new System.Windows.Forms.Label();
            this.txtFovSensorY = new System.Windows.Forms.TextBox();
            this.lblFovSensorYUnit = new System.Windows.Forms.Label();
            this.btnCalcFov = new System.Windows.Forms.Button();
            this.lblFovXTitle = new System.Windows.Forms.Label();
            this.lblFovXResult = new System.Windows.Forms.Label();
            this.lblFovYTitle = new System.Windows.Forms.Label();
            this.lblFovYResult = new System.Windows.Forms.Label();
            this.pnlDiagrams = new System.Windows.Forms.Panel();
            this.lblSideTitle = new System.Windows.Forms.Label();
            this.pnlSideView = new System.Windows.Forms.Panel();
            this.lblFrontTitle = new System.Windows.Forms.Label();
            this.pnlFrontView = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpFov.SuspendLayout();
            this.pnlDiagrams.SuspendLayout();
            this.SuspendLayout();
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel1.Controls.Add(this.grpFov);
            this.splitMain.Panel2.Controls.Add(this.pnlDiagrams);
            this.splitMain.Size = new System.Drawing.Size(1180, 700);
            this.splitMain.SplitterDistance = 380;
            this.splitMain.TabIndex = 0;
            //
            // grpFov
            //
            this.grpFov.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFov.Controls.Add(this.lblFovWD);
            this.grpFov.Controls.Add(this.txtFovWD);
            this.grpFov.Controls.Add(this.lblFovWDUnit);
            this.grpFov.Controls.Add(this.lblFovFL);
            this.grpFov.Controls.Add(this.txtFovFL);
            this.grpFov.Controls.Add(this.lblFovFLUnit);
            this.grpFov.Controls.Add(this.lblFovSensorX);
            this.grpFov.Controls.Add(this.txtFovSensorX);
            this.grpFov.Controls.Add(this.lblFovSensorXUnit);
            this.grpFov.Controls.Add(this.lblFovSensorY);
            this.grpFov.Controls.Add(this.txtFovSensorY);
            this.grpFov.Controls.Add(this.lblFovSensorYUnit);
            this.grpFov.Controls.Add(this.btnCalcFov);
            this.grpFov.Controls.Add(this.lblFovXTitle);
            this.grpFov.Controls.Add(this.lblFovXResult);
            this.grpFov.Controls.Add(this.lblFovYTitle);
            this.grpFov.Controls.Add(this.lblFovYResult);
            this.grpFov.Location = new System.Drawing.Point(8, 8);
            this.grpFov.Name = "grpFov";
            this.grpFov.Size = new System.Drawing.Size(360, 290);
            this.grpFov.TabIndex = 0;
            this.grpFov.TabStop = false;
            this.grpFov.Text = "FOV Calculation  ( WD → FOV )";
            //
            // lblFovWD
            //
            this.lblFovWD.AutoSize = true;
            this.lblFovWD.Location = new System.Drawing.Point(12, 38);
            this.lblFovWD.Text = "WD :";
            //
            // txtFovWD
            //
            this.txtFovWD.Location = new System.Drawing.Point(145, 34);
            this.txtFovWD.Name = "txtFovWD";
            this.txtFovWD.Size = new System.Drawing.Size(110, 27);
            this.txtFovWD.Text = "700";
            this.txtFovWD.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblFovWDUnit
            //
            this.lblFovWDUnit.AutoSize = true;
            this.lblFovWDUnit.Location = new System.Drawing.Point(260, 38);
            this.lblFovWDUnit.Text = "mm";
            //
            // lblFovFL
            //
            this.lblFovFL.AutoSize = true;
            this.lblFovFL.Location = new System.Drawing.Point(12, 78);
            this.lblFovFL.Text = "Focal Length :";
            //
            // txtFovFL
            //
            this.txtFovFL.Location = new System.Drawing.Point(145, 74);
            this.txtFovFL.Name = "txtFovFL";
            this.txtFovFL.Size = new System.Drawing.Size(110, 27);
            this.txtFovFL.Text = "8";
            this.txtFovFL.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblFovFLUnit
            //
            this.lblFovFLUnit.AutoSize = true;
            this.lblFovFLUnit.Location = new System.Drawing.Point(260, 78);
            this.lblFovFLUnit.Text = "mm";
            //
            // lblFovSensorX
            //
            this.lblFovSensorX.AutoSize = true;
            this.lblFovSensorX.Location = new System.Drawing.Point(12, 118);
            this.lblFovSensorX.Text = "Sensor Size X :";
            //
            // txtFovSensorX
            //
            this.txtFovSensorX.Location = new System.Drawing.Point(145, 114);
            this.txtFovSensorX.Name = "txtFovSensorX";
            this.txtFovSensorX.Size = new System.Drawing.Size(110, 27);
            this.txtFovSensorX.Text = "13.13";
            this.txtFovSensorX.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblFovSensorXUnit
            //
            this.lblFovSensorXUnit.AutoSize = true;
            this.lblFovSensorXUnit.Location = new System.Drawing.Point(260, 118);
            this.lblFovSensorXUnit.Text = "mm";
            //
            // lblFovSensorY
            //
            this.lblFovSensorY.AutoSize = true;
            this.lblFovSensorY.Location = new System.Drawing.Point(12, 158);
            this.lblFovSensorY.Text = "Sensor Size Y :";
            //
            // txtFovSensorY
            //
            this.txtFovSensorY.Location = new System.Drawing.Point(145, 154);
            this.txtFovSensorY.Name = "txtFovSensorY";
            this.txtFovSensorY.Size = new System.Drawing.Size(110, 27);
            this.txtFovSensorY.Text = "8.76";
            this.txtFovSensorY.TextChanged += new System.EventHandler(this.InputChanged);
            //
            // lblFovSensorYUnit
            //
            this.lblFovSensorYUnit.AutoSize = true;
            this.lblFovSensorYUnit.Location = new System.Drawing.Point(260, 158);
            this.lblFovSensorYUnit.Text = "mm";
            //
            // btnCalcFov
            //
            this.btnCalcFov.Location = new System.Drawing.Point(145, 194);
            this.btnCalcFov.Name = "btnCalcFov";
            this.btnCalcFov.Size = new System.Drawing.Size(110, 30);
            this.btnCalcFov.TabIndex = 10;
            this.btnCalcFov.Text = "Calculate";
            this.btnCalcFov.UseVisualStyleBackColor = false;
            this.btnCalcFov.Click += new System.EventHandler(this.btnCalcFov_Click);
            //
            // lblFovXTitle
            //
            this.lblFovXTitle.AutoSize = true;
            this.lblFovXTitle.Location = new System.Drawing.Point(12, 240);
            this.lblFovXTitle.Text = "FOV X :";
            //
            // lblFovXResult
            //
            this.lblFovXResult.AutoSize = true;
            this.lblFovXResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFovXResult.Location = new System.Drawing.Point(145, 238);
            this.lblFovXResult.Name = "lblFovXResult";
            this.lblFovXResult.Text = "-";
            //
            // lblFovYTitle
            //
            this.lblFovYTitle.AutoSize = true;
            this.lblFovYTitle.Location = new System.Drawing.Point(12, 266);
            this.lblFovYTitle.Text = "FOV Y :";
            //
            // lblFovYResult
            //
            this.lblFovYResult.AutoSize = true;
            this.lblFovYResult.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFovYResult.Location = new System.Drawing.Point(145, 264);
            this.lblFovYResult.Name = "lblFovYResult";
            this.lblFovYResult.Text = "-";
            //
            // pnlDiagrams
            //
            this.pnlDiagrams.Controls.Add(this.lblSideTitle);
            this.pnlDiagrams.Controls.Add(this.pnlSideView);
            this.pnlDiagrams.Controls.Add(this.lblFrontTitle);
            this.pnlDiagrams.Controls.Add(this.pnlFrontView);
            this.pnlDiagrams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDiagrams.Location = new System.Drawing.Point(0, 0);
            this.pnlDiagrams.Name = "pnlDiagrams";
            this.pnlDiagrams.Size = new System.Drawing.Size(796, 700);
            this.pnlDiagrams.TabIndex = 0;
            //
            // lblSideTitle
            //
            this.lblSideTitle.AutoSize = false;
            this.lblSideTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSideTitle.Location = new System.Drawing.Point(0, 0);
            this.lblSideTitle.Name = "lblSideTitle";
            this.lblSideTitle.Size = new System.Drawing.Size(796, 22);
            this.lblSideTitle.Text = "  Side View  ( Camera ← WD → Subject )";
            this.lblSideTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlSideView
            //
            this.pnlSideView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSideView.Location = new System.Drawing.Point(0, 22);
            this.pnlSideView.Name = "pnlSideView";
            this.pnlSideView.Size = new System.Drawing.Size(796, 326);
            this.pnlSideView.TabIndex = 0;
            this.pnlSideView.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlSideView_Paint);
            this.pnlSideView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseDown);
            this.pnlSideView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseMove);
            this.pnlSideView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlSideView_MouseUp);
            //
            // lblFrontTitle
            //
            this.lblFrontTitle.AutoSize = false;
            this.lblFrontTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFrontTitle.Location = new System.Drawing.Point(0, 352);
            this.lblFrontTitle.Name = "lblFrontTitle";
            this.lblFrontTitle.Size = new System.Drawing.Size(796, 22);
            this.lblFrontTitle.Text = "  Front View  ( FOV at Subject Plane )";
            this.lblFrontTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlFrontView
            //
            this.pnlFrontView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFrontView.Location = new System.Drawing.Point(0, 374);
            this.pnlFrontView.Name = "pnlFrontView";
            this.pnlFrontView.Size = new System.Drawing.Size(796, 320);
            this.pnlFrontView.TabIndex = 1;
            this.pnlFrontView.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFrontView_Paint);
            //
            // CameraCalcControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(12, 14, 18);
            this.Controls.Add(this.splitMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Name = "CameraCalcControl";
            this.Size = new System.Drawing.Size(1180, 700);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpFov.ResumeLayout(false);
            this.grpFov.PerformLayout();
            this.pnlDiagrams.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpFov;
        private System.Windows.Forms.Label lblFovWD;
        private System.Windows.Forms.TextBox txtFovWD;
        private System.Windows.Forms.Label lblFovWDUnit;
        private System.Windows.Forms.Label lblFovFL;
        private System.Windows.Forms.TextBox txtFovFL;
        private System.Windows.Forms.Label lblFovFLUnit;
        private System.Windows.Forms.Label lblFovSensorX;
        private System.Windows.Forms.TextBox txtFovSensorX;
        private System.Windows.Forms.Label lblFovSensorXUnit;
        private System.Windows.Forms.Label lblFovSensorY;
        private System.Windows.Forms.TextBox txtFovSensorY;
        private System.Windows.Forms.Label lblFovSensorYUnit;
        private System.Windows.Forms.Button btnCalcFov;
        private System.Windows.Forms.Label lblFovXTitle;
        private System.Windows.Forms.Label lblFovXResult;
        private System.Windows.Forms.Label lblFovYTitle;
        private System.Windows.Forms.Label lblFovYResult;
        private System.Windows.Forms.Panel pnlDiagrams;
        private System.Windows.Forms.Label lblSideTitle;
        private System.Windows.Forms.Panel pnlSideView;
        private System.Windows.Forms.Label lblFrontTitle;
        private System.Windows.Forms.Panel pnlFrontView;
    }
}
