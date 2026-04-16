using System;
using System.Collections.Generic;

namespace RPG_GAME.App.Logging
{
    public static class GameLog
    {
        private static readonly object _sync = new();
        private static IGameLogger _logger = new CompositeGameLogger();

        public static void Configure(IGameLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            lock (_sync)
            {
                _logger = logger;
            }
        }

        public static void Info(string message)
        {
            Log(new LogEntry(DateTime.Now, message));
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

        public static IReadOnlyList<LogEntry> RecentEntries => FindLogger<RecentEntriesLogger>()?.Entries ?? Array.Empty<LogEntry>();

        public static IReadOnlyList<LogEntry> JournalEntries => FindLogger<InMemoryGameLogger>()?.Entries ?? Array.Empty<LogEntry>();

        public static string? FilePath => FindLogger<FileGameLogger>()?.FilePath;

        private static T? FindLogger<T>() where T : class
        {
            lock (_sync)
            {
                return FindLogger<T>(_logger);
            }
        }

        private static T? FindLogger<T>(IGameLogger logger) where T : class
        {
            if (logger is T typed)
                return typed;

            if (logger is CompositeGameLogger composite)
            {
                foreach (var child in composite.Loggers)
                {
                    var found = FindLogger<T>(child);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }
    }
}