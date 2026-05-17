using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public interface IGameModeHandler
    {
        // Return true if the handled command consumed a player turn
        bool Handle(InputCommand command, World world, GameState state);
    }
}
