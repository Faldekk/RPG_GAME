using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public class JournalModeHandler : IGameModeHandler
    {
        public void Handle(InputCommand command, World world, GameState state)
        {
            switch (command)
            {
                case InputCommand.OpenJournal:
                    state.CurrentMode = GameMode.Normal;
                    world.AddMessage("Journal closed.");
                    break;
                case InputCommand.Quit:
                    world.Stop();
                    break;
                default:
                    world.AddMessage("Journal mode: press J to return.");
                    break;
            }
        }
    }
}