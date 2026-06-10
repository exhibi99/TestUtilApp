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

        public static void DeleteOldDlls()
        {
            string destDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] filesToDelete = { "Python.Runtime.dll", "DICE_Library.dll" };

            // Python*.dll 찾기
            var pythonDlls = Directory.GetFiles(destDir, "python*.dll", SearchOption.TopDirectoryOnly);

            // 삭제할 파일 목록
            var allFiles = new System.Collections.Generic.List<string>(filesToDelete);
            allFiles.AddRange(pythonDlls);

            foreach (string file in allFiles)
            {
                string fullPath = Path.Combine(destDir, file);
                try
                {
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
                catch (IOException)
                {
                    // 파일이 잠겨 있을 수 있음 (무시하고 계속)
                }
                catch
                {
                    // 다른 오류도 무시하고 계속
                }
            }
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
    }
}
