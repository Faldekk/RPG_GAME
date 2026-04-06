using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    public class MockWorld : World
    {
        public int AddMessageCallCount { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public int InventoryCount { get; set; } = 5;
        public bool IsAtCraftingStationReturnValue { get; set; } = false;

        public MockWorld() : base()
        {
        }

        public override void AddMessage(string message)
        {
            LastMessage = message;
            AddMessageCallCount++;
        }

        public new bool IsAtCraftingStation()
        {
            return IsAtCraftingStationReturnValue;
        }
    }
    [TestFixture]
    public class NormalModeHandlerTests
    {
        private NormalModeHandler _handler;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            var moveUp = new MoveUpHandler();
            var unknown = new UnknownCommandHandler();
            moveUp.SetNext(unknown);
            
            _handler = new NormalModeHandler(moveUp);
            _state = new GameState();
            _world = new MockWorld();
        }

        [Test]
        public void NormalModeHandler_OnOpenInventory_TransitionsToInventoryMode()
        {
            _state.CurrentMode = GameMode.Normal;
            _state.SelectedInventoryIndex = 5;
            
            _handler.Handle(InputCommand.OpenInventory, _world, _state);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
                Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
            });
        }

        [Test]
        public void NormalModeHandler_OnOpenInventory_ShowsMessage()
        {
            _handler.Handle(InputCommand.OpenInventory, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Inventory opened"));
        }

        [Test]
        public void NormalModeHandler_OnPickupAtCraftingStation_TransitionsToCraftingMode()
        {
            _world.IsAtCraftingStationReturnValue = true;
            _state.CurrentMode = GameMode.Normal;
            _state.CraftingFirstSelection = 5;
            
            _handler.Handle(InputCommand.Pickup, _world, _state);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.WeaponCrafting));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }

        [Test]
        public void NormalModeHandler_OnPickupAtCraftingStation_ShowsMessage()
        {
            _world.IsAtCraftingStationReturnValue = true;
            
            _handler.Handle(InputCommand.Pickup, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Weapon crafting station"));
        }

        [Test]
        public void NormalModeHandler_OnPickupNotAtStation_DoesNotTransition()
        {
            _world.IsAtCraftingStationReturnValue = false;
            _state.CurrentMode = GameMode.Normal;
            
            _handler.Handle(InputCommand.Pickup, _world, _state);
            
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }
    }
}
