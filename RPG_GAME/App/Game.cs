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
        private readonly CommandHandler _commandPipeline;
        private bool _isRunning;

        public GameMode CurrentMode { get; private set; } = GameMode.Normal;
        public int SelectedInventoryIndex { get; private set; }

        public Game()
        {
            _world = new World();
            _renderer = new Renderer();
            _input = new Input();
            _commandPipeline = BuildCommandPipeline();
        }

        public void Run()
        {
            _isRunning = true;
            _renderer.Render(_world, CurrentMode, SelectedInventoryIndex);

            while (_isRunning)
            {
                var command = _input.ReadCommand(CurrentMode);
                if (command == InputCommand.None)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (CurrentMode == GameMode.Inventory)
                    HandleInventoryMode(command);
                else
                    HandleNormalMode(command);

                if (!_isRunning)
                    break;

                _renderer.Render(_world, CurrentMode, SelectedInventoryIndex);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private void HandleNormalMode(InputCommand command)
        {
            if (command == InputCommand.OpenInventory)
            {
                CurrentMode = GameMode.Inventory;
                ClampInventoryIndex();
                _world.AddMessage("Inventory opened.");
                return;
            }

            _commandPipeline.Handle(command, _world, this);
        }

        private void HandleInventoryMode(InputCommand command)
        {
            switch (command)
            {
                case InputCommand.InventoryUp:
                    MoveInventorySelection(-1);
                    break;
                case InputCommand.InventoryDown:
                    MoveInventorySelection(1);
                    break;
                case InputCommand.InventoryEquip:
                    if (_world.EquipFromBackpack(SelectedInventoryIndex))
                        ClampInventoryIndex();
                    break;
                case InputCommand.InventoryDrop:
                    if (_world.DropFromBackpack(SelectedInventoryIndex))
                        ClampInventoryIndex();
                    break;
                case InputCommand.InventoryUse:
                    if (_world.UseFromBackpack(SelectedInventoryIndex))
                        ClampInventoryIndex();
                    break;
                case InputCommand.CloseInventory:
                    CurrentMode = GameMode.Normal;
                    _world.AddMessage("Inventory closed.");
                    break;
                case InputCommand.Unknown:
                    _world.AddMessage("Unknown command");
                    break;
                default:
                    _world.AddMessage("Inventory mode: use W/S/E/D/U/ESC");
                    break;
            }
        }

        private void MoveInventorySelection(int delta)
        {
            int count = _world.Player.Inventory.Count();
            if (count == 0)
            {
                SelectedInventoryIndex = 0;
                _world.AddMessage("Backpack is empty.");
                return;
            }

            SelectedInventoryIndex += delta;
            if (SelectedInventoryIndex < 0)
                SelectedInventoryIndex = 0;
            if (SelectedInventoryIndex >= count)
                SelectedInventoryIndex = count - 1;
        }

        private void ClampInventoryIndex()
        {
            int count = _world.Player.Inventory.Count();
            if (count <= 0)
            {
                SelectedInventoryIndex = 0;
                return;
            }

            if (SelectedInventoryIndex < 0)
                SelectedInventoryIndex = 0;
            if (SelectedInventoryIndex >= count)
                SelectedInventoryIndex = count - 1;
        }

        private static CommandHandler BuildCommandPipeline()
        {
            var moveUp = new MoveUpHandler();
            var moveDown = new MoveDownHandler();
            var moveLeft = new MoveLeftHandler();
            var moveRight = new MoveRightHandler();
            var pickup = new PickupHandler();
            var backpackAction = new BackpackActionHandler();
            var swapWeapons = new SwapWeaponsHandler();
            var dropLeft = new DropLeftHandler();
            var dropRight = new DropRightHandler();
            var quit = new QuitHandler();
            var unknown = new UnknownCommandHandler();

            moveUp.SetNext(moveDown);
            moveDown.SetNext(moveLeft);
            moveLeft.SetNext(moveRight);
            moveRight.SetNext(pickup);
            pickup.SetNext(backpackAction);
            backpackAction.SetNext(swapWeapons);
            swapWeapons.SetNext(dropLeft);
            dropLeft.SetNext(dropRight);
            dropRight.SetNext(quit);
            quit.SetNext(unknown);

            return moveUp;
        }
    }
}