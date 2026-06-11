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

            if (!Directory.Exists(srcDir))
                return InstallResult.SourceNotFound;

            bool anyLocked = false;
            bool anyChanged = false;

            foreach (string srcFile in Directory.GetFiles(srcDir, "*.*", SearchOption.TopDirectoryOnly))
            {
                string fileName = Path.GetFileName(srcFile);
                string destFile = Path.Combine(destDir, fileName);

                // 내용이 같으면 건너뜀
                if (File.Exists(destFile) && FilesAreEqual(srcFile, destFile))
                    continue;

                try
                {
                    File.Copy(srcFile, destFile, overwrite: true);
                    anyChanged = true;
                }
                catch (IOException)
                {
                    // DLL이 이미 프로세스에 로드되어 잠긴 경우
                    anyLocked = true;
                }
                catch
                {
                    return InstallResult.Error;
                }
            }

            if (anyLocked)  return InstallResult.FileLocked;
            if (anyChanged) return InstallResult.Success;
            return InstallResult.AlreadyCurrent;
        }

        public static void DeleteOldDlls(int timeoutMs = 5000)
        {
            string destDir = AppDomain.CurrentDomain.BaseDirectory;
            string logFile = Path.Combine(destDir, "DLL_Delete_Log.txt");

            try
            {
                using (var log = new System.IO.StreamWriter(logFile, false))
                {
                    log.WriteLine($"=== DeleteOldDlls Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ===");
                    log.WriteLine($"Timeout: {timeoutMs}ms");
                    log.WriteLine($"Target directory: {destDir}");
                    log.Flush();

                    string[] baseFileNames = { "Python.Runtime.dll", "DICE_Library.dll" };

                    // 삭제할 파일 목록 (중복 제거를 위해 HashSet 사용)
                    var filesToDelete = new System.Collections.Generic.HashSet<string>(baseFileNames, StringComparer.OrdinalIgnoreCase);

                    // Python 버전 DLL 찾기 (python3.dll, python37.dll, python38.dll, python39.dll)
                    // Note: "python*.dll" would also match "Python.Runtime.dll", so we're more specific
                    var pythonVersionDlls = Directory.GetFiles(destDir, "python[0-9]*.dll", SearchOption.TopDirectoryOnly);
                    foreach (var pythonDll in pythonVersionDlls)
                    {
                        string fileName = Path.GetFileName(pythonDll);
                        filesToDelete.Add(fileName);
                    }

                    log.WriteLine($"Files to delete: {filesToDelete.Count}");
                    foreach (var f in filesToDelete)
                        log.WriteLine($"  - {f}");
                    log.Flush();

                    foreach (string fileName in filesToDelete)
                    {
                        string fullPath = Path.Combine(destDir, fileName);
                        if (!File.Exists(fullPath))
                        {
                            log.WriteLine($"[SKIP] File not found: {fileName}");
                            log.Flush();
                            continue;
                        }

                        // Try to delete with retry logic
                        log.WriteLine($"[START] Deleting: {fileName}");
                        log.Flush();

                        bool deleted = RetryDelete(fullPath, timeoutMs, log);

                        if (!deleted)
                        {
                            log.WriteLine($"[FAILED] Failed to delete after {timeoutMs}ms: {fileName}");
                        }
                        else
                        {
                            log.WriteLine($"[SUCCESS] Deleted: {fileName}");
                        }
                        log.Flush();
                    }

                    log.WriteLine($"=== DeleteOldDlls Completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ===");
                    log.Flush();
                }

                // 로그 파일 내용을 Console에도 출력
                string logContent = File.ReadAllText(logFile);
                Console.WriteLine(logContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteOldDlls: {ex.Message}");
            }
        }

        private static bool RetryDelete(string filePath, int timeoutMs, System.IO.StreamWriter log)
        {
            int checkInterval = 100; // ms
            int elapsed = 0;
            int attemptCount = 0;

            while (elapsed < timeoutMs)
            {
                attemptCount++;
                try
                {
                    if (!File.Exists(filePath))
                    {
                        log.WriteLine($"  [Attempt {attemptCount}] File already deleted");
                        return true;
                    }

                    File.Delete(filePath);
                    log.WriteLine($"  [Attempt {attemptCount}] ✓ Successfully deleted");
                    return true; // Successfully deleted
                }
                catch (IOException ioEx)
                {
                    // File still locked, retry
                    log.WriteLine($"  [Attempt {attemptCount}] ✗ Locked - {ioEx.Message} (elapsed: {elapsed}ms/{timeoutMs}ms)");
                    log.Flush();
                }
                catch (UnauthorizedAccessException uaEx)
                {
                    // File in use by another process, retry
                    log.WriteLine($"  [Attempt {attemptCount}] ✗ Access Denied - {uaEx.Message} (elapsed: {elapsed}ms/{timeoutMs}ms)");
                    log.Flush();
                }
                catch (Exception ex)
                {
                    log.WriteLine($"  [Attempt {attemptCount}] ✗ Error - {ex.GetType().Name}: {ex.Message}");
                    return false; // Other error, don't retry
                }

                // Wait before next retry attempt (for all retryable exceptions)
                if (elapsed + checkInterval >= timeoutMs)
                    break;

                System.Threading.Thread.Sleep(checkInterval);
                elapsed += checkInterval;
            }

            log.WriteLine($"  [TIMEOUT] Gave up after {attemptCount} attempts ({elapsed}ms)");
            return false; // Timeout reached
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
