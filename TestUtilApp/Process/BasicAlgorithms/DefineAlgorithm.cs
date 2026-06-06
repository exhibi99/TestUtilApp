using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	public class DefineAlgorithm
	{
		public static double RESET_VALUE = -999.999;

		public static double PROFILE_MIN = 0.0;
		public static double PROFILE_MAX = 10.0;
		public static double PROFILE_STEP_X = 2.0;
		public static double PROFILE_STEP_Z = 0.01;

		public enum RESULT
		{
			ERR = -2,
			NG = -1,
			NA = 0,
			OK = 100,
		}
		
		public enum ALGO_DIR
		{
			VER = 0,
			HOR,
			BOTH
		}
		
		public enum ALGO_SEARCH_DIR
		{
			T2B = 0,
			B2T,
			L2R,
			R2L,
			C2T = 0,
			C2B,
			T2C,
			B2C,
			L2C,
			R2C,
			C2L,
			C2R,
			B2W = 0,
			W2B,
		};
		
		public enum ALGO_ROTATE_FLAG
		{
			NONE = -999,
			CLOCK_WISE_90 = RotateFlags.Rotate90Clockwise,
			CLOCK_WISE_180 = RotateFlags.Rotate180,
			CLOCK_WISE_270 = RotateFlags.Rotate90Counterclockwise,
		}

		public static Scalar GetResultColor(RESULT result)
		{
			if (result == RESULT.ERR) return Scalar.Red;
			else if (result == RESULT.NG) return Scalar.Red;
			else if (result == RESULT.OK) return Scalar.Green;
			else return Scalar.Black;
		}

		public static double RAD2DEG(double rad)
		{
			return rad * 180.0 / Math.PI;
		}

		public static double DEG2RAD(double deg)
		{
			return deg * Math.PI / 180.0;
		}

		public static double DIST(double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
		}

		public static double DIST(double distX, double distY)
		{
			return Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
		}

		public static int ROUND(double val)
		{
			return (int)(val + 0.5);
		}
	}
}