using System;
using System.Windows.Forms;
using OpenCvSharp;
using TestUtilApp.Services;

namespace TestUtilApp.UI
{
    public partial class ThresholdDialog : Form
    {
        public ThresholdTypes SelectedType { get; private set; } = ThresholdTypes.Binary;
        public double SelectedThresholdValue { get; private set; } = 127;
        public double SelectedMaxValue { get; private set; } = 255;

        public ThresholdDialog()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        public void SetValues(ThresholdTypes type, double thresh, double maxVal)
        {
            int idx = 0;
            switch (type)
            {
                case ThresholdTypes.BinaryInv: idx = 1; break;
                case ThresholdTypes.Trunc:     idx = 2; break;
                case ThresholdTypes.Tozero:    idx = 3; break;
                case ThresholdTypes.TozeroInv: idx = 4; break;
            }
            cmbType.SelectedIndex = idx;
            nudThreshValue.Value = (decimal)thresh;
            nudMaxValue.Value = (decimal)maxVal;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedType = GetSelectedType();
            SelectedThresholdValue = (double)nudThreshValue.Value;
            SelectedMaxValue = (double)nudMaxValue.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private ThresholdTypes GetSelectedType()
        {
            switch (cmbType.SelectedIndex)
            {
                case 1: return ThresholdTypes.BinaryInv;
                case 2: return ThresholdTypes.Trunc;
                case 3: return ThresholdTypes.Tozero;
                case 4: return ThresholdTypes.TozeroInv;
                default: return ThresholdTypes.Binary;
            }
        }
    }
}
