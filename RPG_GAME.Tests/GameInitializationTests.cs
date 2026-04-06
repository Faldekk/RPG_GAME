using NUnit.Framework;
using RPG_GAME.Model;

namespace RPG_GAME.Tests
{
    [TestFixture]
    public class GameInitializationTests
    {
        [Test]
        public void Game_Constructor_InitializesSuccessfully()
        {
            Assert.DoesNotThrow(() => new Game.Game());
        }

        [Test]
        public void Game_CanCallStop()
        {
            var game = new Game.Game();
            Assert.DoesNotThrow(() => game.Stop());
        }

        [Test]
        public void Game_SimpleInitializationTest()
        {
            var game = new Game.Game();
            Assert.That(game, Is.Not.Null);
        }
    }
}
