using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenCvSharp;

namespace TestUtilApp.Utilities
{
	class Utils
	{
		public static void WriteHead(string processName = "")
		{
			Console.WriteLine(Environment.NewLine);
			Console.WriteLine("==========================");
			Console.WriteLine(processName + " processing begin.");
			Console.WriteLine("==========================");
		}

		public static void WriteTail(string processName = "")
		{
			Console.WriteLine("==========================");
			Console.WriteLine(processName + " processing complete");
			Console.WriteLine("==========================");
		}

		public static void SaveImageFile(string filePath, Mat image)
		{
			if (!image.Empty())
				Cv2.ImWrite(filePath, image);
		}
		
		//
		// Directory, File
		//
		private static object LockObj = new object();
		public static bool CreateDirectory(string path)
		{
			lock (LockObj)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				if (!dirInfo.Exists)
				{
					dirInfo.Create();
				}
				return true;
			}
		}

		public static bool DeleteDirectory(string path)
		{
			lock (LockObj)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				if (dirInfo.Exists)
				{
					foreach (var file in dirInfo.GetFiles())
					{
						file.Delete();
					}
					foreach (var dir in dirInfo.GetDirectories())
					{
						// Recursion function
						DeleteDirectory($@"{path}\{dir.Name}");
					}
					dirInfo.Delete(true);
				}
				return true;
			}
		}

		public static bool CopyFile(string orgPath, string copyPath)
		{
			if (orgPath == copyPath) return false;
			lock (LockObj)
			{
				if (File.Exists(orgPath))
				{
					File.Copy(orgPath, copyPath, true);
					return true;
				}
				return false;
			}
		}

		public static Dictionary<string, Mat> LoadFilePathsAndImages(string rootDir)
		{
			lock (LockObj)
			{
				Dictionary<string, Mat> images = new Dictionary<string, Mat>();
				DirectoryInfo dirInfo = new DirectoryInfo(rootDir);
				if (dirInfo.Exists)
				{
					foreach (var file in dirInfo.GetFiles())
					{
						if (file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".png" || file.Extension.ToLower() == ".jpg")
						{
							var img = Cv2.ImRead(file.FullName, ImreadModes.Grayscale);
							images.Add(file.FullName, img);
						}
					}
					foreach (var dir in dirInfo.GetDirectories())
					{
						string subDir = $@"{rootDir}\{dir.Name}";
						// Recursion function
						var subImages = LoadFilePathsAndImages(subDir);
						foreach (var sub in subImages)
							images.Add(sub.Key, sub.Value);
					}
				}
				return images;
			}
		}

		public static Dictionary<string, string> LoadFileNamesAndFilePaths(string rootDir)
		{
			lock (LockObj)
			{
				Dictionary<string, string> files = new Dictionary<string, string>();
				DirectoryInfo dirInfo = new DirectoryInfo(rootDir);
				if (dirInfo.Exists)
				{
					foreach (var file in dirInfo.GetFiles())
					{
						files.Add(file.Name, file.FullName);
					}
					foreach (var dir in dirInfo.GetDirectories())
					{
						string subDir = $@"{rootDir}\{dir.Name}";
						// Recursion function
						var subfiles = LoadFileNamesAndFilePaths(subDir);
						foreach (var sub in subfiles)
							files.Add(sub.Key, sub.Value);
					}
				}
				return files;
			}
		}

		public static void LoadFilePathsAndImages(string rootDir, ref List<string> filePaths, ref List<Mat> images)
		{
			lock (LockObj)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(rootDir);
				if (dirInfo.Exists)
				{
					foreach (var file in dirInfo.GetFiles())
					{
						if (file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".png")
						{
							var img = Cv2.ImRead(file.FullName, ImreadModes.Grayscale);
							images.Add(img);
							filePaths.Add(file.FullName);
						}
					}
					foreach (var dir in dirInfo.GetDirectories())
					{
						string subDir = $@"{rootDir}\{dir.Name}";
						// Recursion function
						LoadFilePathsAndImages(subDir, ref filePaths, ref images);
					}
				}
			}
		}

		public static string GetDirectoryPathWithDialog()
		{
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.IsFolderPicker = true;
				dialog.Multiselect = false;
				if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
				{
					return dialog.FileName;
				}
			}
			return "";
		}
		
		public static bool CheckValidDatasetDirectory(string datasetDir, ref List<string> subDirs)
		{
			lock (LockObj)
			{
				if (datasetDir == "")
					return false;

				DirectoryInfo dirInfo = new DirectoryInfo(datasetDir);
				if (!dirInfo.Exists)
					return false;

				subDirs = Directory.GetDirectories(datasetDir).ToList();
				if (subDirs.Count() > 0)
					return true;
				return false;
			}
		}

	}
}
