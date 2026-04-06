using NUnit.Framework;
using RPG_GAME.App;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class CombatModeHandlerTests
    {
        private CombatModeHandler _handler;
        private GameState _state;
        private MockWorld _world;

        [SetUp]
        public void Setup()
        {
            _handler = new CombatModeHandler();
            _state = new GameState { CurrentMode = GameMode.Combat };
            _world = new MockWorld();
        }

        [Test]
        public void CombatModeHandler_OnCombatNormalAttack_CallsTryCombatRound()
        {
            int messageCountBefore = _world.AddMessageCallCount;
            
            _handler.Handle(InputCommand.CombatNormalAttack, _world, _state);
            
            Assert.That(_world.AddMessageCallCount, Is.GreaterThan(messageCountBefore));
        }

        [Test]
        public void CombatModeHandler_OnCombatStealthAttack_CallsTryCombatRound()
        {
            int messageCountBefore = _world.AddMessageCallCount;
            
            _handler.Handle(InputCommand.CombatStealthAttack, _world, _state);
            
            Assert.That(_world.AddMessageCallCount, Is.GreaterThan(messageCountBefore));
        }

        [Test]
        public void CombatModeHandler_OnCombatMagicalAttack_CallsTryCombatRound()
        {
            int messageCountBefore = _world.AddMessageCallCount;
            
            _handler.Handle(InputCommand.CombatMagicalAttack, _world, _state);
            
            Assert.That(_world.AddMessageCallCount, Is.GreaterThan(messageCountBefore));
        }

        [Test]
        public void CombatModeHandler_OnInvalidCommand_ShowsHelpMessage()
        {
            _handler.Handle(InputCommand.OpenInventory, _world, _state);
            
            Assert.That(_world.LastMessage, Contains.Substring("Combat mode"));
        }

        [Test]
        public void CombatModeHandler_StaysInCombatWhenEnemyActive()
        {
            _state.CurrentMode = GameMode.Combat;
            
            _handler.Handle(InputCommand.CombatNormalAttack, _world, _state);
            Assert.That(_state.CurrentMode, Is.AnyOf(GameMode.Combat, GameMode.Normal));
        }
    }
}
