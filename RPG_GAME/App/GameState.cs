namespace RPG_GAME.App
{
    public class GameState
    {
        public GameMode CurrentMode { get; set; } = GameMode.Normal;
        public int SelectedInventoryIndex { get; set; }
        public int CraftingFirstSelection { get; set; } = -1;

        public void ResetForMode(GameMode newMode)
        {
            CurrentMode = newMode;
            if (newMode == GameMode.WeaponCrafting)
                CraftingFirstSelection = -1;
            if (newMode == GameMode.Normal || newMode == GameMode.Inventory)
                SelectedInventoryIndex = 0;
        }
    }
}
