using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenCvSharp;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class LabelGeneratorControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private LabelGeneratorService _labelGenService;

        // Results Save
        private List<string> _generatedFiles;
        private Image _currentPreviewImage;
        private string _lastSourceFolder;
        private readonly Dictionary<string, Scalar> _classColorCache = new Dictionary<string, Scalar>(StringComparer.OrdinalIgnoreCase);

        public LabelGeneratorControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;

            try
            {
                _labelGenService = new LabelGeneratorService(_config);

                // Subscribe to events
                _labelGenService.OnProgressUpdate += OnProgressUpdate;
                _labelGenService.OnLogMessage += OnLogMessage;
            }
            catch (Exception ex)
            {
                AppendLog($"LabelGeneratorService reset failed: {ex.Message}");
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            // 마지막 Path 복원
            if (!string.IsNullOrEmpty(_config.LastLabelGenSourceFolder))
            {
                txtSourceFolder.Text = _config.LastLabelGenSourceFolder;
            }

            // LabelGeneration Settings값 Load
            if (_config.LabelGeneration != null)
            {
                numMinConfidence.Value = (decimal)_config.LabelGeneration.MinConfidence;
                chkSkipExistingJson.Checked = _config.LabelGeneration.SkipExistingJson;
            }
            else
            {
                // Default value Settings
                numMinConfidence.Value = 0.5m;
                chkSkipExistingJson.Checked = true;
            }

            EnsureDetectModelConfig();
            txtDetectModelPath.Text = GetConfiguredDetectModelPath();

            // Detect Model Display Information
            UpdateDetectModelInfo();

            // 초기 상태
            ClearResults();
        }

        /// <summary>
        /// Detect Model Path 및 Load Display Status
        /// </summary>
        private void UpdateDetectModelInfo()
        {
            try
            {
                string configuredPath = GetConfiguredDetectModelPath();
                string detectModelPath = GetDetectModelPath();
                bool isDetectModelLoaded = DiceManager.IsDetectModelLoaded(configuredPath);

                if (txtDetectModelPath != null && !txtDetectModelPath.Focused && txtDetectModelPath.Text != configuredPath)
                {
                    txtDetectModelPath.Text = configuredPath;
                }

                if (isDetectModelLoaded)
                {
                    lblDetectModelInfo.Text = $"✓ Loaded";
                    lblDetectModelInfo.ForeColor = UiTheme.Success;
                }
                else
                {
                    lblDetectModelInfo.Text = $"✗ Not Loaded";
                    lblDetectModelInfo.ForeColor = UiTheme.Error;
                }

                // Log 상세 정보 출력
                AppendLog($"Detect Model: {(isDetectModelLoaded ? "Loaded" : "Not Loaded")}");
                AppendLog($"  Path: {detectModelPath}");
            }
            catch (Exception ex)
            {
                AppendLog($"Model Display Information Error: {ex.Message}");
                lblDetectModelInfo.Text = "✗ Cannot retrieve information.";
                lblDetectModelInfo.ForeColor = UiTheme.Error;
            }
        }

        /// <summary>
        /// Detect Model Path 가져오기
        /// </summary>
        private string GetDetectModelPath()
        {
            try
            {
                string modelPath = GetConfiguredDetectModelPath();

                // File 존재 여부 Confirm
                if (File.Exists(modelPath) || Directory.Exists(modelPath))
                {
                    return modelPath;
                }

                return string.IsNullOrWhiteSpace(modelPath) ? "Path not found." : modelPath;
            }
            catch
            {
                return "Path not found.";
            }
        }

        private string GetConfiguredDetectModelPath()
        {
            return _config.DiceModels?.DetectModel?.Path ?? string.Empty;
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

        private bool SaveDetectModelPathFromInput(bool showValidationMessage)
        {
            EnsureDetectModelConfig();

            string modelPath = txtDetectModelPath.Text.Trim();
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
                return false;
            }

            _config.DiceModels.DetectModel.Use = true;
            _config.DiceModels.DetectModel.Path = modelPath;

            try
            {
                _configService.SaveConfig(_config);
            }
            catch (Exception ex)
            {
                AppendLog($"Settings Save failed: {ex.Message}");
            }

            _labelGenService?.UpdateConfig(_config);
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

        private bool EnsureDetectionModelLoadedWithPopup(string message)
        {
            if (DiceManager.IsDetectModelLoaded(GetConfiguredDetectModelPath()))
            {
                UpdateDetectModelInfo();
                return true;
            }

            using (var loadingDialog = ModelLoadingDialog.ShowFor(this, message))
            {
                bool isLoaded = EnsureDetectionModelLoaded();
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

        public void OnActivated()
        {
            AppendLog("Label Gen screen has been activated.");
            UpdateDetectModelInfo();
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the source folder containing images for label generation.";
                if (!string.IsNullOrEmpty(txtSourceFolder.Text))
                {
                    dialog.SelectedPath = txtSourceFolder.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSourceFolder.Text = dialog.SelectedPath;
                    _config.LastLabelGenSourceFolder = dialog.SelectedPath;

                    try
                    {
                        _configService.SaveConfig(_config);
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Settings Save failed: {ex.Message}");
                    }
                }
            }
        }

        private void btnBrowseDetectModel_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the folder containing the detection model.";
                if (!string.IsNullOrEmpty(txtDetectModelPath.Text) && Directory.Exists(txtDetectModelPath.Text))
                {
                    dialog.SelectedPath = txtDetectModelPath.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDetectModelPath.Text = dialog.SelectedPath;
                    SaveDetectModelPathFromInput(false);
                    AppendLog($"Detection model path selected: {dialog.SelectedPath}");
                }
            }
        }

        private async void btnLoadDetectModel_Click(object sender, EventArgs e)
        {
            if (!SaveDetectModelPathFromInput(true))
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
                UpdateDetectModelInfo();
            }
        }

        private async void btnStartLabelGen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSourceFolder.Text))
            {
                MessageBox.Show("Please select source folder.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(txtSourceFolder.Text))
            {
                MessageBox.Show("Selected folder does not exist.", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_labelGenService == null)
            {
                MessageBox.Show("LabelGeneratorService has not been initialized.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!SaveDetectModelPathFromInput(true))
            {
                return;
            }

            try
            {
                // Disable UI
                SetUIEnabled(false);
                progressBar.Value = 0;
                txtLog.Clear();
                ClearResults();

                // LabelGeneration Settings 업데이트
                if (_config.LabelGeneration == null)
                {
                    _config.LabelGeneration = new LabelGenerationConfig();
                }

                _config.LabelGeneration.MinConfidence = (float)numMinConfidence.Value;
                _config.LabelGeneration.SkipExistingJson = chkSkipExistingJson.Checked;

                try
                {
                    _configService.SaveConfig(_config);
                }
                catch (Exception ex)
                {
                    AppendLog($"Settings Save failed: {ex.Message}");
                }

                // LabelGeneratorService 업데이트된 config 전달
                _labelGenService.UpdateConfig(_config);

                _lastSourceFolder = txtSourceFolder.Text;

                AppendLog($"Label Generate: {txtSourceFolder.Text}");
                AppendLog($"MinConfidence: {_config.LabelGeneration.MinConfidence:F2}");
                AppendLog($"SkipExistingJson: {_config.LabelGeneration.SkipExistingJson}");

                bool isModelLoaded = await EnsureDetectionModelLoadedWithPopupAsync(
                    "Loading detection model...\r\nLabel generation will start automatically when loading is complete.");
                if (!isModelLoaded)
                {
                    throw new InvalidOperationException("The detection model could not be loaded.");
                }

                // Asynchronous processing
                _ = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        _labelGenService.GenerateLabelsForFolder(txtSourceFolder.Text);

                        this.Invoke(new Action(() =>
                        {
                            AppendLog("Label Gen task completed.");

                            // Display Results
                            LoadGeneratedResults();

                            MessageBox.Show("Label Gen task completed.", "Complete",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// Generate된 Results Load
        /// </summary>
        private void LoadGeneratedResults()
        {
            if (string.IsNullOrEmpty(_lastSourceFolder) || !Directory.Exists(_lastSourceFolder))
            {
                return;
            }

            try
            {
                // 모드 따라 File 검색
                if (rbShowImages.Checked)
                {
                    // Image File 검색
                    string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };
                    _generatedFiles = extensions.SelectMany(ext =>
                        Directory.GetFiles(_lastSourceFolder, ext, SearchOption.AllDirectories)
                    ).ToList();
                }
                else
                {
                    // JSON File 검색
                    _generatedFiles = Directory.GetFiles(_lastSourceFolder, "*.json", SearchOption.AllDirectories).ToList();
                }

                DisplayResults();
            }
            catch (Exception ex)
            {
                AppendLog($"Results load failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Results List Display
        /// </summary>
        private void DisplayResults()
        {
            listViewResults.BeginUpdate();
            listViewResults.Items.Clear();

            if (_generatedFiles == null || _generatedFiles.Count == 0)
            {
                listViewResults.EndUpdate();
                return;
            }

            foreach (string filePath in _generatedFiles)
            {
                string fileName = Path.GetFileName(filePath);

                ListViewItem item = new ListViewItem(fileName);
                item.SubItems.Add(filePath);
                item.Tag = filePath;

                listViewResults.Items.Add(item);
            }

            listViewResults.EndUpdate();
            AppendLog($"Total {listViewResults.Items.Count} File Display");
        }

        /// <summary>
        /// Display 모드 변경 when
        /// </summary>
        private void rbShowMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbShowImages.Checked || rbShowLabels.Checked)
            {
                ClearPreview();
                LoadGeneratedResults();
            }
        }

        /// <summary>
        /// 리스트 Select 변경 when Preview
        /// </summary>
        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selectedItem = listViewResults.SelectedItems[0];
            string filePath = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                lblPreviewInfo.Text = "File not found.";
                ClearPreview();
                return;
            }

            try
            {
                if (rbShowImages.Checked)
                {
                    // Image + Detection Display Results
                    ShowImageWithDetection(filePath);
                }
                else
                {
                    // JSON Parsing + Image Display
                    ShowImageWithJsonLabels(filePath);
                }
            }
            catch (Exception ex)
            {
                lblPreviewInfo.Text = $"Preview load failed: {ex.Message}";
                ClearPreview();
                AppendLog($"Preview load failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Image + Detection Display Results
        /// </summary>
        private void ShowImageWithDetection(string imagePath)
        {
            ClearPreview();
            float minConf = _config.LabelGeneration.MinConfidence;

            using (Mat image = Cv2.ImRead(imagePath, ImreadModes.Color))
            {
                if (image.Empty())
                {
                    lblPreviewInfo.Text = "Image load failed";
                    return;
                }

                // Image Size 비례한 굵기 및 폰트 Size 계산
                int imageSize = Math.Max(image.Width, image.Height);
                int thickness = Math.Max(2, imageSize / 500);  // Minimum 2, Image Size 비례
                double fontSize = Math.Max(0.8, imageSize / 1000.0);  // Minimum 0.8
                int fontThickness = Math.Max(2, thickness);

                // Detection 수행
                if (!DiceManager.IsDetectModelLoaded(GetConfiguredDetectModelPath()) &&
                    !EnsureDetectionModelLoadedWithPopup("Loading detection model...\r\nPreview will continue when loading is complete."))
                {
                    lblPreviewInfo.Text = "Detection model is not loaded.";
                    return;
                }

                var results = TestUtilApp.Dice.DiceManager.DetectModel.Inference(image);

                if (results != null && results.Count > 0)
                {
                    // Detection Results 그리기
                    foreach (var rect in results[0].listRect)
                    {
                        if (rect.conf < minConf)
                        {
                            //OnLogMessage?.Invoke($"Skipped low-confidence result: {rect.class_name}, {rect.conf:F3} < {minConf:F3}");
                            continue;
                        }

                        Scalar classColor = GetClassColor(rect.class_name);

                        Cv2.Rectangle(image,
                            new OpenCvSharp.Point(rect.rect.X, rect.rect.Y),
                            new OpenCvSharp.Point(rect.rect.X + rect.rect.Width, rect.rect.Y + rect.rect.Height),
                            classColor, thickness);

                        // Label 텍스트 Ready
                        string label = $"{rect.class_name} {rect.conf:F2}";

                        // 텍스트 Size 측정
                        int baseline = 0;
                        var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheySimplex, fontSize, fontThickness, out baseline);

                        // 텍스트 배경 그리기 (검은색 배경)
                        int textX = (int)rect.rect.X;
                        int textY = (int)rect.rect.Y - 10;
                        if (textY < textSize.Height)
                            textY = (int)(rect.rect.Y + rect.rect.Height + textSize.Height + 10);

                        Cv2.Rectangle(image,
                            new OpenCvSharp.Point(textX, textY - textSize.Height - 5),
                            new OpenCvSharp.Point(textX + textSize.Width + 5, textY + 5),
                            Scalar.Black, -1);

                        Cv2.PutText(image, label, new OpenCvSharp.Point(textX + 2, textY),
                            HersheyFonts.HersheySimplex, fontSize, classColor, fontThickness);
                    }

                    lblPreviewInfo.Text = $"File: {Path.GetFileName(imagePath)}\n" +
                                         $"Size: {image.Width} x {image.Height}\n" +
                                         $"Detect: {results[0].listRect.Count} object(s)";
                }
                else
                {
                    lblPreviewInfo.Text = $"File: {Path.GetFileName(imagePath)}\n" +
                                         $"Size: {image.Width} x {image.Height}\n" +
                                         $"Detect: none";
                }

                // Mat을 Bitmap으로 변환
                _currentPreviewImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                pictureBoxPreview.Image = _currentPreviewImage;
            }
        }

        /// <summary>
        /// JSON Parsing + Image Display
        /// </summary>
        private void ShowImageWithJsonLabels(string jsonPath)
        {
            ClearPreview();

            // JSON File 읽기
            string jsonContent = File.ReadAllText(jsonPath);
            var labelData = JsonConvert.DeserializeObject<LabelData>(jsonContent);

            // 해당 Image File 찾기
            string imageDir = Path.GetDirectoryName(jsonPath);
            string baseName = Path.GetFileNameWithoutExtension(jsonPath);
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };

            string imagePath = null;
            foreach (var ext in imageExtensions)
            {
                string testPath = Path.Combine(imageDir, baseName + ext);
                if (File.Exists(testPath))
                {
                    imagePath = testPath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                lblPreviewInfo.Text = "Matching image file not found.";
                return;
            }

            using (Mat image = Cv2.ImRead(imagePath, ImreadModes.Color))
            {
                if (image.Empty())
                {
                    lblPreviewInfo.Text = "Image load failed";
                    return;
                }

                // Image Size 비례한 굵기 및 폰트 Size 계산
                int imageSize = Math.Max(image.Width, image.Height);
                int thickness = Math.Max(2, imageSize / 500);  // Minimum 2, Image Size 비례
                double fontSize = Math.Max(0.8, imageSize / 1000.0);  // Minimum 0.8
                int fontThickness = Math.Max(2, thickness);

                // JSON Label 그리기
                if (labelData?.shapes != null)
                {
                    foreach (var shape in labelData.shapes)
                    {
                        if (shape.points != null && shape.points.Count >= 2)
                        {
                            int x1 = (int)shape.points[0][0];
                            int y1 = (int)shape.points[0][1];
                            int x2 = (int)shape.points[1][0];
                            int y2 = (int)shape.points[1][1];

                            Scalar classColor = GetClassColor(shape.label);

                            Cv2.Rectangle(image, new OpenCvSharp.Point(x1, y1),
                                new OpenCvSharp.Point(x2, y2), classColor, thickness);

                            // Label 텍스트
                            string label = shape.label;

                            // 텍스트 Size 측정
                            int baseline = 0;
                            var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheySimplex, fontSize, fontThickness, out baseline);

                            // 텍스트 배경 그리기 (검은색 배경)
                            int textX = x1;
                            int textY = y1 - 10;
                            if (textY < textSize.Height)
                                textY = y2 + textSize.Height + 10;

                            Cv2.Rectangle(image,
                                new OpenCvSharp.Point(textX, textY - textSize.Height - 5),
                                new OpenCvSharp.Point(textX + textSize.Width + 5, textY + 5),
                                Scalar.Black, -1);

                            Cv2.PutText(image, label, new OpenCvSharp.Point(textX + 2, textY),
                                HersheyFonts.HersheySimplex, fontSize, classColor, fontThickness);
                        }
                    }

                    lblPreviewInfo.Text = $"File: {Path.GetFileName(imagePath)}\n" +
                                         $"Size: {image.Width} x {image.Height}\n" +
                                         $"Label: {labelData.shapes.Count}";
                }
                else
                {
                    lblPreviewInfo.Text = $"File: {Path.GetFileName(imagePath)}\n" +
                                         $"Size: {image.Width} x {image.Height}\n" +
                                         $"Label: none";
                }

                // Mat을 Bitmap으로 변환
                _currentPreviewImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
                pictureBoxPreview.Image = _currentPreviewImage;
            }
        }

        private Scalar GetClassColor(string className)
        {
            string key = string.IsNullOrWhiteSpace(className) ? "Unknown" : className.Trim();

            Scalar cachedColor;
            if (_classColorCache.TryGetValue(key, out cachedColor))
            {
                return cachedColor;
            }

            int hash = 17;
            foreach (char ch in key.ToUpperInvariant())
            {
                hash = unchecked(hash * 31 + ch);
            }

            int positiveHash = hash & 0x7fffffff;
            double hue = 125.0 + positiveHash % 86; // green through blue range
            double saturation = 0.78 + positiveHash % 18 / 100.0;
            double lightness = 0.52 + positiveHash % 12 / 100.0;

            Color rgbColor = ColorFromHsl(hue, saturation, lightness);
            Scalar scalar = new Scalar(rgbColor.B, rgbColor.G, rgbColor.R);
            _classColorCache[key] = scalar;
            return scalar;
        }

        private static Color ColorFromHsl(double hue, double saturation, double lightness)
        {
            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double huePrime = hue / 60.0;
            double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));

            double r1 = 0;
            double g1 = 0;
            double b1 = 0;

            if (huePrime >= 2 && huePrime < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (huePrime >= 3 && huePrime < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }

            double match = lightness - chroma / 2;
            int r = ClampColor((r1 + match) * 255);
            int g = ClampColor((g1 + match) * 255);
            int b = ClampColor((b1 + match) * 255);

            return Color.FromArgb(r, g, b);
        }

        private static int ClampColor(double value)
        {
            return Math.Max(0, Math.Min(255, (int)Math.Round(value)));
        }

        /// <summary>
        /// 프리뷰 Image 정리
        /// </summary>
        private void ClearPreview()
        {
            if (_currentPreviewImage != null)
            {
                pictureBoxPreview.Image = null;
                _currentPreviewImage.Dispose();
                _currentPreviewImage = null;
            }
        }

        /// <summary>
        /// Results 데이터 정리
        /// </summary>
        private void ClearResults()
        {
            listViewResults.Items.Clear();
            ClearPreview();
            lblPreviewInfo.Text = "Select a file to display preview.";
            _generatedFiles = null;
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
            txtDetectModelPath.Enabled = enabled;
            btnBrowseDetectModel.Enabled = enabled;
            btnLoadDetectModel.Enabled = enabled;
            btnStartLabelGen.Enabled = enabled;
            numMinConfidence.Enabled = enabled;
            chkSkipExistingJson.Enabled = enabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPreview();

                if (_labelGenService != null)
                {
                    _labelGenService.OnProgressUpdate -= OnProgressUpdate;
                    _labelGenService.OnLogMessage -= OnLogMessage;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    #region JSON Label Data Classes
    public class LabelData
    {
        public string version { get; set; }
        public string task_type { get; set; }
        public List<Shape> shapes { get; set; }
        public string split { get; set; }
        public int imageHeight { get; set; }
        public int imageWidth { get; set; }
        public int imageDepth { get; set; }
    }

    public class Shape
    {
        public string label { get; set; }
        public List<List<double>> points { get; set; }
        public object group_id { get; set; }
        public string shape_type { get; set; }
        public Dictionary<string, object> flags { get; set; }
    }
    #endregion
}
