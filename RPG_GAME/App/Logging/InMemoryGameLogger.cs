using System.Collections.Generic;
using System.Linq;

namespace RPG_GAME.App.Logging
{
    public sealed class InMemoryGameLogger : IGameLogger, ILogJournalSource
    {
        private readonly object _sync = new();
        private readonly List<LogEntry> _entries = new();

        public IReadOnlyList<LogEntry> Entries
        {
            get
            {
                lock (_sync)
                {
                    return _entries.ToArray();
                }
            }
        }

        public void Log(LogEntry entry)
        {
            if (entry == null)
                return;

            lock (_sync)
            {
                _entries.Add(entry);
            }
        }
    }
}