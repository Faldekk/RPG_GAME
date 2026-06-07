using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public sealed class ClientConnection : IAsyncDisposable
    {
        private readonly TcpClient _client;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public int PlayerId { get; }

        public ClientConnection(TcpClient client, int playerId)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            PlayerId = playerId;

            var stream = _client.GetStream();

            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };
        }

        public Task SendAsync(
            NetworkMessageDto message,
            CancellationToken cancellationToken = default)
        {
            return JsonMessageProtocol.WriteAsync(_writer, message, cancellationToken);
        }

        public Task<NetworkMessageDto?> ReceiveAsync(
            CancellationToken cancellationToken = default)
        {
            return JsonMessageProtocol.ReadAsync(_reader, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            _writer.Dispose();
            _reader.Dispose();
            _client.Close();

            return ValueTask.CompletedTask;
        }
    }
}
