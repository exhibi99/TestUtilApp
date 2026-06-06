using OpenCvSharp;
using TestUtilApp.Dice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Process
{
	class BasicAlgoDisplay
	{
		public static Mat MakeDetResultImage(ref Mat srcImg, ResultDiceDet result, string diceModel)
		{
			var resImg = new Mat();
			if (srcImg.Empty()) return resImg;

			if (srcImg.Channels() == 3)
				resImg = srcImg.Clone();
			else
				Cv2.CvtColor(srcImg, resImg, ColorConversionCodes.GRAY2BGR);

			Cv2.PutText(resImg, diceModel, new Point(50, 100), HersheyFonts.HersheyDuplex, 2, Scalar.Orange, 2);

			int rectIndex = 0;
			List<Scalar> colors = new List<Scalar>() { Scalar.Orange, Scalar.GreenYellow, Scalar.Yellow, Scalar.Red };
			foreach (var rect in result.listRect)
			{
				int offset = 0;
				if (rectIndex > 0) offset = rectIndex * 320;
				Cv2.PutText(resImg, $@"class: {rect.class_name}", new Point(rect.rect.X, rect.rect.Y - 60 + offset), HersheyFonts.HersheyTriplex, 1, colors[rectIndex], 2);
				Cv2.PutText(resImg, $@"conf: {(rect.conf).ToString("0.000")}", new Point(rect.rect.X, rect.rect.Y - 30 + offset), HersheyFonts.HersheyTriplex, 1, colors[rectIndex], 2);
				Cv2.Rectangle(resImg, BasicAlgoRect.IntRect(rect.rect), colors[rectIndex], 2);
				if (rectIndex < colors.Count() - 1) rectIndex++;
			}
			return resImg;
		}

		public static Mat MakeClsResultImage(ref Mat srcImg, ResultDiceCls result, string diceModel, string labeledClass = "")
		{
			var resImg = new Mat();
			if (srcImg.Empty()) return resImg;

			if (srcImg.Channels() == 3)
				resImg = srcImg.Clone();
			else
				Cv2.CvtColor(resImg, resImg, ColorConversionCodes.GRAY2BGR);

			Scalar color = Scalar.Red;
			if (labeledClass == "") color = Scalar.GreenYellow;
			else if (result.class_name == labeledClass) color = Scalar.Green;
			Cv2.PutText(resImg, diceModel, new Point(10, 30), HersheyFonts.HersheyTriplex, 0.75, color);
			Cv2.PutText(resImg, $@"{result.class_name} ({result.conf.Max().ToString("0.000")})", new Point(10, 60), HersheyFonts.HersheyTriplex, 0.75, color);
			return resImg;
		}
	}
}
