using System;
using System.Linq;
using RPG_GAME.App;
using RPG_GAME.Model;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.UI
{
    public class Renderer
    {
        private readonly ConsoleBuffer _buffer;
        private readonly RenderConfig _config;

        public Renderer()
        {
            _config = new RenderConfig();
            int bufferHeight = 50;
            int bufferWidth = World.Width + _config.PanelWidth;
            _buffer = new ConsoleBuffer(bufferHeight, bufferWidth);
        }

        public void Render(World world, GameMode mode, int selectedInventoryIndex, int craftingFirstSelection = -1)
        {
            _buffer.Clear();

            switch (mode.Kind)
            {
                case GameModeKind.Inventory:
                    RenderInventory(world, selectedInventoryIndex);
                    break;
                case GameModeKind.Combat:
                    RenderCombat(world);
                    break;
                case GameModeKind.Death:
                    RenderDeath(world);
                    break;
                case GameModeKind.WeaponCrafting:
                    RenderWeaponCrafting(world, craftingFirstSelection);
                    break;
                default:
                    RenderMap(world);
                    RenderPlayer(world.Player);
                    RenderBottomControls(world);
                    RenderUI(world.Player, world);
                    break;
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
                    char ch = tile.IsCraftingStation
                        ? '◊'
                        : tile.Enemy != null
                            ? tile.Enemy.MapCharacter
                            : tile.Item != null
                                ? tile.Item.MapCharacter
                                : GetTileCharacter(tile);

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

        private void RenderBottomControls(World world)
        {
            int row = World.Height + 1;
            int col = 0;
                    
            _buffer.PutString(row++, col, "[W/A/S/D] Move  [Q] Quit  [E] Interact " );
            _buffer.PutString(row, col, "[B] Backpack  [ESC] Close");

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
          //  currentRow = RenderInstructions(world, panelX, currentRow);
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
                    _buffer.PutString(row++, 2, $"{marker} {i + 1}. [{item.MapCharacter}] {item.Name}");
                    if (item.CanEquip)
                    {
                        _buffer.PutString(row++, 4, $"DMG: {item.Value}  DUR: {item.Durability}");
                    }
                    else if (item.Value > 0 && !item.IsHeal)
                    {
                        _buffer.PutString(row++, 4, $"Value: {item.Value}");
                    }
                }
            }

            row += 2;
            row = RenderInventoryControls(row);
        }

        private int RenderInventoryControls(int row)
        {
            _buffer.PutString(row++, 2, "[W/S] Navigate  [E] Equip  [D] Drop  [U] Use  [C] CraftArmor  [ESC] Close");
            return row;
        }

        private int RenderHeader(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "RPG Game or sum");
            _buffer.PutString(startRow++, panelX, "by Faldekk");
            return startRow + 1;
        }

        private int RenderCurrentTileInfo(int panelX, int startRow, Items? item)
        {
            _buffer.PutString(startRow++, panelX, "=== CURRENT TILE ===");

            if (item == null)
            {
                _buffer.PutString(startRow++, panelX, "(empty)");
                return startRow + 1;
            }

            _buffer.PutString(startRow++, panelX, $"[{item.MapCharacter}] {item.Name}");
            _buffer.PutString(startRow++, panelX, $"Value: {item.Value} | Dur: {item.Durability}");
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
            _buffer.PutString(startRow++, panelX, $"STR: {player.Stats.Strength}");
            _buffer.PutString(startRow++, panelX, $"DEX: {player.Stats.Dexterity}");
            _buffer.PutString(startRow++, panelX, $"LCK: {player.Stats.Luck}");
            _buffer.PutString(startRow++, panelX, $"AGG: {player.Stats.Aggression}");
            _buffer.PutString(startRow++, panelX, $"WIS: {player.Stats.Wisdom}");
            _buffer.PutString(startRow++, panelX, $"ARM: {player.Stats.Armor}");
            return startRow + 1;
        }

        private int RenderEquipment(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== EQUIPMENT ===");

            var leftHand = player.Inventory.LeftHand;
            var rightHand = player.Inventory.RightHand;

            if (leftHand == null && rightHand == null)
            {
                _buffer.PutString(startRow++, panelX, "(none)");
            }
            else if (player.Inventory.HasTwoHandedWeapon)
            {
                var weapon = leftHand ?? rightHand;
                if (weapon != null)
                {
                    _buffer.PutString(startRow++, panelX, $"2H: {weapon.Name}");
                    _buffer.PutString(startRow++, panelX, $"DMG: {weapon.Value}  DUR: {weapon.Durability}");
                }
            }
            else
            {
                if (leftHand != null)
                {
                    _buffer.PutString(startRow++, panelX, $"L: {leftHand.Name}");
                    _buffer.PutString(startRow++, panelX, $"DMG: {leftHand.Value}  DUR: {leftHand.Durability}");
                }
                if (rightHand != null)
                {
                    _buffer.PutString(startRow++, panelX, $"R: {rightHand.Name}");
                    _buffer.PutString(startRow++, panelX, $"DMG: {rightHand.Value}  DUR: {rightHand.Durability}");
                }
            }

            return startRow + 1;
        }

        private void RenderCurrentMessage(World world)
        {
            if (string.IsNullOrWhiteSpace(world.CurrentMessage))
                return;

            int row = 34;
            _buffer.PutString(row, 0, $"Message: {world.CurrentMessage}");
        }
        private int RenderInstructions(World world, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== CONTROLS ===");
            var dynamicInstructions = world.AvailableInstructions
                .Where(i => i.Key != "WASD" && i.Key != "Q" && i.Key != "B" && i.Key != "ESC")
                .GroupBy(i => i.Key)
                .Select(g => g.First())
                .ToList();

            if (dynamicInstructions.Count == 0)
            {
                _buffer.PutString(startRow++, panelX, "(none)");
                return startRow + 1;
            }

            foreach (var instruction in dynamicInstructions)
            {
                _buffer.PutString(startRow++, panelX, instruction.ToDisplayText());
            }

            return startRow + 1;
        }

        private void RenderCombat(World world)
        {
            int row = 1;
            int col = 2;

            _buffer.PutString(row++, col, "=== COMBAT MODE ===");
            row++;

            var enemy = world.ActiveEnemy;
            if (enemy == null)
            {
                _buffer.PutString(row++, col, "No active enemy.");
                return;
            }

            _buffer.PutString(row++, col, $"Enemy: {enemy.Name}");
            _buffer.PutString(row++, col, $"Enemy HP: {enemy.Health}");
            _buffer.PutString(row++, col, $"Enemy ATK: {enemy.AttackMin}-{enemy.AttackMax}");
            _buffer.PutString(row++, col, $"Enemy ARM: {enemy.Armor}");
            row++;

            _buffer.PutString(row++, col, $"Player HP: {world.Player.Stats.Health}/{world.Player.Stats.MaxHealth}");
            _buffer.PutString(row++, col, $"Player ARM: {world.Player.Stats.Armor}");

            var left = world.Player.Inventory.LeftHand;
            var right = world.Player.Inventory.RightHand;
            var weapon = left ?? right;
            if (weapon != null)
            {
                _buffer.PutString(row++, col, $"Weapon: {weapon.Name}");
                _buffer.PutString(row++, col, $"DMG: {weapon.Value}  DUR: {weapon.Durability}");
            }
            else
            {
                _buffer.PutString(row++, col, "Weapon: none");
            }

            row += 2;
            _buffer.PutString(row++, col, "[1] Normal attack");
            _buffer.PutString(row++, col, "[2] Stealth attack");
            _buffer.PutString(row++, col, "[3] Magical attack");
        }

        private void RenderDeath(World world)
        {
            int row = 8;
            int col = 10;

            _buffer.PutString(row++, col, "╔════════════════════════╗");
            _buffer.PutString(row++, col, "║                        ║");
            _buffer.PutString(row++, col, "║       YOU LOST         ║");
            _buffer.PutString(row++, col, "║                        ║");
            _buffer.PutString(row++, col, "╚════════════════════════╝");
            row++;

            _buffer.PutString(row++, col, "Final Stats:");
            _buffer.PutString(row++, col + 2, $"Level reached: ???");
            _buffer.PutString(row++, col + 2, $"Coins collected: {world.Player.Stats.Coins}");
            _buffer.PutString(row++, col + 2, $"Gold collected: {world.Player.Stats.Gold}");
            row++;

            _buffer.PutString(row++, col, "═════════════════════════════");
            row++;

            _buffer.PutString(row++, col, "[R] Respawn and try again");
            _buffer.PutString(row++, col, "[Q] Quit to main menu");
        }

        private void RenderWeaponCrafting(World world, int craftingFirstSelection)
        {
            int row = 1;
            int col = 2;

            _buffer.PutString(row++, col, "=== WEAPON CRAFTING STATION ===");
            row++;

            _buffer.PutString(row++, col, "Available weapons in inventory:");
            row++;

            var weapons = new System.Collections.Generic.List<Items>();
            for (int i = 0; i < world.Player.Inventory.Count(); i++)
            {
                var item = world.Player.Inventory.GetItem(i);
                if (item != null && item.CanEquip)
                    weapons.Add(item);
            }

            if (weapons.Count == 0)
            {
                _buffer.PutString(row++, col, "No weapons to combine!");
                _buffer.PutString(row++, col, "Get back with [ESC]");
                return;
            }

            int selected = craftingFirstSelection < 0 ? 0 : craftingFirstSelection;
            for (int i = 0; i < weapons.Count; i++)
            {
                var weapon = weapons[i];
                string marker = i == selected ? ">>> " : "    ";
                _buffer.PutString(row++, col, $"{marker}[{i + 1}] {weapon.Name} (DMG: {weapon.Value})");
            }

            row += 2;
            _buffer.PutString(row++, col, "Controls:");
            _buffer.PutString(row++, col, "[W/↑] Next weapon");
            _buffer.PutString(row++, col, "[S/↓] Previous weapon");
            _buffer.PutString(row++, col, "[E] Combine selected with next");
            _buffer.PutString(row++, col, "[ESC] Leave station");
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