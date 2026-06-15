namespace TestUtilApp.UI
{
    using TestUtilApp.CameraLight;

    partial class CameraLightControl
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
            this._splitOuter = new System.Windows.Forms.SplitContainer();
            this._splitInner = new System.Windows.Forms.SplitContainer();
            this._palette    = new NodePalettePanel();
            this._canvas     = new CameraLightCanvas();
            this._tabRight   = new System.Windows.Forms.TabControl();

            ((System.ComponentModel.ISupportInitialize)(this._splitOuter)).BeginInit();
            this._splitOuter.Panel1.SuspendLayout();
            this._splitOuter.Panel2.SuspendLayout();
            this._splitOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitInner)).BeginInit();
            this._splitInner.Panel1.SuspendLayout();
            this._splitInner.Panel2.SuspendLayout();
            this._splitInner.SuspendLayout();
            this.SuspendLayout();

            var c0   = System.Drawing.Color.FromArgb(12, 14, 18);
            var c1   = System.Drawing.Color.FromArgb(15, 28, 55);
            var cCyan= System.Drawing.Color.FromArgb(128, 255, 255);

            // ── _splitOuter (전체: 좌=팔레트+캔버스, 우=탭) ─────────
            this._splitOuter.Dock             = System.Windows.Forms.DockStyle.Fill;
            this._splitOuter.Orientation      = System.Windows.Forms.Orientation.Vertical;
            this._splitOuter.SplitterDistance = 600;
            this._splitOuter.BackColor        = c0;
            this._splitOuter.Panel1.Controls.Add(this._splitInner);
            this._splitOuter.Panel2.Controls.Add(this._tabRight);

            // ── _splitInner (좌측: 팔레트 | 캔버스) ─────────────────
            this._splitInner.Dock             = System.Windows.Forms.DockStyle.Fill;
            this._splitInner.Orientation      = System.Windows.Forms.Orientation.Vertical;
            this._splitInner.SplitterDistance = 170;
            this._splitInner.BackColor        = c0;
            this._splitInner.Panel1.Controls.Add(this._palette);
            this._splitInner.Panel2.Controls.Add(this._canvas);

            // ── 팔레트 ──────────────────────────────────────────────
            this._palette.Dock = System.Windows.Forms.DockStyle.Fill;

            // ── 캔버스 ──────────────────────────────────────────────
            this._canvas.Dock = System.Windows.Forms.DockStyle.Fill;

            // ── 우측 탭 ─────────────────────────────────────────────
            this._tabRight.Dock      = System.Windows.Forms.DockStyle.Fill;
            this._tabRight.BackColor = c0;
            this._tabRight.ForeColor = cCyan;
            this._tabRight.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // ── CameraLightControl ────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = c0;
            this.ForeColor           = cCyan;
            this.Name                = "CameraLightControl";
            this.Size                = new System.Drawing.Size(1200, 667);
            this.Controls.Add(this._splitOuter);

            this._splitOuter.Panel1.ResumeLayout(false);
            this._splitOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitOuter)).EndInit();
            this._splitOuter.ResumeLayout(false);
            this._splitInner.Panel1.ResumeLayout(false);
            this._splitInner.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitInner)).EndInit();
            this._splitInner.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitOuter;
        private System.Windows.Forms.SplitContainer _splitInner;
        private NodePalettePanel                     _palette;
        private CameraLightCanvas                    _canvas;
        private System.Windows.Forms.TabControl      _tabRight;
    }
}
