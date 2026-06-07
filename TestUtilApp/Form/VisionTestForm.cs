using System;
using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class VisionTestForm : UserControl, IActivatable
    {
        private OpenCvControl _openCvControl;
        private UserControl _currentControl;
        private Button _activeNavigationButton;

        public VisionTestForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            InitializeControls();
            ShowOpenCvControl();
        }

        public void OnActivated()
        {
            RestoreNavigationButtonStates();

            if (_currentControl is IActivatable activatable)
            {
                activatable.OnActivated();
            }
        }

        private void InitializeControls()
        {
            _openCvControl = new OpenCvControl();
        }

        private void LoadControlToPanel(UserControl control)
        {
            if (control == null)
            {
                return;
            }

            if (_currentControl != null)
            {
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

        private void btnOpenCv_Click(object sender, EventArgs e)
        {
            ShowOpenCvControl();
        }

        private void ShowOpenCvControl()
        {
            LoadControlToPanel(_openCvControl);
            UpdateButtonStates(btnOpenCv);
            UpdateStatusLabel("OpenCV");
        }

        private void UpdateButtonStates(Button activeButton)
        {
            _activeNavigationButton = activeButton;
            RestoreNavigationButtonStates();
        }

        private void RestoreNavigationButtonStates()
        {
            UiTheme.ApplyNavigationButtonDefault(btnOpenCv);

            if (_activeNavigationButton != null)
            {
                UiTheme.ApplyNavigationButtonActive(_activeNavigationButton);
            }
        }

        private void UpdateStatusLabel(string text)
        {
            toolStripStatusLabel.Text = $"{text} Screen";
        }
    }
}
