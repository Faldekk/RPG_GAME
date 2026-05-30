namespace RPG_GAME.Network.Dto
{
    public sealed class ItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public int Durability { get; set; }
        public char MapCharacter { get; set; }
        public bool CanEquip { get; set; }
        public bool IsTwoHanded { get; set; }
        public bool IsHeal { get; set; }
    }
}
