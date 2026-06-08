using System;
using System.Windows.Forms;
using TestUtilApp.Models;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class DICETestForm : UserControl, IActivatable
    {
        private AppConfig _config;
        private ConfigService _configService;

        private CropControl _cropControl;
        private ClassifyControl _classifyControl;
        private LabelGeneratorControl _labelGenControl;
        private FilterControl _filterControl;
        private ConfigControl _configControl;
        private InspectionControl _inspectionControl;

        private UserControl _currentControl;
        private Button _activeNavigationButton;

        public DICETestForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            InitializeServices();
            InitializeControls();

            ShowFilterControl();
        }

        public void OnActivated()
        {
            RestoreNavigationButtonStates();

            if (_currentControl is IActivatable activatable)
            {
                activatable.OnActivated();
            }
        }

        private void InitializeServices()
        {
            try
            {
                _configService = new ConfigService();
                _config = _configService.LoadConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Settings File Load Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                _config = new AppConfig
                {
                    DetectionClasses = new System.Collections.Generic.List<string> { "BELT_TOP", "BELT_BOTTOM" },
                    DefaultPadding = 10,
                    DefaultCropArea = new CropArea { X = 100, Y = 100, Width = 200, Height = 200 },
                    LabelGeneration = new LabelGenerationConfig { MinConfidence = 0.5f, SkipExistingJson = true },
                    Classification = new ClassificationConfig { MinConfidence = 0.5f },
                    FileFilter = new FileFilterConfig()
                };
            }
        }

        private void InitializeControls()
        {
            try
            {
                _cropControl = new CropControl(_config, _configService);
                _classifyControl = new ClassifyControl(_config, _configService);
                _labelGenControl = new LabelGeneratorControl(_config, _configService);
                _filterControl = new FilterControl(_config, _configService);
                _configControl = new ConfigControl(_config, _configService);
                _inspectionControl = new InspectionControl(_config, _configService);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Control initialization error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadControlToPanel(UserControl control)
        {
            try
            {
                if (control == null)
                {
                    return;
                }

                if (_currentControl != null)
                {
                    CommitCurrentControlChanges();
                    contentPanel.Controls.Remove(_currentControl);
                }

                UiTheme.Apply(control);
                control.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(control);
                _currentControl = control;

                if (control is IActivatable activatable)
                {
                    activatable.OnActivated();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Screen transition error: {ex.Message}", "Error !",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            ShowCropControl();
        }

        private void CommitCurrentControlChanges()
        {
            if (_currentControl is FilterControl filterControl)
            {
                filterControl.CommitCurrentSettings();
            }
            else if (_currentControl is InspectionControl inspectionControl)
            {
                inspectionControl.CommitCurrentSettings();
            }
        }

        private void btnClassify_Click(object sender, EventArgs e)
        {
            ShowClassifyControl();
        }

        private void btnLabelGen_Click(object sender, EventArgs e)
        {
            ShowLabelGenControl();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            ShowFilterControl();
        }

        private void btnConfigEdit_Click(object sender, EventArgs e)
        {
            ShowConfigControl();
        }

        private void btnInspection_Click(object sender, EventArgs e)
        {
            ShowInspectionControl();
        }

        private void ShowCropControl()
        {
            LoadControlToPanel(_cropControl);
            UpdateButtonStates(btnCrop);
            UpdateStatusLabel("Image Crop");
        }

        private void ShowClassifyControl()
        {
            LoadControlToPanel(_classifyControl);
            UpdateButtonStates(btnClassify);
            UpdateStatusLabel("Image Classification");
        }

        private void ShowLabelGenControl()
        {
            LoadControlToPanel(_labelGenControl);
            UpdateButtonStates(btnLabelGen);
            UpdateStatusLabel("Label Gen");
        }

        private void ShowFilterControl()
        {
            LoadControlToPanel(_filterControl);
            UpdateButtonStates(btnFilter);
            UpdateStatusLabel("File Filtering");
        }

        private void ShowConfigControl()
        {
            LoadControlToPanel(_configControl);
            UpdateButtonStates(btnConfigEdit);
            UpdateStatusLabel("Settings");
        }

        private void ShowInspectionControl()
        {
            LoadControlToPanel(_inspectionControl);
            UpdateButtonStates(btnInspection);
            UpdateStatusLabel("Auto Inspection");
        }

        private void UpdateButtonStates(Button activeButton)
        {
            _activeNavigationButton = activeButton;
            RestoreNavigationButtonStates();
        }

        private void RestoreNavigationButtonStates()
        {
            UiTheme.ApplyNavigationButtonDefault(btnCrop);
            UiTheme.ApplyNavigationButtonDefault(btnClassify);
            UiTheme.ApplyNavigationButtonDefault(btnLabelGen);
            UiTheme.ApplyNavigationButtonDefault(btnFilter);
            UiTheme.ApplyNavigationButtonDefault(btnInspection);
            UiTheme.ApplyNavigationButtonDefault(btnConfigEdit);

            if (_activeNavigationButton != null)
            {
                UiTheme.ApplyNavigationButtonActive(_activeNavigationButton);
            }
        }

        private void UpdateStatusLabel(string text)
        {
            toolStripStatusLabel.Text = $"{text} Screen";
        }

        private void DisposeLoadedControls()
        {
            try
            {
                UserControl[] controls =
                {
                    _cropControl,
                    _classifyControl,
                    _labelGenControl,
                    _filterControl,
                    _configControl,
                    _inspectionControl
                };

                foreach (UserControl control in controls)
                {
                    if (control != null && !control.IsDisposed)
                    {
                        control.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Resource cleanup error: {ex.Message}");
            }
        }
    }
}
