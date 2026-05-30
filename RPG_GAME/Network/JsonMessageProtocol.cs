using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public static class JsonMessageProtocol
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static string Serialize(NetworkMessageDto message)
        {
            return JsonSerializer.Serialize(message, Options);
        }

        public static NetworkMessageDto? Deserialize(string json)
        {
            return JsonSerializer.Deserialize<NetworkMessageDto>(json, Options);
        }

        public static async Task WriteAsync(TextWriter writer, NetworkMessageDto message, CancellationToken cancellationToken = default)
        {
            await writer.WriteLineAsync(Serialize(message)).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }

        public static async Task<NetworkMessageDto?> ReadAsync(StreamReader reader, CancellationToken cancellationToken = default)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line == null)
                return null;

            return Deserialize(line);
        }
    }
}
