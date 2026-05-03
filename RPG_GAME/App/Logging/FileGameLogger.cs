using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace RPG_GAME.App.Logging
{
    public sealed class FileGameLogger : IGameLogger, ILogFileSource
    {
        private readonly object _sync = new();
        private readonly string _filePath;
        private readonly StreamWriter _writer;

        public string FilePath => _filePath;

        public FileGameLogger(string directoryPath, string playerName)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path must be provided.", nameof(directoryPath));

            playerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
            Directory.CreateDirectory(directoryPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            var safePlayerName = SanitizeFileName(playerName);
            _filePath = Path.Combine(directoryPath, $"{safePlayerName}_{timestamp}.log");

            var stream = new FileStream(_filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }

        public void Log(LogEntry entry)
        {
            if (entry == null)
                return;

            lock (_sync)
            {
                _writer.WriteLine(entry.ToString());
            }
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