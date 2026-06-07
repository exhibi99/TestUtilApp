using System;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    public class ThresholdAlgorithm : IOpenCvAlgorithm
    {
        public ThresholdTypes Type { get; set; } = ThresholdTypes.Binary;
        public double ThresholdValue { get; set; } = 127;
        public double MaxValue { get; set; } = 255;

        public string Name    => "Threshold";
        public string Summary => $"{GetTypeName()}  {ThresholdValue} / {MaxValue}";

        public Mat Execute(Mat input)
        {
            if (input == null || input.Empty()) return null;

            Mat gray = input;
            bool needDispose = false;

            if (input.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
                needDispose = true;
            }

            Mat dst = new Mat();
            Cv2.Threshold(gray, dst, ThresholdValue, MaxValue, Type);

            if (needDispose) gray.Dispose();
            return dst;
        }

        public bool ShowConfigDialog(IWin32Window owner)
        {
            using (var dlg = new ThresholdDialog())
            {
                dlg.SetValues(Type, ThresholdValue, MaxValue);
                if (dlg.ShowDialog(owner) != DialogResult.OK) return false;

                Type = dlg.SelectedType;
                ThresholdValue = dlg.SelectedThresholdValue;
                MaxValue = dlg.SelectedMaxValue;
                return true;
            }
        }

        public AlgorithmSettingsPanel GetSettingsPanel() => new ThresholdSettingsPanel();

        private string GetTypeName()
        {
            switch (Type)
            {
                case ThresholdTypes.BinaryInv: return "BinaryInv";
                case ThresholdTypes.Trunc:     return "Truncate";
                case ThresholdTypes.Tozero:    return "ToZero";
                case ThresholdTypes.TozeroInv: return "ToZeroInv";
                default:                       return "Binary";
            }
        }
    }
}
