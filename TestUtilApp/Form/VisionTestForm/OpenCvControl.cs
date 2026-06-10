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
        private enum CropInteractionMode { None, Drawing, Moving, Resizing }
        private enum CropHoverZone       { None, Body, Corner }

        private bool                 _cropMode;
        private CropInteractionMode  _dragMode  = CropInteractionMode.None;
        private CropHoverZone        _hoverZone = CropHoverZone.None;
        private System.Drawing.Point _dragPbStart;    // screen pt: Drawing 시작점
        private System.Drawing.Point _dragPbCurrent;  // screen pt: Drawing 현재점
        private System.Drawing.Point _dragImgOffset;  // image-space offset for Moving

        private const int ResizeCornerSize = 16;

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
                btnContour.Enabled   = true;
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

        private void btnContour_Click(object sender, EventArgs e)
        {
            algorithmListControl.AddAlgorithm(new ContourAlgorithm());
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            var algo = new CropAlgorithm();

            // 추가될 위치(마지막 스텝) 직전 스텝의 출력 크기를 기준으로 기본 rect 계산
            int refW = 0, refH = 0;
            int lastIdx = algorithmListControl.Algorithms.Count - 1;
            var prevBmp = algorithmListControl.GetPreviewBitmapAt(lastIdx);
            if (prevBmp != null)
            {
                refW = prevBmp.Width;
                refH = prevBmp.Height;
            }
            else if (DisplayMat != null && !DisplayMat.Empty())
            {
                refW = DisplayMat.Width;
                refH = DisplayMat.Height;
            }

            if (refW > 0 && refH > 0)
            {
                int w = refW / 2;
                int h = refH / 2;
                algo.X      = (refW - w) / 2;
                algo.Y      = (refH - h) / 2;
                algo.Width  = w;
                algo.Height = h;
            }

            algorithmListControl.AddAlgorithm(algo);
        }

        // ── Pipeline Execution ────────────────────────────────────

        private void RunPipeline()
        {
            var algorithms = algorithmListControl.Algorithms;

            bool hasSource = (_currentMat != null && !_currentMat.Empty()) ||
                             (algorithms.Count > 0 && algorithms[0] is AcquireAlgorithm aq && aq.HasSource);
            if (!hasSource) return;

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
                        int from   = algorithms[i].InputFromStep;
                        int srcIdx = (from >= 0 && from < i) ? from : i - 1;
                        // 비활성 노드는 결과가 null이므로 활성 노드 결과가 나올 때까지 거슬러 올라감
                        while (srcIdx > 0 && results[srcIdx] == null)
                            srcIdx--;
                        input = results[srcIdx];
                    }

                    if (!algorithms[i].IsEnabled)
                    {
                        // 바이패스: 이전 결과를 그대로 통과 (해당 스텝은 실행 안 함)
                        results[i] = null;
                        bitmaps.Add(null);
                        continue;
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
            if (!(algorithmListControl.SelectedAlgorithm is CropAlgorithm))
            {
                _cropMode  = false;
                _dragMode  = CropInteractionMode.None;
                _hoverZone = CropHoverZone.None;
                pictureBoxMain.Cursor = Cursors.Default;
            }
            RefreshMainDisplay();
        }

        private void EnterCropMode(CropAlgorithm cropAlgo)
        {
            _cropMode  = true;
            _dragMode  = CropInteractionMode.None;
            _hoverZone = CropHoverZone.None;
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

        private Rectangle GetCurrentCropScreenRect()
        {
            var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
            return cropAlgo != null
                ? ImageRectToScreen(cropAlgo.X, cropAlgo.Y, cropAlgo.Width, cropAlgo.Height)
                : Rectangle.Empty;
        }

        private CropHoverZone GetHoverZone(System.Drawing.Point pt)
        {
            Rectangle rect = GetCurrentCropScreenRect();
            if (rect.IsEmpty || !_cropMode) return CropHoverZone.None;
            var corner = new Rectangle(rect.Right - ResizeCornerSize, rect.Bottom - ResizeCornerSize,
                                       ResizeCornerSize, ResizeCornerSize);
            if (corner.Contains(pt)) return CropHoverZone.Corner;
            if (rect.Contains(pt))   return CropHoverZone.Body;
            return CropHoverZone.None;
        }

        private void UpdateCursorForZone(CropHoverZone zone)
        {
            if (!_cropMode) return;
            pictureBoxMain.Cursor = zone == CropHoverZone.Corner ? Cursors.SizeNWSE
                                  : zone == CropHoverZone.Body   ? Cursors.SizeAll
                                  : Cursors.Cross;
        }

        private void pictureBoxMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_cropMode || e.Button != MouseButtons.Left) return;

            var zone = GetHoverZone(e.Location);
            if (zone == CropHoverZone.Corner)
            {
                _dragMode = CropInteractionMode.Resizing;
            }
            else if (zone == CropHoverZone.Body)
            {
                _dragMode = CropInteractionMode.Moving;
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo != null)
                {
                    var imgMouse = ScreenToImagePoint(e.Location);
                    _dragImgOffset = new System.Drawing.Point(imgMouse.X - cropAlgo.X,
                                                              imgMouse.Y - cropAlgo.Y);
                }
            }
            else
            {
                _dragMode      = CropInteractionMode.Drawing;
                _dragPbStart   = e.Location;
                _dragPbCurrent = e.Location;
            }
        }

        private void pictureBoxMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragMode == CropInteractionMode.Drawing)
            {
                _dragPbCurrent = e.Location;
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo != null)
                {
                    var imgA = ScreenToImagePoint(_dragPbStart);
                    var imgB = ScreenToImagePoint(_dragPbCurrent);
                    cropAlgo.X      = Math.Min(imgA.X, imgB.X);
                    cropAlgo.Y      = Math.Min(imgA.Y, imgB.Y);
                    cropAlgo.Width  = Math.Max(1, Math.Abs(imgA.X - imgB.X));
                    cropAlgo.Height = Math.Max(1, Math.Abs(imgA.Y - imgB.Y));
                    algorithmListControl.RefreshCurrentSettingsPanel();
                }
                pictureBoxMain.Invalidate();
            }
            else if (_dragMode == CropInteractionMode.Moving)
            {
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo != null)
                {
                    var imgMouse = ScreenToImagePoint(e.Location);
                    var bmp = pictureBoxMain.Image;
                    cropAlgo.X = bmp != null
                        ? Math.Max(0, Math.Min(bmp.Width  - cropAlgo.Width,  imgMouse.X - _dragImgOffset.X))
                        : imgMouse.X - _dragImgOffset.X;
                    cropAlgo.Y = bmp != null
                        ? Math.Max(0, Math.Min(bmp.Height - cropAlgo.Height, imgMouse.Y - _dragImgOffset.Y))
                        : imgMouse.Y - _dragImgOffset.Y;
                    algorithmListControl.RefreshCurrentSettingsPanel();
                    pictureBoxMain.Invalidate();
                }
            }
            else if (_dragMode == CropInteractionMode.Resizing)
            {
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo != null)
                {
                    var imgMouse = ScreenToImagePoint(e.Location);
                    var bmp = pictureBoxMain.Image;
                    cropAlgo.Width  = bmp != null
                        ? Math.Max(1, Math.Min(bmp.Width  - cropAlgo.X, imgMouse.X - cropAlgo.X))
                        : Math.Max(1, imgMouse.X - cropAlgo.X);
                    cropAlgo.Height = bmp != null
                        ? Math.Max(1, Math.Min(bmp.Height - cropAlgo.Y, imgMouse.Y - cropAlgo.Y))
                        : Math.Max(1, imgMouse.Y - cropAlgo.Y);
                    algorithmListControl.RefreshCurrentSettingsPanel();
                    pictureBoxMain.Invalidate();
                }
            }
            else if (_cropMode)
            {
                var newZone = GetHoverZone(e.Location);
                if (newZone != _hoverZone)
                {
                    _hoverZone = newZone;
                    UpdateCursorForZone(_hoverZone);
                    pictureBoxMain.Invalidate();
                }
            }

            UpdatePixelInfo(e.Location);
        }

        private void pictureBoxMain_MouseLeave(object sender, EventArgs e)
        {
            tslPixel.Text  = "";
            tsSep3.Visible = false;
            if (_hoverZone != CropHoverZone.None)
            {
                _hoverZone = CropHoverZone.None;
                UpdateCursorForZone(CropHoverZone.None);
                pictureBoxMain.Invalidate();
            }
        }

        private void pictureBoxMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragMode == CropInteractionMode.None || e.Button != MouseButtons.Left) return;

            _dragMode = CropInteractionMode.None;

            var zone = GetHoverZone(e.Location);
            _hoverZone = zone;
            UpdateCursorForZone(zone);

            algorithmListControl.RefreshCurrentSettingsPanel();
            pictureBoxMain.Invalidate();
            RunPipeline();
        }

        // ── Paint overlay ─────────────────────────────────────────

        private void pictureBoxMain_Paint(object sender, PaintEventArgs e)
        {
            if (!_cropMode) return;

            Rectangle rect;
            int imgX, imgY, imgW, imgH;

            if (_dragMode == CropInteractionMode.Drawing)
            {
                rect = NormalizeRect(_dragPbStart, _dragPbCurrent);
                var imgA = ScreenToImagePoint(_dragPbStart);
                var imgB = ScreenToImagePoint(_dragPbCurrent);
                imgX = Math.Min(imgA.X, imgB.X);
                imgY = Math.Min(imgA.Y, imgB.Y);
                imgW = Math.Max(1, Math.Abs(imgA.X - imgB.X));
                imgH = Math.Max(1, Math.Abs(imgA.Y - imgB.Y));
            }
            else
            {
                var cropAlgo = algorithmListControl.SelectedAlgorithm as CropAlgorithm;
                if (cropAlgo == null) return;
                rect = ImageRectToScreen(cropAlgo.X, cropAlgo.Y, cropAlgo.Width, cropAlgo.Height);
                imgX = cropAlgo.X;
                imgY = cropAlgo.Y;
                imgW = cropAlgo.Width;
                imgH = cropAlgo.Height;
            }

            if (rect.Width <= 0 || rect.Height <= 0) return;

            var g   = e.Graphics;
            int pbW = pictureBoxMain.ClientSize.Width;
            int pbH = pictureBoxMain.ClientSize.Height;

            // 바깥 영역 반투명 회색 마스크
            using (var mask = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(140, 30, 30, 30)))
            {
                g.FillRectangle(mask, 0, 0, pbW, rect.Top);
                g.FillRectangle(mask, 0, rect.Bottom, pbW, pbH - rect.Bottom);
                g.FillRectangle(mask, 0, rect.Top, rect.Left, rect.Height);
                g.FillRectangle(mask, rect.Right, rect.Top, pbW - rect.Right, rect.Height);
            }

            using (var fill = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(30, 255, 140, 0)))
                g.FillRectangle(fill, rect);
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 255, 140, 0), 4.5f)
                { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                g.DrawRectangle(pen, rect);

            // 좌표 정보 (우측 상단)
            string info = $"x:{imgX}  y:{imgY}  w:{imgW}  h:{imgH}";
            using (var font = new System.Drawing.Font("Consolas", 8.5f, System.Drawing.FontStyle.Bold))
            {
                var sz      = g.MeasureString(info, font);
                int padding = 4;
                float tx = rect.Right - sz.Width - padding;
                float ty = rect.Top - sz.Height - padding;
                if (ty < 0) ty = rect.Top + padding;
                if (tx < 0) tx = rect.Left + padding;
                using (var bg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(175, 30, 20, 0)))
                    g.FillRectangle(bg, tx - padding, ty - 1, sz.Width + padding * 2, sz.Height + 2);
                using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan))
                    g.DrawString(info, font, brush, tx, ty);
            }

            // 리사이즈 핸들 (우측 하단)
            bool cornerActive = _hoverZone == CropHoverZone.Corner || _dragMode == CropInteractionMode.Resizing;
            var handleRect = new Rectangle(rect.Right - ResizeCornerSize, rect.Bottom - ResizeCornerSize,
                                           ResizeCornerSize, ResizeCornerSize);
            using (var hb = new System.Drawing.SolidBrush(cornerActive
                ? System.Drawing.Color.FromArgb(230, 255, 200, 60)
                : System.Drawing.Color.FromArgb(150, 255, 160, 40)))
                g.FillRectangle(hb, handleRect);
            using (var hp = new System.Drawing.Pen(System.Drawing.Color.FromArgb(200, 60, 50, 0), 1.5f))
            {
                int rx = handleRect.Right - 4, ry = handleRect.Bottom - 4;
                g.DrawLine(hp, rx - 8, ry, rx, ry);
                g.DrawLine(hp, rx, ry - 8, rx, ry);
                g.DrawLine(hp, rx - 4, ry - 2, rx, ry - 8);
                g.DrawLine(hp, rx - 2, ry - 4, rx - 8, ry);
            }

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

        private Rectangle GetImageDisplayRect()
        {
            var img = pictureBoxMain.Image;
            if (img == null) return Rectangle.Empty;

            if (pictureBoxMain.SizeMode == PictureBoxSizeMode.Zoom)
            {
                float scale = Math.Min((float)pictureBoxMain.Width  / img.Width,
                                       (float)pictureBoxMain.Height / img.Height);
                int dispW = (int)(img.Width  * scale);
                int dispH = (int)(img.Height * scale);
                int offX  = (pictureBoxMain.Width  - dispW) / 2;
                int offY  = (pictureBoxMain.Height - dispH) / 2;
                return new Rectangle(offX, offY, dispW, dispH);
            }
            return new Rectangle(0, 0,
                (int)(img.Width  * _zoomFactor),
                (int)(img.Height * _zoomFactor));
        }

        private void UpdatePixelInfo(System.Drawing.Point pbPt)
        {
            var bmp = pictureBoxMain.Image as Bitmap;
            if (bmp == null)
            {
                tslPixel.Text  = "";
                tsSep3.Visible = false;
                return;
            }

            // Only show info when cursor is over the actual image, not the letterbox
            if (!GetImageDisplayRect().Contains(pbPt))
            {
                tslPixel.Text  = "";
                tsSep3.Visible = false;
                return;
            }

            var imgPt = ScreenToImagePoint(pbPt);
            if (imgPt.X < 0 || imgPt.Y < 0 || imgPt.X >= bmp.Width || imgPt.Y >= bmp.Height)
            {
                tslPixel.Text  = "";
                tsSep3.Visible = false;
                return;
            }

            var c = bmp.GetPixel(imgPt.X, imgPt.Y);

            string colorStr = (c.R == c.G && c.G == c.B)
                ? $"Gray: {c.R}"
                : $"R: {c.R,3}  G: {c.G,3}  B: {c.B,3}";

            tslPixel.ForeColor = System.Drawing.Color.FromArgb(100, 210, 255);
            tslPixel.Text      = $"X: {imgPt.X,5}  Y: {imgPt.Y,5}    {colorStr}";
            tsSep3.Visible     = true;
        }

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
