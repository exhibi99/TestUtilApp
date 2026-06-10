using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;

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
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the folder containing images to segment.";
                dialog.SelectedPath = txtSourceFolder.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSourceFolder.Text = dialog.SelectedPath;
                    _config.LastSegmentSourceFolder = dialog.SelectedPath;
                    _configService.SaveConfig(_config);
                }
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
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the segmentation model folder.";
                dialog.SelectedPath = txtModelPath.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtModelPath.Text = dialog.SelectedPath;

                    if (_config.DiceModels == null)
                    {
                        _config.DiceModels = new DiceModelsConfig();
                    }

                    if (_config.DiceModels.SegmentModel == null)
                    {
                        _config.DiceModels.SegmentModel = new DiceModelSetting();
                    }

                    _config.DiceModels.SegmentModel.Path = dialog.SelectedPath;
                    _config.DiceModels.SegmentModel.Use = true;
                    _configService.SaveConfig(_config);
                }
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

            foreach (var imagePath in imageFiles)
            {
                try
                {
                    var result = SegmentImage(imagePath);

                    Invoke(new Action(() =>
                    {
                        _results.Add(result);
                        AddResultToListView(result);
                        progressBar.Value = ++processedCount;
                        lblProgress.Text = $"{processedCount} / {imageFiles.Count}";

                        if (result.IsSuccess)
                        {
                            successCount++;
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

            Invoke(new Action(() =>
            {
                AppendLog($"Segmentation complete - Success: {successCount}, Failed: {failCount}");
                MessageBox.Show($"Segmentation completed.\n\nSuccess: {successCount}\nFailed: {failCount}",
                    "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private SegmentationResult SegmentImage(string imagePath)
        {
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
                    var segResults = DiceManager.SegmentModel.Inference(inputImages);

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

            Mat colorMap = new Mat();
            Cv2.ApplyColorMap(resizedMap, colorMap, ColormapTypes.Jet);

            Mat overlaid = new Mat();
            Cv2.AddWeighted(resultImage, 0.6, colorMap, 0.4, 0, overlaid);

            resizedMap.Dispose();
            colorMap.Dispose();
            resultImage.Dispose();

            return overlaid;
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

            pictureBoxPreview.Image = BitmapConverter.ToBitmap(result.ResultImage);
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

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the folder to save result images.";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int savedCount = 0;

                        foreach (var result in _results)
                        {
                            if (result.ResultImage != null)
                            {
                                string outputPath = Path.Combine(dialog.SelectedPath,
                                    Path.GetFileNameWithoutExtension(result.FileName) + "_seg.jpg");

                                Cv2.ImWrite(outputPath, result.ResultImage);
                                savedCount++;
                            }
                        }

                        AppendLog($"{savedCount} result image(s) saved to: {dialog.SelectedPath}");
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
        }

        private void DisposeResults()
        {
            foreach (var result in _results)
            {
                result.ResultImage?.Dispose();
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
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
