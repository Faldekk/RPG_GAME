namespace RPG_GAME.App
{
    public enum GameModeKind
    {
        Normal,
        Inventory,
        Combat,
        Death,
        WeaponCrafting
    }

    public sealed class GameMode
    {
        public static readonly GameMode Normal = new(GameModeKind.Normal, "Normal");
        public static readonly GameMode Inventory = new(GameModeKind.Inventory, "Inventory");
        public static readonly GameMode Combat = new(GameModeKind.Combat, "Combat");
        public static readonly GameMode Death = new(GameModeKind.Death, "Death");
        public static readonly GameMode WeaponCrafting = new(GameModeKind.WeaponCrafting, "WeaponCrafting");

        public GameModeKind Kind { get; }
        public string Name { get; }

        private GameMode(GameModeKind kind, string name)
        {
            Kind = kind;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
