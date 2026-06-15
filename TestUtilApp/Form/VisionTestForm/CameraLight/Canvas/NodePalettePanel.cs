using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestUtilApp.CameraLight
{
    /// <summary>
    /// 좌측 팔레트 — 드래그 가능한 블록 목록.
    /// Camera / LightController / COMPort 를 캔버스로 드래그하면 노드가 생성됩니다.
    /// </summary>
    internal sealed class NodePalettePanel : Panel
    {
        private static readonly Color CBg      = Color.FromArgb(15, 18, 28);
        private static readonly Color CTitle   = Color.FromArgb(100, 110, 130);
        private static readonly Color CText    = Color.FromArgb(200, 205, 215);

        private readonly List<(NodeKind kind, string label, Color color)> _items
            = new List<(NodeKind, string, Color)>
        {
            (NodeKind.Camera,          "Camera",        Color.FromArgb(42, 148, 255)),
            (NodeKind.LightController, "Light Ctrl",    Color.FromArgb(230, 200, 40)),
            (NodeKind.COMPort,         "COM Port",      Color.FromArgb(230, 140, 30)),
        };

        private const int ItemH    = 52;
        private const int ItemPadY = 8;
        private const int StartY   = 40;

        private int _hoverIndex = -1;

        // drag state
        private int   _dragIndex  = -1;
        private Point _dragStart;

        public NodePalettePanel()
        {
            DoubleBuffered = true;
            BackColor      = CBg;
            Width          = 170;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 헤더
            using (var br = new SolidBrush(CTitle))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
                g.DrawString("BLOCKS", new Font("Segoe UI", 8f, FontStyle.Bold), br,
                    new RectangleF(0, 10, Width, 20), sf);

            // 구분선
            using (var pen = new Pen(Color.FromArgb(35, 40, 55)))
                g.DrawLine(pen, 10, 30, Width - 10, 30);

            for (int i = 0; i < _items.Count; i++)
            {
                var (kind, label, color) = _items[i];
                var rc = ItemRect(i);
                bool hover = (i == _hoverIndex);

                // 배경
                var bg = hover
                    ? Color.FromArgb(40, color.R, color.G, color.B)
                    : Color.FromArgb(25, 28, 40);
                using (var path = RoundedRect(rc, 6))
                using (var br   = new SolidBrush(bg))
                    g.FillPath(br, path);

                // 테두리
                using (var path = RoundedRect(rc, 6))
                using (var pen  = new Pen(Color.FromArgb(hover ? 200 : 80, color.R, color.G, color.B), 1.2f))
                    g.DrawPath(pen, path);

                // 색상 바 (좌측)
                using (var br = new SolidBrush(color))
                    g.FillRectangle(br, rc.X, rc.Y + 8, 3, rc.Height - 16);

                // 아이콘 (간단한 종류 표시)
                string icon;
                switch (kind)
                {
                    case NodeKind.Camera:          icon = "Cam";   break;
                    case NodeKind.LightController: icon = "Lgt";   break;
                    case NodeKind.COMPort:         icon = "COM";   break;
                    default:                       icon = "□";     break;
                }
                using (var br = new SolidBrush(color))
                    g.DrawString(icon, new Font("Segoe UI", 8f, System.Drawing.FontStyle.Bold), br,
                        new RectangleF(rc.X + 8, rc.Y + 6, 28, 18));

                // 레이블
                using (var br = new SolidBrush(CText))
                    g.DrawString(label, new Font("Segoe UI", 8.5f, FontStyle.Bold), br,
                        new RectangleF(rc.X + 8, rc.Y + 26, rc.Width - 12, 18));

                // 드래그 힌트
                using (var br = new SolidBrush(CTitle))
                    g.DrawString("drag →", new Font("Segoe UI", 7f), br,
                        new RectangleF(rc.X + 8, rc.Y + 36, rc.Width - 12, 14));
            }
        }

        private Rectangle ItemRect(int i)
            => new Rectangle(8, StartY + i * (ItemH + ItemPadY), Width - 16, ItemH);

        private int HitItem(Point pt)
        {
            for (int i = 0; i < _items.Count; i++)
                if (ItemRect(i).Contains(pt)) return i;
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragIndex >= 0)
            {
                if (Math.Abs(e.X - _dragStart.X) > 5 || Math.Abs(e.Y - _dragStart.Y) > 5)
                {
                    DoDragDrop(_items[_dragIndex].kind.ToString(), DragDropEffects.Copy);
                    _dragIndex = -1;
                }
                return;
            }

            int hit = HitItem(e.Location);
            if (hit != _hoverIndex)
            {
                _hoverIndex = hit;
                Cursor      = hit >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            _dragIndex = HitItem(e.Location);
            _dragStart = e.Location;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverIndex = -1;
            Invalidate();
        }

        private static GraphicsPath RoundedRect(Rectangle rc, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(rc.X,           rc.Y,            r*2, r*2, 180, 90);
            p.AddArc(rc.Right - r*2, rc.Y,            r*2, r*2, 270, 90);
            p.AddArc(rc.Right - r*2, rc.Bottom - r*2, r*2, r*2,   0, 90);
            p.AddArc(rc.X,           rc.Bottom - r*2, r*2, r*2,  90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
