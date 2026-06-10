using System;
using System.IO;

namespace TestUtilApp.Services
{
    public class CudaInfo
    {
        public bool HasGpu { get; set; }
        public string DriverVersion { get; set; }
        public string CudaVersion { get; set; }

        public string DisplayText
        {
            get
            {
                if (!HasGpu) return "GPU: N/A";
                if (!string.IsNullOrEmpty(CudaVersion))
                    return $"GPU  |  CUDA {CudaVersion}  |  Driver {DriverVersion}";
                return $"GPU  |  Driver {DriverVersion}";
            }
        }
    }

    public static class CudaDetector
    {
        // Windows 드라이버 버전 → 최대 지원 CUDA 버전 대응표
        // 내림차순 정렬 (첫 번째 매칭 사용)
        private static readonly (float Driver, string Cuda)[] DriverCudaTable =
        {
            (591.00f, "13.1"),
            (585.00f, "13.0"),
            (576.02f, "12.9"),
            (572.16f, "12.8"),
            (566.36f, "12.7"),
            (560.94f, "12.6"),
            (555.85f, "12.5"),
            (551.78f, "12.4"),
            (546.12f, "12.3"),
            (537.13f, "12.2"),
            (531.68f, "12.1"),
            (527.41f, "12.0"),
            (522.06f, "11.8"),
            (516.94f, "11.7"),
            (511.79f, "11.6"),
            (496.13f, "11.5"),
            (471.41f, "11.4"),
            (465.89f, "11.3"),
            (461.09f, "11.2"),
            (456.38f, "11.1"),
            (451.48f, "11.0"),
        };

        public static CudaInfo Detect()
        {
            string nvcudaPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "nvcuda.dll");

            if (!File.Exists(nvcudaPath))
                return new CudaInfo { HasGpu = false };

            string driverVersion = ParseDriverVersion(nvcudaPath);
            string cudaVersion = LookupCudaVersion(driverVersion);

            return new CudaInfo
            {
                HasGpu = true,
                DriverVersion = driverVersion,
                CudaVersion = cudaVersion
            };
        }

        // nvcuda.dll FileVersion "32.0.15.9186" → "591.86"
        // 3번째 + 4번째 필드 합산 끝 5자리 → "59186" → "591.86"
        private static string ParseDriverVersion(string dllPath)
        {
            try
            {
                var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                string[] parts = vi.FileVersion?.Split('.');
                if (parts == null || parts.Length < 4) return vi.FileVersion;

                string combined = parts[2] + parts[3]; // "15" + "9186" = "159186"
                if (combined.Length < 5) return vi.FileVersion;

                string last5 = combined.Substring(combined.Length - 5); // "59186"
                string major = last5.Substring(0, 3); // "591"
                string minor = last5.Substring(3);    // "86"
                return $"{int.Parse(major)}.{minor}";  // "591.86"
            }
            catch
            {
                return null;
            }
        }

        private static string LookupCudaVersion(string driverVersion)
        {
            if (string.IsNullOrEmpty(driverVersion)) return null;

            if (!float.TryParse(driverVersion,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out float driver))
                return null;

            foreach (var (minDriver, cuda) in DriverCudaTable)
            {
                if (driver >= minDriver)
                    return cuda;
            }

            return null;
        }
    }
}
