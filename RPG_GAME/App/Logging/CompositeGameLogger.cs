using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG_GAME.App.Logging
{
    public sealed class CompositeGameLogger : IGameLogger, ILogJournalSource, ILogRecentEntriesSource, ILogFileSource
    {
        private readonly object _sync = new();
        private readonly List<IGameLogger> _loggers = new();

        public CompositeGameLogger(params IGameLogger[] loggers)
        {
            if (loggers == null)
                return;

            foreach (var logger in loggers)
                Add(logger);
        }

        public IReadOnlyList<LogEntry> Entries
        {
            get
            {
                lock (_sync)
                {
                    var journal = _loggers.OfType<ILogJournalSource>().FirstOrDefault();
                    return journal?.Entries ?? Array.Empty<LogEntry>();
                }
            }
        }

        public string FilePath
        {
            get
            {
                lock (_sync)
                {
                    return _loggers.OfType<ILogFileSource>().Select(x => x.FilePath).FirstOrDefault() ?? string.Empty;
                }
            }
        }

        IReadOnlyList<LogEntry> ILogRecentEntriesSource.Entries
        {
            get
            {
                lock (_sync)
                {
                    var recent = _loggers.OfType<ILogRecentEntriesSource>().FirstOrDefault();
                    return recent?.Entries ?? Array.Empty<LogEntry>();
                }
            }
        }

        public void Add(IGameLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            lock (_sync)
            {
                _loggers.Add(logger);
            }
        }

        public void Log(LogEntry entry)
        {
            if (entry == null)
                return;

            IGameLogger[] snapshot;
            lock (_sync)
            {
                snapshot = _loggers.ToArray();
            }

            foreach (var logger in snapshot)
                logger.Log(entry);
        }
    }
}