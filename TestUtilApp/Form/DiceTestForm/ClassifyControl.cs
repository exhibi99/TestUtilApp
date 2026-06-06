using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class ClassifyControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private ImageProcessorService _imageProcessorService;

        // Classification Results Save
        private Dictionary<string, List<string>> _classificationResults;
        private Image _currentPreviewImage;
        private string _lastOutputFolder;

        public ClassifyControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;

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
            EnsureClassifyModelConfig();
            EnsureClassificationConfig();

            numMinConfidence.Value = Math.Min(
                numMinConfidence.Maximum,
                Math.Max(numMinConfidence.Minimum, (decimal)_config.Classification.MinConfidence));

            // 마지막 Path 복원
            if (!string.IsNullOrEmpty(_config.LastClassifySourceFolder))
            {
                txtSourceFolder.Text = _config.LastClassifySourceFolder;
            }

            // 초기 상태 Settings
            ClearResults();

            // Model Display Information
            UpdateModelInfo();
        }

        /// <summary>
        /// Model Path 및 Load Display Status
        /// </summary>
        private void UpdateModelInfo()
        {
            try
            {
                EnsureClassifyModelConfig();

                // Model A information
                string modelAPath = GetModelPath(nameof(DiceManager.ClassifyModel_A));
                bool isModelALoaded = DiceManager.IsClassifyModelALoaded(modelAPath);

                if (txtModelAPath != null && !txtModelAPath.Focused && txtModelAPath.Text != modelAPath)
                {
                    txtModelAPath.Text = modelAPath;
                }

                if (isModelALoaded)
                {
                    lblModelAInfo.Text = "✓ Loaded";
                    lblModelAInfo.ForeColor = UiTheme.Success;
                }
                else
                {
                    lblModelAInfo.Text = "✗ Not Loaded";
                    lblModelAInfo.ForeColor = UiTheme.Error;
                }

                // Model B information
                string modelBPath = GetModelPath(nameof(DiceManager.ClassifyModel_B));
                bool isModelBLoaded = DiceManager.IsClassifyModelBLoaded(modelBPath);

                if (txtModelBPath != null && !txtModelBPath.Focused && txtModelBPath.Text != modelBPath)
                {
                    txtModelBPath.Text = modelBPath;
                }

                if (isModelBLoaded)
                {
                    lblModelBInfo.Text = "✓ Loaded";
                    lblModelBInfo.ForeColor = UiTheme.Success;
                }
                else
                {
                    lblModelBInfo.Text = "✗ Not Loaded";
                    lblModelBInfo.ForeColor = UiTheme.Error;
                }

                // Log 상세 정보 출력
                AppendLog($"Model A: {(isModelALoaded ? "Loaded" : "Not Loaded")}");
                AppendLog($"  Path: {modelAPath}");
                AppendLog($"Model B: {(isModelBLoaded ? "Loaded" : "Not Loaded")}");
                AppendLog($"  Path: {modelBPath}");
            }
            catch (Exception ex)
            {
                AppendLog($"Model Display Information Error: {ex.Message}");
                lblModelAInfo.Text = "✗ Cannot retrieve information.";
                lblModelAInfo.ForeColor = UiTheme.Error;
                lblModelBInfo.Text = "✗ Cannot retrieve information.";
                lblModelBInfo.ForeColor = UiTheme.Error;
            }
        }

        private void EnsureClassifyModelConfig()
        {
            if (_config.DiceModels == null)
            {
                _config.DiceModels = new DiceModelsConfig();
            }

            if (_config.DiceModels.ClassifyModel_A == null)
            {
                _config.DiceModels.ClassifyModel_A = new DiceModelSetting { Use = true, Path = string.Empty };
            }

            if (_config.DiceModels.ClassifyModel_B == null)
            {
                _config.DiceModels.ClassifyModel_B = new DiceModelSetting { Use = true, Path = string.Empty };
            }
        }

        private void EnsureClassificationConfig()
        {
            if (_config.Classification == null)
            {
                _config.Classification = new ClassificationConfig
                {
                    MinConfidence = 0.5f
                };
            }

            if (_config.Classification.MinConfidence < 0f || _config.Classification.MinConfidence > 1f)
            {
                _config.Classification.MinConfidence = 0.5f;
            }
        }

        /// <summary>
        /// Model Path 가져오기
        /// </summary>
        private string GetModelPath(string modelName)
        {
            try
            {
                EnsureClassifyModelConfig();

                if (modelName == nameof(DiceManager.ClassifyModel_A))
                {
                    return _config.DiceModels.ClassifyModel_A.Path ?? string.Empty;
                }
                else
                {
                    return _config.DiceModels.ClassifyModel_B.Path ?? string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private TextBox GetModelPathTextBox(string modelName)
        {
            return modelName == nameof(DiceManager.ClassifyModel_A) ? txtModelAPath : txtModelBPath;
        }

        private bool SaveClassificationModelPathFromInput(string modelName, bool showValidationMessage, bool appendSavedLog)
        {
            EnsureClassifyModelConfig();

            TextBox pathTextBox = GetModelPathTextBox(modelName);
            string modelPath = pathTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                if (showValidationMessage)
                {
                    MessageBox.Show("Please enter or select a classification model path.", "Input Error",
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
                    AppendLog($"Classification model path was not saved because it does not exist: {modelPath}");
                }
                return false;
            }

            string currentPath = GetModelPath(modelName);
            if (string.Equals(modelPath, currentPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (modelName == nameof(DiceManager.ClassifyModel_A))
            {
                _config.DiceModels.ClassifyModel_A.Use = true;
                _config.DiceModels.ClassifyModel_A.Path = modelPath;
            }
            else
            {
                _config.DiceModels.ClassifyModel_B.Use = true;
                _config.DiceModels.ClassifyModel_B.Path = modelPath;
            }

            try
            {
                _configService.SaveConfig(_config);
                if (appendSavedLog)
                {
                    AppendLog($"Classification model path saved ({modelName}): {modelPath}");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Classification model path save failed: {ex.Message}");
                if (showValidationMessage)
                {
                    MessageBox.Show($"Classification model path save failed: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            UpdateModelInfo();
            return true;
        }

        private bool EnsureClassificationModelLoaded(string modelName)
        {
            string modelPath = GetModelPath(modelName);

            AppendLog($"Preparing classification model: {modelName}");

            bool isLoaded = modelName == nameof(DiceManager.ClassifyModel_A)
                ? DiceManager.EnsureClassifyModelALoaded(modelPath)
                : DiceManager.EnsureClassifyModelBLoaded(modelPath);

            AppendLog(isLoaded
                ? $"Classification model is ready: {modelName}"
                : $"Classification model load failed: {modelName}");

            RefreshModelInfo();
            return isLoaded;
        }

        private async System.Threading.Tasks.Task<bool> EnsureClassificationModelLoadedWithPopupAsync(string modelName)
        {
            string modelPath = GetModelPath(modelName);
            bool alreadyLoaded = modelName == nameof(DiceManager.ClassifyModel_A)
                ? DiceManager.IsClassifyModelALoaded(modelPath)
                : DiceManager.IsClassifyModelBLoaded(modelPath);

            if (alreadyLoaded)
            {
                UpdateModelInfo();
                return true;
            }

            using (var loadingDialog = ModelLoadingDialog.ShowFor(
                this,
                $"Loading classification model...\r\nModel: {modelName}\r\nThis can take a moment. Please wait."))
            {
                bool isLoaded = await System.Threading.Tasks.Task.Run(() => EnsureClassificationModelLoaded(modelName));
                loadingDialog.CloseSafely();
                return isLoaded;
            }
        }

        private void RefreshModelInfo()
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateModelInfo));
                return;
            }

            UpdateModelInfo();
        }

        public void OnActivated()
        {
            AppendLog("Image classification screen has been activated.");
            UpdateModelInfo(); // Model 상태 갱신
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the source folder containing images to classify.";
                if (!string.IsNullOrEmpty(txtSourceFolder.Text))
                {
                    dialog.SelectedPath = txtSourceFolder.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSourceFolder.Text = dialog.SelectedPath;
                    _config.LastClassifySourceFolder = dialog.SelectedPath;

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

        private void BrowseClassificationModelPath(string modelName)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = $"Select the folder containing the classification model: {modelName}";
                string currentPath = GetModelPathTextBox(modelName).Text;
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    dialog.SelectedPath = currentPath;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    GetModelPathTextBox(modelName).Text = dialog.SelectedPath;
                    AppendLog($"Classification model folder selected ({modelName}): {dialog.SelectedPath}");
                    SaveClassificationModelPathFromInput(modelName, false, true);
                }
            }
        }

        private void btnBrowseModelA_Click(object sender, EventArgs e)
        {
            BrowseClassificationModelPath(nameof(DiceManager.ClassifyModel_A));
        }

        private void btnBrowseModelB_Click(object sender, EventArgs e)
        {
            BrowseClassificationModelPath(nameof(DiceManager.ClassifyModel_B));
        }

        private void txtModelAPath_Leave(object sender, EventArgs e)
        {
            SaveClassificationModelPathFromInput(nameof(DiceManager.ClassifyModel_A), false, true);
        }

        private void txtModelBPath_Leave(object sender, EventArgs e)
        {
            SaveClassificationModelPathFromInput(nameof(DiceManager.ClassifyModel_B), false, true);
        }

        private async System.Threading.Tasks.Task LoadClassificationModelFromButtonAsync(string modelName)
        {
            if (!SaveClassificationModelPathFromInput(modelName, true, true))
            {
                return;
            }

            try
            {
                SetUIEnabled(false);
                progressBar.Value = 0;

                bool isLoaded = await EnsureClassificationModelLoadedWithPopupAsync(modelName);
                if (!isLoaded)
                {
                    MessageBox.Show("Classification model load failed.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Classification model loaded.", "Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"Classification model load error: {ex.Message}");
                MessageBox.Show($"Classification model load error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIEnabled(true);
                UpdateModelInfo();
            }
        }

        private async void btnLoadModelA_Click(object sender, EventArgs e)
        {
            await LoadClassificationModelFromButtonAsync(nameof(DiceManager.ClassifyModel_A));
        }

        private async void btnLoadModelB_Click(object sender, EventArgs e)
        {
            await LoadClassificationModelFromButtonAsync(nameof(DiceManager.ClassifyModel_B));
        }

        private async void btnStartClassify_Click(object sender, EventArgs e)
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

            if (_imageProcessorService == null)
            {
                MessageBox.Show("ImageProcessorService not initialized.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Disable UI
                SetUIEnabled(false);
                progressBar.Value = 0;
                txtLog.Clear();
                ClearResults();

                // Select된 Model Confirm
                string selectedModel = GetSelectedModel();
                if (string.IsNullOrEmpty(selectedModel))
                {
                    MessageBox.Show("Please select model to use for classification.", "Input Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetUIEnabled(true);
                    return;
                }

                if (!SaveClassificationModelPathFromInput(selectedModel, true, false))
                {
                    SetUIEnabled(true);
                    return;
                }

                AppendLog($"Classification started: {txtSourceFolder.Text}");
                AppendLog($"Selected model: {selectedModel}");

                EnsureClassificationConfig();
                _config.Classification.MinConfidence = (float)numMinConfidence.Value;

                try
                {
                    _configService.SaveConfig(_config);
                }
                catch (Exception ex)
                {
                    AppendLog($"Classification settings save failed: {ex.Message}");
                }

                AppendLog($"Minimum confidence: {_config.Classification.MinConfidence:F2}");

                bool isModelLoaded = await EnsureClassificationModelLoadedWithPopupAsync(selectedModel);
                if (!isModelLoaded)
                {
                    throw new InvalidOperationException($"The classification model could not be loaded: {selectedModel}");
                }

                // Asynchronous processing
                _ = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        var results = _imageProcessorService.ProcessClassification(
                            txtSourceFolder.Text,
                            selectedModel
                        );

                        this.Invoke(new Action(() =>
                        {
                            // Results Save
                            _classificationResults = results;

                            string parentFolder = Directory.GetParent(txtSourceFolder.Text).FullName;
                            string folderName = Path.GetFileName(txtSourceFolder.Text);
                            _lastOutputFolder = Path.Combine(parentFolder, folderName + "_classified");

                            AppendLog($"Classification completed.");
                            AppendLog($"Processed categories: {results.Count}");

                            foreach (var kvp in results)
                            {
                                AppendLog($"  - {kvp.Key}: {kvp.Value.Count} Image");
                            }

                            AppendLog($"Output Folder: {_lastOutputFolder}");

                            // Display Results
                            DisplayResults();

                            MessageBox.Show($"Classification completed.\nOutput Folder: {_lastOutputFolder}",
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

        /// <summary>
        /// Classification Results를 ListView Display
        /// </summary>
        private void DisplayResults()
        {
            if (_classificationResults == null || _classificationResults.Count == 0)
            {
                AppendLog("There are no results to display.");
                return;
            }

            listViewResults.BeginUpdate();
            listViewResults.Items.Clear();

            foreach (var category in _classificationResults)
            {
                string categoryName = category.Key;
                foreach (string imagePath in category.Value)
                {
                    string fileName = Path.GetFileName(imagePath);

                    ListViewItem item = new ListViewItem(categoryName);
                    item.SubItems.Add(fileName);
                    item.SubItems.Add(imagePath);
                    item.Tag = imagePath; // 전체 Path Save

                    listViewResults.Items.Add(item);
                }
            }

            listViewResults.EndUpdate();
            AppendLog($"Displayed {listViewResults.Items.Count} image result(s).");
        }

        /// <summary>
        /// ListView Select 변경 when Image 프리뷰
        /// </summary>
        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem selectedItem = listViewResults.SelectedItems[0];
            string imagePath = selectedItem.Tag as string;

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                lblPreviewInfo.Text = "Image file not found.";
                ClearPreview();
                return;
            }

            try
            {
                // Existing Image 리소스 해제
                ClearPreview();

                // New Image Load
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    _currentPreviewImage = Image.FromStream(fs);
                }

                pictureBoxPreview.Image = _currentPreviewImage;

                // Display image information
                string category = selectedItem.SubItems[0].Text;
                string fileName = selectedItem.SubItems[1].Text;
                FileInfo fileInfo = new FileInfo(imagePath);

                lblPreviewInfo.Text = $"Category: {category}\n" +
                                     $"Filename: {fileName}\n" +
                                     $"Size: {_currentPreviewImage.Width} x {_currentPreviewImage.Height}\n" +
                                     $"File size: {fileInfo.Length / 1024:N0} KB";
            }
            catch (Exception ex)
            {
                lblPreviewInfo.Text = $"Image load failed: {ex.Message}";
                ClearPreview();
                AppendLog($"Preview load failed: {ex.Message}");
            }
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
            lblPreviewInfo.Text = "Select an image to display preview.";
            _classificationResults = null;
            _lastOutputFolder = null;
        }

        /// <summary>
        /// Select된 분류 Model 가져오기
        /// </summary>
        private string GetSelectedModel()
        {
            // 라디오 버튼 또는 콤보박스서 Select된 Model 가져오기
            // Default value으로 ClassifyModel_A Use
            if (rbModelA != null && rbModelA.Checked)
                return nameof(TestUtilApp.Dice.DiceManager.ClassifyModel_A);

            if (rbModelB != null && rbModelB.Checked)
                return nameof(TestUtilApp.Dice.DiceManager.ClassifyModel_B);

            // Default value
            return nameof(TestUtilApp.Dice.DiceManager.ClassifyModel_A);
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
            btnStartClassify.Enabled = enabled;
            if (rbModelA != null) rbModelA.Enabled = enabled;
            if (rbModelB != null) rbModelB.Enabled = enabled;
            if (txtModelAPath != null) txtModelAPath.Enabled = enabled;
            if (txtModelBPath != null) txtModelBPath.Enabled = enabled;
            if (btnBrowseModelA != null) btnBrowseModelA.Enabled = enabled;
            if (btnBrowseModelB != null) btnBrowseModelB.Enabled = enabled;
            if (btnLoadModelA != null) btnLoadModelA.Enabled = enabled;
            if (btnLoadModelB != null) btnLoadModelB.Enabled = enabled;
            if (numMinConfidence != null) numMinConfidence.Enabled = enabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 프리뷰 Image 정리
                ClearPreview();

                if (_imageProcessorService != null)
                {
                    _imageProcessorService.OnProgressUpdate -= OnProgressUpdate;
                    _imageProcessorService.OnLogMessage -= OnLogMessage;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
