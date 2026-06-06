using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	class BasicAlgoFocus
	{
        public static double GetFocusMeasure(Mat image)
        {
            double focusMeasure = -999.999;
            if (image == null) return focusMeasure;

            focusMeasure = Tenengrad(image);
            return focusMeasure;
        }

        private static double VarianceOfLaplacian(Mat image)
        {
            using (Mat imageTo64FC3 = new Mat())
            {
                image.ConvertTo(imageTo64FC3, MatType.CV_64FC3);

                int ksize = 3, scale = 1, delta = 0;
                var laplacian = new Mat();
                Cv2.Laplacian(imageTo64FC3, laplacian, MatType.CV_64FC4, ksize, scale, delta);
                Cv2.MeanStdDev(laplacian, out var mean, out var stddev);

                return stddev.Val0 * stddev.Val0;
            }
        }

        private static double ModifiedLaplacian(Mat image)
        {
            Mat gaussianKernel = Cv2.GetGaussianKernel(3, -1, MatType.CV_64FC1);
            double[] m = new double[] { -1.0, 2.0, -1.0 };
            Mat secondDerivativeKernel = new Mat<double>(3, 1, m);
            double focusMeasure = 0;

            using (Mat Lx = new Mat())
            {
                Cv2.SepFilter2D(image, Lx, MatType.CV_64FC1, secondDerivativeKernel, gaussianKernel);
                focusMeasure += Cv2.Mean(Cv2.Abs(Lx)).Val0;
            }
            using (Mat Ly = new Mat())
            {
                Cv2.SepFilter2D(image, Ly, MatType.CV_64FC1, gaussianKernel, secondDerivativeKernel);
                focusMeasure += Cv2.Mean(Cv2.Abs(Ly)).Val0;
            }

            return focusMeasure;
            
        }

        private static double Tenengrad(Mat image)
        {
            int ksize = 3;
            double focusMeasure = 0.0;

            using (Mat Gx = new Mat())
            {
                Cv2.Sobel(image, Gx, MatType.CV_64FC1, 1, 0, ksize);
                focusMeasure += Cv2.Mean(Gx.Mul(Gx)).Val0;
            }

            using (Mat Gy = new Mat())
            {
                Cv2.Sobel(image, Gy, MatType.CV_64FC1, 0, 1, ksize);
                focusMeasure += Cv2.Mean(Gy.Mul(Gy)).Val0;
            }

            return focusMeasure;
        }
    }
}
