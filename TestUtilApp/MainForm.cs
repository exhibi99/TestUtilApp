using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Services;
using TestUtilApp.UI;

namespace TestUtilApp
{
    public partial class MainForm : Form
    {
        private DICETestForm _diceTestForm;
        private VisionTestForm _visionTestForm;
        private AppConfig _config;
        private ConfigService _configService;

        public MainForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);

            _configService = new ConfigService();
            _config = _configService.LoadConfig();

            // Clean up old DLLs and install the correct version BEFORE initializing DiceManager
            // This prevents DLL lock issues when switching versions
            DiceDllInstaller.DeleteOldDlls();
            InstallDiceDlls(silent: true);

            InitializeTabs();

            tabControlMain.SelectedTab = tabDiceTest;
            ActivateSelectedTab();

            ShowGpuInfo();
            RefreshDiceVersionButton();
        }

        private void ShowGpuInfo()
        {
            CudaInfo info = CudaDetector.Detect();
            string pythonVersion = DiceDllInstaller.GetPythonVersion(_config.DiceVersion, info.CudaVersion);
            lblGpuInfo.Text = $"{info.DisplayText}  |  {pythonVersion}";
            lblGpuInfo.ForeColor = info.HasGpu ? UiTheme.Success : UiTheme.TextMuted;
        }

        private void RefreshDiceVersionButton()
        {
            bool isV2 = !string.Equals(_config.DiceVersion, "v1", StringComparison.OrdinalIgnoreCase);
            btnDiceVersion.Text = isV2 ? "DICE  v2" : "DICE  v1";
            btnDiceVersion.BackColor = isV2 ? UiTheme.Accent : UiTheme.TealAccent;
            btnDiceVersion.ForeColor = Color.White;
            btnDiceVersion.FlatAppearance.BorderColor = btnDiceVersion.BackColor;

            // Update GPU info with new Python version
            ShowGpuInfo();
        }

        private void btnDiceVersion_Click(object sender, EventArgs e)
        {
            bool isV2 = !string.Equals(_config.DiceVersion, "v1", StringComparison.OrdinalIgnoreCase);
            _config.DiceVersion = isV2 ? "v1" : "v2";
            _configService.SaveConfig(_config);

            // Update DiceManager with new version
            DiceManager.SetDiceVersion(_config.DiceVersion);

            // DLL switching requires app restart due to file locks in memory
            DialogResult dr = MessageBox.Show(
                "DICE version will be switched on next startup.\n\nPlease click OK to restart the app.",
                "Restart Required",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (dr == DialogResult.OK)
            {
                RestartWithDllUnload();
            }
        }

        private void RestartWithDllUnload()
        {
            try
            {
                // Step 1: Unload all loaded models to release DLL locks
                DiceManager.DetectModel?.UnloadModel();
                DiceManager.ClassifyModel_A?.UnloadModel();
                DiceManager.ClassifyModel_B?.UnloadModel();
                DiceManager.SegmentModel?.UnloadModel();

                // Step 2: Force garbage collection and finalization
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Step 3: Brief delay to allow DLL to be released from memory
                System.Threading.Thread.Sleep(500);

                // Step 4: Wait for DLL locks to be released (max 5 seconds)
                bool lockReleased = WaitForDllUnlock(5000);
                if (!lockReleased)
                {
                    MessageBox.Show(
                        "Some DLL files are still in use.\n\nThe app will restart and try again.",
                        "DLL Unlock Timeout",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during DLL unload: {ex.Message}");
            }

            // Step 5: Restart application
            Application.Restart();
            Environment.Exit(0);
        }

        private bool WaitForDllUnlock(int timeoutMs)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] dllFiles = { "DICE_Library.dll", "Python.Runtime.dll" };
            int checkInterval = 100; // ms
            int elapsed = 0;

            while (elapsed < timeoutMs)
            {
                bool allUnlocked = true;

                foreach (string dllName in dllFiles)
                {
                    string dllPath = Path.Combine(exeDir, dllName);
                    if (!File.Exists(dllPath))
                        continue; // File doesn't exist, consider it unlocked

                    // Try to rename file temporarily to check if it's locked
                    // This is the most reliable way to detect file locks
                    if (IsDllLocked(dllPath))
                    {
                        allUnlocked = false;
                        break;
                    }
                }

                if (allUnlocked)
                    return true; // All DLLs unlocked

                System.Threading.Thread.Sleep(checkInterval);
                elapsed += checkInterval;
            }

            return false; // Timeout reached
        }

        private bool IsDllLocked(string filePath)
        {
            try
            {
                // Most reliable way: try to rename the file temporarily
                // If we can move it, the file is not locked
                string tempPath = filePath + ".tmp";

                // Delete temp file if it exists from previous attempt
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                File.Move(filePath, tempPath);
                File.Move(tempPath, filePath);
                return false; // Not locked - successfully renamed and restored
            }
            catch (IOException ex)
            {
                Console.WriteLine($"DLL locked: {filePath} - {ex.Message}");
                return true; // Locked
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking DLL lock: {ex.Message}");
                return true; // Other errors = assume locked
            }
        }

        private void InstallDiceDlls(bool silent, bool versionChanged = false)
        {
            CudaInfo cudaInfo = CudaDetector.Detect();
            string sourceFolder = DiceDllInstaller.ResolveSourceFolder(_config.DiceVersion, cudaInfo.CudaVersion);

            var result = DiceDllInstaller.Install(_config.DiceVersion, cudaInfo.CudaVersion);

            switch (result)
            {
                case DiceDllInstaller.InstallResult.Success:
                    if (!silent)
                    {
                        DialogResult dr = MessageBox.Show(
                            $"DICE DLL has been updated.\n\nSource: {sourceFolder}\n\nPlease click OK to restart the app.",
                            "DICE DLL Updated",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (dr == DialogResult.OK)
                        {
                            Application.Restart();
                            Environment.Exit(0);
                        }
                    }
                    break;

                case DiceDllInstaller.InstallResult.AlreadyCurrent:
                    if (!silent)
                    {
                        MessageBox.Show(
                            $"DICE DLL is already up to date.\n\nSource: {sourceFolder}",
                            "DICE DLL",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;

                case DiceDllInstaller.InstallResult.FileLocked:
                    if (versionChanged)
                    {
                        DialogResult dr = MessageBox.Show(
                            $"Some DLL files are currently in use and could not be replaced.\n\nSource: {sourceFolder}\n\nPlease click OK to restart the app.",
                            "Restart Required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (dr == DialogResult.OK)
                        {
                            Application.Restart();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Some DLL files are currently in use and could not be replaced.\n\nSource: {sourceFolder}\n\nPlease restart the app to apply the changes.",
                            "Restart Required",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;

                case DiceDllInstaller.InstallResult.SourceNotFound:
                    MessageBox.Show(
                        $"DICE DLL source folder not found:\n{sourceFolder}",
                        "Source Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void InitializeTabs()
        {
            // Set DICE version for DiceManager before creating forms
            DiceManager.SetDiceVersion(_config.DiceVersion);

            _diceTestForm = new DICETestForm();
            _visionTestForm = new VisionTestForm();

            AddTabContent(tabDiceTest, _diceTestForm);
            AddTabContent(tabVisionTest, _visionTestForm);
        }

        private void AddTabContent(TabPage tabPage, UserControl control)
        {
            control.Dock = DockStyle.Fill;
            tabPage.Controls.Add(control);
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateSelectedTab();
        }

        private void ActivateSelectedTab()
        {
            if (tabControlMain.SelectedTab == null || tabControlMain.SelectedTab.Controls.Count == 0)
            {
                return;
            }

            if (tabControlMain.SelectedTab.Controls[0] is IActivatable activatable)
            {
                activatable.OnActivated();
            }
        }
    }

    public interface IActivatable
    {
        void OnActivated();
    }
}
