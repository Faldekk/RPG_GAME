using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.App.Configuration;
using RPG_GAME.Model;
using RPG_GAME.Model.DungeonThemes;
using RPG_GAME.Network;

namespace RPG_GAME.App.Startup
{
    public static class StartupRunner
    {
        public static async Task RunAsync(StartupOptions options, GameConfig config)
        {
            var theme = DungeonThemeFactory.CreateRandom();
            var world = new World(theme);
            var session = new GameSession(world);

            if (options.IsServer)
            {
                var server = new GameServer(session, options.Port);
                await server.RunAsync(CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var client = new GameClient();
                try
                {
                    await client.ConnectAsync(options.Host, options.Port, CancellationToken.None).ConfigureAwait(false);
                    while (true)
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine($"Cannot connect to server {options.Host}:{options.Port}. Start server first with --server.");
                }
            }
        }
    }
}
