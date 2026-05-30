namespace RPG_GAME.Network.Dto
{
    public enum NetworkMessageKind
    {
        Hello,
        State,
        Command,
        ServerNotice,
        Disconnect
    }
}
