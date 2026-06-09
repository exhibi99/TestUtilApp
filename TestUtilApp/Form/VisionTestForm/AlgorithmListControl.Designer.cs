namespace TestUtilApp.UI
{
    partial class AlgorithmListControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposePreviewBitmaps();
                _currentSettingsPanel?.Dispose();
                _currentSettingsPanel = null;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components     = new System.ComponentModel.Container();
            // ── Header ───────────────────────────────────────────────
            this.pnlHeader      = new System.Windows.Forms.Panel();
            this.lblPipeline    = new System.Windows.Forms.Label();
            // ── List ─────────────────────────────────────────────────
            this.listView       = new System.Windows.Forms.ListView();
            this.colName        = new System.Windows.Forms.ColumnHeader();
            this.colSummary     = new System.Windows.Forms.ColumnHeader();
            this.contextMenu    = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuRemove     = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSep        = new System.Windows.Forms.ToolStripSeparator();
            this.menuClearAll   = new System.Windows.Forms.ToolStripMenuItem();
            // ── Settings panel ───────────────────────────────────────
            this.pnlSettings    = new System.Windows.Forms.Panel();
            this.lblNoSelection = new System.Windows.Forms.Label();
            this.pnlInputFrom   = new System.Windows.Forms.Panel();
            this.lblInputFrom   = new System.Windows.Forms.Label();
            this.cboInputFrom   = new System.Windows.Forms.ComboBox();
            // ── Preview ──────────────────────────────────────────────
            this.pnlPreview         = new System.Windows.Forms.Panel();
            this.lblPreviewTitle    = new System.Windows.Forms.Label();
            this.pictureBoxPreview  = new System.Windows.Forms.PictureBox();
            // ── Run / move / remove bar ──────────────────────────────
            this.pnlRun            = new System.Windows.Forms.Panel();
            this.btnMoveUp         = new System.Windows.Forms.Button();
            this.btnMoveDown       = new System.Windows.Forms.Button();
            this.btnRemove         = new System.Windows.Forms.Button();
            this.btnPipelineView   = new System.Windows.Forms.Button();
            this.btnRun            = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.pnlInputFrom.SuspendLayout();
            this.pnlPreview.SuspendLayout();
            this.pnlRun.SuspendLayout();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();

            // ════════════════════════════════════════════════════════
            // pnlHeader  (DockTop, 22px)
            // ════════════════════════════════════════════════════════
            this.pnlHeader.Controls.Add(this.lblPipeline);
            this.pnlHeader.Dock    = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Name    = "pnlHeader";
            this.pnlHeader.Size    = new System.Drawing.Size(280, 22);
            this.pnlHeader.TabIndex = 0;

            this.lblPipeline.AutoSize  = false;
            this.lblPipeline.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblPipeline.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPipeline.ForeColor = System.Drawing.Color.Gray;
            this.lblPipeline.Name      = "lblPipeline";
            this.lblPipeline.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblPipeline.Text      = "Pipeline";
            this.lblPipeline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ════════════════════════════════════════════════════════
            // listView  (DockFill)
            // ════════════════════════════════════════════════════════
            this.listView.BorderStyle    = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colName, this.colSummary });
            this.listView.ContextMenuStrip = this.contextMenu;
            this.listView.Dock             = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect    = true;
            this.listView.GridLines        = false;
            this.listView.HeaderStyle      = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView.HideSelection    = false;
            this.listView.MultiSelect      = false;
            this.listView.Name             = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.TabIndex         = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View             = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);

            this.colName.Text  = "Algorithm";
            this.colName.Width = 85;

            this.colSummary.Text  = "Params";
            this.colSummary.Width = 175;

            // contextMenu
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuRemove, this.menuSep, this.menuClearAll });
            this.contextMenu.Name = "contextMenu";

            this.menuRemove.Name  = "menuRemove";
            this.menuRemove.Text  = "Remove";
            this.menuRemove.Click += new System.EventHandler(this.menuRemove_Click);

            this.menuSep.Name = "menuSep";

            this.menuClearAll.Name  = "menuClearAll";
            this.menuClearAll.Text  = "Clear All";
            this.menuClearAll.Click += new System.EventHandler(this.menuClearAll_Click);

            // ════════════════════════════════════════════════════════
            // pnlSettings  (DockBottom, 110px)  – inline settings panel
            // ════════════════════════════════════════════════════════
            this.pnlSettings.Controls.Add(this.lblNoSelection);
            this.pnlSettings.Dock     = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSettings.Name     = "pnlSettings";
            this.pnlSettings.Padding  = new System.Windows.Forms.Padding(4);
            this.pnlSettings.Size     = new System.Drawing.Size(280, 110);
            this.pnlSettings.TabIndex = 3;

            this.lblNoSelection.AutoSize  = false;
            this.lblNoSelection.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.lblNoSelection.ForeColor = System.Drawing.Color.Gray;
            this.lblNoSelection.Name      = "lblNoSelection";
            this.lblNoSelection.Text      = "Select an algorithm to edit";
            this.lblNoSelection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── pnlInputFrom  (DockBottom, 30px) – 독립 패널, pnlSettings 위에 위치
            this.pnlInputFrom.Controls.Add(this.lblInputFrom);
            this.pnlInputFrom.Controls.Add(this.cboInputFrom);
            this.pnlInputFrom.Dock    = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInputFrom.Name    = "pnlInputFrom";
            this.pnlInputFrom.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.pnlInputFrom.Size    = new System.Drawing.Size(280, 30);
            this.pnlInputFrom.Visible = false;

            this.lblInputFrom.AutoSize  = true;
            this.lblInputFrom.Location  = new System.Drawing.Point(4, 7);
            this.lblInputFrom.Name      = "lblInputFrom";
            this.lblInputFrom.Text      = "Input from:";

            this.cboInputFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInputFrom.Location      = new System.Drawing.Point(78, 4);
            this.cboInputFrom.Name          = "cboInputFrom";
            this.cboInputFrom.Size          = new System.Drawing.Size(194, 22);
            this.cboInputFrom.TabIndex      = 0;
            this.cboInputFrom.SelectedIndexChanged += new System.EventHandler(this.cboInputFrom_SelectedIndexChanged);

            // ════════════════════════════════════════════════════════
            // pnlPreview  (DockBottom, 160px)  – intermediate result
            // ════════════════════════════════════════════════════════
            this.pnlPreview.Controls.Add(this.pictureBoxPreview);
            this.pnlPreview.Controls.Add(this.lblPreviewTitle);
            this.pnlPreview.Dock     = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPreview.Name     = "pnlPreview";
            this.pnlPreview.Size     = new System.Drawing.Size(280, 160);
            this.pnlPreview.TabIndex = 2;

            this.lblPreviewTitle.AutoSize  = false;
            this.lblPreviewTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblPreviewTitle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewTitle.Name      = "lblPreviewTitle";
            this.lblPreviewTitle.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblPreviewTitle.Size      = new System.Drawing.Size(280, 20);
            this.lblPreviewTitle.Text      = "Result Preview";
            this.lblPreviewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pictureBoxPreview.BackColor = System.Drawing.Color.FromArgb(8, 10, 14);
            this.pictureBoxPreview.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPreview.Name      = "pictureBoxPreview";
            this.pictureBoxPreview.SizeMode  = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex  = 0;
            this.pictureBoxPreview.TabStop   = false;

            // ════════════════════════════════════════════════════════
            // pnlRun  (DockBottom, 38px)  – ▲ ▼ ✕  +  ▶ Run
            // ════════════════════════════════════════════════════════
            this.pnlRun.Controls.Add(this.btnMoveUp);
            this.pnlRun.Controls.Add(this.btnMoveDown);
            this.pnlRun.Controls.Add(this.btnRemove);
            this.pnlRun.Controls.Add(this.btnPipelineView);
            this.pnlRun.Controls.Add(this.btnRun);
            this.pnlRun.Dock     = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRun.Name     = "pnlRun";
            this.pnlRun.Padding  = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlRun.Size     = new System.Drawing.Size(280, 38);
            this.pnlRun.TabIndex = 1;

            this.btnMoveUp.Enabled   = false;
            this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUp.Location  = new System.Drawing.Point(4, 5);
            this.btnMoveUp.Name      = "btnMoveUp";
            this.btnMoveUp.Size      = new System.Drawing.Size(28, 28);
            this.btnMoveUp.TabIndex  = 0;
            this.btnMoveUp.Text      = "\u25b2";
            this.btnMoveUp.UseVisualStyleBackColor = false;
            this.btnMoveUp.Click    += new System.EventHandler(this.btnMoveUp_Click);

            this.btnMoveDown.Enabled   = false;
            this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDown.Location  = new System.Drawing.Point(36, 5);
            this.btnMoveDown.Name      = "btnMoveDown";
            this.btnMoveDown.Size      = new System.Drawing.Size(28, 28);
            this.btnMoveDown.TabIndex  = 1;
            this.btnMoveDown.Text      = "\u25bc";
            this.btnMoveDown.UseVisualStyleBackColor = false;
            this.btnMoveDown.Click    += new System.EventHandler(this.btnMoveDown_Click);

            this.btnRemove.Enabled   = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location  = new System.Drawing.Point(68, 5);
            this.btnRemove.Name      = "btnRemove";
            this.btnRemove.Size      = new System.Drawing.Size(28, 28);
            this.btnRemove.TabIndex  = 2;
            this.btnRemove.Text      = "\u2715";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click    += new System.EventHandler(this.btnRemove_Click);

            this.btnPipelineView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPipelineView.Location  = new System.Drawing.Point(100, 5);
            this.btnPipelineView.Name      = "btnPipelineView";
            this.btnPipelineView.Size      = new System.Drawing.Size(72, 28);
            this.btnPipelineView.TabIndex  = 3;
            this.btnPipelineView.Text      = "Pipeline";
            this.btnPipelineView.UseVisualStyleBackColor = false;
            this.btnPipelineView.Click    += new System.EventHandler(this.btnPipelineView_Click);

            this.btnRun.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnRun.Enabled   = false;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRun.ForeColor = System.Drawing.Color.White;
            this.btnRun.Location  = new System.Drawing.Point(176, 5);
            this.btnRun.Name      = "btnRun";
            this.btnRun.Size      = new System.Drawing.Size(96, 28);
            this.btnRun.TabIndex  = 4;
            this.btnRun.Text      = "\u25b6  Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click    += new System.EventHandler(this.btnRun_Click);

            // ════════════════════════════════════════════════════════
            // AlgorithmListControl root
            // DockBottom 처리 순서 = Controls 역순 (높은 index → 먼저 처리 → 하단에 배치)
            // 시각적 순서 (아래→위): pnlRun, pnlPreview, pnlInputFrom, pnlSettings, listView
            // ════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView);       // Fill     → [0]
            this.Controls.Add(this.pnlSettings);    // Bottom   → [1]
            this.Controls.Add(this.pnlInputFrom);   // Bottom   → [2] (pnlSettings 바로 위)
            this.Controls.Add(this.pnlPreview);     // Bottom   → [3]
            this.Controls.Add(this.pnlRun);         // Bottom   → [4] (최하단)
            this.Controls.Add(this.pnlHeader);      // Top      → [5]
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "AlgorithmListControl";
            this.Size = new System.Drawing.Size(280, 667);

            this.pnlHeader.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlInputFrom.ResumeLayout(false);
            this.pnlInputFrom.PerformLayout();
            this.pnlPreview.ResumeLayout(false);
            this.pnlRun.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        // ── Header ────────────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlHeader;
        private System.Windows.Forms.Label  lblPipeline;

        // ── List ──────────────────────────────────────────────────
        private System.Windows.Forms.ListView              listView;
        private System.Windows.Forms.ColumnHeader          colName;
        private System.Windows.Forms.ColumnHeader          colSummary;
        private System.Windows.Forms.ContextMenuStrip      contextMenu;
        private System.Windows.Forms.ToolStripMenuItem     menuRemove;
        private System.Windows.Forms.ToolStripSeparator    menuSep;
        private System.Windows.Forms.ToolStripMenuItem     menuClearAll;

        // ── Settings ──────────────────────────────────────────────
        private System.Windows.Forms.Panel    pnlSettings;
        private System.Windows.Forms.Label    lblNoSelection;
        private System.Windows.Forms.Panel    pnlInputFrom;
        private System.Windows.Forms.Label    lblInputFrom;
        private System.Windows.Forms.ComboBox cboInputFrom;

        // ── Preview ───────────────────────────────────────────────
        private System.Windows.Forms.Panel      pnlPreview;
        private System.Windows.Forms.Label      lblPreviewTitle;
        private System.Windows.Forms.PictureBox pictureBoxPreview;

        // ── Run bar ───────────────────────────────────────────────
        private System.Windows.Forms.Panel  pnlRun;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnPipelineView;
        private System.Windows.Forms.Button btnRun;
    }
}
