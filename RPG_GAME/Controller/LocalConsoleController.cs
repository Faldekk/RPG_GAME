namespace RPG_GAME.Controller
{
    public sealed class LocalConsoleController : IInputController
    {
        private readonly RPG_GAME.App.Input _input;

        public LocalConsoleController(RPG_GAME.App.Input input)
        {
            _input = input;
        }

        public RPG_GAME.App.InputCommand ReadCommand(RPG_GAME.App.GameMode mode)
        {
            return _input.ReadCommand(mode);
        }
    }
}
