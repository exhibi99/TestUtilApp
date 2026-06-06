using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.BasicAlgoCoordinate;

namespace TestUtilApp.Process
{
	class BasicAlgoPoint
	{
		//두 포인트의 중심 포인트를 리턴
		public static Point2d CenterPoint(Point2d pt1, Point2d pt2)
		{
			Point2d centerPt = new Point2d((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0);
			return centerPt;
		}

		// 두 포인트 사이의 거리를 계산
		public static double DistOfPoints(Point2d pt1, Point2d pt2)
		{
			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

		// 두 포인트 사이의 거리를 계산
		public static double DistOfPoints(Point2f pt1, Point2f pt2)
		{
			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		// 포인트가 주어진 영상 내에 있는지 여부를 리턴
		public static bool IsPointInside(Point pt, ref Mat mat)
		{
			Rect areaRect = new Rect(0, 0, mat.Cols, mat.Rows);
			return IsPointInside(pt, areaRect);
		}

		// 포인트가 주어진 Rect 내에 있는지 여부를 리턴
		public static bool IsPointInside(Point pt, Rect areaRect)
		{
			if (pt.X < areaRect.X) return false;
			if (pt.Y < areaRect.Y) return false;
			if (pt.X >= areaRect.BottomRight.X) return false;
			if (pt.Y >= areaRect.BottomRight.Y) return false;
			return true;
		}

		// 두 포인트와 반지름을 주고, 원의 중심을 찾아 리턴(Pt1 to Pt2 방향으로 오른쪽 중심점)
		public static Point2d CenterPointOfCircle(Point2d pt1, Point2d pt2, int radius)
		{
			Point2d centerPt, circlePt;
			Line2D line2Pt;
			double distPt2Pt, crossDist;

			centerPt = CenterPoint(pt1, pt2);
			line2Pt = LineFrom2Points(pt1, pt2);
			distPt2Pt = DistOfPoints(pt1, centerPt);

			// r*r = distPt2Pt * distPt2Pt + crossDist*crossDist;
			crossDist = Math.Sqrt(radius * radius - distPt2Pt * distPt2Pt);
			circlePt = CrossPointFromLineByDist(line2Pt, centerPt, crossDist);

			return circlePt;
		}

		// 세 포인트를 주고, 원의 중심을 찾아 리턴
		public static Point2d CenterPointOfCircle(Point2d pt1, Point2d pt2, Point2d pt3)
		{
			Point2d centerPt = new Point2d();
			double startPtX = -1, startPtY = -1;
			double endPtX = -1, endPtY = -1;

			if (startPtX == -1 || startPtX > pt1.X) startPtX = pt1.X;
			if (endPtX == -1 || endPtX < pt1.X) endPtX = pt1.X;
			if (startPtY == -1 || startPtY > pt1.Y) startPtY = pt1.Y;
			if (endPtY == -1 || endPtY < pt1.Y) endPtY = pt1.Y;

			if (startPtX == -1 || startPtX > pt2.X)	startPtX = pt2.X;
			if (endPtX == -1 || endPtX < pt2.X) endPtX = pt2.X;
			if (startPtY == -1 || startPtY > pt2.Y) startPtY = pt2.Y;
			if (endPtY == -1 || endPtY < pt2.Y) endPtY = pt2.Y;

			if (startPtX == -1 || startPtX > pt3.X) startPtX = pt3.X;
			if (endPtX == -1 || endPtX < pt3.X) endPtX = pt3.X;
			if (startPtY == -1 || startPtY > pt3.Y) startPtY = pt3.Y;
			if (endPtY == -1 || endPtY < pt3.Y) endPtY = pt3.Y;
			
			double totalDist = 0.0;
			double dist1, dist2, dist3;
			double minDist = 1000000.0;
			for (int i = (int)startPtY; i < (int)endPtY; i++)
			{
				for (int j = (int)startPtX; j < (int)endPtX; j++)
				{
					dist1 = Math.Sqrt(Math.Pow((double)(pt1.X - j), 2) + Math.Pow((double)(pt1.Y - i), 2));
					dist2 = Math.Sqrt(Math.Pow((double)(pt2.X - j), 2) + Math.Pow((double)(pt2.Y - i), 2));
					dist3 = Math.Sqrt(Math.Pow((double)(pt3.X - j), 2) + Math.Pow((double)(pt3.Y - i), 2));
					totalDist = Math.Abs(dist1 - dist2) + Math.Abs(dist2 - dist3) + Math.Abs(dist3 - dist1);
					if (totalDist < minDist)
					{
						centerPt.X = j;
						centerPt.Y = i;
						minDist = totalDist;
					}
				}
			}
			return centerPt;
		}

        public static Point MovingMaxSearchingAdv(ref Mat srcImg, int icenterPos, int pos, int direction, int windowSize, int minAvg)
        {
            if (direction == (int)DefineAlgorithm.ALGO_SEARCH_DIR.L2C)
            {
                int j = pos;
                int center = (int)(icenterPos * 0.9);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(0, pos);

                if (center - windowSize <= 0) return maxPt;

                for (int i = 0; i < center - windowSize; i++)
                {
                    int pixelSum = 0;
                    for (int p = 0; p < windowSize; p++)
                    {
                        Point pt = new Point(i + p, j);
                        Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                        pixelSum += (int)pixel;
                    }
                    double pixelAvg = (double)pixelSum / (double)windowSize;

                    if (pixelAvg > maxPixelAvg)
                    {
                        maxPixelAvg = pixelAvg;
                        maxPt = new Point(i + windowSize - 1, j);
                    }
                }

                if (maxPixelAvg >= minAvg) return maxPt;
                else return new Point(0, pos);
            }
            else if (direction == (int)DefineAlgorithm.ALGO_SEARCH_DIR.R2C)
            {
                int r = srcImg.Cols - 1;
                int j = pos;
                int center = (int)(icenterPos * 1.1);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(r, pos);

                if (r - center - windowSize <= 0) return new Point(r, pos);

                for (int i = r; i >= center + windowSize; i--)
                {
                    int pixelSum = 0;
                    for (int p = 0; p < windowSize; p++)
                    {
                        Point pt = new Point(i - p, j);
                        Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                        pixelSum += (int)pixel;
                    }
                    double pixelAvg = (double)pixelSum / (double)windowSize;

                    if (pixelAvg > maxPixelAvg)
                    {
                        maxPixelAvg = pixelAvg;
                        maxPt = new Point(i - windowSize + 1, j);
                    }
                }

                if (maxPixelAvg >= minAvg) return maxPt;
                else return new Point(r, pos);
            }
            else if (direction == (int)DefineAlgorithm.ALGO_SEARCH_DIR.T2C)
            {
                int i = pos;
                int center = (int)(icenterPos * 0.9);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(i, 0);

                if (center - windowSize <= 0) return maxPt;

                for (int j = 0; j < center - windowSize; j++)
                {
                    int pixelSum = 0;
                    for (int p = 0; p < windowSize; p++)
                    {
                        Point pt = new Point(i, j + p);
                        Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                        pixelSum += (int)pixel;
                    }
                    double pixelAvg = (double)pixelSum / (double)windowSize;

                    if (pixelAvg > maxPixelAvg)
                    {
                        maxPixelAvg = pixelAvg;
                        maxPt = new Point(i, j + windowSize - 1);
                    }
                }

                if (maxPixelAvg >= minAvg) return maxPt;
                else return new Point(pos, 0);
            }
            else if (direction == (int)DefineAlgorithm.ALGO_SEARCH_DIR.B2C)
            {
                int b = srcImg.Rows - 1;
                int i = pos;
                int center = (int)(icenterPos * 1.1);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(i, 0);

                if (b - center - windowSize <= 0) return new Point(pos, b);

                for (int j = b; j >= center + windowSize; j--)
                {
                    int pixelSum = 0;
                    for (int p = 0; p < windowSize; p++)
                    {
                        Point pt = new Point(i, j - p);
                        Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                        pixelSum += (int)pixel;
                    }
                    double pixelAvg = (double)pixelSum / (double)windowSize;

                    if (pixelAvg > maxPixelAvg)
                    {
                        maxPixelAvg = pixelAvg;
                        maxPt = new Point(i, j - windowSize + 1);
                    }
                }

                if (maxPixelAvg >= minAvg) return maxPt;
                else return new Point(pos, b);
            }
            return new Point();
        }
        public static Point2d LinePtByX(Line2D line, double x)
        {
            double vx = line.Vx;
            double vy = line.Vy;
            double x0 = line.X1;
            double y0 = line.Y1;

            if (vx == 0.0f) vx = 0.00001f;
            double y = (vy * (x - x0) / vx) + y0;

            return new Point(x, y);
        }
        public static Point LinePtByY(Line2D line, double y)
        {
            double vx = line.Vx;
            double vy = line.Vy;
            double x0 = line.X1;
            double y0 = line.Y1;

            if (vy == 0.0f) vy = 0.00001f;
            double x = (vx * (y - y0) / vy) + x0;

            return new Point(x, y);
        }
		public static bool DoesRectangleContainPoint(RotatedRect rectangle, Point2f point)
		{
			List<Point2f> contour = rectangle.Points().ToList();
			double indicator = Cv2.PointPolygonTest(contour, point, false);
			return indicator >= 0;
		}
	}
}
