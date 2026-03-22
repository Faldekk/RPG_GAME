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

        public Player Player { get; }
        public IReadOnlyList<string> MessageLog => _messageLog;

        public World()
            : this(new DungeonGroundsStrategy())
        {
        }

        public World(IDungeonStrategy strategy)
        {
            _tiles = new Tile[Height, Width];
            Player = new Player(new Vec2(1, 1));

            var context = strategy.Build(_tiles, Width, Height);
            SpawnPlayer(context);
            SpawnRandomWeapons(10);
            SpawnRandomItems(8);
            SpawnCurrencyItems(2, 2);
        }

        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            _messageLog.Add(message);

            const int maxMessages = 6;
            if (_messageLog.Count > maxMessages)
                _messageLog.RemoveAt(0);
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

        private void SpawnRandomWeapons(int count)
        {
            var availableTiles = GetAvailableFloorTiles();
            int toSpawn = Math.Min(count, availableTiles.Count);

            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);

                _tiles[y, x].Item = WeaponGenerator.GenerateRandomWeapon(x, y);
            }
        }

        public void SpawnRandomItems(int count)
        {
            var availableTiles = GetAvailableFloorTiles();
            int toSpawn = Math.Min(count, availableTiles.Count);

            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);

                _tiles[y, x].Item = ItemGenerator.GenerateRandomNonWeapon(new Vec2(x, y));
            }
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

            if (TryCollectCurrency(item))
            {
                tile.Item = null;
                AddMessage($"Collected {item.Value} {item.Name}.");
                return true;
            }

            if (!IsWeapon(item))
            {
                Player.Inventory.AddToBackpack(item);
                tile.Item = null;
                AddMessage($"Picked up {item.Name}.");
                return true;
            }

            if (item.IsTwoHanded && (Player.Inventory.LeftHand != null || Player.Inventory.RightHand != null))
            {
                AddMessage("Cannot equip 2H weapon while holding another weapon.");
                return false;
            }

            int handIndex = GetPreferredHand(item);

            var displaced = Player.Inventory.UnequipItem(handIndex);
            if (displaced != null && IsWeapon(displaced))
                RemoveWeaponBonuses(displaced);

            if (!Player.Inventory.EquipItem(item, handIndex))
            {
                if (displaced != null)
                {
                    Player.Inventory.EquipItem(displaced, handIndex);
                    if (IsWeapon(displaced))
                        ApplyWeaponBonuses(displaced);
                }

                AddMessage("Cannot equip item.");
                return false;
            }

            ApplyWeaponBonuses(item);

            tile.Item = displaced;
            if (displaced != null)
                displaced.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);

            AddMessage(displaced == null
                ? $"Equipped {item.Name}."
                : $"Equipped {item.Name} and dropped {displaced.Name}.");

            return true;
        }

        public bool TryBackpackAction()
        {
            if (Player.Inventory.Backpack.Count == 0)
            {
                AddMessage("Backpack is empty.");
                return false;
            }

            int lastIndex = Player.Inventory.Backpack.Count - 1;
            var lastItem = Player.Inventory.Backpack[lastIndex];

            if (IsWeapon(lastItem) && TryEquipWeapon(lastItem))
            {
                Player.Inventory.RemoveFromBackpack(lastIndex);
                ApplyWeaponBonuses(lastItem);
                AddMessage($"Equipped {lastItem.Name} from backpack.");
                return true;
            }

            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.Item != null)
            {
                AddMessage("Cannot drop backpack item: tile already has an item.");
                return false;
            }

            var removed = Player.Inventory.RemoveFromBackpack(lastIndex);
            if (removed == null)
            {
                AddMessage("Backpack action failed.");
                return false;
            }

            removed.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = removed;
            AddMessage($"Dropped {removed.Name} from backpack.");
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

            if (IsWeapon(item))
                RemoveWeaponBonuses(item);

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        private int GetPreferredHand(Items item)
        {
            if (item.IsTwoHanded)
                return 0;

            if (Player.Inventory.LeftHand == null)
                return 0;

            if (Player.Inventory.RightHand == null)
                return 1;

            return 0;
        }

        private bool TryEquipWeapon(Items item)
        {
            if (item.IsTwoHanded)
            {
                if (Player.Inventory.LeftHand != null || Player.Inventory.RightHand != null)
                    return false;

                return Player.Inventory.EquipItem(item, 0);
            }

            if (Player.Inventory.LeftHand == null)
                return Player.Inventory.EquipItem(item, 0);

            if (Player.Inventory.RightHand == null)
                return Player.Inventory.EquipItem(item, 1);

            return false;
        }

        private void ApplyWeaponBonuses(Items weapon)
        {
            var (strength, dexterity, aggression, wisdom, luck) = GetWeaponStatBonuses(weapon);
            Player.Stats.ModifyStat("Strength", strength);
            Player.Stats.ModifyStat("Dexterity", dexterity);
            Player.Stats.ModifyStat("Agression", aggression);
            Player.Stats.ModifyStat("Wisdom", wisdom);
            Player.Stats.ModifyStat("Luck", luck);
        }

        private void RemoveWeaponBonuses(Items weapon)
        {
            var (strength, dexterity, aggression, wisdom, luck) = GetWeaponStatBonuses(weapon);
            Player.Stats.ModifyStat("Strength", -strength);
            Player.Stats.ModifyStat("Dexterity", -dexterity);
            Player.Stats.ModifyStat("Agression", -aggression);
            Player.Stats.ModifyStat("Wisdom", -wisdom);
            Player.Stats.ModifyStat("Luck", -luck);
        }

        private static (int strength, int dexterity, int aggression, int wisdom, int luck) GetWeaponStatBonuses(Items weapon)
        {
            int strength = 0;
            int dexterity = 0;
            int aggression = 0;
            int wisdom = 0;
            int luck = 0;

            if (weapon.Type.Equals("Melee", StringComparison.OrdinalIgnoreCase))
            {
                strength += Math.Max(1, weapon.Value / 4);
                aggression += 1;
                if (weapon.IsTwoHanded)
                    strength += 2;
            }

            if (weapon.Type.Equals("Magic", StringComparison.OrdinalIgnoreCase))
            {
                wisdom += Math.Max(1, weapon.Value / 5);
                luck += 1;
                if (weapon.IsTwoHanded)
                    wisdom += 2;
            }

            if (!weapon.IsTwoHanded)
                dexterity += 1;

            return (strength, dexterity, aggression, wisdom, luck);
        }

        private bool TryCollectCurrency(Items item)
        {
            if (!item.Type.Equals("Currency", StringComparison.OrdinalIgnoreCase))
                return false;

            if (item.Name.Equals("Coins", StringComparison.OrdinalIgnoreCase))
            {
                Player.Stats.ModifyCurrency("Coins", item.Value);
                return true;
            }

            if (item.Name.Equals("Gold", StringComparison.OrdinalIgnoreCase))
            {
                Player.Stats.ModifyCurrency("Gold", item.Value);
                return true;
            }

            return false;
        }

        private static bool IsWeapon(Items item)
        {
            return item.Type.Equals("Melee", StringComparison.OrdinalIgnoreCase) ||
                   item.Type.Equals("Magic", StringComparison.OrdinalIgnoreCase);
        }
    }
}