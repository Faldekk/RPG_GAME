using RPG_GAME.Model;
using System;
using System.Collections.Generic;

namespace RPG_GAME.App
{
    public class GameModeDispatcher
    {
        private readonly Dictionary<GameMode, IGameModeHandler> _handlers;

        public GameModeDispatcher(CommandHandler commandPipeline)
        {
            _handlers = new Dictionary<GameMode, IGameModeHandler>
            {
                { GameMode.Normal, new NormalModeHandler(commandPipeline) },
                { GameMode.Inventory, new InventoryModeHandler() },
                { GameMode.Combat, new CombatModeHandler() },
                { GameMode.Death, new DeathModeHandler() },
                { GameMode.WeaponCrafting, new WeaponCraftingModeHandler() },
                { GameMode.Journal, new JournalModeHandler() }
            };
        }

        public void HandleCommand(InputCommand command, World world, GameState state)
        {
            if (_handlers.TryGetValue(state.CurrentMode, out var handler))
            {
                handler.Handle(command, world, state);
            }
            else
            {
                throw new InvalidOperationException($"No handler registered for mode: {state.CurrentMode.Name}");
            }
        }
    }
}
