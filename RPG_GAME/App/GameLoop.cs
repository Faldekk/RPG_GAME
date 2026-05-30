using System.Threading;
using RPG_GAME.Controller;
using RPG_GAME.Model;
using RPG_GAME.View;

namespace RPG_GAME.App
{
    public class GameLoop
    {
        private const int InputPollDelayMs = 10;
        private bool _isRunning;

        private readonly World _world;
        private readonly IGameView _view;
        private readonly IInputController _input;
        private readonly GameModeDispatcher _dispatcher;
        private readonly GameState _state;

        public GameLoop(World world, IGameView view, IInputController input, GameModeDispatcher dispatcher, GameState state)
        {
            _world = world;
            _view = view;
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
            if (command == RPG_GAME.App.InputCommand.None)
            {
                Thread.Sleep(InputPollDelayMs);
                return;
            }

            var consumedTurn = _dispatcher.HandleCommand(command, _world, _state);

            if (consumedTurn && _state.CurrentMode == RPG_GAME.App.GameMode.Normal)
            {
                _world.ProcessEnemiesTurn();
            }

            // Check win condition
            if (_world.HasAllEnemiesBeenDefeated())
            {
                _state.CurrentMode = RPG_GAME.App.GameMode.Won;
                if (_state.Timer != null && !_state.Timer.IsFrozen)
                    _state.Timer.Freeze();
            }

            if (_world.IsExitRequested)
                Stop();
        }

        private void Render()
        {
            _view.Render(_world, _state.CurrentMode, _state.SelectedInventoryIndex, _state.CraftingFirstSelection, _state.Timer);
        }
    }
}
