using System.Collections.Generic;
using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class NormalModeHandler : IGameModeHandler
    {
        private readonly CommandHandler _commandPipeline;

        public NormalModeHandler(CommandHandler commandPipeline)
        {
            _commandPipeline = commandPipeline;
        }

        public void Handle(InputCommand command, World world, GameState state)
        {
            if (command == InputCommand.OpenInventory)
            {
                state.CurrentMode = GameMode.Inventory;
                state.SelectedInventoryIndex = 0;
                world.AddMessage("Inventory opened.");
                return;
            }

            if (command == InputCommand.OpenJournal)
            {
                state.CurrentMode = GameMode.Journal;
                world.AddMessage("Journal opened.");
                return;
            }

            if (command == InputCommand.Pickup && world.IsAtCraftingStation())
            {
                state.CurrentMode = GameMode.WeaponCrafting;
                state.CraftingFirstSelection = -1;
                world.AddMessage("Weapon crafting station. Select first weapon with W/S, combine with E.");
                return;
            }

            _commandPipeline.Handle(command, world, null);

            if (world.IsCombatActive)
                state.CurrentMode = GameMode.Combat;
        }
    }
}
