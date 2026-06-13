using OpenCvSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TestUtilApp.Dice
{
    class ResultDiceSeg
    {
        public string img_path;
        public int img_width;
        public int img_height;
        public Mat segmentMap; //MatType.CV_8UC1
    }

    class DiceSeg : ADice
	{
        private object segLock = new object();

		public DiceSeg(string name) : base(name)
		{
		}


        public List<ResultDiceSeg> Inference(Mat[] imgs)
		{
            foreach (var img in imgs)
            {
                if (img.Channels() != 3) Cv2.CvtColor(img, img, ColorConversionCodes.GRAY2BGR);
            }

			List<ResultDiceSeg> res = new List<ResultDiceSeg>();
			string res_string = null;

			lock (segLock)
			{
				for (int cnt = 0; cnt <= retryCount; cnt++)
				{
					res_string = _Inference(imgs);
					if (res_string != null && res_string != "") break;
				}

			}

			if (res_string == null || res_string == "")
				return res;

			try
			{
				dynamic json = JsonConvert.DeserializeObject(res_string);
				foreach (var result in json)
				{
					var filename = result.Name;
					ResultDiceSeg resUnit = new ResultDiceSeg();
					resUnit.img_path = filename;
					resUnit.img_height = result.Value["img_size"].height;
					resUnit.img_width = result.Value["img_size"].width;
					var segmentMapArr = result.Value["segments"].ToObject<Byte[,]>();
					int arrRows = segmentMapArr.GetLength(0);
					int arrCols = segmentMapArr.GetLength(1);
					Console.WriteLine($"[DiceSeg] img_size={resUnit.img_width}x{resUnit.img_height} / segments array={arrCols}x{arrRows}");
					resUnit.segmentMap = new Mat(arrRows, arrCols, MatType.CV_8UC1, segmentMapArr);
					res.Add(resUnit);
				}
				return res;
			}

            catch (Exception e)
            {
				Console.WriteLine("DiceSeg Error: " + e.Message);
				return res;
            }
        }
	}
}
