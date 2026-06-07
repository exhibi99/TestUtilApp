using System;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    public class CropAlgorithm : IOpenCvAlgorithm
    {
        public int X      { get; set; } = 0;
        public int Y      { get; set; } = 0;
        public int Width  { get; set; } = 100;
        public int Height { get; set; } = 100;

        public string Name    => "Crop";
        public string Summary => $"({X}, {Y})  {Width} × {Height}";
        public int    InputFromStep { get; set; } = -1;
        public bool   IsSourceNode  => false;

        public Mat Execute(Mat input)
        {
            if (input == null || input.Empty()) return null;

            int x = Math.Max(0, Math.Min(X, input.Width  - 1));
            int y = Math.Max(0, Math.Min(Y, input.Height - 1));
            int w = Math.Max(1, Math.Min(Width,  input.Width  - x));
            int h = Math.Max(1, Math.Min(Height, input.Height - y));

            return new Mat(input, new Rect(x, y, w, h)).Clone();
        }

        public bool ShowConfigDialog(IWin32Window owner) => false;
        public AlgorithmSettingsPanel GetSettingsPanel() => new CropSettingsPanel();
    }
}
