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
        private Mat _currentMat;
        private Mat _resultMat;
        private string _currentFilePath;
        private float _zoomFactor = 1.0f;

        private Mat DisplayMat => _resultMat ?? _currentMat;

        public OpenCvControl()
        {
            InitializeComponent();
            algorithmListControl.RunRequested    += (s, e) => RunPipeline();
            algorithmListControl.SettingsApplied += (s, e) => RunPipeline();
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

                algorithmListControl.Clear();
                btnThreshold.Enabled = true;
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

            try
            {
                var intermediateBitmaps = new List<Bitmap>();
                Mat current  = _currentMat;
                bool ownCurrent = false;

                foreach (var algo in algorithms)
                {
                    Mat next = algo.Execute(current);
                    if (ownCurrent) current.Dispose();
                    if (next == null || next.Empty()) break;
                    current     = next;
                    ownCurrent  = true;

                    // Capture intermediate result bitmap for this step
                    intermediateBitmaps.Add(BitmapConverter.ToBitmap(current));
                }

                _resultMat?.Dispose();
                _resultMat = ownCurrent ? current : null;

                // Transfer ownership of bitmaps to the algorithm list control
                algorithmListControl.SetPreviewBitmaps(intermediateBitmaps);

                _zoomFactor = 1.0f;
                FitToWindow();
                UpdateStatusInfo();
            }
            catch (Exception ex)
            {
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
