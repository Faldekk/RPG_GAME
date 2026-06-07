using System;
using System.Linq;
using System.Text;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.UI
{
    public sealed class SimpleNetworkRenderer
    {
        public void Render(GameStateDto? state, int localPlayerId)
        {
            Console.CursorVisible = false;
            Console.Clear();

            var frame = BuildFrame(state, localPlayerId);
            Console.Write(frame);
        }

        private static string BuildFrame(GameStateDto? state, int localPlayerId)
        {
            int width = Math.Max(40, Console.WindowWidth - 1);
            var sb = new StringBuilder();

            if (state == null)
            {
                AppendLine(sb, "Waiting for server state...", width);
                return sb.ToString();
            }

            if (state.MapRows != null && state.MapRows.Length > 0)
            {
                foreach (var row in state.MapRows)
                    AppendLine(sb, row, width);
            }
            else
            {
                AppendLine(sb, "No map data received.", width);
            }

            AppendLine(sb, "", width);
            AppendLine(sb, $"MULTIPLAYER | You: Player {localPlayerId}", width);
            AppendLine(sb, $"Mode: {state.CurrentMode}", width);
            AppendLine(sb, $"Time: {state.ElapsedSeconds}s", width);

            var player = state.Players.FirstOrDefault(p => p.PlayerId == localPlayerId);

            if (player != null)
            {
                AppendLine(sb, "", width);
                AppendLine(sb, "YOUR STATS", width);
                AppendLine(sb, $"HP: {player.Health}/{player.MaxHealth}", width);
                AppendLine(sb, $"STR:{player.Strength} DEX:{player.Dexterity} LUCK:{player.Luck}", width);
                AppendLine(sb, $"AGG:{player.Aggression} WIS:{player.Wisdom} ARM:{player.Armor}", width);
                AppendLine(sb, $"Coins:{player.Coins} Gold:{player.Gold}", width);
                AppendLine(sb, $"L: {player.LeftHand}", width);
                AppendLine(sb, $"R: {player.RightHand}", width);
            }

            AppendLine(sb, "", width);
            AppendLine(sb, "PLAYERS", width);

            foreach (var p in state.Players.OrderBy(p => p.PlayerId))
            {
                AppendLine(
                    sb,
                    $"{GetPlayerSymbol(p.PlayerId)} Player {p.PlayerId} ({p.X},{p.Y}) HP:{p.Health}",
                    width
                );
            }

            if (!string.IsNullOrWhiteSpace(state.CurrentMessage))
            {
                AppendLine(sb, "", width);
                AppendLine(sb, $"Message: {OneLine(state.CurrentMessage)}", width);
            }

            if (state.IsGameWon)
            {
                AppendLine(sb, "", width);
                AppendLine(sb, "GAME WON", width);
            }

            if (state.IsExitRequested)
            {
                AppendLine(sb, "", width);
                AppendLine(sb, "SERVER EXIT REQUESTED", width);
            }

            AppendLine(sb, "", width);
            AppendLine(sb, "RECENT LOG", width);

            foreach (var entry in state.RecentEntries.TakeLast(6))
                AppendLine(sb, $"{entry.Timestamp} {OneLine(entry.Message)}", width);

            AppendLine(sb, "", width);
            AppendLine(sb, "Controls: WASD move | E pickup | 1/2/3 attack | Q quit", width);

            return sb.ToString();
        }

        private static void AppendLine(StringBuilder sb, string text, int maxWidth)
        {
            text = OneLine(text);

            if (text.Length > maxWidth)
                text = text[..maxWidth];

            sb.AppendLine(text);
        }

        private static string OneLine(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
        }

        private static char GetPlayerSymbol(int playerId)
        {
            if (playerId < 1 || playerId > 9)
                return '?';

            return (char)('0' + playerId);
        }
    }
}