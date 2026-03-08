using System;
using RPG_GAME.Model;

namespace RPG_GAME.UI
{
    public class Renderer
    {
        private readonly ConsoleBuffer _buffer;
        private readonly RenderConfig _config;

        public Renderer()
        {
            _config = new RenderConfig();

            int bufferHeight = 30;
            int bufferWidth = World.Width + _config.PanelWidth;

            _buffer = new ConsoleBuffer(bufferHeight, bufferWidth);
        }

        public void Render(World world)
        {
            _buffer.Clear();

            RenderMap(world);
            RenderItems(world);
            RenderPlayer(world.Player);
            RenderUI(world.Player);

            _buffer.Flush();
        }

        private void RenderMap(World world)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    char tileChar = GetTileCharacter(world.GetTile(y, x));
                    _buffer.PutChar(y, x, tileChar);
                }
            }
        }

        private void RenderItems(World world)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    var tile = world.GetTile(y, x);
                    if (tile.HasItem)
                    {
                        _buffer.PutChar(y, x, _config.ItemCharacter);
                    }
                }
            }
        }

        private char GetTileCharacter(Tile tile)
        {
            return tile.IsWall ? _config.WallCharacter : _config.FloorCharacter;
        }

        private void RenderPlayer(Player player)
        {
            _buffer.PutChar(player.Pos.Y, player.Pos.X, _config.PlayerCharacter);
        }

        private void RenderUI(Player player)
        {
            int currentRow = 0;
            int panelX = World.Width + 2;

            currentRow = RenderHeader(panelX, currentRow);
            currentRow = RenderControls(panelX, currentRow);
            currentRow = RenderCurrency(player, panelX, currentRow);
            currentRow = RenderStats(player, panelX, currentRow);
            currentRow = RenderEquipment(player, panelX, currentRow);
            RenderQuitOption(panelX, currentRow);
        }

        private int RenderHeader(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "RPG Game or sum");
            _buffer.PutString(startRow++, panelX, "by Faldekk");
            return startRow + 1;
        }

        private int RenderControls(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== CONTROLS ===");
            _buffer.PutString(startRow++, panelX, "WASD - move");
            _buffer.PutString(startRow++, panelX, "E - pick up");
            _buffer.PutString(startRow++, panelX, "G - drop");
            _buffer.PutString(startRow++, panelX, "X - swap hands");
            _buffer.PutString(startRow++, panelX, "1 - drop left");
            _buffer.PutString(startRow++, panelX, "2 - drop right");
            return startRow + 1;
        }

        private int RenderCurrency(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== MONEY ===");
            _buffer.PutString(startRow++, panelX, $"Coins: {player.Stats.Coins}");
            _buffer.PutString(startRow++, panelX, $"Gold: {player.Stats.Gold}");
            return startRow + 1;
        }

        private int RenderStats(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== STATS ===");
            _buffer.PutString(startRow++, panelX, $"HP: {player.Stats.Health}/{player.Stats.MaxHealth}");
            _buffer.PutString(startRow++, panelX, $"STRENGTH: {player.Stats.Strength}");
            _buffer.PutString(startRow++, panelX, $"DEXTERITY: {player.Stats.Dexterity}");
            _buffer.PutString(startRow++, panelX, $"LUCK: {player.Stats.Luck}");
            _buffer.PutString(startRow++, panelX, $"AGGRESSION: {player.Stats.Aggression}");
            _buffer.PutString(startRow++, panelX, $"WISDOM: {player.Stats.Wisdom}");
            return startRow + 1;
        }

        private int RenderEquipment(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== EQUIPMENT ===");

            var leftHand = player.Inventory.LeftHand;
            var rightHand = player.Inventory.RightHand;

            if (leftHand == null && rightHand == null)
            {
                _buffer.PutString(startRow++, panelX, "No weapons yet");
                return startRow + 1;
            }

            if (player.Inventory.HasTwoHandedWeapon)
            {
                var weapon = leftHand ?? rightHand;
                if (weapon != null)
                {
                    _buffer.PutString(startRow++, panelX, $"2H: {weapon.Name}");
                    _buffer.PutString(startRow++, panelX, $"DMG: {weapon.Value}");
                }
            }
            else
            {
                string leftText = leftHand != null
                    ? $"L: {leftHand.Name} ({leftHand.Value})"
                    : "L: (empty)";
                _buffer.PutString(startRow++, panelX, leftText);

                string rightText = rightHand != null
                    ? $"R: {rightHand.Name} ({rightHand.Value})"
                    : "R: (empty)";
                _buffer.PutString(startRow++, panelX, rightText);
            }

            return startRow + 1;
        }

        private void RenderQuitOption(int panelX, int startRow)
        {
            _buffer.PutString(startRow, panelX, "Q - quit game");
        }
    }

    public class RenderConfig
    {
        public int PanelWidth { get; set; } = 40;
        public char WallCharacter { get; set; } = '█';
        public char FloorCharacter { get; set; } = ' ';
        public char PlayerCharacter { get; set; } = '¶';
        public char ItemCharacter { get; set; } = 'x';
    }
}