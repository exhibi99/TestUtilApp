using DICE_Library;
using System;
using System.IO;
using System.Windows.Forms;

namespace TestUtilApp.Dice
{
	class DiceManager
	{
        public static readonly string dicePath = "C:\\DICE_PACKAGE";
		public static readonly string envPath = dicePath + "\\env\\DICE_ENV";
		public static readonly string enginePath = dicePath + "\\code\\engine";

		public static string Version { get; private set; }
		public static bool IsInit { get; private set; }
		public static bool IsLoaded { get; private set; }

        public static DiceDet DetectModel { get; private set; } = new DiceDet(nameof(DetectModel));
        public static DiceCls ClassifyModel_A { get; private set; } = new DiceCls(nameof(ClassifyModel_A));
        public static DiceCls ClassifyModel_B { get; private set; } = new DiceCls(nameof(ClassifyModel_B));

        private static bool useDetectModel = true;
        private static bool useClassifyModel_A = true;
        private static bool useClassifyModel_B = true;

        private static string pathDetectModel = "";
        private static string pathClassifyModel_A = "";
        private static string pathClassifyModel_B = "";

        private static readonly object syncRoot = new object();
        private static string loadedDetectModelPath = "";
        private static string loadedClassifyModelAPath = "";
        private static string loadedClassifyModelBPath = "";

        public static object lockObject { get; private set; }

        public static void SetDetectModel(bool use, string path)
        {
            useDetectModel = use;
            pathDetectModel = path;
        }

        public static void SetClassifyModel_A(bool use, string path)
		{
            useClassifyModel_A = use;
            pathClassifyModel_A = path;
		}

		public static void SetClassifyModel_B(bool use, string path)
		{
            useClassifyModel_B = use;
            pathClassifyModel_B = path;
        }

		public static bool Initialize()
		{
            bool result = EnsureInitialized();
            if (!result)
            {
                return false;
            }

            if (useDetectModel)
            {
                result &= EnsureModelLoaded(nameof(DetectModel), pathDetectModel);
            }

            if (useClassifyModel_A)
            {
                result &= EnsureModelLoaded(nameof(ClassifyModel_A), pathClassifyModel_A);
            }

            if (useClassifyModel_B)
            {
                result &= EnsureModelLoaded(nameof(ClassifyModel_B), pathClassifyModel_B);
            }

            RefreshLoadedState();
            return result;
		}

        public static bool EnsureInitialized()
        {
            if (IsInit)
            {
                return true;
            }

            lock (syncRoot)
            {
                if (IsInit)
                {
                    return true;
                }

                try
                {
                    bool result = DICE_Interface.init(envPath, enginePath);
                    if (!result)
                    {
                        Console.WriteLine("DICE_Interface.init() is failed.");
                        return IsInit = false;
                    }

                    Version = DICE_Interface.version();
                    Console.WriteLine("DICE Version: " + Version);
                    Console.WriteLine("Dice Initialization");
                    lockObject = syncRoot;
                    return IsInit = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Dice Initialization is failed.");
                    Console.WriteLine("DiceManager Error: " + e.Message);
                    return IsInit = false;
                }
            }
        }

        public static bool EnsureDetectModelLoaded(string path)
        {
            SetDetectModel(true, path);
            return EnsureModelLoaded(nameof(DetectModel), path);
        }

        public static bool EnsureClassifyModelALoaded(string path)
        {
            SetClassifyModel_A(true, path);
            return EnsureModelLoaded(nameof(ClassifyModel_A), path);
        }

        public static bool EnsureClassifyModelBLoaded(string path)
        {
            SetClassifyModel_B(true, path);
            return EnsureModelLoaded(nameof(ClassifyModel_B), path);
        }

        public static bool IsDetectModelLoaded(string path)
        {
            return IsModelLoaded(nameof(DetectModel), path);
        }

        public static bool IsClassifyModelALoaded(string path)
        {
            return IsModelLoaded(nameof(ClassifyModel_A), path);
        }

