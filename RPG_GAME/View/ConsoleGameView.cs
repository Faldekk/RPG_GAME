using RPG_GAME.Model;
using RPG_GAME.UI;

namespace RPG_GAME.View
{
    public sealed class ConsoleGameView : IGameView
    {
        private readonly Renderer _renderer;

        public ConsoleGameView(Renderer renderer)
        {
            _renderer = renderer;
        }

        public void Render(World world, RPG_GAME.App.GameMode mode, int selectedInventoryIndex, int craftingFirstSelection = -1, GameTimer? timer = null)
        {
            _renderer.Render(world, mode, selectedInventoryIndex, craftingFirstSelection, timer);
        }
    }
}
