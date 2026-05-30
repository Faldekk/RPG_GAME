namespace RPG_GAME.Network.Dto
{
    public sealed class PlayerCommandDto
    {
        public int PlayerId { get; set; }
        public PlayerCommandKind Command { get; set; }
        public int Value { get; set; }
        public string? Text { get; set; }
    }
}
