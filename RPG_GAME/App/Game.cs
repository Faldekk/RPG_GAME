using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            while (_isRunning)
            {
                _renderer.Render(_world);

                var cmd = _input.ReadCommand();
                HandleCommand(cmd);
            }
        }

        private void HandleCommand(InputCommand cmd)
        {
            switch (cmd)
            {
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
                case InputCommand.Quit:
                    _isRunning = false;
                    break;
                
            }
        }
    }
}