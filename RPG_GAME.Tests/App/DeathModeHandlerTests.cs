using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class DeathModeHandlerTests
    {
        private DeathModeHandler _handler;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            _handler = new DeathModeHandler();
            _state = new GameState { CurrentMode = GameMode.Death };
            _world = new MockWorld();
        }

        [Test]
        public void DeathModeHandler_OnDeathRespawn_TransitionsToNormalMode()
        {
            _state.CurrentMode = GameMode.Death;
            _state.SelectedInventoryIndex = 5;
            _state.CraftingFirstSelection = 3;
            
            _handler.Handle(InputCommand.DeathRespawn, _world, _state);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
                Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }

        [Test]
        public void DeathModeHandler_OnDeathRespawn_ShowsWelcomeMessage()
        {
            _handler.Handle(InputCommand.DeathRespawn, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Welcome back"));
        }

        [Test]
        public void DeathModeHandler_OnInvalidCommand_ShowsHelpMessage()
        {
            _handler.Handle(InputCommand.OpenInventory, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Death mode"));
        }

        [Test]
        public void DeathModeHandler_RespawnClearsAllState()
        {
            _state.CurrentMode = GameMode.Death;
            _state.SelectedInventoryIndex = 10;
            _state.CraftingFirstSelection = 5;
            
            _handler.Handle(InputCommand.DeathRespawn, _world, _state);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
                Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }
    }
}
