using System;
using System.Text;
using System.Threading.Tasks;
using RPG_GAME.App;
using RPG_GAME.App.Configuration;
using RPG_GAME.App.Logging;
using RPG_GAME.Model;
using RPG_GAME.Model.DungeonThemes;
using RPG_GAME.Network;
using RPG_GAME.UI;

namespace RPG_GAME
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            if (args.Length > 0 && args[0].Equals("--server", StringComparison.OrdinalIgnoreCase))
            {
                int port = ParseServerPort(args);

                var config = LoadConfig();
                ConfigureLogging(config);

                var theme = DungeonThemeFactory.CreateRandom();
                var world = new World(theme);
                var session = new GameSession(world);
                var server = new GameServer(session, port);

                Console.Clear();
                Console.WriteLine($"Server listening on port {port}");
                Console.WriteLine("Waiting for clients...");

                await server.RunAsync().ConfigureAwait(false);
                return;
            }

            if (args.Length > 0 && args[0].Equals("--client", StringComparison.OrdinalIgnoreCase))
            {
                var (host, port) = ParseClientEndpoint(args);

                var client = new GameClient();

                Console.Clear();
                Console.WriteLine($"Connecting to {host}:{port}...");

                await client.ConnectAsync(host, port).ConfigureAwait(false);

                var renderer = new SimpleNetworkRenderer();
                var loop = new NetworkClientLoop(client, renderer);

                await loop.RunAsync().ConfigureAwait(false);
                return;
            }

            var localConfig = LoadConfig();
            new Game(localConfig).Run();
        }

        private static GameConfig LoadConfig()
        {
            return ConfigLoader.Load("gameconfig.json");
        }

        private static void ConfigureLogging(GameConfig config)
        {
            var journalLogger = new InMemoryGameLogger();
            var recentLogger = new RecentEntriesLogger();
            var fileLogger = new FileGameLogger(config.LogDirectory, config.PlayerName);

            var compositeLogger = new CompositeGameLogger(
                journalLogger,
                recentLogger,
                fileLogger
            );

            GameLog.Configure(
                compositeLogger,
                journalLogger,
                recentLogger,
                fileLogger
            );
        }

        private static int ParseServerPort(string[] args)
        {
            if (args.Length >= 2 && int.TryParse(args[1], out int port))
                return port;

            return 5555;
        }

        private static (string Host, int Port) ParseClientEndpoint(string[] args)
        {
            string host = "127.0.0.1";
            int port = 5555;

            if (args.Length < 2)
                return (host, port);

            var raw = args[1];

            if (raw.Contains(':'))
            {
                var parts = raw.Split(':', 2);

                if (!string.IsNullOrWhiteSpace(parts[0]))
                    host = parts[0];

                if (parts.Length > 1 && int.TryParse(parts[1], out int parsedPort))
                    port = parsedPort;

                return (host, port);
            }

            if (int.TryParse(raw, out int onlyPort))
                port = onlyPort;
            else if (!string.IsNullOrWhiteSpace(raw))
                host = raw;

            return (host, port);
        }
    }
}