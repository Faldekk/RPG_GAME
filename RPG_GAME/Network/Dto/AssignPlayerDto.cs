namespace RPG_GAME.Network.Dto
{
    public sealed class AssignPlayerDto
    {
        public int PlayerId { get; set; }
        public char Symbol { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}