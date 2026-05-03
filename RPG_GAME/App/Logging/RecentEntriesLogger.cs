using System.Collections.Generic;
using System.Linq;

namespace RPG_GAME.App.Logging
{
    public sealed class RecentEntriesLogger : IGameLogger, ILogRecentEntriesSource
    {
        private readonly object _sync = new();
        private readonly Queue<LogEntry> _entries = new();

        public int MaxEntries { get; }

        public RecentEntriesLogger(int maxEntries = 6)
        {
            MaxEntries = maxEntries > 0 ? maxEntries : 6;
        }

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
                _entries.Enqueue(entry);
                while (_entries.Count > MaxEntries)
                    _entries.Dequeue();
            }
        }
    }
}