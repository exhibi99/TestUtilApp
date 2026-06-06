//using OpenCvSharp;
//using TestUtilApp.Dice;
//using TestUtilApp.Utilities;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace TestUtilApp.Process
//{
//	class PreProcess
//	{
//		public static string Rotate(string rootDir, double deg)
//		{
//			Utils.WriteHead("Rotate");
//			var images = Utils.LoadFilePathsAndImages(rootDir);
//			int imageCount = images.Count();
//			Console.WriteLine($@"number of files: {imageCount}");

//			string newDir = rootDir + "_rotate";
//			foreach (var img in images)
//			{
//				Mat srcImg = img.Value.Clone();
//				Mat rotImg = BasicAlgoImage.RotateImage(ref srcImg, deg);
				
//				string newFilePath = img.Key.Replace(rootDir, newDir);
//				Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//				Utils.SaveImageFile(newFilePath, rotImg);
//			}
//			Utils.WriteTail("Rotate");
//			return newDir;
//		}

//		public static string ManualCrop(string rootDir, Rect roiRect)
//		{
//			if (roiRect.X < 0 || roiRect.Y < 0 || roiRect.Width < 0 || roiRect.Height < 0)
//				return rootDir;

//			Utils.WriteHead("ManualCrop");
//			var images = Utils.LoadFilePathsAndImages(rootDir);
//			int imageCount = images.Count();
//			Console.WriteLine($@"number of files: {imageCount}");

//			string newDir = rootDir + "_crop";
//			foreach (var img in images)
//			{
//				Mat srcImg = img.Value.Clone();
//				roiRect = BasicAlgoRect.AdjustRect(roiRect, srcImg.Size(), true);
//				Mat cropImg = srcImg[roiRect].Clone();

//				string newFilePath = img.Key.Replace(rootDir, newDir);
//				Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//				Utils.SaveImageFile(newFilePath, cropImg);
//			}
//			Utils.WriteTail("ManualCrop");
//			return newDir;
//		}

//		public static string ClassifyStep(string rootDir)
//		{
//			if (!DiceManager.LedClsStep.IsLoaded)
//				return rootDir;

//			Utils.WriteHead("ClassifyStep");
//			var images = new List<Mat>();
//			var filePaths = new List<string>();
//			Utils.LoadFilePathsAndImages(rootDir, ref filePaths, ref images);
//			Console.WriteLine($@"number of images: {images.Count()}");
//			Console.WriteLine($@"number of file paths: {filePaths.Count()}");

//			if (filePaths.Count() != images.Count())
//			{
//				Console.WriteLine($@"CountErr : filePath({filePaths.Count()}) != img({images.Count()})");
//				Utils.WriteTail("ClassifyStep");
//				return rootDir;
//			}

//			string newDir = rootDir + "_cls";
//			int groupSize = 100;
//			for (int i = 0; i < (int)(images.Count() / groupSize) + 1; i++)
//			{
//				int rangeStart = i * groupSize;
//				int rangeEnd = (i + 1) * groupSize;
//				if (rangeEnd >= images.Count()) rangeEnd = images.Count() - 1;
//				Console.WriteLine($@"range: {rangeStart} ~ {rangeEnd}");

//				var groupImages = images.GetRange(rangeStart, rangeEnd - rangeStart);
//				var groupFilePaths = filePaths.GetRange(rangeStart, rangeEnd - rangeStart);
				
//				List<ResultDiceCls> diceResults = DiceManager.LedClsStep.Inference(groupImages.ToArray());
//				if (diceResults.Count() != groupImages.Count())
//				{
//					Console.WriteLine($@"CountErr : dice({diceResults.Count()}) != img({groupImages.Count()})");
//					Utils.WriteTail("ClassifyStep");
//					return rootDir;
//				}
//				Console.WriteLine("inference is finished");

//				int resultIndex = 0;
//				foreach (var result in diceResults)
//				{
//					if (groupImages.Count() <= resultIndex || groupFilePaths.Count() <= resultIndex)
//					{
//						Console.WriteLine($@"CountErr : dice({diceResults.Count()}) != img({groupImages.Count()})");
//						Utils.WriteTail("ClassifyStep");
//						return rootDir;
//					}
//					var img = groupImages[resultIndex].Clone();
//					if (img.Empty()) continue;
//					if (img.Channels() != 1) Cv2.CvtColor(img, img, ColorConversionCodes.BGR2GRAY);

//					string fileName = Path.GetFileName(groupFilePaths[resultIndex]);
//					string newFilePath = Path.Combine(newDir, result.class_name, fileName);
//					Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//					Utils.SaveImageFile(newFilePath, img);
//					resultIndex++;
//				}
//			}
//			Utils.WriteTail("ClassifyStep");
//			return newDir;
//		}

//		public static string Split(string rootDir, double testSplitRatio=0.1)
//		{
//			Utils.WriteHead("Split");

//			List<string> subDirs = new List<string>();
//			if (!Utils.CheckValidDatasetDirectory(rootDir, ref subDirs))
//				return rootDir;

//			string newDir = rootDir + "_split";
//			foreach (var dir in subDirs)
//			{
//				var images = Utils.LoadFilePathsAndImages(dir);
//				int imageCount = images.Count();
//				Console.WriteLine($@"number of files: {imageCount}");

//				// random shuffle 3번 수행
//				var shuffledimages = images.OrderBy(a => Guid.NewGuid()).ToList();
//				shuffledimages = shuffledimages.OrderBy(a => Guid.NewGuid()).ToList();
//				shuffledimages = shuffledimages.OrderBy(a => Guid.NewGuid()).ToList();

//				int index = 0;
//				foreach (var img in shuffledimages)
//				{
//					string newFilePath = "";
//					if (index < imageCount * testSplitRatio)
//					{
//						newFilePath = img.Key.Replace(rootDir, $@"{newDir}\test");
//						index++;
//					}
//					else
//					{
//						newFilePath = img.Key.Replace(rootDir, $@"{newDir}\train");
//					}
//					Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//					Utils.SaveImageFile(newFilePath, img.Value);
//				}
//				Console.WriteLine($@"split: total {imageCount}, train {imageCount - index}, test {index}");
//			}
//			Utils.WriteTail("Split");
//			return newDir;
//		}

//		public static string ClassifyUsingBaseDir(string rootDir, string baseDir)
//		{
//			Utils.WriteHead("ClassifyUsingBaseDir");
//			var images = Utils.LoadFilePathsAndImages(rootDir);
//			int imageCount = images.Count();
//			var basefiles = Utils.LoadFileNamesAndFilePaths(baseDir);
//			int basefileCount = basefiles.Count();
//			Console.WriteLine($@"number of root files: {imageCount}");
//			Console.WriteLine($@"number of base files: {basefileCount}");

//			string newDir = rootDir + "_clsbasedir";
//			foreach (var img in images)
//			{
//				string fileName = Path.GetFileName(img.Key);
//				if (!basefiles.ContainsKey(fileName))
//				{
//					string newFilePath = Path.Combine(newDir, "need_to_check", fileName);
//					Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//					Utils.SaveImageFile(newFilePath, img.Value);
//				}
//				else
//				{
//					string newFilePath = basefiles[fileName];
//					newFilePath = newFilePath.Replace(baseDir, newDir);
//					Utils.CreateDirectory(Path.GetDirectoryName(newFilePath));
//					Utils.SaveImageFile(newFilePath, img.Value);
//				}
//			}
//			Utils.WriteTail("ClassifyUsingBaseDir");
//			return newDir;
//		}
//	}
//}
