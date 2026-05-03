using System;
using System.Collections.Generic;
using System.Linq;
using RPG_GAME.App.Logging;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;
using RPG_GAME.Model.DungeonThemes;
using RPG_GAME.UI;

namespace RPG_GAME.Model
{
    public partial class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;
        private readonly List<BuildInstruction> _availableInstructions = new();
        private readonly CombatEngine _combatEngine = new();
        private readonly Dictionary<string, IAttackType> _attackTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["normal"] = new NormalAttackType(),
            ["stealth"] = new StealthAttackType(),
            ["magical"] = new MagicalAttackType()
        };
        private readonly IDungeonTheme? _theme;
        private readonly IDungeonStrategy _strategy;
        private readonly IItemPool? _itemPool;
        private readonly IEnemyFactory? _enemyFactory;

        public Player Player { get; private set; }
        public Enemy? ActiveEnemy { get; private set; }
        public bool IsCombatActive => ActiveEnemy != null;
        public bool IsExitRequested { get; private set; }
        public IReadOnlyList<string> MessageLog => GameLog.JournalEntries.Select(entry => entry.Message).ToList();
        public IReadOnlyList<BuildInstruction> AvailableInstructions => _availableInstructions;
        public string CurrentMessage { get; private set; } = string.Empty;

        public World()
            : this(DungeonThemeFactory.CreateRandom())
        {
        }

        public World(IDungeonTheme theme)
            : this(theme, theme.CreateDungeonStrategy(), theme.CreateItemPool(), theme.CreateEnemyFactory())
        {
        }

        public World(IDungeonStrategy strategy)
            : this(null, strategy, null, null)
        {
        }

        private World(IDungeonTheme? theme, IDungeonStrategy strategy, IItemPool? itemPool, IEnemyFactory? enemyFactory)
        {
            _tiles = new Tile[Height, Width];
            _theme = theme;
            _strategy = strategy;
            _itemPool = itemPool;
            _enemyFactory = enemyFactory;
            Player = new Player(new Vec2(1, 1));
            Initialize();
        }

        private void Initialize()
        {
            _availableInstructions.Clear();
            CurrentMessage = string.Empty;
            ActiveEnemy = null;
            IsExitRequested = false;

            Player = new Player(new Vec2(1, 1));

            var context = _strategy.Build(_tiles, Width, Height);
            SpawnPlayer(context);

            if (_theme != null && _itemPool != null && _enemyFactory != null)
            {
                SpawnThemeContent(context);
                AddMessage(_theme.IntroMessage);
            }
            else
            {
                SpawnCurrencyItems(2, 2);
            }

            BuildInstructionList(context);
        }

        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            CurrentMessage = message;
            GameLog.Info(message);
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

        public Tile GetTile(int y, int x) => _tiles[y, x];

        private void SpawnPlayer(BuildContext context)
        {
            if (context.CentralRoom.HasValue)
            {
                var centralRoom = context.CentralRoom.Value;
                this.Player.MoveTo(new Vec2(centralRoom.CenterX, centralRoom.CenterY));
                return;
            }

            if (context.Rooms.Count > 0)
            {
                var startRoom = context.Rooms[0];
                this.Player.MoveTo(new Vec2(startRoom.CenterX, startRoom.CenterY));
                return;
            }

            this._tiles[1, 1].IsWall = false;
            this.Player.MoveTo(new Vec2(1, 1));
        }

        private List<(int y, int x)> GetAvailableFloorTiles()
        {
            var availableTiles = new List<(int y, int x)>();

            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (!this._tiles[y, x].IsWall && this._tiles[y, x].Item == null && this._tiles[y, x].Enemy == null)
                        availableTiles.Add((y, x));
                }
            }

            return availableTiles;
        }

        private void SpawnThemeContent(BuildContext context)
        {
            var availableTiles = GetAvailableFloorTiles();
            SpawnArtifact(availableTiles);
            SpawnThemeItems(availableTiles, 10);
            SpawnThemeEnemies(availableTiles, 7);
        }

        private void SpawnArtifact(List<(int y, int x)> availableTiles)
        {
            if (this._theme == null || availableTiles.Count == 0)
                return;

            int pickIndex = Random.Shared.Next(availableTiles.Count);
            var (y, x) = availableTiles[pickIndex];
            availableTiles.RemoveAt(pickIndex);
            this._tiles[y, x].Item = this._theme.CreateArtifact(new Vec2(x, y));
        }

        private void SpawnThemeItems(List<(int y, int x)> availableTiles, int count)
        {
            if (this._itemPool == null)
                return;

            int toSpawn = Math.Min(count, availableTiles.Count);
            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);
                this._tiles[y, x].Item = this._itemPool.CreateRandomItem(new Vec2(x, y));
            }
        }

        private void SpawnThemeEnemies(List<(int y, int x)> availableTiles, int count)
        {
            if (this._enemyFactory == null)
                return;

            int toSpawn = Math.Min(count, availableTiles.Count);
            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);
                this._tiles[y, x].Enemy = this._enemyFactory.CreateRandomEnemy(new Vec2(x, y));
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
                    this._tiles[y, x].Item = factory(new Vec2(x, y));
                }
            }

            SpawnCurrency(coinCount, pos => ItemGenerator.CreateCoins(pos, Random.Shared.Next(5, 16)));
            SpawnCurrency(goldCount, pos => ItemGenerator.CreateGold(pos, Random.Shared.Next(1, 5)));
        }

        public bool TryMovePlayer(int dx, int dy)
        {
            var next = this.Player.Pos.Add(dx, dy);

            if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
            {
                this.AddMessage("Walking into a wall.");
                return false;
            }

            var targetTile = this._tiles[next.Y, next.X];
            if (targetTile.IsWall)
            {
                this.AddMessage("Walking into a wall.");
                return false;
            }

            if (targetTile.Enemy != null)
            {
                StartCombat(targetTile.Enemy);
                return true;
            }

            this.Player.MoveTo(next);
            return true;
        }

        public bool TryPickUpItem()
        {
            var tile = this.GetTile(this.Player.Pos.Y, this.Player.Pos.X);
            var item = tile.Item;

            if (item == null)
            {
                this.AddMessage("Cannot pick up: no item here.");
                return false;
            }

            if (item.TryCollect(this.Player, out var collectMessage))
            {
                tile.Item = null;
                this.AddMessage(collectMessage);
                this.AddMessage($"Picking up item: {item.Name}.");
                return true;
            }

            if (!item.CanEquip)
                return TryStoreFromTile(tile, item, $"Picked up {item.Name}.");

            if (TryEquipWeapon(item))
            {
                ApplyWeaponBonuses(item);
                tile.Item = null;
                this.AddMessage($"Equipped {item.Name}.");
                return true;
            }

            return TryStoreFromTile(tile, item, $"Stored {item.Name} in backpack.");
        }

        private bool TryStoreFromTile(Tile tile, Items item, string successMessage)
        {
            if (!this.Player.Inventory.AddToBackpack(item))
            {
                this.AddMessage("Backpack full.");
                return false;
            }

            tile.Item = null;
            this.AddMessage(successMessage);
            return true;
        }

        private bool TryEquipWeapon(Items item)
        {
            if (!item.CanEquip)
                return false;

            if (item.IsTwoHanded)
            {
                if (this.Player.Inventory.LeftHand != null || this.Player.Inventory.RightHand != null)
                    return false;

                return this.Player.Inventory.EquipItem(item, 0);
            }

            if (this.Player.Inventory.HasTwoHandedWeapon)
                return false;

            if (this.Player.Inventory.LeftHand == null)
                return this.Player.Inventory.EquipItem(item, 0);

            if (this.Player.Inventory.RightHand == null)
                return this.Player.Inventory.EquipItem(item, 1);

            return false;
        }

        public bool TryBackpackAction()
        {
            int count = this.Player.Inventory.Count();
            if (count == 0)
            {
                this.AddMessage("Backpack is empty.");
                return false;
            }

            return DropFromBackpack(count - 1);
        }

        public bool EquipFromBackpack(int index)
        {
            var selected = this.Player.Inventory.GetItem(index);
            if (selected == null)
            {
                this.AddMessage("No item selected.");
                return false;
            }

            if (!selected.CanEquip)
            {
                this.AddMessage("Selected item cannot be equipped.");
                return false;
            }

            return selected.IsTwoHanded
                ? EquipTwoHandedFromBackpack(index)
                : EquipOneHandedFromBackpack(index);
        }

        private bool EquipTwoHandedFromBackpack(int index)
        {
            var left = this.Player.Inventory.LeftHand;
            var right = this.Player.Inventory.RightHand;

            int requiredSlots = (left != null ? 1 : 0) + (right != null ? 1 : 0);
            if (this.Player.Inventory.Count() - 1 + requiredSlots > this.Player.Inventory.MaxBackpackSize)
            {
                this.AddMessage("Backpack full.");
                return false;
            }

            var backpackItem = this.Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                this.AddMessage("Cannot equip selected item.");
                return false;
            }

            var unequippedLeft = this.Player.Inventory.UnequipItem(0);
            var unequippedRight = this.Player.Inventory.UnequipItem(1);

            if (unequippedLeft != null)
                RemoveWeaponBonuses(unequippedLeft);
            if (unequippedRight != null)
                RemoveWeaponBonuses(unequippedRight);

            if (unequippedLeft != null)
                this.Player.Inventory.AddToBackpack(unequippedLeft);
            if (unequippedRight != null)
                this.Player.Inventory.AddToBackpack(unequippedRight);

            this.Player.Inventory.EquipItem(backpackItem, 0);
            ApplyWeaponBonuses(backpackItem);
            this.AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private bool EquipOneHandedFromBackpack(int index)
        {
            var backpackItem = this.Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                this.AddMessage("Cannot equip selected item.");
                return false;
            }

            int handIndex = GetOneHandEquipSlot();
            var displaced = this.Player.Inventory.UnequipItem(handIndex);

            if (displaced != null)
            {
                RemoveWeaponBonuses(displaced);

                if (!this.Player.Inventory.AddToBackpack(displaced))
                {
                    ApplyWeaponBonuses(displaced);
                    this.Player.Inventory.EquipItem(displaced, handIndex);
                    this.Player.Inventory.AddToBackpack(backpackItem);
                    this.AddMessage("Backpack full.");
                    return false;
                }
            }

            if (!this.Player.Inventory.EquipItem(backpackItem, handIndex))
            {
                var rollback = this.Player.Inventory.TakeFromBackpack(this.Player.Inventory.Count() - 1);
                if (rollback != null)
                {
                    ApplyWeaponBonuses(rollback);
                    this.Player.Inventory.EquipItem(rollback, handIndex);
                }

                this.Player.Inventory.AddToBackpack(backpackItem);
                this.AddMessage("Cannot equip selected item.");
                return false;
            }

            ApplyWeaponBonuses(backpackItem);
            this.AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private int GetOneHandEquipSlot()
        {
            if (this.Player.Inventory.HasTwoHandedWeapon)
                return 0;

            if (this.Player.Inventory.LeftHand == null)
                return 0;

            if (this.Player.Inventory.RightHand == null)
                return 1;

            return 0;
        }

        public bool DropFromBackpack(int index)
        {
            var tile = this.GetTile(this.Player.Pos.Y, this.Player.Pos.X);
            if (tile.Item != null)
            {
                this.AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            if (tile.Enemy != null)
            {
                this.AddMessage("Cannot drop during enemy occupation.");
                return false;
            }

            var item = this.Player.Inventory.TakeFromBackpack(index);
            if (item == null)
            {
                this.AddMessage("No item selected.");
                return false;
            }

            item.Position = new Tuple<int, int>(this.Player.Pos.X, this.Player.Pos.Y);
            tile.Item = item;
            this.AddMessage($"Dropped {item.Name}.");
            return true;
        }

        public bool UseFromBackpack(int index)
        {
            var item = this.Player.Inventory.GetItem(index);
            if (item == null)
            {
                this.AddMessage("No item selected.");
                return false;
            }

            if (!item.TryUse(this.Player, out var useMessage))
            {
                this.AddMessage(useMessage);
                return false;
            }

            this.Player.Inventory.RemoveFromBackpack(index);
            this.AddMessage(useMessage);
            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            var tile = this.GetTile(this.Player.Pos.Y, this.Player.Pos.X);
            if (tile.Item != null)
            {
                this.AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            if (tile.Enemy != null)
            {
                this.AddMessage("Cannot drop during enemy occupation.");
                return false;
            }

            var item = this.Player.Inventory.UnequipItem(handIndex);
            if (item == null)
            {
                this.AddMessage("Cannot drop: selected hand is empty.");
                return false;
            }

            RemoveWeaponBonuses(item);

            item.Position = new Tuple<int, int>(this.Player.Pos.X, this.Player.Pos.Y);
            tile.Item = item;
            this.AddMessage($"Dropped {item.Name}.");
            return true;
        }

        private void ApplyWeaponBonuses(Items item) => item.ApplyEquipBonuses(this.Player.Stats);

        private void RemoveWeaponBonuses(Items item) => item.RemoveEquipBonuses(this.Player.Stats);

        public bool CraftArmorFromJunk()
        {
            var junkIndexes = new List<int>();
            for (int i = 0; i < this.Player.Inventory.Count(); i++)
            {
                var item = this.Player.Inventory.GetItem(i);
                if (item != null && item.Type.Equals("Junk", StringComparison.OrdinalIgnoreCase))
                    junkIndexes.Add(i);
            }

            if (junkIndexes.Count < 2)
            {
                this.AddMessage("Need 2 junk items to craft armor.");
                return false;
            }

            junkIndexes.Sort((a, b) => b.CompareTo(a));
            this.Player.Inventory.RemoveFromBackpack(junkIndexes[0]);
            this.Player.Inventory.RemoveFromBackpack(junkIndexes[1]);
            this.Player.Stats.Armor += 2;
            this.AddMessage($"Crafted armor from junk. Armor is now {this.Player.Stats.Armor}.");
            return true;
        }

        public bool TryCombatRound(string attackKey)
        {
            if (this.ActiveEnemy == null)
            {
                this.AddMessage("No active enemy.");
                return false;
            }

            if (!this._attackTypes.TryGetValue(attackKey, out var attackType))
            {
                this.AddMessage("Unknown attack style.");
                return false;
            }

            var weapon = ResolveActiveWeapon();
            var enemyBeforeRound = this.ActiveEnemy;
            var result = this._combatEngine.ExecuteRound(this.Player, enemyBeforeRound, attackType, weapon);
            this.AddMessage(result.Summary);
            this.AddMessage($"Player attack dealt {result.PlayerDamageDealt} damage.");
            this.AddMessage($"Enemy attack dealt {result.EnemyDamageDealt} damage.");

            if (result.EnemyDefeated)
            {
                var defeatedPos = enemyBeforeRound.Position;
                RemoveEnemyFromMap(enemyBeforeRound);
                SpawnVictoryLoot(defeatedPos);
                this.Player.Heal(50);
                this.ActiveEnemy = null;
                this.AddMessage("Enemy removed from map.");
                this.AddMessage($"Enemy defeated: {enemyBeforeRound.Name}.");
            }

            if (result.PlayerDefeated)
                this.AddMessage("You died. Game over.");

            return true;
        }

        private Items? ResolveActiveWeapon()
        {
            if (this.Player.Inventory.LeftHand != null)
                return this.Player.Inventory.LeftHand;

            return this.Player.Inventory.RightHand;
        }

        private void StartCombat(Enemy enemy)
        {
            this.ActiveEnemy = enemy;
            this.AddMessage($"Encounter: {enemy.Name} | HP: {enemy.Health} | ATK: {enemy.AttackMin}-{enemy.AttackMax} | ARM: {enemy.Armor}");
        }

        private void RemoveEnemyFromMap(Enemy enemy)
        {
            var pos = enemy.Position;
            if (pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height)
                this._tiles[pos.Y, pos.X].Enemy = null;
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

                    if (this._tiles[y, x].IsWall || this._tiles[y, x].Enemy != null || this._tiles[y, x].Item != null)
                        continue;

                    candidates.Add(new Vec2(x, y));
                }
            }

            if (candidates.Count == 0)
                return;

            if (this._itemPool != null)
            {
                PlaceLoot(candidates, pos => this._itemPool.CreateRandomItem(pos));
                return;
            }

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

            this._tiles[pos.Y, pos.X].Item = factory(pos);
        }

        public Items? CombineWeapons(Items weapon1, Items weapon2)
        {
            if (!weapon1.CanEquip || !weapon2.CanEquip)
            {
                this.AddMessage("Can only combine two weapons.");
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

            this.AddMessage($"Combined into: {combined.Name} (DMG: {combined.Value})");
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
            var tile = this.GetTile(this.Player.Pos.Y, this.Player.Pos.X);
            return tile.IsCraftingStation;
        }

        public void Stop()
        {
            this.IsExitRequested = true;
        }

        public void Respawn()
        {
            this.Initialize();
        }
    }
}