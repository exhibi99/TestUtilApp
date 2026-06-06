using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.BasicAlgoRect;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Process
{
	public class BasicAlgoImage
	{
		// 영상 중심 좌표를 리턴
		public static Point2d ImageCenter(ref Mat img)
		{
			Point2d centerPt = new Point2d(img.Cols / 2.0, img.Rows / 2.0);
			return centerPt;
		}

		// 영상 회전
		public static Mat RotateImage(ref Mat img, double deg)
		{
			Mat rotImg = new Mat();
			if (deg == 90)
			{
				Cv2.Transpose(img, rotImg);
				Cv2.Flip(rotImg, rotImg, FlipMode.Y);
			}
			else if (deg == 180)
			{
				Cv2.Flip(img, rotImg, FlipMode.XY);
			}
			else if (deg == 270)
			{
				Cv2.Transpose(img, rotImg);
				Cv2.Flip(rotImg, rotImg, FlipMode.X);
			}
			else
			{
				Mat rotMat = Cv2.GetRotationMatrix2D(new Point2f((float)((float)img.Cols / 2.0), (float)((float)img.Rows / 2.0)), deg, 1.0);
				Cv2.WarpAffine(img, rotImg, rotMat, img.Size());
			}
			return rotImg;
		}

		// 영상을 X, Y 방향으로 확대
		public static Mat EnlargeImage(ref Mat img, int x, int y)
		{
			x = Math.Abs(x);
			y = Math.Abs(y);

			Mat enlargeImg = new Mat(img.Rows + y * 2, img.Cols + x * 2, img.Type(), new Scalar(0));

			Rect rect = new Rect(x, y, img.Cols, img.Rows);
			img.CopyTo(enlargeImg[rect]);

			return enlargeImg;
		}

		// 영상을 X, Y 방향으로 축소
		public static Mat ReduceImage(ref Mat img, int x, int y)
		{
			x = Math.Abs(x);
			y = Math.Abs(y);

			Mat reduceImg = new Mat(img.Rows - y * 2, img.Cols - x * 2, img.Type(), new Scalar(0));

			Rect rect = new Rect(x, y, img.Cols, img.Rows);
			img[rect].CopyTo(reduceImg);

			return reduceImg;
		}

		// 영상을 X, Y 방향으로 이동 (zero padding)
		public static Mat ShiftImage(ref Mat img, int x, int y)
		{
			Mat shiftImg = new Mat(img.Size(), img.Type(), new Scalar(0));
			shiftImg = EnlargeImage(ref shiftImg, Math.Abs(x), Math.Abs(y));

			Rect shiftRect = new Rect(Math.Abs(x) + x, Math.Abs(y) + y, img.Cols, img.Rows);
			img.CopyTo(shiftImg[shiftRect]);

			shiftImg = ReduceImage(ref shiftImg, -Math.Abs(x), -Math.Abs(y));
			return shiftImg;
		}

		// 영상을 세로 방향으로 이동
		public static void ShiftImageVert(ref Mat img, int yOffset)
		{
			Rect rectFrom = new Rect(0, 0, img.Width, img.Height - Math.Abs(yOffset));
			Rect rectTo = new Rect(0, 0, img.Width, img.Height - Math.Abs(yOffset));
			if (yOffset > 0) rectTo.Y = yOffset;
			else if (yOffset < 0) rectFrom.Y = -yOffset;
			Mat imgShift = Mat.Zeros(img.Size(), img.Type());
			Mat imgCropped = img.Clone(rectFrom);
			imgCropped.CopyTo(imgShift.SubMat(rectTo));
			img = imgShift;
		}

		// ROI 영역 영상 (zero padding)
		public static Mat ZeroPaddingImage(ref Mat img, Rect roiRect, Size retRoiSize)
		{
			Mat roiImg = new Mat(retRoiSize, img.Type(), new Scalar(0));

			roiRect = AdjustRect(roiRect, img.Size(), false);
			if (roiRect.Width >= retRoiSize.Width && roiRect.Height >= retRoiSize.Height)
			{
				roiImg = img[roiRect].Clone();
			}
			else
			{
				Mat copyRoiImg;
				copyRoiImg = img[roiRect].Clone();

				Rect copyRoiRect = new Rect(0, 0, roiRect.Width, roiRect.Height);
				if (roiRect.Width < retRoiSize.Width && roiRect.X <= 0)
					copyRoiRect.X = retRoiSize.Width - roiRect.Width;
				if (roiRect.Height < retRoiSize.Height && roiRect.Y <= 0)
					copyRoiRect.Y = retRoiSize.Height - roiRect.Height;

				copyRoiRect = AdjustRect(copyRoiRect, roiImg.Size(), false);

				if (copyRoiImg.Cols != copyRoiRect.Width || copyRoiImg.Rows != copyRoiRect.Height)
					Cv2.Resize(copyRoiImg, copyRoiImg, new Size(copyRoiRect.Width, copyRoiRect.Height));

				copyRoiImg.CopyTo(roiImg[copyRoiRect]);
			}
			return roiImg;
		}

		// offset 만큼 이동
		public static Mat TranslateImage(Mat srcImg, int x, int y, int padding = 0)
		{
			if (x == 0 && y == 0)
				return srcImg.Clone();

			if (srcImg.Empty())
				return srcImg.Clone();

			Mat transImg = new Mat(srcImg.Size(), srcImg.Type(), new Scalar(0));

			Rect srcRoi = new Rect(
				Math.Max(-x, 0),
				Math.Max(-y, 0),
				srcImg.Width - Math.Abs(x),
				srcImg.Height - Math.Abs(y));
			Rect transRoi = new Rect(
				Math.Max(x, 0),
				Math.Max(y, 0),
				srcImg.Width - Math.Abs(x),
				srcImg.Height - Math.Abs(y));

			srcImg[srcRoi].CopyTo(transImg[transRoi]);
			return transImg;
		}

		public static void MeanBetween(Mat src, double dLower, double dUpper, ref Scalar mean, ref Scalar min, ref Scalar max, int iMinCnt = 0)
		{
			if (src.Channels() == 1)
			{
				double dSum = 0.0;
				double dMin = 255.0;
				double dMax = 0.0;
				int iCnt = 0;

				var mat = new Mat<byte>(src);
				var indexer = mat.GetIndexer();
				for (int j = 0; j < mat.Rows; j++)
				{
					for (int i = 0; i < mat.Cols; i++)
					{
						var pt = new Point(i, j);
						var pixel = BasicAlgoPixel.GetPixel(indexer, pt);
						if (pixel >= dLower && pixel <= dUpper)
						{
							dSum += pixel;
							iCnt++;

							if (pixel > dMax) dMax = pixel;
							if (pixel < dMin) dMin = pixel;
						}
					}
				}

				double dMean = (iCnt == 0 || iCnt < iMinCnt) ? 0.0 : dSum / iCnt;
				mean.Val0 = dMean;
				min.Val0 = dMin;
				max.Val0 = dMax;
			}
			else
			{
				double[] dSum = { 0.0, 0.0, 0.0 };
				double[] dMin = { 255.0, 255.0, 255.0 };
				double[] dMax = { 0.0, 0.0, 0.0 };
				int[] iCnt = { 0, 0, 0 };

				var mat = new Mat<Vec3b>(src);
				var indexer = mat.GetIndexer();
				for (int j = 0; j < mat.Rows; j++)
				{
					for (int i = 0; i < mat.Cols; i++)
					{
						var pt = new Point(i, j);
						var pixel = BasicAlgoPixel.GetPixel(indexer, pt);

						for (int c = 0; c < 3; c++)
						{
							if (pixel[i] >= dLower && pixel[i] <= dUpper)
							{
								dSum[i] += pixel[i];
								iCnt[i]++;

								if (pixel[i] > dMax[i]) dMax[i] = pixel[i];
								if (pixel[i] < dMin[i]) dMin[i] = pixel[i];
							}
						}
					}
				}

				double[] dMean = { 0, 0, 0 };
				for (int i = 0; i < 3; i++)
				{
					if (iCnt[i] == 0) dMean[i] = 0.0;
					else dMean[i] = dSum[i] / iCnt[i];

					mean[i] = dMean[i];
					min[i] = dMin[i];
					max[i] = dMax[i];
				}
			}
		}

		public static Mat FuseImage(Mat darkImg, Mat middleImg, Mat brightImg)
		{
			Mat fuseImg = new Mat(brightImg.Size(), MatType.CV_8UC3, Scalar.Black);

			int h = brightImg.Height;
			int w = brightImg.Width;

			// size check
			if (middleImg.Width != w || middleImg.Height != h || darkImg.Width != w || darkImg.Height != h)
				return fuseImg;
			
			unsafe
			{
				Byte* darkData = (Byte*)darkImg.Data.ToPointer();
				Byte* middleData = (Byte*)middleImg.Data.ToPointer();
				Byte* brightData = (Byte*)brightImg.Data.ToPointer();
				Byte* fuseData = (Byte*)fuseImg.Data.ToPointer();

				// pixel copy
				for (int j = 0; j < h; j++)
				{
					for (int i = 0; i < w; i++)
					{
						fuseData[(j * w + i) * 3 + 0] = darkData[j * w + i];	// B
						fuseData[(j * w + i) * 3 + 1] = middleData[j * w + i];	// G
						fuseData[(j * w + i) * 3 + 2] = brightData[j * w + i];	// R
					}
				}
			}
			
			Mat hsvImg = new Mat();
			Cv2.CvtColor(fuseImg, hsvImg, ColorConversionCodes.BGR2HSV);
			Mat[] hsvSplit = Cv2.Split(hsvImg);

			return hsvSplit[1];
		}

		public static List<Mat> TranslateImages(List<Mat> srcImgs, List<int> yGaps)
		{
			if (srcImgs.Count != yGaps.Count) return srcImgs;

			List<Mat> transImages = new List<Mat>();
			for (int i = 0; i < srcImgs.Count; i++)
			{
				Mat transImg = BasicAlgoImage.TranslateImage(srcImgs[i], 0, yGaps[i]);
				transImages.Add(transImg);
			}
			return transImages;
		}

		public static Mat RotateFlipImage(Mat srcImg, ALGO_ROTATE_FLAG rotateFlag)
		{
			Mat dstImg = srcImg.Clone();
			if (rotateFlag != ALGO_ROTATE_FLAG.NONE) Cv2.Rotate(dstImg, dstImg, (RotateFlags)rotateFlag);
			return dstImg;
		}
		public class ClaheParam
		{
			public bool Use = false;
			public int ClipLimit = 4;
			public int GridWidth = 8;
			public int GridHeight = 8;
		}
		public static Mat Clahe(Mat srcImg, ClaheParam param)
		{
			if (!param.Use) return srcImg;
			if (srcImg.Channels() == 3)
				Cv2.CvtColor(srcImg, srcImg, ColorConversionCodes.BGR2GRAY);
			Mat res = new Mat();
			var clahe = Cv2.CreateCLAHE(param.ClipLimit,
				new Size(param.GridWidth, param.GridHeight));
			clahe.Apply(srcImg, res);
			return res;
		}
	}
}
