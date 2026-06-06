using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Process
{
	class BasicAlgoCoordinate
	{
		// 라인 위의 포인트를 리턴 (x좌표 지정)
		public static Point2d LinePointByX(Line2D line, double x)
		{
			double vx = line.Vx;
			double vy = line.Vy;
			double x0 = line.X1;
			double y0 = line.Y1;

			Point2d linePt = new Point2d(x, 0);
			if (vx == 0.0f) vx = 0.00001f;
			linePt.Y = (vy * (x - x0) / vx) + y0;
			return linePt;
		}

		// 라인 위의 포인트를 리턴 (y좌표 지정)
		public static Point2d LinePointByY(Line2D line, double y)
		{
			double vx = line.Vx;
			double vy = line.Vy;
			double x0 = line.X1;
			double y0 = line.Y1;

			Point2d linePt = new Point2d(0, y);
			if (vy == 0.0f) vy = 0.00001f;
			linePt.X = (vx * (y - y0) / vy) + x0;
			return linePt;
		}

		// 두 점을 지나는 라인을 리턴
		public static Line2D LineFrom2Points(Point2d pt1, Point2d pt2)
		{
			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;
			double vx = dx / Math.Sqrt(dx * dx + dy * dy);
			double vy = dy / Math.Sqrt(dx * dx + dy * dy);

			Line2D line = new Line2D((float)vx, (float)vy, (float)pt1.X, (float)pt1.Y);
			return line;
		}

		// 라인과 포인트의 수직 교차점을 계산
		public static Point2d LineIntersectPoint(Line2D line, Point2d pt)
		{
			Point2d intersect = new Point2d();

			double vx1 = line.Vx;
			double vy1 = line.Vy;
			double x1 = line.X1;
			double y1 = line.Y1;

			if (vx1 == 0 && vy1 != 0)
			{
				intersect.X = x1;
				intersect.Y = pt.Y;
				return intersect;
			}

			if (vy1 == 0 && vx1 != 0)
			{
				intersect.X = pt.X;
				intersect.Y = y1;
				return intersect;
			}

			double vx2 = 1 / vx1;
			double vy2 = -1 / vy1;
			double x2 = pt.X;
			double y2 = pt.Y;

			intersect.X = (vy1 * x1 / vx1 - vy2 * x2 / vx2 + y2 - y1) / (vy1 / vx1 - vy2 / vx2);
			intersect.Y = (vx1 * y1 / vy1 - vx2 * y2 / vy2 + x2 - x1) / (vx1 / vy1 - vx2 / vy2);

			return intersect;
		}

		// 라인과 라인의 교차점을 계산
		public static Point2d LineIntersectPoint(Line2D line1, Line2D line2)
		{
			Point2d intersect = new Point2d();

			double x1 = line1.X1;
			double y1 = line1.Y1;
			double u1 = line1.Vx;
			double u2 = line1.Vy;

			if (u1 == 0.0) u1 = 0.000000000001;
			if (u2 == 0.0) u2 = 0.000000000001;

			double x2 = line2.X1;
			double y2 = line2.Y1;
			double v1 = line2.Vx;
			double v2 = line2.Vy;

			if (v1 == 0.0) v1 = 0.000000000001;
			if (v2 == 0.0) v2 = 0.000000000001;

			intersect.X = (u2 * x1 / u1 - v2 * x2 / v1 + y2 - y1) / (u2 / u1 - v2 / v1);
			intersect.Y = (u1 * y1 / u2 - v1 * y2 / v2 + x2 - x1) / (u1 / u2 - v1 / v2);

			return intersect;
		}

		// 라인과 포인트 사이의 최소 거리를 리턴
		public static double LinePointDist(Line2D line, Point2d pt)
		{
			double vx = line.Vx;
			double vy = line.Vy;
			double x0 = line.X1;
			double y0 = line.Y1;

			Point3d a = new Point3d(pt.X, pt.Y, 0);
			Point3d p = new Point3d(x0, y0, 0);
			Vec3d pa = p - a;
			Vec3d u = new Vec3d(vx, vy, 0);
			Vec3d pau = VecCrossProduct(pa, u);

			double dist = VecSize(pau) / VecSize(u);
			return dist;
		}

		// 라인의 각도를 리턴 (radian)
		public static double LineAngle_rad(Line2D line)
		{
			double vx = line.Vx;
			double vy = line.Vy;
			double x0 = line.X1;
			double y0 = line.Y1;

			double rad = Math.Atan2(vy, vx);
			return rad;
		}

		// 라인의 각도를 리턴 (degree)
		public static double LineAngle_deg(Line2D line)
		{
			double vx = line.Vx;
			double vy = line.Vy;
			double x0 = line.X1;
			double y0 = line.Y1;

			double rad = Math.Atan2(vy, vx);
			double deg = RAD2DEG(rad);
			return deg;
		}

		// 두 점의 각도를 리턴 (radian)
		public static double PointToPointAngle_rad(Point2d pt1, Point2d pt2)
		{
			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;

			double rad = Math.Atan2(dy, dx);
			return rad;
		}
		// 두 점의 각도를 리턴 (degree)
		public static double PointToPointAngle_deg(Point2d pt1, Point2d pt2)
		{
			double dx = pt2.X - pt1.X;
			double dy = pt2.Y - pt1.Y;

			double rad = Math.Atan2(dy, dx);
			double deg = RAD2DEG(rad);
			return deg;
		}
		// 라인 위의 한 포인트에서 일정 거리 떨어져있는 라인 위의 포인트를 리턴
		public static Point2d PointFromLineByDist(Line2D line, Point2d ptInLine, double dist)
		{
			double vx = line.Vx;
			double vy = line.Vy;

			double x = ptInLine.X;
			double y = ptInLine.Y;

			Point2d pt = new Point2d(dist * vx + x, dist * vy + y);
			return pt;
		}

		// 라인위의 한 포인트에서 일정 거리 떨어져 있는 라인과 직각을 이루는 점, 라인의 진행 방향의 오른쪽에 있는 점 리턴
		public static Point2d CrossPointFromLineByDist(Line2D line, Point2d ptInLine, double dist)
		{
			double vx = line.Vx;
			double vy = line.Vy;

			double x = ptInLine.X;
			double y = ptInLine.Y;

			Point2d pt = new Point2d(dist * (-vy) + x, dist * vx + y);
			Point2d pt_round = new Point2d(Math.Floor(pt.X + 0.5), Math.Floor(pt.Y + 0.5));
			return pt_round;
		}

		// 수직방향 라인 위의 한 포인트에서 일정 거리 떨어져 있는 라인과 직각을 이루는 점 리턴
		public static Point2d CrossPtFromVerLineByDist(Line2D verLine, Point2d ptInLine, double dist, bool rightDir)
		{
			double vx = verLine.Vx;
			double vy = verLine.Vy;

			double x = ptInLine.X;
			double y = ptInLine.Y;

			Point2d pt1 = new Point2d(dist * (-vy) + x, dist * vx + y);
			Point2d pt2 = new Point2d(dist * vy + x, dist * (-vx) + y);

			Point2d pt_round1 = new Point2d(Math.Floor(pt1.X + 0.5), Math.Floor(pt1.Y + 0.5));
			Point2d pt_round2 = new Point2d(Math.Floor(pt2.X + 0.5), Math.Floor(pt2.Y + 0.5));

			if (rightDir == true)
			{
				if (pt_round1.X > pt_round2.X) return pt_round1;
				else return pt_round2;
			}
			else
			{
				if (pt_round1.X < pt_round2.X) return pt_round1;
				else return pt_round2;
			}
		}

		// 수평방향 라인 위의 한 포인트에서 일정 거리 떨어져 있는 라인과 직각을 이루는 점 리턴
		public static Point2d CrossPtFromHorLineByDist(Line2D horLine, Point2d ptInLine, double dist, bool bottomDir)
		{
			double vx = horLine.Vx;
			double vy = horLine.Vy;

			double x = ptInLine.X;
			double y = ptInLine.Y;

			Point2d pt1 = new Point2d(dist * (-vy) + x, dist * vx + y);
			Point2d pt2 = new Point2d(dist * vy + x, dist * (-vx) + y);

			Point2d pt_round1 = new Point2d(Math.Floor(pt1.X + 0.5), Math.Floor(pt1.Y + 0.5));
			Point2d pt_round2 = new Point2d(Math.Floor(pt2.X + 0.5), Math.Floor(pt2.Y + 0.5));

			if (bottomDir == true)
			{
				if (pt_round1.Y > pt_round2.Y) return pt_round1;
				else return pt_round2;
			}
			else
			{
				if (pt_round1.Y < pt_round2.Y) return pt_round1;
				else return pt_round2;
			}
		}

		// 두 포인트 사이의 선형 보간 좌표를 리턴 (x좌표 지정)
		public static Point2d InterpolationPointByX(Point2d pt1, Point2d pt2, double x)
		{
			Point2d interpolPt = new Point2d((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0);

			double a = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
			double b = pt1.Y - a * pt1.X;
			double y = a * x + b;

			interpolPt.X = x;
			interpolPt.Y = y;
			return interpolPt;
		}

		// 두 포인트 사이의 선형 보간 좌표를 리턴 (y좌표 지정)
		public static Point2d InterpolationPointByY(Point2d pt1, Point2d pt2, double y)
		{
			Point2d interpolPt = new Point2d((pt1.X + pt2.X) / 2.0, (pt1.Y + pt2.Y) / 2.0);

			double a = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
			double b = pt1.Y - a * pt1.X;
			double x = (y - b) / a;

			interpolPt.X = x;
			interpolPt.Y = y;
			return interpolPt;
		}

		// 두 벡터의 외적 계산
		public static Vec3d VecCrossProduct(Vec3d a, Vec3d b)
		{
			double a1 = a[0];
			double a2 = a[1];
			double a3 = a[2];

			double b1 = b[0];
			double b2 = b[1];
			double b3 = b[2];

			Vec3d ret = new Vec3d(a2 * b3 - a3 * b2, -(a1 * b3 - a3 * b1), a1 * b2 - a2 * b1);
			return ret;
		}

		// 벡터의 크기 계산
		public static double VecSize(Vec3d vec)
		{
			double a = vec[0];
			double b = vec[1];
			double c = vec[2];

			double size = Math.Sqrt(a * a + b * b + c * c);
			return size;
		}

		// 포인트를 주어진 회전 중심과 각도에 따라 회전
		public static Point2d RotatePt(Point2d pt, Point2d center, double deg)
		{
			Point2d shiftPt = pt - center;
			Point2d rotZeroPt = new Point2d(shiftPt.X * Math.Cos(DEG2RAD(deg)) - shiftPt.Y * Math.Sin(DEG2RAD(deg)),
							shiftPt.X * Math.Sin(DEG2RAD(deg)) + shiftPt.Y * Math.Cos(DEG2RAD(deg)));
			Point2d rotPt = rotZeroPt + center;
			return rotPt;
		}
	}
}
