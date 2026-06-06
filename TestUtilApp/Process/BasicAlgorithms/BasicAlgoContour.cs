using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Process
{
	class BasicAlgoContour
	{
		const RetrievalModes RETRIEVAL_MODE = RetrievalModes.External;
		const ContourApproximationModes APPROXIMATION_MODE = ContourApproximationModes.ApproxNone;

		public static int LargestContourIndex(ref Mat srcImg, out Point[][] contours)
		{
			contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxArea = 0.0;

			int index = 0;
			foreach (Point[] cont in contours)
			{
				double area = Cv2.ContourArea(cont);
				if (area > maxArea)
				{
					maxIndex = index;
					maxArea = area;
				}
				index++;
			}
			return maxIndex;
		}

		public static Mat LargestContourImage(ref Mat srcImg)
		{
			Mat dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxArea = 0.0;

			int index = 0;
			foreach (Point[] cont in contours)
			{
				double area = Cv2.ContourArea(cont);
				if (area > maxArea)
				{
					maxIndex = index;
					maxArea = area;
				}
				index++;
			}
			Cv2.DrawContours(dstImg, contours, maxIndex, new Scalar(255), -1);

			return dstImg;
		}

		public static Rect LargestContourRect(ref Mat srcImg)
		{
			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxArea = 0.0;
			Rect dstRect = new Rect(0, 0, 0, 0);

			int index = 0;
			foreach (Point[] cont in contours)
			{
				double area = Cv2.ContourArea(cont);
				if (area > maxArea)
				{
					maxIndex = index;
					maxArea = area;
					dstRect = Cv2.BoundingRect(cont);
				}
				index++;
			}
			return dstRect;
		}

		public static Rect LargestContourRect(ref Mat srcImg, out Mat dstImg)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxArea = 0.0;
			Rect dstRect = new Rect(0, 0, 0, 0);
			int index = 0;
			foreach (Point[] cont in contours)
			{
				double area = Cv2.ContourArea(cont);
				if (area > maxArea)
				{
					maxIndex = index;
					maxArea = area;
					dstRect = Cv2.BoundingRect(cont);
				}
				index++;
			}
			Cv2.DrawContours(dstImg, contours, maxIndex, new Scalar(255), -1);
			return dstRect;
		}

		public static RotatedRect LargestContourRoatedRect(ref Mat srcImg)
		{
			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxArea = 0.0;
			RotatedRect dstRect = new RotatedRect();

			int index = 0;
			foreach (Point[] cont in contours)
			{
				double area = Cv2.ContourArea(cont);
				if (area > maxArea)
				{
					maxIndex = index;
					maxArea = area;
					dstRect = Cv2.MinAreaRect(cont);
				}
				index++;
			}
			return dstRect;
		}

		public static List<Point> LargestRatioContourList(ref Mat srcImg, out Mat dstImg, double sizeMin, ALGO_DIR dir)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));
			List<Point> dstContour = new List<Point>();
			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = -1;
			double maxRatio = 0.0;
			int index = 0;
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				if (dir == ALGO_DIR.VER)
				{
					if (blobRect.Height >= sizeMin)
					{
						double ratio = (double)blobRect.Height / (double)blobRect.Width;
						if (ratio >= maxRatio)
						{
							maxIndex = index;
							maxRatio = ratio;
						}
					}
				}
				else if (dir == ALGO_DIR.HOR)
				{
					if (blobRect.Width >= sizeMin)
					{
						double ratio = (double)blobRect.Width / (double)blobRect.Height;
						if (ratio >= maxRatio)
						{
							maxIndex = index;
							maxRatio = ratio;
						}
					}
				}
				else
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					double size = 0.0;
					double ratio = 0.0;
					if (rotRect.Size.Width < rotRect.Size.Height)
					{
						size = rotRect.Size.Height;
						ratio = rotRect.Size.Height / rotRect.Size.Width;
					}
					else
					{
						size = rotRect.Size.Width;
						ratio = rotRect.Size.Width / rotRect.Size.Height;
					}
					if (size >= sizeMin)
					{
						if (ratio >= maxRatio)
						{
							maxIndex = index;
							maxRatio = ratio;
						}
					}
				}
				index++;
			}

			if (maxIndex >= 0)
			{
				dstContour.AddRange(contours[maxIndex]);
				Cv2.DrawContours(dstImg, contours, maxIndex, new Scalar(255), -1);
			}
			return dstContour;
		}

		public static Mat CenterContourImage(ref Mat srcImg)
		{
			Mat dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxRectSize = 0.0;

			int index = 0;
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double rectSize = blobRect.Width * blobRect.Height;
				if (rectSize > maxRectSize && blobRect.Contains(srcImg.Cols / 2, srcImg.Rows / 2) &&
					rectSize < srcImg.Cols * srcImg.Rows)
				{
					maxIndex = index;
					maxRectSize = rectSize;
				}
				index++;
			}
			Cv2.DrawContours(dstImg, contours, maxIndex, new Scalar(255), -1);
			return dstImg;
		}

		public static Rect CenterContourRect(ref Mat srcImg, out Mat dstImg)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = 0;
			double maxRectSize = 0.0;
			Rect dstRect = new Rect(0, 0, 0, 0);

			int index = 0;
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double rectSize = blobRect.Width * blobRect.Height;
				if (rectSize > maxRectSize && blobRect.Contains(srcImg.Cols / 2, srcImg.Rows / 2))
				{
					maxIndex = index;
					maxRectSize = rectSize;
					dstRect = Cv2.BoundingRect(cont);
				}
				index++;
			}
			Cv2.DrawContours(dstImg, contours, maxIndex, new Scalar(255), -1);
			return dstRect;
		}

		public static Mat DrawAllContours(ref Mat srcImg, ref Mat countourImg)
		{
			Mat dstImg = srcImg.Clone();
			if (dstImg.Channels() != 3) Cv2.CvtColor(dstImg, dstImg, ColorConversionCodes.GRAY2BGR);

			Point[][] contours = Cv2.FindContoursAsArray(countourImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			for (int i = 0; i < contours.Length; i++)
			{
				Cv2.DrawContours(dstImg, contours, i, new Scalar(0, 255, 0), 3, LineTypes.AntiAlias);
			}

			return dstImg;
		}


		public static Mat GetBlobImage(ref Mat srcImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			Mat dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			List<Point[]> newContours = new List<Point[]>();
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize <= sizeMax)
						newContours.Add(cont);
				}
				else if (dir == ALGO_DIR.VER)
				{
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize <= sizeMax && blobRatio >= ratioMin)
						newContours.Add(cont);
				}
				else
				{
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize <= sizeMax && blobRatio >= ratioMin)
						newContours.Add(cont);
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return dstImg;
		}

		public static Rect[] GetBlobRects(ref Mat srcImg, out Mat dstImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<Rect> blobs = new List<Rect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else if (dir == ALGO_DIR.VER)
				{
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else
				{
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return blobs.ToArray();
		}

		public static Point[] GetBlobPoints(ref Mat srcImg)
		{
			List<Point> points = new List<Point>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				points.AddRange(cont);
			}
			
			return points.ToArray();
		}

		public static Point[] GetOutlinePoints(ref Mat srcImg)
		{
			Point[] rawPoints = GetBlobPoints(ref srcImg);

			int w = srcImg.Width;
			int h = srcImg.Height;

			Point[] tPoints = new Point[w];
			Point[] bPoints = new Point[w];
			Point[] lPoints = new Point[h];
			Point[] rPoints = new Point[h];
			
			foreach (Point pt in rawPoints)
			{
				double wd = (double)w;
				double hd = (double)h;
				double xd = (double)pt.X;
				double yd = (double)pt.Y;

				bool check1 = yd < (hd / wd) * xd ? true : false;
				bool check2 = yd < (-hd / wd) * (xd - wd) ? true : false;

				// t
				if (check1 == true && check2 == true)
				{
					if (tPoints[pt.X] == new Point(0, 0) || tPoints[pt.X].Y > pt.Y) tPoints[pt.X] = pt;
				}
				// b
				else if (check1 == false && check2 == false)
				{
					if (bPoints[pt.X] == new Point(0, 0) || bPoints[pt.X].Y < pt.Y) bPoints[pt.X] = pt;
				}
				// l
				else if (check1 == false && check2 == true)
				{
					if (lPoints[pt.Y] == new Point(0, 0) || lPoints[pt.Y].X > pt.X) lPoints[pt.Y] = pt;
				}
				// r
				else
				{
					if (rPoints[pt.Y] == new Point(0, 0) || rPoints[pt.Y].X < pt.X) rPoints[pt.Y] = pt;
				}
			}

			Mat dstImg = new Mat();
			Cv2.CvtColor(srcImg, dstImg, ColorConversionCodes.GRAY2BGR);

			List<Point> resPoints = new List<Point>();
			AddValidPoints(tPoints.ToList(), ref resPoints, ref dstImg);
			AddValidPoints(bPoints.ToList(), ref resPoints, ref dstImg);
			AddValidPoints(lPoints.ToList(), ref resPoints, ref dstImg);
			AddValidPoints(rPoints.ToList(), ref resPoints, ref dstImg);
			
			return resPoints.ToArray();
		}
		
		private static void AddValidPoints(List<Point> points, ref List<Point> resPoints, ref Mat dstImg)
		{
			foreach (Point pt in points)
			{
				if (pt != new Point(0, 0))
				{
					resPoints.Add(pt);
					dstImg.Circle(pt, 1, Scalar.Red, 1);
				}
			}
		}

		public static Point[] GetOutlineCenterPoints(ref Mat srcImg)
		{
			Point[] rawPoints = GetBlobPoints(ref srcImg);

			int w = srcImg.Width;
			int h = srcImg.Height;

			Point[] tPoints = new Point[w];
			Point[] bPoints = new Point[w];
			Point[] lPoints = new Point[h];
			Point[] rPoints = new Point[h];
			
			int[] tCounts = new int[w];
			int[] bCounts = new int[w];
			int[] lCounts = new int[h];
			int[] rCounts = new int[h];

			Mat testImg = new Mat(srcImg.Size(), MatType.CV_8UC3, Scalar.Black);

			foreach (Point pt in rawPoints)
			{
				double wd = (double)w;
				double hd = (double)h;
				double xd = (double)pt.X;
				double yd = (double)pt.Y;

				bool check1 = yd < (hd / wd) * xd ? true : false;
				bool check2 = yd < (-hd / wd) * (xd - wd) ? true : false;

				// t
				if (check1 == true && check2 == true)
				{
					tPoints[pt.X].X += pt.X;
					tPoints[pt.X].Y += pt.Y;
					tCounts[pt.X]++;

					Cv2.Circle(testImg, pt, 1, Scalar.Green, -1);
				}
				// b
				else if (check1 == false && check2 == false)
				{
					bPoints[pt.X].X += pt.X;
					bPoints[pt.X].Y += pt.Y;
					bCounts[pt.X]++;

					Cv2.Circle(testImg, pt, 1, Scalar.Green, -1);
				}
				// l
				else if (check1 == false && check2 == true)
				{
					lPoints[pt.Y].X += pt.X;
					lPoints[pt.Y].Y += pt.Y;
					lCounts[pt.Y]++;

					Cv2.Circle(testImg, pt, 1, Scalar.Green, -1);
				}
				// r
				else
				{
					rPoints[pt.Y].X += pt.X;
					rPoints[pt.Y].Y += pt.Y;
					rCounts[pt.Y]++;

					Cv2.Circle(testImg, pt, 1, Scalar.Green, -1);
				}
			}

			Mat dstImg = new Mat();
			Cv2.CvtColor(srcImg, dstImg, ColorConversionCodes.GRAY2BGR);

			List<Point> resPoints = new List<Point>();
			AddValidAveragePoints(tPoints.ToList(), tCounts.ToList(), ref resPoints, ref dstImg);
			AddValidAveragePoints(bPoints.ToList(), bCounts.ToList(), ref resPoints, ref dstImg);
			AddValidAveragePoints(lPoints.ToList(), lCounts.ToList(), ref resPoints, ref dstImg);
			AddValidAveragePoints(rPoints.ToList(), rCounts.ToList(), ref resPoints, ref dstImg);
			
			return resPoints.ToArray();
		}

		private static void AddValidAveragePoints(List<Point> points, List<int> counts, ref List<Point> resPoints, ref Mat dstImg)
		{
			int index = 0;
			foreach (Point pt in points)
			{
				if (counts[index] >= 2)
				{
					Point avgPoint = new Point();
					avgPoint.X = (int)((double)pt.X / (double)counts[index]);
					avgPoint.Y = (int)((double)pt.Y / (double)counts[index]);

					resPoints.Add(avgPoint);
					dstImg.Circle(avgPoint, 1, Scalar.Red, -1);
				}
				index++;
			}
		}

		public static List<Rect> GetRemoveBlobRects(ref Mat srcImg, out Mat dstImg, out Mat dispImg, double sizeMin, ALGO_DIR dir, bool disp = false)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<Rect> blobs = new List<Rect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);

				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					if (rotRect.Size.Width < sizeMin || rotRect.Size.Height < sizeMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else if (dir == ALGO_DIR.VER)
				{
					if ((double)blobRect.Height < sizeMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else
				{
					if ((double)blobRect.Width < sizeMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
			}

			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);

			if (disp)
			{
				dispImg = new Mat(srcImg.Size(), MatType.CV_8UC3, new Scalar(0));
				Cv2.DrawContours(dispImg, contours, -1, Scalar.Green, 5);
				Cv2.DrawContours(dispImg, newContours, -1, Scalar.Red, 5);
			}
			else
			{
				dispImg = null;
			}

			return blobs;
		}

		public static List<Rect> GetRemoveBlobRects(ref Mat srcImg, out Mat dstImg, out Mat dispImg, double areaMin, bool disp = false)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<Rect> blobs = new List<Rect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double area = Cv2.ContourArea(cont);

				if (area < areaMin)
				{
					blobs.Add(blobRect);
					newContours.Add(cont);
				}
			}

			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);

			if (disp)
			{
				dispImg = new Mat(srcImg.Size(), MatType.CV_8UC3, new Scalar(0));
				Cv2.DrawContours(dispImg, contours, -1, Scalar.Green, 5);
				Cv2.DrawContours(dispImg, newContours, -1, Scalar.Red, 5);
			}
			else
			{
				dispImg = null;
			}

			return blobs;
		}

		public static List<Rect> GetBlobRectList(ref Mat srcImg, out Mat dstImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<Rect> blobs = new List<Rect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else if (dir == ALGO_DIR.VER)
				{
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
				else
				{
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(blobRect);
						newContours.Add(cont);
					}
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return blobs;
		}

		public static RotatedRect[] GetBlobRotatedRects(ref Mat srcImg, out Mat dstImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<RotatedRect> blobs = new List<RotatedRect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				RotatedRect rotRect = Cv2.MinAreaRect(cont);
				Rect blobRect = Cv2.BoundingRect(cont);
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
				else if (dir == ALGO_DIR.VER)
				{
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
				else
				{
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return blobs.ToArray();
		}

		public static List<RotatedRect> GetBlobRotatedRectList(ref Mat srcImg, out Mat dstImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));

			List<RotatedRect> blobs = new List<RotatedRect>();
			List<Point[]> newContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				RotatedRect rotRect = Cv2.MinAreaRect(cont);
				Rect blobRect = Cv2.BoundingRect(cont);
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
				else if (dir == ALGO_DIR.VER)
				{
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
				else
				{
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
					{
						blobs.Add(rotRect);
						newContours.Add(cont);
					}
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return blobs;
		}

		public static Point[] GetLargestOutlineImage(ref Mat srcImg)
		{
			List<Point> dstContour = new List<Point>();
			Point[][] contours;
			HierarchyIndex[] hierarchy;
			Rect largestRect = new Rect(0, 0, 0, 0);

			Cv2.FindContours(srcImg, out contours, out hierarchy, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int maxIndex = -1;
			int index = 0;
			foreach (Point[] cont in contours)
			{
				Rect blobRect = Cv2.BoundingRect(cont);
				if (blobRect.Width * blobRect.Height > largestRect.Width * largestRect.Height)
				{
					maxIndex = index;
					largestRect = blobRect;
				}
				index++;
			}

			if (maxIndex >= 0)
			{
				dstContour.AddRange(contours[maxIndex]);
			}
			return dstContour.ToArray();
		}
		
		public static List<Point[]> GetBlobContours(ref Mat srcImg, double sizeMin, double sizeMax, double ratioMin, ALGO_DIR dir)
		{
			List<Point[]> blobContours = new List<Point[]>();

			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			foreach (Point[] cont in contours)
			{
				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					double blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax)
						blobContours.Add(cont);
				}
				else if (dir == ALGO_DIR.VER)
				{
					Rect blobRect = Cv2.BoundingRect(cont);
					double blobSize = (double)blobRect.Height;
					double blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
						blobContours.Add(cont);
				}
				else
				{
					Rect blobRect = Cv2.BoundingRect(cont);
					double blobSize = (double)blobRect.Width;
					double blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize >= sizeMin && blobSize < sizeMax && blobRatio >= ratioMin)
						blobContours.Add(cont);
				}
			}
			return blobContours;
		}

		public static Mat RemoveBlobImage(ref Mat srcImg, double minSize, ALGO_DIR dir)
		{
			Mat dstImg = new Mat(srcImg.Size(), MatType.CV_8UC1, new Scalar(0));
			Point[][] contours = Cv2.FindContoursAsArray(srcImg, RETRIEVAL_MODE, APPROXIMATION_MODE);
			List<Point[]> newContours = new List<Point[]>();
			foreach (Point[] cont in contours)
			{
				double blobSize = 0.0;
				double blobRatio = 0.0;

				if (dir == ALGO_DIR.BOTH)
				{
					RotatedRect rotRect = Cv2.MinAreaRect(cont);
					blobSize = rotRect.Size.Width;
					if (blobSize < rotRect.Size.Height) blobSize = rotRect.Size.Height;
					if (blobSize > minSize)
						newContours.Add(cont);
				}
				else if (dir == ALGO_DIR.VER)
				{
					Rect blobRect = Cv2.BoundingRect(cont);
					blobSize = (double)blobRect.Height;
					blobRatio = (double)blobRect.Height / (double)blobRect.Width;
					if (blobSize > minSize)
						newContours.Add(cont);
				}
				else
				{
					Rect blobRect = Cv2.BoundingRect(cont);
					blobSize = (double)blobRect.Width;
					blobRatio = (double)blobRect.Width / (double)blobRect.Height;
					if (blobSize > minSize)
						newContours.Add(cont);
				}
			}
			Cv2.DrawContours(dstImg, newContours, -1, new Scalar(255), -1);
			return dstImg;
		}

		public static Mat FindLargestContour(ref Mat srcImg, out Point[] vContour, double dMin, double dMax, bool bDraw, int iDrawLength = -1, Scalar? _drawColor = null, int nthBigContour = 0)
		{
			Scalar drawColor = _drawColor != null ? _drawColor.Value : Scalar.White;

			Mat tempImg = new Mat();
			if (srcImg.Channels() == 3)
				Cv2.CvtColor(srcImg, tempImg, ColorConversionCodes.BGR2GRAY);
			else
				tempImg = srcImg.Clone();

			Point[][] vAllContours = Cv2.FindContoursAsArray(tempImg, RETRIEVAL_MODE, APPROXIMATION_MODE);

			int iMaxIdx = 0;
			double dMaxArea = 0.0;
			List<(double, Point[])> vSelected = new List<(double, Point[])>();
			List<Point[]> vSortedContours = new List<Point[]>();

			for (int i = 0; i < vAllContours.Length; i++)
			{
				var contour = vAllContours[i];
				double dArea = Cv2.ContourArea(contour);

				if (nthBigContour != 0)
				{
					if (dArea >= dMin && dArea <= dMax)
					{
						vSelected.Add((dArea, contour));
					}
				}
				else
				{
					if (dArea > dMaxArea)
					{
						iMaxIdx = i;
						dMaxArea = dArea;
					}
				}
			}

			if (nthBigContour != 0)
				vSelected.Sort((a, b) => a.Item1 > b.Item1 ? 1 : 0);

			vContour = null;

			int idrawContourIdx, iNumContour;
			var contImg = new Mat();
			if (bDraw)
			{
				contImg = new Mat(srcImg.Size(), MatType.CV_8UC3);
				contImg.SetTo(Scalar.Black);

				if (nthBigContour != 0)
				{
					idrawContourIdx = nthBigContour - 1;
					iNumContour = vSelected.Count;

					foreach (var tuple in vSelected)
						vSortedContours.Add(tuple.Item2);

					if (idrawContourIdx >= iNumContour)
						idrawContourIdx = iNumContour - 1;

					Cv2.DrawContours(contImg, vSortedContours, idrawContourIdx, drawColor, Cv2.FILLED);
					vContour = vSortedContours[idrawContourIdx].ToArray();
				}
				else
				{
					if (iMaxIdx < vAllContours.Length)
					{
						Cv2.DrawContours(contImg, vAllContours, iMaxIdx, drawColor, iDrawLength);
						vContour = vAllContours[iMaxIdx];
					}
				}
			}
			else
			{
				if (nthBigContour != 0)
				{
					idrawContourIdx = nthBigContour - 1;
					iNumContour = vSelected.Count;

					foreach (var tuple in vSelected)
						vSortedContours.Add(tuple.Item2);

					if (idrawContourIdx >= iNumContour)
						idrawContourIdx = iNumContour - 1;

					vContour = vSortedContours[idrawContourIdx].ToArray();
				}
				else
				{
					if (iMaxIdx < vAllContours.Length)
					{
						vContour = vAllContours[iMaxIdx];
					}
				}
			}
			return contImg;
		}
	}
}
