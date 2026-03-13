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
        //tak ma sie renderowac imo goated
        //Mapa przed graczem aby bylo latwiej zeby gracz sie w scianie nie zrespil 
        //tym bardziej po patchu z korytarzami
        public void Render(World world)
        {
            _buffer.Clear();

            RenderMap(world);
            RenderPlayer(world.Player);
            RenderUI(world.Player, world);

            _buffer.Flush();
        }
        
        private void RenderMap(World world)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    var tile = world.GetTile(y, x);
                    char ch = tile.HasItem ? _config.ItemCharacter : GetTileCharacter(tile);
                    _buffer.PutChar(y, x, ch);
                }
            }
        }
        //TeRnArY OpErAtOr
        private char GetTileCharacter(Tile tile)
        {
            return tile.IsWall ? _config.WallCharacter : _config.FloorCharacter;
        }
        //Trzeba gracza polozyc gdzies tez nie
        private void RenderPlayer(Player player)
        {
            _buffer.PutChar(player.Pos.Y, player.Pos.X, _config.PlayerCharacter);
        }
        private void RenderUI(Player player, World world)
        {
            int currentRow = 0;
            int panelX = World.Width + 2;

            int x = player.Pos.X;
            int y = player.Pos.Y;
            var tile = world.GetTile(y, x);

            currentRow = RenderHeader(panelX, currentRow);
            currentRow = RenderControls(panelX, currentRow);
            if (tile.HasItem)
            {
                currentRow = RenderPickupDrop(panelX, currentRow);     
            }
            currentRow = RenderCurrency(player, panelX, currentRow);
            currentRow = RenderStats(player, panelX, currentRow);
            currentRow = RenderEquipment(player, panelX, currentRow);
            RenderQuitOption(panelX, currentRow);
        }
        //nazwa gry i tego co to stworzyl hahaha
        private int RenderHeader(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "RPG Game or sum");
            _buffer.PutString(startRow++, panelX, "by Faldekk");
            return startRow + 1;
        }
        //ej ale ty wiesz jak chodzic cn?
        private int RenderControls(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== CONTROLS ===");
            _buffer.PutString(startRow++, panelX, "WASD - move");
            _buffer.PutString(startRow++, panelX, "E - pick up");
            _buffer.PutString(startRow++, panelX, "X - swap hands");
           
            return startRow + 1;
        }
        private int RenderPickupDrop(int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "1 - drop left");
            _buffer.PutString(startRow++, panelX, "2 - drop right");
            _buffer.PutString(startRow++, panelX, "WASD - move");
            return startRow+1;
        }
        //Money money money
        private int RenderCurrency(Player player, int panelX, int startRow)
        {
            _buffer.PutString(startRow++, panelX, "=== MONEY ===");
            _buffer.PutString(startRow++, panelX, $"Coins: {player.Stats.Coins}");
            _buffer.PutString(startRow++, panelX, $"Gold: {player.Stats.Gold}");
            return startRow + 1;
        }
        // wypisz statystyki
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
        //Pomocy pisalem to 3 h  ale wypisuje itemy
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
        // bardzo wazne
        private void RenderQuitOption(int panelX, int startRow)
        {
            _buffer.PutString(startRow, panelX, "Q - quit game");
        }
    }
    //no i taka klasa zeby bylo latwiej dodawac rzeczy 
    public class RenderConfig
    {
        public int PanelWidth { get; set; } = 50;
        public char WallCharacter { get; set; } = '█';
        public char FloorCharacter { get; set; } = ' ';
        public char PlayerCharacter { get; set; } = '¶';
        public char ItemCharacter { get; set; } = 'x';
        //public char HealCharacter { get; set; } = '+';
        //nie mam czasu tego zmieniac zmienie to na nastepny raz
    }
}