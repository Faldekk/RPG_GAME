using System;

namespace RPG_GAME.Model.DungeonThemes
{
    public static class DungeonThemeFactory
    {
        public static IDungeonTheme CreateRandom()
        {
            return Random.Shared.Next(3) switch
            {
                0 => new ArcaneTheme(),
                1 => new IndustrialTheme(),
                _ => new TreasuryTheme(),
            };
        }
    }
}