using System;
using System.Linq;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.UI
{
    public sealed class NetworkRenderer
    {
        private readonly ConsoleBuffer _buffer;

        public NetworkRenderer(ConsoleBuffer buffer)
        {
            _buffer = buffer;
        }

        public void Render(GameStateDto? state, int localPlayerId)
        {
            _buffer.Clear();

            if (state == null)
            {
                _buffer.PutString(0, 0, "Waiting for server state...");
                _buffer.Flush();
                return;
            }

            RenderMap(state);
            RenderSidePanel(state, localPlayerId);
            RenderRecentLog(state);

            _buffer.Flush();
        }

        private void RenderMap(GameStateDto state)
        {
            if (state.MapRows != null && state.MapRows.Length > 0)
            {
                for (int y = 0; y < state.MapRows.Length; y++)
                {
                    _buffer.PutString(y, 0, state.MapRows[y]);
                }

                return;
            }

            char[,] grid = new char[state.Height, state.Width];

            for (int y = 0; y < state.Height; y++)
            {
                for (int x = 0; x < state.Width; x++)
                {
                    grid[y, x] = '.';
                }
            }

            foreach (var tile in state.Tiles)
            {
                if (tile.Y < 0 || tile.Y >= state.Height ||
                    tile.X < 0 || tile.X >= state.Width)
                    continue;

                grid[tile.Y, tile.X] = tile.Symbol != '\0'
                    ? tile.Symbol
                    : tile.IsWall ? '#' : '.';
            }

            foreach (var item in state.Items)
            {
                if (item.Y < 0 || item.Y >= state.Height ||
                    item.X < 0 || item.X >= state.Width)
                    continue;

                grid[item.Y, item.X] = item.MapCharacter != '\0'
                    ? item.MapCharacter
                    : '?';
            }

            foreach (var enemy in state.Enemies)
            {
                if (enemy.Y < 0 || enemy.Y >= state.Height ||
                    enemy.X < 0 || enemy.X >= state.Width)
                    continue;

                grid[enemy.Y, enemy.X] = enemy.MapCharacter != '\0'
                    ? enemy.MapCharacter
                    : 'E';
            }

            foreach (var player in state.Players)
            {
                if (player.Y < 0 || player.Y >= state.Height ||
                    player.X < 0 || player.X >= state.Width)
                    continue;

                grid[player.Y, player.X] = GetPlayerSymbol(player.PlayerId);
            }

            for (int y = 0; y < state.Height; y++)
            {
                var line = new char[state.Width];

                for (int x = 0; x < state.Width; x++)
                    line[x] = grid[y, x];

                _buffer.PutString(y, 0, new string(line));
            }
        }

        private void RenderSidePanel(GameStateDto state, int localPlayerId)
        {
            int col = state.Width + 3;
            int row = 0;

            _buffer.PutString(row++, col, "MULTIPLAYER");
            _buffer.PutString(row++, col, $"You: Player {localPlayerId}");
            _buffer.PutString(row++, col, $"Mode: {state.CurrentMode}");
            _buffer.PutString(row++, col, $"Time: {state.ElapsedSeconds}s");
            row++;

            var localPlayer = state.Players.FirstOrDefault(p => p.PlayerId == localPlayerId);

            if (localPlayer != null)
            {
                _buffer.PutString(row++, col, "YOUR STATS");
                _buffer.PutString(row++, col, $"HP: {localPlayer.Health}/{localPlayer.MaxHealth}");
                _buffer.PutString(row++, col, $"STR: {localPlayer.Strength}");
                _buffer.PutString(row++, col, $"DEX: {localPlayer.Dexterity}");
                _buffer.PutString(row++, col, $"LUK: {localPlayer.Luck}");
                _buffer.PutString(row++, col, $"AGG: {localPlayer.Aggression}");
                _buffer.PutString(row++, col, $"WIS: {localPlayer.Wisdom}");
                _buffer.PutString(row++, col, $"ARM: {localPlayer.Armor}");
                _buffer.PutString(row++, col, $"Coins: {localPlayer.Coins}");
                _buffer.PutString(row++, col, $"Gold: {localPlayer.Gold}");
                _buffer.PutString(row++, col, $"L: {localPlayer.LeftHand}");
                _buffer.PutString(row++, col, $"R: {localPlayer.RightHand}");
                row++;
            }

            _buffer.PutString(row++, col, "PLAYERS");

            foreach (var player in state.Players.OrderBy(p => p.PlayerId))
            {
                _buffer.PutString(
                    row++,
                    col,
                    $"{GetPlayerSymbol(player.PlayerId)} P{player.PlayerId} ({player.X},{player.Y}) HP:{player.Health}"
                );
            }

            row++;

            if (!string.IsNullOrWhiteSpace(state.CurrentMessage))
            {
                _buffer.PutString(row++, col, "MESSAGE");
                _buffer.PutString(row++, col, Trim(state.CurrentMessage, 45));
                row++;
            }

            if (state.IsGameWon)
                _buffer.PutString(row++, col, "GAME WON");

            if (state.IsExitRequested)
                _buffer.PutString(row++, col, "SERVER EXIT REQUESTED");
        }

        private void RenderRecentLog(GameStateDto state)
        {
            int row = state.Height + 1;
            int col = 0;

            _buffer.PutString(row++, col, "Recent log:");

            foreach (var entry in state.RecentEntries.TakeLast(6))
            {
                string line = $"{entry.Timestamp} {entry.Message}";
                _buffer.PutString(row++, col, Trim(line, 100));
            }
        }

        private static char GetPlayerSymbol(int playerId)
        {
            if (playerId < 1 || playerId > 9)
                return '?';

            return (char)('0' + playerId);
        }

        private static string Trim(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text[..Math.Max(0, maxLength - 3)] + "...";
        }
    }
}