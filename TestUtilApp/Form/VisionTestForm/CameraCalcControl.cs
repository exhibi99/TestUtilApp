using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class CameraCalcControl : UserControl, IActivatable
    {
        // ── calculated values
        private double? _wd, _fl, _sensorX, _sensorY, _fovX, _fovY;

        // ── side-view layout constants (shared between Paint and mouse handlers)
        private const int SideMarginL = 60;
        private const int SideMarginR = 80;
        private const int SideMarginTop = 35;
        private const int SideMarginBot = 52;

        // ── drag state
        private double _pixelsPerMm = 0;   // set in Calculate(); fixed while dragging
        private bool _isDragging = false;
        private int _dragOffsetX = 0;
        private bool _suppressInputEvent = false;

        public CameraCalcControl()
        {
            InitializeComponent();
            ApplyTheme();
        }

        public void OnActivated()
        {
            Calculate(silent: true);
        }

        // ─────────────────────────────────────────────
        //  Layout helper  (shared by Paint + mouse)
        // ─────────────────────────────────────────────
        private void GetSideLayout(out int midY, out int subX, out int camX)
        {
            int W = Math.Max(pnlSideView.Width, 200);
            int H = Math.Max(pnlSideView.Height, 100);
            int drawW = W - SideMarginL - SideMarginR;
            int drawH = H - SideMarginTop - SideMarginBot;
            midY = SideMarginTop + drawH / 2;
            subX = SideMarginL + (int)(drawW * 0.90);

            if (_pixelsPerMm > 0 && _wd != null)
            {
                camX = subX - (int)(_wd.Value * _pixelsPerMm);
                int leftMin = SideMarginL + 10;
                int rightMax = subX - 30;
                camX = Math.Max(leftMin, Math.Min(camX, rightMax));
            }
            else
            {
                camX = SideMarginL + (int)(drawW * 0.08);
            }
        }

        private Rectangle GetCameraHitRect(int camX, int midY)
        {
            return new Rectangle(camX - 38, midY - 22, 54, 44);
        }

        // ─────────────────────────────────────────────
        //  Calculate
        // ─────────────────────────────────────────────
        private void btnCalcFov_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void InputChanged(object sender, EventArgs e)
        {
            if (_suppressInputEvent) return;
            Calculate(silent: true);
        }

        private void Calculate(bool silent = false)
        {
            if (!double.TryParse(txtFovWD.Text.Trim(), out double wd) ||
                !double.TryParse(txtFovFL.Text.Trim(), out double fl) ||
                !double.TryParse(txtFovSensorX.Text.Trim(), out double sx) ||
                !double.TryParse(txtFovSensorY.Text.Trim(), out double sy) ||
                fl == 0)
            {
                if (!silent)
                    MessageBox.Show("입력값을 확인하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _wd = wd; _fl = fl; _sensorX = sx; _sensorY = sy;
            _fovX = wd * sx / fl;
            _fovY = wd * sy / fl;

            // Scale: WD spans 75% of available pixel range → allows dragging further
            if (!_isDragging)
            {
                int W = Math.Max(pnlSideView.Width, 200);
                int drawW = W - SideMarginL - SideMarginR;
                int subX = SideMarginL + (int)(drawW * 0.90);
                int leftMin = SideMarginL + 10;
                _pixelsPerMm = (subX - leftMin) * 0.75 / wd;
            }

            lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
            lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";

            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        // ─────────────────────────────────────────────
        //  Side View — mouse drag
        // ─────────────────────────────────────────────
        private void pnlSideView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_wd == null || _pixelsPerMm <= 0) return;
            GetSideLayout(out int midY, out int subX, out int camX);

            if (GetCameraHitRect(camX, midY).Contains(e.Location))
            {
                _isDragging = true;
                _dragOffsetX = e.X - camX;
                pnlSideView.Cursor = Cursors.SizeWE;
            }
        }

        private void pnlSideView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_wd == null) return;

            GetSideLayout(out int midY, out int subX, out int camX);

            if (!_isDragging)
            {
                // hover cursor feedback
                pnlSideView.Cursor = GetCameraHitRect(camX, midY).Contains(e.Location)
                    ? Cursors.SizeWE
                    : Cursors.Default;
                return;
            }

            // drag: compute new WD from mouse position
            int newCamX = e.X - _dragOffsetX;
            int leftMin = SideMarginL + 10;
            int rightMax = subX - 30;
            newCamX = Math.Max(leftMin, Math.Min(newCamX, rightMax));

            double newWD = (subX - newCamX) / _pixelsPerMm;
            newWD = Math.Round(newWD / 10.0) * 10.0;
            newWD = Math.Max(10.0, newWD);

            _wd = newWD;
            _fovX = _wd.Value * _sensorX.Value / _fl.Value;
            _fovY = _wd.Value * _sensorY.Value / _fl.Value;

            // update UI without triggering InputChanged → Calculate
            _suppressInputEvent = true;
            txtFovWD.Text = _wd.Value.ToString("F1");
            _suppressInputEvent = false;

            lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
            lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";

            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        private void pnlSideView_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;
            _isDragging = false;
            pnlSideView.Cursor = Cursors.Default;
        }

        // ─────────────────────────────────────────────
        //  Side View Paint
        // ─────────────────────────────────────────────
        private void pnlSideView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int W = pnlSideView.Width;
            int H = pnlSideView.Height;
            g.Clear(Color.FromArgb(16, 20, 28));

            if (_fovX == null)
            {
                DrawPlaceholder(g, W, H, "값을 입력하고 Calculate를 누르세요.");
                return;
            }

            int drawH = H - SideMarginTop - SideMarginBot;
            GetSideLayout(out int midY, out int subX, out int camX);

            int fovHalfH = (int)(drawH * 0.36);
            fovHalfH = Math.Max(20, Math.Min(fovHalfH, drawH / 2 - 18));

            Color colFov = Color.FromArgb(60, 200, 100);
            Color colWD = Color.FromArgb(80, 160, 255);
            Color colSubject = Color.FromArgb(200, 180, 80);
            Color colCamera = _isDragging
                ? Color.FromArgb(220, 230, 255)
                : Color.FromArgb(150, 180, 220);
            Color colDim = Color.FromArgb(120, 130, 145);

            // FOV shaded area
            using (SolidBrush brushFovArea = new SolidBrush(Color.FromArgb(18, 60, 200, 100)))
            {
                g.FillPolygon(brushFovArea, new Point[]
                {
                    new Point(camX, midY),
                    new Point(subX, midY - fovHalfH),
                    new Point(subX, midY + fovHalfH),
                });
            }

            // FOV lines
            using (Pen penFov = new Pen(colFov, 1.5f))
            {
                penFov.DashStyle = DashStyle.Dash;
                g.DrawLine(penFov, camX, midY, subX, midY - fovHalfH);
                g.DrawLine(penFov, camX, midY, subX, midY + fovHalfH);
            }

            // Optical axis
            using (Pen penAxis = new Pen(Color.FromArgb(60, 100, 140), 1f))
            {
                penAxis.DashStyle = DashStyle.Dot;
                g.DrawLine(penAxis, camX, midY, subX + 12, midY);
            }

            // Camera body
            Rectangle camRect = new Rectangle(camX - 30, midY - 12, 30, 24);
            Color camFill = _isDragging
                ? Color.FromArgb(55, 72, 100)
                : Color.FromArgb(40, 55, 75);
            using (SolidBrush bCam = new SolidBrush(camFill))
                g.FillRectangle(bCam, camRect);
            using (Pen penCam = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawRectangle(penCam, camRect);

            // Lens circle
            using (Pen penCam = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawEllipse(penCam, camX - 8, midY - 8, 16, 16);

            // Drag hint (↔ icon above camera when hovering/dragging)
            if (_isDragging)
            {
                using (Font fHint = new Font("Segoe UI", 7.5f))
                using (SolidBrush bHint = new SolidBrush(Color.FromArgb(180, 200, 255)))
                {
                    string hint = "↔";
                    SizeF szH = g.MeasureString(hint, fHint);
                    g.DrawString(hint, fHint, bHint, camX - 30 + 15 - szH.Width / 2f, midY - 27);
                }
            }

            // Subject plane
            using (Pen penSub = new Pen(colSubject, 2.5f))
                g.DrawLine(penSub, subX, midY - fovHalfH - 12, subX, midY + fovHalfH + 12);

            // Labels
            using (Font fLabel = new Font("Segoe UI", 8.5f))
            using (SolidBrush bDim = new SolidBrush(colDim))
            using (SolidBrush bSub = new SolidBrush(colSubject))
            {
                g.DrawString("Camera", fLabel, bDim, camX - 30, midY + 16);
                g.DrawString("Subject", fLabel, bSub, subX + 4, midY - 8);
            }

            // WD dimension arrow (bottom)
            using (Pen penWD = new Pen(colWD, 1.5f))
            using (SolidBrush bWD = new SolidBrush(colWD))
            using (Font fVal = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                int arrowY = SideMarginTop + drawH + 20;
                DrawDimArrow(g, penWD, bWD, fVal, camX, arrowY, subX,
                    "WD = " + _wd.Value.ToString("F1") + " mm", colWD);
            }

            // FOV Y annotation (right side)
            using (Pen penFovS = new Pen(colFov, 2f))
            using (SolidBrush bFov = new SolidBrush(colFov))
            using (Font fVal = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                DrawDimArrowV(g, penFovS, bFov, fVal, subX + 18,
                    midY - fovHalfH, midY + fovHalfH,
                    "FOV Y\n" + _fovY.Value.ToString("F1") + " mm", colFov, leftSide: false);
            }

            // FOV X label (top)
            using (Font fVal = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush bFov = new SolidBrush(colFov))
            {
                string fovXLabel = "FOV X = " + _fovX.Value.ToString("F1") + " mm";
                SizeF sz = g.MeasureString(fovXLabel, fVal);
                g.DrawString(fovXLabel, fVal, bFov,
                    (camX + subX) / 2f - sz.Width / 2f, SideMarginTop - 4);
            }
        }

        // ─────────────────────────────────────────────
        //  Front View Paint
        // ─────────────────────────────────────────────
        private void pnlFrontView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int W = pnlFrontView.Width;
            int H = pnlFrontView.Height;
            g.Clear(Color.FromArgb(16, 20, 28));

            if (_fovX == null)
            {
                DrawPlaceholder(g, W, H, "");
                return;
            }

            double fovX = _fovX.Value;
            double fovY = _fovY.Value;

            int margin = 60;
            int maxW = W - margin * 2;
            int maxH = H - margin * 2;

            double aspect = fovX / fovY;
            int rectW, rectH;
            if (aspect >= (double)maxW / maxH)
            {
                rectW = maxW;
                rectH = (int)(maxW / aspect);
            }
            else
            {
                rectH = maxH;
                rectW = (int)(maxH * aspect);
            }

            int rx = (W - rectW) / 2;
            int ry = (H - rectH) / 2;

            Color colBorder = Color.FromArgb(60, 200, 100);
            Color colDim = Color.FromArgb(80, 160, 255);

            using (SolidBrush bFill = new SolidBrush(Color.FromArgb(14, 60, 200, 100)))
                g.FillRectangle(bFill, rx, ry, rectW, rectH);
            using (Pen penBorder = new Pen(colBorder, 2f))
                g.DrawRectangle(penBorder, rx, ry, rectW, rectH);

            using (Pen penCross = new Pen(Color.FromArgb(50, 100, 130), 1f))
            {
                penCross.DashStyle = DashStyle.Dot;
                g.DrawLine(penCross, rx, ry + rectH / 2, rx + rectW, ry + rectH / 2);
                g.DrawLine(penCross, rx + rectW / 2, ry, rx + rectW / 2, ry + rectH);
            }

            using (Pen penDim = new Pen(colDim, 1.5f))
            using (SolidBrush bDim = new SolidBrush(colDim))
            using (Font fVal = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                DrawDimArrow(g, penDim, bDim, fVal, rx, ry + rectH + 26, rx + rectW,
                    "FOV X = " + fovX.ToString("F1") + " mm", colDim);
                DrawDimArrowV(g, penDim, bDim, fVal, rx - 32, ry, ry + rectH,
                    "FOV Y\n" + fovY.ToString("F1") + " mm", colDim, leftSide: true);
            }

            using (Font fCenter = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (SolidBrush bCenter = new SolidBrush(Color.FromArgb(180, 230, 180)))
            {
                string centerLabel = fovX.ToString("F1") + " × " + fovY.ToString("F1") + " mm";
                SizeF sz = g.MeasureString(centerLabel, fCenter);
                g.DrawString(centerLabel, fCenter, bCenter,
                    rx + rectW / 2f - sz.Width / 2f,
                    ry + rectH / 2f - sz.Height / 2f);
            }
        }

        // ─────────────────────────────────────────────
        //  Drawing helpers
        // ─────────────────────────────────────────────
        private static void DrawDimArrow(Graphics g, Pen pen, SolidBrush brush, Font font,
            int x1, int y, int x2, string label, Color textColor)
        {
            g.DrawLine(pen, x1, y - 5, x1, y + 5);
            g.DrawLine(pen, x2, y - 5, x2, y + 5);
            g.DrawLine(pen, x1, y, x2, y);
            DrawArrowHead(g, brush, x1, y, x2, y);
            DrawArrowHead(g, brush, x2, y, x1, y);

            using (SolidBrush b = new SolidBrush(textColor))
            {
                SizeF sz = g.MeasureString(label, font);
                g.DrawString(label, font, b, (x1 + x2) / 2f - sz.Width / 2f, y - sz.Height - 3);
            }
        }

        private static void DrawDimArrowV(Graphics g, Pen pen, SolidBrush brush, Font font,
            int x, int y1, int y2, string label, Color textColor, bool leftSide)
        {
            g.DrawLine(pen, x - 5, y1, x + 5, y1);
            g.DrawLine(pen, x - 5, y2, x + 5, y2);
            g.DrawLine(pen, x, y1, x, y2);
            DrawArrowHead(g, brush, x, y1, x, y2);
            DrawArrowHead(g, brush, x, y2, x, y1);

            using (SolidBrush b = new SolidBrush(textColor))
            {
                SizeF sz = g.MeasureString(label, font);
                float tx = leftSide ? x - sz.Width - 6 : x + 6;
                float ty = (y1 + y2) / 2f - sz.Height / 2f;
                g.DrawString(label, font, b, tx, ty);
            }
        }

        private static void DrawArrowHead(Graphics g, SolidBrush brush, int fromX, int fromY, int toX, int toY)
        {
            float dx = toX - fromX;
            float dy = toY - fromY;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 1) return;
            dx /= len; dy /= len;
            int size = 6;
            g.FillPolygon(brush, new PointF[]
            {
                new PointF(fromX + dx * size, fromY + dy * size),
                new PointF(fromX - dy * size * 0.45f, fromY + dx * size * 0.45f),
                new PointF(fromX + dy * size * 0.45f, fromY - dx * size * 0.45f),
            });
        }

        private static void DrawPlaceholder(Graphics g, int W, int H, string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            using (Font f = new Font("Segoe UI", 10f))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(60, 80, 100)))
            {
                SizeF sz = g.MeasureString(msg, f);
                g.DrawString(msg, f, b, W / 2f - sz.Width / 2f, H / 2f - sz.Height / 2f);
            }
        }

        // ─────────────────────────────────────────────
        //  Theme
        // ─────────────────────────────────────────────
        private void ApplyTheme()
        {
            Color dark = Color.FromArgb(12, 14, 18);
            Color panel = Color.FromArgb(22, 28, 38);
            Color border = Color.FromArgb(40, 55, 80);
            Color fg = Color.FromArgb(200, 210, 220);
            Color accent = Color.FromArgb(0, 220, 180);
            Color btnBg = Color.FromArgb(25, 42, 75);
            Color inputBg = Color.FromArgb(18, 22, 32);

            BackColor = dark;
            splitMain.BackColor = dark;
            splitMain.Panel1.BackColor = dark;
            splitMain.Panel2.BackColor = dark;
            splitMain.SplitterWidth = 4;

            grpFov.BackColor = panel;
            grpFov.ForeColor = accent;

            foreach (Control c in grpFov.Controls)
            {
                if (c is Label lbl) { lbl.ForeColor = fg; lbl.BackColor = panel; }
                else if (c is TextBox txt) { txt.BackColor = inputBg; txt.ForeColor = Color.White; txt.BorderStyle = BorderStyle.FixedSingle; }
                else if (c is Button btn) { btn.BackColor = btnBg; btn.ForeColor = accent; btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderColor = border; }
            }

            lblFovXResult.ForeColor = accent;
            lblFovXResult.BackColor = panel;
            lblFovYResult.ForeColor = accent;
            lblFovYResult.BackColor = panel;

            pnlDiagrams.BackColor = dark;
            pnlSideView.BackColor = Color.FromArgb(16, 20, 28);
            pnlFrontView.BackColor = Color.FromArgb(16, 20, 28);
            lblSideTitle.ForeColor = Color.FromArgb(100, 140, 180);
            lblSideTitle.BackColor = dark;
            lblFrontTitle.ForeColor = Color.FromArgb(100, 140, 180);
            lblFrontTitle.BackColor = dark;
        }
    }
}
