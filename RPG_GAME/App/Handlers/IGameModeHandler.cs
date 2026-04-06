using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public interface IGameModeHandler
    {
        void Handle(InputCommand command, World world, GameState state);
    }
}