        public static bool IsClassifyModelBLoaded(string path)
        {
            return IsModelLoaded(nameof(ClassifyModel_B), path);
        }

        private static bool EnsureModelLoaded(string model, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"{model} path is empty.");
                return false;
            }

            string normalizedPath = NormalizeModelPath(path);

            if (!EnsureInitialized())
            {
                return false;
            }

            lock (syncRoot)
            {
                try
                {
                    if (model == nameof(DetectModel) && useDetectModel)
                    {
                        if (IsDetectModelLoaded(normalizedPath))
                        {
                            return true;
                        }

                        bool result = DetectModel.LoadModel(normalizedPath);
                        loadedDetectModelPath = result ? normalizedPath : "";
                        RefreshLoadedState();
                        if (!result) Console.WriteLine($@"{nameof(DetectModel)}.LoadModel() is failed.");
                        return result;
                    }

                    if (model == nameof(ClassifyModel_A) && useClassifyModel_A)
                    {
                        if (IsClassifyModelALoaded(normalizedPath))
                        {
                            return true;
                        }

                        bool result = ClassifyModel_A.LoadModel(normalizedPath);
                        loadedClassifyModelAPath = result ? normalizedPath : "";
                        RefreshLoadedState();
                        if (!result) Console.WriteLine($@"{nameof(ClassifyModel_A)}.LoadModel() is failed.");
                        return result;
                    }

                    if (model == nameof(ClassifyModel_B) && useClassifyModel_B)
                    {
                        if (IsClassifyModelBLoaded(normalizedPath))
                        {
                            return true;
                        }

                        bool result = ClassifyModel_B.LoadModel(normalizedPath);
                        loadedClassifyModelBPath = result ? normalizedPath : "";
                        RefreshLoadedState();
                        if (!result) Console.WriteLine($@"{nameof(ClassifyModel_B)}.LoadModel() is failed.");
                        return result;
                    }

                    return false;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{model} loading is failed.");
                    Console.WriteLine("DiceManager Error: " + e.Message);
                    RefreshLoadedState();
                    return false;
                }
            }
        }

		public static bool ReloadModel(string model, string path)
		{
            return EnsureModelLoaded(model, path);
		}

		public static bool IsLoad(string model)
		{
			switch (model)
			{
				case nameof(DiceManager.DetectModel):
					return DiceManager.DetectModel.IsLoaded;
                case nameof(DiceManager.ClassifyModel_A):
                    return DiceManager.ClassifyModel_A.IsLoaded;
                case nameof(DiceManager.ClassifyModel_B):
                    return DiceManager.ClassifyModel_B.IsLoaded;
                default:
					return false;
			}
		}

        private static bool IsModelLoaded(string model, string path)
        {
            bool checkPath = !string.IsNullOrWhiteSpace(path);
            string normalizedPath = checkPath ? NormalizeModelPath(path) : "";

            switch (model)
            {
				case nameof(DiceManager.DetectModel):
                    return DiceManager.DetectModel.IsLoaded &&
                        (!checkPath || string.Equals(loadedDetectModelPath, normalizedPath, StringComparison.OrdinalIgnoreCase));
                case nameof(DiceManager.ClassifyModel_A):
                    return DiceManager.ClassifyModel_A.IsLoaded &&
                        (!checkPath || string.Equals(loadedClassifyModelAPath, normalizedPath, StringComparison.OrdinalIgnoreCase));
                case nameof(DiceManager.ClassifyModel_B):
                    return DiceManager.ClassifyModel_B.IsLoaded &&
                        (!checkPath || string.Equals(loadedClassifyModelBPath, normalizedPath, StringComparison.OrdinalIgnoreCase));
                default:
					return false;
			}
        }

        private static string NormalizeModelPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            try
            {
                return Path.GetFullPath(path.Trim())
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch
            {
                return path.Trim()
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }

        private static void RefreshLoadedState()
        {
            IsLoaded = DetectModel.IsLoaded || ClassifyModel_A.IsLoaded || ClassifyModel_B.IsLoaded;
        }
	}
}
