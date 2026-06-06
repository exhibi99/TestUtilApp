using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace TestUtilApp.Dice
{
	public class ResultDiceDetRect
	{
		public int pred;
		public string class_name;
		public float conf;
		public Rect2f rect;
		public float width;
		public float height;
	}

	public class ResultDiceDet
	{
		public string img_path;
		public int num_rect;
		public List<ResultDiceDetRect> listRect = new List<ResultDiceDetRect>();
	}

	class DiceDet : ADice
    {
		private object detLock = new object();

		public DiceDet(string name) : base(name)
        {

        }

		public List<ResultDiceDet> Inference(Mat img)
		{
			Mat[] arr = {img};
			return Inference(arr);
		}

		public List<ResultDiceDet> Inference(Mat[] imgs)
		{
			List<ResultDiceDet> res = new List<ResultDiceDet>();
			string res_string = null;

			lock (detLock)
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

					ResultDiceDet resDet = new ResultDiceDet();
					resDet.img_path = filename;

					foreach (var rect in result.Value)
					{
						if (rect.Name == "result_path")
							continue;

						var box = rect.Value["bbox"] ?? null;
						var width = box[2] - box[0];
						var height = box[3] - box[1];

						ResultDiceDetRect resRect = new ResultDiceDetRect();
						resRect.pred = rect.Value["pred"];
						resRect.class_name = rect.Value["class_name"];
						resRect.width = width;
						resRect.height = height;
						resRect.conf = rect.Value["conf"];
						resRect.rect = new Rect2f((float)box[0], (float)box[1], (float)width, (float)height);
						resDet.listRect.Add(resRect);
					}
					resDet.num_rect = resDet.listRect.Count;
					res.Add(resDet);
				}
				return res;
			}
			catch (Exception e)
			{
				Console.WriteLine("DiceDet Error: " + e.Message);
				return res;
			}
		}
	}
}
