using System;
using System.Collections.Generic;
using RPG_GAME.Model.Map;

namespace RPG_GAME.Model
{
    public class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;
        public Player Player { get; }

        public World()
        {
            _tiles = new Tile[Height, Width];

            var statistics = new Dictionary<string, int>();
            var income = new Dictionary<string, int>();
            var slots = new List<Items?>();

            Player = new Player(new Vec2(1, 1), statistics, income, slots);

            InitializeTiles();
        }

        private void InitializeTiles()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _tiles[y, x] = new Tile(true);
                }
            }

            var rooms = DungeonGenerator.Generate(_tiles, Width, Height);

            if (rooms.Count > 0)
            {
                var start = rooms[0];
                Player.MoveTo(new Vec2(start.CenterX, start.CenterY));
            }
            else
            {
                _tiles[1, 1].IsWall = false;
                Player.MoveTo(new Vec2(1, 1));
            }

            SpawnRandomWeapons(10);
        }

        private void SpawnRandomWeapons(int count)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = count * 20;

            while (spawned < count && attempts < maxAttempts)
            {
                int randomY = Random.Shared.Next(1, Height - 1);
                int randomX = Random.Shared.Next(1, Width - 1);

                if (!_tiles[randomY, randomX].IsWall && !_tiles[randomY, randomX].HasItem)
                {
                    _tiles[randomY, randomX].Item = WeaponGenerator.GenerateRandomWeapon(randomX, randomY);
                    spawned++;
                }

                attempts++;
            }
        }

        public Tile GetTile(int y, int x)
        {
            return _tiles[y, x];
        }

        public bool TryMovePlayer(int dx, int dy)
        {
            Vec2 next = Player.Pos.Add(dx, dy);

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
                return false;

            if ((item.Both_hands && Player.Inventory.LeftHand != null) && (Player.Inventory.RightHand != null))
                return false;

            int hand = item.Both_hands
                ? (Player.Inventory.LeftHand  != null ? 0 : Player.Inventory.RightHand != null ? 1 : 0)
                : (Player.Inventory.LeftHand  == null ? 0 : Player.Inventory.RightHand == null ? 1 : 0);

            var displaced = Player.UnequipWeapon(hand);

            if (!Player.EquipWeapon(item, hand))
            {
                if (displaced != null) Player.EquipWeapon(displaced, hand);
                return false;
            }

            tile.Item = displaced;
            if (displaced != null)
                displaced.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);

            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);

            if (tile.HasItem)
                return false;

            var item = Player.UnequipWeapon(handIndex);

            if (item != null)
            {
                item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
                tile.Item = item;
                return true;
            }

            return false;
        }

        public void SwapPlayerWeapons()
        {
            Player.SwapWeapons();
        }

        public bool TryDropSpecificHand(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);

            if (tile.HasItem)
                return false;

            var item = Player.UnequipWeapon(handIndex);

            if (item != null)
            {
                item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
                tile.Item = item;
                return true;
            }

            return false;
        }
    }
}