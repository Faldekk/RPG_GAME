using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class JournalModeHandler : IGameModeHandler
    {
        public bool Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.OpenJournal:
                    state.CurrentMode = GameMode.Normal;
                    world.AddMessage("Journal closed.");
                    return false;
                case InputCommand.Quit:
                    world.Stop();
                    return false;
                default:
                    world.AddMessage("Journal mode: press J to return.");
                    return false;
            }
        }
    }
}