using System.Text.Json;

namespace RPG_GAME.Network.Dto
{
    public sealed class NetworkMessageDto
    {
        public string Type { get; set; } = string.Empty;
        public JsonElement Payload { get; set; }
    }
}
