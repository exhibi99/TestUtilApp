using System;
using System.IO;

namespace TestUtilApp.Services
{
    public static class DiceDllInstaller
    {
        // 각 조건에 해당하는 소스 폴더명
        private const string FolderV1                = "01_DICEv1";
        private const string FolderV2UnderCuda128    = "02_DICEv2_under_cuda12.8";
        private const string FolderV2OverCuda128     = "03_DICEv2_over_cuda12.8";

        private static readonly Version Cuda128 = new Version(12, 8);

        public enum InstallResult { Success, AlreadyCurrent, SourceNotFound, FileLocked, Error }

        public static string ResolveSourceFolder(string diceVersion, string cudaVersion)
        {
            string solutionDir = FindSolutionDirectory();
            string diceFolderName = SelectFolderName(diceVersion, cudaVersion);
            return Path.Combine(solutionDir, "DICE", diceFolderName);
        }

        private static string FindSolutionDirectory()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            // First, check if DICE folder exists in exe directory
            string diceInExeDir = Path.Combine(exeDir, "DICE");
            if (Directory.Exists(diceInExeDir))
            {
                return exeDir;
            }

            // Otherwise, try to find solution directory
            // bin\Debug (or bin\Release) → bin → project → solution
            var dir = new System.IO.DirectoryInfo(exeDir);
            while (dir != null)
            {
                if (dir.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) && dir.Parent != null)
                {
                    // dir.Parent = project folder, dir.Parent.Parent = solution folder
                    return dir.Parent.Parent?.FullName ?? dir.Parent.FullName;
                }
                dir = dir.Parent;
            }

