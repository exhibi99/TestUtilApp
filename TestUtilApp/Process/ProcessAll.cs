//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenCvSharp;
//using TestUtilApp.Dice;
//using TestUtilApp.Utilities;
//using static TestUtilApp.Process.DefineAlgorithm;

//namespace TestUtilApp.Process
//{
//	public static class Spec
//	{
//		public static int SpecLedDetPosMinCount = 3;
//		public static int SpecLedClsStepMinCount = 1;
//		public static double SpecLedClsAllLightsRatio = 0.5;
//	}

//	public class MaterialInfo
//	{
//		public string Name = "";
//		public List<ImageInfo> ImageInfos = new List<ImageInfo>();
//		public Dictionary<string, int> StepImageInfoIndexs = new Dictionary<string, int>();
//		public Mat StepResultImage = new Mat();
//		public Mat FinalResultImage = new Mat();

//		public int DetPosCount = 0;
//		public Rect LedRect = new Rect();   // 대표 LedRect
//		public Rect Seg88Rect = new Rect(); // 대표 Seg88Rect
//		public RESULT Result = RESULT.NA;
//	}

//	public class ImageInfo
//	{
//		public string Name = "";
//		public int Index = -1;
//		public Mat SourceImage = new Mat();
//		public Mat CropLedImage = new Mat();
//		public Mat CropSeg88Image = new Mat();

//		public bool IsDetectRects = false;
//		public Rect LedRect = new Rect();
//		public Rect Seg88Rect = new Rect();
//		public string ClassStep = "";
//		public double ConfStep = 0.0;
//		public string ClassDisplayType = "";
//		public double ConfDisplayType = 0.0;
//		public string ClassAllLights = "";
//		public double ConfAllLights = 0.0;
//	}

//	class ProcessAll
//	{
//		public static MaterialInfo ProcessAllDemo(string materialDir)
//		{
//			Utils.WriteHead("ProcessAllDemo");
//			MaterialInfo material = new MaterialInfo();
//			material.Name = materialDir;

//			int index = 0;
//			var imagefiles = Utils.LoadFilePathsAndImages(materialDir);
//			foreach (var file in imagefiles)
//			{
//				// 1. Image Grab
//				Mat grabImg = file.Value;

//				// 2. 영상이 Grab되면 90도 회전하여 ImageInfo에 저장
//				ImageInfo imageInfo = new ImageInfo();
//				//Mat rotImg = BasicAlgoImage.RotateImage(ref grabImg, 90);
//				//imageInfo.SourceImage = rotImg.Clone();
//				imageInfo.SourceImage = grabImg.Clone();
//				imageInfo.Name = file.Key;
//				imageInfo.Index = index;
//				index++;

//				// 3. 영상 그랩 초반에 step9 영상이 3회 이상 들어오도록 셋팅했다고 가정하고,
//				// LED와 Seg88이 3회(SpecLedDetPosMinCount) 이상 detect될 때까지 LED_DetPos 실시간 처리
//				if (material.DetPosCount < Spec.SpecLedDetPosMinCount)
//				{
//					if (_LedDetPos(ref imageInfo, material.Name) == RESULT.OK)
//						material.DetPosCount++;
//				}
//				material.ImageInfos.Add(imageInfo);
//			}

//			// (*) step9가 3장(SpecLedDetPosMinCount) 이상이 아니면, NG
//			if (material.DetPosCount < Spec.SpecLedDetPosMinCount)
//			{
//				material.Result = RESULT.NG;
//				return material;
//			}

//			// 4. 영상 그랩이 완료되면 LEDRect와 Seg88Rect를 계산하고 모든 영상 crop
//			_CalcDetectedRects(ref material);
//			_CropAllImages(ref material);

//			// 5. 모든 영상을 가지고 시퀀스 Step 확인 (한꺼번에 LED_ClsStep에 inference)
//			// 6. 각 step별 대표 ImageInfo Index 추출
//			// (*) step이 총 9개가 아니면, NG
//			if (_LedClsStep(ref material) != RESULT.OK)
//			{
//				material.Result = RESULT.NG;
//				return material;
//			}

