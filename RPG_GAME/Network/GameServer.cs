using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.Model;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public sealed class GameServer
    {
        private const int MaxPlayers = 9;

        private readonly GameSession _session;
        private readonly TcpListener _listener;
     
        private readonly ConcurrentDictionary<int, ClientConnection> _clients = new();
        private readonly ConcurrentQueue<PlayerCommandDto> _commandQueue = new();
        private readonly object _sync = new();

        public GameServer(GameSession session, int port)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
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
                TcpClient tcpClient;

                try
                {
                    tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    throw;
                }

                int playerId;

                lock (_sync)
                {
                    if (_clients.Count >= MaxPlayers)
                    {
                        tcpClient.Close();
                        continue;
                    }

                    playerId = _session.RegisterPlayer(FindSpawnPosition());
                }

                if (playerId < 1 || playerId > MaxPlayers)
                {
                    tcpClient.Close();
                    continue;
                }

                var connection = new ClientConnection(tcpClient, playerId);
                _clients[playerId] = connection;

                await SendAssignPlayerAsync(connection, cancellationToken).ConfigureAwait(false);
                await SendStateAsync(connection, cancellationToken).ConfigureAwait(false);

                _ = Task.Run(
                    () => ReceiveLoopAsync(connection, cancellationToken),
                    cancellationToken);
            }
        }

        private async Task ReceiveLoopAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await connection.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                    if (message == null)
                        break;

                    if (message.Type == nameof(NetworkMessageKind.Command) &&
                        message.Payload.ValueKind != JsonValueKind.Undefined)
                    {
                        var command = JsonSerializer.Deserialize<PlayerCommandDto>(
                            message.Payload.GetRawText());

                        if (command != null)
                        {
                            command.PlayerId = connection.PlayerId;
                            _commandQueue.Enqueue(command);
                        }
                    }
                }
            }
            finally
            {
                _clients.TryRemove(connection.PlayerId, out _);

                lock (_sync)
                {
                    _session.UnregisterPlayer(connection.PlayerId);
                }

                await connection.DisposeAsync().ConfigureAwait(false);
                await BroadcastStateSafeAsync(cancellationToken).ConfigureAwait(false);
            }
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

                    await BroadcastStateSafeAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(10, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task SendAssignPlayerAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            var assign = new AssignPlayerDto
            {
                PlayerId = connection.PlayerId,
                Symbol = GetPlayerSymbol(connection.PlayerId),
                Name = $"Player {connection.PlayerId}"
            };

            var message = new NetworkMessageDto
            {
                Type = nameof(NetworkMessageKind.AssignPlayer),
                Payload = JsonSerializer.SerializeToElement(assign)
            };

            await connection.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        private async Task SendStateAsync(ClientConnection connection, CancellationToken cancellationToken)
        {
            var state = CreateState();

            var message = new NetworkMessageDto
            {
                Type = nameof(NetworkMessageKind.State),
                Payload = JsonSerializer.SerializeToElement(state)
            };

            await connection.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        private async Task BroadcastStateSafeAsync(CancellationToken cancellationToken)
        {
            if (_clients.IsEmpty)
                return;

            var state = CreateState();

            var message = new NetworkMessageDto
            {
                Type = nameof(NetworkMessageKind.State),
                Payload = JsonSerializer.SerializeToElement(state)
            };

            var sendTasks = _clients.Values
                .Select(client => SendSafeAsync(client, message, cancellationToken))
                .ToArray();

            await Task.WhenAll(sendTasks).ConfigureAwait(false);
        }

        private async Task SendSafeAsync(
            ClientConnection connection,
            NetworkMessageDto message,
            CancellationToken cancellationToken)
        {
            try
            {
                await connection.SendAsync(message, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _clients.TryRemove(connection.PlayerId, out _);
                await connection.DisposeAsync().ConfigureAwait(false);
            }
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
            int height = RPG_GAME.Model.World.Height;
            int width = RPG_GAME.Model.World.Width;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tile = _session.GameWorld.GetTile(y, x);

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

        private static char GetPlayerSymbol(int playerId)
        {
            if (playerId < 1 || playerId > 9)
                return '?';

            return (char)('0' + playerId);
        }
    }
}