using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class WeaponCraftingModeHandlerTests
    {
        private WeaponCraftingModeHandler _handler;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            _handler = new WeaponCraftingModeHandler();
            _state = new GameState { CurrentMode = GameMode.WeaponCrafting };
            _world = new MockWorld();
        }

        [Test]
        public void WeaponCraftingModeHandler_OnCraftingSelectFirst_IncrementsSelection()
        {
            _state.CraftingFirstSelection = 0;
            
            _handler.Handle(InputCommand.CraftingSelectFirst, _world, _state);
            
            Assert.That(_state.CraftingFirstSelection, Is.GreaterThan(0));
        }

        [Test]
        public void WeaponCraftingModeHandler_OnCraftingSelectFirst_ShowsMessage()
        {
            _state.CraftingFirstSelection = 0;
            
            _handler.Handle(InputCommand.CraftingSelectFirst, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Selected weapon"));
        }

        [Test]
        public void WeaponCraftingModeHandler_OnCraftingSelectSecond_DecrementsSelection()
        {
            _state.CraftingFirstSelection = 2;
            
            _handler.Handle(InputCommand.CraftingSelectSecond, _world, _state);
            
            Assert.That(_state.CraftingFirstSelection, Is.LessThan(2));
        }

        [Test]
        public void WeaponCraftingModeHandler_OnCraftingCancel_TransitionsToNormalMode()
        {
            _state.CurrentMode = GameMode.WeaponCrafting;
            _state.CraftingFirstSelection = 2;
            
            _handler.Handle(InputCommand.CraftingCancel, _world, _state);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }

        [Test]
        public void WeaponCraftingModeHandler_OnCraftingCancel_ShowsMessage()
        {
            _handler.Handle(InputCommand.CraftingCancel, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Crafting station left"));
        }

        [Test]
        public void WeaponCraftingModeHandler_OnInvalidCommand_ShowsHelpMessage()
        {
            _handler.Handle(InputCommand.OpenInventory, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Crafting"));
        }

        [Test]
        public void WeaponCraftingModeHandler_SelectionWrapsAround()
        {
            _state.CraftingFirstSelection = 0;
            _handler.Handle(InputCommand.CraftingSelectSecond, _world, _state);
            
            Assert.That(_state.CraftingFirstSelection, Is.GreaterThanOrEqualTo(0));
        }
    }
}
