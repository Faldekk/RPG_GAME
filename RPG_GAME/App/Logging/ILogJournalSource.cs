using System.Collections.Generic;

namespace RPG_GAME.App.Logging
{
    public interface ILogJournalSource
    {
        IReadOnlyList<LogEntry> Entries { get; }
    }
}