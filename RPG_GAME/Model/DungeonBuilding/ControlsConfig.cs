namespace RPG_GAME.Model.DungeonBuilding
{
    public static class ControlsConfig
    {
        // === MOVEMENT ===
        
        public static readonly BuildInstruction Movement = new("WASD", "move");
        public static readonly BuildInstruction Quit = new("Q", "quit");

        // === PICKUP & INVENTORY ===
        
        public static readonly BuildInstruction PickUp = new("E", "pick up");
        public static readonly BuildInstruction OpenInventory = new("B", "inventory");
        public static readonly BuildInstruction CloseInventory = new("ESC", "close inventory");

        // === EQUIPMENT ===
        public static readonly BuildInstruction SwapWeapons = new("X", "swap weapons");
        public static readonly BuildInstruction DropLeft = new("1", "drop left");
        public static readonly BuildInstruction DropRight = new("2", "drop right");

        // === INVENTORY ACTIONS ===

        public static readonly BuildInstruction InventoryUp = new("W", "up");
        
        public static readonly BuildInstruction InventoryDown = new("S", "down");

        public static readonly BuildInstruction InventoryEquip = new("E", "equip");
        
        public static readonly BuildInstruction InventoryDrop = new("D", "drop");
        public static readonly BuildInstruction InventoryUse = new("U", "use");
    }
}
