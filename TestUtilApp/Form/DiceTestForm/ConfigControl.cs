using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class ConfigControl : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;
        private const double PropertyNameColumnRatio = 0.30;
        private const string InitialPropertyGridCategory = "1. Deep Learning";

        public ConfigControl(AppConfig config, ConfigService configService)
        {
            InitializeComponent();
            UiTheme.Apply(this);
            _config = config;
            _configService = configService;

            propertyGridConfig.PropertyValueChanged -= propertyGridConfig_PropertyValueChanged;
            propertyGridConfig.PropertyValueChanged += propertyGridConfig_PropertyValueChanged;
            propertyGridConfig.SelectedGridItemChanged -= propertyGridConfig_SelectedGridItemChanged;
            propertyGridConfig.SelectedGridItemChanged += propertyGridConfig_SelectedGridItemChanged;
            propertyGridConfig.Resize += propertyGridConfig_Resize;
            propertyGridConfig.HandleCreated += propertyGridConfig_HandleCreated;
            propertyGridConfig.ControlAdded += propertyGridConfig_ControlAdded;
            HookPropertyGridKeyHandlers(propertyGridConfig);
        }

        public void OnActivated()
        {
            LoadConfig();
            AppendLog("Settings screen has been activated.");
        }

        private void LoadConfig()
        {
            try
            {
                // PropertyGrid Settings 객체 바인딩
                propertyGridConfig.SelectedObject = _config;
                propertyGridConfig.ExpandAllGridItems();
                BeginInvoke(new Action(() =>
                {
                    ApplyPropertyGridColumnRatio();
                    SelectInitialPropertyGridCategory();
                    HookPropertyGridKeyHandlers(propertyGridConfig);
                }));

                // Display Status
                string statusMessage = $"Execution folder: {_configService.GetConfigFilePath()}";

                if (_configService.HasProjectRootConfig())
                {
                    statusMessage += $"\nProject root: {_configService.GetProjectRootConfigPath()}";
                    AppendLog($"Settings will be saved to both locations.");
                }
                else
                {
                    AppendLog("Project root path not found. Using execution folder only.");
                }

                lblStatus.Text = statusMessage;
                lblStatus.ForeColor = UiTheme.TextPrimary;

                AppendLog("Settings loaded.");
            }
            catch (Exception ex)
            {
                AppendLog($"Settings Load Error: {ex.Message}");
                MessageBox.Show($"Settings Load Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Do you want to save settings?\n\nSome settings may require application restart.",
                    "Settings Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                _configService.SaveConfig(_config);

                lblStatus.Text = $"Save completed: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                lblStatus.ForeColor = UiTheme.Success;

                if (_configService.HasProjectRootConfig())
                {
                    AppendLog("Settings saved. (Execution folder + Project root)");
                }
                else
                {
                    AppendLog("Settings saved. (Execution folder only)");
                }

                MessageBox.Show("Settings saved.", "Save Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Save failed: {ex.Message}";
                lblStatus.ForeColor = UiTheme.Error;

                AppendLog($"Save error: {ex.Message}");
                MessageBox.Show($"Save error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Unsaved changes will be lost.\nDo you want to reload settings?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                _config = _configService.LoadConfig();
                LoadConfig();

                lblStatus.Text = "Settings reloaded.";
                lblStatus.ForeColor = UiTheme.AccentHover;

                AppendLog("Settings reloaded.");
            }
            catch (Exception ex)
            {
                AppendLog($"Reload error: {ex.Message}");
                MessageBox.Show($"Reload error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string configPath = _configService.GetConfigFilePath();
                string folderPath = System.IO.Path.GetDirectoryName(configPath);

                System.Diagnostics.Process.Start("explorer.exe", folderPath);
                AppendLog($"Open Folder: {folderPath}");
            }
            catch (Exception ex)
            {
                AppendLog($"Folder open error: {ex.Message}");
                MessageBox.Show($"Folder open error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void propertyGridConfig_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            AppendLog($"Settings changed: {e.ChangedItem.Label} = {e.ChangedItem.Value}");
        }

        private void propertyGridConfig_SelectedGridItemChanged(object s, SelectedGridItemChangedEventArgs e)
        {
            // DICE Model Path Select when 도움말 Display
            if (e.NewSelection != null && e.NewSelection.PropertyDescriptor != null)
            {
                string propertyName = e.NewSelection.PropertyDescriptor.Name;

                // DICE Model Path 속성인 경우
                if (propertyName == "Path" &&
                    e.NewSelection.Parent != null &&
                    e.NewSelection.Parent.PropertyDescriptor != null)
                {
                    string parentName = e.NewSelection.Parent.PropertyDescriptor.Name;

                    if (parentName.Contains("DetectModel") ||
                        parentName.Contains("ClassifyModel"))
                    {
                        AppendLog("DICE Model Path: Click the '...' button to browse, or type a path and press Enter to save.");
                    }
                }
            }
        }

        private void propertyGridConfig_Resize(object sender, EventArgs e)
        {
            ApplyPropertyGridColumnRatio();
        }

        private void propertyGridConfig_HandleCreated(object sender, EventArgs e)
        {
            BeginInvoke(new Action(ApplyPropertyGridColumnRatio));
        }

        private void propertyGridConfig_ControlAdded(object sender, ControlEventArgs e)
        {
            HookPropertyGridKeyHandlers(e.Control);
        }

        private void propertyGridConfig_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || !IsDiceModelPathGridItem(propertyGridConfig.SelectedGridItem))
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            BeginInvoke(new Action(CommitAndSaveDiceModelPathEdit));
        }

        private void CommitAndSaveDiceModelPathEdit()
        {
            try
            {
                CommitPropertyGridEdit();
                propertyGridConfig.Refresh();
                _configService.SaveConfig(_config);

                lblStatus.Text = $"DICE model path saved: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                lblStatus.ForeColor = UiTheme.Success;
                AppendLog("DICE Model Path saved.");
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"DICE model path save failed: {ex.Message}";
                lblStatus.ForeColor = UiTheme.Error;
                AppendLog($"DICE model path save failed: {ex.Message}");
            }
        }

        private void CommitPropertyGridEdit()
        {
            Control gridView = GetPropertyGridView();
            if (gridView == null)
            {
                return;
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (MethodInfo method in gridView.GetType().GetMethods(flags))
            {
                if (method.Name != "Commit")
                {
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    method.Invoke(gridView, null);
                    return;
                }

                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(bool))
                {
                    method.Invoke(gridView, new object[] { true });
                    return;
                }
            }
        }

        private void ApplyPropertyGridColumnRatio()
        {
            Control gridView = GetPropertyGridView();
            if (gridView == null || gridView.Width <= 0)
            {
                return;
            }

            int splitterPosition = Math.Max(180, (int)(gridView.Width * PropertyNameColumnRatio));
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo moveSplitterTo = gridView.GetType().GetMethod("MoveSplitterTo", flags);
            moveSplitterTo?.Invoke(gridView, new object[] { splitterPosition });
        }

        private Control GetPropertyGridView()
        {
            foreach (Control child in propertyGridConfig.Controls)
            {
                if (child.GetType().Name == "PropertyGridView")
                {
                    return child;
                }
            }

            return null;
        }

        private void SelectInitialPropertyGridCategory()
        {
            GridItem root = GetRootGridItem(propertyGridConfig.SelectedGridItem);
            GridItem category = FindGridItem(root, InitialPropertyGridCategory, GridItemType.Category);

            if (category == null)
            {
                return;
            }

            if (!category.Select())
            {
                SelectFirstChildGridItem(category);
            }
        }

        private GridItem GetRootGridItem(GridItem item)
        {
            while (item != null && item.Parent != null)
            {
                item = item.Parent;
            }

            return item;
        }

        private GridItem FindGridItem(GridItem item, string label, GridItemType type)
        {
            if (item == null)
            {
                return null;
            }

            if (item.GridItemType == type && item.Label == label)
            {
                return item;
            }

            foreach (GridItem child in item.GridItems)
            {
                GridItem found = FindGridItem(child, label, type);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private bool SelectFirstChildGridItem(GridItem item)
        {
            if (item == null)
            {
                return false;
            }

            foreach (GridItem child in item.GridItems)
            {
                if (child.Select() || SelectFirstChildGridItem(child))
                {
                    return true;
                }
            }

            return false;
        }

        private void HookPropertyGridKeyHandlers(Control root)
        {
            if (root == null)
            {
                return;
            }

            root.KeyDown -= propertyGridConfig_KeyDown;
            root.KeyDown += propertyGridConfig_KeyDown;
            root.ControlAdded -= propertyGridConfig_ControlAdded;
            root.ControlAdded += propertyGridConfig_ControlAdded;

            foreach (Control child in root.Controls)
            {
                HookPropertyGridKeyHandlers(child);
            }
        }

        private bool IsDiceModelPathGridItem(GridItem item)
        {
            if (item == null || item.PropertyDescriptor == null || item.PropertyDescriptor.Name != "Path")
            {
                return false;
            }

            GridItem parent = item.Parent;
            while (parent != null)
            {
                if (parent.PropertyDescriptor != null)
                {
                    string name = parent.PropertyDescriptor.Name;
                    if (name == "DetectModel" || name.StartsWith("ClassifyModel", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                parent = parent.Parent;
            }

            return false;
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
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
