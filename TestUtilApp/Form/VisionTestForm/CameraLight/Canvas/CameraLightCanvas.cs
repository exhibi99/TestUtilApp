using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestUtilApp.CameraLight
{
    /// <summary>
    /// 좌우 흐름 노드 그래프 캔버스.
    ///  • 좌측 팔레트에서 드래그 → 캔버스에 노드 생성
    ///  • 포트 클릭 후 다른 포트 클릭 → 연결 / 연결 해제
    ///  • 노드 바디 드래그 → 이동
    ///  • 노드 더블클릭 → NodeDoubleClicked 이벤트
    ///  • 우클릭 → 컨텍스트 메뉴 (삭제 등)
    /// </summary>
    internal sealed class CameraLightCanvas : Panel
    {
        // ── 레이아웃 ────────────────────────────────────────────────
        private const int NodeW   = 160;
        private const int NodeH   = 60;
        private const int PortR   = 7;
        private const int PortHit = 12;

        // ── 색상 ────────────────────────────────────────────────────
        private static readonly Color CBg          = Color.FromArgb(18, 20, 26);
        private static readonly Color CGrid        = Color.FromArgb(30, 34, 44);
        private static readonly Color CText        = Color.FromArgb(210, 215, 220);
        private static readonly Color CSub         = Color.FromArgb(110, 118, 130);
        private static readonly Color CDragLine    = Color.FromArgb(220, 218, 50);
        private static readonly Color CConnLine    = Color.FromArgb(80, 180, 100);

        // 노드 종류별 색상
        private static Color NodeBg(NodeKind k)
        {
            switch (k)
            {
                case NodeKind.Camera:          return Color.FromArgb(20, 55, 95);
                case NodeKind.COMPort:         return Color.FromArgb(80, 52, 12);
                case NodeKind.LightController: return Color.FromArgb(65, 55, 10);
                case NodeKind.ImageArea:       return Color.FromArgb(12, 60, 35);
                default:                       return Color.FromArgb(40, 40, 48);
            }
        }
        private static Color NodeBdr(NodeKind k)
        {
            switch (k)
            {
                case NodeKind.Camera:          return Color.FromArgb(42, 148, 255);
                case NodeKind.COMPort:         return Color.FromArgb(230, 140, 30);
                case NodeKind.LightController: return Color.FromArgb(230, 200, 40);
                case NodeKind.ImageArea:       return Color.FromArgb(46, 180, 80);
                default:                       return Color.FromArgb(80, 80, 90);
            }
        }
        private static readonly Color COutPort  = Color.FromArgb(238, 148, 28);
        private static readonly Color CInPort   = Color.FromArgb(46, 176, 74);
        private static readonly Color CPortPend = Color.FromArgb(240, 240, 60);

        // ── 상태 ────────────────────────────────────────────────────
        private readonly List<CameraLightNode>   _nodes       = new List<CameraLightNode>();
        private readonly List<NodeConnection>    _connections = new List<NodeConnection>();

        // 노드 드래그
        private bool            _dragging;
        private CameraLightNode _dragNode;
        private Point           _dragOffset;

        // 포트 연결 pending
        private CameraLightNode _pendingNode;
        private bool            _pendingIsOutput;
        private Point           _mousePos;

        // ── 이벤트 ──────────────────────────────────────────────────
        public event Action<CameraLightNode>               NodeDoubleClicked;
        public event Action<NodeConnection>                ConnectionAdded;
        public event Action<NodeConnection>                ConnectionRemoved;
        public event Action<CameraLightNode>               NodeRemoved;
        /// <summary>우클릭 메뉴에서 카메라 액션 선택 시 발생. action = "GrabOnce" | "StartLive" | "StopLive"</summary>
        public event Action<CameraLightNode, string>       NodeActionRequested;

        // ── 생성 ────────────────────────────────────────────────────
        public CameraLightCanvas()
        {
            DoubleBuffered = true;
            BackColor      = CBg;
            AutoScroll     = true;
            AllowDrop      = true;
        }

        // ── 공개 API ─────────────────────────────────────────────────

        public void AddNode(CameraLightNode node)
        {
            _nodes.Add(node);
            UpdateScrollSize();
            Invalidate();
        }

        public IReadOnlyList<CameraLightNode>   Nodes       => _nodes;
        public IReadOnlyList<NodeConnection>    Connections => _connections;

        public NodeConnection FindConnection(CameraLightNode source, CameraLightNode target)
            => _connections.Find(c => c.Source == source && c.Target == target);

        public void RefreshNode(CameraLightNode node)
        {
            if (_nodes.Contains(node)) Invalidate();
        }

        // ── 지오메트리 ──────────────────────────────────────────────

        private Rectangle NodeRect(CameraLightNode n)
            => new Rectangle(n.Position.X, n.Position.Y, NodeW, NodeH);

        // Output port: 우측 중앙
        private Point OutPort(CameraLightNode n)
        {
            var r = NodeRect(n);
            return new Point(r.Right, r.Y + r.Height / 2);
        }

        // Input port: 좌측 중앙
        private Point InPort(CameraLightNode n)
        {
            var r = NodeRect(n);
            return new Point(r.Left, r.Y + r.Height / 2);
        }

        private bool HitPort(Point pt, Point port)
            => Math.Abs(pt.X - port.X) <= PortHit && Math.Abs(pt.Y - port.Y) <= PortHit;

        private (CameraLightNode node, bool isOutput)? HitAnyPort(Point pt)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                var n = _nodes[i];
                if (n.HasOutputPort && HitPort(pt, OutPort(n))) return (n, true);
                if (n.HasInputPort  && HitPort(pt, InPort(n)))  return (n, false);
            }
            return null;
        }

        private CameraLightNode HitNode(Point pt)
        {
            for (int i = _nodes.Count - 1; i >= 0; i--)
                if (NodeRect(_nodes[i]).Contains(pt)) return _nodes[i];
            return null;
        }

        private Point ToCanvas(Point screen)
            => new Point(screen.X - AutoScrollPosition.X, screen.Y - AutoScrollPosition.Y);

        // ── 그리기 ──────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

            DrawGrid(g);

            if (_nodes.Count == 0)
            {
                DrawEmptyHint(g);
                return;
            }

            DrawConnections(g);
            DrawNodes(g);
            DrawPorts(g);
            DrawRubberband(g);
        }

        private void DrawGrid(Graphics g)
        {
            int step = 30;
            using (var pen = new Pen(CGrid, 1f))
            {
                int w = Math.Max(Width,  AutoScrollMinSize.Width)  + step;
                int h = Math.Max(Height, AutoScrollMinSize.Height) + step;
                for (int x = 0; x < w; x += step) g.DrawLine(pen, x, 0, x, h);
                for (int y = 0; y < h; y += step) g.DrawLine(pen, 0, y, w, y);
            }
        }

        private void DrawEmptyHint(Graphics g)
        {
            using (var br = new SolidBrush(Color.FromArgb(70, 75, 88)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString("좌측 팔레트에서 블록을 드래그하세요.",
                    new Font("Segoe UI", 10f), br, new RectangleF(0, 0, Width, Height), sf);
        }

        private void DrawRubberband(Graphics g)
        {
            if (_pendingNode == null) return;
            var src = _pendingIsOutput ? OutPort(_pendingNode) : InPort(_pendingNode);
            var dst = ToCanvas(_mousePos);
            using (var pen = new Pen(CDragLine, 2f) { DashStyle = DashStyle.Dash })
                g.DrawLine(pen, src, dst);
        }

        private void DrawConnections(Graphics g)
        {
            foreach (var c in _connections)
                DrawBezier(g, OutPort(c.Source), InPort(c.Target), CConnLine, 2f);
        }

        private static void DrawBezier(Graphics g, Point src, Point dst, Color col, float width)
        {
            float dx = Math.Abs(dst.X - src.X);
            float t  = Math.Min(dx * 0.5f, 100f);
            var cp1  = new PointF(src.X + t, src.Y);
            var cp2  = new PointF(dst.X - t, dst.Y);

            using (var pen = new Pen(col, width))
                g.DrawBezier(pen,
                    new PointF(src.X, src.Y), cp1, cp2,
                    new PointF(dst.X, dst.Y));

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
            using (var nameFont = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var subFont  = new Font("Segoe UI", 7.5f))
            using (var fmt      = new StringFormat { Trimming = StringTrimming.EllipsisCharacter })
            {
                foreach (var n in _nodes)
                {
                    var rc  = NodeRect(n);
                    var bg  = NodeBg(n.Kind);
                    var bdr = NodeBdr(n.Kind);

                    // 노드 배경
                    using (var path = RoundedPath(rc, 8))
                    using (var br   = new SolidBrush(bg))
                    using (var pen  = new Pen(bdr, n.IsActive ? 2f : 1.2f))
                    { g.FillPath(br, path); g.DrawPath(pen, path); }

                    // 핀 표시 (ImageArea)
                    if (n.IsPinned)
                    {
                        using (var br = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                            g.FillRectangle(br, rc.X, rc.Y, 4, NodeH);
                    }

                    // 레이블
                    var labelRc = new RectangleF(rc.X + 10, rc.Y + 6, rc.Width - 16, rc.Height / 2 - 2);
                    using (var br = new SolidBrush(CText))
                        g.DrawString(n.Label, nameFont, br, labelRc, fmt);

                    // 상태 텍스트
                    var statusRc = new RectangleF(rc.X + 10, rc.Y + rc.Height / 2 + 2, rc.Width - 16, rc.Height / 2 - 6);
                    var statusColor = n.IsActive ? Color.LimeGreen : CSub;
                    using (var br = new SolidBrush(statusColor))
                        g.DrawString(n.StatusText, subFont, br, statusRc, fmt);
                }
            }
        }

        private void DrawPorts(Graphics g)
        {
            foreach (var n in _nodes)
            {
                if (n.HasOutputPort)
                {
                    bool pend = (_pendingNode == n && _pendingIsOutput);
                    DrawCircle(g, OutPort(n), PortR,
                        pend ? CPortPend : COutPort,
                        Color.FromArgb(160, 90, 0));
                }
                if (n.HasInputPort)
                {
                    bool pend = (_pendingNode == n && !_pendingIsOutput);
                    DrawCircle(g, InPort(n), PortR,
                        pend ? CPortPend : CInPort,
                        Color.FromArgb(24, 110, 44));
                }
            }
        }

        private static void DrawCircle(Graphics g, Point c, int r, Color fill, Color border)
        {
            var rc = new Rectangle(c.X - r, c.Y - r, r * 2, r * 2);
            using (var br  = new SolidBrush(fill))       g.FillEllipse(br, rc);
            using (var pen = new Pen(border, 1.5f))       g.DrawEllipse(pen, rc);
        }

        private static GraphicsPath RoundedPath(Rectangle rc, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(rc.X,           rc.Y,            r*2, r*2, 180, 90);
            p.AddArc(rc.Right - r*2, rc.Y,            r*2, r*2, 270, 90);
            p.AddArc(rc.Right - r*2, rc.Bottom - r*2, r*2, r*2,   0, 90);
            p.AddArc(rc.X,           rc.Bottom - r*2, r*2, r*2,  90, 90);
            p.CloseFigure();
            return p;
        }

        // ── 마우스 ──────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            var pt = ToCanvas(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                // 1. 포트 클릭
                var portHit = HitAnyPort(pt);
                if (portHit.HasValue)
                {
                    HandlePortClick(portHit.Value.node, portHit.Value.isOutput);
                    return;
                }

                // 2. pending 중 빈 공간 클릭 → 취소
                if (_pendingNode != null) { CancelPending(); return; }

                // 3. 노드 클릭 → 드래그 준비
                var nodeHit = HitNode(pt);
                if (nodeHit != null)
                {
                    _dragging   = true;
                    _dragNode   = nodeHit;
                    _dragOffset = new Point(pt.X - nodeHit.Position.X, pt.Y - nodeHit.Position.Y);
                    Capture     = true;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                CancelPending();
                var nodeHit = HitNode(pt);
                if (nodeHit != null) ShowContextMenu(nodeHit, e.Location);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button != MouseButtons.Left) return;
            var node = HitNode(ToCanvas(e.Location));
            if (node != null) NodeDoubleClicked?.Invoke(node);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mousePos = e.Location;

            if (_dragging)
            {
                var pt = ToCanvas(e.Location);
                _dragNode.Position = new Point(
                    Math.Max(0, pt.X - _dragOffset.X),
                    Math.Max(0, pt.Y - _dragOffset.Y));
                UpdateScrollSize();
                Invalidate();
                return;
            }

            if (_pendingNode != null) { Invalidate(); return; }

            Cursor = HitAnyPort(ToCanvas(e.Location)).HasValue ? Cursors.Cross : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_dragging)
            {
                Capture   = false;
                _dragging = false;
                _dragNode = null;
                UpdateScrollSize();
                Invalidate();
            }
        }

        // ── 포트 연결 로직 ───────────────────────────────────────────

        private void HandlePortClick(CameraLightNode node, bool isOutput)
        {
            if (_pendingNode == null)
            {
                _pendingNode     = node;
                _pendingIsOutput = isOutput;
                Invalidate();
                return;
            }

            // 같은 포트 재클릭 → 취소
            if (_pendingNode == node && _pendingIsOutput == isOutput)
            {
                CancelPending();
                return;
            }

            // src(output) → dst(input) 방향 결정
            CameraLightNode src, dst;
            if (_pendingIsOutput && !isOutput)      { src = _pendingNode; dst = node; }
            else if (!_pendingIsOutput && isOutput) { src = node; dst = _pendingNode; }
            else { CancelPending(); return; } // output-output 또는 input-input

            if (!src.CanConnectTo(dst))
            {
                ShowIncompatibleHint();
                CancelPending();
                return;
            }

            ToggleConnection(src, dst);
            CancelPending();
        }

        private void ToggleConnection(CameraLightNode src, CameraLightNode dst)
        {
            var existing = FindConnection(src, dst);
            if (existing != null)
            {
                _connections.Remove(existing);
                ConnectionRemoved?.Invoke(existing);
            }
            else
            {
                var conn = new NodeConnection(src, dst);
                _connections.Add(conn);
                ConnectionAdded?.Invoke(conn);
            }
            Invalidate();
        }

        private void CancelPending()
        {
            _pendingNode = null;
            Cursor       = Cursors.Default;
            Invalidate();
        }

        private void ShowIncompatibleHint()
        {
            // 잠시 툴팁 대신 상태는 부모가 처리 — 여기선 생략
        }

        // ── DragDrop (팔레트에서 받기) ───────────────────────────────

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            drgevent.Effect = drgevent.Data.GetDataPresent(DataFormats.StringFormat)
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            drgevent.Effect = drgevent.Data.GetDataPresent(DataFormats.StringFormat)
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (!(drgevent.Data.GetData(DataFormats.StringFormat) is string kindStr)) return;
            if (!Enum.TryParse(kindStr, out NodeKind kind)) return;

            var clientPt = PointToClient(new Point(drgevent.X, drgevent.Y));
            var canvasPt = ToCanvas(clientPt);
            canvasPt     = new Point(Math.Max(0, canvasPt.X - NodeW / 2),
                                     Math.Max(0, canvasPt.Y - NodeH / 2));

            string label;
            switch (kind)
            {
                case NodeKind.Camera:          label = $"Camera {CountOf(kind) + 1}";    break;
                case NodeKind.COMPort:         label = $"COM Port {CountOf(kind) + 1}";  break;
                case NodeKind.LightController: label = $"Light Ctrl {CountOf(kind) + 1}"; break;
                default:                       label = kind.ToString();                   break;
            }

            AddNode(new CameraLightNode(kind, label, canvasPt));
        }

        private int CountOf(NodeKind kind)
        {
            int c = 0;
            foreach (var n in _nodes) if (n.Kind == kind) c++;
            return c;
        }

        // ── 컨텍스트 메뉴 ────────────────────────────────────────────

        private void ShowContextMenu(CameraLightNode node, Point loc)
        {
            var menu = new ContextMenuStrip();
            ApplyDarkTheme(menu);

            // ── Camera 전용 항목 ──────────────────────────────────────
            if (node.Kind == NodeKind.Camera)
            {
                bool isLive  = node.IsActive && node.StatusText == "Live";
                bool canGrab = node.IsActive;

                var itemGrab = new ToolStripMenuItem("Grab Once") { Enabled = canGrab };
                itemGrab.Click += (_, __) => NodeActionRequested?.Invoke(node, "GrabOnce");

                var itemStart = new ToolStripMenuItem("Start Live") { Enabled = canGrab && !isLive };
                itemStart.Click += (_, __) => NodeActionRequested?.Invoke(node, "StartLive");

                var itemStop = new ToolStripMenuItem("Stop Live") { Enabled = isLive };
                itemStop.Click += (_, __) => NodeActionRequested?.Invoke(node, "StopLive");

                menu.Items.Add(itemGrab);
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(itemStart);
                menu.Items.Add(itemStop);
                menu.Items.Add(new ToolStripSeparator());
            }

            // ── 공통 항목 ────────────────────────────────────────────
            menu.Items.Add("설정", null, (_, __) => NodeDoubleClicked?.Invoke(node));

            if (!node.IsPinned)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("노드 제거", null, (_, __) => RemoveNode(node));
            }

            menu.Show(this, loc);
        }

        private static void ApplyDarkTheme(ToolStripDropDown menu)
        {
            menu.BackColor = Color.FromArgb(25, 28, 40);
            menu.ForeColor = Color.FromArgb(210, 215, 220);
            menu.Renderer  = new ToolStripProfessionalRenderer(new DarkMenuColorTable());
        }

        private void RemoveNode(CameraLightNode node)
        {
            // 연결된 커넥션 먼저 제거
            var toRemove = _connections.FindAll(c => c.Source == node || c.Target == node);
            foreach (var c in toRemove) { _connections.Remove(c); ConnectionRemoved?.Invoke(c); }

            _nodes.Remove(node);
            NodeRemoved?.Invoke(node);
            Invalidate();
        }

        // ── 스크롤 ───────────────────────────────────────────────────

        private void UpdateScrollSize()
        {
            int maxX = 400, maxY = 300;
            foreach (var n in _nodes)
            {
                maxX = Math.Max(maxX, n.Position.X + NodeW + 60);
                maxY = Math.Max(maxY, n.Position.Y + NodeH + 60);
            }
            AutoScrollMinSize = new Size(maxX, maxY);
        }
    }

    // ── 다크 메뉴 테마 ───────────────────────────────────────────────────
    internal sealed class DarkMenuColorTable : ProfessionalColorTable
    {
        private static readonly Color CBg       = Color.FromArgb(25, 28, 40);
        private static readonly Color CHover    = Color.FromArgb(42, 148, 255);
        private static readonly Color CBorder   = Color.FromArgb(50, 55, 70);

        public override Color MenuItemSelected          => CHover;
        public override Color MenuItemSelectedGradientBegin => CHover;
        public override Color MenuItemSelectedGradientEnd   => CHover;
        public override Color MenuItemPressedGradientBegin  => CHover;
        public override Color MenuItemPressedGradientEnd    => CHover;
        public override Color MenuBorder                => CBorder;
        public override Color ToolStripDropDownBackground   => CBg;
        public override Color ImageMarginGradientBegin  => CBg;
        public override Color ImageMarginGradientMiddle => CBg;
        public override Color ImageMarginGradientEnd    => CBg;
        public override Color SeparatorDark             => CBorder;
        public override Color SeparatorLight            => CBorder;
    }
}
