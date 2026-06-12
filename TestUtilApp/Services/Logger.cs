using System;
using System.IO;

namespace TestUtilApp.Services
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        private static string LogDirectory =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        private static string CurrentLogFile =>
            Path.Combine(LogDirectory, $"{DateTime.Now:yyyyMMdd}_TestUtilApp_LOG_.txt");

        public static void Info(string message)  => Write("INFO ", message);
        public static void Warn(string message)  => Write("WARN ", message);

        public static void Error(string message, Exception ex = null)
        {
            string detail = ex == null ? message : $"{message} | {ex.GetType().Name}: {ex.Message}";
            Write("ERROR", detail);
        }

        private static void Write(string level, string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
            Console.WriteLine(line);

            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(CurrentLogFile, line + Environment.NewLine);
                }
            }
            catch { }
        }
    }
}
