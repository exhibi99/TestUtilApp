using System;
using System.Windows.Forms;
using TestUtilApp.Services;
using TestUtilApp.UI;

namespace TestUtilApp
{
    public partial class MainForm : Form
    {
        private DICETestForm _diceTestForm;
        private VisionTestForm _visionTestForm;

        public MainForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            InitializeTabs();

            tabControlMain.SelectedTab = tabDiceTest;
            ActivateSelectedTab();
        }

        private void InitializeTabs()
        {
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
