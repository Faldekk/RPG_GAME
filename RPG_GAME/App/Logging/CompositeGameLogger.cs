using System;
using System.Collections.Generic;

namespace RPG_GAME.App.Logging
{
    public sealed class CompositeGameLogger : IGameLogger
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