using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class CropControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private ImageProcessorService _imageProcessorService;

        private string _cropSourceFolderPath = string.Empty;
        private string _cropOutputFolderPath = string.Empty;
        private int _totalCropImages = 0;

        // Results File List
        private List<string> _croppedImagePaths = new List<string>();

        public CropControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;

            // ImageProcessorService Reset
            try
            {
                _imageProcessorService = new ImageProcessorService(_config);

                // Subscribe to events
                _imageProcessorService.OnProgressUpdate += OnProgressUpdate;
                _imageProcessorService.OnLogMessage += OnLogMessage;
            }
            catch (Exception ex)
            {
                AppendLog($"ImageProcessorService initialization failed: {ex.Message}");
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Manual Crop 기본값 Settings
            if (_config.DefaultCropArea != null)
            {
                numManualX.Value = _config.DefaultCropArea.X;
                numManualY.Value = _config.DefaultCropArea.Y;
                numManualWidth.Value = _config.DefaultCropArea.Width;
                numManualHeight.Value = _config.DefaultCropArea.Height;
            }

            // Detection Model Display Information
            UpdateDetectModelInfo();

            // 마지막 Path 복원
            if (!string.IsNullOrEmpty(_config.LastCropSourceFolder) &&
                Directory.Exists(_config.LastCropSourceFolder))
            {
                _cropSourceFolderPath = _config.LastCropSourceFolder;
                txtSourceFolder.Text = _cropSourceFolderPath;

                var imageFiles = GetImageFiles(_cropSourceFolderPath);
                _totalCropImages = imageFiles.Length;
                lblImageCount.Text = $"Images Found: {_totalCropImages}";

                string parentFolder = Directory.GetParent(_cropSourceFolderPath).FullName;
                string folderName = Path.GetFileName(_cropSourceFolderPath);
                _cropOutputFolderPath = Path.Combine(parentFolder, folderName + "_crop");

                UpdateStartButtonState();

                // Existing Crop Results가 있으면 Load
                LoadExistingCropResults();
            }

            // Manual Crop 체크박스 이벤트
            chkUseManualCrop.CheckedChanged += ChkUseManualCrop_CheckedChanged;

            // File 리스트 Select 이벤트
            lstCroppedFiles.SelectedIndexChanged += LstCroppedFiles_SelectedIndexChanged;

            // Manual Crop Area 활성화 상태 Reset
            UpdateManualCropControlsState();
        }

        private void LoadExistingCropResults()
        {
            if (!Directory.Exists(_cropOutputFolderPath))
                return;

            try
            {
                var croppedFiles = GetImageFiles(_cropOutputFolderPath);
                if (croppedFiles.Length > 0)
                {
                    _croppedImagePaths = croppedFiles.ToList();
                    UpdateFileList();
                    AppendLog($"Existing crop results found: {croppedFiles.Length} File");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Failed to load existing results: {ex.Message}");
            }
        }

        private void UpdateFileList()
        {
            lstCroppedFiles.Items.Clear();

            foreach (var filePath in _croppedImagePaths)
            {
                string fileName = Path.GetFileName(filePath);
                lstCroppedFiles.Items.Add(fileName);
            }

            lblResultCount.Text = $"Cropped Files: {_croppedImagePaths.Count}";
        }

        private void LstCroppedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCroppedFiles.SelectedIndex < 0 ||
                lstCroppedFiles.SelectedIndex >= _croppedImagePaths.Count)
            {
                return;
            }

            string selectedPath = _croppedImagePaths[lstCroppedFiles.SelectedIndex];
            ShowPreview(selectedPath);
        }

        private void ShowPreview(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    lblPreviewInfo.Text = "File not found.";
                    pictureBoxPreview.Image = null;
                    return;
                }

                // 원본 Image Path 추정
                string fileName = Path.GetFileName(imagePath);
                string originalFileName = ExtractOriginalFileName(fileName);
                string originalPath = Path.Combine(_cropSourceFolderPath, originalFileName);

                using (Mat image = Cv2.ImRead(imagePath, ImreadModes.Color))
                {
                    if (image.Empty())
                    {
                        lblPreviewInfo.Text = "Image load failed";
                        return;
                    }

                    // 원본 Image서 Display Detection Area
                    Mat displayImage = image.Clone();

                    // Auto Detect 모드였다면 Detect Area 그리기
                    if (!chkUseManualCrop.Checked && File.Exists(originalPath))
                    {
                        DrawDetectionBox(ref displayImage, originalPath);
                    }

                    // PictureBox Display
                    if (pictureBoxPreview.Image != null)
                    {
                        pictureBoxPreview.Image.Dispose();
                    }

                    pictureBoxPreview.Image = BitmapConverter.ToBitmap(displayImage);
                    displayImage.Dispose();

                    FileInfo fileInfo = new FileInfo(imagePath);
                    lblPreviewInfo.Text = $"{Path.GetFileName(imagePath)} | " +
                                         $"{image.Width}x{image.Height} | " +
                                         $"{fileInfo.Length / 1024}KB";
                }
            }
            catch (Exception ex)
            {
                lblPreviewInfo.Text = $"Preview error: {ex.Message}";
                AppendLog($"Preview error: {ex.Message}");
            }
        }

        private void DrawDetectionBox(ref Mat image, string originalPath)
        {
            try
            {
                using (Mat originalImage = Cv2.ImRead(originalPath, ImreadModes.Color))
                {
                    if (originalImage.Empty() || !DiceManager.IsDetectModelLoaded(GetConfiguredDetectModelPath()))
                        return;

                    // Detection 수행
                    var results = DiceManager.DetectModel.Inference(originalImage);
                    if (results == null || results.Count == 0)
                        return;

                    // Detect된 Area 그리기
                    foreach (var rect in results[0].listRect)
                    {
                        // Rectangle 그리기 (두 Point Use)
                        OpenCvSharp.Point pt1 = new OpenCvSharp.Point(rect.rect.X, rect.rect.Y);
                        OpenCvSharp.Point pt2 = new OpenCvSharp.Point(rect.rect.X + rect.rect.Width,
                                                                       rect.rect.Y + rect.rect.Height);
                        Cv2.Rectangle(image, pt1, pt2, Scalar.Red, 3);

                        // 클래스명and 신뢰도 Display
                        string label = $"{rect.class_name} {rect.conf:F2}";
                        int baseline;
                        var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheySimplex, 0.7, 2, out baseline);

                        // Draw label background
                        OpenCvSharp.Point bgPt1 = new OpenCvSharp.Point(rect.rect.X, rect.rect.Y - textSize.Height - 10);
                        OpenCvSharp.Point bgPt2 = new OpenCvSharp.Point(rect.rect.X + textSize.Width, rect.rect.Y);
                        Cv2.Rectangle(image, bgPt1, bgPt2, Scalar.Red, -1);

                        // Draw label text
                        OpenCvSharp.Point textPt = new OpenCvSharp.Point(rect.rect.X, rect.rect.Y - 5);
                        Cv2.PutText(image, label, textPt,
                            HersheyFonts.HersheySimplex, 0.7, Scalar.White, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Display Detection Area Error: {ex.Message}");
            }
        }

        private string ExtractOriginalFileName(string croppedFileName)
        {
            // Cropped Files명서 원본 Filename 추출
            // 예: "BELT_TOP_image001.jpg" -> "image001.jpg"
            string[] parts = croppedFileName.Split('_');
            if (parts.Length >= 3)
            {
                // BELT_TOP_ 또는 BELT_BOTTOM_ 등 제거
                return string.Join("_", parts.Skip(2));
            }
            return croppedFileName;
        }

        private void UpdateDetectModelInfo()
        {
            EnsureDetectModelConfig();

            string modelPath = GetConfiguredDetectModelPath();
            if (txtDetModelPath != null && !txtDetModelPath.Focused && txtDetModelPath.Text != modelPath)
            {
                txtDetModelPath.Text = modelPath;
            }

            bool isLoaded = DiceManager.IsDetectModelLoaded(modelPath);
            lblDetModelStatus.Text = isLoaded ? "Loaded ✓" : "Not loaded";
            lblDetModelStatus.ForeColor = isLoaded ? UiTheme.Success : UiTheme.Error;
        }

        private void EnsureDetectModelConfig()
        {
            if (_config.DiceModels == null)
            {
                _config.DiceModels = new DiceModelsConfig();
            }

            if (_config.DiceModels.DetectModel == null)
            {
                _config.DiceModels.DetectModel = new DiceModelSetting { Use = true, Path = string.Empty };
            }
        }

        private string GetConfiguredDetectModelPath()
        {
            return _config.DiceModels?.DetectModel?.Path ?? string.Empty;
        }

        private bool SaveDetectModelPathFromInput(bool showValidationMessage, bool appendSavedLog)
        {
            EnsureDetectModelConfig();

            string modelPath = txtDetModelPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                if (showValidationMessage)
                {
                    MessageBox.Show("Please enter or select a detection model path.", "Input Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return false;
            }

            if (!Directory.Exists(modelPath) && !File.Exists(modelPath))
            {
                if (showValidationMessage)
                {
                    MessageBox.Show("Selected model path does not exist.", "Path Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    AppendLog($"Detection model path was not saved because it does not exist: {modelPath}");
                }
                return false;
            }

            string currentPath = GetConfiguredDetectModelPath();
            if (string.Equals(modelPath, currentPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            _config.DiceModels.DetectModel.Use = true;
            _config.DiceModels.DetectModel.Path = modelPath;

            try
            {
                _configService.SaveConfig(_config);
                if (appendSavedLog)
                {
                    AppendLog($"Detection model path saved: {modelPath}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Detection model path save failed: {ex.Message}");
                if (showValidationMessage)
                {
                    MessageBox.Show($"Detection model path save failed: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            UpdateDetectModelInfo();
            return true;
        }

        private bool EnsureDetectionModelLoaded()
        {
            string modelPath = GetConfiguredDetectModelPath();

            AppendLog("Preparing detection model...");
            bool isLoaded = DiceManager.EnsureDetectModelLoaded(modelPath);
            AppendLog(isLoaded ? "Detection model is ready." : "Detection model load failed.");
            RefreshDetectModelInfo();

            return isLoaded;
        }

        private async System.Threading.Tasks.Task<bool> EnsureDetectionModelLoadedWithPopupAsync(string message)
        {
            if (DiceManager.IsDetectModelLoaded(GetConfiguredDetectModelPath()))
            {
                UpdateDetectModelInfo();
                return true;
            }

            using (var loadingDialog = ModelLoadingDialog.ShowFor(this, message))
            {
                bool isLoaded = await System.Threading.Tasks.Task.Run(() => EnsureDetectionModelLoaded());
                loadingDialog.CloseSafely();
                return isLoaded;
            }
        }

        private bool EnsureCropClassificationModelsLoaded()
        {
            bool hasEnabledModel = false;
            bool allLoaded = true;

            if (_config.DiceModels?.ClassifyModel_A?.Use == true)
            {
                hasEnabledModel = true;
                string modelPath = _config.DiceModels.ClassifyModel_A.Path;
                AppendLog("Preparing classification model: ClassifyModel_A");
                bool isLoaded = DiceManager.EnsureClassifyModelALoaded(modelPath);
                AppendLog(isLoaded
                    ? "Classification model is ready: ClassifyModel_A"
                    : "Classification model load failed: ClassifyModel_A");
                allLoaded &= isLoaded;
            }

            if (_config.DiceModels?.ClassifyModel_B?.Use == true)
            {
                hasEnabledModel = true;
                string modelPath = _config.DiceModels.ClassifyModel_B.Path;
                AppendLog("Preparing classification model: ClassifyModel_B");
                bool isLoaded = DiceManager.EnsureClassifyModelBLoaded(modelPath);
                AppendLog(isLoaded
                    ? "Classification model is ready: ClassifyModel_B"
                    : "Classification model load failed: ClassifyModel_B");
                allLoaded &= isLoaded;
            }

            if (!hasEnabledModel)
            {
                AppendLog("No classification models are enabled for Image Crop.");
            }

            return allLoaded;
        }

        private bool AreCropClassificationModelsLoaded()
        {
            bool hasEnabledModel = false;
            bool allLoaded = true;

            if (_config.DiceModels?.ClassifyModel_A?.Use == true)
            {
                hasEnabledModel = true;
                allLoaded &= DiceManager.IsClassifyModelALoaded(_config.DiceModels.ClassifyModel_A.Path);
            }

            if (_config.DiceModels?.ClassifyModel_B?.Use == true)
            {
                hasEnabledModel = true;
                allLoaded &= DiceManager.IsClassifyModelBLoaded(_config.DiceModels.ClassifyModel_B.Path);
            }

            return !hasEnabledModel || allLoaded;
        }

        private async System.Threading.Tasks.Task<bool> EnsureCropModelsLoadedWithPopupAsync(bool includeDetectionModel)
        {
            if (!includeDetectionModel || DiceManager.IsDetectModelLoaded(GetConfiguredDetectModelPath()))
            {
                UpdateDetectModelInfo();
                return true;
            }

            using (var loadingDialog = ModelLoadingDialog.ShowFor(
                this,
                "Loading model...\r\nImage Crop will continue when loading is complete."))
            {
                bool isLoaded = await System.Threading.Tasks.Task.Run(() => EnsureDetectionModelLoaded());

                loadingDialog.CloseSafely();
                return isLoaded;
            }
        }

        private void RefreshDetectModelInfo()
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateDetectModelInfo));
                return;
            }

            UpdateDetectModelInfo();
        }

        private void UpdateManualCropControlsState()
        {
            bool isManual = chkUseManualCrop.Checked;
            numManualX.Enabled = isManual;
            numManualY.Enabled = isManual;
            numManualWidth.Enabled = isManual;
            numManualHeight.Enabled = isManual;
        }

        private void ChkUseManualCrop_CheckedChanged(object sender, EventArgs e)
        {
            UpdateManualCropControlsState();
            UpdateStartButtonState();
        }

        private void UpdateStartButtonState()
        {
            if (chkUseManualCrop.Checked)
            {
                // Manual Mode: Image만 있으면 OK
                btnStartCrop.Enabled = _totalCropImages > 0;
            }
            else
            {
                // Auto Mode: the detection model is loaded lazily when the crop starts.
                btnStartCrop.Enabled = _totalCropImages > 0;
            }
        }

        public void OnActivated()
        {
            AppendLog("Image Crop screen has been activated.");
            UpdateDetectModelInfo(); // Model 상태 갱신
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the source folder containing images to crop.";
                if (!string.IsNullOrEmpty(txtSourceFolder.Text))
                {
                    dialog.SelectedPath = txtSourceFolder.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _cropSourceFolderPath = dialog.SelectedPath;
                    txtSourceFolder.Text = _cropSourceFolderPath;

                    var imageFiles = GetImageFiles(_cropSourceFolderPath);
                    _totalCropImages = imageFiles.Length;
                    lblImageCount.Text = $"Images Found: {_totalCropImages}";

                    string parentFolder = Directory.GetParent(_cropSourceFolderPath).FullName;
                    string folderName = Path.GetFileName(_cropSourceFolderPath);
                    _cropOutputFolderPath = Path.Combine(parentFolder, folderName + "_crop");

                    UpdateStartButtonState();

                    // Config Save
                    _config.LastCropSourceFolder = _cropSourceFolderPath;
                    try
                    {
                        _configService.SaveConfig(_config);
                        AppendLog($"Source Folder Select: {_cropSourceFolderPath}");
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Settings Save failed: {ex.Message}");
                    }
                }
            }
        }

        private void btnBrowseDetModel_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the folder containing the detection model (.dice file).";
                if (!string.IsNullOrEmpty(txtDetModelPath.Text))
                {
                    dialog.SelectedPath = txtDetModelPath.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDetModelPath.Text = dialog.SelectedPath;
                    AppendLog($"Detection model folder selected: {dialog.SelectedPath}");
                    SaveDetectModelPathFromInput(false, true);
                }
            }
        }

        private void txtDetModelPath_Leave(object sender, EventArgs e)
        {
            SaveDetectModelPathFromInput(false, true);
        }

        private async void btnLoadCropModel_Click(object sender, EventArgs e)
        {
            if (!SaveDetectModelPathFromInput(true, true))
            {
                return;
            }

            try
            {
                SetUIEnabled(false);
                progressBar.Value = 0;

                bool isLoaded = await EnsureDetectionModelLoadedWithPopupAsync(
                    "Loading detection model...\r\nThis can take a moment. Please wait.");
                if (!isLoaded)
                {
                    MessageBox.Show("Detection model load failed.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Detection model loaded.", "Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"Detection model load error: {ex.Message}");
                MessageBox.Show($"Detection model load error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIEnabled(true);
                UpdateStartButtonState();
                UpdateDetectModelInfo();
            }
        }

        private async void btnStartCrop_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_cropSourceFolderPath))
            {
                MessageBox.Show("Please select a source folder first.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(_cropSourceFolderPath))
            {
                MessageBox.Show("Selected folder does not exist.", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_imageProcessorService == null)
            {
                MessageBox.Show("ImageProcessorService not initialized.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool useManualCrop = chkUseManualCrop.Checked;

            if (!useManualCrop && !SaveDetectModelPathFromInput(true, false))
            {
                return;
            }

            // Manual Crop 검증
            Rect manualCropArea = Rect.Empty;
            if (useManualCrop)
            {
                if (!ValidateManualCropInput(out manualCropArea))
                {
                    MessageBox.Show("Please enter a valid crop area.\n" +
                        "Width and height must be greater than 0.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            try
            {
                // Disable UI
                SetUIEnabled(false);
                progressBar.Value = 0;
                txtLog.Clear();

                AppendLog($"Crop operation started: {_cropSourceFolderPath}");
                AppendLog($"Mode: {(useManualCrop ? "Manual Crop" : "Auto Detect Crop")}");

                if (useManualCrop)
                {
                    AppendLog($"Crop Area: X={manualCropArea.X}, Y={manualCropArea.Y}, " +
                             $"W={manualCropArea.Width}, H={manualCropArea.Height}");
                }

                bool areModelsReady = await EnsureCropModelsLoadedWithPopupAsync(!useManualCrop);
                if (!areModelsReady)
                {
                    throw new InvalidOperationException("The detection model could not be loaded.");
                }

                // Asynchronous processing
                _ = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        if (useManualCrop)
                        {
                            // Manual Crop
                            _imageProcessorService.ProcessManualCropImages(
                                _cropSourceFolderPath,
                                _cropOutputFolderPath,
                                manualCropArea
                            );
                        }
                        else
                        {
                            // Auto Detect Crop
                            _imageProcessorService.ProcessCropImages(
                                _cropSourceFolderPath,
                                _cropOutputFolderPath
                            );
                        }

                        this.Invoke(new Action(() =>
                        {
                            AppendLog($"Crop operation completed.");
                            AppendLog($"Output Folder: {_cropOutputFolderPath}");

                            // 크롭된 File List Load
                            LoadCroppedFiles();

                            MessageBox.Show($"Crop operation completed.\nOutput Folder: {_cropOutputFolderPath}",
                                "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            SetUIEnabled(true);
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            AppendLog($"Error occurred: {ex.Message}");
                            MessageBox.Show($"Error occurred: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            SetUIEnabled(true);
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                AppendLog($"Error occurred: {ex.Message}");
                MessageBox.Show($"Error occurred: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetUIEnabled(true);
            }
        }

        private bool ValidateManualCropInput(out Rect cropArea)
        {
            cropArea = Rect.Empty;

            try
            {
                int x = (int)numManualX.Value;
                int y = (int)numManualY.Value;
                int width = (int)numManualWidth.Value;
                int height = (int)numManualHeight.Value;

                if (width <= 0 || height <= 0)
                {
                    return false;
                }

                cropArea = new Rect(x, y, width, height);

                // Config Save
                _config.DefaultCropArea.X = x;
                _config.DefaultCropArea.Y = y;
                _config.DefaultCropArea.Width = width;
                _config.DefaultCropArea.Height = height;

                try
                {
                    _configService.SaveConfig(_config);
                }
                catch { }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string[] GetImageFiles(string folderPath)
        {
            string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };
            return extensions.SelectMany(ext =>
                Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories)
            ).ToArray();
        }

        private void LoadCroppedFiles()
        {
            try
            {
                if (!Directory.Exists(_cropOutputFolderPath))
                    return;

                var croppedFiles = GetImageFiles(_cropOutputFolderPath);
                _croppedImagePaths = croppedFiles.ToList();
                UpdateFileList();

                if (_croppedImagePaths.Count > 0)
                {
                    AppendLog($"Cropped Files {_croppedImagePaths.Count} Loaded");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Crop file load error: {ex.Message}");
            }
        }

        private void OnProgressUpdate(int current, int total)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnProgressUpdate(current, total)));
                return;
            }

            if (total > 0)
            {
                progressBar.Maximum = total;
                progressBar.Value = Math.Min(current, total);
            }
        }

        private void OnLogMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnLogMessage(message)));
                return;
            }

            AppendLog(message);
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

        private void SetUIEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetUIEnabled(enabled)));
                return;
            }

            btnBrowseSource.Enabled = enabled;
            btnStartCrop.Enabled = enabled;
            chkUseManualCrop.Enabled = enabled;
            numManualX.Enabled = enabled && chkUseManualCrop.Checked;
            numManualY.Enabled = enabled && chkUseManualCrop.Checked;
            numManualWidth.Enabled = enabled && chkUseManualCrop.Checked;
            numManualHeight.Enabled = enabled && chkUseManualCrop.Checked;
            btnBrowseDetModel.Enabled = enabled;
            txtDetModelPath.Enabled = enabled;
            btnLoadCropModel.Enabled = enabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_imageProcessorService != null)
                {
                    _imageProcessorService.OnProgressUpdate -= OnProgressUpdate;
                    _imageProcessorService.OnLogMessage -= OnLogMessage;
                }

                if (pictureBoxPreview?.Image != null)
                {
                    pictureBoxPreview.Image.Dispose();
                    pictureBoxPreview.Image = null;
                }

                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
