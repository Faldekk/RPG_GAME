namespace RPG_GAME.App.Startup
{
    public sealed class StartupOptions
    {
        public bool IsServer { get; init; }
        public bool IsClient { get; init; }
        public int Port { get; init; } = 5555;
        public string Host { get; init; } = "127.0.0.1";
    }
}
