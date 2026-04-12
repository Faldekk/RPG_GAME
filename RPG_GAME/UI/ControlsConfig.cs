using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.UI
{
    public static class ControlsConfig
    {
        
        public static readonly BuildInstruction Movement = new("WASD", "move");
        public static readonly BuildInstruction Quit = new("Q", "quit");
        
        public static readonly BuildInstruction PickUp = new("E", "pick up");
        public static readonly BuildInstruction OpenInventory = new("B", "inventory");
        public static readonly BuildInstruction CloseInventory = new("ESC", "close inventory");
        public static readonly BuildInstruction SwapWeapons = new("X", "swap weapons");
        public static readonly BuildInstruction DropLeft = new("1", "drop left");
        public static readonly BuildInstruction DropRight = new("2", "drop right");

        public static readonly BuildInstruction InventoryUp = new("W", "up");
        
        public static readonly BuildInstruction InventoryDown = new("S", "down");

        public static readonly BuildInstruction InventoryEquip = new("E", "equip");
        
        public static readonly BuildInstruction InventoryDrop = new("D", "drop");
        public static readonly BuildInstruction InventoryUse = new("U", "use");
        public static readonly BuildInstruction InventoryCraftArmor = new("C", "craft armor");
        
        public static readonly BuildInstruction CombatNormal = new("1", "normal attack");
        public static readonly BuildInstruction CombatStealth = new("2", "stealth attack");
        public static readonly BuildInstruction CombatMagical = new("3", "magical attack");
    }
}
