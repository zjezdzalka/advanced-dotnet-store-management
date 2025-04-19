using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using projektOOP.Interfaces;

namespace projektOOP.Utils
{
    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText("logs.txt", logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging: {ex.Message}");
            }
        }

        public IEnumerable<string> GetLogs()
        {
            if (!File.Exists("logs.txt")) return Enumerable.Empty<string>();
            return File.ReadLines("logs.txt").OrderByDescending(line =>
            {
                var timestamp = line.Substring(1, 19);
                return DateTime.TryParse(timestamp, out var dt) ? dt : DateTime.MinValue;
            });
        }
    }
}
