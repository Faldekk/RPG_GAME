using RPG_GAME.Model;
using RPG_GAME.UI;
using System.Threading;

namespace RPG_GAME.App
{
   
    public class GameLoop
    {
        private const int InputPollDelayMs = 10;
        private bool _isRunning;

        private readonly World _world;
        private readonly Renderer _renderer;
        private readonly Input _input;
        private readonly GameModeDispatcher _dispatcher;
        private readonly GameState _state;

        public GameLoop(World world, Renderer renderer, Input input, GameModeDispatcher dispatcher, GameState state)
        {
            _world = world;
            _renderer = renderer;
            _input = input;
            _dispatcher = dispatcher;
            _state = state;
        }

        public void Run()
        {
            _isRunning = true;
            Render();

            while (_isRunning)
            {
                ProcessInput();
                Render();
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private void ProcessInput()
        {
            var command = _input.ReadCommand(_state.CurrentMode);
            if (command == InputCommand.None)
            {
                Thread.Sleep(InputPollDelayMs);
                return;
            }

            _dispatcher.HandleCommand(command, _world, _state);

            if (_world.IsExitRequested)
                Stop();
        }

        private void Render()
        {
            _renderer.Render(_world, _state.CurrentMode, _state.SelectedInventoryIndex, _state.CraftingFirstSelection);
        }
    }
}
