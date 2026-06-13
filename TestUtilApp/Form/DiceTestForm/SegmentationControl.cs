using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;
using TestUtilApp.UI;

namespace TestUtilApp.UI
{
    public partial class SegmentationControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private List<SegmentationResult> _results;
        private bool _isProcessing;

        public SegmentationControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;
            _results = new List<SegmentationResult>();

            InitializeListView();
        }

        public void OnActivated()
        {
            AppendLog("Segmentation screen has been activated.");

            if (!string.IsNullOrEmpty(_config.LastSegmentSourceFolder))
            {
                txtSourceFolder.Text = _config.LastSegmentSourceFolder;
            }

            if (!string.IsNullOrEmpty(_config.DiceModels?.SegmentModel?.Path))
            {
                txtModelPath.Text = _config.DiceModels.SegmentModel.Path;
            }

            RefreshModelStatusLabel();
        }

        private void RefreshModelStatusLabel()
        {
            string modelPath = txtModelPath.Text;
            bool loaded = DiceManager.IsSegmentModelLoaded(modelPath);

            lblModelStatus.Text = loaded ? "✓ Loaded" : "✗ Not Loaded";
            lblModelStatus.ForeColor = loaded ? UiTheme.Success : UiTheme.Error;
        }

        private void InitializeListView()
        {
            listViewResults.View = View.Details;
            listViewResults.FullRowSelect = true;
            listViewResults.GridLines = true;
            listViewResults.MultiSelect = false;

            listViewResults.Columns.Add("Filename", 220);
            listViewResults.Columns.Add("Path", 300);
            listViewResults.Columns.Add("Size", 100);
            listViewResults.Columns.Add("Status", 120);
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            string selected = FolderPickerDialog.Show(this, "Select the folder containing images to segment.", txtSourceFolder.Text);
            if (selected != null)
            {
                txtSourceFolder.Text = selected;
                _config.LastSegmentSourceFolder = selected;
                _configService.SaveConfig(_config);
            }
        }

        private async void btnLoadModel_Click(object sender, EventArgs e)
        {
            string modelPath = txtModelPath.Text;
            if (string.IsNullOrEmpty(modelPath) || !Directory.Exists(modelPath))
            {
                MessageBox.Show("Please select a model folder first using [Browse].", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLoadModel.Enabled = false;
            try
            {
                bool isLoaded = await EnsureSegmentModelLoadedAsync(modelPath);
                if (isLoaded)
                {
                    AppendLog("Segmentation model loaded successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to load the segmentation model.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnLoadModel.Enabled = true;
                RefreshModelStatusLabel();
            }
        }

        private void btnBrowseModel_Click(object sender, EventArgs e)
        {
            string selected = FolderPickerDialog.Show(this, "Select the segmentation model folder.", txtModelPath.Text);
            if (selected != null)
            {
                txtModelPath.Text = selected;

                if (_config.DiceModels == null)
                {
                    _config.DiceModels = new DiceModelsConfig();
                }

                if (_config.DiceModels.SegmentModel == null)
                {
                    _config.DiceModels.SegmentModel = new DiceModelSetting();
                }

                _config.DiceModels.SegmentModel.Path = selected;
                _config.DiceModels.SegmentModel.Use = true;
                _configService.SaveConfig(_config);
            }
        }

        private async void btnStartSegmentation_Click(object sender, EventArgs e)
        {
            if (_isProcessing)
            {
                MessageBox.Show("Segmentation is already in progress.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtSourceFolder.Text) || !Directory.Exists(txtSourceFolder.Text))
            {
                MessageBox.Show("Please select a valid source folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modelPath = txtModelPath.Text;
            if (string.IsNullOrEmpty(modelPath) || !Directory.Exists(modelPath))
            {
                MessageBox.Show("Please select a valid segmentation model folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isProcessing = true;
            btnStartSegmentation.Enabled = false;
            btnClearResults.Enabled = false;
            progressBar.Value = 0;

            try
            {
                bool isModelReady = await EnsureSegmentModelLoadedAsync(modelPath);
                RefreshModelStatusLabel();
                if (!isModelReady)
                {
                    throw new InvalidOperationException("Segmentation model could not be loaded.");
                }

                await Task.Run(() => ProcessSegmentation());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during segmentation: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppendLog($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                btnStartSegmentation.Enabled = true;
                btnClearResults.Enabled = true;
            }
        }

        private async Task<bool> EnsureSegmentModelLoadedAsync(string modelPath)
        {
            if (DiceManager.IsSegmentModelLoaded(modelPath))
            {
                return true;
            }

            AppendLog("Loading segmentation model...");

            using (var loadingDialog = ModelLoadingDialog.ShowFor(
                this,
                "Loading Segmentation model...\r\nSegmentation will start when loading is complete."))
            {
                bool isLoaded = await Task.Run(() =>
                {
                    if (!DiceManager.EnsureInitialized())
                    {
                        return false;
                    }

                    return DiceManager.EnsureSegmentModelLoaded(modelPath);
                });

                loadingDialog.CloseSafely();

                if (isLoaded)
                {
                    AppendLog("Segmentation model is ready.");
                }
                else
                {
                    AppendLog("Segmentation model load failed.");
                }

                return isLoaded;
            }
        }

        private void ProcessSegmentation()
        {
            string sourceFolder = txtSourceFolder.Text;

            var imageFiles = GetImageFiles(sourceFolder);

            Invoke(new Action(() =>
            {
                AppendLog($"Found {imageFiles.Count} image file(s).");

                if (imageFiles.Count == 0)
                {
                    MessageBox.Show("There are no images to process.", "Notice",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _results.Clear();
                listViewResults.Items.Clear();
                pictureBoxPreview.Image = null;
                progressBar.Maximum = imageFiles.Count;
                AppendLog($"Starting segmentation for {imageFiles.Count} image(s)...");
            }));

            if (imageFiles.Count == 0)
            {
                return;
            }

            int processedCount = 0;
            int successCount = 0;
            int failCount = 0;
            long totalInferenceMs = 0;

            var swTotal = Stopwatch.StartNew();

            foreach (var imagePath in imageFiles)
            {
                try
                {
                    var result = SegmentImage(imagePath, out long inferenceMs);
                    totalInferenceMs += inferenceMs;

                    Invoke(new Action(() =>
                    {
                        _results.Add(result);
                        AddResultToListView(result);
                        progressBar.Value = ++processedCount;
                        lblProgress.Text = $"{processedCount} / {imageFiles.Count}";

                        if (result.IsSuccess)
                        {
                            successCount++;
                            AppendLog($"[{result.FileName}] inference: {inferenceMs}ms");
                        }
                        else
                        {
                            failCount++;
                        }

                        if (processedCount == imageFiles.Count && result.ResultImage != null)
                        {
                            UpdatePreviewImage(result);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        AppendLog($"Error [{Path.GetFileName(imagePath)}]: {ex.Message}");
                        failCount++;
                        progressBar.Value = ++processedCount;
                        lblProgress.Text = $"{processedCount} / {imageFiles.Count}";
                    }));
                }
            }

            swTotal.Stop();
            long avgMs = successCount > 0 ? totalInferenceMs / successCount : 0;

            Invoke(new Action(() =>
            {
                AppendLog($"--- Segmentation complete ---");
                AppendLog($"  Success: {successCount}, Failed: {failCount}");
                AppendLog($"  Total inference: {totalInferenceMs}ms / Avg per image: {avgMs}ms");
                AppendLog($"  Total elapsed: {swTotal.ElapsedMilliseconds}ms");
                MessageBox.Show($"Segmentation completed.\n\nSuccess: {successCount}\nFailed: {failCount}\n\nTotal inference: {totalInferenceMs}ms\nAvg per image: {avgMs}ms\nTotal elapsed: {swTotal.ElapsedMilliseconds}ms",
                    "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private SegmentationResult SegmentImage(string imagePath, out long inferenceMs)
        {
            inferenceMs = 0;
            var result = new SegmentationResult
            {
                ImagePath = imagePath,
                FileName = Path.GetFileName(imagePath)
            };

            using (Mat originalImage = Cv2.ImRead(imagePath))
            {
                if (originalImage.Empty())
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Image load failed";
                    return result;
                }

                result.ImageWidth = originalImage.Width;
                result.ImageHeight = originalImage.Height;

                Mat[] inputImages = { originalImage };

                lock (DiceManager.lockObject)
                {
                    var swInference = Stopwatch.StartNew();
                    var segResults = DiceManager.SegmentModel.Inference(inputImages);
                    swInference.Stop();
                    inferenceMs = swInference.ElapsedMilliseconds;

                    if (segResults == null || segResults.Count == 0)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "No segmentation result";
                        return result;
                    }

                    var segResult = segResults[0];

                    if (segResult.segmentMap == null || segResult.segmentMap.Empty())
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "Empty segment map";
                        return result;
                    }

                    result.IsSuccess = true;

                    // segmentMap을 원본 크기로 리사이즈해서 저장 (라인 검출 재활용)
                    Mat resizedForResult = new Mat();
                    Cv2.Resize(segResult.segmentMap, resizedForResult,
                        new OpenCvSharp.Size(originalImage.Width, originalImage.Height),
                        interpolation: InterpolationFlags.Nearest);
                    result.SegmentMap = resizedForResult;

                    result.ResultImage = DrawSegmentationOnImage(originalImage, segResult.segmentMap);
                }
            }

            return result;
        }

        private Mat DrawSegmentationOnImage(Mat originalImage, Mat segmentMap)
        {
            Mat resultImage = originalImage.Clone();

            Mat resizedMap = new Mat();
            Cv2.Resize(segmentMap, resizedMap, new OpenCvSharp.Size(originalImage.Width, originalImage.Height),
                interpolation: InterpolationFlags.Nearest);

            if (Cv2.CountNonZero(resizedMap) > 0)
            {
                // 불량 영역만 빨간색(BGR: 0,0,255) 50% 오버레이
                Mat overlay = resultImage.Clone();
                overlay.SetTo(new Scalar(0, 0, 255), resizedMap);
                Cv2.AddWeighted(resultImage, 0.5, overlay, 0.5, 0, resultImage);
                overlay.Dispose();
            }

            resizedMap.Dispose();

            return resultImage;
        }

        private List<string> GetImageFiles(string folder)
        {
            var files = new List<string>();

            try
            {
                foreach (var file in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                {
                    if (IsSupportedImageFile(file))
                    {
                        files.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"File scan error: {ex.Message}");
            }

            return files;
        }

        private bool IsSupportedImageFile(string filePath)
        {
            return filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
        }

        private void AddResultToListView(SegmentationResult result)
        {
            var item = new ListViewItem(result.FileName);
            item.SubItems.Add(result.ImagePath);
            item.SubItems.Add($"{result.ImageWidth} x {result.ImageHeight}");
            item.SubItems.Add(result.IsSuccess ? "Success" : $"Failed: {result.ErrorMessage}");
            item.Tag = result;

            if (!result.IsSuccess)
            {
                item.ForeColor = UiTheme.Error;
            }

            listViewResults.Items.Add(item);
        }

        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count == 0)
            {
                return;
            }

            var selectedResult = listViewResults.SelectedItems[0].Tag as SegmentationResult;

            if (selectedResult != null && selectedResult.ResultImage != null)
            {
                UpdatePreviewImage(selectedResult);
                AppendLog($"Selected: {selectedResult.FileName} ({selectedResult.ImageWidth}x{selectedResult.ImageHeight})");
            }
        }

        private void UpdatePreviewImage(SegmentationResult result)
        {
            if (result?.ResultImage == null)
            {
                return;
            }

            if (pictureBoxPreview.Image != null)
            {
                var oldImage = pictureBoxPreview.Image;
                pictureBoxPreview.Image = null;
                oldImage.Dispose();
            }

            Mat display = result.ResultImage.Clone();

            if (chkLineDetect.Checked && result.SegmentMap != null && Cv2.CountNonZero(result.SegmentMap) > 0)
            {
                bool leftToRight = rbLeftToRight.Checked;
                var swLine = Stopwatch.StartNew();
                var distInfo = DrawLineDetect(display, result.SegmentMap, leftToRight);
                swLine.Stop();
                AppendLog($"[{result.FileName}] line detect: {swLine.ElapsedMilliseconds}ms ({(leftToRight ? "좌→우" : "상→하")}) | center dist — avg:{distInfo.Avg}px min:{distInfo.Min}px max:{distInfo.Max}px");
            }

            pictureBoxPreview.Image = BitmapConverter.ToBitmap(display);
            display.Dispose();
        }

        private void rbLineDirection_CheckedChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count == 0) return;
            var selectedResult = listViewResults.SelectedItems[0].Tag as SegmentationResult;
            if (selectedResult != null && selectedResult.ResultImage != null)
            {
                UpdatePreviewImage(selectedResult);
            }
        }

        private (int Avg, int Min, int Max) DrawLineDetect(Mat image, Mat segmentMap, bool leftToRight)
        {
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(segmentMap, out contours, out hierarchy,
                RetrievalModes.External, ContourApproximationModes.ApproxNone);

            if (contours.Length == 0) return (0, 0, 0);

            var allPoints = contours.SelectMany(c => c);

            List<OpenCvSharp.Point> linePoints;

            if (leftToRight)
            {
                linePoints = allPoints
                    .GroupBy(p => p.Y)
                    .OrderBy(g => g.Key)
                    .Select(g => new OpenCvSharp.Point(g.Min(p => p.X), g.Key))
                    .ToList();
            }
            else
            {
                linePoints = allPoints
                    .GroupBy(p => p.X)
                    .OrderBy(g => g.Key)
                    .Select(g => new OpenCvSharp.Point(g.Key, g.Min(p => p.Y)))
                    .ToList();
            }

            if (linePoints.Count < 2) return (0, 0, 0);

            int thickness = Math.Max(2, Math.Min(image.Rows, image.Cols) / 90);
            double fontScale = Math.Max(0.8, Math.Min(image.Rows, image.Cols) / 600.0);
            int textThickness = Math.Max(1, thickness / 3);
            int margin = thickness * 3;
            var green = new Scalar(0, 160, 0);
            var orange = new Scalar(0, 165, 255);

            // 중앙선 좌표 및 각 포인트와의 거리 계산
            int centerValue = leftToRight ? image.Cols / 2 : image.Rows / 2;
            var distances = linePoints.Select(p => Math.Abs((leftToRight ? p.X : p.Y) - centerValue)).ToList();
            int avgDist = (int)distances.Average();
            int minDist = distances.Min();
            int maxDist = distances.Max();
            var minPoint = linePoints[distances.IndexOf(minDist)];
            var maxPoint = linePoints[distances.IndexOf(maxDist)];

            // 중앙선 (주황색)
            if (leftToRight)
                Cv2.Line(image, new OpenCvSharp.Point(centerValue, 0), new OpenCvSharp.Point(centerValue, image.Rows - 1), orange, thickness: thickness);
            else
                Cv2.Line(image, new OpenCvSharp.Point(0, centerValue), new OpenCvSharp.Point(image.Cols - 1, centerValue), orange, thickness: thickness);

            // 검출된 경계선 (초록색)
            for (int i = 0; i < linePoints.Count - 1; i++)
                Cv2.Line(image, linePoints[i], linePoints[i + 1], green, thickness: thickness);

            var blue = new Scalar(220, 150, 50);

            // 이미지 Y 중앙에 가장 가까운 검출선 포인트에서 거리 하나만 표시
            int imageMid = leftToRight ? image.Rows / 2 : image.Cols / 2;
            var midLinePoint = linePoints.OrderBy(p => Math.Abs((leftToRight ? p.Y : p.X) - imageMid)).First();
            int midDist = Math.Abs((leftToRight ? midLinePoint.X : midLinePoint.Y) - centerValue);
            DrawDistanceArrow(image, midLinePoint, centerValue, leftToRight, $"{midDist}px", blue, thickness, fontScale, textThickness);

            // 좌상단 요약 텍스트
            PutTextWithBackground(image, $"Avg: {avgDist}px", new OpenCvSharp.Point(margin, margin + (int)(40 * fontScale)),  HersheyFonts.HersheySimplex, fontScale, blue, textThickness);
            PutTextWithBackground(image, $"Min: {minDist}px", new OpenCvSharp.Point(margin, margin + (int)(85 * fontScale)),  HersheyFonts.HersheySimplex, fontScale, blue, textThickness);
            PutTextWithBackground(image, $"Max: {maxDist}px", new OpenCvSharp.Point(margin, margin + (int)(130 * fontScale)), HersheyFonts.HersheySimplex, fontScale, blue, textThickness);

            return (avgDist, minDist, maxDist);
        }

        private void DrawDistanceArrow(Mat image, OpenCvSharp.Point linePoint, int centerValue, bool leftToRight,
            string label, Scalar color, int thickness, double fontScale, int textThickness)
        {
            // 두 선의 좌표 설정 (검출선 ↔ 중앙선)
            OpenCvSharp.Point ptA, ptB;
            if (leftToRight)
            {
                ptA = new OpenCvSharp.Point(linePoint.X, linePoint.Y);
                ptB = new OpenCvSharp.Point(centerValue, linePoint.Y);
            }
            else
            {
                ptA = new OpenCvSharp.Point(linePoint.X, linePoint.Y);
                ptB = new OpenCvSharp.Point(linePoint.X, centerValue);
            }

            int arrowLen = Math.Abs(ptA.X - ptB.X) + Math.Abs(ptA.Y - ptB.Y);

            // 화살표를 그릴 만큼 거리가 충분한 경우만 표시 (thickness * 4 이상)
            if (arrowLen < thickness * 4)
            {
                // 거리가 너무 짧으면 짧은 눈금선으로 대체
                int tickLen = thickness * 3;
                if (leftToRight)
                {
                    Cv2.Line(image, new OpenCvSharp.Point(ptA.X, ptA.Y - tickLen), new OpenCvSharp.Point(ptA.X, ptA.Y + tickLen), color, thickness);
                    Cv2.Line(image, new OpenCvSharp.Point(ptB.X, ptB.Y - tickLen), new OpenCvSharp.Point(ptB.X, ptB.Y + tickLen), color, thickness);
                    Cv2.Line(image, ptA, ptB, color, thickness);
                }
                else
                {
                    Cv2.Line(image, new OpenCvSharp.Point(ptA.X - tickLen, ptA.Y), new OpenCvSharp.Point(ptA.X + tickLen, ptA.Y), color, thickness);
                    Cv2.Line(image, new OpenCvSharp.Point(ptB.X - tickLen, ptB.Y), new OpenCvSharp.Point(ptB.X + tickLen, ptB.Y), color, thickness);
                    Cv2.Line(image, ptA, ptB, color, thickness);
                }
            }
            else
            {
                // tipLength는 0.05~0.3 사이로 제한
                double tipLength = Math.Min(0.15, Math.Max(0.025, (thickness * 4.0) / arrowLen));
                Cv2.ArrowedLine(image, ptA, ptB, color, thickness, tipLength: tipLength);
                Cv2.ArrowedLine(image, ptB, ptA, color, thickness, tipLength: tipLength);
            }

            // 오른쪽 라인 점(두 점 중 X가 큰 쪽) 오른쪽에 라벨 표시
            int rightX = Math.Max(ptA.X, ptB.X);
            int labelY = leftToRight ? ptA.Y : (ptA.Y + ptB.Y) / 2;
            int textOffset = thickness * 2;
            PutTextWithBackground(image, label, new OpenCvSharp.Point(rightX + textOffset, labelY),
                HersheyFonts.HersheySimplex, fontScale * 0.85, color, textThickness);
        }

        private void PutTextWithBackground(Mat image, string text, OpenCvSharp.Point pos,
            HersheyFonts font, double fontScale, Scalar textColor, int thickness)
        {
            int pad = Math.Max(4, (int)(fontScale * 8));
            var textSize = Cv2.GetTextSize(text, font, fontScale, thickness, out int baseline);

            var bgRect = new OpenCvSharp.Rect(
                pos.X - pad,
                pos.Y - textSize.Height - pad,
                textSize.Width + pad * 2,
                textSize.Height + baseline + pad * 2);

            // 이미지 경계 클램핑
            bgRect = new OpenCvSharp.Rect(
                Math.Max(0, bgRect.X),
                Math.Max(0, bgRect.Y),
                Math.Min(bgRect.Width,  image.Cols - Math.Max(0, bgRect.X)),
                Math.Min(bgRect.Height, image.Rows - Math.Max(0, bgRect.Y)));

            if (bgRect.Width > 0 && bgRect.Height > 0)
            {
                Mat roi = image[bgRect];
                Mat black = new Mat(roi.Size(), roi.Type(), Scalar.Black);
                Cv2.AddWeighted(roi, 0.35, black, 0.65, 0, roi);
                black.Dispose();
            }

            Cv2.PutText(image, text, pos, font, fontScale, textColor, thickness);
        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Do you want to clear all segmentation results?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                DisposeResults();
                listViewResults.Items.Clear();

                if (pictureBoxPreview.Image != null)
                {
                    var oldImage = pictureBoxPreview.Image;
                    pictureBoxPreview.Image = null;
                    oldImage.Dispose();
                }

                progressBar.Value = 0;
                lblProgress.Text = "0 / 0";
                txtLog.Clear();
                AppendLog("Segmentation results have been reset.");
            }
        }

        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            if (_results.Count == 0)
            {
                MessageBox.Show("There are no results to save.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string saveFolder = FolderPickerDialog.Show(this, "Select the folder to save result images.");
            if (saveFolder != null)
            {
                try
                {
                    int savedCount = 0;

                    bool lineDetectOn = chkLineDetect.Checked;
                    bool leftToRight = rbLeftToRight.Checked;

                    foreach (var result in _results)
                    {
                        if (result.ResultImage != null)
                        {
                            string outputPath = Path.Combine(saveFolder,
                                Path.GetFileNameWithoutExtension(result.FileName) + "_seg.jpg");

                            Mat saveImage = result.ResultImage.Clone();
                            if (lineDetectOn && result.SegmentMap != null && Cv2.CountNonZero(result.SegmentMap) > 0)
                            {
                                DrawLineDetect(saveImage, result.SegmentMap, leftToRight);
                            }

                            Cv2.ImWrite(outputPath, saveImage);
                            saveImage.Dispose();
                            savedCount++;
                        }
                    }

                    AppendLog($"{savedCount} result image(s) saved to: {saveFolder}");
                    MessageBox.Show($"{savedCount} result image(s) saved.",
                        "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(message)));
                return;
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
            Logger.Info($"[Segmentation] {message}");
        }

        private void DisposeResults()
        {
            foreach (var result in _results)
            {
                result.ResultImage?.Dispose();
                result.SegmentMap?.Dispose();
            }

            _results.Clear();
        }
    }

    public class SegmentationResult
    {
        public string ImagePath { get; set; }
        public string FileName { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public Mat ResultImage { get; set; }
        public Mat SegmentMap { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
