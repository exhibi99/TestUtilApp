using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    public class ContourAlgorithm : IOpenCvAlgorithm
    {
        public RetrievalModes   RetrMode   { get; set; } = RetrievalModes.External;
        public ContourApproximationModes ApproxMode { get; set; } = ContourApproximationModes.ApproxSimple;
        public double MinArea     { get; set; } = 0;
        public double MaxArea     { get; set; } = 0;   // 0 = no limit
        public int    Thickness   { get; set; } = 1;
        public ContourDrawColor DrawColor  { get; set; } = ContourDrawColor.Green;
        /// <summary>밝은 배경 + 어두운 객체일 때 자동으로 반전해서 컨투어를 검출합니다.</summary>
        public bool AutoInvert { get; set; } = true;

        // 마지막 실행에서 찾은 Contour 수 (Summary 표시용)
        private int _lastCount = -1;

        public string Name => "Contour";
        public string Summary
        {
            get
            {
                string retr = RetrMode == RetrievalModes.External ? "Ext"
                            : RetrMode == RetrievalModes.List     ? "List"
                            : "Tree";
                string area = MaxArea > 0 ? $"{MinArea}~{MaxArea}" : $"≥{MinArea}";
                string cnt  = _lastCount >= 0 ? $"  [{_lastCount}]" : "";
                return $"{retr} | area:{area}{cnt}";
            }
        }

        public int  InputFromStep { get; set; } = -1;
        public bool IsSourceNode  => false;
        public bool IsEnabled     { get; set; } = true;

        public Mat Execute(Mat input)
        {
            if (input == null || input.Empty()) return null;

            // Contour 검출은 단채널(이진 또는 그레이스케일) 이미지가 필요
            Mat gray;
            bool disposeGray = false;
            if (input.Channels() > 1)
            {
                gray = new Mat();
                Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
                disposeGray = true;
            }
            else
            {
                gray = input;
            }

            // AutoInvert: 밝은 배경 + 어두운 객체 → 반전하여 흰 객체 기준으로 검출
            if (AutoInvert)
            {
                Mat inv = new Mat();
                Cv2.BitwiseNot(gray, inv);
                if (disposeGray) gray.Dispose();
                gray = inv;
                disposeGray = true;
            }

            // Contour 추출
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(gray, out contours, out hierarchy, RetrMode, ApproxMode);

            if (disposeGray) gray.Dispose();

            // Area 필터링
            var filtered = new List<OpenCvSharp.Point[]>();
            foreach (var c in contours)
            {
                double area = Cv2.ContourArea(c);
                if (area < MinArea) continue;
                if (MaxArea > 0 && area > MaxArea) continue;
                filtered.Add(c);
            }

            _lastCount = filtered.Count;

            // 결과 이미지: 컬러로 Contour를 그림
            Mat dst;
            if (input.Channels() == 1)
            {
                dst = new Mat();
                Cv2.CvtColor(input, dst, ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                dst = input.Clone();
            }

            Scalar color = GetScalar(DrawColor);
            for (int i = 0; i < filtered.Count; i++)
                Cv2.DrawContours(dst, filtered, i, color, Thickness);

            return dst;
        }

        public bool ShowConfigDialog(IWin32Window owner) => false;
        public AlgorithmSettingsPanel GetSettingsPanel() => new ContourSettingsPanel();

        private static Scalar GetScalar(ContourDrawColor c)
        {
            switch (c)
            {
                case ContourDrawColor.Red:     return new Scalar(0,   0,   255);
                case ContourDrawColor.Blue:    return new Scalar(255, 0,   0);
                case ContourDrawColor.Yellow:  return new Scalar(0,   255, 255);
                case ContourDrawColor.Cyan:    return new Scalar(255, 255, 0);
                case ContourDrawColor.Magenta: return new Scalar(255, 0,   255);
                case ContourDrawColor.White:   return new Scalar(255, 255, 255);
                default:                       return new Scalar(0,   255, 0);   // Green
            }
        }
    }

    public enum ContourDrawColor
    {
        Green, Red, Blue, Yellow, Cyan, Magenta, White
    }
}
