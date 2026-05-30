using System;

namespace RPG_GAME.App.Startup
{
    public static class StartupParser
    {
        public static StartupOptions Parse(string[] args)
        {
            var options = new StartupOptions();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].Trim();
                if (arg.Equals("--server", StringComparison.OrdinalIgnoreCase))
                {
                    options = new StartupOptions { IsServer = true, Port = ReadPort(args, ref i, options.Port) };
                }
                else if (arg.Equals("--client", StringComparison.OrdinalIgnoreCase))
                {
                    var endpoint = ReadEndpoint(args, ref i);
                    options = new StartupOptions { IsClient = true, Host = endpoint.host, Port = endpoint.port };
                }
            }

            return options;
        }

        private static int ReadPort(string[] args, ref int index, int defaultPort)
        {
            if (index + 1 >= args.Length)
                return defaultPort;

            if (int.TryParse(args[index + 1], out var port))
            {
                index++;
                return port;
            }

            return defaultPort;
        }

        private static (string host, int port) ReadEndpoint(string[] args, ref int index)
        {
            if (index + 1 >= args.Length)
                return ("127.0.0.1", 5555);

            var value = args[index + 1];
            index++;

            var parts = value.Split(':', 2);
            if (parts.Length == 2 && int.TryParse(parts[1], out var port))
                return (parts[0], port);

            if (int.TryParse(value, out port))
                return ("127.0.0.1", port);

            return (value, 5555);
        }
    }
}
