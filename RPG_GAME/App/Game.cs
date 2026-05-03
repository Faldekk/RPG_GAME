using RPG_GAME.App.Configuration;
using RPG_GAME.App.Logging;
using RPG_GAME.Model;
using RPG_GAME.Model.DungeonThemes;
using RPG_GAME.UI;

namespace RPG_GAME.App
{
    public class Game
    {
        private readonly GameLoop _gameLoop;
        private readonly World _world;
        private readonly GameConfig _config;
        private readonly InMemoryGameLogger _journalLogger;
        private readonly RecentEntriesLogger _recentLogger;
        private readonly FileGameLogger _fileLogger;

        public Game(GameConfig config)
        {
            _config = config;
            _journalLogger = new InMemoryGameLogger();
            _recentLogger = new RecentEntriesLogger();
            _fileLogger = new FileGameLogger(config.LogDirectory, config.PlayerName);

            var compositeLogger = new CompositeGameLogger(_journalLogger, _recentLogger, _fileLogger);
            GameLog.Configure(compositeLogger, _journalLogger, _recentLogger, _fileLogger);

            var theme = DungeonThemeFactory.CreateRandom();
            _world = new World(theme);
            var renderer = new Renderer();
            var input = new Input();
            var state = new GameState();
            var commandPipeline = BuildCommandPipeline();
            var dispatcher = new GameModeDispatcher(commandPipeline);

            _gameLoop = new GameLoop(_world, renderer, input, dispatcher, state);
        }

        public void Run()
        {
            _gameLoop.Run();
        }

        public void Stop()
        {
            _gameLoop.Stop();
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