using System;
using System.Collections.Generic;
using System.Linq;
using RPG_GAME.App.Logging;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;
using RPG_GAME.Model.DungeonThemes;
using RPG_GAME.UI;
using RPG_GAME.Model.Events;

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

        public static Tile[,] TilesForServices { get; private set; }

        private NoisePublisher _noisePublisher;
        private Sound.ISoundPropagation _soundPropagation;
        private Dictionary<string, SpeciesDeathPublisher> _speciesPublishers;
        private List<EnemySubscriptions> _enemySubscriptions;
        private Model.Movement.IEnemyMovementStrategy _enemyMovementStrategy;

        public INoiseEmitter NoiseEmitter { get; private set; }

        private readonly Dictionary<Enemy, EnemySubscriptions> _enemySubscriptionMap = new();
        private readonly Dictionary<Enemy, EnemySpecies> _enemySpecies = new();

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
            TilesForServices = _tiles;
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

            // initialize event systems early so factories and spawn code can use them
            _noisePublisher = new NoisePublisher();
            NoiseEmitter = new NoiseEmitter(_noisePublisher);
            _soundPropagation = new Sound.DungeonSoundPropagation();
            _speciesPublishers = new Dictionary<string, SpeciesDeathPublisher>(StringComparer.OrdinalIgnoreCase);
            _enemySubscriptions = new List<EnemySubscriptions>();
            _enemyMovementStrategy = new Model.Movement.RandomWalkMovementStrategy();

            if (_theme != null && _itemPool != null && _enemyFactory != null)
            {
                SpawnThemeContent();
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

        private void SpawnThemeContent()
        {
            var availableTiles = GetAvailableFloorTiles();
            SpawnArtifact(availableTiles);
            SpawnThemeItems(availableTiles, 10);
            SpawnThemeEnemies(availableTiles, 7);
        }

        private void SpawnArtifact(List<(int y, int x)> availableTiles)
        {
            if (_theme == null || availableTiles.Count == 0)
                return;

            int pickIndex = Random.Shared.Next(availableTiles.Count);
            var (y, x) = availableTiles[pickIndex];
            availableTiles.RemoveAt(pickIndex);
            _tiles[y, x].Item = _theme.CreateArtifact(new Vec2(x, y));
        }

        private void SpawnThemeItems(List<(int y, int x)> availableTiles, int count)
        {
            if (_itemPool == null)
                return;

            int toSpawn = Math.Min(count, availableTiles.Count);
            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);
                _tiles[y, x].Item = _itemPool.CreateRandomItem(new Vec2(x, y));
            }
        }

        private void SpawnThemeEnemies(List<(int y, int x)> availableTiles, int count)
        {
            if (_enemyFactory == null)
                return;

            var spawnPlans = _enemyFactory.CreateSpeciesSpawnPlan();
            if (spawnPlans == null || spawnPlans.Count == 0)
                return;

            foreach (var plan in spawnPlans)
            {
                int toSpawn = Math.Min(plan.Count, availableTiles.Count);
                for (int i = 0; i < toSpawn; i++)
                {
                    if (availableTiles.Count == 0)
                        return;

                    int pickIndex = Random.Shared.Next(availableTiles.Count);
                    var (y, x) = availableTiles[pickIndex];
                    availableTiles.RemoveAt(pickIndex);

                    var enemy = plan.CreateEnemy(new Vec2(x, y));
                    _tiles[y, x].Enemy = enemy;
                    RegisterEnemy(enemy, plan.Species);
                }
            }
        }

        private void RegisterEnemy(Enemy enemy, EnemySpecies species)
        {
            _enemySpecies[enemy] = species;

            var subscriptions = new EnemySubscriptions(enemy, _noisePublisher, species.DeathPublisher, _soundPropagation, species.DeathReaction);
            subscriptions.Subscribe();
            _enemySubscriptionMap[enemy] = subscriptions;
            _enemySubscriptions.Add(subscriptions);
        }

        private void UnsubscribeEnemy(Enemy enemy)
        {
            if (_enemySubscriptionMap.TryGetValue(enemy, out var subscriptions))
            {
                subscriptions.Unsubscribe();
                _enemySubscriptionMap.Remove(enemy);
            }

            _enemySpecies.Remove(enemy);
        }

        private void RemoveEnemyFromMap(Enemy enemy)
        {
            UnsubscribeEnemy(enemy);
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

            if (_itemPool != null)
            {
                PlaceLoot(candidates, pos => _itemPool.CreateRandomItem(pos));
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
                weapon1.GetWeaponCategory());

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

        public bool IsAtCraftingStation(Player player)
        {
            var tile = GetTile(player.Pos.Y, player.Pos.X);
            return tile.IsCraftingStation;
        }

        public void Stop() => IsExitRequested = true;
        public void Respawn() => Initialize();

        private bool TryStoreFromTile(Player player, Tile tile, Items item, string successMessage)
        {
            if (!player.Inventory.AddToBackpack(item))
            {
                AddMessage("Backpack full.");
                return false;
            }

            tile.Item = null;
            AddMessage(successMessage);
            return true;
        }

        private bool TryEquipWeapon(Player player, Items item)
        {
            if (!item.CanEquip)
                return false;

            if (item.IsTwoHanded)
            {
                if (player.Inventory.LeftHand != null || player.Inventory.RightHand != null)
                    return false;

                return player.Inventory.EquipItem(item, 0);
            }

            if (player.Inventory.HasTwoHandedWeapon)
                return false;

            if (player.Inventory.LeftHand == null)
                return player.Inventory.EquipItem(item, 0);

            if (player.Inventory.RightHand == null)
                return player.Inventory.EquipItem(item, 1);

            return false;
        }

        private static void ApplyWeaponBonuses(Player player, Items item) => item.ApplyEquipBonuses(player.Stats);
        private static void RemoveWeaponBonuses(Player player, Items item) => item.RemoveEquipBonuses(player.Stats);

        public bool TryBackpackAction()
        {
            return TryBackpackAction(Player);
        }

        public bool TryBackpackAction(Player player)
        {
            int count = player.Inventory.Count();
            if (count == 0)
            {
                AddMessage("Backpack is empty.");
                return false;
            }

            return DropFromBackpack(player, count - 1);
        }

        public bool EquipFromBackpack(int index)
        {
            return EquipFromBackpack(Player, index);
        }

        public bool EquipFromBackpack(Player player, int index)
        {
            var selected = player.Inventory.GetItem(index);
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
                ? EquipTwoHandedFromBackpack(player, index)
                : EquipOneHandedFromBackpack(player, index);
        }

        private bool EquipTwoHandedFromBackpack(Player player, int index)
        {
            var left = player.Inventory.LeftHand;
            var right = player.Inventory.RightHand;

            int requiredSlots = (left != null ? 1 : 0) + (right != null ? 1 : 0);
            if (player.Inventory.Count() - 1 + requiredSlots > player.Inventory.MaxBackpackSize)
            {
                AddMessage("Backpack full.");
                return false;
            }

            var backpackItem = player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            var unequippedLeft = player.Inventory.UnequipItem(0);
            var unequippedRight = player.Inventory.UnequipItem(1);

            if (unequippedLeft != null)
                RemoveWeaponBonuses(player, unequippedLeft);
            if (unequippedRight != null)
                RemoveWeaponBonuses(player, unequippedRight);

            if (unequippedLeft != null)
                player.Inventory.AddToBackpack(unequippedLeft);
            if (unequippedRight != null)
                player.Inventory.AddToBackpack(unequippedRight);

            player.Inventory.EquipItem(backpackItem, 0);
            ApplyWeaponBonuses(player, backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private bool EquipOneHandedFromBackpack(Player player, int index)
        {
            var backpackItem = player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            int handIndex = GetOneHandEquipSlot(player);
            var displaced = player.Inventory.UnequipItem(handIndex);

            if (displaced != null)
            {
                RemoveWeaponBonuses(player, displaced);

                if (!player.Inventory.AddToBackpack(displaced))
                {
                    ApplyWeaponBonuses(player, displaced);
                    player.Inventory.EquipItem(displaced, handIndex);
                    player.Inventory.AddToBackpack(backpackItem);
                    AddMessage("Backpack full.");
                    return false;
                }
            }

            if (!player.Inventory.EquipItem(backpackItem, handIndex))
            {
                var rollback = player.Inventory.TakeFromBackpack(player.Inventory.Count() - 1);
                if (rollback != null)
                {
                    ApplyWeaponBonuses(player, rollback);
                    player.Inventory.EquipItem(rollback, handIndex);
                }

                player.Inventory.AddToBackpack(backpackItem);
                AddMessage("Cannot equip selected item.");
                return false;
            }

            ApplyWeaponBonuses(player, backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private void ResetPlayerStats()
        {
            Player = new Player(new Vec2(Player.Pos.X, Player.Pos.Y));
        }

        public void ProcessEnemiesTurn()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var enemy = _tiles[y, x].Enemy;
                    if (enemy == null || !enemy.IsAlive)
                        continue;

                    var next = _enemyMovementStrategy.ChooseNextPosition(enemy, this);
                    if (next.X == enemy.Position.X && next.Y == enemy.Position.Y)
                        continue;

                    if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
                        continue;

                    if (_tiles[next.Y, next.X].IsWall || _tiles[next.Y, next.X].Enemy != null)
                        continue;

                    if (Player.Pos.X == next.X && Player.Pos.Y == next.Y)
                        continue;

                    _tiles[y, x].Enemy = null;
                    _tiles[next.Y, next.X].Enemy = enemy;
                    enemy.MoveTo(next);
                }
            }
        }

        public bool DropFromBackpack(int index)
        {
            return DropFromBackpack(Player, index);
        }

        public bool DropFromBackpack(Player player, int index)
        {
            var tile = GetTile(player.Pos.Y, player.Pos.X);
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

            var item = player.Inventory.TakeFromBackpack(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            item.Position = new Tuple<int, int>(player.Pos.X, player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        public bool UseFromBackpack(int index)
        {
            return UseFromBackpack(Player, index);
        }

        public bool UseFromBackpack(Player player, int index)
        {
            var item = player.Inventory.GetItem(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            if (!item.TryUse(player, out var useMessage))
            {
                AddMessage(useMessage);
                return false;
            }

            player.Inventory.RemoveFromBackpack(index);
            AddMessage(useMessage);
            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            return TryDropItem(Player, handIndex);
        }

        public bool TryDropItem(Player player, int handIndex)
        {
            var tile = GetTile(player.Pos.Y, player.Pos.X);
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

            var item = player.Inventory.UnequipItem(handIndex);
            if (item == null)
            {
                AddMessage("Cannot drop: selected hand is empty.");
                return false;
            }

            RemoveWeaponBonuses(player, item);
            item.Position = new Tuple<int, int>(player.Pos.X, player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        public bool CraftArmorFromJunk()
        {
            return CraftArmorFromJunk(Player);
        }

        public bool CraftArmorFromJunk(Player player)
        {
            var junkIndexes = new List<int>();
            for (int i = 0; i < player.Inventory.Count(); i++)
            {
                var item = player.Inventory.GetItem(i);
                if (item != null && item.CanBeCraftingMaterial)
                    junkIndexes.Add(i);
            }

            if (junkIndexes.Count < 2)
            {
                AddMessage("Need 2 junk items to craft armor.");
                return false;
            }

            junkIndexes.Sort((a, b) => b.CompareTo(a));
            player.Inventory.RemoveFromBackpack(junkIndexes[0]);
            player.Inventory.RemoveFromBackpack(junkIndexes[1]);
            player.Stats.Armor += 2;
            AddMessage($"Crafted armor from junk. Armor is now {player.Stats.Armor}.");
            return true;
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

        public bool TryMovePlayer(int dx, int dy)
        {
            return TryMovePlayer(Player, dx, dy, null);
        }

        public bool TryMovePlayer(Player player, int dx, int dy, IEnumerable<Player>? otherPlayers = null)
        {
            var next = player.Pos.Add(dx, dy);

            if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
            {
                AddMessage("Walking into a wall.");
                return false;
            }

            var targetTile = _tiles[next.Y, next.X];
            if (targetTile.IsWall)
            {
                AddMessage("Walking into a wall.");
                return false;
            }

            if (IsOccupiedByOtherPlayer(next, player, otherPlayers))
            {
                AddMessage("Another player is standing there.");
                return false;
            }

            if (targetTile.Enemy != null)
            {
                StartCombat(targetTile.Enemy);
                return true;
            }

            player.MoveTo(next);
            return true;
        }

        private static bool IsOccupiedByOtherPlayer(Vec2 next, Player movingPlayer, IEnumerable<Player>? otherPlayers)
        {
            if (otherPlayers == null)
                return false;

            foreach (var player in otherPlayers)
            {
                if (!ReferenceEquals(player, movingPlayer) && player.Pos.X == next.X && player.Pos.Y == next.Y)
                    return true;
            }

            return false;
        }

        public bool TryPickUpItem()
        {
            return TryPickUpItem(Player);
        }

        public bool TryPickUpItem(Player player)
        {
            var tile = GetTile(player.Pos.Y, player.Pos.X);
            var item = tile.Item;

            if (item == null)
            {
                AddMessage("Cannot pick up: no item here.");
                return false;
            }

            if (item.TryCollect(player, out var collectMessage))
            {
                tile.Item = null;
                AddMessage(collectMessage);
                AddMessage($"Picking up item: {item.Name}.");

                if (item.CanEquip && NoiseEmitter != null)
                    NoiseOnPickupHook.OnItemPickedUp(this, item, NoiseEmitter);

                return true;
            }

            if (!item.CanEquip)
                return TryStoreFromTile(player, tile, item, $"Picked up {item.Name}.");

            if (TryEquipWeapon(player, item))
            {
                ApplyWeaponBonuses(player, item);
                tile.Item = null;
                AddMessage($"Equipped {item.Name}.");

                if (NoiseEmitter != null)
                    NoiseOnPickupHook.OnItemPickedUp(this, item, NoiseEmitter);

                return true;
            }

            var stored = TryStoreFromTile(player, tile, item, $"Stored {item.Name} in backpack.");
            if (stored && NoiseEmitter != null)
                NoiseOnPickupHook.OnItemPickedUp(this, item, NoiseEmitter);

            return stored;
        }

        private int GetOneHandEquipSlot(Player player)
        {
            if (player.Inventory.HasTwoHandedWeapon)
                return 0;

            if (player.Inventory.LeftHand == null)
                return 0;

            if (player.Inventory.RightHand == null)
                return 1;

            return 0;
        }

        private Items? ResolveActiveWeapon(Player player)
        {
            if (player.Inventory.LeftHand != null)
                return player.Inventory.LeftHand;

            return player.Inventory.RightHand;
        }

        private void StartCombat(Enemy enemy)
        {
            ActiveEnemy = enemy;
            AddMessage($"Encounter: {enemy.Name} | HP: {enemy.Health} | ATK: {enemy.AttackMin}-{enemy.AttackMax} | ARM: {enemy.Armor}");
        }

        public bool TryCombatRound(string attackKey)
        {
            return TryCombatRound(Player, attackKey);
        }

        public bool TryCombatRound(Player player, string attackKey)
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

            var weapon = ResolveActiveWeapon(player);
            var enemyBeforeRound = ActiveEnemy;
            var result = _combatEngine.ExecuteRound(player, enemyBeforeRound, attackType, weapon);
            AddMessage(result.Summary);
            AddMessage($"Player attack dealt {result.PlayerDamageDealt} damage.");
            AddMessage($"Enemy attack dealt {result.EnemyDamageDealt} damage.");

            if (result.EnemyDefeated)
            {
                var defeatedPos = enemyBeforeRound.Position;

                if (_enemySpecies.TryGetValue(enemyBeforeRound, out var species))
                    species.DeathPublisher.Publish(new SpeciesDeathEvent(enemyBeforeRound, species.Name));

                RemoveEnemyFromMap(enemyBeforeRound);
                SpawnVictoryLoot(defeatedPos);
                player.Heal(50);
                ActiveEnemy = null;
                AddMessage("Enemy removed from map.");
                AddMessage($"Enemy defeated: {enemyBeforeRound.Name}.");
            }

            if (result.PlayerDefeated)
            {
                AddMessage("You died. Game over.");
                if (!string.IsNullOrWhiteSpace(GameLog.FilePath))
                    AddMessage($"Log file: {GameLog.FilePath}");
            }

            return true;
        }

        public bool HasAllEnemiesBeenDefeated()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (_tiles[y, x].Enemy != null)
                        return false;
                }
            }

            return true;
        }
    }
}

    

