using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RPG_GAME.App.Logging
{
    public sealed class FileGameLogger : IGameLogger
    {
        private readonly object _sync = new();
        private readonly string _filePath;

        public string FilePath => _filePath;

        public FileGameLogger(string directoryPath, string playerName)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path must be provided.", nameof(directoryPath));

            playerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
            Directory.CreateDirectory(directoryPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            var safePlayerName = SanitizeFileName(playerName);
            var baseName = $"{safePlayerName}_{timestamp}";
            _filePath = CreateUniquePath(directoryPath, baseName, ".log");

            using var stream = new FileStream(_filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        }

        public void Log(LogEntry entry)
        {
            if (entry == null)
                return;

            var line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss} {entry.Message}{Environment.NewLine}";

            lock (_sync)
            {
                File.AppendAllText(_filePath, line, Encoding.UTF8);
            }
        }

        private static string CreateUniquePath(string directoryPath, string baseName, string extension)
        {
            var candidate = Path.Combine(directoryPath, baseName + extension);
            int suffix = 1;

            while (File.Exists(candidate))
            {
                candidate = Path.Combine(directoryPath, $"{baseName}_{suffix++}{extension}");
            }

            return candidate;
        }

        private static string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);

            foreach (var ch in name)
            {
                builder.Append(Array.IndexOf(invalidChars, ch) >= 0 ? '_' : ch);
            }

            return builder.ToString();
        }
    }
}