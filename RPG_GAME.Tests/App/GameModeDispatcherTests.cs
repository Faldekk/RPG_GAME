using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class GameModeDispatcherTests
    {
        private GameModeDispatcher _dispatcher;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            var moveUp = new MoveUpHandler();
            var unknown = new UnknownCommandHandler();
            moveUp.SetNext(unknown);
            
            _dispatcher = new GameModeDispatcher(moveUp);
            _state = new GameState();
            _world = new MockWorld();
        }

        [Test]
        public void GameModeDispatcher_NormalMode_RoutesToNormalModeHandler()
        {
            _state.CurrentMode = GameMode.Normal;
            _world.IsAtCraftingStationReturnValue = false;
            
            _dispatcher.HandleCommand(InputCommand.OpenInventory, _world, _state);
            
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
        }

        [Test]
        public void GameModeDispatcher_InventoryMode_RoutesToInventoryModeHandler()
        {
            _state.CurrentMode = GameMode.Inventory;
            _state.SelectedInventoryIndex = 2;
            
            _dispatcher.HandleCommand(InputCommand.InventoryUp, _world, _state);
            
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(1));
        }

        [Test]
        public void GameModeDispatcher_CombatMode_RoutesToCombatModeHandler()
        {
            _state.CurrentMode = GameMode.Combat;
            int messageCountBefore = _world.AddMessageCallCount;
            
            _dispatcher.HandleCommand(InputCommand.CombatNormalAttack, _world, _state);
            
            Assert.That(_world.AddMessageCallCount, Is.GreaterThan(messageCountBefore));
        }

        [Test]
        public void GameModeDispatcher_DeathMode_RoutesToDeathModeHandler()
        {
            _state.CurrentMode = GameMode.Death;
            _state.SelectedInventoryIndex = 5;
            
            _dispatcher.HandleCommand(InputCommand.DeathRespawn, _world, _state);
            
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void GameModeDispatcher_WeaponCraftingMode_RoutesToWeaponCraftingModeHandler()
        {
            _state.CurrentMode = GameMode.WeaponCrafting;
            _state.CraftingFirstSelection = 0;
            
            _dispatcher.HandleCommand(InputCommand.CraftingSelectFirst, _world, _state);
            Assert.That(_world.AddMessageCallCount, Is.GreaterThan(0));
        }

        [Test]
        public void GameModeDispatcher_CorrectlyTransitionsAcrossModes()
        {
            _state.CurrentMode = GameMode.Normal;
            _dispatcher.HandleCommand(InputCommand.OpenInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
            _dispatcher.HandleCommand(InputCommand.CloseInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void GameModeDispatcher_AllModesRegistered()
        {
            foreach (var mode in new[] { GameMode.Normal, GameMode.Inventory, GameMode.Combat, GameMode.Death, GameMode.WeaponCrafting })
            {
                _state.CurrentMode = mode;
                Assert.DoesNotThrow(() => _dispatcher.HandleCommand(InputCommand.None, _world, _state));
            }
        }
    }
}
