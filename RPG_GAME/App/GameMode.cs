namespace RPG_GAME.App
{
    public sealed class GameMode
    {
        public static readonly GameMode Normal = new("Normal");
        public static readonly GameMode Inventory = new("Inventory");
        public static readonly GameMode Combat = new("Combat");
        public static readonly GameMode Death = new("Death");
        public static readonly GameMode WeaponCrafting = new("WeaponCrafting");
        public string Name { get; }

        private GameMode(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
