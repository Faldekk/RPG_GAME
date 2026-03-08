using System;
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
        private CommandHandler _commandChain;

        public Game()
        {
            _world = new World();
            _renderer = new Renderer();
            _input = new Input();
            BuildCommandChain();
        }

        private void BuildCommandChain()
        {
            var up = new MoveUpHandler();
            var down = new MoveDownHandler();
            var left = new MoveLeftHandler();
            var right = new MoveRightHandler();
            var pickup = new PickupHandler();
            var drop = new DropHandler();
            var swap = new SwapWeaponsHandler();
            var dropL = new DropLeftHandler();
            var dropR = new DropRightHandler();
            var quit = new QuitHandler();

            up.SetNext(down);
            down.SetNext(left);
            left.SetNext(right);
            right.SetNext(pickup);
            pickup.SetNext(drop);
            drop.SetNext(swap);
            swap.SetNext(dropL);
            dropL.SetNext(dropR);
            dropR.SetNext(quit);

            _commandChain = up;
        }

        public void Run()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            _isRunning = true;

            while (_isRunning)
            {
                _renderer.Render(_world);

                var cmd = _input.ReadCommand();
                HandleCommand(cmd);
            }

            Console.Clear();
            Console.WriteLine("Thanks for playing!");
        }

        private void HandleCommand(InputCommand cmd)
        {
            _commandChain.Handle(cmd, _world, this);
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}