namespace TestUtilApp.UI
{
    partial class OpenCvControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeMats();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            // ── Panels ──────────────────────────────────────────
            this.pnlButtons        = new System.Windows.Forms.Panel();
            this.pnlImage          = new System.Windows.Forms.Panel();
            this.pnlScrollable     = new System.Windows.Forms.Panel();
            // ── Left button panel ────────────────────────────────
            this.btnAcquire        = new System.Windows.Forms.Button();
            this.lblSectionProcess = new System.Windows.Forms.Label();
            this.btnThreshold      = new System.Windows.Forms.Button();
            // ── Algorithm list ───────────────────────────────────
            this.algorithmListControl = new TestUtilApp.UI.AlgorithmListControl();
            // ── Image viewer ─────────────────────────────────────
            this.toolStripImage = new System.Windows.Forms.ToolStrip();
            this.tsbFit         = new System.Windows.Forms.ToolStripButton();
            this.tsbZoomIn      = new System.Windows.Forms.ToolStripButton();
            this.tsbZoomOut     = new System.Windows.Forms.ToolStripButton();
            this.tsSep1         = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSave        = new System.Windows.Forms.ToolStripButton();
            this.tsSep2         = new System.Windows.Forms.ToolStripSeparator();
            this.tslInfo        = new System.Windows.Forms.ToolStripLabel();
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();

            this.pnlButtons.SuspendLayout();
            this.pnlImage.SuspendLayout();
            this.pnlScrollable.SuspendLayout();
            this.toolStripImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            this.SuspendLayout();

            // ════════════════════════════════════════════════════
            // pnlButtons  (DockLeft, 150px) – Acquire + algo buttons
            // ════════════════════════════════════════════════════
            this.pnlButtons.Controls.Add(this.btnAcquire);
            this.pnlButtons.Controls.Add(this.lblSectionProcess);
            this.pnlButtons.Controls.Add(this.btnThreshold);
            this.pnlButtons.Dock    = System.Windows.Forms.DockStyle.Left;
            this.pnlButtons.Name    = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(6);
            this.pnlButtons.Size    = new System.Drawing.Size(150, 667);
            this.pnlButtons.TabIndex = 0;

            // btnAcquire
            this.btnAcquire.Dock      = System.Windows.Forms.DockStyle.Top;
            this.btnAcquire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcquire.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAcquire.Location  = new System.Drawing.Point(6, 6);
            this.btnAcquire.Name      = "btnAcquire";
            this.btnAcquire.Size      = new System.Drawing.Size(138, 36);
            this.btnAcquire.TabIndex  = 0;
            this.btnAcquire.Text      = "Acquire";
            this.btnAcquire.UseVisualStyleBackColor = false;
            this.btnAcquire.Click += new System.EventHandler(this.btnAcquire_Click);

            // lblSectionProcess
            this.lblSectionProcess.AutoSize  = false;
            this.lblSectionProcess.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblSectionProcess.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblSectionProcess.ForeColor = System.Drawing.Color.Gray;
            this.lblSectionProcess.Name      = "lblSectionProcess";
            this.lblSectionProcess.Size      = new System.Drawing.Size(138, 20);
            this.lblSectionProcess.Text      = "Process";
            this.lblSectionProcess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSectionProcess.Padding   = new System.Windows.Forms.Padding(2, 0, 0, 0);

            // btnThreshold
            this.btnThreshold.Dock      = System.Windows.Forms.DockStyle.Top;
            this.btnThreshold.Enabled   = false;
            this.btnThreshold.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThreshold.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnThreshold.Name      = "btnThreshold";
            this.btnThreshold.Size      = new System.Drawing.Size(138, 36);
            this.btnThreshold.TabIndex  = 1;
            this.btnThreshold.Text      = "Threshold";
            this.btnThreshold.UseVisualStyleBackColor = false;
            this.btnThreshold.Click += new System.EventHandler(this.btnThreshold_Click);

            // ════════════════════════════════════════════════════
            // algorithmListControl  (DockRight, 280px)
            // ════════════════════════════════════════════════════
            this.algorithmListControl.Dock     = System.Windows.Forms.DockStyle.Right;
            this.algorithmListControl.Name     = "algorithmListControl";
            this.algorithmListControl.Size     = new System.Drawing.Size(280, 667);
            this.algorithmListControl.TabIndex = 1;

