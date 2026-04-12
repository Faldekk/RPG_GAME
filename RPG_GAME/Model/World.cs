using System;
using System.Collections.Generic;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;
using RPG_GAME.UI;

namespace RPG_GAME.Model
{
    public class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;
        private readonly List<string> _messageLog = new();
        private readonly List<BuildInstruction> _availableInstructions = new();
        private readonly CombatEngine _combatEngine = new();
        private readonly Dictionary<string, IAttackType> _attackTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["normal"] = new NormalAttackType(),
            ["stealth"] = new StealthAttackType(),
            ["magical"] = new MagicalAttackType()
        };

        public Player Player { get; private set; }
        public Enemy? ActiveEnemy { get; private set; }
        public bool IsCombatActive => ActiveEnemy != null;
        public bool IsExitRequested { get; private set; }
        public IReadOnlyList<string> MessageLog => _messageLog;
        public IReadOnlyList<BuildInstruction> AvailableInstructions => _availableInstructions;
        public string CurrentMessage { get; private set; } = string.Empty;

        public World()
            : this(new DungeonGroundsStrategy())
        {
        }

        public World(IDungeonStrategy strategy)
        {
            _tiles = new Tile[Height, Width];
            Player = new Player(new Vec2(1, 1));
            Initialize(strategy);
        }

        private void Initialize(IDungeonStrategy strategy)
        {
            _messageLog.Clear();
            _availableInstructions.Clear();
            CurrentMessage = string.Empty;
            ActiveEnemy = null;
            IsExitRequested = false;

            Player = new Player(new Vec2(1, 1));

            var context = strategy.Build(_tiles, Width, Height);
            SpawnPlayer(context);
            SpawnCurrencyItems(2, 2);  // Dla smaku
            BuildInstructionList(context);
        }
        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            CurrentMessage = message;
            _messageLog.Add(message);

            const int maxMessages = 6;
            if (_messageLog.Count > maxMessages)
                _messageLog.RemoveAt(0);
        }
        private void BuildInstructionList(BuildContext context)
        {
            _availableInstructions.Clear();
            foreach (var instruction in context.Instructions)
                _availableInstructions.Add(instruction);

            AddUniqueInstruction(ControlsConfig.OpenInventory);
            AddUniqueInstruction(ControlsConfig.CloseInventory);
            AddUniqueInstruction(ControlsConfig.InventoryCraftArmor);
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
                    if (!_tiles[y, x].IsWall && _tiles[y, x].Item == null && _tiles[y, x].Enemy == null)
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

            var targetTile = _tiles[next.Y, next.X];
            if (targetTile.IsWall)
                return false;

            if (targetTile.Enemy != null)
            {
                StartCombat(targetTile.Enemy);
                return true;
            }

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

            if (tile.Enemy != null)
            {
                AddMessage("Cannot drop during enemy occupation.");
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

            if (tile.Enemy != null)
            {
                AddMessage("Cannot drop during enemy occupation.");
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

        public bool CraftArmorFromJunk()
        {
            var junkIndexes = new List<int>();
            for (int i = 0; i < Player.Inventory.Count(); i++)
            {
                var item = Player.Inventory.GetItem(i);
                if (item != null && item.Type.Equals("Junk", StringComparison.OrdinalIgnoreCase))
                    junkIndexes.Add(i);
            }

            if (junkIndexes.Count < 2)
            {
                AddMessage("Need 2 junk items to craft armor.");
                return false;
            }

            junkIndexes.Sort((a, b) => b.CompareTo(a));
            Player.Inventory.RemoveFromBackpack(junkIndexes[0]);
            Player.Inventory.RemoveFromBackpack(junkIndexes[1]);
            Player.Stats.Armor += 2;
            AddMessage($"Crafted armor from junk. Armor is now {Player.Stats.Armor}.");
            return true;
        }

        public bool TryCombatRound(string attackKey)
        {
            if (ActiveEnemy == null)
            {
                AddMessage("No active enemy.");
                return false;
            }

            if (!_attackTypes.TryGetValue(attackKey, out var attackType))
            {
                AddMessage("Unknown attack style.");
                return false;
            }

            var weapon = ResolveActiveWeapon();
            var result = _combatEngine.ExecuteRound(Player, ActiveEnemy, attackType, weapon);
            AddMessage(result.Summary);

            if (result.EnemyDefeated)
            {
                var defeatedPos = ActiveEnemy.Position;
                RemoveEnemyFromMap(ActiveEnemy);
                SpawnVictoryLoot(defeatedPos);
                Player.Heal(50);
                ActiveEnemy = null;
                AddMessage("Enemy removed from map.");
            }

            if (result.PlayerDefeated)
                AddMessage("You died. Game over.");

            return true;
        }

        private Items? ResolveActiveWeapon()
        {
            if (Player.Inventory.LeftHand != null)
                return Player.Inventory.LeftHand;

            return Player.Inventory.RightHand;
        }

        private void StartCombat(Enemy enemy)
        {
            ActiveEnemy = enemy;
            AddMessage($"Encounter: {enemy.Name} | HP: {enemy.Health} | ATK: {enemy.AttackMin}-{enemy.AttackMax} | ARM: {enemy.Armor}");
        }

        private void RemoveEnemyFromMap(Enemy enemy)
        {
            var pos = enemy.Position;
            if (pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height)
                _tiles[pos.Y, pos.X].Enemy = null;
        }

        private void SpawnVictoryLoot(Vec2 center)
        {
            var candidates = new List<Vec2>();

            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    int x = center.X + dx;
                    int y = center.Y + dy;

                    if (x < 1 || x >= Width - 1 || y < 1 || y >= Height - 1)
                        continue;

                    if (_tiles[y, x].IsWall || _tiles[y, x].Enemy != null || _tiles[y, x].Item != null)
                        continue;

                    candidates.Add(new Vec2(x, y));
                }
            }

            if (candidates.Count == 0)
                return;

            PlaceLoot(candidates, pos => ItemGenerator.CreateCoins(pos, Random.Shared.Next(6, 16)));
            PlaceLoot(candidates, pos => ItemGenerator.CreateGold(pos, Random.Shared.Next(1, 5)));
            PlaceLoot(candidates, pos => WeaponGenerator.GenerateRandomWeapon(pos));
        }

        private void PlaceLoot(List<Vec2> candidates, Func<Vec2, Items> factory)
        {
            if (candidates.Count == 0)
                return;

            int index = Random.Shared.Next(candidates.Count);
            var pos = candidates[index];
            candidates.RemoveAt(index);

            _tiles[pos.Y, pos.X].Item = factory(pos);
        }

        public Items? CombineWeapons(Items weapon1, Items weapon2)
        {
            if (!weapon1.CanEquip || !weapon2.CanEquip)
            {
                AddMessage("Can only combine two weapons.");
                return null;
            }

            var combinedName = $"{weapon1.Name} & {weapon2.Name}";
            var combinedValue = weapon1.Value + weapon2.Value;

            var strengthBonus = GetStatBonus(weapon1, "Strength") + GetStatBonus(weapon2, "Strength");
            var dexterityBonus = GetStatBonus(weapon1, "Dexterity") + GetStatBonus(weapon2, "Dexterity");
            var aggressionBonus = GetStatBonus(weapon1, "Agression") + GetStatBonus(weapon2, "Agression");
            var wisdomBonus = GetStatBonus(weapon1, "Wisdom") + GetStatBonus(weapon2, "Wisdom");
            var luckBonus = GetStatBonus(weapon1, "Luck") + GetStatBonus(weapon2, "Luck");

            var combined = new WeaponItem(
                combinedName,
                "Combined",
                combinedValue,
                false,
                strengthBonus,
                dexterityBonus,
                aggressionBonus,
                wisdomBonus,
                luckBonus,
                weapon1.GetWeaponCategory()
            );

            AddMessage($"Combined into: {combined.Name} (DMG: {combined.Value})");
            return combined;
        }

        private int GetStatBonus(Items weapon, string statName)
        {
            var stats = new PlayerStats();
            weapon.ApplyEquipBonuses(stats);

            return statName switch
            {
                "Strength" => stats.Strength - 10,
                "Dexterity" => stats.Dexterity - 10,
                "Agression" => stats.Aggression - 25,
                "Wisdom" => stats.Wisdom,
                "Luck" => stats.Luck - 50,
                _ => 0
            };
        }

        public bool IsAtCraftingStation()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            return tile.IsCraftingStation;
        }

        public void Stop()
        {
            IsExitRequested = true;
        }

        public void Respawn()
        {
            Initialize(new DungeonGroundsStrategy());
        }
    }
}