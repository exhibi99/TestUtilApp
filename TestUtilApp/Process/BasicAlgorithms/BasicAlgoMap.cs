using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	class BasicAlgoMap
	{
		public enum MAP_DIR
		{
			X = 0,
			Y,
			XY,
			DEG45,
			DEG135
		}

		public enum MAP_TYPE
		{
			DARK = 0,
			BRIGHT,
			BOTH
		}

		public class MapParam
		{
			public double Alpha = 1.0;
			public double Beta = 0.0;
			public MAP_DIR Dir = MAP_DIR.X;
			public MAP_TYPE Type = MAP_TYPE.DARK;

			public MapParam(double alpha, double beta, MAP_DIR dir, MAP_TYPE type)
			{
				Alpha = alpha;
				Beta = beta;
				Dir = dir;
				Type = type;
			}
		}

		public static Mat MakeMapImage_X_Both(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.X, MAP_TYPE.BOTH);
			return mapImg;
		}

		public static Mat MakeMapImage_X_Dark(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.X, MAP_TYPE.DARK);
			return mapImg;
		}

		public static Mat MakeMapImage_X_Bright(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.X, MAP_TYPE.BRIGHT);
			return mapImg;
		}

		public static Mat MakeMapImage_Y_Both(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.Y, MAP_TYPE.BOTH);
			return mapImg;
		}

		public static Mat MakeMapImage_Y_Dark(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.Y, MAP_TYPE.DARK);
			return mapImg;
		}

		public static Mat MakeMapImage_Y_Bright(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.Y, MAP_TYPE.BRIGHT);
			return mapImg;
		}

		public static Mat MakeMapImage_XY_Both(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.XY, MAP_TYPE.BOTH);
			return mapImg;
		}

		public static Mat MakeMapImage_XY_Dark(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.XY, MAP_TYPE.DARK);
			return mapImg;
		}

		public static Mat MakeMapImage_XY_Bright(ref Mat srcImg, int dist, double thr, double alpha, double beta)
		{
			Mat mapImg = MakeMapImage(ref srcImg, dist, thr, alpha, beta, MAP_DIR.XY, MAP_TYPE.BRIGHT);
			return mapImg;
		}

		// 픽셀에 직접 접근하지 않고 영상으로 연산
		private static Mat MakeMapImage(ref Mat srcImg, int dist, double thr, double alpha, double beta, MAP_DIR mapDir, MAP_TYPE mapType)
		{
			Mat mapImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			if (mapDir != MAP_DIR.X && mapDir != MAP_DIR.Y && mapDir != MAP_DIR.XY && mapDir != MAP_DIR.DEG45 && mapDir != MAP_DIR.DEG135) return mapImg;
			if (mapType != MAP_TYPE.DARK && mapType != MAP_TYPE.BRIGHT && mapType != MAP_TYPE.BOTH) return mapImg;

			Mat xImg = new Mat();
			Mat yImg = new Mat();

			double offset = 100.0;
			Mat logLut = new Mat(1, 256, MatType.CV_32FC1);
			var logLutB = new Mat<float>(logLut);
			var logLutIndexer = logLutB.GetIndexer();
			for (int i = 0; i < 256; i++)
			{
				float val = 0.0F;
				if (i > thr) val = (float)(Math.Log((double)i + offset));
				logLutIndexer[0, i] = val;
			}

			Mat cImg = new Mat();
			Cv2.LUT(srcImg, logLut, cImg);

			if (mapDir == MAP_DIR.X || mapDir == MAP_DIR.XY)
			{
				Mat tImg = BasicAlgoImage.TranslateImage(srcImg, 0, -dist);
				Mat bImg = BasicAlgoImage.TranslateImage(srcImg, 0, dist);
				Cv2.LUT(tImg, logLut, tImg);
				Cv2.LUT(bImg, logLut, bImg);

				if (mapType == MAP_TYPE.BOTH) xImg = Cv2.Abs((tImg + bImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.DARK) xImg = ((tImg + bImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.BRIGHT) xImg = (cImg - (tImg + bImg) / 2) * alpha + beta;

				xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
			}
			if (mapDir == MAP_DIR.Y || mapDir == MAP_DIR.XY)
			{
				Mat lImg = BasicAlgoImage.TranslateImage(srcImg, -dist, 0);
				Mat rImg = BasicAlgoImage.TranslateImage(srcImg, dist, 0);
				Cv2.LUT(lImg, logLut, lImg);
				Cv2.LUT(rImg, logLut, rImg);

				if (mapType == MAP_TYPE.BOTH) yImg = Cv2.Abs((lImg + rImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.DARK) yImg = ((lImg + rImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.BRIGHT) yImg = (cImg - (lImg + rImg) / 2) * alpha + beta;

				yImg.ConvertTo(yImg, MatType.CV_8UC1, 1.0);
			}
			else if (mapDir == MAP_DIR.DEG45)
			{
				Mat trImg = BasicAlgoImage.TranslateImage(srcImg, dist, -dist);
				Mat blImg = BasicAlgoImage.TranslateImage(srcImg, -dist, dist);
				Cv2.LUT(trImg, logLut, trImg);
				Cv2.LUT(blImg, logLut, blImg);

				if (mapType == MAP_TYPE.BOTH) xImg = Cv2.Abs((trImg + blImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.DARK) xImg = ((trImg + blImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.BRIGHT) xImg = (cImg - (trImg + blImg) / 2) * alpha + beta;

				xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
			}
			else if (mapDir == MAP_DIR.DEG135)
			{
				Mat tlImg = BasicAlgoImage.TranslateImage(srcImg, -dist, -dist);
				Mat brImg = BasicAlgoImage.TranslateImage(srcImg, dist, dist);
				Cv2.LUT(tlImg, logLut, tlImg);
				Cv2.LUT(brImg, logLut, brImg);

				if (mapType == MAP_TYPE.BOTH) xImg = Cv2.Abs((tlImg + brImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.DARK) xImg = ((tlImg + brImg) / 2 - cImg) * alpha + beta;
				else if (mapType == MAP_TYPE.BRIGHT) xImg = (cImg - (tlImg + brImg) / 2) * alpha + beta;

				xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
			}

			if (mapDir == MAP_DIR.X || mapDir == MAP_DIR.DEG45 || mapDir == MAP_DIR.DEG135) return xImg;
			else if (mapDir == MAP_DIR.Y) return yImg;
			else { Cv2.BitwiseOr(xImg, yImg, mapImg); return mapImg; }
		}

		// 여러개의 Map Image 리턴
		public static Mat[] MakeMapImages(ref Mat srcImg, int dist, double thr, MapParam[] mapParam)
		{
			Mat xImg = new Mat();
			Mat yImg = new Mat();

			double offset = 100.0;
			Mat logLut = new Mat(1, 256, MatType.CV_32FC1);
			var logLutB = new Mat<float>(logLut);
			var logLutIndexer = logLutB.GetIndexer();
			for (int i = 0; i < 256; i++)
			{
				float val = 0.0F;
				if (i > thr) val = (float)(Math.Log((double)i + offset));
				logLutIndexer[0, i] = val;
			}

			Mat cImg = srcImg.Clone();
			Mat tImg = BasicAlgoImage.TranslateImage(srcImg, 0, -dist);
			Mat bImg = BasicAlgoImage.TranslateImage(srcImg, 0, dist);
			Mat lImg = BasicAlgoImage.TranslateImage(srcImg, -dist, 0);
			Mat rImg = BasicAlgoImage.TranslateImage(srcImg, dist, 0);
			Mat trImg = BasicAlgoImage.TranslateImage(srcImg, dist, -dist);
			Mat blImg = BasicAlgoImage.TranslateImage(srcImg, -dist, dist);
			Mat tlImg = BasicAlgoImage.TranslateImage(srcImg, -dist, -dist);
			Mat brImg = BasicAlgoImage.TranslateImage(srcImg, dist, dist);

			Cv2.LUT(cImg, logLut, cImg);
			Cv2.LUT(tImg, logLut, tImg);
			Cv2.LUT(bImg, logLut, bImg);
			Cv2.LUT(lImg, logLut, lImg);
			Cv2.LUT(rImg, logLut, rImg);
			Cv2.LUT(trImg, logLut, trImg);
			Cv2.LUT(blImg, logLut, blImg);
			Cv2.LUT(tlImg, logLut, tlImg);
			Cv2.LUT(brImg, logLut, brImg);

			List<Mat> mapImgs = new List<Mat>();
			foreach (var param in mapParam)
			{
				if (param.Dir == MAP_DIR.X || param.Dir == MAP_DIR.XY)
				{
					if (param.Type == MAP_TYPE.BOTH) xImg = Cv2.Abs((tImg + bImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.DARK) xImg = ((tImg + bImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.BRIGHT) xImg = (cImg - (tImg + bImg) / 2) * param.Alpha + param.Beta;

					xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
				}
				if (param.Dir == MAP_DIR.Y || param.Dir == MAP_DIR.XY)
				{
					if (param.Type == MAP_TYPE.BOTH) yImg = Cv2.Abs((lImg + rImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.DARK) yImg = ((lImg + rImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.BRIGHT) yImg = (cImg - (lImg + rImg) / 2) * param.Alpha + param.Beta;

					yImg.ConvertTo(yImg, MatType.CV_8UC1, 1.0);
				}
				else if (param.Dir == MAP_DIR.DEG45)
				{
					if (param.Type == MAP_TYPE.BOTH) xImg = Cv2.Abs((trImg + blImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.DARK) xImg = ((trImg + blImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.BRIGHT) xImg = (cImg - (trImg + blImg) / 2) * param.Alpha + param.Beta;

					xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
				}
				else if (param.Dir == MAP_DIR.DEG135)
				{
					if (param.Type == MAP_TYPE.BOTH) xImg = Cv2.Abs((tlImg + brImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.DARK) xImg = ((tlImg + brImg) / 2 - cImg) * param.Alpha + param.Beta;
					else if (param.Type == MAP_TYPE.BRIGHT) xImg = (cImg - (tlImg + brImg) / 2) * param.Alpha + param.Beta;

					xImg.ConvertTo(xImg, MatType.CV_8UC1, 1.0);
				}

				Mat mapImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));
				if (param.Dir == MAP_DIR.X || param.Dir == MAP_DIR.DEG45 || param.Dir == MAP_DIR.DEG135) mapImgs.Add(xImg);
				else if (param.Dir == MAP_DIR.Y) mapImgs.Add(yImg);
				else { Cv2.BitwiseOr(xImg, yImg, mapImg); mapImgs.Add(mapImg); }

			}
			return mapImgs.ToArray();
		}
	}
}
