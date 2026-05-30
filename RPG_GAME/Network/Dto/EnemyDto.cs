namespace RPG_GAME.Network.Dto
{
    public sealed class EnemyDto
    {
        public string Name { get; set; } = string.Empty;
        public string SpeciesKey { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public int AttackMin { get; set; }
        public int AttackMax { get; set; }
        public int Armor { get; set; }
        public char MapCharacter { get; set; }
    }
}
