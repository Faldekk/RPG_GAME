using System;

namespace RPG_GAME.Network.Dto
{
    public sealed class LogEntryDto
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
