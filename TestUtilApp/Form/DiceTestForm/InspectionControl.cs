using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;
using TestUtilApp.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TestUtilApp.UI
{
    public partial class InspectionControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private List<InspectionResult> _inspectionResults;
        private bool _isProcessing;
        private List<InspectionSpec> _inspectionSpecs;
        private string _specJsonPath;
        private bool _isLoadingConditions;

        private const string ConditionColumnUse = "Use";
        private const string ConditionColumnKeywordEnable = "KeywordEnable";
        private const string ConditionColumnFileNameKeyword = "FileNameKeyword";
        private const string ConditionColumnDetectClasses = "DetectClasses";
        private const string ConditionColumnClassifyModel = "ClassifyModel";
        private const string ConditionColumnMinConfidence = "MinConfidence";

        public event Action NavigateToFileFilter;

        public InspectionControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;
            _inspectionResults = new List<InspectionResult>();
            _inspectionSpecs = new List<InspectionSpec>();

            InitializeListView();
            InitializeConditionGrid();
            LoadInspectionConditionsToGrid();
        }

        public void OnActivated()
        {
            AppendLog("Inspection screen has been activated.");
            LoadInspectionConditionsToGrid();
            UpdateFileFilterPresetLabel();

            if (!string.IsNullOrEmpty(_config.LastClassifySourceFolder))
            {
                txtSourceFolder.Text = _config.LastClassifySourceFolder;
            }

            // Inspection Spec JSON Path Load
            if (!string.IsNullOrEmpty(_config.InspectionSpecJsonPath))
            {
                // 절대 Path인지 Confirm
                if (Path.IsPathRooted(_config.InspectionSpecJsonPath))
                {
                    _specJsonPath = _config.InspectionSpecJsonPath;
                }
                else
                {
                    // 상대 Path면 실행 File 기준으로 변환
                    _specJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _config.InspectionSpecJsonPath);
                }

                txtSpecJsonPath.Text = _specJsonPath;

                // File이 존재하면 자동 Load
                if (File.Exists(_specJsonPath))
                {
                    LoadInspectionSpecs();
                }
                else
                {
                    AppendLog($"Inspection spec file not found: {_specJsonPath}");
                }
            }
        }

        private void InitializeListView()
        {
            listViewResults.View = View.Details;
            listViewResults.FullRowSelect = true;
            listViewResults.GridLines = true;
            listViewResults.MultiSelect = false;

            listViewResults.Columns.Add("Filename", 200);
            listViewResults.Columns.Add("Path", 300);
            listViewResults.Columns.Add("Detect Count", 80);
            listViewResults.Columns.Add("Model A Results", 110);
            listViewResults.Columns.Add("Model B Results", 110);
            listViewResults.Columns.Add("Status", 150);
        }

        private void InitializeConditionGrid()
        {
            dataGridViewConditions.Columns.Clear();
            dataGridViewConditions.AutoGenerateColumns = false;
            dataGridViewConditions.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridViewConditions.CellFormatting += dataGridViewConditions_CellFormatting;
            dataGridViewConditions.CellValueChanged += dataGridViewConditions_CellValueChanged;
            dataGridViewConditions.CellBeginEdit += dataGridViewConditions_CellBeginEdit;
            dataGridViewConditions.CurrentCellDirtyStateChanged += dataGridViewConditions_CurrentCellDirtyStateChanged;

            dataGridViewConditions.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ConditionColumnUse,
                HeaderText = "Use",
                Width = 45
            });

            dataGridViewConditions.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = ConditionColumnKeywordEnable,
                HeaderText = "Keyword",
                Width = 65,
                ToolTipText = "Uncheck to match all files"
            });

            dataGridViewConditions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ConditionColumnFileNameKeyword,
                HeaderText = "Filename Keyword",
                Width = 150
            });

            dataGridViewConditions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ConditionColumnDetectClasses,
                HeaderText = "Detect Classes",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            var modelColumn = new DataGridViewComboBoxColumn
            {
                Name = ConditionColumnClassifyModel,
                HeaderText = "Classify Model",
                Width = 95,
                FlatStyle = FlatStyle.Flat
            };
            modelColumn.Items.AddRange("A", "B");
            dataGridViewConditions.Columns.Add(modelColumn);

            dataGridViewConditions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = ConditionColumnMinConfidence,
                HeaderText = "Min Conf",
                Width = 75,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }

        private void UpdateFileFilterPresetLabel()
        {
            FileFilterProfile activeProfile = FileFilterMatcher.GetActiveProfile(_config.FileFilter);
            string presetName = FileFilterMatcher.GetProfileDisplayName(activeProfile);
            lblFileFilterPreset.Text = $"File Filter Preset: {presetName}";
        }

        private void btnGoToFileFilter_Click(object sender, EventArgs e)
        {
            NavigateToFileFilter?.Invoke();
        }

        private void dataGridViewConditions_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewConditions.CurrentCell == null)
            {
                return;
            }

            int colIndex = dataGridViewConditions.CurrentCell.ColumnIndex;
            if (colIndex == dataGridViewConditions.Columns[ConditionColumnKeywordEnable].Index
                || colIndex == dataGridViewConditions.Columns[ConditionColumnUse].Index)
            {
                dataGridViewConditions.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridViewConditions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == dataGridViewConditions.Columns[ConditionColumnKeywordEnable].Index)
            {
                dataGridViewConditions.InvalidateRow(e.RowIndex);
            }
        }

        private void dataGridViewConditions_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex != dataGridViewConditions.Columns[ConditionColumnFileNameKeyword].Index)
            {
                return;
            }

            bool keywordEnable = Convert.ToBoolean(
                dataGridViewConditions.Rows[e.RowIndex].Cells[ConditionColumnKeywordEnable].Value ?? false);

            if (!keywordEnable)
            {
                e.Cancel = true;
            }
        }

        private void dataGridViewConditions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != dataGridViewConditions.Columns[ConditionColumnFileNameKeyword].Index)
            {
                return;
            }

            bool keywordEnable = Convert.ToBoolean(
                dataGridViewConditions.Rows[e.RowIndex].Cells[ConditionColumnKeywordEnable].Value ?? false);

            if (!keywordEnable)
            {
                e.Value = "(All Files)";
                e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(140, 180, 180, 180);
                e.CellStyle.Font = new System.Drawing.Font(dataGridViewConditions.Font, System.Drawing.FontStyle.Italic);
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                e.FormattingApplied = true;
            }
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            string selected = FolderPickerDialog.Show(this, "Select the folder containing images to inspect.", txtSourceFolder.Text);
            if (selected != null)
            {
                txtSourceFolder.Text = selected;
                _config.LastClassifySourceFolder = selected;
                _configService.SaveConfig(_config);
            }
        }

        private void btnBrowseSpecJson_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Inspection Spec JSON File Select";
                dialog.Filter = "JSON File (*.json)|*.json|All Files (*.*)|*.*";
                dialog.Multiselect = false;

                if (!string.IsNullOrEmpty(_specJsonPath))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(_specJsonPath);
                    dialog.FileName = Path.GetFileName(_specJsonPath);
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _specJsonPath = dialog.FileName;
                    txtSpecJsonPath.Text = _specJsonPath;

                    // config Save
                    _config.InspectionSpecJsonPath = _specJsonPath;
                    _configService.SaveConfig(_config);

                    LoadInspectionSpecs();
                }
            }
        }

        private void LoadInspectionSpecs()
        {
            try
            {
                if (string.IsNullOrEmpty(_specJsonPath) || !File.Exists(_specJsonPath))
                {
                    _inspectionSpecs.Clear();
                    return;
                }

                string jsonContent = File.ReadAllText(_specJsonPath);
                _inspectionSpecs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InspectionSpec>>(jsonContent);

                if (_inspectionSpecs == null)
                {
                    _inspectionSpecs = new List<InspectionSpec>();
                }

                // USE_YN이 Y이고 INSP_YN이 Y인 항목만 Filter링
                _inspectionSpecs = _inspectionSpecs
                    .Where(s => s.UseYn == "Y" && s.IsInspectionEnabled())
                    .ToList();

                AppendLog($"Inspection spec loaded: {_inspectionSpecs.Count} item(s)");

                // Load된 Spec Display Information
                var mcCodes = _inspectionSpecs.Select(s => s.McCode).Distinct().ToList();
                var cameras = _inspectionSpecs.Select(s => s.GetCameraName()).Distinct().ToList();

                AppendLog($"MC_CODE: {string.Join(", ", mcCodes)}");
                AppendLog($"CAMERA: {string.Join(", ", cameras)}");
            }
            catch (Exception ex)
            {
                AppendLog($"Inspection Spec Load Error: {ex.Message}");
                MessageBox.Show($"Inspection Spec File Load Error:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _inspectionSpecs.Clear();
            }
        }

        private void chkUseSpecFilter_CheckedChanged(object sender, EventArgs e)
        {
            bool isEnabled = chkUseSpecFilter.Checked;

            txtSpecJsonPath.Enabled = isEnabled;
            btnBrowseSpecJson.Enabled = isEnabled;

            if (isEnabled)
            {
                // 체크 when config Path를 기본값으로 Settings
                if (string.IsNullOrEmpty(_specJsonPath) && !string.IsNullOrEmpty(_config.InspectionSpecJsonPath))
                {
                    string configPath = _config.InspectionSpecJsonPath;

                    // 상대 Path면 절대 Path로 변환
                    if (!Path.IsPathRooted(configPath))
                    {
                        configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);
                    }

                    _specJsonPath = configPath;
                    txtSpecJsonPath.Text = _specJsonPath;

                    // File이 존재하면 자동 Load
                    if (File.Exists(_specJsonPath))
                    {
                        LoadInspectionSpecs();
                    }
                    else
                    {
                        AppendLog($"Inspection spec file not found: {_specJsonPath}");
                        AppendLog("Click the 'Browse' button to select a file.");
                    }
                }
                else if (string.IsNullOrEmpty(_specJsonPath))
                {
                    AppendLog("Please select an inspection spec JSON file.");
                }
            }
            else
            {
                AppendLog("Inspection spec filter is disabled. All images will be inspected.");
            }
        }

        private void LoadInspectionConditionsToGrid()
        {
            EnsureInspectionConditions();

            _isLoadingConditions = true;
            try
            {
                dataGridViewConditions.Rows.Clear();

                foreach (InspectionConditionRule condition in _config.InspectionConditions)
                {
                    string confDisplay = condition.MinConfidence > 0f
                        ? condition.MinConfidence.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                        : "";
                    bool keywordEnable = !string.IsNullOrWhiteSpace(condition.FileNameKeyword);
                    dataGridViewConditions.Rows.Add(
                        condition.Use,
                        keywordEnable,
                        condition.FileNameKeyword ?? "",
                        FormatKeywords(condition.DetectClassKeywords),
                        NormalizeClassifyModel(condition.ClassifyModel),
                        confDisplay);
                }
            }
            finally
            {
                _isLoadingConditions = false;
            }
        }

        public bool CommitCurrentSettings()
        {
            return SaveInspectionConditionsFromGrid(showLog: false);
        }

        private bool SaveInspectionConditionsFromGrid(bool showLog)
        {
            if (_isLoadingConditions)
            {
                return true;
            }

            try
            {
                if (dataGridViewConditions.IsCurrentCellDirty)
                {
                    dataGridViewConditions.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                dataGridViewConditions.EndEdit();

                var conditions = new List<InspectionConditionRule>();
                foreach (DataGridViewRow row in dataGridViewConditions.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    bool use = Convert.ToBoolean(row.Cells[ConditionColumnUse].Value ?? false);
                    bool keywordEnable = Convert.ToBoolean(row.Cells[ConditionColumnKeywordEnable].Value ?? false);
                    string fileNameKeyword = keywordEnable
                        ? Convert.ToString(row.Cells[ConditionColumnFileNameKeyword].Value)?.Trim() ?? ""
                        : "";
                    string detectClassesText = Convert.ToString(row.Cells[ConditionColumnDetectClasses].Value) ?? "";
                    string classifyModel = NormalizeClassifyModel(Convert.ToString(row.Cells[ConditionColumnClassifyModel].Value));

                    string confText = Convert.ToString(row.Cells[ConditionColumnMinConfidence].Value)?.Trim() ?? "";
                    float minConfidence = 0f;
                    if (!string.IsNullOrEmpty(confText) &&
                        float.TryParse(confText, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out float parsedConf))
                    {
                        minConfidence = Math.Max(0f, Math.Min(1f, parsedConf));
                    }

                    if (string.IsNullOrWhiteSpace(fileNameKeyword) && string.IsNullOrWhiteSpace(detectClassesText))
                    {
                        continue;
                    }

                    conditions.Add(new InspectionConditionRule
                    {
                        Use = use,
                        FileNameKeyword = fileNameKeyword,
                        DetectClassKeywords = ParseKeywords(detectClassesText),
                        ClassifyModel = classifyModel,
                        MinConfidence = minConfidence
                    });
                }

                _config.InspectionConditions = conditions;
                _configService.SaveConfig(_config);

                if (showLog)
                {
                    AppendLog($"Inspection conditions saved: {conditions.Count} rule(s)");
                }

                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Inspection condition save failed: {ex.Message}");
                MessageBox.Show($"Inspection condition save failed:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void EnsureInspectionConditions()
        {
            if (_config.InspectionConditions == null)
            {
                _config.InspectionConditions = new List<InspectionConditionRule>();
            }

            foreach (InspectionConditionRule condition in _config.InspectionConditions)
            {
                if (condition.DetectClassKeywords == null)
                {
                    condition.DetectClassKeywords = new List<string>();
                }

                condition.ClassifyModel = NormalizeClassifyModel(condition.ClassifyModel);
            }
        }

        private List<string> ParseKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return text.Split(',')
                .Select(keyword => keyword.Trim())
                .Where(keyword => !string.IsNullOrEmpty(keyword))
                .ToList();
        }

        private string FormatKeywords(List<string> keywords)
        {
            return keywords == null ? "" : string.Join(", ", keywords);
        }

        private string NormalizeClassifyModel(string classifyModel)
        {
            return string.Equals(classifyModel?.Trim(), "B", StringComparison.OrdinalIgnoreCase)
                ? "B"
                : "A";
        }

        private void btnAddCondition_Click(object sender, EventArgs e)
        {
            dataGridViewConditions.Rows.Add(true, false, "", "", "A", "0.75");
        }

        private void btnRemoveCondition_Click(object sender, EventArgs e)
        {
            if (dataGridViewConditions.SelectedRows.Count == 0)
            {
                return;
            }

            foreach (DataGridViewRow row in dataGridViewConditions.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    dataGridViewConditions.Rows.Remove(row);
                }
            }
        }

        private void btnSaveConditions_Click(object sender, EventArgs e)
        {
            SaveInspectionConditionsFromGrid(showLog: true);
        }

        private async void btnStartInspection_Click(object sender, EventArgs e)
        {
            if (_isProcessing)
            {
                MessageBox.Show("Inspection is already in progress.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtSourceFolder.Text) || !Directory.Exists(txtSourceFolder.Text))
            {
                MessageBox.Show("Please select a valid source folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SaveInspectionConditionsFromGrid(showLog: true))
            {
                return;
            }

            // Inspection Spec Filter Use when 검증
            if (chkUseSpecFilter.Checked)
            {
                if (string.IsNullOrEmpty(_specJsonPath) || !File.Exists(_specJsonPath))
                {
                    MessageBox.Show("Please select an inspection spec JSON file.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_inspectionSpecs == null || _inspectionSpecs.Count == 0)
                {
                    MessageBox.Show("No valid inspection spec was found.\nPlease check the JSON file.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (!ShowInspectionConditionsGuide())
            {
                return;
            }

            _isProcessing = true;
            btnStartInspection.Enabled = false;
            btnClearResults.Enabled = false;
            progressBar.Value = 0;

            try
            {
                bool areModelsReady = await EnsureInspectionModelsLoadedWithPopupAsync();
                if (!areModelsReady)
                {
                    throw new InvalidOperationException("One or more DICE models could not be loaded.");
                }

                await Task.Run(() => ProcessInspection());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during inspection: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppendLog($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                btnStartInspection.Enabled = true;
                btnClearResults.Enabled = true;
            }
        }

        private bool ShowInspectionConditionsGuide()
        {
            EnsureInspectionConditions();

            var sb = new System.Text.StringBuilder();

            // File Filter Preset
            FileFilterProfile activeProfile = FileFilterMatcher.GetActiveProfile(_config.FileFilter);
            string presetName = FileFilterMatcher.GetProfileDisplayName(activeProfile);
            sb.AppendLine("[File Filter Preset]");
            sb.AppendLine($"  {presetName}");
            sb.AppendLine();

            // Inspection Conditions
            var activeConditions = _config.InspectionConditions?
                .Where(c => c.Use)
                .ToList() ?? new List<InspectionConditionRule>();

            sb.AppendLine($"[Inspection Conditions]  ({activeConditions.Count} active rule(s))");
            if (activeConditions.Count == 0)
            {
                sb.AppendLine("  (no active conditions)");
            }
            else
            {
                foreach (var cond in activeConditions)
                {
                    string keyword = string.IsNullOrWhiteSpace(cond.FileNameKeyword) ? "(All Files)" : cond.FileNameKeyword;
                    string detectClasses = cond.DetectClassKeywords?.Count > 0
                        ? string.Join(", ", cond.DetectClassKeywords)
                        : "All";
                    string confStr = cond.MinConfidence > 0f
                        ? cond.MinConfidence.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                        : "default";
                    sb.AppendLine($"  • Filename(Keyword Include): {keyword} ");
                    sb.AppendLine($"  • Detect: {detectClasses}");
                    sb.AppendLine($"  • Classify Model: {NormalizeClassifyModel(cond.ClassifyModel)}  |  MinConf: {confStr}");
                    sb.AppendLine($"------------------------------------------------------");
                }
            }
            sb.AppendLine();

            // Spec Filter
            sb.AppendLine("[Spec Filter]");
            if (chkUseSpecFilter.Checked)
            {
                string specFile = string.IsNullOrEmpty(_specJsonPath) ? "(not set)" : Path.GetFileName(_specJsonPath);
                sb.AppendLine($"  Enabled  —  {specFile}  ({_inspectionSpecs?.Count ?? 0} spec(s) loaded)");
            }
            else
            {
                sb.AppendLine("  Disabled");
            }
            sb.AppendLine();
            sb.Append("Start inspection with the above conditions?");

            var result = ThemedDialog.Show(
                this,
                "Inspection Conditions Confirmation",
                sb.ToString(),
                ThemedDialog.DialogButtons.YesNo,
                ThemedDialog.DialogIcon.Question);

            return result == DialogResult.Yes;
        }

        private bool EnsureInspectionModelsLoaded()
        {
            bool allLoaded = true;

            AppendLog("Preparing detection model...");
            bool detectLoaded = DiceManager.EnsureDetectModelLoaded(_config.DiceModels?.DetectModel?.Path);
            AppendLog(detectLoaded ? "Detection model is ready." : "Detection model load failed.");
            allLoaded &= detectLoaded;

            AppendLog("Preparing classification model: ClassifyModel_A");
            bool classifyALoaded = DiceManager.EnsureClassifyModelALoaded(_config.DiceModels?.ClassifyModel_A?.Path);
            AppendLog(classifyALoaded
                ? "Classification model is ready: ClassifyModel_A"
                : "Classification model load failed: ClassifyModel_A");
            allLoaded &= classifyALoaded;

            AppendLog("Preparing classification model: ClassifyModel_B");
            bool classifyBLoaded = DiceManager.EnsureClassifyModelBLoaded(_config.DiceModels?.ClassifyModel_B?.Path);
            AppendLog(classifyBLoaded
                ? "Classification model is ready: ClassifyModel_B"
                : "Classification model load failed: ClassifyModel_B");
            allLoaded &= classifyBLoaded;

            return allLoaded;
        }

        private bool AreInspectionModelsLoaded()
        {
            return DiceManager.IsDetectModelLoaded(_config.DiceModels?.DetectModel?.Path)
                && DiceManager.IsClassifyModelALoaded(_config.DiceModels?.ClassifyModel_A?.Path)
                && DiceManager.IsClassifyModelBLoaded(_config.DiceModels?.ClassifyModel_B?.Path);
        }

        private async Task<bool> EnsureInspectionModelsLoadedWithPopupAsync()
        {
            if (AreInspectionModelsLoaded())
            {
                return true;
            }

            using (var loadingDialog = ModelLoadingDialog.ShowFor(
                this,
                "Loading DICE models...\r\nAuto Inspection will start when loading is complete."))
            {
                bool isLoaded = await Task.Run(() => EnsureInspectionModelsLoaded());
                loadingDialog.CloseSafely();
                return isLoaded;
            }
        }

        private void ProcessInspection()
        {
            string sourceFolder = txtSourceFolder.Text;
            bool useSpecFilter = false;

            Invoke(new Action(() =>
            {
                useSpecFilter = chkUseSpecFilter.Checked;
            }));

            FileFilterProfile activeProfile = FileFilterMatcher.GetActiveProfile(_config.FileFilter);
            var imageFiles = FileFilterMatcher.GetMatchingFiles(sourceFolder, _config.FileFilter, IsSupportedImageFile);

            Invoke(new Action(() =>
            {
                AppendLog($"File filtering preset applied: {FileFilterMatcher.GetProfileDisplayName(activeProfile)}");
                AppendLog($"File filtering selected {imageFiles.Count} image file(s).");
            }));

            // Inspection Spec Apply Filter
            if (useSpecFilter && _inspectionSpecs != null && _inspectionSpecs.Count > 0)
            {
                var filteredFiles = new List<string>();

                foreach (var imagePath in imageFiles)
                {
                    string fileName = Path.GetFileName(imagePath);

                    // Filename서 MC_CODEand CAMERA_NM 매칭 Confirm
                    foreach (var spec in _inspectionSpecs)
                    {
                        // MC_CODE가 Filename 포함되어 있는지 Confirm
                        if (!string.IsNullOrEmpty(spec.McCode) &&
                            fileName.IndexOf(spec.McCode, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // CAMERA_NM Confirm
                            string cameraName = spec.GetCameraName();
                            if (!string.IsNullOrEmpty(cameraName))
                            {
                                // Filename CAMERA_NM이 포함되어 있는지 Confirm
                                if (fileName.IndexOf(cameraName, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    filteredFiles.Add(imagePath);
                                    break; // 하나의 Specand 매칭되면 다음 File로
                                }
                            }
                        }
                    }
                }

                imageFiles = filteredFiles;

                Invoke(new Action(() =>
                {
                    AppendLog($"Inspection spec filter applied: {imageFiles.Count} file(s) selected");
                }));
            }

            if (imageFiles.Count == 0)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("There are no images to process.", "Notice",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
                return;
            }

            Invoke(new Action(() =>
            {
                _inspectionResults.Clear();
                listViewResults.Items.Clear();
                pictureBoxPreview.Image = null;
                progressBar.Maximum = imageFiles.Count;
                AppendLog($"Starting inspection for {imageFiles.Count} image(s)...");
            }));

            int processedCount = 0;
            int successCount = 0;
            int failCount = 0;
            InspectionResult lastResult = null;

            foreach (var imagePath in imageFiles)
            {
                try
                {
                    var result = InspectImage(imagePath);

                    Invoke(new Action(() =>
                    {
                        _inspectionResults.Add(result);
                        AddResultToListView(result);
                        progressBar.Value = ++processedCount;
                        lblProgress.Text = $"{processedCount} / {imageFiles.Count}";

                        if (result.IsSuccess)
                            successCount++;
                        else
                            failCount++;

                        // 마지막 파일만 Preview 에 표시
                        if (processedCount == imageFiles.Count)
                        {
                            lastResult = result;
                            UpdatePreviewImage(lastResult);
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
                    }));
                }
            }

            Invoke(new Action(() =>
            {
                AppendLog($"Inspection complete - Success: {successCount}, Failed: {failCount}");
                MessageBox.Show($"Inspection completed.\n\nSuccess: {successCount}\nFailed: {failCount}",
                    "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private bool IsSupportedImageFile(string filePath)
        {
            return filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
        }

        private InspectionConditionRule GetMatchingInspectionCondition(string imagePath)
        {
            EnsureInspectionConditions();

            string fileName = Path.GetFileName(imagePath);
            return _config.InspectionConditions
                .Where(condition => condition.Use)
                .FirstOrDefault(condition =>
                    string.IsNullOrWhiteSpace(condition.FileNameKeyword) ||
                    fileName.IndexOf(condition.FileNameKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool IsDetectClassAllowed(InspectionConditionRule condition, string detectClassName)
        {
            if (condition?.DetectClassKeywords == null || condition.DetectClassKeywords.Count == 0)
            {
                return true;
            }

            return condition.DetectClassKeywords.Any(keyword =>
                !string.IsNullOrWhiteSpace(keyword) &&
                !string.IsNullOrWhiteSpace(detectClassName) &&
                detectClassName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private string GetDetectClassDisplay(InspectionConditionRule condition)
        {
            var detectClasses = GetRequiredDetectClassKeywords(condition);

            return detectClasses.Count > 0
                ? string.Join(", ", detectClasses)
                : "Detect Class";
        }

        private string GetModelResultsDisplay(List<ClassificationDetail> modelResults)
        {
            return modelResults.Count > 0
                ? string.Join(", ", modelResults.Select(r => r.Result))
                : "-";
        }

        private List<string> GetRequiredDetectClassKeywords(InspectionConditionRule condition)
        {
            return condition?.DetectClassKeywords?
                .Where(keyword => !string.IsNullOrWhiteSpace(keyword))
                .Select(keyword => keyword.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();
        }

        private List<string> GetMissingDetectClassKeywords(InspectionConditionRule condition, IEnumerable<string> detectedClassNames)
        {
            var requiredKeywords = GetRequiredDetectClassKeywords(condition);
            if (requiredKeywords.Count == 0)
            {
                return new List<string>();
            }

            var detectedNames = detectedClassNames?
                .Where(className => !string.IsNullOrWhiteSpace(className))
                .ToList() ?? new List<string>();

            return requiredKeywords
                .Where(keyword => !detectedNames.Any(className =>
                    className.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();
        }

        private bool IsClassificationNg(string classifyResult)
        {
            return !string.IsNullOrWhiteSpace(classifyResult) &&
                classifyResult.IndexOf("NG", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsClassificationOk(string classifyResult)
        {
            return !IsClassificationNg(classifyResult) &&
                !string.IsNullOrWhiteSpace(classifyResult) &&
                classifyResult.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ClassifyCroppedRegion(Mat croppedRegion, string classifyModel, float minConf, out string classifyResult, out float confidence)
        {
            classifyResult = "UNKNOWN";
            confidence = 0f;

            Mat[] arrCropImage = { croppedRegion };
            var clsResult = classifyModel == "B"
                ? DiceManager.ClassifyModel_B.Inference(arrCropImage)
                : DiceManager.ClassifyModel_A.Inference(arrCropImage);

            if (clsResult == null || clsResult.Count == 0)
            {
                return;
            }

            classifyResult = clsResult[0].class_name ?? "UNKNOWN";
            confidence = GetMaxConfidence(clsResult[0].conf);

            if (confidence < minConf)
            {
                classifyResult = "NG";
            }
        }

        private float GetMaxConfidence(IEnumerable<float> confidenceValues)
        {
            if (confidenceValues == null)
            {
                return 0f;
            }

            float maxValue = 0f;
            foreach (float value in confidenceValues)
            {
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }

            return maxValue;
        }

        private InspectionResult InspectImage(string imagePath)
        {
            var result = new InspectionResult
            {
                ImagePath = imagePath,
                FileName = Path.GetFileName(imagePath)
            };

            InspectionConditionRule condition = GetMatchingInspectionCondition(imagePath);
            if (condition == null)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "No matching inspection condition";
                return result;
            }

            string classifyModel = NormalizeClassifyModel(condition.ClassifyModel);
            string detectClassDisplay = GetDetectClassDisplay(condition);
            float minConf = condition.MinConfidence > 0f
                ? condition.MinConfidence
                : (_config.Classification?.MinConfidence ?? 0f);

            using (Mat originalImage = Cv2.ImRead(imagePath))
            {
                if (originalImage.Empty())
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Image load failed";
                    return result;
                }

                // 1. Detection 수행
                lock (DiceManager.lockObject)
                {
                    var detectionResults = DiceManager.DetectModel.Inference(originalImage);

                    if (detectionResults == null || detectionResults.Count == 0 || detectionResults[0].listRect == null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = "No detected area";
                        result.StatusOverlayText = $"NG - No Detections {detectClassDisplay}";
                        result.ResultImage = DrawResultOnImage(originalImage, result);
                        return result;
                    }

                    var matchingDetections = detectionResults[0].listRect
                        .Where(detection => IsDetectClassAllowed(condition, detection.class_name))
                        .GroupBy(detection => detection.class_name)
                        .Select(group => group.OrderByDescending(d => d.conf).First())
                        .ToList();

                    result.DetectedCount = matchingDetections.Count;

                    var missingDetectClasses = GetMissingDetectClassKeywords(
                        condition,
                        detectionResults[0].listRect.Select(detection => detection.class_name));

                    if (matchingDetections.Count == 0)
                    {
                        string missingDetectClassDisplay = missingDetectClasses.Count > 0
                            ? string.Join(", ", missingDetectClasses)
                            : detectClassDisplay;
                        result.IsSuccess = false;
                        result.ErrorMessage = $"No detections: {missingDetectClassDisplay}";
                        result.StatusOverlayText = $"NG - No Detections {missingDetectClassDisplay}";
                        result.ResultImage = DrawResultOnImage(originalImage, result);
                        return result;
                    }

                    // 2. 각 Detect Area 대해 분류 수행
                    foreach (var detection in matchingDetections)
                    {
                        string className = detection.class_name;
                        OpenCvSharp.Rect2f bbox = detection.rect;

                        // Area 크롭
                        OpenCvSharp.Rect safeRect = GetSafeRect(bbox, originalImage.Width, originalImage.Height);
                        using (Mat croppedRegion = new Mat(originalImage, safeRect))
                        {
                            ClassifyCroppedRegion(croppedRegion, classifyModel, minConf, out string classifyResult, out float confidence);

                            var detail = new ClassificationDetail
                            {
                                ClassName = className,
                                BoundingBox = safeRect,
                                Result = classifyResult,
                                Confidence = confidence,
                                ClassifyModel = classifyModel
                            };

                            if (classifyModel == "A")
                            {
                                result.Model_A_Results.Add(detail);
                            }
                            else
                            {
                                result.Model_B_Results.Add(detail);
                            }
                        }
                    }

                    var classificationNg = result.Model_A_Results
                        .Concat(result.Model_B_Results)
                        .FirstOrDefault((detail =>
                         (IsClassificationNg(detail.Result)) ||
                         (IsClassificationOk(detail.Result) && detail.Confidence < minConf)
                         ));

                    if (missingDetectClasses.Count > 0)
                    {
                        string missingDetectClassDisplay = string.Join(", ", missingDetectClasses);
                        result.IsSuccess = false;
                        result.ErrorMessage = $"No detections: {missingDetectClassDisplay}";
                        result.StatusOverlayText = $"NG - No Detections {missingDetectClassDisplay}";
                    }
                    else if (classificationNg != null)
                    {
                        result.IsSuccess = false;
                        result.ErrorMessage = $"Classification NG: {classificationNg.ClassName}";
                        result.StatusOverlayText = $"NG - Classification {classificationNg.ClassName}";
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.StatusOverlayText = "OK";
                    }

                    // 3. Results Image Generate
                    result.ResultImage = DrawResultOnImage(originalImage, result);
                }
            }

            return result;
        }

        private Mat DrawResultOnImage(Mat originalImage, InspectionResult result)
        {
            Mat resultImage = originalImage.Clone();

            // Image Size 따른 동적 스케일 계산 (기준: 1000x1000 Image)
            int baseSize = 1000;
            double scaleFactor = Math.Sqrt((originalImage.Width * originalImage.Height) / (double)(baseSize * baseSize));
            scaleFactor = Math.Max(0.5, Math.Min(scaleFactor, 5.0)); // 0.5 ~ 5.0 범위로 제한

            // 동적으로 계산된 값
            int lineThickness = Math.Max(2, (int)(5 * scaleFactor));
            double fontSize = Math.Max(0.4, 0.8 * scaleFactor);
            int fontThickness = Math.Max(1, (int)(2 * scaleFactor));
            int textOffset = Math.Max(5, (int)(10 * scaleFactor));

            DrawClassificationDetails(resultImage, result.Model_A_Results, new Scalar(240, 200, 100),
                lineThickness, fontSize, fontThickness, textOffset);
            DrawClassificationDetails(resultImage, result.Model_B_Results, new Scalar(120, 200, 120),
                lineThickness, fontSize, fontThickness, textOffset);
            DrawStatusOverlay(resultImage, result.StatusOverlayText, result.IsSuccess, fontSize, fontThickness);

            return resultImage;
        }

        private void DrawClassificationDetails(
            Mat resultImage,
            IEnumerable<ClassificationDetail> details,
            Scalar okColor,
            int lineThickness,
            double fontSize,
            int fontThickness,
            int textOffset)
        {
            foreach (var detail in details)
            {
                Scalar color = IsClassificationNg(detail.Result)
                    ? new Scalar(0, 0, 255)
                    : okColor;

                Cv2.Rectangle(resultImage, detail.BoundingBox, color, lineThickness);

                string className = string.IsNullOrWhiteSpace(detail.ClassName)
                    ? "Detect Class"
                    : detail.ClassName;
                string label = $"{className}: {detail.Result} ({detail.Confidence:F2})";
                var textOrg = new OpenCvSharp.Point(detail.BoundingBox.X, detail.BoundingBox.Y - textOffset);
                DrawLabel(resultImage, label, textOrg, color, fontSize, fontThickness);
            }
        }

        private void DrawStatusOverlay(Mat resultImage, string message, bool isSuccess, double fontSize, int fontThickness)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            int margin = Math.Max(10, (int)(14 * fontSize));
            Scalar statusColor = isSuccess ? new Scalar(0, 255, 0) : new Scalar(0, 0, 255);
            double statusFontSize = Math.Max(fontSize * 5.0, 4.0);
            int statusFontThickness = Math.Max(fontThickness * 5, 8);

            if (!isSuccess && message.StartsWith("NG", StringComparison.OrdinalIgnoreCase))
            {
                string suffix = message.Length > 2 ? message.Substring(2).TrimStart() : "";
                DrawLabel(resultImage, "NG", new OpenCvSharp.Point(margin, margin),
                    statusColor, statusFontSize, statusFontThickness);

                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    int baseline = 0;
                    var ngSize = Cv2.GetTextSize("NG", HersheyFonts.HersheySimplex,
                        statusFontSize, statusFontThickness, out baseline);
                    int suffixGap = Math.Max(12, (int)(8 * fontSize));
                    double suffixFontSize = Math.Max(statusFontSize / 3.0, fontSize);
                    int suffixFontThickness = Math.Max(statusFontThickness / 3, fontThickness);
                    DrawLabel(resultImage, suffix, new OpenCvSharp.Point(margin + ngSize.Width + suffixGap, margin),
                        statusColor, suffixFontSize, suffixFontThickness);
                }

                return;
            }

            DrawLabel(resultImage, message, new OpenCvSharp.Point(margin, margin),
                statusColor, statusFontSize, statusFontThickness);
        }

        private void DrawLabel(
            Mat image,
            string label,
            OpenCvSharp.Point requestedTextOrg,
            Scalar textColor,
            double fontSize,
            int fontThickness)
        {
            int baseline = 0;
            var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheySimplex, fontSize, fontThickness, out baseline);
            int padding = Math.Max(4, fontThickness + 2);

            int x = Math.Max(0, Math.Min(requestedTextOrg.X, Math.Max(0, image.Width - textSize.Width - (padding * 2))));
            int baselineY = requestedTextOrg.Y;
            int textRectTop = baselineY - textSize.Height - baseline - padding;

            if (textRectTop < 0)
            {
                baselineY = requestedTextOrg.Y + textSize.Height + baseline + padding;
                textRectTop = baselineY - textSize.Height - baseline - padding;
            }

            baselineY = Math.Min(baselineY, image.Height - padding);
            textRectTop = Math.Max(0, baselineY - textSize.Height - baseline - padding);

            var textRect = new OpenCvSharp.Rect(
                x,
                textRectTop,
                Math.Min(textSize.Width + (padding * 2), image.Width - x),
                Math.Min(textSize.Height + baseline + (padding * 2), image.Height - textRectTop));

            Cv2.Rectangle(image, textRect, new Scalar(0, 0, 0), -1);
            Cv2.PutText(image, label, new OpenCvSharp.Point(x + padding, baselineY),
                HersheyFonts.HersheySimplex, fontSize, textColor, fontThickness, LineTypes.AntiAlias);
        }

        private OpenCvSharp.Rect GetSafeRect(OpenCvSharp.Rect rect, int imageWidth, int imageHeight)
        {
            int x = Math.Max(0, rect.X);
            int y = Math.Max(0, rect.Y);
            int width = Math.Min(rect.Width, imageWidth - x);
            int height = Math.Min(rect.Height, imageHeight - y);

            return new OpenCvSharp.Rect(x, y, width, height);
        }

        private OpenCvSharp.Rect GetSafeRect(OpenCvSharp.Rect2f rect, int imageWidth, int imageHeight)
        {
            int x = Math.Max(0, (int)rect.X);
            int y = Math.Max(0, (int)rect.Y);
            int width = Math.Min((int)rect.Width, imageWidth - x);
            int height = Math.Min((int)rect.Height, imageHeight - y);

            return new OpenCvSharp.Rect(x, y, width, height);
        }

        private void AddResultToListView(InspectionResult result)
        {
            var item = new ListViewItem(result.FileName);
            item.SubItems.Add(result.ImagePath);
            item.SubItems.Add(result.DetectedCount.ToString());

            string modelAResults = result.Model_A_Results.Count > 0
                ? string.Join(", ", result.Model_A_Results.Select(r => r.Result))
                : "-";
            string modelBResults = result.Model_B_Results.Count > 0
                ? string.Join(", ", result.Model_B_Results.Select(r => r.Result))
                : "-";

            item.SubItems.Add(modelAResults);
            item.SubItems.Add(modelBResults);
            item.SubItems.Add(result.IsSuccess ? "Success" : $"Failed: {result.ErrorMessage}");
            item.Tag = result;

            if (!result.IsSuccess)
                item.ForeColor = UiTheme.Error;

            listViewResults.Items.Add(item);
        }

        private void listViewResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count == 0)
                return;

            var selectedItem = listViewResults.SelectedItems[0];
            var result = selectedItem.Tag as InspectionResult;

            if (result != null && result.ResultImage != null)
            {
                ShowResultImage(result);
            }
        }

        private void ShowResultImage(InspectionResult result)
        {
            try
            {
                UpdatePreviewImage(result);

                // 상세 Display Information
                AppendLog($"File: {result.FileName}");
                AppendLog($"Detect count: {result.DetectedCount}");

                if (result.Model_A_Results.Count > 0)
                {
                    AppendLog($"Model A Results:");
                    foreach (var detail in result.Model_A_Results)
                    {
                        AppendLog($"  - {detail.ClassName}: {detail.Result} (confidence: {detail.Confidence:F2})");
                    }
                }

                if (result.Model_B_Results.Count > 0)
                {
                    AppendLog($"Model B Results:");
                    foreach (var detail in result.Model_B_Results)
                    {
                        AppendLog($"  - {detail.ClassName}: {detail.Result} (confidence: {detail.Confidence:F2})");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Image Display Error: {ex.Message}");
            }
        }

        private void UpdatePreviewImage(InspectionResult result)
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
            lblPreviewFilename.Text = result.FileName;
            lblPreviewFilename.ForeColor = result.IsSuccess ? UiTheme.Success : UiTheme.Error;
        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to clear all inspection results?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Image 메모리 해제
                foreach (var inspectionResult in _inspectionResults)
                {
                    inspectionResult.ResultImage?.Dispose();
                }

                _inspectionResults.Clear();
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
                AppendLog("Inspection results have been reset.");
            }
        }

        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            if (_inspectionResults.Count == 0)
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

                    foreach (var result in _inspectionResults)
                    {
                        if (result.ResultImage != null)
                        {
                            string outputPath = Path.Combine(saveFolder,
                                Path.GetFileNameWithoutExtension(result.FileName) + "_result.jpg");

                            Cv2.ImWrite(outputPath, result.ResultImage);
                            savedCount++;
                        }
                    }

                    AppendLog($"{savedCount} Results Image Save completed: {saveFolder}");
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
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Results Image 메모리 해제
                foreach (var result in _inspectionResults)
                {
                    result.ResultImage?.Dispose();
                }
                _inspectionResults.Clear();

                if (pictureBoxPreview.Image != null)
                {
                    pictureBoxPreview.Image.Dispose();
                }

                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Inspection Results 를 Save 하는 클래스
    /// </summary>
    public class InspectionResult
    {
        public string ImagePath { get; set; }
        public string FileName { get; set; }
        public int DetectedCount { get; set; }
        public List<ClassificationDetail> Model_A_Results { get; set; } = new List<ClassificationDetail>();
        public List<ClassificationDetail> Model_B_Results { get; set; } = new List<ClassificationDetail>();
        public Mat ResultImage { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string StatusOverlayText { get; set; }
    }

    /// <summary>
    /// 분류 상세 정보
    /// </summary>
    public class ClassificationDetail
    {
        public string ClassName { get; set; }
        public OpenCvSharp.Rect BoundingBox { get; set; }
        public string Result { get; set; }
        public float Confidence { get; set; }
        public string ClassifyModel { get; set; }
    }
}
