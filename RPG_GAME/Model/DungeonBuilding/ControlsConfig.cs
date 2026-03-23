namespace RPG_GAME.Model.DungeonBuilding
{
    // Jedno miejsce gdzie żyją wszystkie instrukcje - DRY principle
    // Zmień tutaj jeden raz i wszędzie się zaktualizuje
    public static class ControlsConfig
    {
        // === MOVEMENT ===
        // Poruszanie się - podstawa gry
        public static readonly BuildInstruction Movement = new("WASD", "move");
        // Wyjście z gry - do czego służy powinna wiedzieć każda babcia
        public static readonly BuildInstruction Quit = new("Q", "quit");

        // === PICKUP & INVENTORY ===
        // Podniesienie przedmiotu
        public static readonly BuildInstruction PickUp = new("E", "pick up");
        // Otwórz plecak
        public static readonly BuildInstruction OpenInventory = new("B", "inventory");
        // Zamknij plecak
        public static readonly BuildInstruction CloseInventory = new("ESC", "close inventory");

        // === EQUIPMENT ===
        // Zamieniaj bronie między rękami
        public static readonly BuildInstruction SwapWeapons = new("X", "swap weapons");
        // Wyrzuć co masz w lewej ręce
        public static readonly BuildInstruction DropLeft = new("1", "drop left");
        // Wyrzuć co masz w prawej ręce
        public static readonly BuildInstruction DropRight = new("2", "drop right");

        // === INVENTORY ACTIONS ===
        // Przejdź do góry w plecaku
        public static readonly BuildInstruction InventoryUp = new("W", "up");
        // Przejdź na dół w plecaku
        public static readonly BuildInstruction InventoryDown = new("S", "down");
        // Załóż przedmiot
        public static readonly BuildInstruction InventoryEquip = new("E", "equip");
        // Wyrzuć z plecaka
        public static readonly BuildInstruction InventoryDrop = new("D", "drop");
        // Użyj przedmiotu (zdrowucha, combo, itp)
        public static readonly BuildInstruction InventoryUse = new("U", "use");
    }
}
