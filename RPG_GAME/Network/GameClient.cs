using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public sealed class GameClient : IAsyncDisposable
    {
        private readonly TcpClient _client = new();
        private StreamReader? _reader;
        private StreamWriter? _writer;

        public GameStateDto? CurrentState { get; private set; }

        public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            await _client.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
            var stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
            _ = Task.Run(() => ReceiveLoopAsync(cancellationToken), cancellationToken);
        }

        public Task SendCommandAsync(PlayerCommandDto command, CancellationToken cancellationToken = default)
        {
            if (_writer == null)
                throw new InvalidOperationException("Client is not connected.");

            var message = new NetworkMessageDto
            {
                Type = nameof(NetworkMessageKind.Command),
                Payload = System.Text.Json.JsonSerializer.SerializeToElement(command)
            };

            return JsonMessageProtocol.WriteAsync(_writer, message, cancellationToken);
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            if (_reader == null)
                return;

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await JsonMessageProtocol.ReadAsync(_reader, cancellationToken).ConfigureAwait(false);
                if (message == null)
                    break;

                if (message.Type == nameof(NetworkMessageKind.State))
                    CurrentState = System.Text.Json.JsonSerializer.Deserialize<GameStateDto>(message.Payload.GetRawText());
            }
        }

        public ValueTask DisposeAsync()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _client.Close();
            return ValueTask.CompletedTask;
        }
    }
}
