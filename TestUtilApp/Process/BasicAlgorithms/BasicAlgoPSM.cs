using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilApp.Process
{
	class BasicAlgoPSM
	{
		private const int DIR_X = 0;
		private const int DIR_Y = 1;
		private const int DIR_XY = 2;
		private const int DIR_XY_MARKING = 3;
		private const int METHOD_SUB = 0; // Center에서 L/R/T/B를 뺌 -> 밝은 불량 검출
        private const int METHOD_SUB_INV = 0; // L/R/T/B에서 Center를 뺌 -> 어두운 불량 검출
        private const int METHOD_ABSDIFF = 1;
		private const int METHOD_XOR = 2;

		public static Mat MakePSMImage_X_Sub(ref Mat srcImg, int stepX)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, 0, DIR_X, METHOD_SUB);
			return psmImg;
		}

		public static Mat MakePSMImage_Y_Sub(ref Mat srcImg, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, 0, stepY, DIR_Y, METHOD_SUB);
			return psmImg;
		}

		public static Mat MakePSMImage_XY_Sub(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY, METHOD_SUB);
			return psmImg;
		}

		public static Mat MakePSMMarkingImage_XY_Sub(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY_MARKING, METHOD_SUB);
			return psmImg;
        }
        public static Mat MakePSMImage_X_Sub_Inv(ref Mat srcImg, int stepX)
        {
            Mat psmImg = MakePSMImage(ref srcImg, stepX, 0, DIR_X, METHOD_SUB_INV);
            return psmImg;
        }

        public static Mat MakePSMImage_Y_Sub_Inv(ref Mat srcImg, int stepY)
        {
            Mat psmImg = MakePSMImage(ref srcImg, 0, stepY, DIR_Y, METHOD_SUB_INV);
            return psmImg;
        }

        public static Mat MakePSMImage_XY_Sub_Inv(ref Mat srcImg, int stepX, int stepY)
        {
            Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY, METHOD_SUB_INV);
            return psmImg;
        }

        public static Mat MakePSMMarkingImage_XY_Sub_Inv(ref Mat srcImg, int stepX, int stepY)
        {
            Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY_MARKING, METHOD_SUB_INV);
            return psmImg;
        }

        public static Mat MakePSMImage_X_AbsDiff(ref Mat srcImg, int stepX)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, 0, DIR_X, METHOD_ABSDIFF);
			return psmImg;
		}

		public static Mat MakePSMImage_Y_AbsDiff(ref Mat srcImg, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, 0, stepY, DIR_Y, METHOD_ABSDIFF);
			return psmImg;
		}

		public static Mat MakePSMImage_XY_AbsDiff(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY, METHOD_ABSDIFF);
			return psmImg;
		}

		public static Mat MakePSMMarkingImage_XY_AbsDiff(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY_MARKING, METHOD_ABSDIFF);
			return psmImg;
		}

		public static Mat MakePSMImage_X_Xor(ref Mat srcImg, int stepX)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, 0, DIR_X, METHOD_XOR);
			return psmImg;
		}

		public static Mat MakePSMImage_Y_Xor(ref Mat srcImg, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, 0, stepY, DIR_Y, METHOD_XOR);
			return psmImg;
		}

		public static Mat MakePSMImage_XY_Xor(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY, METHOD_XOR);
			return psmImg;
		}

		public static Mat MakePSMMarkingImage_XY_Xor(ref Mat srcImg, int stepX, int stepY)
		{
			Mat psmImg = MakePSMImage(ref srcImg, stepX, stepY, DIR_XY_MARKING, METHOD_XOR);
			return psmImg;
		}

		private static Mat MakePSMImage(ref Mat srcImg, int stepX, int stepY, int dir, int method)
		{
			Mat psmImg = new Mat(srcImg.Size(), srcImg.Type(), new Scalar(0));

			int width = srcImg.Cols - (stepX * 2);
			int height = srcImg.Rows - (stepY * 2);

			Mat leftImg;
			Mat rightImg;
			Mat topImg;
			Mat bottomImg;
			Mat centerImg;
			
			Rect rect = new Rect(stepX, stepY, width, height);
			centerImg = srcImg[rect].Clone();

			Mat psmLRImg = new Mat();
			Mat psmTBImg = new Mat();

			if (dir == DIR_X || dir == DIR_XY || dir == DIR_XY_MARKING)
			{
				rect = new Rect(0, stepY, width, height);
				leftImg = srcImg[rect].Clone();

				rect = new Rect(stepX * 2, stepY, width, height);
				rightImg = srcImg[rect].Clone();

				Mat tmpImg1 = new Mat();
				Mat tmpImg2 = new Mat();
				if (method == METHOD_SUB)
				{
					tmpImg1 = centerImg - leftImg;
					tmpImg2 = centerImg - rightImg;
					Cv2.Max(tmpImg1, tmpImg2, psmLRImg);
                }
                else if (method == METHOD_SUB_INV)
                {
                    tmpImg1 = leftImg - centerImg;
                    tmpImg2 = rightImg - centerImg;
                    Cv2.Max(tmpImg1, tmpImg2, psmLRImg);
                }
                else if (method == METHOD_ABSDIFF)
				{
					Cv2.Absdiff(leftImg, centerImg, tmpImg1);
					Cv2.Absdiff(rightImg, centerImg, tmpImg2);
					Cv2.Min(tmpImg1, tmpImg2, psmLRImg);
				}
				else
				{
					Cv2.BitwiseXor(leftImg, centerImg, tmpImg1);
					Cv2.BitwiseXor(rightImg, centerImg, tmpImg2);
					Cv2.Min(tmpImg1, tmpImg2, psmLRImg);
				}
			}
			if (dir == DIR_Y || dir == DIR_XY || dir == DIR_XY_MARKING)
            {
				rect = new Rect(stepX, 0, width, height);
				topImg = srcImg[rect].Clone();

				rect = new Rect(stepX, stepY * 2, width, height);
				bottomImg = srcImg[rect].Clone();

				Mat tmpImg1 = new Mat();
				Mat tmpImg2 = new Mat();
				if (method == METHOD_SUB)
				{
					tmpImg1 = centerImg - topImg;
					tmpImg2 = centerImg - bottomImg;
					Cv2.Max(tmpImg1, tmpImg2, psmTBImg);
                }
                else if (method == METHOD_SUB_INV)
                {
                    tmpImg1 = topImg - centerImg;
                    tmpImg2 = bottomImg - centerImg;
                    Cv2.Max(tmpImg1, tmpImg2, psmTBImg);
                }
                else if (method == METHOD_ABSDIFF)
				{
					Cv2.Absdiff(topImg, centerImg, tmpImg1);
					Cv2.Absdiff(bottomImg, centerImg, tmpImg2);
					Cv2.Min(tmpImg1, tmpImg2, psmTBImg);
				}
				else
				{
					Cv2.BitwiseXor(topImg, centerImg, tmpImg1);
					Cv2.BitwiseXor(bottomImg, centerImg, tmpImg2);
					Cv2.Min(tmpImg1, tmpImg2, psmTBImg);
				}
			}


            rect = new Rect(stepX, stepY, width, height);
            Mat psmRoiImg = psmImg.SubMat(rect);
            Mat temp = null;
            if (dir == DIR_X) psmLRImg.CopyTo(psmRoiImg);
            else if (dir == DIR_Y) psmTBImg.CopyTo(psmRoiImg);
            else if (dir == DIR_XY)
            {
                temp = new Mat();
                Cv2.Min(psmLRImg, psmTBImg, temp);
                temp.CopyTo(psmRoiImg);
            }
            else
            {
                temp = new Mat();
                Cv2.Max(psmLRImg, psmTBImg, temp);
                temp.CopyTo(psmImg.SubMat(rect));
            }
            return psmImg;
		}
	}
}
