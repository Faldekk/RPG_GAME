using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class InventoryModeHandler : IGameModeHandler
    {
        private const string InventoryHelpMessage = "Inventory mode: use W/S/E/D/U/C/ESC";

        public bool Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.InventoryUp:
                    MoveInventorySelection(world, state, -1);
                    return false;

                case InputCommand.InventoryDown:
                    MoveInventorySelection(world, state, 1);
                    return false;

                case InputCommand.InventoryEquip:
                    if (world.EquipFromBackpack(state.SelectedInventoryIndex))
                        ClampInventoryIndex(world, state);
                    return true;

                case InputCommand.InventoryDrop:
                    if (world.DropFromBackpack(state.SelectedInventoryIndex))
                        ClampInventoryIndex(world, state);
                    return true;

                case InputCommand.InventoryUse:
                    if (world.UseFromBackpack(state.SelectedInventoryIndex))
                        ClampInventoryIndex(world, state);
                    return true;

                case InputCommand.InventoryCraftArmor:
                    world.CraftArmorFromJunk();
                    ClampInventoryIndex(world, state);
                    return true;

                case InputCommand.CloseInventory:
                    state.CurrentMode = GameMode.Normal;
                    world.AddMessage("Inventory closed.");
                    return false;

                case InputCommand.Unknown:
                    world.AddMessage("Unknown command");
                    return false;

                default:
                    world.AddMessage(InventoryHelpMessage);
                    return false;
            }
        }

        private void MoveInventorySelection(World world, GameState state, int delta)
        {
            int count = world.Player.Inventory.Count();
            if (count == 0)
            {
                state.SelectedInventoryIndex = 0;
                world.AddMessage("Backpack is empty.");
                return;
            }

            state.SelectedInventoryIndex += delta;
            state.SelectedInventoryIndex = System.Math.Max(0, System.Math.Min(state.SelectedInventoryIndex, count - 1));
        }

        private void ClampInventoryIndex(World world, GameState state)
        {
            int count = world.Player.Inventory.Count();
            state.SelectedInventoryIndex = count <= 0 ? 0 : System.Math.Max(0, System.Math.Min(state.SelectedInventoryIndex, count - 1));
        }
    }
}
