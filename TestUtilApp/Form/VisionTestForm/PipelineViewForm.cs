using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    /// <summary>
    /// Modeless popup that shows the pipeline as a free-form node graph.
    /// Left panel: algorithm palette (drag cards onto canvas).
    /// Right panel: selected node settings.
    /// </summary>
    internal sealed class PipelineViewForm : Form
    {
        // ── Algorithm type registry ───────────────────────────────
        private static readonly (string TypeName, string Label, string Sub)[] AlgorithmTypes =
        {
            ("Acquire",   "Open Image", "이미지 열기"),
            ("Threshold", "Threshold",  "이진화"),
            ("Crop",      "Crop",       "자르기"),
            ("Contour",   "Contour",    "윤곽선 검출"),
        };

        private static IOpenCvAlgorithm CreateAlgorithm(string typeName)
        {
            switch (typeName)
            {
                case "Threshold": return new ThresholdAlgorithm();
                case "Crop":      return new CropAlgorithm();
                case "Contour":   return new ContourAlgorithm();
                default:          return null;
            }
        }

        // ── Fields ───────────────────────────────────────────────
        private readonly List<IOpenCvAlgorithm> _algorithms;
        private readonly PipelineCanvasControl  _canvas;
        private readonly Panel                  _pnlSettings;
        private readonly Label                  _lblSettingsTitle;
        private readonly Label                  _lblNoSel;
        private AlgorithmSettingsPanel          _currentPanel;

        // ── Events forwarded to AlgorithmListControl ──────────────
        public event Action<int, int>          MoveRequested;        // (idx, direction -1/+1)
        public event EventHandler              RemoveRequested;
        public event EventHandler              ClearAllRequested;
        public event EventHandler              SelectionChanged;
        public event EventHandler              ConnectionsChanged;
        public event EventHandler              SettingsApplied;
        public event Action<IOpenCvAlgorithm>  AlgorithmAddRequested; // drag-drop new node

        public int SelectedIndex => _canvas.SelectedIndex;

        // ── Construction ─────────────────────────────────────────

        public PipelineViewForm(List<IOpenCvAlgorithm> algorithms)
        {
            _algorithms = algorithms;

            Text            = "Pipeline View";
            Size            = new Size(820, 580);
            MinimumSize     = new Size(650, 360);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            StartPosition   = FormStartPosition.Manual;
            ShowInTaskbar   = false;
            BackColor       = Color.FromArgb(28, 28, 32);
            Font            = new Font("Segoe UI", 9f);

            // ── Bottom hint bar ───────────────────────────────────
            var pnlBottom = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 36,
                Padding   = new Padding(8, 0, 8, 0),
                BackColor = Color.FromArgb(22, 22, 26)
            };
            var lblHint = new Label
            {
                Text      = "카드 드래그 → 캔버스  |  포트(●) 클릭×2 → 연결/해제  |  노드 드래그 → 이동  |  더블클릭 → 비활성화 토글",
                Dock      = DockStyle.Fill,
                ForeColor = Color.FromArgb(100, 104, 116),
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 8f)
            };
            var btnClose = new Button
            {
                Text      = "닫기",
                Dock      = DockStyle.Right,
                Width     = 64,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(190, 190, 200),
                Font      = new Font("Segoe UI", 9f)
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(75, 75, 82);
            btnClose.Click += (_, __) => Close();
            pnlBottom.Controls.Add(lblHint);
            pnlBottom.Controls.Add(btnClose);

            // ── Right settings panel ──────────────────────────────
            var pnlRight = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 260,
                BackColor = Color.FromArgb(30, 30, 36)
            };
            var pnlRightHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 28,
                Padding   = new Padding(8, 0, 0, 0),
                BackColor = Color.FromArgb(24, 24, 30)
            };
            _lblSettingsTitle = new Label
            {
                Dock      = DockStyle.Fill,
                Text      = "Settings",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(205, 207, 220),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlRightHeader.Controls.Add(_lblSettingsTitle);

            var sepRight = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(50, 50, 58) };
            _lblNoSel = new Label
            {
                Dock      = DockStyle.Fill,
                Text      = "노드를 선택하면\n설정이 표시됩니다.",
                ForeColor = Color.FromArgb(130, 132, 148),
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 9f)
            };
            _pnlSettings = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = new Padding(4),
                BackColor = Color.FromArgb(30, 30, 36),
                ForeColor = Color.FromArgb(205, 207, 220)
            };
            _pnlSettings.Controls.Add(_lblNoSel);

            pnlRight.Controls.Add(_pnlSettings);
            pnlRight.Controls.Add(sepRight);
            pnlRight.Controls.Add(pnlRightHeader);

            // ── Left palette panel ────────────────────────────────
            var pnlPalette = BuildPalettePanel();

            // Separators
            var vSepRight = new Panel { Dock = DockStyle.Right, Width = 1, BackColor = Color.FromArgb(50, 50, 58) };
            var vSepLeft  = new Panel { Dock = DockStyle.Left,  Width = 1, BackColor = Color.FromArgb(50, 50, 58) };

            // ── Canvas ────────────────────────────────────────────
            _canvas = new PipelineCanvasControl(_algorithms) { Dock = DockStyle.Fill };
            _canvas.SelectionChanged   += OnCanvasSelectionChanged;
            _canvas.ConnectionsChanged += (s, e) => ConnectionsChanged?.Invoke(this, e);
            _canvas.MoveRequested      += (idx, dir) => MoveRequested?.Invoke(idx, dir);
            _canvas.RemoveRequested    += (s, e) => RemoveRequested?.Invoke(this, e);
            _canvas.ClearAllRequested  += (s, e) => ClearAllRequested?.Invoke(this, e);
            _canvas.AlgorithmDropped   += OnAlgorithmDropped;

            // Add order: Fill last, Bottom/Left/Right before Fill
            Controls.Add(_canvas);
            Controls.Add(vSepRight);
            Controls.Add(pnlRight);
            Controls.Add(vSepLeft);
            Controls.Add(pnlPalette);
            Controls.Add(pnlBottom);

            _canvas.RefreshView();
        }

        // ── Palette builder ───────────────────────────────────────

        private Panel BuildPalettePanel()
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 128,
                BackColor = Color.FromArgb(22, 22, 28),
                Padding   = new Padding(8, 8, 8, 8)
            };

            // Header
            var hdr = new Label
            {
                Text      = "Algorithms",
                Dock      = DockStyle.Top,
                Height    = 22,
                ForeColor = Color.FromArgb(130, 132, 150),
                Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(2, 0, 0, 0)
            };
            var hdrSep = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(48, 50, 58) };

            pnl.Controls.Add(BuildCardContainer());
            pnl.Controls.Add(hdrSep);
            pnl.Controls.Add(hdr);
            return pnl;
        }

        private Panel BuildCardContainer()
        {
            var container = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding   = new Padding(0, 6, 0, 0)
            };

            int y = 6;
            foreach (var (typeName, label, sub) in AlgorithmTypes)
            {
                var card = CreatePaletteCard(typeName, label, sub);
                card.Location = new Point(4, y);
                container.Controls.Add(card);
                y += card.Height + 8;
            }

            return container;
        }

        private Panel CreatePaletteCard(string typeName, string label, string sub)
        {
            const int W = 108, H = 46;
            var card = new Panel
            {
                Size      = new Size(W, H),
                BackColor = Color.Transparent,
                Cursor    = Cursors.Hand,
                Tag       = typeName
            };

            card.Paint += (s, pe) => DrawCard(pe.Graphics, (Panel)s, label, sub);

            // Hover highlight
            card.MouseEnter += (s, _) => { ((Panel)s).Tag = "hover:" + typeName; card.Invalidate(); };
            card.MouseLeave += (s, _) => { ((Panel)s).Tag = typeName;            card.Invalidate(); };

            // Drag detection
            Point dragStart = Point.Empty;
            bool  dragging  = false;

            card.MouseDown += (s, me) =>
            {
                if (me.Button == MouseButtons.Left) { dragStart = me.Location; dragging = false; }
            };
            card.MouseMove += (s, me) =>
            {
                if (me.Button != MouseButtons.Left || dragging) return;
                if (Math.Abs(me.X - dragStart.X) > SystemInformation.DragSize.Width ||
                    Math.Abs(me.Y - dragStart.Y) > SystemInformation.DragSize.Height)
                {
                    dragging = true;
                    card.DoDragDrop(typeName, DragDropEffects.Copy);
                }
            };
            card.MouseUp += (s, _) => dragging = false;

            return card;
        }

        private static void DrawCard(Graphics g, Panel card, string label, string sub)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            bool hover = card.Tag?.ToString().StartsWith("hover:") == true;

            var rc     = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
            var bgCol  = hover ? Color.FromArgb(55, 56, 64) : Color.FromArgb(40, 42, 50);
            var bdrCol = hover ? Color.FromArgb(90, 92, 110) : Color.FromArgb(62, 64, 74);

            using (var path = RoundedPath(rc, 6))
            using (var br   = new SolidBrush(bgCol))
            using (var pen  = new Pen(bdrCol, 1f))
            {
                g.FillPath(br, path);
                g.DrawPath(pen, path);
            }

            // Grip dots (2×3 grid)
            using (var br = new SolidBrush(Color.FromArgb(90, 92, 108)))
                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 2; col++)
                        g.FillEllipse(br, 7 + col * 5, 11 + row * 7, 3, 3);

            // Label (bold top line)
            TextRenderer.DrawText(g, label,
                new Font("Segoe UI", 9f, FontStyle.Bold),
                new Rectangle(20, 4, card.Width - 24, 20),
                Color.FromArgb(205, 207, 220),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // Sub (small bottom line)
            TextRenderer.DrawText(g, sub,
                new Font("Segoe UI", 7.5f),
                new Rectangle(20, 24, card.Width - 24, 16),
                Color.FromArgb(115, 118, 130),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private static GraphicsPath RoundedPath(Rectangle rc, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(rc.X,           rc.Y,           r*2, r*2, 180, 90);
            p.AddArc(rc.Right - r*2, rc.Y,           r*2, r*2, 270, 90);
            p.AddArc(rc.Right - r*2, rc.Bottom - r*2, r*2, r*2,   0, 90);
            p.AddArc(rc.X,           rc.Bottom - r*2, r*2, r*2,  90, 90);
            p.CloseFigure();
            return p;
        }

        // ── Drag-drop from palette ────────────────────────────────

        private void OnAlgorithmDropped(string typeName, Point canvasPos)
        {
            if (typeName == "Acquire")
            {
                HandleAcquireDrop(canvasPos);
                return;
            }

            var algo = CreateAlgorithm(typeName);
            if (algo == null) return;

            _canvas.SetNextDropPosition(canvasPos);
            AlgorithmAddRequested?.Invoke(algo);
        }

        private void HandleAcquireDrop(Point canvasPos)
        {
            using (var dlg = new AcquireDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var algo = new AcquireAlgorithm();
                algo.LoadSource(dlg.ImagePath, dlg.LoadAsGray);

                if (!algo.HasSource) return;

                _canvas.SetNextDropPosition(canvasPos);
                AlgorithmAddRequested?.Invoke(algo);
            }
        }

        // ── Public API ────────────────────────────────────────────

        public void RefreshView(int select = -2)
        {
            _canvas.RefreshView(select);
            RefreshSettingsPanel();
        }

        public void SetSelection(int idx)
        {
            _canvas.SelectedIndex = idx;
        }

        // ── Settings panel management ─────────────────────────────

        private void OnCanvasSelectionChanged(object sender, EventArgs e)
        {
            RefreshSettingsPanel();
            SelectionChanged?.Invoke(this, e);
        }

        private void RefreshSettingsPanel()
        {
            if (_currentPanel != null)
            {
                _currentPanel.Applied -= OnSettingsPanelApplied;
                _pnlSettings.Controls.Remove(_currentPanel);
                _currentPanel.Dispose();
                _currentPanel = null;
            }

            var algo = _canvas.SelectedAlgorithm;
            if (algo == null)
            {
                _lblSettingsTitle.Text = "Settings";
                _lblNoSel.Visible      = true;
                return;
            }

            _lblSettingsTitle.Text = $"{algo.Name}  설정";
            _lblNoSel.Visible      = false;

            var panel = algo.GetSettingsPanel();
            if (panel == null) { _lblNoSel.Visible = true; return; }

            panel.LoadFrom(algo);
            panel.Applied  += OnSettingsPanelApplied;
            panel.Dock      = DockStyle.Fill;
            panel.BackColor = Color.FromArgb(30, 30, 36);
            panel.ForeColor = Color.FromArgb(205, 207, 220);
            _pnlSettings.Controls.Add(panel);
            _currentPanel = panel;
        }

        private void OnSettingsPanelApplied(object sender, EventArgs e)
        {
            var algo = _canvas.SelectedAlgorithm;
            if (algo == null || _currentPanel == null) return;

            _currentPanel.ApplyTo(algo);
            _canvas.Invalidate();
            SettingsApplied?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _canvas.RefreshView();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _currentPanel?.Dispose();
        }
    }
}
