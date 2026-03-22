using System.Threading;
using RPG_GAME.Model;
using RPG_GAME.UI;

namespace RPG_GAME.App
{
    public class Game
    {
        private readonly World _world;
        private readonly Renderer _renderer;
        private readonly Input _input;
        private bool _isRunning;

        public Game()
        {
            _world = new World();
            _renderer = new Renderer();
            _input = new Input();
        }

        public void Run()
        {
            _isRunning = true;
            _renderer.Render(_world);

            while (_isRunning)
            {
                var command = _input.ReadCommand();
                if (command == InputCommand.None)
                {
                    Thread.Sleep(10);
                    continue;
                }

                HandleCommand(command);

                if (!_isRunning)
                    break;

                _renderer.Render(_world);
            }
        }

        private void HandleCommand(InputCommand command)
        {
            switch (command)
            {
                case InputCommand.None:
                    break;
                case InputCommand.Unknown:
                    _world.AddMessage("Unknown command");
                    break;
                case InputCommand.Up:
                    _world.TryMovePlayer(0, -1);
                    break;
                case InputCommand.Down:
                    _world.TryMovePlayer(0, 1);
                    break;
                case InputCommand.Left:
                    _world.TryMovePlayer(-1, 0);
                    break;
                case InputCommand.Right:
                    _world.TryMovePlayer(1, 0);
                    break;
                case InputCommand.Pickup:
                    _world.TryPickUpItem();
                    break;
                case InputCommand.Drop:
                    if (!_world.TryDropItem(0))
                        _world.TryDropItem(1);
                    break;
                case InputCommand.BackpackAction:
                    _world.TryBackpackAction();
                    break;
                case InputCommand.SwapWeapons:
                    _world.Player.SwapWeapons();
                    break;
                case InputCommand.DropLeftHand:
                    _world.TryDropItem(0);
                    break;
                case InputCommand.DropRightHand:
                    _world.TryDropItem(1);
                    break;
                case InputCommand.Quit:
                    Stop();
                    break;
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}