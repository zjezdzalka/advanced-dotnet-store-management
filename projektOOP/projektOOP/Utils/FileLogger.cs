using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using projektOOP.Interfaces;

namespace projektOOP.Utils
{
    /// <summary>
    /// Provides functionality to log messages to a file and retrieve logged messages.
    /// Implements the <see cref="ILogger"/> interface.
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Logs a message to a file named "logs.txt".
        /// Each log entry is timestamped with the current date and time.
        /// </summary>
        /// <param name="message">The message to log.</param>
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

        /// <summary>
        /// Retrieves all log entries from the "logs.txt" file in descending order of their timestamps.
        /// If the file does not exist, returns an empty collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of log entries, ordered by timestamp in descending order.
        /// </returns>
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
