using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    public class View3DForm : Form
    {
        // ── Scene data ───────────────────────────────────────────
        private readonly float _wd;
        private readonly float _camHeight, _camOffsetX;
        private readonly float _fovX, _fovY;
        private readonly bool  _obstEnabled;
        private readonly float _obstDist, _obstHeight, _obstOffsetX;
        private readonly float _obstHalfW, _obstHalfH;
        private readonly float _subjectW, _subjectH;

        // ── View state ───────────────────────────────────────────
        private float _rotX   = -28f;
        private float _rotY   = 38f;
        private float _zoom   =  1.0f;
        private float _panX   =  0f;
        private float _panY   =  0f;

        // ── Pivot / view distance ────────────────────────────────
        private float _pivotX, _pivotY, _pivotZ, _viewDist;

        // ── Mouse ────────────────────────────────────────────────
        private Point _lastMouse;
        private bool  _isRotating, _isPanning;

        private Panel _canvas;

        // ────────────────────────────────────────────────────────
        public View3DForm(
            double wd,
            double camHeight, double camOffsetX,
            double fovX,      double fovY,
            bool   obstEnabled,
            double obstDist,  double obstHeight, double obstOffsetX,
            double obstHalfW, double obstHalfH,
            double subjectW,  double subjectH)
        {
            _wd = (float)wd;
            _camHeight = (float)camHeight;  _camOffsetX = (float)camOffsetX;
            _fovX = (float)fovX;            _fovY = (float)fovY;
            _obstEnabled = obstEnabled;
            _obstDist    = (float)obstDist; _obstHeight  = (float)obstHeight;
            _obstOffsetX = (float)obstOffsetX;
            _obstHalfW   = (float)obstHalfW; _obstHalfH = (float)obstHalfH;
            _subjectW    = (float)subjectW;  _subjectH  = (float)subjectH;

            _pivotX   = 0f;
            _pivotY   = _subjectH / 2f;
            _pivotZ   = _wd / 2f;
            _viewDist = Math.Max(_wd, _subjectH) * 1.2f;

            InitUI();
        }

        // ────────────────────────────────────────────────────────
        private void InitUI()
        {
            Text            = "3D View";
            Size            = new Size(1020, 740);
            MinimumSize     = new Size(600, 420);
            BackColor       = Color.FromArgb(10, 12, 18);
            StartPosition   = FormStartPosition.CenterParent;

            _canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(10, 12, 18) };
            SetDoubleBuffered(_canvas);
            _canvas.Paint      += Canvas_Paint;
            _canvas.MouseDown  += Canvas_MouseDown;
            _canvas.MouseMove  += Canvas_MouseMove;
            _canvas.MouseUp    += Canvas_MouseUp;
            _canvas.MouseWheel += Canvas_MouseWheel;

            var btnReset = new Button
            {
                Text      = "Reset View",
                Size      = new Size(90, 26),
                Location  = new Point(8, 8),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(25, 42, 75),
                ForeColor = Color.FromArgb(0, 220, 180),
            };
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(40, 55, 80);
            btnReset.Click += (s, e) =>
            {
                _rotX = -28f; _rotY = 38f; _zoom = 1f; _panX = 0; _panY = 0;
                _canvas.Invalidate();
            };

            Controls.Add(_canvas);
            Controls.Add(btnReset);
            btnReset.BringToFront();
        }

        private static void SetDoubleBuffered(Control c) =>
            typeof(Control)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(c, true);

        // ── Perspective projection ───────────────────────────────
        private PointF Project(float x, float y, float z, int W, int H)
        {
            float rx = x - _pivotX;
            float ry = y - _pivotY;
            float rz = z - _pivotZ;

            // Yaw (Y axis)
            double radY = _rotY * Math.PI / 180.0;
            float cosY  = (float)Math.Cos(radY), sinY = (float)Math.Sin(radY);
            float x1 =  rx * cosY + rz * sinY;
            float y1 =  ry;
            float z1 = -rx * sinY + rz * cosY;

            // Pitch (X axis)
            double radX = _rotX * Math.PI / 180.0;
            float cosX  = (float)Math.Cos(radX), sinX = (float)Math.Sin(radX);
            float x2 = x1;
            float y2 = y1 * cosX - z1 * sinX;
            float z2 = y1 * sinX + z1 * cosX;

            float zCam = z2 + _viewDist / _zoom;
            if (zCam < 0.1f) zCam = 0.1f;

            float focal = Math.Min(W, H) * 0.54f;
            return new PointF(
                W / 2f + _panX + x2 * focal / zCam,
                H / 2f + _panY - y2 * focal / zCam);
        }

        // ── 3-D drawing helpers ──────────────────────────────────
        private void Line3D(Graphics g, Pen pen,
            float x1, float y1, float z1,
            float x2, float y2, float z2, int W, int H)
        {
            g.DrawLine(pen, Project(x1, y1, z1, W, H), Project(x2, y2, z2, W, H));
        }

        private void FillQuad3D(Graphics g, Brush brush,
            float ax, float ay, float az,
            float bx, float by, float bz,
            float cx, float cy, float cz,
            float dx, float dy, float dz, int W, int H)
        {
            g.FillPolygon(brush, new[]
            {
                Project(ax, ay, az, W, H), Project(bx, by, bz, W, H),
                Project(cx, cy, cz, W, H), Project(dx, dy, dz, W, H),
            });
        }

        private void OutlineQuad3D(Graphics g, Pen pen,
            float ax, float ay, float az,
            float bx, float by, float bz,
            float cx, float cy, float cz,
            float dx, float dy, float dz, int W, int H)
        {
            var a = Project(ax, ay, az, W, H); var b = Project(bx, by, bz, W, H);
            var c = Project(cx, cy, cz, W, H); var d = Project(dx, dy, dz, W, H);
            g.DrawLine(pen, a, b); g.DrawLine(pen, b, c);
            g.DrawLine(pen, c, d); g.DrawLine(pen, d, a);
        }

        private void Box3D(Graphics g, Pen pen,
            float cx, float cy, float cz, float hw, float hh, float hd, int W, int H)
        {
            var pts = new PointF[8];
            pts[0] = Project(cx-hw, cy-hh, cz-hd, W, H);
            pts[1] = Project(cx+hw, cy-hh, cz-hd, W, H);
            pts[2] = Project(cx-hw, cy+hh, cz-hd, W, H);
            pts[3] = Project(cx+hw, cy+hh, cz-hd, W, H);
            pts[4] = Project(cx-hw, cy-hh, cz+hd, W, H);
            pts[5] = Project(cx+hw, cy-hh, cz+hd, W, H);
            pts[6] = Project(cx-hw, cy+hh, cz+hd, W, H);
            pts[7] = Project(cx+hw, cy+hh, cz+hd, W, H);

            int[,] edges = {
                {0,1},{1,3},{3,2},{2,0},  // front face
                {4,5},{5,7},{7,6},{6,4},  // back face
                {0,4},{1,5},{2,6},{3,7},  // connecting
            };
            for (int i = 0; i < 12; i++)
                g.DrawLine(pen, pts[edges[i, 0]], pts[edges[i, 1]]);
        }

        // ── Main paint ───────────────────────────────────────────
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode    = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int W = _canvas.Width, H = _canvas.Height;
            g.Clear(Color.FromArgb(10, 12, 18));

            // ── Floor grid ───────────────────────────────────────
            float step   = 200f;
            float gXMin  = -Math.Max(_subjectW, 600f);
            float gXMax  =  Math.Max(_subjectW, 600f);
            float gZMin  = -200f;
            float gZMax  =  _wd + 200f;

            // Floor fill
            using (SolidBrush b = new SolidBrush(Color.FromArgb(38, 70, 72, 50)))
                FillQuad3D(g, b, gXMin, 0, gZMin, gXMax, 0, gZMin, gXMax, 0, gZMax, gXMin, 0, gZMax, W, H);

            using (Pen p = new Pen(Color.FromArgb(28, 42, 58), 1f))
            {
                for (float x = (float)(Math.Ceiling(gXMin / step) * step); x <= gXMax; x += step)
                    Line3D(g, p, x, 0, gZMin, x, 0, gZMax, W, H);
                for (float z = (float)(Math.Ceiling(gZMin / step) * step); z <= gZMax; z += step)
                    Line3D(g, p, gXMin, 0, z, gXMax, 0, z, W, H);
            }

            // ── World axes ───────────────────────────────────────
            float al = 150f;
            using (Pen px = new Pen(Color.FromArgb(180, 55, 55), 1.5f))
            using (Pen py = new Pen(Color.FromArgb(55, 175, 55), 1.5f))
            using (Pen pz = new Pen(Color.FromArgb(55, 95, 215), 1.5f))
            using (Font fLbl = new Font("Segoe UI", 7.5f))
            {
                Line3D(g, px, 0, 0, 0, al, 0,  0, W, H);
                Line3D(g, py, 0, 0, 0, 0,  al, 0, W, H);
                Line3D(g, pz, 0, 0, 0, 0,  0, al, W, H);
                using (SolidBrush bx = new SolidBrush(Color.FromArgb(180, 55, 55)))
                using (SolidBrush by = new SolidBrush(Color.FromArgb(55, 175, 55)))
                using (SolidBrush bz = new SolidBrush(Color.FromArgb(55, 95, 215)))
                {
                    var px2 = Project(al + 20, 0, 0, W, H);
                    var py2 = Project(0, al + 20, 0, W, H);
                    var pz2 = Project(0, 0, al + 20, W, H);
                    g.DrawString("X", fLbl, bx, px2); g.DrawString("Y", fLbl, by, py2); g.DrawString("Z(WD)", fLbl, bz, pz2);
                }
            }

            // ── Subject plane ────────────────────────────────────
            float sw = _subjectW, sh = _subjectH, sz = _wd;
            float sx1 = -sw / 2f, sx2 = sw / 2f;

            using (SolidBrush b = new SolidBrush(Color.FromArgb(22, 200, 180, 80)))
                FillQuad3D(g, b, sx1, 0, sz, sx2, 0, sz, sx2, sh, sz, sx1, sh, sz, W, H);
            using (Pen p = new Pen(Color.FromArgb(200, 180, 80), 2f))
                OutlineQuad3D(g, p, sx1, 0, sz, sx2, 0, sz, sx2, sh, sz, sx1, sh, sz, W, H);

            var subLbl = Project(sx2 + 10, sh, sz, W, H);
            using (Font f = new Font("Segoe UI", 8.5f))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(200, 180, 80)))
                g.DrawString($"Subject  {_subjectW:F0}×{_subjectH:F0} mm", f, b, subLbl.X, subLbl.Y - 18);

            // ── FOV at subject plane ─────────────────────────────
            float fx1 = _camOffsetX - _fovX / 2f, fx2 = _camOffsetX + _fovX / 2f;
            float fy1 = _camHeight  - _fovY / 2f, fy2 = _camHeight  + _fovY / 2f;

            using (SolidBrush b = new SolidBrush(Color.FromArgb(18, 60, 200, 100)))
                FillQuad3D(g, b, fx1, fy1, sz, fx2, fy1, sz, fx2, fy2, sz, fx1, fy2, sz, W, H);
            using (Pen p = new Pen(Color.FromArgb(60, 200, 100), 1.5f))
                OutlineQuad3D(g, p, fx1, fy1, sz, fx2, fy1, sz, fx2, fy2, sz, fx1, fy2, sz, W, H);

            // ── FOV cone ─────────────────────────────────────────
            float cz = 0f;
            using (Pen p = new Pen(Color.FromArgb(38, 60, 200, 100), 1f) { DashStyle = DashStyle.Dash })
            {
                Line3D(g, p, _camOffsetX, _camHeight, cz, fx1, fy1, sz, W, H);
                Line3D(g, p, _camOffsetX, _camHeight, cz, fx2, fy1, sz, W, H);
                Line3D(g, p, _camOffsetX, _camHeight, cz, fx2, fy2, sz, W, H);
                Line3D(g, p, _camOffsetX, _camHeight, cz, fx1, fy2, sz, W, H);
            }

            // Optical axis
            using (Pen p = new Pen(Color.FromArgb(40, 80, 130, 210), 1f) { DashStyle = DashStyle.Dot })
                Line3D(g, p, _camOffsetX, _camHeight, cz, _camOffsetX, _camHeight, sz, W, H);

            // Camera height guide (vertical from floor)
            using (Pen p = new Pen(Color.FromArgb(40, 60, 95, 140), 1f) { DashStyle = DashStyle.Dot })
                Line3D(g, p, _camOffsetX, 0, cz, _camOffsetX, _camHeight, cz, W, H);

            // ── Obstacle ─────────────────────────────────────────
            if (_obstEnabled && _obstDist > 0 && _obstDist < _wd)
            {
                float od  = _obstDist;
                float oh  = _obstHeight;
                float ox  = _obstOffsetX;
                float dep = Math.Max(10f, Math.Min(_obstHalfW, 30f)); // visual box depth

                // Shadow on subject plane
                float pr    = _wd / od;
                float shL   = _camOffsetX + pr * (ox - _obstHalfW - _camOffsetX);
                float shR   = _camOffsetX + pr * (ox + _obstHalfW - _camOffsetX);
                float shTop = _camHeight  + pr * (oh + _obstHalfH - _camHeight);
                float shBot = _camHeight  + pr * (oh - _obstHalfH - _camHeight);
                shL   = Math.Max(-sw / 2f, Math.Min(sw / 2f, shL));
                shR   = Math.Max(-sw / 2f, Math.Min(sw / 2f, shR));
                shTop = Math.Max(0, Math.Min(sh, shTop));
                shBot = Math.Max(0, Math.Min(sh, shBot));

                if (shR > shL && shTop > shBot)
                {
                    // Gray shadow
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(55, 140, 140, 150)))
                        FillQuad3D(g, b, shL, shBot, sz, shR, shBot, sz, shR, shTop, sz, shL, shTop, sz, W, H);

                    // Red: FOV intersection (blind zone)
                    float isL = Math.Max(shL, fx1), isR = Math.Min(shR, fx2);
                    float isB = Math.Max(shBot, fy1), isT = Math.Min(shTop, fy2);
                    if (isR > isL && isT > isB)
                    {
                        using (SolidBrush b = new SolidBrush(Color.FromArgb(100, 210, 75, 65)))
                            FillQuad3D(g, b, isL, isB, sz, isR, isB, sz, isR, isT, sz, isL, isT, sz, W, H);
                    }
                }

                // Shadow projection lines (corner rays from camera through obstacle)
                using (Pen p = new Pen(Color.FromArgb(55, 220, 150, 60), 1f) { DashStyle = DashStyle.Dot })
                {
                    Line3D(g, p, _camOffsetX, _camHeight, 0, ox - _obstHalfW, oh - _obstHalfH, od, W, H);
                    Line3D(g, p, _camOffsetX, _camHeight, 0, ox + _obstHalfW, oh - _obstHalfH, od, W, H);
                    Line3D(g, p, _camOffsetX, _camHeight, 0, ox - _obstHalfW, oh + _obstHalfH, od, W, H);
                    Line3D(g, p, _camOffsetX, _camHeight, 0, ox + _obstHalfW, oh + _obstHalfH, od, W, H);
                }

                // Obstacle face fill
                using (SolidBrush b = new SolidBrush(Color.FromArgb(75, 220, 150, 60)))
                    FillQuad3D(g, b,
                        ox - _obstHalfW, oh - _obstHalfH, od,
                        ox + _obstHalfW, oh - _obstHalfH, od,
                        ox + _obstHalfW, oh + _obstHalfH, od,
                        ox - _obstHalfW, oh + _obstHalfH, od, W, H);

                // Obstacle box outline
                using (Pen p = new Pen(Color.FromArgb(220, 150, 60), 2f))
                    Box3D(g, p, ox, oh, od, _obstHalfW, _obstHalfH, dep, W, H);

                // Height guide
                using (Pen p = new Pen(Color.FromArgb(50, 220, 150, 60), 1f) { DashStyle = DashStyle.Dot })
                    Line3D(g, p, ox, 0, od, ox, oh, od, W, H);

                // Label
                var oLbl = Project(ox + _obstHalfW + 10, oh + _obstHalfH, od, W, H);
                using (Font f = new Font("Segoe UI", 8f))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(220, 150, 60)))
                    g.DrawString($"장애물  Z={od:F0}mm", f, b, oLbl.X, oLbl.Y - 14);
            }

            // ── Camera body ───────────────────────────────────────
            float bW = 30f, bH = 22f, bD = 38f;
            using (SolidBrush b = new SolidBrush(Color.FromArgb(45, 90, 130, 200)))
            {
                FillQuad3D(g, b,
                    _camOffsetX - bW, _camHeight - bH, 0,
                    _camOffsetX + bW, _camHeight - bH, 0,
                    _camOffsetX + bW, _camHeight + bH, 0,
                    _camOffsetX - bW, _camHeight + bH, 0, W, H);
            }
            using (Pen p = new Pen(Color.FromArgb(130, 170, 230), 2f))
                Box3D(g, p, _camOffsetX, _camHeight, -bD / 2f, bW, bH, bD / 2f, W, H);

            // Lens ring
            float lr = 11f;
            using (Pen p = new Pen(Color.FromArgb(190, 210, 255), 1.5f))
                OutlineQuad3D(g, p,
                    _camOffsetX - lr, _camHeight - lr, 0,
                    _camOffsetX + lr, _camHeight - lr, 0,
                    _camOffsetX + lr, _camHeight + lr, 0,
                    _camOffsetX - lr, _camHeight + lr, 0, W, H);

            var camLbl = Project(_camOffsetX + bW + 8, _camHeight + bH, 0, W, H);
            using (Font f = new Font("Segoe UI", 8.5f))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(150, 180, 230)))
                g.DrawString("Camera", f, b, camLbl.X, camLbl.Y - 16);

            // ── Distance annotations (2D screen-space overlay) ────
            DrawDimAnnotations(g, W, H, bW);

            // ── Info panel (top-right) ────────────────────────────
            {
                var lines = new System.Collections.Generic.List<(string label, string val, Color col)>();
                lines.Add(("[ Camera ]", "", Color.FromArgb(150, 180, 230)));
                lines.Add(("  X offset", $"{_camOffsetX:+0.#;-0.#;0} mm", Color.FromArgb(190, 210, 255)));
                lines.Add(("  Height",   $"{_camHeight:F0} mm",            Color.FromArgb(190, 210, 255)));
                lines.Add(("  WD",       $"{_wd:F0} mm",                   Color.FromArgb(190, 210, 255)));
                lines.Add(("  FOV",      $"{_fovX:F0} × {_fovY:F0} mm",   Color.FromArgb(190, 210, 255)));
                lines.Add(("", "", Color.Transparent));
                lines.Add(("[ Subject ]", "", Color.FromArgb(200, 180, 80)));
                lines.Add(("  Size",     $"{_subjectW:F0} × {_subjectH:F0} mm", Color.FromArgb(220, 200, 100)));
                lines.Add(("  Z",        $"{_wd:F0} mm (WD)",              Color.FromArgb(220, 200, 100)));
                if (_obstEnabled && _obstDist > 0 && _obstDist < _wd)
                {
                    lines.Add(("", "", Color.Transparent));
                    lines.Add(("[ 장애물 ]", "", Color.FromArgb(220, 150, 60)));
                    lines.Add(("  X offset", $"{_obstOffsetX:+0.#;-0.#;0} mm",  Color.FromArgb(240, 180, 90)));
                    lines.Add(("  Center H", $"{_obstHeight:F0} mm",             Color.FromArgb(240, 180, 90)));
                    lines.Add(("  Size",     $"{_obstHalfW*2:F0} × {_obstHalfH*2:F0} mm", Color.FromArgb(240, 180, 90)));
                    lines.Add(("  Cam→장애물", $"{_obstDist:F0} mm",             Color.FromArgb(240, 180, 90)));
                    lines.Add(("  장애물→Sub", $"{_wd - _obstDist:F0} mm",       Color.FromArgb(240, 180, 90)));
                }

                using (Font fInfo = new Font("Segoe UI", 8.5f))
                using (Font fHdr  = new Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold))
                {
                    float lineH = 16f;
                    float panelW = 185f;
                    float panelH = lines.Count * lineH + 10f;
                    float px2 = W - panelW - 10;
                    float py2 = 10;

                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(110, 12, 16, 24)))
                        g.FillRectangle(bg, px2 - 6, py2 - 4, panelW, panelH);

                    for (int i = 0; i < lines.Count; i++)
                    {
                        var (lbl, val, col) = lines[i];
                        if (lbl == "") continue;
                        bool isHdr = lbl.StartsWith("[");
                        using (SolidBrush b2 = new SolidBrush(col))
                        {
                            Font f2 = isHdr ? fHdr : fInfo;
                            g.DrawString(lbl, f2, b2, px2, py2 + i * lineH);
                            if (val != "")
                            {
                                SizeF vs = g.MeasureString(val, fInfo);
                                using (SolidBrush bv = new SolidBrush(Color.FromArgb(220, 230, 240)))
                                    g.DrawString(val, fInfo, bv, px2 + panelW - vs.Width - 8, py2 + i * lineH);
                            }
                        }
                    }
                }
            }

            // ── Info hint ─────────────────────────────────────────
            using (Font f = new Font("Segoe UI", 8f))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(60, 80, 100)))
                g.DrawString("좌클릭 드래그: 회전  |  우클릭 드래그: 이동  |  휠: 줌", f, b, 8, H - 22);
        }

        // ── Distance annotations (2D overlay) ────────────────────
        private void DrawDimAnnotations(Graphics g, int W, int H, float bW)
        {
            using (Font f = new Font("Segoe UI", 8f))
            using (Pen pLine = new Pen(Color.FromArgb(80, 100, 120), 1f) { DashStyle = DashStyle.Dot })
            using (Pen pObst = new Pen(Color.FromArgb(160, 120, 50), 1f) { DashStyle = DashStyle.Dot })
            {
                // Helper: draw a 2D dim label between two screen points with a bracket line
                void Dim2D(PointF a, PointF b, string text, Color col, Pen pen, float offsetPx = -18f)
                {
                    float mx = (a.X + b.X) / 2f;
                    float my = (a.Y + b.Y) / 2f;
                    // bracket
                    g.DrawLine(pen, a.X, a.Y, a.X, a.Y + offsetPx);
                    g.DrawLine(pen, b.X, b.Y, b.X, b.Y + offsetPx);
                    g.DrawLine(pen, a.X, a.Y + offsetPx, b.X, b.Y + offsetPx);
                    // tick ends
                    g.DrawLine(pen, a.X - 3, a.Y + offsetPx, a.X + 3, a.Y + offsetPx);
                    g.DrawLine(pen, b.X - 3, b.Y + offsetPx, b.X + 3, b.Y + offsetPx);
                    // label
                    SizeF sz = g.MeasureString(text, f);
                    float lx = mx - sz.Width / 2f;
                    float ly = my + offsetPx - sz.Height - 2f;
                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(130, 10, 12, 18)))
                        g.FillRectangle(bg, lx - 2, ly - 1, sz.Width + 4, sz.Height + 2);
                    using (SolidBrush b2 = new SolidBrush(col))
                        g.DrawString(text, f, b2, lx, ly);
                }

                void HeightDim(PointF bottom, PointF top, string text, Color col, Pen pen)
                {
                    float ox = 22f;
                    g.DrawLine(pen, bottom.X, bottom.Y, bottom.X + ox, bottom.Y);
                    g.DrawLine(pen, top.X,    top.Y,    top.X    + ox, top.Y);
                    g.DrawLine(pen, bottom.X + ox, bottom.Y, top.X + ox, top.Y);
                    g.DrawLine(pen, bottom.X + ox - 3, bottom.Y, bottom.X + ox + 3, bottom.Y);
                    g.DrawLine(pen, top.X    + ox - 3, top.Y,    top.X    + ox + 3, top.Y);
                    float my = (bottom.Y + top.Y) / 2f;
                    SizeF sz = g.MeasureString(text, f);
                    float lx = bottom.X + ox + 4;
                    float ly = my - sz.Height / 2f;
                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(130, 10, 12, 18)))
                        g.FillRectangle(bg, lx - 2, ly - 1, sz.Width + 4, sz.Height + 2);
                    using (SolidBrush b2 = new SolidBrush(col))
                        g.DrawString(text, f, b2, lx, ly);
                }

                // Project key points (at ground level for distance markers)
                var pCamFloor  = Project(_camOffsetX, 0, 0,    W, H);
                var pSubFloor  = Project(0,           0, _wd,  W, H);
                var pCamTop    = Project(_camOffsetX, _camHeight, 0, W, H);

                Color colWD   = Color.FromArgb(120, 160, 210);
                Color colObst = Color.FromArgb(210, 160, 70);

                // WD: cam floor → subject floor (along bottom of scene)
                Dim2D(pCamFloor, pSubFloor, $"WD = {_wd:F0} mm", colWD, pLine, 28f);

                // Camera height
                HeightDim(pCamFloor, pCamTop, $"{_camHeight:F0} mm", colWD, pLine);

                if (_obstEnabled && _obstDist > 0 && _obstDist < _wd)
                {
                    var pObstFloor = Project(_obstOffsetX, 0,             _obstDist, W, H);
                    var pObstTop   = Project(_obstOffsetX, _obstHeight + _obstHalfH, _obstDist, W, H);

                    // Cam → Obstacle
                    Dim2D(pCamFloor, pObstFloor, $"{_obstDist:F0} mm", colObst, pObst, 8f);
                    // Obstacle → Subject
                    Dim2D(pObstFloor, pSubFloor, $"{_wd - _obstDist:F0} mm", colObst, pObst, 8f);
                    // Obstacle height
                    HeightDim(pObstFloor, pObstTop, $"{_obstHeight + _obstHalfH:F0} mm", colObst, pObst);
                }
            }
        }

        // ── Mouse events ──────────────────────────────────────────
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _lastMouse = e.Location;
            if (e.Button == MouseButtons.Left)  { _isRotating = true; _canvas.Cursor = Cursors.SizeAll; }
            if (e.Button == MouseButtons.Right) { _isPanning  = true; _canvas.Cursor = Cursors.SizeAll; }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isRotating && !_isPanning) return;
            int dx = e.X - _lastMouse.X;
            int dy = e.Y - _lastMouse.Y;
            _lastMouse = e.Location;

            if (_isRotating)
            {
                _rotY -= dx * 0.45f;
                _rotX -= dy * 0.45f;
                _rotX  = Math.Max(-89f, Math.Min(89f, _rotX));
            }
            else
            {
                _panX += dx;
                _panY += dy;
            }
            _canvas.Invalidate();
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            _isRotating = _isPanning = false;
            _canvas.Cursor = Cursors.Default;
        }

        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            _zoom *= e.Delta > 0 ? 1.12f : (1f / 1.12f);
            _zoom  = Math.Max(0.08f, Math.Min(12f, _zoom));
            _canvas.Invalidate();
        }
    }
}