            // ════════════════════════════════════════════════════
            // pnlImage  (DockFill)
            // ════════════════════════════════════════════════════
            this.pnlImage.Controls.Add(this.pnlScrollable);
            this.pnlImage.Controls.Add(this.toolStripImage);
            this.pnlImage.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.pnlImage.Name     = "pnlImage";
            this.pnlImage.TabIndex = 2;

            // toolStripImage  (DockTop)
            this.toolStripImage.Dock      = System.Windows.Forms.DockStyle.Top;
            this.toolStripImage.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbFit, this.tsbZoomIn, this.tsbZoomOut,
                this.tsSep1, this.tsbSave, this.tsSep2, this.tslInfo });
            this.toolStripImage.Name     = "toolStripImage";
            this.toolStripImage.Size     = new System.Drawing.Size(830, 27);
            this.toolStripImage.TabIndex = 0;

            this.tsbFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbFit.Name         = "tsbFit";
            this.tsbFit.Text         = "Fit";
            this.tsbFit.ToolTipText  = "Fit to Window";
            this.tsbFit.Click       += new System.EventHandler(this.tsbFit_Click);

            this.tsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbZoomIn.Name         = "tsbZoomIn";
            this.tsbZoomIn.Text         = " + ";
            this.tsbZoomIn.ToolTipText  = "Zoom In";
            this.tsbZoomIn.Click       += new System.EventHandler(this.tsbZoomIn_Click);

            this.tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbZoomOut.Name         = "tsbZoomOut";
            this.tsbZoomOut.Text         = " - ";
            this.tsbZoomOut.ToolTipText  = "Zoom Out";
            this.tsbZoomOut.Click       += new System.EventHandler(this.tsbZoomOut_Click);

            this.tsSep1.Name = "tsSep1";

            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSave.Enabled      = false;
            this.tsbSave.Name         = "tsbSave";
            this.tsbSave.Text         = "Save";
            this.tsbSave.ToolTipText  = "Save Image";
            this.tsbSave.Click       += new System.EventHandler(this.tsbSave_Click);

            this.tsSep2.Name = "tsSep2";

            this.tslInfo.Name = "tslInfo";
            this.tslInfo.Text = "No image loaded";

            // pnlScrollable  (DockFill, AutoScroll)
            this.pnlScrollable.AutoScroll = true;
            this.pnlScrollable.Controls.Add(this.pictureBoxMain);
            this.pnlScrollable.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.pnlScrollable.Name     = "pnlScrollable";
            this.pnlScrollable.TabIndex = 1;

            // pictureBoxMain
            this.pictureBoxMain.BackColor = System.Drawing.Color.FromArgb(8, 10, 14);
            this.pictureBoxMain.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxMain.Name      = "pictureBoxMain";
            this.pictureBoxMain.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMain.TabIndex  = 0;
            this.pictureBoxMain.TabStop   = false;

            // ════════════════════════════════════════════════════
            // OpenCvControl root
            // Controls.Add: Fill first, then Left, then Right last
            //   Processing (back→front): algorithmListControl(Right),
            //   pnlButtons(Left), pnlImage(Fill)
            // ════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlImage);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.algorithmListControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "OpenCvControl";
            this.Size = new System.Drawing.Size(1200, 667);

            this.pnlButtons.ResumeLayout(false);
            this.pnlImage.ResumeLayout(false);
            this.pnlImage.PerformLayout();
            this.toolStripImage.ResumeLayout(false);
            this.toolStripImage.PerformLayout();
            this.pnlScrollable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        // ── Left buttons ─────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlButtons;
        private System.Windows.Forms.Button btnAcquire;
        private System.Windows.Forms.Label  lblSectionProcess;
        private System.Windows.Forms.Button btnThreshold;

        // ── Algorithm list ───────────────────────────────────────
        private TestUtilApp.UI.AlgorithmListControl algorithmListControl;

        // ── Image viewer ─────────────────────────────────────────
        private System.Windows.Forms.Panel              pnlImage;
        private System.Windows.Forms.ToolStrip          toolStripImage;
        private System.Windows.Forms.ToolStripButton    tsbFit;
        private System.Windows.Forms.ToolStripButton    tsbZoomIn;
        private System.Windows.Forms.ToolStripButton    tsbZoomOut;
        private System.Windows.Forms.ToolStripSeparator tsSep1;
        private System.Windows.Forms.ToolStripButton    tsbSave;
        private System.Windows.Forms.ToolStripSeparator tsSep2;
        private System.Windows.Forms.ToolStripLabel     tslInfo;
        private System.Windows.Forms.Panel              pnlScrollable;
        private System.Windows.Forms.PictureBox         pictureBoxMain;
    }
}
