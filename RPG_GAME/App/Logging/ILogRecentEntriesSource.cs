using System.Collections.Generic;

namespace RPG_GAME.App.Logging
{
    public interface ILogRecentEntriesSource
    {
        IReadOnlyList<LogEntry> Entries { get; }
    }
}