//			// 7. step9 영상들만 가지고 Display Type 확인 (추후 구현 예정)
//			RESULT resultDisplayType = _LedClsDisplayType(ref material);
//			// 8. step9 영상들만 가지고 올점등 확인
//			RESULT resultAllLights = _LedClsAllLights(ref material);

//			// (*) DisplayType이나 올점등 OK가 아니면, NG
//			if (resultDisplayType != RESULT.OK || resultAllLights != RESULT.OK)
//			{
//				material.Result = RESULT.NG;
//				return material;
//			}

//			Utils.WriteTail("ProcessAllDemo");
//			return material;
//		}

//		private static RESULT _LedDetPos(ref ImageInfo imageInfo, string materialName = "")
//		{
//			string diceModel = nameof(DiceManager.LedDetPos);

//			List<ResultDiceDet> diceResults = DiceManager.LedDetPos.Inference(imageInfo.SourceImage);
//			if (diceResults.Count() != 1)
//			{
//				Console.WriteLine($@"CountErr : dice({diceResults.Count()}) != img(1)");
//				return RESULT.ERR;
//			}

//			bool detectLed = false;
//			bool detectSeg88 = false;
//			var result = diceResults[0];
//			foreach (var rect in result.listRect)
//			{
//				if (rect.class_name == "led" && rect.conf > 0.9)
//                {
//					detectLed = true;
//					imageInfo.LedRect = BasicAlgoRect.IntRect(rect.rect);
//				}
//				else if (rect.class_name == "88" && rect.conf > 0.85)
//				{
//					detectSeg88 = true;
//					imageInfo.Seg88Rect = BasicAlgoRect.IntRect(rect.rect);
//				}
//			}

//			Mat resImg = BasicAlgoDisplay.MakeDetResultImage(ref imageInfo.SourceImage, result, diceModel);
//			string resultFilePath = imageInfo.Name.Replace(materialName, $@"{materialName}_Result\{diceModel}");
//			Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
//			Utils.SaveImageFile(resultFilePath, resImg);

//			if (!detectLed || !detectSeg88)
//				return RESULT.NG;

//			imageInfo.IsDetectRects = true;
//			return RESULT.OK;
//		}

//		private static void _CalcDetectedRects(ref MaterialInfo material)
//		{
//			material.LedRect = new Rect();
//			material.Seg88Rect = new Rect();

//			int count = 0;
//			foreach (var imageInfo in material.ImageInfos)
//			{
//				if (imageInfo.IsDetectRects)
//				{
//					material.LedRect = BasicAlgoRect.SumRect(material.LedRect, imageInfo.LedRect);
//					material.Seg88Rect = BasicAlgoRect.SumRect(material.Seg88Rect, imageInfo.Seg88Rect);
//					count++;
//				}
//			}
//			if (count > 0)
//			{
//				material.LedRect = BasicAlgoRect.AvgRect(material.LedRect, count);
//				material.Seg88Rect = BasicAlgoRect.AvgRect(material.Seg88Rect, count);
//			}
//		}

//		private static void _CropAllImages(ref MaterialInfo material)
//		{
//			foreach (var imageInfo in material.ImageInfos)
//			{
//				if (imageInfo.SourceImage.Empty())
//					continue;

//				Mat srcImg = imageInfo.SourceImage.Clone();
//				Rect ledRect = BasicAlgoRect.AdjustRect(material.LedRect, srcImg.Size(), false);
//				Rect Seg88Rect = BasicAlgoRect.AdjustRect(material.Seg88Rect, srcImg.Size(), false);
//				imageInfo.CropLedImage = srcImg[ledRect].Clone();
//				imageInfo.CropSeg88Image = srcImg[Seg88Rect].Clone();
//			}
//		}

//		private static RESULT _LedClsStep(ref MaterialInfo material)
//		{
//			string diceModel = nameof(DiceManager.LedClsStep);

