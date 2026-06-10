using System;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    public partial class ContourSettingsPanel : AlgorithmSettingsPanel
    {
        public ContourSettingsPanel()
        {
            InitializeComponent();

            cmbRetr.Items.AddRange(new object[]
            {
                RetrievalModes.External,
                RetrievalModes.List,
                RetrievalModes.Tree
            });
            cmbRetr.SelectedIndex = 0;

            cmbApprox.Items.AddRange(new object[]
            {
                ContourApproximationModes.ApproxSimple,
                ContourApproximationModes.ApproxNone,
                ContourApproximationModes.ApproxTC89L1,
                ContourApproximationModes.ApproxTC89KCOS
            });
            cmbApprox.SelectedIndex = 0;

            foreach (ContourDrawColor v in Enum.GetValues(typeof(ContourDrawColor)))
                cmbColor.Items.Add(v);
            cmbColor.SelectedIndex = 0;
        }

        public override void LoadFrom(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is ContourAlgorithm algo)) return;

            int ri = cmbRetr.Items.IndexOf(algo.RetrMode);
            cmbRetr.SelectedIndex = ri >= 0 ? ri : 0;

            int ai = cmbApprox.Items.IndexOf(algo.ApproxMode);
            cmbApprox.SelectedIndex = ai >= 0 ? ai : 0;

            nudMinArea.Value = (decimal)Math.Max((double)nudMinArea.Minimum,
                               Math.Min((double)nudMinArea.Maximum, algo.MinArea));
            nudMaxArea.Value = (decimal)Math.Max((double)nudMaxArea.Minimum,
                               Math.Min((double)nudMaxArea.Maximum, algo.MaxArea));
            nudThickness.Value = Math.Max(nudThickness.Minimum,
                                 Math.Min(nudThickness.Maximum, algo.Thickness));

            int ci = cmbColor.Items.IndexOf(algo.DrawColor);
            cmbColor.SelectedIndex = ci >= 0 ? ci : 0;

            chkAutoInvert.Checked = algo.AutoInvert;
        }

        public override void ApplyTo(IOpenCvAlgorithm algorithm)
        {
            if (!(algorithm is ContourAlgorithm algo)) return;

            algo.RetrMode   = (RetrievalModes)cmbRetr.SelectedItem;
            algo.ApproxMode = (ContourApproximationModes)cmbApprox.SelectedItem;
            algo.MinArea    = (double)nudMinArea.Value;
            algo.MaxArea    = (double)nudMaxArea.Value;
            algo.Thickness  = (int)nudThickness.Value;
            algo.DrawColor  = (ContourDrawColor)cmbColor.SelectedItem;
            algo.AutoInvert = chkAutoInvert.Checked;
        }

        private void btnApply_Click(object sender, EventArgs e) => RaiseApplied();
    }
}
