using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class FilterControl : UserControl, IActivatable
    {
        private const int FilterProfileCount = 3;

        private AppConfig _config;
        private ConfigService _configService;
        private TextBox[] _includeKeywordTextBoxes;
        private TextBox[] _includeFileKeywordTextBoxes;
        private TextBox[] _excludeKeywordTextBoxes;
        private TextBox[] _excludeExtensionTextBoxes;
        private Label[] _includeKeywordLabels;
        private Label[] _includeFileKeywordLabels;
        private Label[] _excludeKeywordLabels;
        private Label[] _excludeExtensionLabels;
        private CheckBox[] _useFolderIncludeCheckBoxes;
        private CheckBox[] _useFileIncludeCheckBoxes;
        private CheckBox[] _useFileExcludeCheckBoxes;
        private CheckBox[] _useExtensionExcludeCheckBoxes;
        private bool _isInitializingProfiles;

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        public FilterControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;

            InitializeUI();
        }

        private void InitializeUI()
        {
            EnsureFileFilterConfig();
            InitializeProfileTabs();
            LoadFilterProfilesToUI();

            if (!string.IsNullOrEmpty(_config.FileFilter.LastTargetFolder))
            {
                txtTargetFolder.Text = _config.FileFilter.LastTargetFolder;
            }
            if (!string.IsNullOrEmpty(_config.FileFilter.OutputPostfix))
            {
                txtOutputPostfix.Text = _config.FileFilter.OutputPostfix;
            }
            if (!string.IsNullOrEmpty(_config.FileFilter.LastOutputFolder))
            {
                txtOutputFolder.Text = _config.FileFilter.LastOutputFolder;
            }

            rbOutputCustom.Checked = _config.FileFilter.UseCustomOutputFolder;
            rbOutputAuto.Checked = !_config.FileFilter.UseCustomOutputFolder;

            bool useCopyFolderCount = _config.FileFilter.CopyFolderCount > 0;
            chkCopyFolderCount.Checked = useCopyFolderCount;
            if (useCopyFolderCount)
            {
                numCopyFolderCount.Value = Math.Max(1, Math.Min(_config.FileFilter.CopyFolderCount, (int)numCopyFolderCount.Maximum));
            }
            numCopyFolderCount.Enabled = useCopyFolderCount;

            SetConditionControlsEnabled(true);
            UpdateOutputControlsEnabled();
            ApplyTabControlNativeTheme();
        }

        private void InitializeProfileTabs()
        {
            _includeKeywordLabels = new[] { label2, labelIncludeKeywordsProfile2, labelIncludeKeywordsProfile3 };
            _includeKeywordTextBoxes = new[] { txtIncludeKeywords, txtIncludeKeywordsProfile2, txtIncludeKeywordsProfile3 };
            _includeFileKeywordLabels = new[] { labelIncludeFileKeywords, labelIncludeFileKeywordsProfile2, labelIncludeFileKeywordsProfile3 };
            _includeFileKeywordTextBoxes = new[] { txtIncludeFileKeywords, txtIncludeFileKeywordsProfile2, txtIncludeFileKeywordsProfile3 };
            _excludeKeywordLabels = new[] { label3, labelExcludeKeywordsProfile2, labelExcludeKeywordsProfile3 };
            _excludeKeywordTextBoxes = new[] { txtExcludeKeywords, txtExcludeKeywordsProfile2, txtExcludeKeywordsProfile3 };
            _excludeExtensionLabels = new[] { label5, labelExcludeExtensionsProfile2, labelExcludeExtensionsProfile3 };
            _excludeExtensionTextBoxes = new[] { txtExcludeExtensions, txtExcludeExtensionsProfile2, txtExcludeExtensionsProfile3 };
            _useFolderIncludeCheckBoxes = new[] { chkUseFolderIncludeProfile1, chkUseFolderIncludeProfile2, chkUseFolderIncludeProfile3 };
            _useFileIncludeCheckBoxes = new[] { chkUseFileIncludeProfile1, chkUseFileIncludeProfile2, chkUseFileIncludeProfile3 };
            _useFileExcludeCheckBoxes = new[] { chkUseFileExcludeProfile1, chkUseFileExcludeProfile2, chkUseFileExcludeProfile3 };
            _useExtensionExcludeCheckBoxes = new[] { chkUseExtensionExcludeProfile1, chkUseExtensionExcludeProfile2, chkUseExtensionExcludeProfile3 };
        }

        private void EnsureFileFilterConfig()
        {
            if (_config.FileFilter == null)
            {
                _config.FileFilter = new FileFilterConfig();
            }

            if (string.IsNullOrEmpty(_config.FileFilter.OutputPostfix))
            {
                _config.FileFilter.OutputPostfix = "_filtered";
            }
            if (_config.FileFilter.LastOutputFolder == null)
            {
                _config.FileFilter.LastOutputFolder = "";
            }

            if (_config.FileFilter.ConditionProfiles == null)
            {
                _config.FileFilter.ConditionProfiles = new List<FileFilterProfile>();
            }

            if (_config.FileFilter.ConditionProfiles.Count == 0)
            {
                _config.FileFilter.ConditionProfiles.Add(new FileFilterProfile
                {
                    Name = "Preset 1",
                    IncludeFolderKeywords = new List<string> { "LED", "Blade" },
                    IncludeFileKeywords = new List<string>(),
                    ExcludeFileKeywords = new List<string> { "_temp", "backup" },
                    ExcludeExtensions = new List<string> { "tmp", "bak" },
                    UseFolderIncludeFilter = true,
                    UseFileIncludeFilter = false,
                    UseFileExcludeFilter = true,
                    UseExtensionExcludeFilter = true
                });
            }

            while (_config.FileFilter.ConditionProfiles.Count < FilterProfileCount)
            {
                int presetNumber = _config.FileFilter.ConditionProfiles.Count + 1;
                _config.FileFilter.ConditionProfiles.Add(new FileFilterProfile
                {
                    Name = $"Preset {presetNumber}",
                    IncludeFolderKeywords = new List<string>(),
                    IncludeFileKeywords = new List<string>(),
                    ExcludeFileKeywords = new List<string>(),
                    ExcludeExtensions = new List<string>(),
                    UseFolderIncludeFilter = true,
                    UseFileIncludeFilter = false,
                    UseFileExcludeFilter = true,
                    UseExtensionExcludeFilter = true
                });
            }

            for (int i = 0; i < _config.FileFilter.ConditionProfiles.Count; i++)
            {
                var profile = _config.FileFilter.ConditionProfiles[i];
                if (string.IsNullOrEmpty(profile.Name))
                {
                    profile.Name = $"Preset {i + 1}";
                }
                if (profile.IncludeFolderKeywords == null)
                {
                    profile.IncludeFolderKeywords = new List<string>();
                }
                if (profile.ExcludeFileKeywords == null)
                {
                    profile.ExcludeFileKeywords = new List<string>();
                }
                if (profile.IncludeFileKeywords == null)
                {
                    profile.IncludeFileKeywords = new List<string>();
                }
                if (profile.ExcludeExtensions == null)
                {
                    profile.ExcludeExtensions = new List<string>();
                }

            }

            if (_config.FileFilter.ActiveProfileIndex < 0 || _config.FileFilter.ActiveProfileIndex >= FilterProfileCount)
            {
                _config.FileFilter.ActiveProfileIndex = 0;
            }
        }

        private void LoadFilterProfilesToUI()
        {
            _isInitializingProfiles = true;

            for (int i = 0; i < FilterProfileCount; i++)
            {
                var profile = _config.FileFilter.ConditionProfiles[i];
                tabFilterProfiles.TabPages[i].Text = profile.Name;
                _includeKeywordTextBoxes[i].Text = string.Join(", ", profile.IncludeFolderKeywords);
                _includeFileKeywordTextBoxes[i].Text = string.Join(", ", profile.IncludeFileKeywords);
                _excludeKeywordTextBoxes[i].Text = string.Join(", ", profile.ExcludeFileKeywords);
                _excludeExtensionTextBoxes[i].Text = string.Join(", ", profile.ExcludeExtensions);
                _useFolderIncludeCheckBoxes[i].Checked = profile.UseFolderIncludeFilter;
                _useFileIncludeCheckBoxes[i].Checked = profile.UseFileIncludeFilter;
                _useFileExcludeCheckBoxes[i].Checked = profile.UseFileExcludeFilter;
                _useExtensionExcludeCheckBoxes[i].Checked = profile.UseExtensionExcludeFilter;
            }

            tabFilterProfiles.SelectedIndex = _config.FileFilter.ActiveProfileIndex;
            _isInitializingProfiles = false;
            SetConditionControlsEnabled(true);
        }

        private void SaveFilterProfilesFromUI()
        {
            EnsureFileFilterConfig();

            for (int i = 0; i < FilterProfileCount; i++)
            {
                var profile = _config.FileFilter.ConditionProfiles[i];
                profile.IncludeFolderKeywords = ParseKeywords(_includeKeywordTextBoxes[i].Text);
                profile.IncludeFileKeywords = ParseKeywords(_includeFileKeywordTextBoxes[i].Text);
                profile.ExcludeFileKeywords = ParseKeywords(_excludeKeywordTextBoxes[i].Text);
                profile.ExcludeExtensions = ParseExtensions(_excludeExtensionTextBoxes[i].Text);
                profile.UseFolderIncludeFilter = _useFolderIncludeCheckBoxes[i].Checked;
                profile.UseFileIncludeFilter = _useFileIncludeCheckBoxes[i].Checked;
                profile.UseFileExcludeFilter = _useFileExcludeCheckBoxes[i].Checked;
                profile.UseExtensionExcludeFilter = _useExtensionExcludeCheckBoxes[i].Checked;
            }

            _config.FileFilter.ActiveProfileIndex = tabFilterProfiles.SelectedIndex;
        }

        private FileFilterProfile GetActiveFilterProfile()
        {
            int profileIndex = tabFilterProfiles.SelectedIndex;
            if (profileIndex < 0 || profileIndex >= FilterProfileCount)
            {
                profileIndex = 0;
            }

            return _config.FileFilter.ConditionProfiles[profileIndex];
        }

        public void OnActivated()
        {
            AppendLog("File Filtering screen has been activated.");
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the target folder to filter.";
                if (!string.IsNullOrEmpty(txtTargetFolder.Text))
                {
                    dialog.SelectedPath = txtTargetFolder.Text;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtTargetFolder.Text = dialog.SelectedPath;

                    // Config Save
                    if (_config.FileFilter == null)
                    {
                        EnsureFileFilterConfig();
                    }
                    _config.FileFilter.LastTargetFolder = dialog.SelectedPath;

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

        private void conditionFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInitializingProfiles)
            {
                return;
            }

            SetConditionControlsEnabled(true);
        }

        private void tabFilterProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializingProfiles)
            {
                return;
            }

            tabFilterProfiles.Invalidate();

            try
            {
                SaveFilterProfilesFromUI();
                _configService.SaveConfig(_config);
                AppendLog($"Filter preset selected: {GetActiveFilterProfile().Name}");
            }
            catch (Exception ex)
            {
                AppendLog($"Settings Save failed: {ex.Message}");
            }
        }

        private void tabFilterProfiles_HandleCreated(object sender, EventArgs e)
        {
            ApplyTabControlNativeTheme();
        }

        private void tabFilterProfiles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= tabFilterProfiles.TabPages.Count)
            {
                return;
            }

            if (e.Index == 0)
            {
                PaintProfileTabBackground(e.Graphics);
            }

            DrawProfileTab(e.Graphics, e.Index);
        }

        private void PaintProfileTabBackground(Graphics graphics)
        {
            Rectangle pageBounds = tabFilterProfiles.DisplayRectangle;
            pageBounds.Inflate(2, 2);

            int headerHeight = Math.Max(tabFilterProfiles.ItemSize.Height + 8, tabFilterProfiles.DisplayRectangle.Top);
            Rectangle headerBounds = new Rectangle(0, 0, tabFilterProfiles.Width, headerHeight);

            using (var headerBrush = new SolidBrush(UiTheme.Window))
            using (var pageBrush = new SolidBrush(UiTheme.Surface))
            using (var borderPen = new Pen(UiTheme.Border))
            using (var headerRegion = new Region(headerBounds))
            {
                for (int i = 0; i < tabFilterProfiles.TabPages.Count; i++)
                {
                    headerRegion.Exclude(tabFilterProfiles.GetTabRect(i));
                }

                graphics.FillRegion(headerBrush, headerRegion);
                graphics.FillRectangle(pageBrush, pageBounds);
                graphics.DrawRectangle(borderPen, pageBounds.X, pageBounds.Y, pageBounds.Width - 1, pageBounds.Height - 1);
            }
        }

        private void DrawProfileTab(Graphics graphics, int index)
        {
            Rectangle bounds = tabFilterProfiles.GetTabRect(index);
            bounds.Inflate(-1, -1);
            bool selected = index == tabFilterProfiles.SelectedIndex;

            using (var backBrush = new SolidBrush(selected ? UiTheme.Accent : UiTheme.SurfaceAlt))
            using (var borderPen = new Pen(selected ? UiTheme.Accent : UiTheme.Border))
            {
                graphics.FillRectangle(backBrush, bounds);
                graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            TextRenderer.DrawText(
                graphics,
                tabFilterProfiles.TabPages[index].Text,
                tabFilterProfiles.Font,
                bounds,
                selected ? Color.White : UiTheme.TextMuted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void ApplyTabControlNativeTheme()
        {
            if (tabFilterProfiles == null || !tabFilterProfiles.IsHandleCreated)
            {
                return;
            }

            SetWindowTheme(tabFilterProfiles.Handle, "", "");
            tabFilterProfiles.Invalidate();
        }

        private void outputFolderMode_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOutputControlsEnabled();
        }

        private void chkCopyFolderCount_CheckedChanged(object sender, EventArgs e)
        {
            numCopyFolderCount.Enabled = chkCopyFolderCount.Checked;
        }

        private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the output folder.";

                if (!string.IsNullOrWhiteSpace(txtOutputFolder.Text))
                {
                    dialog.SelectedPath = txtOutputFolder.Text;
                }
                else if (!string.IsNullOrWhiteSpace(txtTargetFolder.Text))
                {
                    string parentFolder = Directory.GetParent(txtTargetFolder.Text)?.FullName;
                    if (!string.IsNullOrEmpty(parentFolder))
                    {
                        dialog.SelectedPath = parentFolder;
                    }
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtOutputFolder.Text = dialog.SelectedPath;
                    rbOutputCustom.Checked = true;

                    try
                    {
                        EnsureFileFilterConfig();
                        _config.FileFilter.LastOutputFolder = dialog.SelectedPath;
                        _config.FileFilter.UseCustomOutputFolder = true;
                        _configService.SaveConfig(_config);
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Settings Save failed: {ex.Message}");
                    }
                }
            }
        }

        private void btnOpenOutputFolder_Click(object sender, EventArgs e)
        {
            string outputFolder;

            try
            {
                outputFolder = GetSelectedOutputFolder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid output folder: {ex.Message}", "Output Folder",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                MessageBox.Show("Please select or configure an output folder first.", "Output Folder",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(outputFolder))
            {
                MessageBox.Show("Output folder does not exist yet.", "Output Folder",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", outputFolder);
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTargetFolder.Text))
            {
                MessageBox.Show("Please select a target folder.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(txtTargetFolder.Text))
            {
                MessageBox.Show("Selected folder does not exist.", "Path Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string targetFolder = txtTargetFolder.Text;
            bool useCustomOutputFolder = rbOutputCustom.Checked;
            string outputFolder;

            try
            {
                outputFolder = GetSelectedOutputFolder(targetFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid output folder: {ex.Message}", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                MessageBox.Show("Please configure an output folder.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsSamePath(outputFolder, targetFolder))
            {
                MessageBox.Show("Output folder cannot be the same as the target folder.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Do you want to copy allowed files from the selected folder?\n\n" +
                $"Target folder: {targetFolder}\n" +
                $"Output folder: {outputFolder}",
                "Confirm Execution",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Disable UI
                SetUIEnabled(false);
                progressBar.Value = 0;
                txtLog.Clear();

                UpdateConfigFromUI();
                var activeProfile = GetActiveFilterProfile();
                List<string> includeFolderKeywords = new List<string>(activeProfile.IncludeFolderKeywords);
                List<string> includeFileKeywords = new List<string>(activeProfile.IncludeFileKeywords);
                List<string> excludeFileKeywords = new List<string>(activeProfile.ExcludeFileKeywords);
                List<string> excludeExtensions = new List<string>(activeProfile.ExcludeExtensions);
                bool useFolderIncludeFilter = activeProfile.UseFolderIncludeFilter;
                bool useFileIncludeFilter = activeProfile.UseFileIncludeFilter;
                bool useFileExcludeFilter = activeProfile.UseFileExcludeFilter;
                bool useExtensionExcludeFilter = activeProfile.UseExtensionExcludeFilter;
                int copyFolderCount = chkCopyFolderCount.Checked ? (int)numCopyFolderCount.Value : 0;

                AppendLog($"File filtering started: {targetFolder}");
                AppendLog($"Filter preset: {activeProfile.Name}");
                AppendLog("Execution mode: copy");
                AppendLog($"Output mode: {(useCustomOutputFolder ? "Selected folder" : "Target folder postfix")}");
                AppendLog($"Output folder: {outputFolder}");
                AppendLog($"Copy Folder Count: {(copyFolderCount > 0 ? $"Max {copyFolderCount} (random selection)" : "Disabled (all folders)")}");

                // Asynchronous processing
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        ExecuteCopyMode(targetFolder, useFolderIncludeFilter, includeFolderKeywords, useFileIncludeFilter, includeFileKeywords, useFileExcludeFilter, excludeFileKeywords, useExtensionExcludeFilter, excludeExtensions, outputFolder, !useCustomOutputFolder, copyFolderCount);

                        this.Invoke(new Action(() =>
                        {
                            AppendLog("File filtering completed.");
                            MessageBox.Show("File filtering completed.", "Complete",
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
        /// UI서 Config 업데이트
        /// </summary>
        private void UpdateConfigFromUI()
        {
            if (CommitCurrentSettings())
            {
                AppendLog("Settings saved.");
            }
        }

        public bool CommitCurrentSettings()
        {
            if (_config.FileFilter == null)
            {
                _config.FileFilter = new FileFilterConfig();
            }

            SaveFilterProfilesFromUI();

            _config.FileFilter.OutputPostfix = txtOutputPostfix.Text;
            _config.FileFilter.LastOutputFolder = txtOutputFolder.Text;
            _config.FileFilter.UseCustomOutputFolder = rbOutputCustom.Checked;
            _config.FileFilter.CopyFolderCount = chkCopyFolderCount.Checked ? (int)numCopyFolderCount.Value : 0;

            try
            {
                _configService.SaveConfig(_config);
                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"Settings Save failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Keyword 문자열 Parsing
        /// </summary>
        private List<string> ParseKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return text.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        private List<string> ParseExtensions(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return text.Split(',')
                .Select(s => s.Trim().TrimStart('.'))
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private string GetSelectedOutputFolder()
        {
            return GetSelectedOutputFolder(txtTargetFolder.Text);
        }

        private string GetSelectedOutputFolder(string targetFolder)
        {
            if (rbOutputCustom.Checked)
            {
                return NormalizeFolderPath(txtOutputFolder.Text);
            }

            if (string.IsNullOrWhiteSpace(targetFolder))
            {
                return "";
            }

            string parentFolder = Directory.GetParent(targetFolder)?.FullName;
            if (string.IsNullOrEmpty(parentFolder))
            {
                return "";
            }

            string folderName = new DirectoryInfo(targetFolder).Name;
            string postfix = string.IsNullOrWhiteSpace(txtOutputPostfix.Text) ? "_filtered" : txtOutputPostfix.Text.Trim();

            return Path.Combine(parentFolder, folderName + postfix);
        }

        private string NormalizeFolderPath(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return "";
            }

            return Path.GetFullPath(folderPath.Trim());
        }

        private bool IsSamePath(string firstPath, string secondPath)
        {
            if (string.IsNullOrWhiteSpace(firstPath) || string.IsNullOrWhiteSpace(secondPath))
            {
                return false;
            }

            string first = NormalizeFolderPath(firstPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string second = NormalizeFolderPath(secondPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 복사 모드 실행
        /// </summary>
        private void ExecuteCopyMode(
            string targetFolder,
            bool useFolderIncludeFilter,
            List<string> includeFolderKeywords,
            bool useFileIncludeFilter,
            List<string> includeFileKeywords,
            bool useFileExcludeFilter,
            List<string> excludeFileKeywords,
            bool useExtensionExcludeFilter,
            List<string> excludeExtensions,
            string outputFolder,
            bool clearOutputFolder,
            int copyFolderCount)
        {
            AppendFilterSummary(useFolderIncludeFilter, includeFolderKeywords, useFileIncludeFilter, includeFileKeywords, useFileExcludeFilter, excludeFileKeywords, useExtensionExcludeFilter, excludeExtensions);

            // Target 폴더 찾기
            List<string> targetFolders = GetTargetFolders(targetFolder, includeFolderKeywords, useFolderIncludeFilter);
            targetFolders = ApplyFileIncludeFolderFilter(targetFolders, useFileIncludeFilter, includeFileKeywords);
            targetFolders = ExcludeOutputFolderFromTargets(targetFolders, outputFolder);

            // Copy Folder Count 제한: 조건에 맞는 폴더 중 랜덤으로 지정 개수만 선택
            if (copyFolderCount > 0 && targetFolders.Count > copyFolderCount)
            {
                int originalCount = targetFolders.Count;
                var rng = new Random();
                targetFolders = targetFolders.OrderBy(x => rng.Next()).Take(copyFolderCount).ToList();
                AppendLog($"Copy Folder Count limit applied: randomly selected {targetFolders.Count} of {originalCount} folders.");
            }

            if (clearOutputFolder && Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
            }

            Directory.CreateDirectory(outputFolder);
            AppendLog($"Output folder ready: {outputFolder}");

            AppendLog($"Target folder count: {targetFolders.Count}");

            int totalCopied = 0;
            int totalProcessed = 0;

            foreach (string folder in targetFolders)
            {
                // 상대 Path 계산
                string relativePath = folder.Substring(targetFolder.Length).TrimStart(Path.DirectorySeparatorChar);
                string destFolder = Path.Combine(outputFolder, relativePath);

                Directory.CreateDirectory(destFolder);
                AppendLog($"Processing: {folder}");

                string[] files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
                totalProcessed += files.Length;

                foreach (string file in files)
                {
                    if (!ShouldExcludeFile(file, useFileExcludeFilter, excludeFileKeywords, useExtensionExcludeFilter, excludeExtensions))
                    {
                        try
                        {
                            string destFile = Path.Combine(destFolder, Path.GetFileName(file));
                            File.Copy(file, destFile, true);
                            totalCopied++;
                            AppendLog($"  Copied: {Path.GetFileName(file)}");
                        }
                        catch (Exception ex)
                        {
                            AppendLog($"  Copy failed: {Path.GetFileName(file)} - {ex.Message}");
                        }
                    }
                }

                UpdateProgress(targetFolders.IndexOf(folder) + 1, targetFolders.Count);
            }

            AppendLog($"Complete: copied {totalCopied} of {totalProcessed} files");
            AppendLog($"Output Folder: {outputFolder}");

            // 빈 폴더 삭제
            AppendLog("Cleaning up empty folders...");
            int deletedFolders = DeleteEmptyFolders(outputFolder);
            AppendLog($"Empty folder cleanup completed: {deletedFolders} folder(s) deleted");
        }

        /// <summary>
        /// 빈 폴더 삭제 (재귀적으로 하위 폴더부터 삭제)
        /// </summary>
        private int DeleteEmptyFolders(string rootFolder)
        {
            int deletedCount = 0;

            try
            {
                // 하위 폴더 먼저 처리 (깊이 우선)
                string[] subFolders = Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories);

                // 가장 깊은 폴더부터 처리하기 위해 역순 정렬
                Array.Sort(subFolders);
                Array.Reverse(subFolders);

                foreach (string folder in subFolders)
                {
                    try
                    {
                        // 폴더가 비어있는지 Confirm (Fileand 하위 폴더 모두 없어야 함)
                        if (IsDirectoryEmpty(folder))
                        {
                            Directory.Delete(folder, false);
                            deletedCount++;
                            AppendLog($"  Empty folder deleted: {folder.Substring(rootFolder.Length).TrimStart(Path.DirectorySeparatorChar)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"  Folder delete failed: {folder} - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Empty folder cleanup error: {ex.Message}");
            }

            return deletedCount;
        }

        /// <summary>
        /// 폴더가 비어있는지 Confirm
        /// </summary>
        private bool IsDirectoryEmpty(string path)
        {
            try
            {
                return !Directory.EnumerateFileSystemEntries(path).Any();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Target 폴더 List 가져오기
        /// </summary>
        private void AppendFilterSummary(
            bool useFolderIncludeFilter,
            List<string> includeFolderKeywords,
            bool useFileIncludeFilter,
            List<string> includeFileKeywords,
            bool useFileExcludeFilter,
            List<string> excludeFileKeywords,
            bool useExtensionExcludeFilter,
            List<string> excludeExtensions)
        {
            AppendLog($"Folder include filter: {(useFolderIncludeFilter ? string.Join(", ", includeFolderKeywords) : "Disabled (all folders)")}");
            AppendLog($"File include filter: {(useFileIncludeFilter ? string.Join(", ", includeFileKeywords) : "Disabled")}");
            AppendLog($"File exclude filter: {(useFileExcludeFilter ? string.Join(", ", excludeFileKeywords) : "Disabled")}");
            AppendLog($"Extension exclude filter: {(useExtensionExcludeFilter ? FormatExtensionsForLog(excludeExtensions) : "Disabled")}");
        }

        private string FormatExtensionsForLog(List<string> excludeExtensions)
        {
            if (excludeExtensions == null || excludeExtensions.Count == 0)
            {
                return "(none)";
            }

            return string.Join(", ", excludeExtensions.Select(extension => "." + extension.TrimStart('.')));
        }

        private List<string> GetTargetFolders(
            string rootFolder,
            List<string> includeKeywords,
            bool useFolderIncludeFilter)
        {
            List<string> result = new List<string>();
            var allFolders = new List<string> { rootFolder };
            allFolders.AddRange(Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories));

            if (!useFolderIncludeFilter || includeKeywords == null || includeKeywords.Count == 0)
            {
                return allFolders;
            }

            foreach (string folder in allFolders)
            {
                string folderName = Path.GetFileName(folder);

                foreach (string keyword in includeKeywords)
                {
                    if (folderName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        result.Add(folder);
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Select folders that contain a file matching the filename include keywords.
        /// </summary>
        private List<string> ApplyFileIncludeFolderFilter(List<string> targetFolders, bool useFileIncludeFilter, List<string> includeFileKeywords)
        {
            if (!useFileIncludeFilter || includeFileKeywords == null || includeFileKeywords.Count == 0)
            {
                return targetFolders;
            }

            var filteredFolders = targetFolders
                .Where(folder => Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                    .Any(file => FileNameContainsKeyword(file, includeFileKeywords)))
                .ToList();

            AppendLog($"File include folder filter matched {filteredFolders.Count} of {targetFolders.Count} folder(s).");
            return filteredFolders;
        }

        private List<string> ExcludeOutputFolderFromTargets(List<string> targetFolders, string outputFolder)
        {
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                return targetFolders;
            }

            string normalizedOutput = NormalizeFolderPath(outputFolder)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return targetFolders
                .Where(folder =>
                {
                    string normalizedFolder = NormalizeFolderPath(folder)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    return !string.Equals(normalizedFolder, normalizedOutput, StringComparison.OrdinalIgnoreCase)
                        && !normalizedFolder.StartsWith(normalizedOutput + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();
        }

        private bool ShouldExcludeFile(
            string filePath,
            bool useFileExcludeFilter,
            List<string> excludeKeywords,
            bool useExtensionExcludeFilter,
            List<string> excludeExtensions)
        {
            string fileName = Path.GetFileName(filePath);

            if (useExtensionExcludeFilter && excludeExtensions != null && excludeExtensions.Count > 0)
            {
                string fileExtension = Path.GetExtension(filePath).TrimStart('.');
                if (excludeExtensions.Any(extension =>
                    string.Equals(extension.TrimStart('.'), fileExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            // Filename Keyword 체크
            if (useFileExcludeFilter && excludeKeywords != null && excludeKeywords.Count > 0)
            {
                foreach (string keyword in excludeKeywords)
                {
                    if (fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool FileNameContainsKeyword(string filePath, List<string> keywords)
        {
            string fileName = Path.GetFileName(filePath);

            foreach (string keyword in keywords)
            {
                if (fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateProgress(int current, int total)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgress(current, total)));
                return;
            }

            if (total > 0)
            {
                progressBar.Maximum = total;
                progressBar.Value = Math.Min(current, total);
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

        private void SetUIEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetUIEnabled(enabled)));
                return;
            }

            btnBrowseTarget.Enabled = enabled;
            // tabFilterProfiles 는 작업 중에도 표시 (내부 TextBox 만 비활성화)
            SetConditionControlsEnabled(enabled);
            UpdateOutputControlsEnabled(enabled);
            btnExecute.Enabled = enabled;
        }

        private void UpdateOutputControlsEnabled()
        {
            UpdateOutputControlsEnabled(true);
        }

        private void UpdateOutputControlsEnabled(bool enabled)
        {
            bool autoOutputEnabled = enabled && rbOutputAuto.Checked;
            bool customOutputEnabled = enabled && rbOutputCustom.Checked;

            // RadioButton, 라벨, 버튼은 작업 중에도 항상 표시
            // TextBox 만 비활성화
            txtOutputPostfix.Enabled = autoOutputEnabled;
            txtOutputFolder.Enabled = customOutputEnabled;
            btnBrowseOutputFolder.Enabled = customOutputEnabled;
            btnOpenOutputFolder.Enabled = enabled;
            numCopyFolderCount.Enabled = enabled && chkCopyFolderCount.Checked;
        }

        private void SetConditionControlsEnabled(bool enabled)
        {
            if (_includeKeywordTextBoxes == null)
            {
                txtIncludeKeywords.Enabled = enabled;
                txtExcludeKeywords.Enabled = enabled;
                return;
            }

            for (int i = 0; i < FilterProfileCount; i++)
            {
                bool folderIncludeEnabled = enabled && _useFolderIncludeCheckBoxes[i].Checked;
                bool fileIncludeEnabled = enabled && _useFileIncludeCheckBoxes[i].Checked;
                bool fileExcludeEnabled = enabled && _useFileExcludeCheckBoxes[i].Checked;
                bool extensionExcludeEnabled = enabled && _useExtensionExcludeCheckBoxes[i].Checked;

                // 체크박스, 그룹박스, 라벨은 작업 중에도 항상 표시
                // TextBox 만 비활성화
                _includeKeywordTextBoxes[i].Enabled = folderIncludeEnabled;
                _includeFileKeywordTextBoxes[i].Enabled = fileIncludeEnabled;
                _excludeKeywordTextBoxes[i].Enabled = fileExcludeEnabled;
                _excludeExtensionTextBoxes[i].Enabled = extensionExcludeEnabled;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
