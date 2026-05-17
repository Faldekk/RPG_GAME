using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class DeathModeHandler : IGameModeHandler
    {
        private const string DeathHelpMessage = "Death mode: press [R] to respawn or [Q] to quit.";

        public bool Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.DeathRespawn:
                    HandleRespawn(world, state);
                    return true;

                case InputCommand.Quit:
                    world.Stop();
                    return false;

                default:
                    world.AddMessage(DeathHelpMessage);
                    return false;
            }
        }

        private void HandleRespawn(World world, GameState state)
        {
            world.Respawn();
            state.CurrentMode = GameMode.Normal;
            state.SelectedInventoryIndex = 0;
            state.CraftingFirstSelection = -1;
            world.AddMessage("Welcome back, adventurer!");
        }
    }
}
