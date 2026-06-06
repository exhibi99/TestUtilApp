using System;
using System.Collections.Generic;
using System.IO;

namespace TestUtilApp.Services
{
    /// <summary>
    /// 파일 내보내기 서비스
    /// </summary>
    public static class FileExporter
    {
        public static void ExportClassificationResults(
            string filePath,
            Dictionary<string, List<string>> classificationResults)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Classification,FileName");
                foreach (var kvp in classificationResults)
                {
                    foreach (var fileName in kvp.Value)
                    {
                        writer.WriteLine($"{kvp.Key},{fileName}");
                    }
                }
            }
        }
    }
}