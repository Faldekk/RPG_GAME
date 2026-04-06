using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        private GameState _state;

        [SetUp]
        public void Setup()
        {
            _state = new GameState();
        }

        [Test]
        public void GameState_Constructor_InitializesWithNormalMode()
        {
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Normal));
        }

        [Test]
        public void GameState_Constructor_InitializesInventoryIndexToZero()
        {
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
        }

        [Test]
        public void GameState_Constructor_InitializesCraftingSelectionToNegativeOne()
        {
            Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
        }

        [Test]
        public void GameState_CurrentMode_CanBeChanged()
        {
            _state.CurrentMode = GameMode.Inventory;
            Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.Inventory));
        }

        [Test]
        public void GameState_SelectedInventoryIndex_CanBeChanged()
        {
            _state.SelectedInventoryIndex = 5;
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(5));
        }

        [Test]
        public void GameState_CraftingFirstSelection_CanBeChanged()
        {
            _state.CraftingFirstSelection = 2;
            Assert.That(_state.CraftingFirstSelection, Is.EqualTo(2));
        }

        [Test]
        public void GameState_ResetForMode_SetsModeAndClearsRelatedState()
        {
            _state.SelectedInventoryIndex = 10;
            _state.CraftingFirstSelection = 5;
            
            _state.ResetForMode(GameMode.WeaponCrafting);
            
            Assert.Multiple(() =>
            {
                Assert.That(_state.CurrentMode, Is.EqualTo(GameMode.WeaponCrafting));
                Assert.That(_state.CraftingFirstSelection, Is.EqualTo(-1));
            });
        }

        [Test]
        public void GameState_ResetForMode_NormalMode_ClearsInventorySelection()
        {
            _state.SelectedInventoryIndex = 10;
            
            _state.ResetForMode(GameMode.Normal);
            
            Assert.That(_state.SelectedInventoryIndex, Is.EqualTo(0));
        }
    }
}
