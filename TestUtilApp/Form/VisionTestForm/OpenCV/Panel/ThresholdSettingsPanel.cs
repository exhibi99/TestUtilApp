using System;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    public partial class ThresholdSettingsPanel : AlgorithmSettingsPanel
    {
        public ThresholdSettingsPanel()
        {
            InitializeComponent();

            cmbType.Items.AddRange(new object[]
            {
                ThresholdTypes.Binary,
                ThresholdTypes.BinaryInv,
                ThresholdTypes.Trunc,
                ThresholdTypes.Tozero,
                ThresholdTypes.TozeroInv
            });
            cmbType.SelectedIndex = 0;
        }

        public override void LoadFrom(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is ThresholdAlgorithm algo)) return;

            int idx = cmbType.Items.IndexOf(algo.Type);
            cmbType.SelectedIndex = idx >= 0 ? idx : 0;
            nudThreshValue.Value = (decimal)Math.Max((double)nudThreshValue.Minimum,
                                           Math.Min((double)nudThreshValue.Maximum, algo.ThresholdValue));
            nudMaxValue.Value    = (decimal)Math.Max((double)nudMaxValue.Minimum,
                                           Math.Min((double)nudMaxValue.Maximum, algo.MaxValue));
        }

        public override void ApplyTo(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is ThresholdAlgorithm algo)) return;

            algo.Type           = (ThresholdTypes)cmbType.SelectedItem;
            algo.ThresholdValue = (double)nudThreshValue.Value;
            algo.MaxValue       = (double)nudMaxValue.Value;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            RaiseApplied();
        }
    }
}
