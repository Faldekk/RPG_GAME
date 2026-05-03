namespace RPG_GAME.App.Logging
{
    public sealed class JournalViewLogger : IGameLogger
    {
        private readonly InMemoryGameLogger _journal;

        public JournalViewLogger(InMemoryGameLogger journal)
        {
            _journal = journal;
        }

        public void Log(LogEntry entry)
        {
            _journal.Log(entry);
        }
    }
}