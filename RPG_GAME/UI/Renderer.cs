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
            _buffer = new ConsoleBuffer(World.Height + 2, World.Width + _config.PanelWidth);
        }

        public void Render(World world)
        {
            _buffer.Clear();

            RenderMap(world);
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
            _buffer.PutString(startRow++, panelX, "Get off the Island Game :)");
            _buffer.PutString(startRow++, panelX, "created by Epstein's survivor");
            return startRow + 1;
        }

        private int RenderControls(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "WASD - move");
            startRow++;
            _buffer.PutString(startRow++, panelX, "E - pick up the item");
            _buffer.PutString(startRow++, panelX, "G - drop the item");
            return startRow + 1;
        }

        private int RenderCurrency(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "Money:");
            _buffer.PutString(startRow++, panelX, $"  Coins: {player.Stats.Coins}");
            _buffer.PutString(startRow++, panelX, $"  Gold: {player.Stats.Gold}");
            return startRow + 1;
        }

        private int RenderStats(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "Stats:");
            _buffer.PutString(startRow++, panelX, $"  HP: {player.Stats.Health}/{player.Stats.MaxHealth}");
            _buffer.PutString(startRow++, panelX, $"  STR: {player.Stats.Strength}");
            _buffer.PutString(startRow++, panelX, $"  DEX: {player.Stats.Dexterity}");
            _buffer.PutString(startRow++, panelX, $"  LCK: {player.Stats.Luck}");
            return startRow + 1;
        }

        private int RenderEquipment(Player player, int panelX, int startRow)
        {
            var leftHand = player.Inventory.LeftHand;
            var rightHand = player.Inventory.RightHand;

            // No weapons equipped
            if (leftHand == null && rightHand == null)
            {
                _buffer.PutString(startRow++, panelX, "No weapons yet. Grind more");
                return startRow;
            }

            // Two-handed weapon
            if (player.Inventory.HasTwoHandedWeapon)
            {
                var weapon = leftHand ?? rightHand;
                if (weapon != null)
                {
                    _buffer.PutString(startRow++, panelX, $"2H: {weapon.Name}");
                    _buffer.PutString(startRow++, panelX, $"  DMG: {weapon.Damage_Heal} | DUR: {weapon.Lifespan}%");
                }
            }
            else
            {
                // Left hand
                string leftText = leftHand != null
                    ? $"L: {leftHand.Name} (DMG: {leftHand.Damage_Heal})"
                    : "L: (empty)";
                _buffer.PutString(startRow++, panelX, leftText);

                // Right hand
                string rightText = rightHand != null
                    ? $"R: {rightHand.Name} (DMG: {rightHand.Damage_Heal})"
                    : "R: (empty)";
                _buffer.PutString(startRow++, panelX, rightText);
            }

            return startRow;
        }

        private void RenderQuitOption(int panelX, int startRow)
        {
            _buffer.PutString(startRow, panelX, "Q - quit");
        }
    }

    /// <summary>
    /// Configuration for rendering settings
    /// </summary>
    public class RenderConfig
    {
        public int PanelWidth { get; set; } = 30;
        public char WallCharacter { get; set; } = '█';
        public char FloorCharacter { get; set; } = ' ';
        public char PlayerCharacter { get; set; } = 'ᒊ';
    }
}