namespace RPG_GAME.App.Logging
{
    public sealed class NullGameLogger : IGameLogger
    {
        public void Log(LogEntry entry)
        {
        }
    }
}