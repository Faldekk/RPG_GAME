using RPG_GAME.Model;

namespace RPG_GAME.View
{
    public interface IGameView
    {
        void Render(World world, RPG_GAME.App.GameMode mode, int selectedInventoryIndex, int craftingFirstSelection = -1, GameTimer? timer = null);
    }
}
