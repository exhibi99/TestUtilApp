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
            this._palette = new TestUtilApp.CameraLight.NodePalettePanel();
            this._canvas = new TestUtilApp.CameraLight.CameraLightCanvas();
            this._previewPanel = new System.Windows.Forms.Panel();
            this._tabRight = new System.Windows.Forms.TabControl();
            this._previewToolbar = new System.Windows.Forms.Panel();
            this._btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._splitOuter)).BeginInit();
            this._splitOuter.Panel1.SuspendLayout();
            this._splitOuter.Panel2.SuspendLayout();
            this._splitOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitInner)).BeginInit();
            this._splitInner.Panel1.SuspendLayout();
            this._splitInner.Panel2.SuspendLayout();
            this._splitInner.SuspendLayout();
            this._previewPanel.SuspendLayout();
            this._previewToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitOuter
            // 
            this._splitOuter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this._splitOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitOuter.Location = new System.Drawing.Point(0, 0);
            this._splitOuter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._splitOuter.Name = "_splitOuter";
            // 
            // _splitOuter.Panel1
            // 
            this._splitOuter.Panel1.Controls.Add(this._splitInner);
            // 
            // _splitOuter.Panel2
            // 
            this._splitOuter.Panel2.Controls.Add(this._previewPanel);
            this._splitOuter.Size = new System.Drawing.Size(1200, 667);
            this._splitOuter.SplitterDistance = 600;
            this._splitOuter.TabIndex = 0;
            // 
            // _splitInner
            // 
            this._splitInner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this._splitInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitInner.Location = new System.Drawing.Point(0, 0);
            this._splitInner.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._splitInner.Name = "_splitInner";
            // 
            // _splitInner.Panel1
            // 
            this._splitInner.Panel1.Controls.Add(this._palette);
            // 
            // _splitInner.Panel2
            // 
            this._splitInner.Panel2.Controls.Add(this._canvas);
            this._splitInner.Size = new System.Drawing.Size(600, 667);
            this._splitInner.SplitterDistance = 170;
            this._splitInner.TabIndex = 0;
            // 
            // _palette
            // 
            this._palette.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(18)))), ((int)(((byte)(28)))));
            this._palette.Dock = System.Windows.Forms.DockStyle.Fill;
            this._palette.Location = new System.Drawing.Point(0, 0);
            this._palette.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._palette.Name = "_palette";
            this._palette.Size = new System.Drawing.Size(682, 400);
            this._palette.TabIndex = 0;
            // 
            // _canvas
            // 
            this._canvas.AllowDrop = true;
            this._canvas.AutoScroll = true;
            this._canvas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(20)))), ((int)(((byte)(26)))));
            this._canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this._canvas.Location = new System.Drawing.Point(0, 0);
            this._canvas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(160, 400);
            this._canvas.TabIndex = 0;
            // 
            // _previewPanel
            // 
            this._previewPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this._previewPanel.Controls.Add(this._tabRight);
            this._previewPanel.Controls.Add(this._previewToolbar);
            this._previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewPanel.Location = new System.Drawing.Point(0, 0);
            this._previewPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._previewPanel.Name = "_previewPanel";
            this._previewPanel.Size = new System.Drawing.Size(200, 400);
            this._previewPanel.TabIndex = 0;
            // 
            // _tabRight
            // 
            this._tabRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabRight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._tabRight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._tabRight.Location = new System.Drawing.Point(0, 26);
            this._tabRight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._tabRight.Name = "_tabRight";
            this._tabRight.SelectedIndex = 0;
            this._tabRight.Size = new System.Drawing.Size(200, 374);
            this._tabRight.TabIndex = 0;
            // 
            // _previewToolbar
            // 
            this._previewToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(34)))), ((int)(((byte)(58)))));
            this._previewToolbar.Controls.Add(this._btnSave);
            this._previewToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this._previewToolbar.Location = new System.Drawing.Point(0, 0);
            this._previewToolbar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._previewToolbar.Name = "_previewToolbar";
            this._previewToolbar.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this._previewToolbar.Size = new System.Drawing.Size(200, 26);
            this._previewToolbar.TabIndex = 1;
            // 
            // _btnSave
            // 
            this._btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(80)))));
            this._btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this._btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))));
            this._btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSave.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this._btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._btnSave.Location = new System.Drawing.Point(140, 2);
            this._btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(56, 22);
            this._btnSave.TabIndex = 0;
            this._btnSave.Text = "저장";
            this._btnSave.UseVisualStyleBackColor = false;
            this._btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // CameraLightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(18)))));
            this.Controls.Add(this._splitOuter);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CameraLightControl";
            this.Size = new System.Drawing.Size(1200, 667);
            this._splitOuter.Panel1.ResumeLayout(false);
            this._splitOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitOuter)).EndInit();
            this._splitOuter.ResumeLayout(false);
            this._splitInner.Panel1.ResumeLayout(false);
            this._splitInner.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitInner)).EndInit();
            this._splitInner.ResumeLayout(false);
            this._previewPanel.ResumeLayout(false);
            this._previewToolbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitOuter;
        private System.Windows.Forms.SplitContainer _splitInner;
        private NodePalettePanel                     _palette;
        private CameraLightCanvas                    _canvas;
        private System.Windows.Forms.Panel           _previewPanel;
        private System.Windows.Forms.Panel           _previewToolbar;
        private System.Windows.Forms.Button          _btnSave;
        private System.Windows.Forms.TabControl      _tabRight;
    }
}
