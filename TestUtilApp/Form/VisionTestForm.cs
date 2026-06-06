using System.Windows.Forms;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class VisionTestForm : UserControl, IActivatable
    {
        public VisionTestForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        public void OnActivated()
        {
            toolStripStatusLabel.Text = "Vision Test Ready";
        }
    }
}
