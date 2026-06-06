//using OpenCvSharp;
//using TestUtilApp.Dice;
//using TestUtilApp.Utilities;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestUtilApp.Process
//{
//	class ProcessSingle
//	{
//		public static void DetTest(string testsetDir, string model)
//		{
//			Utils.WriteHead(model);

//			if (!DiceManager.IsLoad(model)) return;

//			List<string> subDirs = new List<string>();
//			if (!Utils.CheckValidDatasetDirectory(testsetDir, ref subDirs))
//				return;

//			foreach (var dir in subDirs)
//			{
//				string[] files = Directory.GetFiles(dir);
//				int totalCount = files.Count();

//				if (totalCount <= 0) continue;

//				string labeledClass = dir.Split(Path.DirectorySeparatorChar).Last();

//				var images = new List<Mat>();
//				var filePaths = new List<string>();
//				Utils.LoadFilePathsAndImages(dir, ref filePaths, ref images);
//				if (filePaths.Count() != images.Count())
//				{
//					Console.WriteLine($@"{labeledClass}_CountErr : filePath({filePaths.Count()}) != img({images.Count()})");
//					continue;
//				}
//				List<ResultDiceDet> diceResults = _DetInference(model, images.ToArray());
//				if (diceResults.Count() != images.Count())
//				{
//					Console.WriteLine($@"{labeledClass}_CountErr : dice({diceResults.Count()}) != img({images.Count()})");
//					continue;
//				}

//				int resultIndex = 0;
//				foreach (var result in diceResults)
//				{
//					var srcImg = images[resultIndex].Clone();
//					var resImg = BasicAlgoDisplay.MakeDetResultImage(ref srcImg, result, model);

//					string filePath = filePaths[resultIndex];
//					string resultFilePath = filePath.Replace(testsetDir, testsetDir + "_result");
//					Utils.DeleteDirectory(Path.GetDirectoryName(resultFilePath));
//					Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
//					Utils.SaveImageFile(resultFilePath, resImg);

//					resultIndex++;
//				}
//				Console.WriteLine($@"number of files: {totalCount}");
//			}
//			Utils.WriteTail(model);
//		}

//		public static void ClsTest(string testsetDir, string model)
//		{
//			Utils.WriteHead(model);

//			if (!DiceManager.IsLoad(model)) return;

//			List<string> subDirs = new List<string>();
//			if (!Utils.CheckValidDatasetDirectory(testsetDir, ref subDirs))
//				return;

//			foreach (var dir in subDirs)
//			{
//				string[] files = Directory.GetFiles(dir);
//				int totalCount = files.Count();

//				if (totalCount <= 0) continue;

//				string labeledClass = dir.Split(Path.DirectorySeparatorChar).Last();

//				var images = new List<Mat>();
//				var filePaths = new List<string>();
//				Utils.LoadFilePathsAndImages(dir, ref filePaths, ref images);
//				if (filePaths.Count() != images.Count())
//				{
//					Console.WriteLine($@"{labeledClass}_CountErr : filePath({filePaths.Count()}) != img({images.Count()})");
//					continue;
//				}
//				List<ResultDiceCls> diceResults = _ClsInference(model, images.ToArray());
//				if (diceResults.Count() != images.Count())
//				{
//					Console.WriteLine($@"{labeledClass}_CountErr : dice({diceResults.Count()}) != img({images.Count()})");
//					continue;
//				}

//				int resultIndex = 0;
//				foreach (var result in diceResults)
//				{
//					var srcImg = images[resultIndex].Clone();
//					var resImg = BasicAlgoDisplay.MakeClsResultImage(ref srcImg, result, model, labeledClass);

//					string filePath = filePaths[resultIndex];
//					string resultFilePath = filePath.Replace(testsetDir, testsetDir + "_result");
//					Utils.DeleteDirectory(Path.GetDirectoryName(resultFilePath));
//					Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
//					Utils.SaveImageFile(resultFilePath, resImg);

//					resultIndex++;
//				}
//				Console.WriteLine($@"number of files: {totalCount}");
//			}
//			Utils.WriteTail(model);
//		}

//		private static List<ResultDiceDet> _DetInference(string model, Mat[] imgs)
//		{
//			switch (model)
//			{
//				case nameof(DiceManager.LedDetPos):
//					return DiceManager.LedDetPos.Inference(imgs.ToArray());

//				default:
//					return new List<ResultDiceDet>();
//			}
//		}

//		public static List<ResultDiceCls> _ClsInference(string model, Mat[] imgs)
//		{
//			switch (model)
//			{
//				case nameof(DiceManager.LedClsStep):
//					return DiceManager.LedClsStep.Inference(imgs.ToArray());

//				case nameof(DiceManager.LedClsType):
//					return DiceManager.LedClsType.Inference(imgs.ToArray());

//				case nameof(DiceManager.LedClsAllLights):
//					return DiceManager.LedClsAllLights.Inference(imgs.ToArray());

//				default:
//					return new List<ResultDiceCls>();
//			}
//		}
//	}
//}
