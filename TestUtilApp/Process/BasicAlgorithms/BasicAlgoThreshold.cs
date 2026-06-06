using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	class BasicAlgoThreshold
	{
		public static Mat RangeThreshold(ref Mat srcImg, double lowerThresh, double upperThresh, ThresholdTypes type)
		{
			Mat procImg = srcImg.Clone();
			if (procImg.Channels() != 1) Cv2.CvtColor(procImg, procImg, ColorConversionCodes.BGR2GRAY);

			Mat binImg = new Mat();
			Mat lowerImg = procImg.Clone();
			Mat upperImg = procImg.Clone();
			
			if (type == ThresholdTypes.Binary)
			{
				Cv2.Threshold(lowerImg, lowerImg, lowerThresh, 255.0, ThresholdTypes.Binary);
				Cv2.Threshold(upperImg, upperImg, upperThresh, 255.0, ThresholdTypes.BinaryInv);
				Cv2.BitwiseAnd(lowerImg, upperImg, binImg);
			}
			else if (type == ThresholdTypes.BinaryInv)
			{
				Cv2.Threshold(lowerImg, lowerImg, lowerThresh, 255.0, ThresholdTypes.BinaryInv);
				Cv2.Threshold(upperImg, upperImg, upperThresh, 255.0, ThresholdTypes.Binary);
				Cv2.BitwiseOr(lowerImg, upperImg, binImg);
			}
			else if (type == ThresholdTypes.Tozero)
			{
				Cv2.Threshold(lowerImg, lowerImg, lowerThresh, 255.0, ThresholdTypes.Tozero);
				Cv2.Threshold(upperImg, upperImg, upperThresh, 255.0, ThresholdTypes.TozeroInv);
				Cv2.BitwiseAnd(lowerImg, upperImg, binImg);
			}
			else if (type == ThresholdTypes.TozeroInv)
			{
				Cv2.Threshold(lowerImg, lowerImg, lowerThresh, 255.0, ThresholdTypes.TozeroInv);
				Cv2.Threshold(upperImg, upperImg, upperThresh, 255.0, ThresholdTypes.Tozero);
				Cv2.BitwiseOr(lowerImg, upperImg, binImg);
			}
			return binImg;
		}

		public static Mat MeanThreshold(ref Mat srcImg, Rect roiRect, double alpha, double beta, ThresholdTypes type)
		{
			Mat binImg = new Mat();

			Mat maskImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));
			Cv2.Rectangle(maskImg, roiRect, new Scalar(255), -1);
			double avgBrightness = Cv2.Mean(srcImg, maskImg)[0];
			Cv2.Threshold(srcImg, binImg, avgBrightness * alpha + beta, 255, type);

			return binImg;
		}

		public static Mat MeanThreshold(ref Mat srcImg, ref Mat maskImg, double alpha, double beta, ThresholdTypes type)
		{
			Mat binImg = new Mat();

			double avgBrightness = Cv2.Mean(srcImg, maskImg)[0];
			Cv2.Threshold(srcImg, binImg, avgBrightness * alpha + beta, 255, type);

			return binImg;
		}
	}
}
