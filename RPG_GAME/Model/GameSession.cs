using System;
using System.Collections.Generic;
using System.Linq;
using RPG_GAME.App.Logging;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Model
{
    public sealed class GameSession
    {
        private readonly World _world;
        private readonly GameTimer _timer;
        private readonly Dictionary<int, Player> _players = new();
        private int _nextPlayerId = 1;

        public GameSession(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _timer = new GameTimer();
            _players[1] = world.Player;
            _nextPlayerId = 2;
        }

        public World GameWorld => _world;

        public IReadOnlyDictionary<int, Player> Players => _players;

        public int RegisterPlayer(Vec2 spawnPosition)
        {
            if (_players.Count >= 9)
                return -1;

            int playerId = _nextPlayerId;
            while (_players.ContainsKey(playerId) && playerId <= 9)
                playerId++;

            if (playerId > 9)
                return -1;

            _players[playerId] = new Player(spawnPosition);
            _nextPlayerId = playerId + 1;
            return playerId;
        }

       public bool RemovePlayer(int playerId){
            if (playerId == 1)
                return false;

            return _players.Remove(playerId);
            }

        public bool UnregisterPlayer(int playerId)
        {
        return RemovePlayer(playerId);

        }

        public bool ApplyCommand(PlayerCommandDto command)
        {
            if (!_players.TryGetValue(command.PlayerId, out var player))
                return false;

            return command.Command switch
            {
                PlayerCommandKind.MoveUp => _world.TryMovePlayer(player, 0, -1, _players.Values),
                PlayerCommandKind.MoveDown => _world.TryMovePlayer(player, 0, 1, _players.Values),
                PlayerCommandKind.MoveLeft => _world.TryMovePlayer(player, -1, 0, _players.Values),
                PlayerCommandKind.MoveRight => _world.TryMovePlayer(player, 1, 0, _players.Values),
                PlayerCommandKind.PickUp => _world.TryPickUpItem(player),
                PlayerCommandKind.BackpackAction => _world.TryBackpackAction(player),
                PlayerCommandKind.SwapWeapons => SwapWeapons(player),
                PlayerCommandKind.DropLeftHand => _world.TryDropItem(player, 0),
                PlayerCommandKind.DropRightHand => _world.TryDropItem(player, 1),
                PlayerCommandKind.InventoryEquip => _world.EquipFromBackpack(player, command.Value),
                PlayerCommandKind.InventoryDrop => _world.DropFromBackpack(player, command.Value),
                PlayerCommandKind.InventoryUse => _world.UseFromBackpack(player, command.Value),
                PlayerCommandKind.InventoryCraftArmor => _world.CraftArmorFromJunk(player),
                PlayerCommandKind.CombatNormalAttack => _world.TryCombatRound(player, "normal"),
                PlayerCommandKind.CombatStealthAttack => _world.TryCombatRound(player, "stealth"),
                PlayerCommandKind.CombatMagicalAttack => _world.TryCombatRound(player, "magical"),
                PlayerCommandKind.Quit => HandleQuit(),
                _ => false
            };
        }

        private bool HandleQuit()
        {
            _world.Stop();
            return true;
        }

        private static bool SwapWeapons(Player player)
        {
            player.SwapWeapons();
            return true;
        }

        public GameStateDto CreateStateDto()
        {
            var mapRows = BuildMapRows();
            bool isWon = _world.HasAllEnemiesBeenDefeated();

            if (isWon && !_timer.IsFrozen)
                _timer.Freeze();

            return new GameStateDto
            {
                Width = World.Width,
                Height = World.Height,
                MapRows = mapRows,
                Players = _players.Select(pair => ToPlayerDto(pair.Key, pair.Value)).ToList(),
                Enemies = CollectEnemies(),
                Items = CollectItems(),
                Tiles = CollectTiles(mapRows),
                RecentEntries = GameLog.RecentEntries.Select(ToLogEntryDto).ToList(),
                JournalEntries = GameLog.JournalEntries.Select(ToLogEntryDto).ToList(),
                CurrentMessage = _world.CurrentMessage,
                CurrentMode = "Normal",
                ActivePlayerId = 1,
                IsExitRequested = _world.IsExitRequested,
                ElapsedSeconds = _timer.ElapsedSeconds,
                IsGameWon = isWon
            };
        }

        private static LogEntryDto ToLogEntryDto(LogEntry entry)
        {
            return new LogEntryDto
            {
                Timestamp = entry.Timestamp,
                Message = entry.Message
            };
        }

        private static PlayerDto ToPlayerDto(int playerId, Player player)
        {
            return new PlayerDto
            {
                PlayerId = playerId,
                X = player.Pos.X,
                Y = player.Pos.Y,
                Health = player.Stats.Health,
                MaxHealth = player.Stats.MaxHealth,
                Strength = player.Stats.Strength,
                Dexterity = player.Stats.Dexterity,
                Luck = player.Stats.Luck,
                Aggression = player.Stats.Aggression,
                Wisdom = player.Stats.Wisdom,
                Armor = player.Stats.Armor,
                Coins = player.Stats.Coins,
                Gold = player.Stats.Gold,
                LeftHand = player.Inventory.LeftHand?.Name ?? string.Empty,
                RightHand = player.Inventory.RightHand?.Name ?? string.Empty
            };
        }

        private List<EnemyDto> CollectEnemies()
        {
            var result = new List<EnemyDto>();
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    var enemy = _world.GetTile(y, x).Enemy;
                    if (enemy == null)
                        continue;

                    result.Add(new EnemyDto
                    {
                        Name = enemy.Name,
                        SpeciesKey = enemy.SpeciesKey,
                        X = enemy.Position.X,
                        Y = enemy.Position.Y,
                        Health = enemy.Health,
                        AttackMin = enemy.AttackMin,
                        AttackMax = enemy.AttackMax,
                        Armor = enemy.Armor,
                        MapCharacter = enemy.MapCharacter
                    });
                }
            }

            return result;
        }

        private List<ItemDto> CollectItems()
        {
            var result = new List<ItemDto>();
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    var item = _world.GetTile(y, x).Item;
                    if (item == null)
                        continue;

                    result.Add(new ItemDto
                    {
                        Name = item.Name,
                        Type = item.Type,
                        X = x,
                        Y = y,
                        Value = item.Value,
                        Durability = item.Durability,
                        MapCharacter = item.MapCharacter,
                        CanEquip = item.CanEquip,
                        IsTwoHanded = item.IsTwoHanded,
                        IsHeal = item.IsHeal
                    });
                }
            }

            return result;
        }

        private static List<TileDto> CollectTiles(string[] mapRows)
        {
            var tiles = new List<TileDto>();
            for (int y = 0; y < mapRows.Length; y++)
            {
                var row = mapRows[y];
                for (int x = 0; x < row.Length; x++)
                {
                    tiles.Add(new TileDto
                    {
                        X = x,
                        Y = y,
                        IsWall = row[x] == '#',
                        IsCraftingStation = row[x] == '◊',
                        Symbol = row[x]
                    });
                }
            }

            return tiles;
        }

        private string[] BuildMapRows()
        {
            var rows = new string[World.Height];

            for (int y = 0; y < World.Height; y++)
            {
                var line = new char[World.Width];
                for (int x = 0; x < World.Width; x++)
                {
                    var tile = _world.GetTile(y, x);
                    char ch = tile.IsWall ? '#' : '.';

                    if (tile.IsCraftingStation)
                        ch = '◊';
                    else if (tile.Item != null)
                        ch = tile.Item.MapCharacter;
                    else if (tile.Enemy != null)
                        ch = tile.Enemy.MapCharacter;

                    line[x] = ch;
                }

                rows[y] = new string(line);
            }

            foreach (var pair in _players)
            {
                int id = pair.Key;
                var player = pair.Value;
                if (id < 1 || id > 9)
                    continue;

                char[] row = rows[player.Pos.Y].ToCharArray();
                row[player.Pos.X] = (char)('0' + id);
                rows[player.Pos.Y] = new string(row);
            }

            return rows;
        }
    }
}
