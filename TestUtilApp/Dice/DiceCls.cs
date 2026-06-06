using OpenCvSharp;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TestUtilApp.Dice
{
	public class ResultDiceCls
	{
		public string img_path;
		public int label;
		public int pred;
		public string class_name;
		public List<float> conf;
	}

	class DiceCls : ADice
	{
		private object clsLock = new object();

		public DiceCls(string name) : base(name)
		{
		}

		public List<ResultDiceCls> Inference(Mat[] imgs)
		{
			foreach (var img in imgs)
			{
				if (img.Channels() != 3) Cv2.CvtColor(img, img, ColorConversionCodes.GRAY2BGR);
			}

			List<ResultDiceCls> res = new List<ResultDiceCls>();
			string res_string = null;
			
			lock(clsLock)
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
				JObject jobj = JObject.Parse(res_string);
				foreach (KeyValuePair<string, JToken> property in jobj) //이미지단위
				{
					ResultDiceCls resUnit = new ResultDiceCls();
					resUnit.img_path = property.Key;
					resUnit.label = jobj[property.Key]["label"].ToObject<int>();
					resUnit.pred = jobj[property.Key]["pred"].ToObject<int>();
					resUnit.class_name = jobj[property.Key]["class_name"].ToString();
					List<float> confs = new List<float>();
					foreach (JToken conf in jobj[property.Key]["conf"])
					{
						confs.Add(conf.ToObject<float>());
					}
					resUnit.conf = confs;
					res.Add(resUnit);
				}
				return res;
			}
			catch (Exception e)
			{
				Console.WriteLine("DiceCls Error: " + e.Message);
				return res;
			}
		}
	}
}
