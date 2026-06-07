using OpenCvSharp;

namespace TestUtilApp.UI
{
    public class ThresholdAlgorithm : IAlgorithm
    {
        public ThresholdTypes ThresholdType { get; set; } = ThresholdTypes.Binary;
        public double Value    { get; set; } = 127;
        public double MaxValue { get; set; } = 255;

        public string Name => "Threshold";

        public string Summary
        {
            get
            {
                string t;
                switch (ThresholdType)
                {
                    case ThresholdTypes.BinaryInv: t = "BinaryInv"; break;
                    case ThresholdTypes.Trunc:     t = "Truncate";  break;
                    case ThresholdTypes.Tozero:    t = "ToZero";    break;
                    case ThresholdTypes.TozeroInv: t = "ToZeroInv"; break;
                    default:                       t = "Binary";    break;
                }
                return $"{t}  {Value}/{MaxValue}";
            }
        }

        public Mat Apply(Mat input)
        {
            Mat gray = input;
            bool needDispose = false;

            if (input.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
                needDispose = true;
            }

            Mat dst = new Mat();
            Cv2.Threshold(gray, dst, Value, MaxValue, ThresholdType);

            if (needDispose) gray.Dispose();
            return dst;
        }
    }
}
