using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class CameraCalcControl : UserControl, IActivatable
    {
        // ── Camera catalog
        private struct CameraSpec
        {
            public string Name;
            public double SensorX, SensorY; // mm
            public double PixelUm;           // μm
            public int PixelW, PixelH;
        }

        // ── JSON 직렬화 DTO (CameraEditorForm에서도 접근)
        public class CameraEntry
        {
            public string name    { get; set; }
            public double sensorX { get; set; }
            public double sensorY { get; set; }
            public double pixelUm { get; set; }
            public int    pixelW  { get; set; }
            public int    pixelH  { get; set; }
        }

        // ── 파일 경로 (CameraEditorForm에서도 접근)
        public static string RuntimeJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras.json");
        // bin\Debug\ 기준 두 단계 위 → 프로젝트 소스 Config (Debug 전용)
        public static string SourceConfigJsonPath => Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Config\cameras.json"));
        // 빌드 출력 Config (앱 실행 시 복사 원본)
        public static string ConfigJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "cameras.json");

        // ── 최소 내장 폴백 (JSON 파일이 아예 없을 때만 사용)
        private static readonly CameraSpec[] _builtinCameras =
        {
            new CameraSpec { Name="Basler a2A1920-51gm", SensorX=5.31, SensorY=3.33, PixelUm=2.74, PixelW=1936, PixelH=1216 },
            new CameraSpec { Name="Basler a2A2040-35gm", SensorX=5.66, SensorY=4.23, PixelUm=2.74, PixelW=2064, PixelH=1544 },
            new CameraSpec { Name="Basler a2A2590-22gm", SensorX=8.45, SensorY=7.07, PixelUm=3.45, PixelW=2448, PixelH=2048 },
            new CameraSpec { Name="Basler acA1300-60gm", SensorX=4.86, SensorY=3.62, PixelUm=3.75, PixelW=1296, PixelH=966  },
            new CameraSpec { Name="Basler acA2500-14gm", SensorX=8.45, SensorY=7.07, PixelUm=3.45, PixelW=2592, PixelH=1944 },
        };

        private static readonly double[] _standardFLs = { 6, 8, 12, 16, 25, 35, 50, 75, 100 };

        // ── 런타임 카탈로그 (내장 + JSON 병합)
        private List<CameraSpec> _cameras = new List<CameraSpec>(_builtinCameras);

        // ── Forward calc state
        private double? _wd, _fl, _sensorX, _sensorY, _fovX, _fovY;

        // ── Reverse calc state
        private double? _revWd, _revFovX, _revFovY;

        // ── Side-view layout constants
        private const int SideMarginL   = 60;
        private const int SideMarginR   = 80;
        private const int SideMarginTop = 35;
        private const int SideMarginBot = 52;

        // ── Drag state
        private double _pixelsPerMm = 0;
        private bool _isDragging = false;
        private int _dragOffsetX = 0;
        private bool _suppressInputEvent = false;

        public CameraCalcControl()
        {
            InitializeComponent();
            ApplyTheme();
            LoadCatalog();
        }

        public void OnActivated()
        {
            if (tabMain.SelectedIndex == 1)
                Calculate(silent: true);
        }

        // ─────────────────────────────────────────────
        //  카탈로그 로드 / JSON 관리
        // ─────────────────────────────────────────────
        private void LoadCatalog()
        {
            // Config\cameras.json → cameras.json 복사 (런타임 파일이 없을 때만)
            if (!File.Exists(RuntimeJsonPath) && File.Exists(ConfigJsonPath))
                try { File.Copy(ConfigJsonPath, RuntimeJsonPath); }
                catch { /* 무시 */ }

            string loadPath = File.Exists(RuntimeJsonPath) ? RuntimeJsonPath
                            : File.Exists(ConfigJsonPath)  ? ConfigJsonPath
                            : null;

            _cameras = new List<CameraSpec>(_builtinCameras);

            if (loadPath != null)
            {
                try
                {
                    var list = JsonConvert.DeserializeObject<List<CameraEntry>>(File.ReadAllText(loadPath));
                    if (list != null)
                    {
                        _cameras.Clear();
                        foreach (var c in list)
                            _cameras.Add(new CameraSpec
                            {
                                Name = c.name, SensorX = c.sensorX, SensorY = c.sensorY,
                                PixelUm = c.pixelUm, PixelW = c.pixelW, PixelH = c.pixelH,
                            });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("cameras.json 로드 실패:\n" + ex.Message,
                        "카탈로그 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            UpdateCatalogLabel();
        }

        private void UpdateCatalogLabel()
        {
            lblCatalogInfo.Text = $"카탈로그: {_cameras.Count}개";
        }

        private void btnCatalogReload_Click(object sender, EventArgs e)
        {
            LoadCatalog();
            dgvResults.Rows.Clear();
            ClearDiagram();
        }

        private void btnCatalogEdit_Click(object sender, EventArgs e)
        {
            var entries = new List<CameraEntry>();
            foreach (var c in _cameras)
                entries.Add(new CameraEntry
                {
                    name = c.Name, sensorX = c.SensorX, sensorY = c.SensorY,
                    pixelUm = c.PixelUm, pixelW = c.PixelW, pixelH = c.PixelH,
                });

            using (var dlg = new CameraEditorForm(entries))
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    LoadCatalog();
                    dgvResults.Rows.Clear();
                    ClearDiagram();
                }
            }
        }

        // ─────────────────────────────────────────────
        //  Tab switch
        // ─────────────────────────────────────────────
        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedIndex == 1)
                Calculate(silent: true);
            else
                RestoreReverseSelection();
        }

        // ─────────────────────────────────────────────
        //  Reverse calculation
        // ─────────────────────────────────────────────
        private void btnRevCalc_Click(object sender, EventArgs e) => ReverseCalculate();

        private void ReverseCalculate()
        {
            if (!double.TryParse(txtRevWD.Text.Trim(),      out double wd)     || wd      <= 0) { ShowRevError("WD를 확인하세요.");          return; }
            if (!double.TryParse(txtRevFovX.Text.Trim(),    out double targetX) || targetX <= 0) { ShowRevError("검사 영역 가로를 확인하세요."); return; }
            if (!double.TryParse(txtRevFovY.Text.Trim(),    out double targetY) || targetY <= 0) { ShowRevError("검사 영역 세로를 확인하세요."); return; }
            if (!double.TryParse(txtRevMinFeat.Text.Trim(), out double minFeat) || minFeat <= 0) { ShowRevError("최소 검출 크기를 확인하세요.");  return; }

            _revWd = wd; _revFovX = targetX; _revFovY = targetY;

            dgvResults.Rows.Clear();

            foreach (var cam in _cameras)
            {
                foreach (double fl in _standardFLs)
                {
                    double actualFovX = wd * cam.SensorX / fl;
                    double actualFovY = wd * cam.SensorY / fl;
                    if (actualFovX < targetX || actualFovY < targetY) continue;

                    double resMm  = actualFovX / cam.PixelW;
                    bool resFits  = resMm <= minFeat;

                    int rowIdx = dgvResults.Rows.Add(
                        cam.Name,
                        fl.ToString("F0") + " mm",
                        actualFovX.ToString("F1") + " mm",
                        actualFovY.ToString("F1") + " mm",
                        resMm.ToString("F3") + " mm/px",
                        resFits ? "✓" : "✗"
                    );

                    var row = dgvResults.Rows[rowIdx];
                    row.DefaultCellStyle.ForeColor = resFits
                        ? Color.FromArgb(80, 220, 130)
                        : Color.FromArgb(200, 100, 80);
                    row.Tag = (cam, fl, actualFovX, actualFovY, resMm);
                }
            }

            if (dgvResults.Rows.Count == 0)
            {
                ShowRevError("조건을 만족하는 조합이 없습니다.\nWD를 늘리거나 검사 영역을 줄여보세요.");
                ClearDiagram();
            }
        }

        private void dgvResults_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0) return;
            var row = dgvResults.SelectedRows[0];
            if (row.Tag == null) return;

            var (cam, fl, fovX, fovY, _) = ((CameraSpec cam, double fl, double fovX, double fovY, double res))row.Tag;

            _wd = _revWd;
            _fl = fl;
            _sensorX = cam.SensorX;
            _sensorY = cam.SensorY;
            _fovX = fovX;
            _fovY = fovY;

            RebuildPixelsPerMm();
            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        private void RestoreReverseSelection()
        {
            if (dgvResults.SelectedRows.Count > 0 && dgvResults.SelectedRows[0].Tag != null)
                dgvResults_SelectionChanged(this, EventArgs.Empty);
            else
                ClearDiagram();
        }

        private void ClearDiagram()
        {
            _fovX = null; _fovY = null;
            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        private static void ShowRevError(string msg) =>
            MessageBox.Show(msg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // ─────────────────────────────────────────────
        //  Forward calculation
        // ─────────────────────────────────────────────
        private void btnCalcFov_Click(object sender, EventArgs e) => Calculate();

        private void InputChanged(object sender, EventArgs e)
        {
            if (_suppressInputEvent) return;
            Calculate(silent: true);
        }

        private void Calculate(bool silent = false)
        {
            if (!double.TryParse(txtFovWD.Text.Trim(),      out double wd) ||
                !double.TryParse(txtFovFL.Text.Trim(),      out double fl) ||
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

            if (!_isDragging) RebuildPixelsPerMm();

            lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
            lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";

            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        // ─────────────────────────────────────────────
        //  Layout helpers
        // ─────────────────────────────────────────────
        private void RebuildPixelsPerMm()
        {
            if (_wd == null || _wd.Value <= 0) return;
            int W     = Math.Max(pnlSideView.Width, 200);
            int drawW = W - SideMarginL - SideMarginR;
            int subX  = SideMarginL + (int)(drawW * 0.90);
            _pixelsPerMm = (subX - (SideMarginL + 10)) * 0.75 / _wd.Value;
        }

        private void GetSideLayout(out int midY, out int subX, out int camX)
        {
            int W     = Math.Max(pnlSideView.Width, 200);
            int H     = Math.Max(pnlSideView.Height, 100);
            int drawW = W - SideMarginL - SideMarginR;
            int drawH = H - SideMarginTop - SideMarginBot;
            midY = SideMarginTop + drawH / 2;
            subX = SideMarginL + (int)(drawW * 0.90);

            if (_pixelsPerMm > 0 && _wd != null)
            {
                camX = subX - (int)(_wd.Value * _pixelsPerMm);
                camX = Math.Max(SideMarginL + 10, Math.Min(camX, subX - 30));
            }
            else
            {
                camX = SideMarginL + (int)(drawW * 0.08);
            }
        }

        private Rectangle GetCameraHitRect(int camX, int midY)
            => new Rectangle(camX - 38, midY - 22, 54, 44);

        // ─────────────────────────────────────────────
        //  Side View — mouse drag (양쪽 탭 모두 가능)
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
                pnlSideView.Cursor = GetCameraHitRect(camX, midY).Contains(e.Location)
                    ? Cursors.SizeWE : Cursors.Default;
                return;
            }

            int newCamX = Math.Max(SideMarginL + 10, Math.Min(e.X - _dragOffsetX, subX - 30));
            double newWD = Math.Max(10.0, Math.Round((subX - newCamX) / _pixelsPerMm / 10.0) * 10.0);

            _wd   = newWD;
            _fovX = _wd.Value * _sensorX.Value / _fl.Value;
            _fovY = _wd.Value * _sensorY.Value / _fl.Value;

            _suppressInputEvent = true;
            if (tabMain.SelectedIndex == 0)
            {
                // 역방향 탭: revWd 업데이트
                _revWd = newWD;
                txtRevWD.Text = newWD.ToString("F1");
            }
            else
            {
                // 순방향 탭: txtFovWD + 결과 레이블
                txtFovWD.Text = newWD.ToString("F1");
                lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
                lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";
            }
            _suppressInputEvent = false;

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

            int W = pnlSideView.Width, H = pnlSideView.Height;
            g.Clear(Color.FromArgb(16, 20, 28));

            if (_fovX == null)
            {
                DrawPlaceholder(g, W, H,
                    tabMain.SelectedIndex == 0
                        ? "조합 목록에서 행을 선택하면 다이어그램이 표시됩니다."
                        : "값을 입력하고 Calculate를 누르세요.");
                return;
            }

            int drawH = H - SideMarginTop - SideMarginBot;
            GetSideLayout(out int midY, out int subX, out int camX);
            int fovHalfH = Math.Max(20, Math.Min((int)(drawH * 0.36), drawH / 2 - 18));

            Color colFov     = Color.FromArgb(60, 200, 100);
            Color colWD      = Color.FromArgb(80, 160, 255);
            Color colSubject = Color.FromArgb(200, 180, 80);
            Color colCamera  = _isDragging ? Color.FromArgb(220, 230, 255) : Color.FromArgb(150, 180, 220);
            Color colDim     = Color.FromArgb(120, 130, 145);

            using (SolidBrush b = new SolidBrush(Color.FromArgb(18, 60, 200, 100)))
                g.FillPolygon(b, new[] { new Point(camX, midY), new Point(subX, midY - fovHalfH), new Point(subX, midY + fovHalfH) });

            using (Pen p = new Pen(colFov, 1.5f) { DashStyle = DashStyle.Dash })
            { g.DrawLine(p, camX, midY, subX, midY - fovHalfH); g.DrawLine(p, camX, midY, subX, midY + fovHalfH); }

            using (Pen p = new Pen(Color.FromArgb(60, 100, 140), 1f) { DashStyle = DashStyle.Dot })
                g.DrawLine(p, camX, midY, subX + 12, midY);

            Rectangle camRect = new Rectangle(camX - 30, midY - 12, 30, 24);
            using (SolidBrush b = new SolidBrush(_isDragging ? Color.FromArgb(55, 72, 100) : Color.FromArgb(40, 55, 75)))
                g.FillRectangle(b, camRect);
            using (Pen p = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawRectangle(p, camRect);
            using (Pen p = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawEllipse(p, camX - 8, midY - 8, 16, 16);

            if (_isDragging)
            {
                using (Font f = new Font("Segoe UI", 7.5f))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(180, 200, 255)))
                {
                    SizeF sz = g.MeasureString("↔", f);
                    g.DrawString("↔", f, b, camX - 15 - sz.Width / 2f, midY - 27);
                }
            }

            using (Pen p = new Pen(colSubject, 2.5f))
                g.DrawLine(p, subX, midY - fovHalfH - 12, subX, midY + fovHalfH + 12);

            using (Font f = new Font("Segoe UI", 8.5f))
            using (SolidBrush bDim = new SolidBrush(colDim))
            using (SolidBrush bSub = new SolidBrush(colSubject))
            { g.DrawString("Camera", f, bDim, camX - 30, midY + 16); g.DrawString("Subject", f, bSub, subX + 4, midY - 8); }

            using (Pen p = new Pen(colWD, 1.5f))
            using (SolidBrush b = new SolidBrush(colWD))
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                DrawDimArrow(g, p, b, f, camX, SideMarginTop + drawH + 20, subX, "WD = " + _wd.Value.ToString("F1") + " mm", colWD);

            using (Pen p = new Pen(colFov, 2f))
            using (SolidBrush b = new SolidBrush(colFov))
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                DrawDimArrowV(g, p, b, f, subX + 18, midY - fovHalfH, midY + fovHalfH, "FOV Y\n" + _fovY.Value.ToString("F1") + " mm", colFov, leftSide: false);

            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(colFov))
            {
                string lbl = "FOV X = " + _fovX.Value.ToString("F1") + " mm";
                SizeF sz = g.MeasureString(lbl, f);
                g.DrawString(lbl, f, b, (camX + subX) / 2f - sz.Width / 2f, SideMarginTop - 4);
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

            int W = pnlFrontView.Width, H = pnlFrontView.Height;
            g.Clear(Color.FromArgb(16, 20, 28));

            if (_fovX == null) { DrawPlaceholder(g, W, H, ""); return; }

            double fovX = _fovX.Value, fovY = _fovY.Value;
            int margin = 60, maxW = W - margin * 2, maxH = H - margin * 2;
            double aspect = fovX / fovY;
            int rectW, rectH;
            if (aspect >= (double)maxW / maxH) { rectW = maxW; rectH = (int)(maxW / aspect); }
            else                               { rectH = maxH; rectW = (int)(maxH * aspect); }
            int rx = (W - rectW) / 2, ry = (H - rectH) / 2;

            Color colBorder = Color.FromArgb(60, 200, 100);
            Color colDim    = Color.FromArgb(80, 160, 255);

            using (SolidBrush b = new SolidBrush(Color.FromArgb(14, 60, 200, 100)))
                g.FillRectangle(b, rx, ry, rectW, rectH);
            using (Pen p = new Pen(colBorder, 2f))
                g.DrawRectangle(p, rx, ry, rectW, rectH);
            using (Pen p = new Pen(Color.FromArgb(50, 100, 130), 1f) { DashStyle = DashStyle.Dot })
            { g.DrawLine(p, rx, ry + rectH / 2, rx + rectW, ry + rectH / 2); g.DrawLine(p, rx + rectW / 2, ry, rx + rectW / 2, ry + rectH); }

            // 역방향 탭: 필요 FOV 점선 오버레이
            if (tabMain.SelectedIndex == 0 && _revFovX != null && _revFovY != null)
            {
                int reqW = (int)(_revFovX.Value * rectW / fovX);
                int reqH = (int)(_revFovY.Value * rectH / fovY);
                int reqX = rx + (rectW - reqW) / 2, reqY = ry + (rectH - reqH) / 2;
                using (Pen p = new Pen(Color.FromArgb(200, 180, 80), 1.5f) { DashStyle = DashStyle.Dash })
                    g.DrawRectangle(p, reqX, reqY, reqW, reqH);
                using (SolidBrush b = new SolidBrush(Color.FromArgb(160, 170, 60)))
                using (Font f = new Font("Segoe UI", 7.5f))
                    g.DrawString("필요 FOV", f, b, reqX + 4, reqY + 4);
            }

            using (Pen p = new Pen(colDim, 1.5f))
            using (SolidBrush b = new SolidBrush(colDim))
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                DrawDimArrow(g, p, b, f, rx, ry + rectH + 26, rx + rectW, "FOV X = " + fovX.ToString("F1") + " mm", colDim);
                DrawDimArrowV(g, p, b, f, rx - 32, ry, ry + rectH, "FOV Y\n" + fovY.ToString("F1") + " mm", colDim, leftSide: true);
            }

            using (Font f = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(180, 230, 180)))
            {
                string lbl = fovX.ToString("F1") + " × " + fovY.ToString("F1") + " mm";
                SizeF sz = g.MeasureString(lbl, f);
                g.DrawString(lbl, f, b, rx + rectW / 2f - sz.Width / 2f, ry + rectH / 2f - sz.Height / 2f);
            }
        }

        // ─────────────────────────────────────────────
        //  Drawing helpers
        // ─────────────────────────────────────────────
        private static void DrawDimArrow(Graphics g, Pen pen, SolidBrush brush, Font font,
            int x1, int y, int x2, string label, Color textColor)
        {
            g.DrawLine(pen, x1, y - 5, x1, y + 5); g.DrawLine(pen, x2, y - 5, x2, y + 5);
            g.DrawLine(pen, x1, y, x2, y);
            DrawArrowHead(g, brush, x1, y, x2, y); DrawArrowHead(g, brush, x2, y, x1, y);
            using (SolidBrush b = new SolidBrush(textColor))
            {
                SizeF sz = g.MeasureString(label, font);
                g.DrawString(label, font, b, (x1 + x2) / 2f - sz.Width / 2f, y - sz.Height - 3);
            }
        }

        private static void DrawDimArrowV(Graphics g, Pen pen, SolidBrush brush, Font font,
            int x, int y1, int y2, string label, Color textColor, bool leftSide)
        {
            g.DrawLine(pen, x - 5, y1, x + 5, y1); g.DrawLine(pen, x - 5, y2, x + 5, y2);
            g.DrawLine(pen, x, y1, x, y2);
            DrawArrowHead(g, brush, x, y1, x, y2); DrawArrowHead(g, brush, x, y2, x, y1);
            using (SolidBrush b = new SolidBrush(textColor))
            {
                SizeF sz = g.MeasureString(label, font);
                g.DrawString(label, font, b, leftSide ? x - sz.Width - 6 : x + 6, (y1 + y2) / 2f - sz.Height / 2f);
            }
        }

        private static void DrawArrowHead(Graphics g, SolidBrush brush, int fromX, int fromY, int toX, int toY)
        {
            float dx = toX - fromX, dy = toY - fromY;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 1) return;
            dx /= len; dy /= len;
            g.FillPolygon(brush, new PointF[]
            {
                new PointF(fromX + dx * 6,           fromY + dy * 6),
                new PointF(fromX - dy * 6 * 0.45f,   fromY + dx * 6 * 0.45f),
                new PointF(fromX + dy * 6 * 0.45f,   fromY - dx * 6 * 0.45f),
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
            Color dark     = Color.FromArgb(12, 14, 18);
            Color panel    = Color.FromArgb(22, 28, 38);
            Color border   = Color.FromArgb(40, 55, 80);
            Color fg       = Color.FromArgb(200, 210, 220);
            Color accent   = Color.FromArgb(0, 220, 180);
            Color btnBg    = Color.FromArgb(25, 42, 75);
            Color inputBg  = Color.FromArgb(18, 22, 32);
            Color headerBg = Color.FromArgb(30, 40, 60);

            BackColor = dark;
            splitMain.BackColor = dark;
            splitMain.Panel1.BackColor = dark;
            splitMain.Panel2.BackColor = dark;
            splitMain.SplitterWidth = 4;

            tabMain.BackColor = dark;
            tabReverse.BackColor = dark;
            tabForward.BackColor = dark;

            // Catalog bar
            pnlCatalogBar.BackColor = Color.FromArgb(18, 24, 36);
            lblCatalogInfo.ForeColor = Color.FromArgb(100, 130, 160);
            lblCatalogInfo.BackColor = Color.Transparent;

            StyleButton(btnCatalogEdit,   btnBg, accent, border);
            StyleButton(btnCatalogReload, btnBg, accent, border);

            // Reverse input group
            grpRevInput.BackColor = panel;
            grpRevInput.ForeColor = accent;
            foreach (Control c in grpRevInput.Controls)
            {
                if (c is Label lbl)   { lbl.ForeColor = fg;     lbl.BackColor = panel; }
                if (c is TextBox txt) { txt.BackColor = inputBg; txt.ForeColor = Color.White; txt.BorderStyle = BorderStyle.FixedSingle; }
                if (c is Button btn)  StyleButton(btn, btnBg, accent, border);
            }

            // DataGridView
            dgvResults.BackgroundColor = inputBg;
            dgvResults.GridColor       = border;
            dgvResults.BorderStyle     = BorderStyle.None;
            dgvResults.EnableHeadersVisualStyles = false;
            dgvResults.DefaultCellStyle.BackColor          = inputBg;
            dgvResults.DefaultCellStyle.ForeColor          = fg;
            dgvResults.DefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 55, 90);
            dgvResults.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = headerBg;
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = accent;
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            dgvResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Forward group
            grpFov.BackColor = panel;
            grpFov.ForeColor = accent;
            foreach (Control c in grpFov.Controls)
            {
                if (c is Label lbl)   { lbl.ForeColor = fg;     lbl.BackColor = panel; }
                if (c is TextBox txt) { txt.BackColor = inputBg; txt.ForeColor = Color.White; txt.BorderStyle = BorderStyle.FixedSingle; }
                if (c is Button btn)  StyleButton(btn, btnBg, accent, border);
            }
            lblFovXResult.ForeColor = accent; lblFovXResult.BackColor = panel;
            lblFovYResult.ForeColor = accent; lblFovYResult.BackColor = panel;

            // Diagram
            pnlDiagrams.BackColor  = dark;
            pnlSideView.BackColor  = Color.FromArgb(16, 20, 28);
            pnlFrontView.BackColor = Color.FromArgb(16, 20, 28);
            lblSideTitle.ForeColor  = Color.FromArgb(100, 140, 180); lblSideTitle.BackColor  = dark;
            lblFrontTitle.ForeColor = Color.FromArgb(100, 140, 180); lblFrontTitle.BackColor = dark;
        }

        private static void StyleButton(Button btn, Color bg, Color fg, Color border)
        {
            btn.BackColor = bg; btn.ForeColor = fg;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = border;
        }
    }
}
