using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	class BasicAlgoPixel
	{
		//public static byte GetPixel(Mat srcImg, Point pt)
		//{
		//	return srcImg.At<byte>(pt.X, pt.Y);
		//}

		public static byte GetPixel(Mat<byte> srcImg, Point pt)
		{
			return GetPixel(srcImg.GetIndexer(), pt);
		}

		public static byte GetPixel(MatIndexer<byte> indexer, Point pt)
		{
			return indexer[pt.X, pt.Y];
		}

		public static Vec3b GetPixel(Mat<Vec3b> srcImg, Point pt)
		{
			return GetPixel(srcImg.GetIndexer(), pt);
		}

		public static Vec3b GetPixel(MatIndexer<Vec3b> indexer, Point pt)
		{
			return indexer[pt.X, pt.Y];
		}
        public static Point MovingMaxSearching(ref Mat srcImg, int pos, int direction, int windowSize, int minAvg)
        {
            if (direction == (int)(DefineAlgorithm.ALGO_SEARCH_DIR.L2C))
            {
                int j = pos;
                int center = (int)(srcImg.Cols * 0.4);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(0, pos);

                if (center - windowSize <= 0) return maxPt;

                for (int i = 0; i < center - windowSize; i++)
                {
                    int pixelSum = 0;
                    for (int p = 0; p < windowSize; p++)
                    {
                        Point pt = new Point(i + p, j);
                        byte pixel = srcImg.At<byte>(pt.X, pt.Y);
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
                    int center = (int)(srcImg.Cols * 0.6);
                    double maxPixelAvg = 0.0;
                    Point maxPt = new Point(r, pos);

		            if (r - center - windowSize <= 0) return new Point(r, pos);

		            for (int i = r; i >= center + windowSize; i--)
		            {
			            int pixelSum = 0;
			            for (int p = 0; p<windowSize; p++)
			            {
				            Point pt = new Point(i - p, j);
                            Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                            pixelSum += (int) pixel;
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
                    int center = (int)(srcImg.Rows * 0.45);
                    double maxPixelAvg = 0.0;
                    Point maxPt = new Point(i, 0);

		            if (center - windowSize <= 0) return maxPt;

		            for (int j = 0; j<center - windowSize; j++)
	               	{
			            int pixelSum = 0;
			            for (int p = 0; p<windowSize; p++)
			            {
				            Point pt = new Point(i, j + p);
                            Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                            pixelSum += (int) pixel;
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
                int center = (int)(srcImg.Rows * 0.55);
                double maxPixelAvg = 0.0;
                Point maxPt = new Point(i, 0);

        		if (b - center - windowSize <= 0) return new Point(pos, b);

		        for (int j = b; j >= center + windowSize; j--)
		        {
			        int pixelSum = 0;
			        for (int p = 0; p<windowSize; p++)
			        {
				        Point pt = new Point(i, j - p);
                        Byte pixel = srcImg.At<Byte>(pt.X, pt.Y);
                        pixelSum += (int) pixel;
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

	}
}
