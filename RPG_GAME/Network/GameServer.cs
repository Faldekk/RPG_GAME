using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.Model;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public sealed class GameServer
    {
        private readonly GameSession _session;
        private readonly TcpListener _listener;
        private readonly ConcurrentDictionary<int, ClientConnection> _clients = new();
        private readonly ConcurrentQueue<PlayerCommandDto> _commandQueue = new();
        private readonly object _sync = new();

        public GameServer(GameSession session, int port)
        {
            _session = session;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            _listener.Start();
            var acceptTask = AcceptLoopAsync(cancellationToken);
            var processTask = ProcessLoopAsync(cancellationToken);
            await Task.WhenAll(acceptTask, processTask).ConfigureAwait(false);
        }

        private async Task AcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                int playerId;
                lock (_sync)
                {
                    playerId = _session.RegisterPlayer(FindSpawnPosition());
                }

                if (playerId < 0)
                {
                    tcpClient.Close();
                    continue;
                }

                var connection = new ClientConnection(tcpClient, playerId);
                _clients[playerId] = connection;
                _ = Task.Run(() => ReceiveLoopAsync(connection, cancellationToken), cancellationToken);
                await SendStateAsync(connection, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ReceiveLoopAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await connection.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                if (message == null)
                    break;

                if (message.Type == nameof(NetworkMessageKind.Command) && message.Payload.ValueKind != System.Text.Json.JsonValueKind.Undefined)
                {
                    var command = System.Text.Json.JsonSerializer.Deserialize<PlayerCommandDto>(message.Payload.GetRawText());
                    if (command != null)
                    {
                        command.PlayerId = connection.PlayerId;
                        _commandQueue.Enqueue(command);
                    }
                }
            }

            _clients.TryRemove(connection.PlayerId, out _);
            await connection.DisposeAsync().ConfigureAwait(false);
        }

        private async Task ProcessLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_commandQueue.TryDequeue(out var command))
                {
                    lock (_sync)
                    {
                        _session.ApplyCommand(command);
                    }

                    await BroadcastStateAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task BroadcastStateAsync(CancellationToken cancellationToken)
        {
            var state = CreateState();
            var message = new NetworkMessageDto { Type = nameof(NetworkMessageKind.State), Payload = System.Text.Json.JsonSerializer.SerializeToElement(state) };
            var sendTasks = _clients.Values.Select(client => client.SendAsync(message, cancellationToken));
            await Task.WhenAll(sendTasks).ConfigureAwait(false);
        }

        private async Task SendStateAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            var state = CreateState();
            var message = new NetworkMessageDto { Type = nameof(NetworkMessageKind.State), Payload = System.Text.Json.JsonSerializer.SerializeToElement(state) };
            await connection.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        private GameStateDto CreateState()
        {
            lock (_sync)
            {
                return _session.CreateStateDto();
            }
        }

        private Vec2 FindSpawnPosition()
        {
            for (int y = 1; y < World.Height - 1; y++)
            {
                for (int x = 1; x < World.Width - 1; x++)
                {
                    var tile = _session.World.GetTile(y, x);
                    if (tile.IsWall || tile.Enemy != null || tile.Item != null)
                        continue;

                    bool occupied = false;
                    foreach (var player in _session.Players.Values)
                    {
                        if (player.Pos.X == x && player.Pos.Y == y)
                        {
                            occupied = true;
                            break;
                        }
                    }

                    if (!occupied)
                        return new Vec2(x, y);
                }
            }

            return new Vec2(1, 1);
        }
    }
}