//			List<ImageInfo> imageInfos = new List<ImageInfo>();
//			List<Mat> images = new List<Mat>();
//			foreach (var imageInfo in material.ImageInfos)
//			{
//				if (!imageInfo.CropLedImage.Empty())
//				{
//					imageInfos.Add(imageInfo);
//					images.Add(imageInfo.CropLedImage.Clone());
//				}
//			}
			
//			List<ResultDiceCls> diceResults = DiceManager.LedClsStep.Inference(images.ToArray());
//			if (diceResults.Count() != images.Count())
//			{
//				Console.WriteLine($@"CountErr : dice({diceResults.Count()}) != img({images.Count()})");
//				return RESULT.ERR;
//			}

//			material.StepImageInfoIndexs.Clear();
//			int curStep = 0;
//			int resultIndex = 0;
//			foreach (var result in diceResults)
//			{
//				var imageInfo = imageInfos[resultIndex];
//				var srcImg = images[resultIndex].Clone();
//				var resImg = BasicAlgoDisplay.MakeClsResultImage(ref srcImg, result, diceModel);

//				string resultFilePath = imageInfo.Name.Replace(material.Name, $@"{material.Name}_Result\{diceModel}");
//				Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
//				Utils.SaveImageFile(resultFilePath, resImg);

//				if (result.class_name == $@"step{curStep + 1}")
//				{
//					material.StepImageInfoIndexs.Add(result.class_name, imageInfo.Index);
//					curStep++;
//				}

//				imageInfo.ClassStep = result.class_name;
//				imageInfo.ConfStep = result.conf.Max();

//				resultIndex++;
//			}

//			if (material.StepImageInfoIndexs.Count() == 9)
//				return RESULT.OK;
//			else
//				return RESULT.NG;
//		}

//		private static RESULT _LedClsDisplayType(ref MaterialInfo material)
//		{
//			return RESULT.OK;
//		}

//		private static RESULT _LedClsAllLights(ref MaterialInfo material)
//		{
//			string diceModel = nameof(DiceManager.LedClsAllLights);

//			List<ImageInfo> imageInfos = new List<ImageInfo>();
//			List<Mat> images = new List<Mat>();
//			foreach (var imageInfo in material.ImageInfos)
//			{
//				if (imageInfo.ClassStep == "step9")
//				{
//					imageInfos.Add(imageInfo);
//					images.Add(imageInfo.CropLedImage.Clone());
//				}
//			}

//			if (images.Count() <= 0)
//			{
//				Console.WriteLine("CountNG : there is not 'step9' image.");
//				return RESULT.NG;
//			}

//			List<ResultDiceCls> diceResults = DiceManager.LedClsAllLights.Inference(images.ToArray());
//			if (diceResults.Count() != images.Count())
//			{
//				Console.WriteLine($@"CountErr : dice({diceResults.Count()}) != img({images.Count()})");
//				return RESULT.ERR;
//			}

//			int specOkCount = (int)((double)images.Count() * Spec.SpecLedClsAllLightsRatio);
//			if (specOkCount <= 0) specOkCount = 1;

//			int okCount = 0;
//			int resultIndex = 0;
//			foreach (var result in diceResults)
//			{
//				var imageInfo = imageInfos[resultIndex];
//				var srcImg = images[resultIndex].Clone();
//				var resImg = BasicAlgoDisplay.MakeClsResultImage(ref srcImg, result, diceModel);

//				string resultFilePath = imageInfo.Name.Replace(material.Name, $@"{material.Name}_Result\{diceModel}");
//				Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
//				Utils.SaveImageFile(resultFilePath, resImg);

//				imageInfo.ClassAllLights = result.class_name;
//				imageInfo.ConfAllLights = result.conf.Max();
//				if (imageInfo.ClassAllLights == "ok")
//					okCount++;

//				resultIndex++;
//			}

//			if (okCount >= specOkCount) return RESULT.OK;
//			else return RESULT.NG;
//		}
//	}
//}