            return exeDir;
        }

        public static InstallResult Install(string diceVersion, string cudaVersion)
        {
            string destDir = AppDomain.CurrentDomain.BaseDirectory;
            string srcDir  = ResolveSourceFolder(diceVersion, cudaVersion);

            Logger.Info($"InstallDiceDlls: version={diceVersion}, cuda={cudaVersion}, src={srcDir}");

            if (!Directory.Exists(srcDir))
            {
                Logger.Warn($"Source folder not found: {srcDir}");
                return InstallResult.SourceNotFound;
            }

            bool anyLocked = false;
            bool anyChanged = false;

            foreach (string srcFile in Directory.GetFiles(srcDir, "*.*", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(srcFile);
                string destFile = Path.Combine(destDir, fileName);

                if (File.Exists(destFile) && FilesAreEqual(srcFile, destFile))
                {
                    Logger.Info($"  [SKIP] Already current: {fileName}");
                    continue;
                }

                try
                {
                    File.Copy(srcFile, destFile, overwrite: true);
                    Logger.Info($"  [COPY] {fileName}");
                    anyChanged = true;
                }
                catch (IOException ioEx)
                {
                    Logger.Warn($"  [LOCKED] {fileName}: {ioEx.Message}");
                    anyLocked = true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"  [ERROR] {fileName}", ex);
                    return InstallResult.Error;
                }
            }

            var result = anyLocked ? InstallResult.FileLocked
                       : anyChanged ? InstallResult.Success
                       : InstallResult.AlreadyCurrent;

            Logger.Info($"InstallDiceDlls result: {result}");
            return result;
        }

        public static void DeleteOldDlls(int timeoutMs = 5000)
        {
            string destDir = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                Logger.Info($"DeleteOldDlls started (timeout: {timeoutMs}ms, dir: {destDir})");

                string[] baseFileNames = { "Python.Runtime.dll", "DICE_Library.dll" };

                var filesToDelete = new System.Collections.Generic.HashSet<string>(baseFileNames, StringComparer.OrdinalIgnoreCase);

                var pythonVersionDlls = Directory.GetFiles(destDir, "python[0-9]*.dll", SearchOption.TopDirectoryOnly);
                foreach (var pythonDll in pythonVersionDlls)
                    filesToDelete.Add(Path.GetFileName(pythonDll));

                Logger.Info($"Files to delete: {string.Join(", ", filesToDelete)}");

                foreach (string fileName in filesToDelete)
                {
                    string fullPath = Path.Combine(destDir, fileName);
                    if (!File.Exists(fullPath))
                    {
                        Logger.Info($"[SKIP] Not found: {fileName}");
                        continue;
                    }

                    Logger.Info($"[START] Deleting: {fileName}");
                    bool deleted = RetryDelete(fullPath, timeoutMs);

                    if (deleted)
                        Logger.Info($"[OK] Deleted: {fileName}");
                    else
                        Logger.Warn($"[FAILED] Could not delete after {timeoutMs}ms: {fileName}");
                }

                Logger.Info("DeleteOldDlls completed");
            }
            catch (Exception ex)
            {
                Logger.Error("Error in DeleteOldDlls", ex);
            }
        }

        private static bool RetryDelete(string filePath, int timeoutMs)
        {
            int checkInterval = 100;
            int elapsed = 0;
            int attemptCount = 0;

            while (elapsed < timeoutMs)
            {
                attemptCount++;
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Logger.Info($"  [#{attemptCount}] Already deleted");
                        return true;
                    }

                    File.Delete(filePath);
                    Logger.Info($"  [#{attemptCount}] Deleted successfully");
                    return true;
                }
                catch (IOException ioEx)
                {
                    Logger.Info($"  [#{attemptCount}] Locked ({elapsed}ms): {ioEx.Message}");
                }
                catch (UnauthorizedAccessException uaEx)
                {
                    Logger.Info($"  [#{attemptCount}] Access denied ({elapsed}ms): {uaEx.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"  [#{attemptCount}] Unexpected error", ex);
                    return false;
                }

                if (elapsed + checkInterval >= timeoutMs)
                    break;

                System.Threading.Thread.Sleep(checkInterval);
                elapsed += checkInterval;
            }

            Logger.Warn($"  [TIMEOUT] Gave up after {attemptCount} attempts ({elapsed}ms)");
            return false;
        }

        private static string SelectFolderName(string diceVersion, string cudaVersion)
        {
            bool isV1 = string.Equals(diceVersion, "v1", StringComparison.OrdinalIgnoreCase);
            if (isV1)
                return FolderV1;

            // v2: CUDA 버전으로 분기
            if (TryParseCudaVersion(cudaVersion, out Version cuda) && cuda >= Cuda128)
                return FolderV2OverCuda128;

            return FolderV2UnderCuda128;
        }

        private static bool TryParseCudaVersion(string cudaVersion, out Version version)
        {
            version = null;
            if (string.IsNullOrEmpty(cudaVersion)) return false;

            // "12.8", "13.1" 등 major.minor 형태
            string[] parts = cudaVersion.Split('.');
            if (parts.Length < 2) return false;

            if (int.TryParse(parts[0], out int major) && int.TryParse(parts[1], out int minor))
            {
                version = new Version(major, minor);
                return true;
            }
            return false;
        }

        private static bool FilesAreEqual(string a, string b)
        {
            try
            {
                var infoA = new FileInfo(a);
                var infoB = new FileInfo(b);
                if (infoA.Length != infoB.Length) return false;

                // 크기가 같으면 마지막 수정 시간까지 확인 (전체 바이트 비교는 비용 큼)
                return infoA.LastWriteTimeUtc == infoB.LastWriteTimeUtc;
            }
            catch
            {
                return false;
            }
        }

        public static string GetPythonVersion(string diceVersion, string cudaVersion)
        {
            bool isV1 = string.Equals(diceVersion, "v1", StringComparison.OrdinalIgnoreCase);

            if (isV1)
                return "Python 3.7";

            // v2: CUDA 버전으로 분기
            if (TryParseCudaVersion(cudaVersion, out Version cuda) && cuda >= Cuda128)
                return "Python 3.9";

            return "Python 3.8";
        }
    }
}
