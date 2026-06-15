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

        // ── Lens catalog
        private struct LensSpec
        {
            public string Name;
            public string Manufacturer;
            public double FocalLength;    // mm (0 if fixed-magnification)
            public double Magnification;  // >0 for fixed-mag macro lenses (e.g. 0.2 = 0.2X)
            public double MinWD;          // mm
            public double ImageCircle;    // mm (직경)
            public bool IsFixedMag => Magnification > 0;
        }

        // ── Camera JSON DTO (CameraEditorForm에서도 접근)
        public class CameraEntry
        {
            public string name    { get; set; }
            public double sensorX { get; set; }
            public double sensorY { get; set; }
            public double pixelUm { get; set; }
            public int    pixelW  { get; set; }
            public int    pixelH  { get; set; }
        }

        // ── Lens JSON DTO (LensEditorForm에서도 접근)
        public class LensEntry
        {
            public string  name          { get; set; }
            public string  manufacturer  { get; set; }
            public double  focalLength   { get; set; }
            public double? magnification { get; set; }  // fixed-mag macro lenses only
            public double  minWD         { get; set; }
            public double  imageCircle   { get; set; }
        }

        // ── Camera 파일 경로
        public static string RuntimeJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cameras.json");
        public static string SourceConfigJsonPath => Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Config\cameras.json"));
        public static string ConfigJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "cameras.json");

        // ── Lens 파일 경로
        public static string RuntimeLensJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lenses.json");
        public static string SourceLensConfigJsonPath => Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Config\lenses.json"));
        public static string LensConfigJsonPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "lenses.json");

        // ── 내장 폴백 카메라 (JSON 없을 때만)
        private static readonly CameraSpec[] _builtinCameras =
        {
            new CameraSpec { Name="Basler a2A1920-51gm", SensorX=5.31, SensorY=3.33, PixelUm=2.74, PixelW=1936, PixelH=1216 },
            new CameraSpec { Name="Basler a2A2040-35gm", SensorX=5.66, SensorY=4.23, PixelUm=2.74, PixelW=2064, PixelH=1544 },
            new CameraSpec { Name="Basler a2A2590-22gm", SensorX=8.45, SensorY=7.07, PixelUm=3.45, PixelW=2448, PixelH=2048 },
            new CameraSpec { Name="Basler acA1300-60gm", SensorX=4.86, SensorY=3.62, PixelUm=3.75, PixelW=1296, PixelH=966  },
            new CameraSpec { Name="Basler acA2500-14gm", SensorX=8.45, SensorY=7.07, PixelUm=3.45, PixelW=2592, PixelH=1944 },
        };

        // ── 런타임 카탈로그
        private List<CameraSpec> _cameras = new List<CameraSpec>(_builtinCameras);
        private List<LensSpec>   _lenses  = new List<LensSpec>();

        // ── Forward calc state
        private double? _wd, _fl, _mag, _sensorX, _sensorY, _fovX, _fovY;

        // ── Reverse calc state
        private double? _revWd, _revFovX, _revFovY, _revMinFeat;

        // ── Search filters
        private bool   _minPixFilterEnabled = false;
        private int    _minPixCount         = 10;
        private string _camKeyword          = "";
        private string _lensKeyword         = "";
        private Form   _searchingPopup      = null;

        // ── Subject size (mm)
        private double _subjectMmX = 800.0;
        private double _subjectMmY = 800.0;

        // ── Obstacle / occlusion state
        private bool   _obstEnabled     = true;
        private double _obstDistFromCam = 250.0; // mm from camera along WD axis
        private double _obstHeightMm    = 100.0; // obstacle center height from floor (mm) — floor: center == halfH
        private double _obstHalfH       = 100.0; // vertical half-size (mm)
        private double _obstHalfW       = 100.0; // horizontal half-size (mm)
        private double _obstOffsetX     = 0;    // horizontal offset in front view (mm)
        private bool   _isDraggingObst  = false;
        private int    _obstDragOffX    = 0;
        private int    _obstDragOffY    = 0;
        private bool   _isDraggingObstFront = false;
        private int    _obstFrontDragOffX   = 0;
        private int    _obstFrontDragOffY   = 0;
        private bool   _isResizingObstFront = false;
        private double _obstResizeBaseW     = 0;
        private double _obstResizeBaseH     = 0;
        private int    _obstResizeStartX    = 0;
        private int    _obstResizeStartY    = 0;
        private double _obstResizeFixedLeft = 0;  // 리사이즈 중 고정될 좌측 mm (camera-relative)
        private double _obstResizeFixedTop  = 0;  // 리사이즈 중 고정될 상단 mm (from floor)

        // ── Side-view layout constants
        private const int SideMarginL   = 60;
        private const int SideMarginR   = 80;
        private const int SideMarginTop = 35;
        private const int SideMarginBot = 52;

        // ── Drag state (Side View)
        private double _pixelsPerMm    = 0;
        private int    _baseW          = 0;         // panel W when _pixelsPerMm was last set
        private bool   _isDragging     = false;
        private int    _dragOffsetX    = 0;
        private int    _dragOffsetY    = 0;
        private double _cameraHeightMm  = 400.0;   // 0–800 mm
        private double _cameraOffsetX   = 0.0;     // mm, horizontal offset from subject center

        // ── Drag state (Front View)
        private bool _isDraggingFront  = false;
        private int  _frontDragOffsetX = 0;
        private int  _frontDragOffsetY = 0;

        private bool _suppressInputEvent = false;

        private static void SetDoubleBuffered(Control c)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(c, true);
        }

        // Dirty rect covering camera body + height guide column in Side View
        private Rectangle SideCamDirtyRect(int camX, int camY)
        {
            int floorY = pnlSideView.Height - SideMarginBot;
            return Rectangle.FromLTRB(camX - 68, camY - 36, camX + 34, floorY + 12);
        }

        // Dirty rect covering camera + FOV rect in Front View
        private Rectangle FrontCamDirtyRect(int cx, int cy, int fovPxW, int fovPxH)
        {
            int W = pnlFrontView.Width, H = pnlFrontView.Height;
            int floorY = H - 50;
            int l = Math.Min(cx - fovPxW / 2, cx - 28) - 10;
            int t = Math.Min(cy - fovPxH / 2, cy - 32) - 10;
            int r = Math.Max(cx + fovPxW / 2, cx + 28) + 10;
            int b = floorY + 12;
            return Rectangle.FromLTRB(
                Math.Max(0, l), Math.Max(0, t),
                Math.Min(W, r), Math.Min(H, b));
        }

        public CameraCalcControl()
        {
            InitializeComponent();
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.ScrollBars = ScrollBars.Vertical;
            SetDoubleBuffered(pnlSideView);
            SetDoubleBuffered(pnlFrontView);
            splitDiagram.SplitterMoved += (s, e) => { pnlSideView.Invalidate(); pnlFrontView.Invalidate(); };
            this.Load += (s, e) =>
            {
                if (splitDiagram.Height > 20)
                    splitDiagram.SplitterDistance = splitDiagram.Height / 2;
            };
            ApplyTheme();
            LoadCatalog();
            ReverseCalculate(silent: true);
            InitObstacleEvents();
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
            // Config 폴더 버전이 더 최신이면 bin root 파일을 덮어씀
            if (File.Exists(ConfigJsonPath))
            {
                if (!File.Exists(RuntimeJsonPath) ||
                    File.GetLastWriteTime(ConfigJsonPath) > File.GetLastWriteTime(RuntimeJsonPath))
                    try { File.Copy(ConfigJsonPath, RuntimeJsonPath, true); } catch { }
            }

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

            LoadLenses();
            PopulateForwardCombos();
            UpdateCatalogLabel();
        }

        private void LoadLenses()
        {
            // Config 폴더 버전이 더 최신이면 bin root 파일을 덮어씀
            if (File.Exists(LensConfigJsonPath))
            {
                if (!File.Exists(RuntimeLensJsonPath) ||
                    File.GetLastWriteTime(LensConfigJsonPath) > File.GetLastWriteTime(RuntimeLensJsonPath))
                    try { File.Copy(LensConfigJsonPath, RuntimeLensJsonPath, true); } catch { }
            }

            string loadPath = File.Exists(RuntimeLensJsonPath) ? RuntimeLensJsonPath
                            : File.Exists(LensConfigJsonPath)  ? LensConfigJsonPath
                            : null;

            _lenses = new List<LensSpec>();

            if (loadPath == null) return;

            try
            {
                var list = JsonConvert.DeserializeObject<List<LensEntry>>(File.ReadAllText(loadPath));
                if (list != null)
                {
                    foreach (var l in list)
                        _lenses.Add(new LensSpec
                        {
                            Name = l.name, Manufacturer = l.manufacturer ?? "",
                            FocalLength = l.focalLength,
                            Magnification = l.magnification ?? 0,
                            MinWD = l.minWD, ImageCircle = l.imageCircle,
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("lenses.json 로드 실패:\n" + ex.Message,
                    "렌즈 카탈로그 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PopulateForwardCombos()
        {
            cmbFovCamera.BeginUpdate();
            int prevCam = cmbFovCamera.SelectedIndex;
            cmbFovCamera.Items.Clear();
            foreach (var c in _cameras)
                cmbFovCamera.Items.Add(c.Name);
            if (prevCam >= 0 && prevCam < cmbFovCamera.Items.Count)
                cmbFovCamera.SelectedIndex = prevCam;
            cmbFovCamera.EndUpdate();

            cmbFovLens.BeginUpdate();
            int prevLens = cmbFovLens.SelectedIndex;
            cmbFovLens.Items.Clear();
            foreach (var l in _lenses)
                cmbFovLens.Items.Add(l.IsFixedMag
                    ? $"{l.Name}  ({l.Magnification:F3}X)"
                    : $"{l.Name}  ({l.FocalLength:F0}mm)");
            if (prevLens >= 0 && prevLens < cmbFovLens.Items.Count)
                cmbFovLens.SelectedIndex = prevLens;
            cmbFovLens.EndUpdate();
        }

        private void UpdateCatalogLabel()
        {
            lblCatalogInfo.Text     = $"카메라: {_cameras.Count}개";
            lblLensCatalogInfo.Text = $"렌즈: {_lenses.Count}개";
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

        private void btnLensCatalogReload_Click(object sender, EventArgs e)
        {
            LoadCatalog();
            dgvResults.Rows.Clear();
            ClearDiagram();
        }

        private void btnLensCatalogEdit_Click(object sender, EventArgs e)
        {
            var entries = new List<LensEntry>();
            foreach (var l in _lenses)
                entries.Add(new LensEntry
                {
                    name = l.Name, manufacturer = l.Manufacturer,
                    focalLength = l.FocalLength, minWD = l.MinWD, imageCircle = l.ImageCircle,
                });

            using (var dlg = new LensEditorForm(entries))
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
        private void btnRevCalc_Click(object sender, EventArgs e) => RunReverseCalculate();

        private void chkMinPixFilter_CheckedChanged(object sender, EventArgs e)
        {
            _minPixFilterEnabled = chkMinPixFilter.Checked;
            txtMinPixCount.Enabled = _minPixFilterEnabled;
            RunReverseCalculate(silent: true);
        }

        private void txtMinPixCount_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtMinPixCount.Text.Trim(), out int n) && n > 0)
            {
                _minPixCount = n;
                if (_minPixFilterEnabled)
                    RunReverseCalculate(silent: true);
            }
        }

        private void txtCamKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            _camKeyword = txtCamKeyword.Text.Trim();
            RunReverseCalculate(silent: true);
            e.SuppressKeyPress = true;
        }

        private void txtLensKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            _lensKeyword = txtLensKeyword.Text.Trim();
            RunReverseCalculate(silent: true);
            e.SuppressKeyPress = true;
        }

        private void RunReverseCalculate(bool silent = false)
        {
            ShowSearchingPopup(true);
            // Update()로 팝업을 즉시 렌더한 뒤 계산 시작 (BeginInvoke만으로는 WM_PAINT 순서 보장 안됨)
            _searchingPopup?.Update();
            BeginInvoke(new Action(() =>
            {
                try { ReverseCalculate(silent); }
                finally { ShowSearchingPopup(false); }
            }));
        }

        private void ShowSearchingPopup(bool show)
        {
            if (show)
            {
                if (_searchingPopup != null) return;
                var parentForm = this.FindForm();
                var center = dgvResults.PointToScreen(
                    new Point(dgvResults.Width / 2, dgvResults.Height / 2));
                Color popupBg     = Color.FromArgb(10, 16, 26);    // 거의 검정
                Color popupFg     = Color.FromArgb(0, 220, 200);   // 밝은 cyan 텍스트
                Color popupBorder = Color.FromArgb(0, 180, 160);   // cyan 테두리
                var lbl = new Label
                {
                    Text      = "검색 중...",
                    Dock      = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = popupFg,
                    BackColor = popupBg,
                    Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                };
                // 테두리 Panel로 감싸기
                var border = new Panel
                {
                    Dock      = DockStyle.Fill,
                    BackColor = popupBorder,
                    Padding   = new Padding(2),
                };
                border.Controls.Add(lbl);
                _searchingPopup = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size            = new Size(400, 104),
                    BackColor       = popupBg,
                    StartPosition   = FormStartPosition.Manual,
                    Location        = new Point(center.X - 200, center.Y - 52),
                    ShowInTaskbar   = false,
                    TopMost         = true,
                };
                _searchingPopup.Controls.Add(border);
                _searchingPopup.Show(parentForm);
            }
            else
            {
                _searchingPopup?.Close();
                _searchingPopup?.Dispose();
                _searchingPopup = null;
            }
        }

        private void ReverseCalculate(bool silent = false)
        {
            void Err(string msg) { if (!silent) ShowRevError(msg); }

            if (!double.TryParse(txtRevWD.Text.Trim(),      out double wd)     || wd      <= 0) { Err("WD를 확인하세요.");          return; }
            if (!double.TryParse(txtRevFovX.Text.Trim(),    out double targetX) || targetX <= 0) { Err("검사 영역 가로를 확인하세요."); return; }
            if (!double.TryParse(txtRevFovY.Text.Trim(),    out double targetY) || targetY <= 0) { Err("검사 영역 세로를 확인하세요."); return; }
            if (!double.TryParse(txtRevMinFeat.Text.Trim(), out double minFeat) || minFeat <= 0) { Err("최소 검출 크기를 확인하세요.");  return; }

            if (_lenses.Count == 0)
            {
                Err("렌즈 카탈로그가 비어 있습니다.\n렌즈 편집에서 렌즈를 추가하거나 lenses.json을 확인하세요.");
                return;
            }

            _revWd = wd; _revFovX = targetX; _revFovY = targetY; _revMinFeat = minFeat;

            dgvResults.Rows.Clear();

            bool hasCamKw  = !string.IsNullOrEmpty(_camKeyword);
            bool hasLensKw = !string.IsNullOrEmpty(_lensKeyword);

            foreach (var cam in _cameras)
            {
                if (hasCamKw && cam.Name.IndexOf(_camKeyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                double sensorDiag = Math.Sqrt(cam.SensorX * cam.SensorX + cam.SensorY * cam.SensorY);

                foreach (var lens in _lenses)
                {
                    if (hasLensKw && lens.Name.IndexOf(_lensKeyword, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;
                    // 이미지 서클 호환성 체크: 센서 대각선 ≤ 이미지 서클
                    if (lens.ImageCircle < sensorDiag) continue;

                    // 최소 WD 체크
                    if (wd < lens.MinWD) continue;

                    double actualFovX, actualFovY;
                    if (lens.IsFixedMag)
                    {
                        actualFovX = cam.SensorX / lens.Magnification;
                        actualFovY = cam.SensorY / lens.Magnification;
                    }
                    else
                    {
                        if (lens.FocalLength <= 0) continue;
                        actualFovX = wd * cam.SensorX / lens.FocalLength;
                        actualFovY = wd * cam.SensorY / lens.FocalLength;
                    }
                    if (actualFovX < targetX || actualFovY < targetY) continue;

                    double resMm = actualFovX / cam.PixelW;
                    bool resFits = resMm <= minFeat;

                    // 최소 픽셀 필터
                    if (_minPixFilterEnabled)
                    {
                        double px = minFeat / resMm;
                        if (px < _minPixCount) continue;
                    }

                    int rowIdx = dgvResults.Rows.Add(
                        cam.Name,
                        lens.Name,
                        resMm.ToString("F3") + " mm/px",
                        actualFovX.ToString("F1") + " mm",
                        actualFovY.ToString("F1") + " mm",
                        resFits ? "✓" : "✗"
                    );

                    var row = dgvResults.Rows[rowIdx];
                    row.DefaultCellStyle.ForeColor = resFits
                        ? Color.FromArgb(80, 220, 130)
                        : Color.FromArgb(200, 100, 80);
                    row.Tag = (cam, lens, actualFovX, actualFovY, resMm);
                }
            }

            if (dgvResults.Rows.Count == 0)
            {
                Err("조건을 만족하는 조합이 없습니다.\nWD를 늘리거나 검사 영역을 줄여보세요.");
                ClearDiagram();
            }
            else
            {
                dgvResults.ClearSelection();
                dgvResults.Rows[0].Selected = true;
                dgvResults.CurrentCell = dgvResults.Rows[0].Cells[0];
            }
        }

        private void dgvResults_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0) return;
            var row = dgvResults.SelectedRows[0];
            if (row.Tag == null) return;

            var (cam, lens, fovX, fovY, resMm) = ((CameraSpec, LensSpec, double, double, double))row.Tag;

            _wd = _revWd;
            _fl  = lens.IsFixedMag ? (double?)null : lens.FocalLength;
            _mag = lens.IsFixedMag ? lens.Magnification : (double?)null;
            _sensorX = cam.SensorX;
            _sensorY = cam.SensorY;
            _fovX = fovX;
            _fovY = fovY;

            UpdateRevResPanel(resMm);
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
            ClearRevResPanel();
            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        private void UpdateRevResPanel(double resMm)
        {
            lblRevResMm.Text = $"{resMm:F4} mm/px";
            lblRevResUm.Text = $"{resMm * 1000:F2} μm/px";

            if (_revMinFeat.HasValue)
            {
                double px = _revMinFeat.Value / resMm;
                lblRevResPx.Text      = $"최소검출 크기의 픽셀({_revMinFeat.Value:F2} mm) : {px:F1} px";
                lblRevResPx.ForeColor = px >= 1.5
                    ? Color.FromArgb(80, 210, 130)
                    : Color.FromArgb(220, 100, 80);
            }
            else
            {
                lblRevResPx.Text = "";
            }
        }

        private void ClearRevResPanel()
        {
            lblRevResMm.Text = "-";
            lblRevResUm.Text = "";
            lblRevResPx.Text = "";
        }

        private static void ShowRevError(string msg) =>
            MessageBox.Show(msg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // ─────────────────────────────────────────────
        //  Obstacle bar event wiring
        // ─────────────────────────────────────────────
        private void InitObstacleEvents()
        {
            // Subject size — update on Enter only
            KeyEventHandler subjEnter = (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                if (double.TryParse(txtSubjX.Text, out double sx) && sx > 0) _subjectMmX = sx;
                if (double.TryParse(txtSubjY.Text, out double sy) && sy > 0) _subjectMmY = sy;
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                e.SuppressKeyPress = true;
            };
            txtSubjX.KeyDown += subjEnter;
            txtSubjY.KeyDown += subjEnter;

            // Obstacle checkbox
            chkObst.CheckedChanged += (s, e) =>
            {
                _obstEnabled = chkObst.Checked;
                if (_obstEnabled)
                {
                    _obstDistFromCam = _wd.HasValue ? _wd.Value / 3.0 : 250.0;
                    _obstHeightMm    = _obstHalfH; // floor: bottom = 0
                }
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
            };

            // Obstacle size
            EventHandler obstSizeChanged = (s, e) =>
            {
                if (double.TryParse(txtObstH.Text, out double h) && h > 0) _obstHalfH = h / 2.0;
                if (double.TryParse(txtObstW.Text, out double w) && w > 0) _obstHalfW = w / 2.0;
                if (_obstEnabled) { pnlSideView.Invalidate(); pnlFrontView.Invalidate(); }
            };
            txtObstH.TextChanged += obstSizeChanged;
            txtObstW.TextChanged += obstSizeChanged;

            // pnlSubjSide = 50% width, dynamic
            pnlObstBar.Resize += (s, e) => pnlSubjSide.Width = pnlObstBar.Width / 2;
            pnlSubjSide.Width = pnlObstBar.Width / 2;
        }

        private void btn3DView_Click(object sender, EventArgs e) => Open3DView();

        private void Open3DView()
        {
            if (_fovX == null || _wd == null) return;

            using (var dlg = new View3DForm(
                wd          : _wd.Value,
                camHeight   : _cameraHeightMm,
                camOffsetX  : _cameraOffsetX,
                fovX        : _fovX.Value,
                fovY        : _fovY.Value,
                obstEnabled : _obstEnabled,
                obstDist    : _obstDistFromCam,
                obstHeight  : _obstHeightMm,
                obstOffsetX : _obstOffsetX,
                obstHalfW   : _obstHalfW,
                obstHalfH   : _obstHalfH,
                subjectW    : _subjectMmX,
                subjectH    : _subjectMmY))
            {
                dlg.ShowDialog(this);
            }
        }

        // ─────────────────────────────────────────────
        //  Forward calculation
        // ─────────────────────────────────────────────
        private void btnCalcFov_Click(object sender, EventArgs e) => Calculate();

        private void InputChanged(object sender, EventArgs e)
        {
            if (_suppressInputEvent) return;
            Calculate(silent: true);
        }

        private void cmbFovCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFovCamera.SelectedIndex < 0 || cmbFovCamera.SelectedIndex >= _cameras.Count) return;
            var cam = _cameras[cmbFovCamera.SelectedIndex];
            _suppressInputEvent = true;
            txtFovSensorX.Text = cam.SensorX.ToString("F2");
            txtFovSensorY.Text = cam.SensorY.ToString("F2");
            _suppressInputEvent = false;
            UpdateCompatibilityWarning();
            Calculate(silent: true);
        }

        private void cmbFovLens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFovLens.SelectedIndex < 0 || cmbFovLens.SelectedIndex >= _lenses.Count) return;
            var lens = _lenses[cmbFovLens.SelectedIndex];
            _suppressInputEvent = true;
            if (lens.IsFixedMag)
            {
                txtFovFL.Text    = lens.Magnification.ToString("F3");
                txtFovFL.Enabled = false;
            }
            else
            {
                txtFovFL.Text    = lens.FocalLength.ToString("F0");
                txtFovFL.Enabled = true;
            }
            _suppressInputEvent = false;
            UpdateCompatibilityWarning();
            Calculate(silent: true);
        }

        private void UpdateCompatibilityWarning()
        {
            if (cmbFovCamera.SelectedIndex < 0 || cmbFovCamera.SelectedIndex >= _cameras.Count ||
                cmbFovLens.SelectedIndex   < 0 || cmbFovLens.SelectedIndex   >= _lenses.Count)
            {
                lblFovCompatible.Text = "";
                return;
            }

            var cam  = _cameras[cmbFovCamera.SelectedIndex];
            var lens = _lenses[cmbFovLens.SelectedIndex];
            double sensorDiag = Math.Sqrt(cam.SensorX * cam.SensorX + cam.SensorY * cam.SensorY);

            if (lens.ImageCircle < sensorDiag)
            {
                lblFovCompatible.Text      = $"⚠ 이미지 서클({lens.ImageCircle:F0}mm) < 센서 대각({sensorDiag:F1}mm)";
                lblFovCompatible.ForeColor = Color.FromArgb(220, 100, 80);
            }
            else
            {
                lblFovCompatible.Text      = $"✓ 호환  (서클 {lens.ImageCircle:F0}mm ≥ 대각 {sensorDiag:F1}mm)";
                lblFovCompatible.ForeColor = Color.FromArgb(80, 200, 130);
            }
        }

        private void Calculate(bool silent = false)
        {
            if (!double.TryParse(txtFovWD.Text.Trim(),      out double wd) ||
                !double.TryParse(txtFovSensorX.Text.Trim(), out double sx) ||
                !double.TryParse(txtFovSensorY.Text.Trim(), out double sy))
            {
                if (!silent)
                    MessageBox.Show("입력값을 확인하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if selected lens is fixed-magnification
            bool isFixedMag = cmbFovLens.SelectedIndex >= 0 &&
                              cmbFovLens.SelectedIndex < _lenses.Count &&
                              _lenses[cmbFovLens.SelectedIndex].IsFixedMag;

            if (isFixedMag)
            {
                double mag = _lenses[cmbFovLens.SelectedIndex].Magnification;
                _wd = wd; _fl = 0; _mag = mag; _sensorX = sx; _sensorY = sy;
                _fovX = sx / mag;
                _fovY = sy / mag;
            }
            else
            {
                if (!double.TryParse(txtFovFL.Text.Trim(), out double fl) || fl == 0)
                {
                    if (!silent)
                        MessageBox.Show("입력값을 확인하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _wd = wd; _fl = fl; _mag = null; _sensorX = sx; _sensorY = sy;
                _fovX = wd * sx / fl;
                _fovY = wd * sy / fl;
            }

            if (!_isDragging) RebuildPixelsPerMm();

            lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
            lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";

            if (cmbFovCamera.SelectedIndex >= 0 && cmbFovCamera.SelectedIndex < _cameras.Count)
            {
                var cam = _cameras[cmbFovCamera.SelectedIndex];
                double resMm = _fovX.Value / cam.PixelW;
                lblResResult.Text = $"{resMm:F4} mm/px  ({resMm * 1000:F2} μm/px)";
            }
            else
            {
                lblResResult.Text = "-";
            }

            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        // ─────────────────────────────────────────────
        //  Layout helpers
        // ─────────────────────────────────────────────
        private void RebuildPixelsPerMm()
        {
            if (_wd == null || _wd.Value <= 0) return;
            int W = Math.Max(pnlSideView.Width, 200);
            // Set scale so camera starts at W*0.25 (subject at W*0.75)
            _pixelsPerMm = W * 0.50 / _wd.Value;
            _baseW       = W;
        }

        private void GetSideLayout(out int midY, out int subX, out int camX)
        {
            int W      = Math.Max(pnlSideView.Width, 200);
            int H      = Math.Max(pnlSideView.Height, 100);
            int floorY = H - SideMarginBot;
            int availH = floorY - SideMarginTop;
            int subPx  = (int)(availH * 0.80);
            double scaleV = subPx / _subjectMmY;
            midY = floorY - (int)(_cameraHeightMm * scaleV);

            subX = (int)(W * 0.75);

            if (_pixelsPerMm > 0 && _baseW > 0 && _wd != null && _wd.Value > 0)
            {
                // Proportionally rescale to current panel width, then camera moves with WD
                double ppm = _pixelsPerMm * W / (double)_baseW;
                _pixelsPerMm = ppm;   // normalize so drag handler sees current-W scale
                _baseW       = W;
                camX = subX - (int)(_wd.Value * ppm);
                camX = Math.Max(SideMarginL + 10, Math.Min(camX, subX - 30));
            }
            else
            {
                camX = (int)(W * 0.25);
            }
        }

        private Rectangle GetCameraHitRect(int camX, int midY)
            => new Rectangle(camX - 38, midY - 22, 54, 44);

        private Rectangle GetObstSideRect(int camX, int subX, int floorY, double scaleV)
        {
            if (_wd == null || _wd.Value <= 0 || _obstDistFromCam <= 0) return Rectangle.Empty;
            int ox  = camX + (int)((_obstDistFromCam / _wd.Value) * (subX - camX));
            int oty = floorY - (int)((_obstHeightMm + _obstHalfH) * scaleV);
            int oby = floorY - (int)((_obstHeightMm - _obstHalfH) * scaleV);
            return Rectangle.FromLTRB(ox - 8, oty - 4, ox + 8, oby + 4);
        }

        // ─────────────────────────────────────────────
        //  Side View — mouse drag
        // ─────────────────────────────────────────────
        private void pnlSideView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_wd == null || _pixelsPerMm <= 0) return;
            GetSideLayout(out int midY, out int subX, out int camX);

            int H      = pnlSideView.Height;
            int floorY = H - SideMarginBot;
            int availH = floorY - SideMarginTop;
            int subPx  = (int)(availH * 0.80);
            double scaleV = subPx / _subjectMmY;

            // Obstacle hit check (higher priority than camera)
            if (_obstEnabled && !GetObstSideRect(camX, subX, floorY, scaleV).IsEmpty &&
                GetObstSideRect(camX, subX, floorY, scaleV).Contains(e.Location))
            {
                _isDraggingObst = true;
                int ox = camX + (int)((_obstDistFromCam / _wd.Value) * (subX - camX));
                int oy = floorY - (int)(_obstHeightMm * scaleV);
                _obstDragOffX = e.X - ox;
                _obstDragOffY = e.Y - oy;
                pnlSideView.Cursor = Cursors.SizeAll;
                return;
            }

            if (GetCameraHitRect(camX, midY).Contains(e.Location))
            {
                _isDragging  = true;
                _dragOffsetX = e.X - camX;
                _dragOffsetY = e.Y - midY;
                pnlSideView.Cursor = Cursors.SizeAll;
            }
        }

        private void pnlSideView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_wd == null) return;
            GetSideLayout(out int midY, out int subX, out int camX);

            int H2     = pnlSideView.Height;
            int floorY2 = H2 - SideMarginBot;
            int availH2 = floorY2 - SideMarginTop;
            double scaleV2 = (int)(availH2 * 0.80) / _subjectMmY;

            if (_isDraggingObst)
            {
                int ox = Math.Max(camX + 10, Math.Min(subX - 10, e.X - _obstDragOffX));
                double newDist = (ox - camX) / (double)(subX - camX) * _wd.Value;
                _obstDistFromCam = Math.Max(10, Math.Min(_wd.Value - 10, Math.Round(newDist / 5.0) * 5.0));

                int oy = e.Y - _obstDragOffY;
                double obstHtWorld = (floorY2 - oy) / scaleV2;
                _obstHeightMm = Math.Max(0, Math.Min(800, Math.Round(obstHtWorld / 5.0) * 5.0));

                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }

            if (!_isDragging)
            {
                bool overObst = _obstEnabled && !GetObstSideRect(camX, subX, floorY2, scaleV2).IsEmpty &&
                                GetObstSideRect(camX, subX, floorY2, scaleV2).Contains(e.Location);
                pnlSideView.Cursor = (overObst || GetCameraHitRect(camX, midY).Contains(e.Location))
                    ? Cursors.SizeAll : Cursors.Default;
                return;
            }

            // 이전 카메라 영역 기록
            Rectangle oldDirty = SideCamDirtyRect(camX, midY);

            // ── Horizontal drag → WD
            int newCamXPx = Math.Max(SideMarginL + 10, Math.Min(e.X - _dragOffsetX, subX - 30));
            double newWD  = Math.Max(10.0, Math.Round((subX - newCamXPx) / _pixelsPerMm / 10.0) * 10.0);

            // ── Vertical drag → camera height
            int H      = pnlSideView.Height;
            int floorY = H - SideMarginBot;
            int availH = floorY - SideMarginTop;
            int subPx  = (int)(availH * 0.80);
            double scaleV = subPx / _subjectMmY;
            int newCamY   = e.Y - _dragOffsetY;
            double newHt  = (floorY - newCamY) / scaleV;
            _cameraHeightMm = Math.Max(0, Math.Min(800, Math.Round(newHt / 5.0) * 5.0));

            // Keep obstacle fixed in world space (maintain distance from subject)
            if (_obstEnabled && _obstDistFromCam > 0)
            {
                double distFromSubject = _wd.Value - _obstDistFromCam;
                _obstDistFromCam = Math.Max(10, Math.Min(newWD - 10, newWD - distFromSubject));
            }

            _wd = newWD;
            if (_mag.HasValue && _mag.Value > 0)
            {
                // fixed-mag: FOV does not change with WD
            }
            else if (_fl.HasValue && _fl.Value > 0)
            {
                _fovX = _wd.Value * _sensorX.Value / _fl.Value;
                _fovY = _wd.Value * _sensorY.Value / _fl.Value;
            }

            _suppressInputEvent = true;
            if (tabMain.SelectedIndex == 0)
            {
                _revWd = newWD;
                txtRevWD.Text = newWD.ToString("F1");

                if (dgvResults.CurrentRow?.Tag is ValueTuple<CameraSpec, LensSpec, double, double, double> tag)
                {
                    var (cam, lens, _, _, _) = tag;
                    double fovX = lens.IsFixedMag
                        ? cam.SensorX / lens.Magnification
                        : newWD * cam.SensorX / lens.FocalLength;
                    double resMm = fovX / cam.PixelW;
                    UpdateRevResPanel(resMm);
                }
            }
            else
            {
                txtFovWD.Text = newWD.ToString("F1");
                lblFovXResult.Text = _fovX.Value.ToString("F2") + " mm";
                lblFovYResult.Text = _fovY.Value.ToString("F2") + " mm";
            }
            _suppressInputEvent = false;

            pnlSideView.Invalidate();   // WD 화살표 포함 전체 갱신 (DoubleBuffered로 깜빡임 없음)
            // Front View는 MouseUp에서만 갱신
        }

        private void pnlSideView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDraggingObst)
            {
                _isDraggingObst = false;
                pnlSideView.Cursor = Cursors.Default;
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }
            if (!_isDragging) return;
            _isDragging = false;
            pnlSideView.Cursor = Cursors.Default;
            pnlSideView.Invalidate();
            pnlFrontView.Invalidate();
        }

        // ─────────────────────────────────────────────
        //  Front View — layout helper
        // ─────────────────────────────────────────────
        private double GetFrontScale(int W, int H)
        {
            int marginH = 60, marginV = 20;
            double sv = (H - marginV - 50) * 0.97 / _subjectMmY;  // 50 = floorY bottom margin
            double sh = (W - marginH * 2) * 0.78 / _subjectMmX;
            return Math.Max(0.05, Math.Min(sv, sh));
        }

        private Rectangle GetFrontCameraHitRect(int cx, int cy)
            => new Rectangle(cx - 10, cy - 12, 20, 24);

        private Rectangle GetObstFrontRect(int W, int floorY, double scale)
        {
            int ox = W / 2 + (int)(_obstOffsetX * scale);
            int oy = floorY - (int)(_obstHeightMm * scale);
            int hw = Math.Max(4, (int)(_obstHalfW * scale));
            int hh = Math.Max(4, (int)(_obstHalfH * scale));
            return Rectangle.FromLTRB(ox - hw, oy - hh, ox + hw, oy + hh);
        }

        // 우측하단 리사이즈 핸들 영역 (12×12 px)
        private Rectangle GetObstFrontResizeHandle(int W, int floorY, double scale)
        {
            var r = GetObstFrontRect(W, floorY, scale);
            return new Rectangle(r.Right - 12, r.Bottom - 12, 12, 12);
        }

        // ─────────────────────────────────────────────
        //  Front View — mouse drag
        // ─────────────────────────────────────────────
        private void pnlFrontView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_fovX == null) return;
            int W = pnlFrontView.Width, H = pnlFrontView.Height;
            double scale = GetFrontScale(W, H);
            int floorY = H - 50;
            int camCX = W / 2 + (int)(_cameraOffsetX  * scale);
            int camCY = floorY  - (int)(_cameraHeightMm * scale);

            // Obstacle resize handle (우선순위 최상)
            if (_obstEnabled && GetObstFrontResizeHandle(W, floorY, scale).Contains(e.Location))
            {
                _isResizingObstFront = true;
                _obstResizeBaseW     = _obstHalfW * 2;
                _obstResizeBaseH     = _obstHalfH * 2;
                _obstResizeStartX    = e.X;
                _obstResizeStartY    = e.Y;
                _obstResizeFixedLeft = _obstOffsetX - _obstHalfW;        // 좌측 고정
                _obstResizeFixedTop  = _obstHeightMm + _obstHalfH;       // 상단 고정
                pnlFrontView.Cursor  = Cursors.SizeNWSE;
                return;
            }

            // Obstacle move
            if (_obstEnabled && GetObstFrontRect(W, floorY, scale).Contains(e.Location))
            {
                var r = GetObstFrontRect(W, floorY, scale);
                _isDraggingObstFront = true;
                _obstFrontDragOffX   = e.X - (r.Left + r.Width  / 2);
                _obstFrontDragOffY   = e.Y - (r.Top  + r.Height / 2);
                pnlFrontView.Cursor  = Cursors.SizeAll;
                return;
            }

            if (GetFrontCameraHitRect(camCX, camCY).Contains(e.Location))
            {
                _isDraggingFront  = true;
                _frontDragOffsetX = e.X - camCX;
                _frontDragOffsetY = e.Y - camCY;
                pnlFrontView.Cursor = Cursors.SizeAll;
            }
        }

        private void pnlFrontView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_fovX == null) return;
            int W = pnlFrontView.Width, H = pnlFrontView.Height;
            double scale = GetFrontScale(W, H);
            int floorY = H - 50;
            int camCX = W / 2 + (int)(_cameraOffsetX  * scale);
            int camCY = floorY  - (int)(_cameraHeightMm * scale);

            if (_isResizingObstFront)
            {
                double newW = Math.Max(10, _obstResizeBaseW + (e.X - _obstResizeStartX) / scale);
                double newH = Math.Max(10, _obstResizeBaseH + (e.Y - _obstResizeStartY) / scale);
                _obstHalfW    = Math.Round(newW / 2 / 5.0) * 5.0;
                _obstHalfH    = Math.Round(newH / 2 / 5.0) * 5.0;
                // 상단-좌측 고정: 중심 위치 보정
                _obstOffsetX  = _obstResizeFixedLeft + _obstHalfW;
                _obstHeightMm = _obstResizeFixedTop  - _obstHalfH;
                // UI 텍스트박스 동기화
                txtObstW.Text = (_obstHalfW * 2).ToString("F0");
                txtObstH.Text = (_obstHalfH * 2).ToString("F0");
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }

            if (_isDraggingObstFront)
            {
                int cx = e.X - _obstFrontDragOffX;
                int cy = e.Y - _obstFrontDragOffY;
                _obstOffsetX  = Math.Round((cx - W / 2) / scale / 5.0) * 5.0;
                _obstHeightMm = Math.Max(0, Math.Round((floorY - cy) / scale / 5.0) * 5.0);
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }

            if (!_isDraggingFront)
            {
                if (_obstEnabled)
                {
                    if (GetObstFrontResizeHandle(W, floorY, scale).Contains(e.Location))
                    { pnlFrontView.Cursor = Cursors.SizeNWSE; return; }
                    if (GetObstFrontRect(W, floorY, scale).Contains(e.Location))
                    { pnlFrontView.Cursor = Cursors.SizeAll; return; }
                }
                pnlFrontView.Cursor = GetFrontCameraHitRect(camCX, camCY).Contains(e.Location)
                    ? Cursors.SizeAll : Cursors.Default;
                return;
            }

            int fovPxW = (int)((_fovX ?? 0) * scale);
            int fovPxH = (int)((_fovY ?? 0) * scale);

            // 이전 카메라+FOV 영역 기록
            Rectangle oldDirty = FrontCamDirtyRect(camCX, camCY, fovPxW, fovPxH);

            int newCX = e.X - _frontDragOffsetX;
            int newCY = e.Y - _frontDragOffsetY;
            _cameraOffsetX  = Math.Max(-800, Math.Min(800, (newCX - W / 2) / scale));
            _cameraHeightMm = Math.Max(0, Math.Min(800, (floorY - newCY) / scale));
            _cameraOffsetX  = Math.Round(_cameraOffsetX  / 5.0) * 5.0;
            _cameraHeightMm = Math.Round(_cameraHeightMm / 5.0) * 5.0;

            int newCamCX = W / 2 + (int)(_cameraOffsetX * scale);
            int newCamCY = floorY - (int)(_cameraHeightMm * scale);
            Rectangle newDirty = FrontCamDirtyRect(newCamCX, newCamCY, fovPxW, fovPxH);

            // Full repaint: obstacle shadow region may extend beyond camera+FOV dirty rect
            pnlFrontView.Invalidate();
            // Side View는 MouseUp에서만 갱신
        }

        private void pnlFrontView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isResizingObstFront)
            {
                _isResizingObstFront = false;
                pnlFrontView.Cursor = Cursors.Default;
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }
            if (_isDraggingObstFront)
            {
                _isDraggingObstFront = false;
                pnlFrontView.Cursor = Cursors.Default;
                pnlSideView.Invalidate();
                pnlFrontView.Invalidate();
                return;
            }
            if (!_isDraggingFront) return;
            _isDraggingFront = false;
            pnlFrontView.Cursor = Cursors.Default;
            pnlSideView.Invalidate();
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

            GetSideLayout(out int camY, out int subX, out int camX);

            // ── Layout derived from GetSideLayout (must match its formula)
            int floorY    = H - SideMarginBot;
            int availH    = floorY - SideMarginTop;
            int subjectPx = (int)(availH * 0.80);                    // subject height in pixels
            double scaleV = subjectPx / _subjectMmY;                // px per mm (vertical)
            int subTop    = floorY - subjectPx;
            int fovHalfH  = (int)(_fovY.Value / 2.0 * scaleV);
            fovHalfH = Math.Max(8, fovHalfH);

            Color colFov     = Color.FromArgb(60, 200, 100);
            Color colWD      = Color.FromArgb(80, 160, 255);
            Color colSubject = Color.FromArgb(80, 170, 230);
            Color colCamera  = _isDragging ? Color.FromArgb(220, 230, 255) : Color.FromArgb(150, 180, 220);
            Color colFloor   = Color.FromArgb(70, 90, 110);
            Color colDim     = Color.FromArgb(120, 130, 145);

            // ── Floor line
            using (Pen p = new Pen(colFloor, 1.5f))
                g.DrawLine(p, SideMarginL, floorY, W - SideMarginR, floorY);
            using (Font f = new Font("Segoe UI", 7.5f))
            using (SolidBrush b = new SolidBrush(colFloor))
                g.DrawString("Floor", f, b, SideMarginL + 2, floorY + 3);

            // ── Coordinate axis (origin = camera position / floor)
            {
                int axOX = camX;
                int axOY = floorY;
                int axLenY = 36, axLenX = 72;   // X = depth (WD direction)
                Color colAx = Color.FromArgb(200, 200, 60);
                using (Pen p = new Pen(colAx, 1.5f))
                using (SolidBrush b = new SolidBrush(colAx))
                using (Font fAx = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                {
                    // Y axis (up)
                    g.DrawLine(p, axOX, axOY, axOX, axOY - axLenY);
                    g.DrawLine(p, axOX, axOY - axLenY, axOX - 4, axOY - axLenY + 7);
                    g.DrawLine(p, axOX, axOY - axLenY, axOX + 4, axOY - axLenY + 7);
                    g.DrawString("Y", fAx, b, axOX + 5, axOY - axLenY - 2);
                    // Z axis (right = depth toward subject)
                    g.DrawLine(p, axOX, axOY, axOX + axLenX, axOY);
                    g.DrawLine(p, axOX + axLenX, axOY, axOX + axLenX - 7, axOY - 4);
                    g.DrawLine(p, axOX + axLenX, axOY, axOX + axLenX - 7, axOY + 4);
                    g.DrawString("Z", fAx, b, axOX + axLenX + 3, axOY - 8);
                    // Origin dot
                    g.FillEllipse(b, axOX - 3, axOY - 3, 6, 6);
                }
            }

            // ── Subject box (side view — visible as a vertical bar, 800mm tall)
            const int subBarW = 8;
            using (SolidBrush b = new SolidBrush(Color.FromArgb(45, colSubject.R, colSubject.G, colSubject.B)))
                g.FillRectangle(b, subX - subBarW / 2, subTop, subBarW, subjectPx);
            using (Pen p = new Pen(colSubject, 2.5f))
                g.DrawRectangle(p, subX - subBarW / 2, subTop, subBarW, subjectPx);

            // Subject height dimension arrow (left of bar)
            using (Pen p = new Pen(colSubject, 1f))
            using (SolidBrush b = new SolidBrush(colSubject))
            using (Font f = new Font("Segoe UI", 7.5f))
                DrawDimArrowV(g, p, b, f, subX - subBarW / 2 - 18, subTop, floorY, $"{_subjectMmY:F0}\nmm", colSubject, leftSide: true);

            // Subject label (right of bar)
            using (Font f = new Font("Segoe UI", 8f))
            using (SolidBrush b = new SolidBrush(colSubject))
            {
                g.DrawString("Subject", f, b, subX + subBarW / 2 + 5, camY - 8);
                g.DrawString($"{_subjectMmX:F0}×{_subjectMmY:F0}mm", f, b, subX + subBarW / 2 + 5, camY + 6);
            }

            // ── Camera height guide (dashed vertical from floor to camera center)
            using (Pen p = new Pen(Color.FromArgb(55, 75, 105), 1f) { DashStyle = DashStyle.Dot })
                g.DrawLine(p, camX - 12, floorY, camX - 12, camY);
            using (Font f = new Font("Segoe UI", 7.5f))
            using (SolidBrush b = new SolidBrush(colDim))
                g.DrawString(_cameraHeightMm.ToString("F0") + "mm", f, b, camX - 52, (floorY + camY) / 2 - 7);

            // ── FOV cone
            using (SolidBrush b = new SolidBrush(Color.FromArgb(18, 60, 200, 100)))
                g.FillPolygon(b, new[] { new Point(camX, camY), new Point(subX, camY - fovHalfH), new Point(subX, camY + fovHalfH) });
            using (Pen p = new Pen(colFov, 1.5f) { DashStyle = DashStyle.Dash })
            {
                g.DrawLine(p, camX, camY, subX, camY - fovHalfH);
                g.DrawLine(p, camX, camY, subX, camY + fovHalfH);
            }
            using (Pen p = new Pen(Color.FromArgb(60, 100, 140), 1f) { DashStyle = DashStyle.Dot })
                g.DrawLine(p, camX, camY, subX + 12, camY);

            // ── Camera body
            Rectangle camRect = new Rectangle(camX - 30, camY - 12, 30, 24);
            using (SolidBrush b = new SolidBrush(_isDragging ? Color.FromArgb(55, 72, 100) : Color.FromArgb(40, 55, 75)))
                g.FillRectangle(b, camRect);
            using (Pen p = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawRectangle(p, camRect);
            using (Pen p = new Pen(colCamera, _isDragging ? 2.5f : 2f))
                g.DrawEllipse(p, camX - 8, camY - 8, 16, 16);

            if (_isDragging)
            {
                using (Font f = new Font("Segoe UI", 7.5f))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(180, 200, 255)))
                {
                    SizeF sz = g.MeasureString("↔", f);
                    g.DrawString("↔", f, b, camX - 15 - sz.Width / 2f, camY - 27);
                }
            }

            using (Font f = new Font("Segoe UI", 8.5f))
            using (SolidBrush b = new SolidBrush(colDim))
                g.DrawString("Camera", f, b, camX - 30, camY + 16);

            // ── WD dimension arrow (below floor)
            using (Pen p = new Pen(colWD, 1.5f))
            using (SolidBrush b = new SolidBrush(colWD))
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                DrawDimArrow(g, p, b, f, camX, floorY + 22, subX, "WD = " + _wd.Value.ToString("F1") + " mm", colWD);

            // ── FOV Y arrow (right of subject bar)
            int fovArrowX = subX + subBarW / 2 + 50;
            using (Pen p = new Pen(colFov, 2f))
            using (SolidBrush b = new SolidBrush(colFov))
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                DrawDimArrowV(g, p, b, f, fovArrowX, camY - fovHalfH, camY + fovHalfH, "FOV Y\n" + _fovY.Value.ToString("F1") + " mm", colFov, leftSide: false);

            // ── FOV X label (top center)
            using (Font f = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(colFov))
            {
                string lbl = "FOV X = " + _fovX.Value.ToString("F1") + " mm";
                SizeF sz = g.MeasureString(lbl, f);
                g.DrawString(lbl, f, b, (camX + subX) / 2f - sz.Width / 2f, SideMarginTop - 4);
            }

            // ── Obstacle + shadow
            if (_obstEnabled && _wd.HasValue && _obstDistFromCam > 0 && _obstDistFromCam < _wd.Value)
            {
                double ratio = _obstDistFromCam / _wd.Value;
                int ox    = camX + (int)(ratio * (subX - camX));
                int otY   = floorY - (int)((_obstHeightMm + _obstHalfH) * scaleV);
                int obY   = floorY - (int)((_obstHeightMm - _obstHalfH) * scaleV);
                int oCtrY = floorY - (int)(_obstHeightMm * scaleV);

                // Shadow: project obstacle top/bottom edges from camera through to subject
                double projRatio = _wd.Value / _obstDistFromCam;
                double shadowTopWorld = _cameraHeightMm + projRatio * (_obstHeightMm + _obstHalfH - _cameraHeightMm);
                double shadowBotWorld = _cameraHeightMm + projRatio * (_obstHeightMm - _obstHalfH - _cameraHeightMm);
                int stY = floorY - (int)(shadowTopWorld * scaleV);
                int sbY = floorY - (int)(shadowBotWorld * scaleV);

                // Clamp shadow to FOV cone at subject
                int fovTopAtSub = camY - fovHalfH;
                int fovBotAtSub = camY + fovHalfH;
                stY = Math.Max(fovTopAtSub, Math.Min(fovBotAtSub, stY));
                sbY = Math.Max(fovTopAtSub, Math.Min(fovBotAtSub, sbY));

                Color colObst   = Color.FromArgb(240, 120, 30);
                Color colShadow = Color.FromArgb(200, 80, 60);

                // Shadow polygon (obstacle face → subject)
                if (sbY > stY)
                {
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(50, colShadow.R, colShadow.G, colShadow.B)))
                        g.FillPolygon(b, new[]
                        {
                            new Point(ox, otY), new Point(ox, obY),
                            new Point(subX, sbY), new Point(subX, stY),
                        });
                    using (Pen p = new Pen(Color.FromArgb(100, colShadow.R, colShadow.G, colShadow.B), 1f) { DashStyle = DashStyle.Dash })
                    {
                        g.DrawLine(p, camX, camY, ox, otY);
                        g.DrawLine(p, camX, camY, ox, obY);
                        g.DrawLine(p, ox, otY, subX, stY);
                        g.DrawLine(p, ox, obY, subX, sbY);
                    }
                    // Shadow bar on subject
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(140, colShadow.R, colShadow.G, colShadow.B)))
                        g.FillRectangle(b, subX - 4, stY, 8, sbY - stY);
                }

                // Obstacle body
                bool hoverObst = _isDraggingObst;
                using (SolidBrush b = new SolidBrush(Color.FromArgb(hoverObst ? 130 : 90, colObst.R, colObst.G, colObst.B)))
                    g.FillRectangle(b, ox - 7, otY, 14, obY - otY);
                using (Pen p = new Pen(colObst, hoverObst ? 2.5f : 2f))
                    g.DrawRectangle(p, ox - 7, otY, 14, obY - otY);

                // Obstacle label
                using (Font f = new Font("Segoe UI", 7.5f))
                using (SolidBrush b = new SolidBrush(colObst))
                    g.DrawString("장애물", f, b, ox + 10, oCtrY - 7);
            }

            // ── Top-right info panel: Camera(X,Y,Z) + Obstacle(Y,Z)  [camera = Z origin]
            {
                Color colCamVal  = Color.FromArgb(150, 180, 220);
                Color colObstVal = Color.FromArgb(240, 120, 30);
                Color colHeader  = Color.FromArgb(200, 200, 210);
                Color colLabel   = Color.FromArgb(130, 145, 165);

                double camZ  = 0;                                         // camera is Z origin
                double obstZ = _obstEnabled ? _obstDistFromCam : double.NaN;
                double obstY = _obstEnabled ? _obstHeightMm    : double.NaN;

                var rows = new (string lbl, string val, bool isTitle, bool isObst)[]
                {
                    ("[ Camera ]",   "",                                                          true,  false),
                    ("  X",          $"{(_cameraOffsetX >= 0 ? "+" : "")}{_cameraOffsetX:F0} mm", false, false),
                    ("  Y",          $"{_cameraHeightMm:F0} mm",                                  false, false),
                    ("  Z",          $"{camZ:F0} mm",                                             false, false),
                    ("",             "",                                                          false, false),
                    ("[ Obstacle ]", "",                                                          true,  true),
                    ("  Y",          _obstEnabled ? $"{obstY:F0} mm" : "-",                       false, true),
                    ("  Z",          _obstEnabled ? $"{obstZ:F0} mm" : "-",                       false, true),
                };

                using (Font fInfo  = new Font("Segoe UI", 8f))
                using (Font fTitle = new Font("Segoe UI", 8f, FontStyle.Bold))
                {
                    float lineH  = fInfo.GetHeight(g) + 1f;
                    float panelH = rows.Length * lineH + 10f;
                    float panelW = 148f;
                    float px     = W - panelW - 6f;
                    float py     = SideMarginTop + 2f;

                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(55, 20, 25, 35)))
                        g.FillRectangle(bg, px - 4, py - 2, panelW, panelH);
                    using (Pen border = new Pen(Color.FromArgb(55, 200, 210, 220), 1f))
                        g.DrawRectangle(border, px - 4, py - 2, panelW, panelH);

                    float y0 = py + 3f;
                    foreach (var (lbl, val, isTitle, isObst) in rows)
                    {
                        Color valColor = isObst ? colObstVal : colCamVal;
                        if (isTitle)
                        {
                            Color hdrColor = isObst ? colObstVal : colHeader;
                            using (SolidBrush b = new SolidBrush(hdrColor))
                                g.DrawString(lbl, fTitle, b, px, y0);
                        }
                        else if (lbl != "")
                        {
                            using (SolidBrush b = new SolidBrush(colLabel))
                                g.DrawString(lbl, fInfo, b, px, y0);
                            SizeF vsz = g.MeasureString(val, fInfo);
                            using (SolidBrush b = new SolidBrush(valColor))
                                g.DrawString(val, fInfo, b, px + panelW - vsz.Width - 8f, y0);
                        }
                        y0 += lineH;
                    }
                }
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

            // ── Unified scale: fit subject in available space
            double scale     = GetFrontScale(W, H);
            int floorY       = H - 50;
            int subjectPxW   = (int)(_subjectMmX * scale);
            int subjectPxH   = (int)(_subjectMmY * scale);

            // Subject: centered horizontally, sits on floor
            int sx = W / 2 - subjectPxW / 2;
            int sy = floorY - subjectPxH;

            // Camera position (in front-elevation coords)
            int camCX = W / 2 + (int)(_cameraOffsetX  * scale);
            int camCY = floorY - (int)(_cameraHeightMm * scale);

            // FOV rect: centered at camera aim point, scaled by mm
            int fovPxW = (int)(fovX * scale);
            int fovPxH = (int)(fovY * scale);
            int fx = camCX - fovPxW / 2;
            int fy = camCY - fovPxH / 2;

            Color colSubject = Color.FromArgb(80, 170, 230);
            Color colFov     = Color.FromArgb(60, 200, 100);
            Color colDim     = Color.FromArgb(80, 160, 255);
            Color colFloor   = Color.FromArgb(70, 90, 110);
            Color colCamera  = _isDraggingFront
                ? Color.FromArgb(220, 230, 255)
                : Color.FromArgb(150, 180, 220);

            // ── Floor line
            using (Pen p = new Pen(colFloor, 1.5f))
                g.DrawLine(p, 30, floorY, W - 30, floorY);
            using (Font f = new Font("Segoe UI", 7.5f))
            using (SolidBrush b = new SolidBrush(colFloor))
                g.DrawString("Floor", f, b, 32, floorY - 16);

            // ── Coordinate axis (bottom-left, origin = camera center / floor)
            {
                int camAxisX = W / 2 + (int)(_cameraOffsetX * scale);
                int axOX = camAxisX;
                int axOY = floorY;
                int axLenY = 36, axLenX = 72;
                Color colAx = Color.FromArgb(200, 200, 60);
                using (Pen p = new Pen(colAx, 1.5f))
                using (SolidBrush b = new SolidBrush(colAx))
                using (Font fAx = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                {
                    // Y axis (up)
                    g.DrawLine(p, axOX, axOY, axOX, axOY - axLenY);
                    g.DrawLine(p, axOX, axOY - axLenY, axOX - 4, axOY - axLenY + 7);
                    g.DrawLine(p, axOX, axOY - axLenY, axOX + 4, axOY - axLenY + 7);
                    g.DrawString("Y", fAx, b, axOX + 5, axOY - axLenY - 2);
                    // X axis (right)
                    g.DrawLine(p, axOX, axOY, axOX + axLenX, axOY);
                    g.DrawLine(p, axOX + axLenX, axOY, axOX + axLenX - 7, axOY - 4);
                    g.DrawLine(p, axOX + axLenX, axOY, axOX + axLenX - 7, axOY + 4);
                    g.DrawString("X", fAx, b, axOX + axLenX + 3, axOY - 8);
                    // Origin dot
                    g.FillEllipse(b, axOX - 3, axOY - 3, 6, 6);
                }
            }

            // ── Subject rectangle
            using (SolidBrush b = new SolidBrush(Color.FromArgb(30, colSubject.R, colSubject.G, colSubject.B)))
                g.FillRectangle(b, sx, sy, subjectPxW, subjectPxH);
            using (Pen p = new Pen(colSubject, 2f))
                g.DrawRectangle(p, sx, sy, subjectPxW, subjectPxH);
            using (Font f = new Font("Segoe UI", 8f))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(180, colSubject.R, colSubject.G, colSubject.B)))
                g.DrawString($"Subject  {_subjectMmX:F0}×{_subjectMmY:F0} mm", f, b, sx + 4, sy + 4);

            // ── FOV rectangle
            using (SolidBrush b = new SolidBrush(Color.FromArgb(20, colFov.R, colFov.G, colFov.B)))
                g.FillRectangle(b, fx, fy, fovPxW, fovPxH);
            using (Pen p = new Pen(colFov, 2f))
                g.DrawRectangle(p, fx, fy, fovPxW, fovPxH);
            // center crosshairs inside FOV
            using (Pen p = new Pen(Color.FromArgb(35, colFov.R, colFov.G, colFov.B), 1f) { DashStyle = DashStyle.Dot })
            {
                g.DrawLine(p, fx, camCY, fx + fovPxW, camCY);
                g.DrawLine(p, camCX, fy, camCX, fy + fovPxH);
            }

            // ── Camera height guide (dashed vertical from floor to camera)
            using (Pen p = new Pen(Color.FromArgb(55, 75, 105), 1f) { DashStyle = DashStyle.Dot })
                g.DrawLine(p, camCX, floorY, camCX, camCY + 14);
            using (Font f = new Font("Segoe UI", 7.5f))
            using (SolidBrush b = new SolidBrush(colDim))
                g.DrawString(_cameraHeightMm.ToString("F0") + "mm", f, b, camCX + 4, (floorY + camCY) / 2 - 7);

            // ── Camera icon (navy rectangle)
            Rectangle camBody = new Rectangle(camCX - 10, camCY - 10, 20, 20);
            using (SolidBrush b = new SolidBrush(_isDraggingFront ? Color.FromArgb(60, 80, 140) : Color.FromArgb(30, 45, 100)))
                g.FillRectangle(b, camBody);
            using (Pen p = new Pen(_isDraggingFront ? Color.FromArgb(140, 170, 255) : Color.FromArgb(80, 110, 200), _isDraggingFront ? 2.5f : 2f))
                g.DrawRectangle(p, camBody);

            // Camera offset X label
            if (Math.Abs(_cameraOffsetX) > 1)
            {
                string offLbl = (_cameraOffsetX >= 0 ? "+" : "") + _cameraOffsetX.ToString("F0") + "mm";
                using (Font f = new Font("Segoe UI", 7.5f))
                using (SolidBrush b = new SolidBrush(colDim))
                    g.DrawString(offLbl, f, b, camCX - 15, camCY + 14);
            }

            using (Font f = new Font("Segoe UI", 8.5f))
            using (SolidBrush b = new SolidBrush(colDim))
            {
                SizeF sz = g.MeasureString("Camera", f);
                g.DrawString("Camera", f, b, camCX - sz.Width / 2f, camCY - 28);
            }

            // ── Obstacle rectangle (on subject plane projection)
            if (_obstEnabled)
            {
                Rectangle or = GetObstFrontRect(W, floorY, scale);
                Color colObst = Color.FromArgb(240, 120, 30);
                bool hovObst  = _isDraggingObstFront || _isResizingObstFront;
                using (SolidBrush b = new SolidBrush(Color.FromArgb(hovObst ? 100 : 65, colObst.R, colObst.G, colObst.B)))
                    g.FillRectangle(b, or);
                using (Pen p = new Pen(colObst, hovObst ? 2.5f : 2f))
                    g.DrawRectangle(p, or);
                using (Font f = new Font("Segoe UI", 7.5f))
                using (SolidBrush b = new SolidBrush(colObst))
                {
                    string lbl = "장애물";
                    SizeF sz = g.MeasureString(lbl, f);
                    g.DrawString(lbl, f, b, or.Left + (or.Width - sz.Width) / 2f, or.Top - sz.Height - 2);
                }
                // Resize handle (bottom-right corner)
                Rectangle rh = GetObstFrontResizeHandle(W, floorY, scale);
                using (SolidBrush b = new SolidBrush(Color.FromArgb(200, colObst.R, colObst.G, colObst.B)))
                    g.FillRectangle(b, rh);
                using (Pen p = new Pen(Color.White, 1f))
                {
                    g.DrawLine(p, rh.Left + 3, rh.Bottom - 3, rh.Right - 3, rh.Bottom - 3);
                    g.DrawLine(p, rh.Right - 3, rh.Top + 3,  rh.Right - 3, rh.Bottom - 3);
                }
            }

            // ── Obstacle shadow overlay on subject/FOV plane
            if (_obstEnabled && _wd.HasValue)
            {
                // Auto-initialize if not yet placed (e.g. checkbox checked before WD was known)
                if (_obstDistFromCam <= 0 || _obstDistFromCam >= _wd.Value)
                    _obstDistFromCam = _wd.Value / 2.0;
            }
            if (_obstEnabled && _wd.HasValue && _obstDistFromCam > 0 && _obstDistFromCam < _wd.Value)
            {
                double projRatio = _wd.Value / _obstDistFromCam;

                // Vertical shadow bounds (world mm from floor)
                double shadowTopMm  = _cameraHeightMm + projRatio * (_obstHeightMm + _obstHalfH - _cameraHeightMm);
                double shadowBotMm  = _cameraHeightMm + projRatio * (_obstHeightMm - _obstHalfH - _cameraHeightMm);

                // Horizontal shadow bounds (world mm)
                double shadowLeftMm  = _cameraOffsetX + projRatio * (_obstOffsetX - _obstHalfW - _cameraOffsetX);
                double shadowRightMm = _cameraOffsetX + projRatio * (_obstOffsetX + _obstHalfW - _cameraOffsetX);

                // Convert to screen (subject center = W/2, floor = floorY, scale = scale)
                int shLeft  = W / 2 + (int)(shadowLeftMm  * scale);
                int shRight = W / 2 + (int)(shadowRightMm * scale);
                int shTop   = floorY - (int)(shadowTopMm  * scale);
                int shBot   = floorY - (int)(shadowBotMm  * scale);

                // Clamp to subject area (not FOV — shadow can extend beyond FOV)
                shLeft  = Math.Max(sx,           Math.Min(sx + subjectPxW, shLeft));
                shRight = Math.Max(sx,           Math.Min(sx + subjectPxW, shRight));
                shTop   = Math.Max(sy,           Math.Min(sy + subjectPxH, shTop));
                shBot   = Math.Max(sy,           Math.Min(sy + subjectPxH, shBot));

                if (shRight > shLeft && shBot > shTop)
                {
                    Color colGray = Color.FromArgb(150, 160, 165);
                    Color colBlind = Color.FromArgb(210, 80, 70);

                    // Full shadow area — gray
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(55, colGray.R, colGray.G, colGray.B)))
                        g.FillRectangle(b, shLeft, shTop, shRight - shLeft, shBot - shTop);
                    using (Pen p = new Pen(Color.FromArgb(140, colGray.R, colGray.G, colGray.B), 1.5f) { DashStyle = DashStyle.Dash })
                        g.DrawRectangle(p, shLeft, shTop, shRight - shLeft, shBot - shTop);
                    using (Font f = new Font("Segoe UI", 8f))
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(160, colGray.R, colGray.G, colGray.B)))
                    {
                        string lbl = "shadow";
                        SizeF sz = g.MeasureString(lbl, f);
                        float tx = (shLeft + shRight) / 2f - sz.Width / 2f;
                        float ty = (shTop + shBot) / 2f - sz.Height / 2f;
                        if (shBot - shTop > 16 && shRight - shLeft > 40)
                            g.DrawString(lbl, f, b, tx, ty);
                    }

                    // FOV 간섭 영역 — red
                    int isLeft  = Math.Max(shLeft,  fx);
                    int isRight = Math.Min(shRight, fx + fovPxW);
                    int isTop   = Math.Max(shTop,   fy);
                    int isBot   = Math.Min(shBot,   fy + fovPxH);
                    if (isRight > isLeft && isBot > isTop)
                    {
                        using (SolidBrush b = new SolidBrush(Color.FromArgb(110, colBlind.R, colBlind.G, colBlind.B)))
                            g.FillRectangle(b, isLeft, isTop, isRight - isLeft, isBot - isTop);
                        using (Pen p = new Pen(Color.FromArgb(220, colBlind.R, colBlind.G, colBlind.B), 1.5f))
                            g.DrawRectangle(p, isLeft, isTop, isRight - isLeft, isBot - isTop);
                        using (Font f = new Font("Segoe UI", 8f, FontStyle.Bold))
                        using (SolidBrush b = new SolidBrush(colBlind))
                        {
                            string lbl = "blind";
                            SizeF sz = g.MeasureString(lbl, f);
                            float tx = (isLeft + isRight) / 2f - sz.Width / 2f;
                            float ty = (isTop + isBot) / 2f - sz.Height / 2f;
                            if (isBot - isTop > 16 && isRight - isLeft > 30)
                                g.DrawString(lbl, f, b, tx, ty);
                        }
                    }
                }
            }

            // ── Dimension arrows
            using (Pen p = new Pen(colFov, 1.5f))
            using (SolidBrush b = new SolidBrush(colFov))
            using (Font f = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                int arrowY = floorY + 26;
                DrawDimArrow(g, p, b, f, fx, arrowY, fx + fovPxW,
                    "FOV X = " + fovX.ToString("F1") + " mm", colFov);
                DrawDimArrowV(g, p, b, f, Math.Min(fx, sx) - 36, fy, fy + fovPxH,
                    "FOV Y\n" + fovY.ToString("F1") + " mm", colFov, leftSide: true);
            }

            // ── Top-right info panel: obstacle position + blind area (FOV intersection)
            if (_obstEnabled && _wd.HasValue && _obstDistFromCam > 0 && _obstDistFromCam < _wd.Value)
            {
                double projRatio     = _wd.Value / _obstDistFromCam;
                double shadowTopMm   = _cameraHeightMm + projRatio * (_obstHeightMm + _obstHalfH - _cameraHeightMm);
                double shadowBotMm   = _cameraHeightMm + projRatio * (_obstHeightMm - _obstHalfH - _cameraHeightMm);
                double shadowLeftMm  = _cameraOffsetX  + projRatio * (_obstOffsetX - _obstHalfW - _cameraOffsetX);
                double shadowRightMm = _cameraOffsetX  + projRatio * (_obstOffsetX + _obstHalfW - _cameraOffsetX);

                // FOV bounds on subject plane (world mm)
                double fovLeft   = _cameraOffsetX  - fovX / 2;
                double fovRight  = _cameraOffsetX  + fovX / 2;
                double fovBottom = _cameraHeightMm - fovY / 2;
                double fovTop2   = _cameraHeightMm + fovY / 2;

                // Blind = shadow ∩ FOV
                double blindLeft  = Math.Max(shadowLeftMm,  fovLeft);
                double blindRight = Math.Min(shadowRightMm, fovRight);
                double blindBot   = Math.Max(shadowBotMm,   fovBottom);
                double blindTop2  = Math.Min(shadowTopMm,   fovTop2);
                bool   hasBlind   = blindRight > blindLeft && blindTop2 > blindBot;

                // X: camera-center relative, Y: from floor
                double obstDispX  = _obstOffsetX - _cameraOffsetX;
                double obstDispY  = _obstHeightMm;
                double blindDispX = blindLeft - _cameraOffsetX;
                double blindDispY = blindBot;
                double blindW     = blindRight - blindLeft;
                double blindH     = blindTop2  - blindBot;

                Color colObstVal  = Color.FromArgb(240, 120, 30);
                Color colBlindVal = Color.FromArgb(220, 70, 60);
                Color colHeader   = Color.FromArgb(200, 200, 210);
                Color colLabel    = Color.FromArgb(160, 170, 185);

                var lines = new (string label, string value, bool isTitle, bool isBlind)[]
                {
                    ("[ Obstacle ]",    "",                                          true,  false),
                    ("  Dist",          $"{_obstDistFromCam:F1} mm",                 false, false),
                    ("  X",             $"{obstDispX:+0.0;-0.0;0.0} mm",             false, false),
                    ("  Y",             $"{obstDispY:F1} mm",                        false, false),
                    ("",                "",                                          false, false),
                    ("[ Blind / FOV ]", "",                                          true,  true),
                    ("  X",             hasBlind ? $"{blindDispX:+0.0;-0.0;0.0} mm" : "-",  false, true),
                    ("  Y",             hasBlind ? $"{blindDispY:F1} mm"             : "-",  false, true),
                    ("  W",             hasBlind ? $"{blindW:F1} mm"                 : "-",  false, true),
                    ("  H",             hasBlind ? $"{blindH:F1} mm"                 : "-",  false, true),
                };

                using (Font fInfo  = new Font("Segoe UI", 8f))
                using (Font fTitle = new Font("Segoe UI", 8f, FontStyle.Bold))
                {
                    float lineH  = fInfo.GetHeight(g) + 1f;
                    float panelH = lines.Length * lineH + 10f;
                    float panelW = 160f;
                    float px     = W - panelW - 8f;
                    float py     = 8f;

                    using (SolidBrush bg = new SolidBrush(Color.FromArgb(55, 20, 25, 35)))
                        g.FillRectangle(bg, px - 4, py - 2, panelW, panelH);
                    using (Pen border = new Pen(Color.FromArgb(60, 200, 210, 220), 1f))
                        g.DrawRectangle(border, px - 4, py - 2, panelW, panelH);

                    float y0 = py + 3f;
                    foreach (var (label, value, isTitle, isBlind) in lines)
                    {
                        Color valColor = isBlind ? colBlindVal : colObstVal;
                        if (isTitle)
                        {
                            Color hdrColor = isBlind ? colBlindVal : colHeader;
                            using (SolidBrush b = new SolidBrush(hdrColor))
                                g.DrawString(label, fTitle, b, px, y0);
                        }
                        else if (label != "")
                        {
                            using (SolidBrush b = new SolidBrush(colLabel))
                                g.DrawString(label, fInfo, b, px, y0);
                            SizeF vsz = g.MeasureString(value, fInfo);
                            using (SolidBrush b = new SolidBrush(valColor))
                                g.DrawString(value, fInfo, b, px + panelW - vsz.Width - 8f, y0);
                        }
                        y0 += lineH;
                    }

                }
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
            lblCatalogInfo.ForeColor     = Color.FromArgb(100, 130, 160);
            lblCatalogInfo.BackColor     = Color.Transparent;
            lblLensCatalogInfo.ForeColor = Color.FromArgb(100, 130, 160);
            lblLensCatalogInfo.BackColor = Color.Transparent;

            StyleButton(btnCatalogEdit,       btnBg, accent, border);
            StyleButton(btnCatalogReload,     btnBg, accent, border);
            StyleButton(btnLensCatalogEdit,   btnBg, accent, border);
            StyleButton(btnLensCatalogReload, btnBg, accent, border);

            // Reverse input group
            grpRevInput.BackColor = panel;
            grpRevInput.ForeColor = accent;
            foreach (Control c in grpRevInput.Controls)
            {
                if (c is Label lbl)   { lbl.ForeColor = fg;     lbl.BackColor = panel; }
                if (c is TextBox txt) { txt.BackColor = inputBg; txt.ForeColor = Color.White; txt.BorderStyle = BorderStyle.FixedSingle; }
                if (c is Button btn)  StyleButton(btn, btnBg, accent, border);
            }
            // Search filter panel
            pnlSearchFilter.BackColor = Color.FromArgb(18, 24, 36);
            foreach (Control c in pnlSearchFilter.Controls)
            {
                if (c is Label  lbl) { lbl.ForeColor = fg; lbl.BackColor = Color.Transparent; }
                if (c is TextBox tb) { tb.BackColor = inputBg; tb.ForeColor = Color.White; tb.BorderStyle = BorderStyle.FixedSingle; }
                if (c is CheckBox cb) { cb.ForeColor = fg; cb.BackColor = Color.Transparent; }
            }
            txtMinPixCount.Enabled = false;  // disabled until checkbox is checked

            grpRevRes.ForeColor = Color.FromArgb(0, 220, 180); grpRevRes.BackColor = panel;
            lblRevResMm.ForeColor    = accent;                        lblRevResMm.BackColor    = panel;
            lblRevResUm.ForeColor    = Color.FromArgb(120, 160, 200); lblRevResUm.BackColor    = panel;
            lblRevResPx.BackColor    = panel;

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
                if (c is ComboBox cmb)
                {
                    cmb.BackColor = inputBg;
                    cmb.ForeColor = Color.White;
                    cmb.FlatStyle = FlatStyle.Flat;
                }
            }
            lblFovXResult.ForeColor = accent; lblFovXResult.BackColor = panel;
            lblFovYResult.ForeColor = accent; lblFovYResult.BackColor = panel;
            lblResResult.ForeColor  = Color.FromArgb(160, 200, 255); lblResResult.BackColor = panel;
            lblFovCompatible.BackColor = panel;

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
