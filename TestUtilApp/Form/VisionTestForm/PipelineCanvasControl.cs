using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    /// <summary>
    /// Free-form node-graph canvas for the pipeline.
    ///
    /// Interaction model:
    ///  • Drag node body  → reposition node
    ///  • Click a port    → "pending" (highlighted + rubberband)
    ///    Second click on another port → add connection if none, remove if exists
    ///    Click same port or empty area → cancel
    /// </summary>
    internal sealed class PipelineCanvasControl : Panel
    {
        // ── Layout constants ─────────────────────────────────────
        private const int NodeWidth  = 160;
        private const int NodeHeight = 54;
        private const int PortR      = 7;       // port circle radius
        private const int PortHit    = 12;      // hit-test radius (larger for ease)
        private const int DefaultColX = 60;
        private const int DefaultRowH  = 100;   // vertical spacing for auto-layout

        // ── Colors ───────────────────────────────────────────────
        private static readonly Color CBg           = Color.FromArgb(20, 20, 24);
        private static readonly Color CNode         = Color.FromArgb(42, 42, 48);
        private static readonly Color CNodeBdr      = Color.FromArgb(65, 65, 72);
        private static readonly Color CNodeSel      = Color.FromArgb(14, 84, 158);
        private static readonly Color CNodeSelBdr   = Color.FromArgb(42, 150, 255);
        private static readonly Color CNodeOff      = Color.FromArgb(28, 28, 32);      // disabled bg
        private static readonly Color CNodeOffBdr   = Color.FromArgb(50, 50, 56);      // disabled border
        private static readonly Color CText         = Color.FromArgb(210, 212, 216);
        private static readonly Color CTextOff      = Color.FromArgb(80, 82, 90);      // disabled text
        private static readonly Color CSub          = Color.FromArgb(115, 118, 128);
        private static readonly Color CSubOff       = Color.FromArgb(60, 62, 70);
        private static readonly Color COutPort      = Color.FromArgb(238, 148, 28);
        private static readonly Color CInPort       = Color.FromArgb(46, 176, 74);
        private static readonly Color CPortPend     = Color.FromArgb(240, 240, 60);
        private static readonly Color CPortOff      = Color.FromArgb(70, 70, 76);      // disabled port
        private static readonly Color CSeqArrow     = Color.FromArgb(70, 160, 70);
        private static readonly Color CSkipArrow    = Color.FromArgb(228, 126, 36);
        private static readonly Color CBypassArrow  = Color.FromArgb(80, 80, 90);      // bypass (disabled path)
        private static readonly Color CDragLine     = Color.FromArgb(220, 218, 50);

        // ── State ────────────────────────────────────────────────
        private readonly List<IOpenCvAlgorithm>               _algorithms;
        private readonly Dictionary<IOpenCvAlgorithm, Point>  _pos =
            new Dictionary<IOpenCvAlgorithm, Point>();
        private Point? _pendingDropPos;   // canvas position for the next newly-added algorithm

        // node-drag
        private bool              _draggingNode;
        private IOpenCvAlgorithm  _dragAlgo;
        private Point             _dragOffset;

        // port-connect
        private IOpenCvAlgorithm  _pendingAlgo;
        private bool              _pendingIsOutput;

        // rubberband when port is pending
        private Point _mousePos;

        private int _selectedIndex = -1;

        // ── Events ───────────────────────────────────────────────
        public event EventHandler        SelectionChanged;
        public event EventHandler        ConnectionsChanged;   // notify parent to re-run
        public event Action<int, int>    MoveRequested;        // (idx, direction -1/+1)
        public event EventHandler        RemoveRequested;
        public event EventHandler        ClearAllRequested;
        public event Action<string, Point> AlgorithmDropped;  // (typeName, canvasPos)

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value) return;
                _selectedIndex = value;
                Invalidate();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IOpenCvAlgorithm SelectedAlgorithm =>
            _selectedIndex >= 0 && _selectedIndex < _algorithms.Count
                ? _algorithms[_selectedIndex] : null;

        // ── Construction ─────────────────────────────────────────

        public PipelineCanvasControl(List<IOpenCvAlgorithm> algorithms)
        {
            _algorithms    = algorithms;
            DoubleBuffered = true;
            BackColor      = CBg;
            AutoScroll     = true;
            AllowDrop      = true;
        }

        /// <summary>
        /// Tell the canvas where to place the next newly-added algorithm.
        /// Call this before the algorithm is added to the list.
        /// </summary>
        public void SetNextDropPosition(Point canvasPos) => _pendingDropPos = canvasPos;

        /// <summary>
        /// Call after the algorithm list changes.
        /// New algorithms receive auto-layout positions; removed ones are forgotten.
        /// </summary>
        public void RefreshView(int select = -2)
        {
            SyncPositions();
            if (select != -2) _selectedIndex = select;
            UpdateAutoScroll();
            Invalidate();
        }

        private void SyncPositions()
        {
            // Remove stale entries
            var toRemove = new List<IOpenCvAlgorithm>();
            foreach (var key in _pos.Keys)
                if (!_algorithms.Contains(key)) toRemove.Add(key);
            foreach (var k in toRemove) _pos.Remove(k);

            // Add new entries — use pending drop position if available, else auto-layout
            foreach (var algo in _algorithms)
            {
                if (_pos.ContainsKey(algo)) continue;

                if (_pendingDropPos.HasValue)
                {
                    _pos[algo]      = _pendingDropPos.Value;
                    _pendingDropPos = null;   // consume
                }
                else
                {
                    int row = _pos.Count;
                    _pos[algo] = new Point(DefaultColX, 20 + row * DefaultRowH);
                }
            }
        }

        private void UpdateAutoScroll()
        {
            int maxX = 200, maxY = 200;
            foreach (var p in _pos.Values)
            {
                maxX = Math.Max(maxX, p.X + NodeWidth + 60);
                maxY = Math.Max(maxY, p.Y + NodeHeight + 60);
            }
            AutoScrollMinSize = new Size(maxX, maxY);
        }

        // ── Geometry helpers ─────────────────────────────────────

        private Rectangle NodeRect(IOpenCvAlgorithm algo)
        {
            var p = _pos.ContainsKey(algo) ? _pos[algo] : Point.Empty;
            return new Rectangle(p.X, p.Y, NodeWidth, NodeHeight);
        }

        /// <summary>Output port = bottom-center of node.</summary>
        private Point OutPort(IOpenCvAlgorithm algo)
        {
            var r = NodeRect(algo);
            return new Point(r.X + r.Width / 2, r.Bottom);
        }

        /// <summary>Input port = top-center of node.</summary>
        private Point InPort(IOpenCvAlgorithm algo)
        {
            var r = NodeRect(algo);
            return new Point(r.X + r.Width / 2, r.Top);
        }

        private bool HitPort(Point pt, Point port) =>
            Math.Abs(pt.X - port.X) <= PortHit && Math.Abs(pt.Y - port.Y) <= PortHit;

        /// <summary>Returns (algo, isOutput) if pt hits any port, else null.</summary>
        private (IOpenCvAlgorithm algo, bool isOutput)? HitAnyPort(Point pt)
        {
            foreach (var algo in _algorithms)
            {
                if (HitPort(pt, OutPort(algo))) return (algo, true);
                if (!algo.IsSourceNode && HitPort(pt, InPort(algo))) return (algo, false);
            }
            return null;
        }

        private IOpenCvAlgorithm HitNode(Point pt)
        {
            // Iterate in reverse so topmost-drawn (last index) is hit first
            for (int i = _algorithms.Count - 1; i >= 0; i--)
            {
                if (NodeRect(_algorithms[i]).Contains(pt))
                    return _algorithms[i];
            }
            return null;
        }

        private Point ToCanvas(Point screen) =>
            new Point(screen.X - AutoScrollPosition.X, screen.Y - AutoScrollPosition.Y);

        // ── Connection helpers ────────────────────────────────────

        private int IndexOf(IOpenCvAlgorithm algo) => _algorithms.IndexOf(algo);

        /// <summary>
        /// Returns true if there is an explicit or implicit connection from srcAlgo to dstAlgo.
        /// </summary>
        private bool ConnectionExists(IOpenCvAlgorithm srcAlgo, IOpenCvAlgorithm dstAlgo)
        {
            int si = IndexOf(srcAlgo);
            int di = IndexOf(dstAlgo);
            if (si < 0 || di <= 0 || di <= si) return false;

            int from = _algorithms[di].InputFromStep;
            if (from == si) return true;                          // explicit
            if (from < 0 && si == di - 1) return true;           // implicit sequential
            return false;
        }

        /// <summary>Toggle connection between srcAlgo (output) and dstAlgo (input).</summary>
        private void ToggleConnection(IOpenCvAlgorithm srcAlgo, IOpenCvAlgorithm dstAlgo)
        {
            int si = IndexOf(srcAlgo);
            int di = IndexOf(dstAlgo);
            if (si < 0 || di <= 0 || si == di) return;

            // Ensure dst > src so data flows forward
            if (di < si)
            {
                // Swap: user may have clicked input first then output
                var tmp = srcAlgo; srcAlgo = dstAlgo; dstAlgo = tmp;
                int ti = si; si = di; di = ti;
            }

            if (ConnectionExists(srcAlgo, dstAlgo))
            {
                // Remove: reset to implicit sequential (previous step)
                _algorithms[di].InputFromStep = -1;
            }
            else
            {
                // Add: only explicit if not already sequential neighbour
                _algorithms[di].InputFromStep = (si == di - 1) ? -1 : si;
            }

            ConnectionsChanged?.Invoke(this, EventArgs.Empty);
        }

        // ── Paint ─────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_algorithms.Count == 0)
            {
                DrawEmptyHint(g);
                return;
            }

            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

            DrawConnections(g);
            DrawNodes(g);
            DrawPorts(g);
            DrawRubberband(g);
        }

        private void DrawEmptyHint(Graphics g)
        {
            using (var br = new SolidBrush(Color.FromArgb(70, 70, 82)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString("알고리즘을 추가하면\n파이프라인이 여기에 표시됩니다.",
                    new Font("Segoe UI", 10f), br, new RectangleF(0, 0, Width, Height), sf);
        }

        private void DrawRubberband(Graphics g)
        {
            if (_pendingAlgo == null) return;
            var srcPt = _pendingIsOutput ? OutPort(_pendingAlgo) : InPort(_pendingAlgo);
            var dstPt = ToCanvas(_mousePos);
            using (var pen = new Pen(CDragLine, 2f) { DashStyle = DashStyle.Dash })
                g.DrawLine(pen, srcPt, dstPt);
        }

        private void DrawConnections(Graphics g)
        {
            // First pass: draw all disabled-node bypass arrows (faint, dashed)
            for (int di = 1; di < _algorithms.Count; di++)
            {
                if (!_algorithms[di].IsEnabled) continue;

                // Find the actual active source (skipping disabled nodes)
                int si = _algorithms[di].InputFromStep;
                if (si < 0 || si >= di) si = di - 1;
                while (si > 0 && !_algorithms[si].IsEnabled) si--;

                // Nominal (declared) source
                int nominalSi = _algorithms[di].InputFromStep;
                if (nominalSi < 0 || nominalSi >= di) nominalSi = di - 1;

                // If actual != nominal, some nodes in between are disabled → draw bypass
                if (si != nominalSi)
                {
                    DrawBezier(g, OutPort(_algorithms[si]), InPort(_algorithms[di]),
                        CBypassArrow, 1.5f, dashed: true);
                }
            }

            // Second pass: draw normal (enabled) arrows
            for (int di = 1; di < _algorithms.Count; di++)
            {
                int nominalSi = _algorithms[di].InputFromStep;
                if (nominalSi < 0 || nominalSi >= di) nominalSi = di - 1;

                var srcAlgo = _algorithms[nominalSi];
                var dstAlgo = _algorithms[di];
                bool skip   = nominalSi != di - 1;
                bool active = srcAlgo.IsEnabled && dstAlgo.IsEnabled;

                Color col   = active
                    ? (skip ? CSkipArrow : CSeqArrow)
                    : CBypassArrow;
                float width = active ? (skip ? 2f : 1.5f) : 1f;

                DrawBezier(g, OutPort(srcAlgo), InPort(dstAlgo), col, width,
                    dashed: !active);
            }
        }

        private static void DrawBezier(Graphics g, Point src, Point dst, Color col,
                                        float width, bool dashed = false)
        {
            float dy  = dst.Y - src.Y;
            float dx  = dst.X - src.X;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            float t   = Math.Min(len * 0.4f, 80f);

            var cp1 = new PointF(src.X, src.Y + t);
            var cp2 = new PointF(dst.X, dst.Y - t);

            using (var pen = new Pen(col, width))
            {
                if (dashed) pen.DashStyle = DashStyle.Dash;
                g.DrawBezier(pen,
                    new PointF(src.X, src.Y), cp1, cp2,
                    new PointF(dst.X, dst.Y));
            }

            DrawArrow(g, dst, new PointF(dst.X - cp2.X, dst.Y - cp2.Y), col);
        }

        private static void DrawArrow(Graphics g, Point tip, PointF tang, Color col)
        {
            float len = (float)Math.Sqrt(tang.X * tang.X + tang.Y * tang.Y);
            if (len < 0.1f) return;
            float ux = tang.X / len, uy = tang.Y / len;
            const float L = 9f, H = 4.5f;
            var p1 = new PointF(tip.X - L * ux - H * uy, tip.Y - L * uy + H * ux);
            var p2 = new PointF(tip.X - L * ux + H * uy, tip.Y - L * uy - H * ux);
            using (var br = new SolidBrush(col))
                g.FillPolygon(br, new[] { new PointF(tip.X, tip.Y), p1, p2 });
        }

        private void DrawNodes(Graphics g)
        {
            using (var nameFont    = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var nameFontOff = new Font("Segoe UI", 9f, FontStyle.Bold | FontStyle.Strikeout))
            using (var subFont     = new Font("Segoe UI", 7.5f))
            using (var numFont     = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var offFont     = new Font("Segoe UI", 7f, FontStyle.Bold))
            using (var fmt         = new StringFormat { Trimming = StringTrimming.EllipsisCharacter })
            {
                for (int i = 0; i < _algorithms.Count; i++)
                {
                    var  algo    = _algorithms[i];
                    var  rc      = NodeRect(algo);
                    bool sel     = (i == _selectedIndex);
                    bool enabled = algo.IsEnabled;

                    // Background & border
                    Color bg  = sel    ? CNodeSel
                              : enabled ? CNode : CNodeOff;
                    Color bdr = sel    ? CNodeSelBdr
                              : enabled ? CNodeBdr : CNodeOffBdr;

                    using (var path = RoundedPath(rc, 7))
                    using (var br   = new SolidBrush(bg))
                    using (var pen  = new Pen(bdr, sel ? 2f : 1f)
                                { DashStyle = enabled ? DashStyle.Solid : DashStyle.Dash })
                    { g.FillPath(br, path); g.DrawPath(pen, path); }

                    // Step badge — always bright when selected
                    Color badgeCol = sel    ? Color.White
                                   : enabled ? Color.FromArgb(140, 140, 158)
                                   :           Color.FromArgb(70, 72, 82);
                    using (var br = new SolidBrush(badgeCol))
                        g.DrawString($"{i + 1}", numFont, br, rc.X + 6, rc.Y + 5);

                    // Name — selected node always gets bright text
                    Color nameCol = sel    ? Color.White
                                  : enabled ? CText : CTextOff;
                    var nameRc = new RectangleF(rc.X + 22, rc.Y + 4, rc.Width - 26, rc.Height / 2 - 2);
                    using (var br = new SolidBrush(nameCol))
                        g.DrawString(algo.Name, enabled ? nameFont : nameFontOff, br, nameRc, fmt);

                    // Summary — selected node gets a readable muted color
                    Color subCol = sel    ? Color.FromArgb(170, 190, 220)
                                 : enabled ? CSub : CSubOff;
                    var subRc = new RectangleF(rc.X + 22, rc.Y + rc.Height / 2 + 2, rc.Width - 26, rc.Height / 2 - 6);
                    using (var br = new SolidBrush(subCol))
                        g.DrawString(algo.Summary, subFont, br, subRc, fmt);

                    // "OFF" badge for disabled nodes
                    if (!enabled)
                    {
                        var offRc = new RectangleF(rc.Right - 28, rc.Y + 4, 24, 14);
                        using (var br  = new SolidBrush(Color.FromArgb(80, 80, 90)))
                        using (var pen = new Pen(Color.FromArgb(100, 100, 110), 1f))
                        using (var fmtC = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            g.FillRectangle(br, offRc);
                            g.DrawRectangle(pen, offRc.X, offRc.Y, offRc.Width, offRc.Height);
                            using (var brT = new SolidBrush(Color.FromArgb(160, 160, 170)))
                                g.DrawString("OFF", offFont, brT, offRc, fmtC);
                        }
                    }
                }
            }
        }

        private void DrawPorts(Graphics g)
        {
            foreach (var algo in _algorithms)
            {
                bool enabled = algo.IsEnabled;
                bool outPend = (_pendingAlgo == algo && _pendingIsOutput);
                bool inPend  = (_pendingAlgo == algo && !_pendingIsOutput);

                Color outFill = outPend ? CPortPend : (enabled ? COutPort : CPortOff);
                Color inFill  = inPend  ? CPortPend : (enabled ? CInPort  : CPortOff);

                DrawCircle(g, OutPort(algo), PortR, outFill, Color.FromArgb(165, 90, 0));

                if (!algo.IsSourceNode)
                    DrawCircle(g, InPort(algo), PortR, inFill, Color.FromArgb(24, 116, 44));
            }
        }

        private static void DrawCircle(Graphics g, Point c, int r, Color fill, Color border)
        {
            var rc = new Rectangle(c.X - r, c.Y - r, r * 2, r * 2);
            using (var br  = new SolidBrush(fill)) g.FillEllipse(br, rc);
            using (var pen = new Pen(border, 1.5f)) g.DrawEllipse(pen, rc);
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

        // ── Mouse ─────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var pt = ToCanvas(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                // 1. Port hit?
                var portHit = HitAnyPort(pt);
                if (portHit.HasValue)
                {
                    HandlePortClick(portHit.Value.algo, portHit.Value.isOutput);
                    return;
                }

                // 2. Cancel pending connection if clicking empty space
                if (_pendingAlgo != null)
                {
                    CancelPending();
                    return;
                }

                // 3. Node hit → start dragging or select
                var nodeHit = HitNode(pt);
                if (nodeHit != null)
                {
                    SelectedIndex = IndexOf(nodeHit);

                    var nodePos   = _pos[nodeHit];
                    _draggingNode = true;
                    _dragAlgo     = nodeHit;
                    _dragOffset   = new Point(pt.X - nodePos.X, pt.Y - nodePos.Y);
                    Capture       = true;
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                CancelPending();
                var nodeHit = HitNode(pt);
                if (nodeHit != null)
                {
                    SelectedIndex = IndexOf(nodeHit);
                    ShowContextMenu(nodeHit, e.Location);
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button != MouseButtons.Left) return;
            var pt      = ToCanvas(e.Location);
            var nodeHit = HitNode(pt);
            if (nodeHit == null || nodeHit.IsSourceNode) return;

            nodeHit.IsEnabled = !nodeHit.IsEnabled;
            Invalidate();
            ConnectionsChanged?.Invoke(this, EventArgs.Empty);   // trigger re-run
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mousePos = e.Location;

            if (_draggingNode)
            {
                var pt  = ToCanvas(e.Location);
                _pos[_dragAlgo] = new Point(
                    Math.Max(0, pt.X - _dragOffset.X),
                    Math.Max(0, pt.Y - _dragOffset.Y));
                UpdateAutoScroll();
                Invalidate();
                return;
            }

            if (_pendingAlgo != null)
            {
                Invalidate();
                return;
            }

            // Change cursor near ports
            var cpt = ToCanvas(e.Location);
            Cursor = HitAnyPort(cpt).HasValue ? Cursors.Cross : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_draggingNode)
            {
                Capture       = false;
                _draggingNode = false;
                _dragAlgo     = null;
                UpdateAutoScroll();
                Invalidate();
            }
        }

        private void HandlePortClick(IOpenCvAlgorithm algo, bool isOutput)
        {
            if (_pendingAlgo == null)
            {
                // First click: mark as pending
                _pendingAlgo     = algo;
                _pendingIsOutput = isOutput;
                Invalidate();
                return;
            }

            // Second click on same port: cancel
            if (_pendingAlgo == algo && _pendingIsOutput == isOutput)
            {
                CancelPending();
                return;
            }

            // Determine src (output) and dst (input) regardless of click order
            IOpenCvAlgorithm srcAlgo, dstAlgo;
            if (_pendingIsOutput && !isOutput)
            {
                srcAlgo = _pendingAlgo; dstAlgo = algo;
            }
            else if (!_pendingIsOutput && isOutput)
            {
                srcAlgo = algo; dstAlgo = _pendingAlgo;
            }
            else
            {
                // Both same type — cancel
                CancelPending();
                return;
            }

            ToggleConnection(srcAlgo, dstAlgo);
            CancelPending();
        }

        private void CancelPending()
        {
            _pendingAlgo = null;
            Cursor       = Cursors.Default;
            Invalidate();
        }

        // ── DragDrop (receive from palette) ──────────────────────

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            drgevent.Effect = drgevent.Data.GetDataPresent(DataFormats.StringFormat)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            drgevent.Effect = drgevent.Data.GetDataPresent(DataFormats.StringFormat)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            string typeName = drgevent.Data.GetData(DataFormats.StringFormat) as string;
            if (string.IsNullOrEmpty(typeName)) return;

            var screenPt  = new Point(drgevent.X, drgevent.Y);
            var clientPt  = PointToClient(screenPt);
            var canvasPt  = ToCanvas(clientPt);
            // Center the new node on the drop point
            canvasPt = new Point(canvasPt.X - NodeWidth / 2, canvasPt.Y - NodeHeight / 2);
            canvasPt = new Point(Math.Max(0, canvasPt.X), Math.Max(0, canvasPt.Y));

            AlgorithmDropped?.Invoke(typeName, canvasPt);
        }

        // ── Context menu ──────────────────────────────────────────

        private void ShowContextMenu(IOpenCvAlgorithm algo, Point loc)
        {
            int idx  = IndexOf(algo);
            var menu = new ContextMenuStrip();

            if (idx > 0)
                menu.Items.Add("▲ 위로",   null, (_, __) => MoveRequested?.Invoke(idx, -1));
            if (idx < _algorithms.Count - 1)
                menu.Items.Add("▼ 아래로", null, (_, __) => MoveRequested?.Invoke(idx, +1));

            // Enable / Disable toggle (source node cannot be disabled)
            if (!algo.IsSourceNode)
            {
                menu.Items.Add(new ToolStripSeparator());
                string toggleLabel = algo.IsEnabled ? "⏸  비활성화 (Bypass)" : "▶  활성화";
                menu.Items.Add(toggleLabel, null, (_, __) =>
                {
                    algo.IsEnabled = !algo.IsEnabled;
                    Invalidate();
                    ConnectionsChanged?.Invoke(this, EventArgs.Empty);
                });
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("제거",        null, (_, __) => RemoveRequested?.Invoke(this, EventArgs.Empty));
            menu.Items.Add("전체 초기화", null, (_, __) => ClearAllRequested?.Invoke(this, EventArgs.Empty));
            menu.Show(this, loc);
        }
    }
}
