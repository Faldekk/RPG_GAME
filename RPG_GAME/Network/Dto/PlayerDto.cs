namespace RPG_GAME.Network.Dto
{
    public sealed class PlayerDto
    {
        public int PlayerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }
        public int Armor { get; set; }
        public int Coins { get; set; }
        public int Gold { get; set; }
        public string LeftHand { get; set; } = string.Empty;
        public string RightHand { get; set; } = string.Empty;
    }
}
