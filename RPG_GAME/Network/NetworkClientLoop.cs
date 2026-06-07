using System;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.UI;

namespace RPG_GAME.Network
{
    public sealed class NetworkClientLoop
    {
        private readonly GameClient _client;
        private readonly SimpleNetworkRenderer _renderer;

        public NetworkClientLoop(GameClient client, SimpleNetworkRenderer renderer)
        {
            _client = client;
            _renderer = renderer;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Console.Clear();
            Console.CursorVisible = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                _renderer.Render(_client.CurrentState, _client.PlayerId);

                if (Console.KeyAvailable)
                {
                    var command = ClientInputMapper.ReadCommand();

                    if (command != null)
                        await _client.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
                }

                await Task.Delay(150, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}