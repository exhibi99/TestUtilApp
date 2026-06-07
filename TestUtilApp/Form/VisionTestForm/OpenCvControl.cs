using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace TestUtilApp.UI
{
    public partial class OpenCvControl : UserControl, IActivatable
    {
        private Mat              _currentMat;
        private Mat              _resultMat;
        private string           _currentFilePath;
        private float            _zoomFactor = 1.0f;
        private AcquireAlgorithm _acquireAlgo;

        // ── Crop drag state ───────────────────────────────────────
        private bool                 _cropMode;
        private bool                 _cropDragging;
        private System.Drawing.Point _dragPbStart;
        private System.Drawing.Point _dragPbCurrent;

        private Mat DisplayMat => _resultMat ?? _currentMat;

        public OpenCvControl()
        {
            InitializeComponent();
            algorithmListControl.RunRequested           += (s, e) => RunPipeline();
            algorithmListControl.SettingsApplied        += (s, e) => RunPipeline();
            algorithmListControl.SelectedAlgorithmChanged += OnSelectedAlgorithmChanged;
        }

        public void OnActivated() { }

        // ── Acquire ──────────────────────────────────────────────

        private void btnAcquire_Click(object sender, EventArgs e)
        {
            using (var dlg = new AcquireDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                LoadImage(dlg.ImagePath, dlg.LoadAsGray);
            }
        }

        private void LoadImage(string filePath, bool asGray)
        {
            try
            {
                var mode = asGray ? ImreadModes.Grayscale : ImreadModes.Color;
                Mat mat = Cv2.ImRead(filePath, mode);

                if (mat == null || mat.Empty())
                {
                    MessageBox.Show("Failed to load image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DisposeMats();
                _currentMat      = mat;
                _currentFilePath = filePath;
                _zoomFactor      = 1.0f;

                // Acquire 알고리즘을 파이프라인 첫 번째 스텝으로 추가
                algorithmListControl.Clear();
                _acquireAlgo = new AcquireAlgorithm();
                _acquireAlgo.SetSource(_currentMat, filePath);
                algorithmListControl.AddAlgorithm(_acquireAlgo);

                btnThreshold.Enabled = true;
                btnCrop.Enabled      = true;
                tsbSave.Enabled      = true;
                FitToWindow();
                UpdateStatusInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Image load error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Algorithm Buttons ─────────────────────────────────────

        private void btnThreshold_Click(object sender, EventArgs e)
        {
            algorithmListControl.AddAlgorithm(new ThresholdAlgorithm());
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            var algo = new CropAlgorithm();
            // 현재 이미지 크기를 기본값으로 설정
            if (_currentMat != null && !_currentMat.Empty())
            {
                algo.Width  = _currentMat.Width;
                algo.Height = _currentMat.Height;
            }
            algorithmListControl.AddAlgorithm(algo);
        }

        // ── Pipeline Execution ────────────────────────────────────

        private void RunPipeline()
        {
            if (_currentMat == null || _currentMat.Empty()) return;

            var algorithms = algorithmListControl.Algorithms;

            if (algorithms.Count == 0)
            {
                _resultMat?.Dispose();
                _resultMat = null;
                algorithmListControl.SetPreviewBitmaps(new List<Bitmap>());
                _zoomFactor = 1.0f;
                FitToWindow();
                UpdateStatusInfo();
                return;
            }

            var results = new Mat[algorithms.Count];
            var bitmaps = new List<Bitmap>();

            try
            {
                for (int i = 0; i < algorithms.Count; i++)
                {
                    // 이전 스텝 결정: InputFromStep이 유효한 값이면 해당 스텝, 아니면 바로 앞 스텝
                    Mat input = null;
                    if (i > 0)
                    {
                        int from = algorithms[i].InputFromStep;
                        int srcIdx = (from >= 0 && from < i) ? from : i - 1;
                        input = results[srcIdx];
                    }

                    results[i] = algorithms[i].Execute(input);
                    bitmaps.Add(results[i] != null && !results[i].Empty()
                        ? BitmapConverter.ToBitmap(results[i])
                        : null);
                }

                // 마지막으로 유효한 결과를 _resultMat에 저장
                _resultMat?.Dispose();
                _resultMat = null;
                for (int i = results.Length - 1; i >= 0; i--)
                {
                    if (results[i] != null && !results[i].Empty())
                    {
                        _resultMat = results[i].Clone();
                        break;
                    }
                }

                foreach (var r in results) r?.Dispose();

                algorithmListControl.SetPreviewBitmaps(bitmaps);
                _zoomFactor = 1.0f;
                RefreshMainDisplay();
                UpdateStatusInfo();
            }
            catch (Exception ex)
            {
                foreach (var r in results) r?.Dispose();
                foreach (var b in bitmaps) b?.Dispose();
                MessageBox.Show($"Pipeline error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Zoom / Fit ────────────────────────────────────────────

        private void FitToWindow()
        {
            if (DisplayMat == null || DisplayMat.Empty()) return;
            _zoomFactor = 1.0f;
            var bmp = BitmapConverter.ToBitmap(DisplayMat);
            ApplyPictureBox(bmp, DockStyle.Fill, PictureBoxSizeMode.Zoom);
        }

        private void tsbFit_Click(object sender, EventArgs e)    => FitToWindow();
        private void tsbZoomIn_Click(object sender, EventArgs e)  => ApplyZoom(1.25f);
        private void tsbZoomOut_Click(object sender, EventArgs e) => ApplyZoom(1.0f / 1.25f);

        private void ApplyZoom(float factor)
        {
            if (DisplayMat == null || DisplayMat.Empty()) return;

            _zoomFactor = Math.Max(0.05f, Math.Min(20.0f, _zoomFactor * factor));
            int newW = Math.Max(1, (int)(DisplayMat.Width  * _zoomFactor));
            int newH = Math.Max(1, (int)(DisplayMat.Height * _zoomFactor));

            using (var resized = DisplayMat.Resize(new OpenCvSharp.Size(newW, newH)))
            {
                var bmp = BitmapConverter.ToBitmap(resized);
                ApplyPictureBox(bmp, DockStyle.None, PictureBoxSizeMode.AutoSize);
                pictureBoxMain.Location = new System.Drawing.Point(0, 0);
            }
        }

        private void ApplyPictureBox(Bitmap bmp, DockStyle dock, PictureBoxSizeMode sizeMode)
        {
            pictureBoxMain.Dock     = dock;
            pictureBoxMain.SizeMode = sizeMode;
            var old = pictureBoxMain.Image;
            pictureBoxMain.Image = bmp;
            old?.Dispose();
        }

        // ── Save ──────────────────────────────────────────────────

        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (DisplayMat == null || DisplayMat.Empty()) return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter     = "PNG Files|*.png|JPEG Files|*.jpg|BMP Files|*.bmp|All Files|*.*";
                dlg.Title      = "Save Image";
                dlg.DefaultExt = "png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try { Cv2.ImWrite(dlg.FileName, DisplayMat); }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Save error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // ── Crop drag ─────────────────────────────────────────────

        private void OnSelectedAlgorithmChanged(object sender, EventArgs e)
        {
            // Crop이 아니면 드래그 상태만 초기화
            if (!(algorithmListControl.SelectedAlgorithm is CropAlgorithm))
            {
                _cropMode     = false;
                _cropDragging = false;
                pictureBoxMain.Cursor = Cursors.Default;
            }

            RefreshMainDisplay();
        }

        private void EnterCropMode(CropAlgorithm cropAlgo)
        {
            _cropMode = true;
            pictureBoxMain.Cursor = Cursors.Cross;

            // 이 Crop 스텝의 입력 = 이전 스텝의 출력
            int cropIdx  = algorithmListControl.SelectedIndex;
            int from     = cropAlgo.InputFromStep;
            int inputIdx = (from >= 0 && from < cropIdx) ? from : cropIdx - 1;

            Bitmap inputBmp = algorithmListControl.GetPreviewBitmapAt(inputIdx);
            if (inputBmp != null)
            {
                // 빌려온 비트맵의 복사본을 만들어 pictureBox에 표시
                ApplyPictureBox((Bitmap)inputBmp.Clone(), DockStyle.Fill, PictureBoxSizeMode.Zoom);
            }
            else if (_currentMat != null && !_currentMat.Empty())
            {
                ApplyPictureBox(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(_currentMat),
                    DockStyle.Fill, PictureBoxSizeMode.Zoom);
            }

            pictureBoxMain.Invalidate();
        }

        /// <summary>
        /// 선택된 스텝의 출력을 메인 뷰에 표시한다.
        /// Crop 선택 시 → 입력 이미지 표시 + 드래그 모드.
        /// 그 외 스텝 선택 시 → 해당 스텝의 미리보기 비트맵 표시.
        /// 선택 없음 / 미리보기 없음 → 최종 결과(DisplayMat) 표시.
        /// </summary>
        private void RefreshMainDisplay()
        {
            if (algorithmListControl.SelectedAlgorithm is CropAlgorithm cropAlgo)
            {
                EnterCropMode(cropAlgo);
                return;
            }

            int idx     = algorithmListControl.SelectedIndex;
            Bitmap prev = algorithmListControl.GetPreviewBitmapAt(idx);

            if (prev != null)
                ApplyPictureBox((Bitmap)prev.Clone(), DockStyle.Fill, PictureBoxSizeMode.Zoom);
            else if (DisplayMat != null && !DisplayMat.Empty())
                FitToWindow();
            else
            {
                var old = pictureBoxMain.Image;
                pictureBoxMain.Image = null;
                old?.Dispose();
            }
        }

        // ── PictureBox mouse events ───────────────────────────────

        private void pictureBoxMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_cropMode || e.Button != MouseButtons.Left) return;
            _cropDragging  = true;
            _dragPbStart   = e.Location;
            _dragPbCurrent = e.Location;
        }

        private void pictureBoxMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_cropDragging) return;
            _dragPbCurrent = e.Location;
            pictureBoxMain.Invalidate();
        }

        private void pictureBoxMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_cropDragging || e.Button != MouseButtons.Left) return;
            _cropDragging  = false;
            _dragPbCurrent = e.Location;

            var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
            if (cropAlgo == null) return;

            System.Drawing.Point imgA = ScreenToImagePoint(_dragPbStart);
            System.Drawing.Point imgB = ScreenToImagePoint(_dragPbCurrent);

            cropAlgo.X      = Math.Min(imgA.X, imgB.X);
            cropAlgo.Y      = Math.Min(imgA.Y, imgB.Y);
            cropAlgo.Width  = Math.Max(1, Math.Abs(imgA.X - imgB.X));
            cropAlgo.Height = Math.Max(1, Math.Abs(imgA.Y - imgB.Y));

            algorithmListControl.RefreshCurrentSettingsPanel();
            pictureBoxMain.Invalidate();
            RunPipeline();
        }

        // ── Paint overlay ─────────────────────────────────────────

        private void pictureBoxMain_Paint(object sender, PaintEventArgs e)
        {
            if (!_cropMode) return;

            Rectangle rect;
            if (_cropDragging)
            {
                rect = NormalizeRect(_dragPbStart, _dragPbCurrent);
            }
            else
            {
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo == null) return;
                rect = ImageRectToScreen(cropAlgo.X, cropAlgo.Y, cropAlgo.Width, cropAlgo.Height);
            }

            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var fill = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(50, 0, 200, 255)))
                e.Graphics.FillRectangle(fill, rect);
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1.5f)
                { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                e.Graphics.DrawRectangle(pen, rect);
        }

        // ── Coordinate conversion ─────────────────────────────────

        private System.Drawing.Point ScreenToImagePoint(System.Drawing.Point pb)
        {
            var img = pictureBoxMain.Image;
            if (img == null) return pb;

            if (pictureBoxMain.SizeMode == PictureBoxSizeMode.Zoom)
            {
                float scale = Math.Min((float)pictureBoxMain.Width / img.Width,
                                       (float)pictureBoxMain.Height / img.Height);
                float offX  = (pictureBoxMain.Width  - img.Width  * scale) / 2f;
                float offY  = (pictureBoxMain.Height - img.Height * scale) / 2f;
                int x = (int)((pb.X - offX) / scale);
                int y = (int)((pb.Y - offY) / scale);
                return new System.Drawing.Point(Math.Max(0, Math.Min(img.Width  - 1, x)),
                                                Math.Max(0, Math.Min(img.Height - 1, y)));
            }
            else
            {
                return new System.Drawing.Point(
                    Math.Max(0, Math.Min(img.Width  - 1, (int)(pb.X / _zoomFactor))),
                    Math.Max(0, Math.Min(img.Height - 1, (int)(pb.Y / _zoomFactor))));
            }
        }

        private Rectangle ImageRectToScreen(int x, int y, int w, int h)
        {
            var img = pictureBoxMain.Image;
            if (img == null) return Rectangle.Empty;

            if (pictureBoxMain.SizeMode == PictureBoxSizeMode.Zoom)
            {
                float scale = Math.Min((float)pictureBoxMain.Width / img.Width,
                                       (float)pictureBoxMain.Height / img.Height);
                float offX  = (pictureBoxMain.Width  - img.Width  * scale) / 2f;
                float offY  = (pictureBoxMain.Height - img.Height * scale) / 2f;
                return new Rectangle((int)(offX + x * scale), (int)(offY + y * scale),
                                     (int)(w * scale), (int)(h * scale));
            }
            else
            {
                return new Rectangle((int)(x * _zoomFactor), (int)(y * _zoomFactor),
                                     (int)(w * _zoomFactor), (int)(h * _zoomFactor));
            }
        }

        private static Rectangle NormalizeRect(System.Drawing.Point a, System.Drawing.Point b) =>
            new Rectangle(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y),
                          Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));

        // ── Helpers ───────────────────────────────────────────────

        private void UpdateStatusInfo()
        {
            if (_currentMat == null) return;

            Mat    display = DisplayMat;
            string name    = Path.GetFileName(_currentFilePath);
            string ch      = display.Channels() == 1 ? "Gray" : "Color";
            string info    = $"{name}  |  {display.Width} \u00d7 {display.Height}  |  {ch}";

            int count = algorithmListControl.Algorithms.Count;
            if (_resultMat != null && count > 0)
                info += $"  |  Pipeline [{count} step{(count > 1 ? "s" : "")}]";

            tslInfo.Text = info;
        }

        private void DisposeMats()
        {
            _resultMat?.Dispose();
            _resultMat = null;
            _currentMat?.Dispose();
            _currentMat = null;
            _currentFilePath = null;

            var img = pictureBoxMain?.Image;
            if (img != null)
            {
                pictureBoxMain.Image = null;
                img.Dispose();
            }
        }
    }
}
