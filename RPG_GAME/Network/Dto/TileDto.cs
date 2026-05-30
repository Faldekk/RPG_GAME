namespace RPG_GAME.Network.Dto
{
    public sealed class TileDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsWall { get; set; }
        public bool IsCraftingStation { get; set; }
        public char Symbol { get; set; }
    }
}
