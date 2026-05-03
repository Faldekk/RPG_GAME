using System;

namespace RPG_GAME.Model.DungeonThemes
{
    public static class DungeonThemeFactory
    {
        private static readonly Func<IDungeonTheme>[] ThemeFactories =
        {
            () => new ArcaneTheme(),
            () => new IndustrialTheme(),
            () => new TreasuryTheme()
        };

        public static IDungeonTheme CreateRandom()
        {
            return ThemeFactories[Random.Shared.Next(ThemeFactories.Length)]();
        }
    }
}