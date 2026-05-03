using System;

namespace RPG_GAME.App.Logging
{
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; }
        public string Message { get; }

        public LogEntry(string message)
            : this(DateTime.Now, message)
        {
        }

        public LogEntry(DateTime timestamp, string message)
        {
            Timestamp = timestamp;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public override string ToString() => $"[{Timestamp:HH:mm:ss}] {Message}";
    }
}