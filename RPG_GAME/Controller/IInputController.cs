namespace RPG_GAME.Controller
{
    public interface IInputController
    {
        RPG_GAME.App.InputCommand ReadCommand(RPG_GAME.App.GameMode mode);
    }
}
