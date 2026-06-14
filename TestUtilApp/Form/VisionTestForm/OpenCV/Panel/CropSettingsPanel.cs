using System;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    public partial class CropSettingsPanel : AlgorithmSettingsPanel
    {
        public CropSettingsPanel()
        {
            InitializeComponent();
        }

        public override void LoadFrom(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is CropAlgorithm algo)) return;
            nudX.Value      = Clamp(algo.X,      nudX);
            nudY.Value      = Clamp(algo.Y,      nudY);
            nudWidth.Value  = Clamp(algo.Width,  nudWidth);
            nudHeight.Value = Clamp(algo.Height, nudHeight);
        }

        public override void ApplyTo(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is CropAlgorithm algo)) return;
            algo.X      = (int)nudX.Value;
            algo.Y      = (int)nudY.Value;
            algo.Width  = (int)nudWidth.Value;
            algo.Height = (int)nudHeight.Value;
        }

        private static decimal Clamp(int value, NumericUpDown nud)
            => Math.Max(nud.Minimum, Math.Min(nud.Maximum, value));

        private void btnApply_Click(object sender, EventArgs e) => RaiseApplied();
    }
}
