using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class InventoryModeHandlerTests
    {
        private InventoryModeHandler _handler;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            _handler = new InventoryModeHandler();
            _state = new GameState();
            _world = new MockWorld();
            _state.CurrentMode = GameMode.Inventory;
        }

        [Test]
        public void InventoryModeHandler_OnInventoryUp_WithValidIndex_DecrementsIndex()
        {
            _state.SelectedInventoryIndex = 2;
            
            _handler.Handle(InputCommand.InventoryUp, _world, _state);
            
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(1));
        }

        [Test]
        public void InventoryModeHandler_OnInventoryUp_AtMinIndex_StaysAtMin()
        {
            _state.SelectedInventoryIndex = 0;
            
            _handler.Handle(InputCommand.InventoryUp, _world, _state);
            
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
        }

        [Test]
        public void InventoryModeHandler_OnInventoryDown_WithValidIndex_IncrementsIndex()
        {
            _state.SelectedInventoryIndex = 1;
            
            _handler.Handle(InputCommand.InventoryDown, _world, _state);
            
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(2));
        }

        [Test]
        public void InventoryModeHandler_OnInventoryDown_AtMaxIndex_StaysAtMax()
        {
            _state.SelectedInventoryIndex = _world.InventoryCount - 1;
            int maxIndex = _state.SelectedInventoryIndex;
            
            _handler.Handle(InputCommand.InventoryDown, _world, _state);
            
            Assert.That(_state.SelectedInventoryIndex, Is.LessThanOrEqualTo(maxIndex));
        }

        [Test]
        public void InventoryModeHandler_OnCloseInventory_TransitionsToNormalMode()
        {
            _state.CurrentMode = GameMode.Inventory;
            
            _handler.Handle(InputCommand.CloseInventory, _world, _state);
            
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void InventoryModeHandler_OnCloseInventory_ShowsMessage()
        {
            _handler.Handle(InputCommand.CloseInventory, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Inventory closed"));
        }

        [Test]
        public void InventoryModeHandler_OnUnknownCommand_ShowsErrorMessage()
        {
            _handler.Handle(InputCommand.Unknown, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Unknown command"));
        }

        [Test]
        public void InventoryModeHandler_OnInvalidCommand_ShowsHelpMessage()
        {
            _handler.Handle(InputCommand.CombatNormalAttack, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Inventory mode"));
        }
    }
}
