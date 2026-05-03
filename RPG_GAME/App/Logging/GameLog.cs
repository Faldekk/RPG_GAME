using System;
using System.Collections.Generic;

namespace RPG_GAME.App.Logging
{
    public static class GameLog
    {
        private static readonly object _sync = new();
        private static IGameLogger _logger = new NullGameLogger();
        private static ILogJournalSource? _journalSource;
        private static ILogRecentEntriesSource? _recentSource;
        private static ILogFileSource? _fileSource;

        public static void Configure(IGameLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            lock (_sync)
            {
                _logger = logger;
                _journalSource = null;
                _recentSource = null;
                _fileSource = null;
            }
        }

        public static void Configure(IGameLogger logger, ILogJournalSource journalSource, ILogRecentEntriesSource recentSource, ILogFileSource fileSource)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (journalSource == null)
                throw new ArgumentNullException(nameof(journalSource));
            if (recentSource == null)
                throw new ArgumentNullException(nameof(recentSource));
            if (fileSource == null)
                throw new ArgumentNullException(nameof(fileSource));

            lock (_sync)
            {
                _logger = logger;
                _journalSource = journalSource;
                _recentSource = recentSource;
                _fileSource = fileSource;
            }
        }

        public static void Info(string message)
        {
            Log(new LogEntry(message));
        }

        public static void Log(LogEntry entry)
        {
            if (entry == null)
                return;

            lock (_sync)
            {
                _logger.Log(entry);
            }
        }

        public static IReadOnlyList<LogEntry> JournalEntries => _journalSource?.Entries ?? Array.Empty<LogEntry>();
        public static IReadOnlyList<LogEntry> RecentEntries => _recentSource?.Entries ?? Array.Empty<LogEntry>();
        public static string? FilePath => _fileSource?.FilePath;
    }
}