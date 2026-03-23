using System;
using System.Collections.Generic;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model
{
    public class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;
        private readonly List<string> _messageLog = new();  
        private readonly List<BuildInstruction> _availableInstructions = new();  

        public Player Player { get; }
        public IReadOnlyList<string> MessageLog => _messageLog;
        public IReadOnlyList<BuildInstruction> AvailableInstructions => _availableInstructions;
        public string CurrentMessage { get; private set; } = string.Empty; 

        public World()
            : this(new DungeonGroundsStrategy())
        {
        }

        // Zbuduj świat - najpierw mapa, potem gracz, potem monety i instrukcje
        public World(IDungeonStrategy strategy)
        {
            _tiles = new Tile[Height, Width];
            Player = new Player(new Vec2(1, 1));

            var context = strategy.Build(_tiles, Width, Height);
            SpawnPlayer(context);
            SpawnCurrencyItems(2, 2);  // Dla smaku
            BuildInstructionList(context);
        }

        // Dodaj wiadomość do dziennika - ostatnia wiadomość się wyświetla
        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            CurrentMessage = message;
            _messageLog.Add(message);

            const int maxMessages = 6;
            // Nie trzymaj zbyt dużo - to zajmuje pamięć
            if (_messageLog.Count > maxMessages)
                _messageLog.RemoveAt(0);
        }

        // Zbierz instrukcje z budowania i dodaj UI-owe
        private void BuildInstructionList(BuildContext context)
        {
            _availableInstructions.Clear();
            foreach (var instruction in context.Instructions)
                _availableInstructions.Add(instruction);

            AddUniqueInstruction(ControlsConfig.OpenInventory);
            AddUniqueInstruction(ControlsConfig.CloseInventory);
        }
        private void AddUniqueInstruction(BuildInstruction instruction)
        {
            foreach (var existing in _availableInstructions)
            {
                if (existing.Key == instruction.Key && existing.Description == instruction.Description)
                    return;
            }

            _availableInstructions.Add(instruction);
        } 
        private void SpawnPlayer(BuildContext context)
        {
            if (context.CentralRoom.HasValue)
            {
                var centralRoom = context.CentralRoom.Value;
                Player.MoveTo(new Vec2(centralRoom.CenterX, centralRoom.CenterY));
                return;
            }

            if (context.Rooms.Count > 0)
            {
                var startRoom = context.Rooms[0];
                Player.MoveTo(new Vec2(startRoom.CenterX, startRoom.CenterY));
                return;
            }

            _tiles[1, 1].IsWall = false;
            Player.MoveTo(new Vec2(1, 1));
        }

        private List<(int y, int x)> GetAvailableFloorTiles()
        {
            var availableTiles = new List<(int y, int x)>();

            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (!_tiles[y, x].IsWall && _tiles[y, x].Item == null)
                        availableTiles.Add((y, x));
                }
            }

            return availableTiles;
        }

        private void SpawnCurrencyItems(int coinCount, int goldCount)
        {
            var availableTiles = GetAvailableFloorTiles();

            void SpawnCurrency(int count, Func<Vec2, Items> factory)
            {
                int toSpawn = Math.Min(count, availableTiles.Count);

                for (int i = 0; i < toSpawn; i++)
                {
                    int pickIndex = Random.Shared.Next(availableTiles.Count);
                    var (y, x) = availableTiles[pickIndex];
                    availableTiles.RemoveAt(pickIndex);
                    _tiles[y, x].Item = factory(new Vec2(x, y));
                }
            }

            SpawnCurrency(coinCount, pos => ItemGenerator.CreateCoins(pos, Random.Shared.Next(5, 16)));
            SpawnCurrency(goldCount, pos => ItemGenerator.CreateGold(pos, Random.Shared.Next(1, 5)));
        }

        public Tile GetTile(int y, int x) => _tiles[y, x];

        public bool TryMovePlayer(int dx, int dy)
        {
            var next = Player.Pos.Add(dx, dy);

            if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
                return false;

            if (_tiles[next.Y, next.X].IsWall)
                return false;

            Player.MoveTo(next);
            return true;
        }

        public bool TryPickUpItem()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            var item = tile.Item;

            if (item == null)
            {
                AddMessage("Cannot pick up: no item here.");
                return false;
            }

            if (item.TryCollect(Player, out var collectMessage))
            {
                tile.Item = null;
                AddMessage(collectMessage);
                return true;
            }

            if (!item.CanEquip)
                return TryStoreFromTile(tile, item, $"Picked up {item.Name}.");

            if (TryEquipWeapon(item))
            {
                ApplyWeaponBonuses(item);
                tile.Item = null;
                AddMessage($"Equipped {item.Name}.");
                return true;
            }

            return TryStoreFromTile(tile, item, $"Stored {item.Name} in backpack.");
        }

        private bool TryStoreFromTile(Tile tile, Items item, string successMessage)
        {
            if (!Player.Inventory.AddToBackpack(item))
            {
                AddMessage("Backpack full.");
                return false;
            }

            tile.Item = null;
            AddMessage(successMessage);
            return true;
        }

        private bool TryEquipWeapon(Items item)
        {
            if (!item.CanEquip)
                return false;

            if (item.IsTwoHanded)
            {
                if (Player.Inventory.LeftHand != null || Player.Inventory.RightHand != null)
                    return false;

                return Player.Inventory.EquipItem(item, 0);
            }

            if (Player.Inventory.HasTwoHandedWeapon)
                return false;

            if (Player.Inventory.LeftHand == null)
                return Player.Inventory.EquipItem(item, 0);

            if (Player.Inventory.RightHand == null)
                return Player.Inventory.EquipItem(item, 1);

            return false;
        }

        public bool TryBackpackAction()
        {
            int count = Player.Inventory.Count();
            if (count == 0)
            {
                AddMessage("Backpack is empty.");
                return false;
            }

            return DropFromBackpack(count - 1);
        }

        public bool EquipFromBackpack(int index)
        {
            var selected = Player.Inventory.GetItem(index);
            if (selected == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            if (!selected.CanEquip)
            {
                AddMessage("Selected item cannot be equipped.");
                return false;
            }

            return selected.IsTwoHanded
                ? EquipTwoHandedFromBackpack(index)
                : EquipOneHandedFromBackpack(index);
        }

        private bool EquipTwoHandedFromBackpack(int index)
        {
            var left = Player.Inventory.LeftHand;
            var right = Player.Inventory.RightHand;

            int requiredSlots = (left != null ? 1 : 0) + (right != null ? 1 : 0);
            if (Player.Inventory.Count() - 1 + requiredSlots > Player.Inventory.MaxBackpackSize)
            {
                AddMessage("Backpack full.");
                return false;
            }

            var backpackItem = Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            var unequippedLeft = Player.Inventory.UnequipItem(0);
            var unequippedRight = Player.Inventory.UnequipItem(1);

            if (unequippedLeft != null)
                RemoveWeaponBonuses(unequippedLeft);
            if (unequippedRight != null)
                RemoveWeaponBonuses(unequippedRight);

            if (unequippedLeft != null)
                Player.Inventory.AddToBackpack(unequippedLeft);
            if (unequippedRight != null)
                Player.Inventory.AddToBackpack(unequippedRight);

            Player.Inventory.EquipItem(backpackItem, 0);
            ApplyWeaponBonuses(backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private bool EquipOneHandedFromBackpack(int index)
        {
            var backpackItem = Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            int handIndex = GetOneHandEquipSlot();
            var displaced = Player.Inventory.UnequipItem(handIndex);

            if (displaced != null)
            {
                RemoveWeaponBonuses(displaced);

                if (!Player.Inventory.AddToBackpack(displaced))
                {
                    ApplyWeaponBonuses(displaced);
                    Player.Inventory.EquipItem(displaced, handIndex);
                    Player.Inventory.AddToBackpack(backpackItem);
                    AddMessage("Backpack full.");
                    return false;
                }
            }

            if (!Player.Inventory.EquipItem(backpackItem, handIndex))
            {
                var rollback = Player.Inventory.TakeFromBackpack(Player.Inventory.Count() - 1);
                if (rollback != null)
                {
                    ApplyWeaponBonuses(rollback);
                    Player.Inventory.EquipItem(rollback, handIndex);
                }

                Player.Inventory.AddToBackpack(backpackItem);
                AddMessage("Cannot equip selected item.");
                return false;
            }

            ApplyWeaponBonuses(backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private int GetOneHandEquipSlot()
        {
            if (Player.Inventory.HasTwoHandedWeapon)
                return 0;

            if (Player.Inventory.LeftHand == null)
                return 0;

            if (Player.Inventory.RightHand == null)
                return 1;

            return 0;
        }

        public bool DropFromBackpack(int index)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.Item != null)
            {
                AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            var item = Player.Inventory.TakeFromBackpack(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        public bool UseFromBackpack(int index)
        {
            var item = Player.Inventory.GetItem(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            if (!item.TryUse(Player, out var useMessage))
            {
                AddMessage(useMessage);
                return false;
            }

            Player.Inventory.RemoveFromBackpack(index);
            AddMessage(useMessage);
            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.Item != null)
            {
                AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            var item = Player.Inventory.UnequipItem(handIndex);
            if (item == null)
            {
                AddMessage("Cannot drop: selected hand is empty.");
                return false;
            }

            RemoveWeaponBonuses(item);

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        private void ApplyWeaponBonuses(Items item)
        {
            item.ApplyEquipBonuses(Player.Stats);
        }

        private void RemoveWeaponBonuses(Items item)
        {
            item.RemoveEquipBonuses(Player.Stats);
        }
    }
}