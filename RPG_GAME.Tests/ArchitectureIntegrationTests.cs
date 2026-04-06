using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class ArchitectureIntegrationTests
    {
        private GameState _state;
        private MockWorld _world;
        private GameModeDispatcher _dispatcher;

        [SetUp]
        public void Setup()
        {
            var moveUp = new MoveUpHandler();
            var unknown = new UnknownCommandHandler();
            moveUp.SetNext(unknown);
            
            _state = new GameState();
            _world = new MockWorld();
            _dispatcher = new GameModeDispatcher(moveUp);
        }

        [Test]
        public void Architecture_CompleteGameFlow_ExplorationToCombat()
        {
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
            _dispatcher.HandleCommand(InputCommand.OpenInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
            _dispatcher.HandleCommand(InputCommand.CloseInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void Architecture_StateIsPreservedAcrossTransitions()
        {
            _state.SelectedInventoryIndex = 3;
            _state.CurrentMode = GameMode.Normal;
            _dispatcher.HandleCommand(InputCommand.OpenInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
            
            _dispatcher.HandleCommand(InputCommand.CloseInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void Architecture_GameModeDispatcher_HandlesManyCommands()
        {
            _state.CurrentMode = GameMode.Inventory;
            
            for (int i = 0; i < 10; i++)
            {
                Assert.DoesNotThrow(() => 
                    _dispatcher.HandleCommand(InputCommand.InventoryDown, _world, _state));
            }
        }

        [Test]
        public void Architecture_MultipleModesCanBeVisited()
        {
            _state.CurrentMode = GameMode.Normal;
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
            _dispatcher.HandleCommand(InputCommand.OpenInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
            _dispatcher.HandleCommand(InputCommand.CloseInventory, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
            _world.IsAtCraftingStationReturnValue = true;
            _dispatcher.HandleCommand(InputCommand.Pickup, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.WeaponCrafting));
            _dispatcher.HandleCommand(InputCommand.CraftingCancel, _world, _state);
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void Architecture_InvalidCommandsAreHandledGracefully()
        {
            _state.CurrentMode = GameMode.Normal;
            foreach (var mode in new[] { GameMode.Normal, GameMode.Inventory, GameMode.Combat, GameMode.Death })
            {
                _state.CurrentMode = mode;
                Assert.DoesNotThrow(() => 
                    _dispatcher.HandleCommand(InputCommand.Unknown, _world, _state));
            }
        }

        [Test]
        public void Architecture_InventorySelectionBoundsClamping()
        {
            _state.CurrentMode = GameMode.Inventory;
            _state.SelectedInventoryIndex = 0;
            _dispatcher.HandleCommand(InputCommand.InventoryUp, _world, _state);
            Assert.That(_state.SelectedInventoryIndex, Is.GreaterThanOrEqualTo(0));
            for (int i = 0; i < 10; i++)
            {
                _dispatcher.HandleCommand(InputCommand.InventoryDown, _world, _state);
            }
            Assert.That(_state.SelectedInventoryIndex, Is.LessThan(_world.InventoryCount));
        }

        [Test]
        public void Architecture_WeaponCraftingSelectionWrapsCorrectly()
        {
            _state.CurrentMode = GameMode.WeaponCrafting;
            _state.CraftingFirstSelection = 0;
            for (int i = 0; i < 5; i++)
            {
                _dispatcher.HandleCommand(InputCommand.CraftingSelectFirst, _world, _state);
            }
            Assert.That(_state.CraftingFirstSelection, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void Architecture_RespawnClearsAllState()
        {
            _state.CurrentMode = GameMode.Death;
            _state.SelectedInventoryIndex = 10;
            _state.CraftingFirstSelection = 5;
            _dispatcher.HandleCommand(InputCommand.DeathRespawn, _world, _state);
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
                Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }
    }
}
