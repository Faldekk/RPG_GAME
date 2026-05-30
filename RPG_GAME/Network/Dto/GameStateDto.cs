using System.Collections.Generic;

namespace RPG_GAME.Network.Dto
{
    public sealed class GameStateDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string[] MapRows { get; set; } = [];
        public List<PlayerDto> Players { get; set; } = [];
        public List<EnemyDto> Enemies { get; set; } = [];
        public List<ItemDto> Items { get; set; } = [];
        public List<TileDto> Tiles { get; set; } = [];
        public List<LogEntryDto> RecentEntries { get; set; } = [];
        public List<LogEntryDto> JournalEntries { get; set; } = [];
        public string CurrentMessage { get; set; } = string.Empty;
        public string CurrentMode { get; set; } = string.Empty;
        public int ActivePlayerId { get; set; }
        public bool IsExitRequested { get; set; }
        public int ElapsedSeconds { get; set; }
        public bool IsGameWon { get; set; }
    }
}
