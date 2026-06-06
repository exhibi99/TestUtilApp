using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Process
{
	public class BasicAlgoRect
	{
		// Rect2f를 Rect로 변환
		public static Rect IntRect(Rect2f rect2f)
		{
			return new Rect((int)rect2f.X, (int)rect2f.Y, (int)rect2f.Width, (int)rect2f.Height);
		}

		// Rect를 Rect2f로 변환
		public static Rect2f FloatRect(Rect rect)
		{
			return new Rect2f((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		// Rect2d를 Rect로 변환
		public static Rect IntRect(Rect2d rect2d)
		{
			return new Rect((int)rect2d.X, (int)rect2d.Y, (int)rect2d.Width, (int)rect2d.Height);
		}

		// Rect를 X, Y 방향으로 확장
		public static Rect EnlargeRect(Rect rect, int x, int y)
		{
			Rect enlarge = new Rect(rect.X - x, rect.Y - y, rect.Width + x * 2, rect.Height + y * 2);
			return enlarge;
		}

		// Rect를 X, Y 방향으로 확장
		public static Rect2f EnlargeRect(Rect2f rect, float x, float y)
		{
			Rect2f enlarge = new Rect2f(rect.X - x, rect.Y - y, rect.Width + x * 2, rect.Height + y * 2);
			return enlarge;
		}

		// Rect를 X, Y 방향으로 확장 및 주어진 Size에 맞게 Rect의 크기를 조절
		public static Rect EnlargeRect(Rect rect, int x, int y, Size adjustSize, bool fixedSize)
		{
			Rect enlarge = new Rect(rect.X - x, rect.Y - y, rect.Width + x * 2, rect.Height + y * 2);
			enlarge = AdjustRect(enlarge, adjustSize, fixedSize);
			return enlarge;
		}

		// Rect를 T,B,R,L 방향으로 확장
		public static Rect EnlargeRect(Rect rect, int t, int b, int l, int r)
		{
			Rect enlarge = new Rect(rect.X-l, rect.Y - t, rect.Width + l + r, rect.Height + t + b);
			return enlarge;
		}

		// Rect를 정해진 크기로 확대 또는 축소
		public static Rect EnlargeRect(Rect rect, Size targetSize, Size adjustSize, bool fixedSize)
		{
			Point2d centerPt = RectCenter(rect);
			Rect enlarge = new Rect((int)(centerPt.X - targetSize.Width / 2), (int)(centerPt.Y - targetSize.Height / 2), targetSize.Width, targetSize.Height);
			enlarge = AdjustRect(enlarge, adjustSize, fixedSize);
			return enlarge;
		}

        /***
         * Rect를 상하좌우 다른 값으로 확대, 축소하는 경우 음수 전달
         * */
        public static Rect EnlargeRect(Rect rect, int left, int right, int up, int down, Size size)
        {
            Rect result = new Rect(rect.X - left, rect.Y - up, rect.Width + left + right, rect.Height + up + down);
            if (result.Right >= size.Width)
                result.Width = size.Width - result.X - 1;
            if (result.Bottom >= size.Height)
                result.Height = size.Height - result.Y - 1;
            if (result.X < 0)
            {
                result.Width += result.X;
                result.X = 0;
            }
            if (result.Y < 0)
            {
                result.Height += result.Y;
                result.Y = 0;
            }

            return result;
        }

		public static Rect SumRect(Rect rect1, Rect rect2)
		{
			Rect result = new Rect();
			result.X = rect1.X + rect2.X;
			result.Y = rect1.Y + rect2.Y;
			result.Width = rect1.Width + rect2.Width;
			result.Height = rect1.Height + rect2.Height;
			return result;
		}

		public static Rect AvgRect(Rect rect, int divisor)
		{
			Rect result = new Rect();
			result.X = rect.X / divisor;
			result.Y = rect.Y / divisor;
			result.Width = rect.Width / divisor;
			result.Height = rect.Height / divisor;
			return result;
		}

		public class MarginParam
		{
			public int Up = 0;
			public int Down = 0;
			public int Left = 0;
			public int Right = 0;

			public MarginParam()
			{

			}

			public MarginParam(int up, int down, int left, int right)
			{
				Up = up;
				Down = down;
				Left = left;
				Right = right;
			}
		}
		public static Rect EnlargeRect(Rect rect, MarginParam param, Size size)
		{
			return EnlargeRect(rect, param.Left, param.Right, param.Up, param.Down, size);
		}

		// Rect를 주어진 비율만큼 변환
		public static Rect MultiplyRect(Rect rect, double m)
		{
			Rect multiply = rect;
			multiply.X = (int)(rect.X * m);
			multiply.Y = (int)(rect.Y * m);
			multiply.Width = (int)(rect.Width * m);
			multiply.Height = (int)(rect.Height * m);
			return multiply;
		}
		
		// Rect를 주어진 각도만큼 회전한 후 Bounding Rect를 리턴
		public static Rect RotateRect(Rect rect, double angle)
		{
			Point2d center2d = RectCenter(rect);
			Point2f center = new Point2f((float)center2d.X, (float)center2d.Y);
			RotatedRect rotRect = new RotatedRect(center, new Size2f(rect.Width, rect.Height), (float)angle);
			return rotRect.BoundingRect();
		}
		
		// Rect의 중심 좌표를 리턴
		public static Point2d RectCenter(Rect rect)
		{
			Point2d center = new Point2d(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
			return center;
		}
		
		// 주어진 Size에 맞게 Rect의 크기를 조절
		public static Rect AdjustRect(Rect rect, Size size, bool fixedSize)
		{
			Rect adjust = rect;
			if (rect.Width > size.Width) adjust.Width = size.Width;
			if (rect.Height > size.Height) adjust.Height = size.Height;
			if (rect.X < 0)
			{
				adjust.X = 0;
				if (!fixedSize) adjust.Width += rect.X;
			}
			if (rect.Y < 0)
			{
				adjust.Y = 0;
				if (!fixedSize) adjust.Height += rect.Y;
			}
			if (rect.Width < 0) adjust.Width = 0;
			if (rect.Height < 0) adjust.Height = 0;

			if (adjust.X + adjust.Width >= size.Width)
			{
				if (!fixedSize) adjust.Width = size.Width - adjust.X;
				else adjust.X = size.Width - adjust.Width;
			}
			if (adjust.Y + adjust.Height >= size.Height)
			{
				if (!fixedSize) adjust.Height = size.Height - adjust.Y;
				else adjust.Y = size.Height - adjust.Height;
			}
			return adjust;
        }

        // 주어진 Size에 맞게 Rect의 크기를 조절
        public static Rect AdjustRectSimple(Rect rect, Size size)
        {
			Point tl = new Point(Math.Max(0,rect.X), Math.Max(0, rect.Y));
			Point br = new Point(Math.Min(size.Width, rect.Right), Math.Min(size.Height, rect.Bottom));
            return new Rect(tl.X, tl.Y, br.X-tl.X, br.Y-tl.Y);
		}
		// 주어진 Size에 맞게 Rect의 크기를 조절
		public static Rect2f AdjustRectSimple(Rect2f rect, Size size)
		{
			Point2f tl = new Point2f(Math.Max(0, rect.X), Math.Max(0, rect.Y));
			Point2f br = new Point2f(Math.Min(size.Width, rect.Right), Math.Min(size.Height, rect.Bottom));
			return new Rect2f(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
		}

		// Rect가 주어진 Rect 내에 있는지 여부를 리턴
		public static bool IsRectInside(Rect rect, ref Mat mat)
		{
			Rect areaRect = new Rect(0, 0, mat.Cols, mat.Rows);
			return IsRectInside(rect, areaRect);
		}

		// Rect가 주어진 영상 내에 있는지 여부를 리턴
		public static bool IsRectInside(Rect rect, Rect areaRect)
		{
			if (rect.X < areaRect.X) return false;
			if (rect.Y < areaRect.Y) return false;
			if (rect.BottomRight.X > areaRect.BottomRight.X) return false;
			if (rect.BottomRight.Y > areaRect.BottomRight.Y) return false;
			if (rect.Width < 0) return false;
			if (rect.Height < 0) return false;
			return true;
		}

		// Rect가 주어진 Rect 밖에 있는지 여부를 리턴
		public static bool IsRectOutside(Rect rect, Rect areaRect)
		{
			if (rect.X > areaRect.BottomRight.X) return true;
			if (rect.Y > areaRect.BottomRight.Y) return true;
			if (rect.BottomRight.X < areaRect.X) return true;
			if (rect.BottomRight.Y < areaRect.Y) return true;
			if (rect.Width < 0) return false;
			if (rect.Height < 0) return false;
			return false;
		}

		public static double MeanRect(Mat src, Rect rect, double min = 0.0, double max = 255.0, int cnt = 1)
		{
			Mat grayImg = new Mat();
			if (src.Channels() == 3) Cv2.CvtColor(src, grayImg, ColorConversionCodes.BGR2GRAY);
			else grayImg = src;

			// 기본값인 경우 기본 Mean 함수 이용
			if (min == 0.0 && max == 255.0 && cnt == 1)
			{
				var subMat = grayImg.SubMat(rect);
				var scalarMean = Cv2.Mean(subMat);
				return scalarMean.Val0;
			}

			int x = rect.X;
			int y = rect.Y;
			int w = rect.Width;
			int h = rect.Height;

			double sum = 0.0;
			int select = 0;

			var mat = new Mat<byte>(grayImg);
			var indexer = mat.GetIndexer();
			for (int j = y; j < y + h; j++)
			{
				for (int i = x; i < x + w; i++)
				{
					var pt = new Point(i, j);
					var pixel = BasicAlgoPixel.GetPixel(indexer, pt);
					if (pixel >= min && pixel <= max)
					{
						sum += pixel;
						select++;
					}
				}
			}

			double mean = sum / select;
			if (select < cnt) mean = 0.0;

			return mean;
		}

        public static Rect FlipRect(Rect rect, Size imgSize, bool hor, bool ver)
        {
            Rect result = new Rect(rect.X, rect.Y, rect.Width, rect.Height);

            if(hor)
            {
                result.X = imgSize.Width - result.Right -1;
            }

            if(ver)
            {
                result.Y = imgSize.Height - result.Bottom -1;
            }

            return result;
        }

		// 영상을 회전하는 경우 기존 영상의 roiRect를 회전하고 싶은 영상에 맞게 Translate 해서 리턴
		public static Rect TranslateRect(Mat SrcImg, Rect roiRect, ALGO_ROTATE_FLAG rotateFlag)
		{
			Rect TransRect = roiRect;

			if (rotateFlag != ALGO_ROTATE_FLAG.NONE)
			{
				Mat DstImg = new Mat();
				Cv2.Rotate(SrcImg, DstImg, (RotateFlags)rotateFlag);
				
				Point2f[] src = new Point2f[4];
				Point2f[] dst = new Point2f[4];

				List<Point2f> SrcPoint = new List<Point2f>();
				Point2f[] DstPoint = new Point2f[4];

				src[0] = new Point2f(0.0f, 0.0f);
				src[1] = new Point2f((float)SrcImg.Cols, 0.0f);
				src[2] = new Point2f((float)SrcImg.Cols, (float)SrcImg.Rows);
				src[3] = new Point2f(0.0f, (float)SrcImg.Rows);

				if ((RotateFlags)rotateFlag == RotateFlags.Rotate90Clockwise)
				{
					dst[0] = new Point2f((float)DstImg.Cols, 0.0f);
					dst[1] = new Point2f((float)DstImg.Cols, (float)DstImg.Rows);
					dst[2] = new Point2f(0.0f, (float)DstImg.Rows);
					dst[3] = new Point2f(0.0f, 0.0f);

					Mat per_mat = Cv2.GetPerspectiveTransform(src, dst);

					SrcPoint.Add(new Point2f((float)roiRect.X, (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)(roiRect.Y + roiRect.Height)));
					SrcPoint.Add(new Point2f((float)roiRect.X, (float)(roiRect.Y + roiRect.Height)));

					DstPoint = Cv2.PerspectiveTransform(SrcPoint, per_mat);

					TransRect.X = (int)DstPoint[3].X;
					TransRect.Y = (int)DstPoint[3].Y;
					TransRect.Height = (int)DstPoint[1].Y - (int)DstPoint[0].Y;
					TransRect.Width = (int)DstPoint[0].X - (int)DstPoint[3].X;
				}
				else if ((RotateFlags)rotateFlag == RotateFlags.Rotate90Counterclockwise)
				{
					dst[0] = new Point2f(0.0f, (float)DstImg.Rows);
					dst[1] = new Point2f(0.0f, 0.0f);
					dst[2] = new Point2f((float)DstImg.Cols, 0.0f);
					dst[3] = new Point2f((float)DstImg.Cols, (float)DstImg.Rows);

					Mat per_mat = Cv2.GetPerspectiveTransform(src, dst);

					SrcPoint.Add(new Point2f((float)roiRect.X, (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)(roiRect.Y + roiRect.Height)));
					SrcPoint.Add(new Point2f((float)roiRect.X, (float)(roiRect.Y + roiRect.Height)));

					DstPoint = Cv2.PerspectiveTransform(SrcPoint, per_mat);

					TransRect.X = (int)(DstPoint[1].X);
					TransRect.Y = (int)(DstPoint[1].Y);
					TransRect.Height = (int)(DstPoint[0].Y - DstPoint[1].Y);
					TransRect.Width = (int)(DstPoint[3].X - DstPoint[0].X);
				}
				else if ((RotateFlags)rotateFlag == RotateFlags.Rotate180)
				{
					dst[0] = new Point2f((float)DstImg.Cols, (float)DstImg.Rows);
					dst[1] = new Point2f(0.0f, (float)DstImg.Rows);
					dst[2] = new Point2f(0.0f, 0.0f);
					dst[3] = new Point2f((float)DstImg.Cols, 0.0f);

					Mat per_mat = Cv2.GetPerspectiveTransform(src, dst);

					SrcPoint.Add(new Point2f((float)roiRect.X, (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)roiRect.Y));
					SrcPoint.Add(new Point2f((float)(roiRect.X + roiRect.Width), (float)(roiRect.Y + roiRect.Height)));
					SrcPoint.Add(new Point2f((float)roiRect.X, (float)(roiRect.Y + roiRect.Height)));

					DstPoint = Cv2.PerspectiveTransform(SrcPoint, per_mat);

					TransRect.X = (int)(DstPoint[2].X);
					TransRect.Y = (int)(DstPoint[2].Y);
					TransRect.Height = (int)(DstPoint[0].Y - DstPoint[3].Y);
					TransRect.Width = (int)(DstPoint[3].X - DstPoint[2].X);
				}
			}
			return TransRect;
		}

	}
}
