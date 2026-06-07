using System;
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
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<NetworkMessageDto>(json, Options);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static async Task WriteAsync(
            TextWriter writer,
            NetworkMessageDto message,
            CancellationToken cancellationToken = default)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            cancellationToken.ThrowIfCancellationRequested();

            await writer.WriteLineAsync(Serialize(message)).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }

        public static async Task<NetworkMessageDto?> ReadAsync(
            StreamReader reader,
            CancellationToken cancellationToken = default)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync().ConfigureAwait(false);

            if (line == null)
                return null;

            return Deserialize(line);
        }
    }
}