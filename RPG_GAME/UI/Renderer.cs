using System;
using RPG_GAME.App;
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
            int bufferHeight = 36;
            int bufferWidth = World.Width + _config.PanelWidth;
            _buffer = new ConsoleBuffer(bufferHeight, bufferWidth);
        }

        public void Render(World world, GameMode mode, int selectedInventoryIndex)
        {
            _buffer.Clear();

            if (mode == GameMode.Inventory)
            {
                RenderInventory(world, selectedInventoryIndex);
            }
            else
            {
                RenderMap(world);
                RenderPlayer(world.Player);
                RenderBottomControls();
                RenderUI(world.Player, world);
            }

            RenderCurrentMessage(world);
            _buffer.Flush();
        }

        private void RenderMap(World world)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    var tile = world.GetTile(y, x);
                    char ch = tile.Item != null ? _config.ItemCharacter : GetTileCharacter(tile);
                    _buffer.PutChar(y, x, ch);
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

        private void RenderBottomControls()
        {
            int row = World.Height + 1;
            _buffer.PutString(row++, 0, "[WASD] move  [E] pick up ");
            _buffer.PutString(row++, 0, "[X] swap  [1] drop left  [2] drop right");
            _buffer.PutString(row++, 0, "[B] inventory [Q] quit");
        }

        private void RenderUI(Player player, World world)
        {
            int currentRow = 0;
            int panelX = World.Width + 2;

            int x = player.Pos.X;
            int y = player.Pos.Y;
            var tile = world.GetTile(y, x);

            currentRow = RenderHeader(panelX, currentRow);
            currentRow = RenderCurrentTileInfo(panelX, currentRow, tile.Item);
            currentRow = RenderCurrency(player, panelX, currentRow);
            currentRow = RenderStats(player, panelX, currentRow);
            currentRow = RenderEquipment(player, panelX, currentRow);
            //RenderMessageLog(panelX, currentRow, world);
        }

        private void RenderInventory(World world, int selectedInventoryIndex)
        {
            int row = 1;
            _buffer.PutString(row++, 2, "=== BACKPACK ===");
            row++;

            int count = world.Player.Inventory.Count();
            if (count == 0)
            {
                _buffer.PutString(row++, 2, "(empty)");
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var item = world.Player.Inventory.GetItem(i);
                    if (item == null)
                        continue;

                    string marker = i == selectedInventoryIndex ? ">" : " ";
                    _buffer.PutString(row++, 2, $"{marker} {i + 1}. {item.Name} ({item.Type})");
                }
            }

            row += 2;
            _buffer.PutString(row++, 2, "[W] Up  [S] Down");
            _buffer.PutString(row++, 2, "[E] Equip  [D] Drop  [U] Use  [ESC] Close");
        }

        private int RenderHeader(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "RPG Game or sum");
            _buffer.PutString(startRow++, panelX, "by Faldekk");
            return startRow + 1;
        }

        private int RenderCurrentTileInfo(int panelX, int startRow, Items? item)
        {
            _buffer.PutString(startRow++, panelX, "=== CURRENT TILE INFO ===");

            if (item == null)
            {
                _buffer.PutString(startRow++, panelX, "Item: (none)");
                return startRow + 1;
            }

            _buffer.PutString(startRow++, panelX, $"Name: {item.Name}");
            _buffer.PutString(startRow++, panelX, $"Type: {item.Type}");
            _buffer.PutString(startRow++, panelX, $"Value: {item.Value}");
            _buffer.PutString(startRow++, panelX, $"Durability: {item.Durability}");
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
            }
            else if (player.Inventory.HasTwoHandedWeapon)
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

            _buffer.PutString(startRow++, panelX, $"Backpack: {player.Inventory.Count()}/{player.Inventory.MaxBackpackSize}");
            return startRow + 1;
        }

        //private int RenderMessageLog(int panelX, int startRow, World world)
        //{
        //    _buffer.PutString(startRow++, panelX, "=== MESSAGE LOG ===");

        //    if (world.MessageLog.Count == 0)
        //    {
        //        _buffer.PutString(startRow++, panelX, "(no messages)");
        //        return startRow + 1;
        //    }

        //    foreach (var message in world.MessageLog)
        //    {
        //        _buffer.PutString(startRow++, panelX, $"- {message}");
        //    }

        //    return startRow + 1;
        //}

        private void RenderCurrentMessage(World world)
        {
            if (string.IsNullOrWhiteSpace(world.CurrentMessage))
                return;

            int row = 34;
            _buffer.PutString(row, 0, $"Message: {world.CurrentMessage}");
        }
    }

    public class RenderConfig
    {
        public int PanelWidth { get; set; } = 50;
        public char WallCharacter { get; set; } = '█';
        public char FloorCharacter { get; set; } = ' ';
        public char PlayerCharacter { get; set; } = '¶';
        public char ItemCharacter { get; set; } = 'x';
    }
}