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
            InitializeTiles();

            var statistics = new Dictionary<string, int>();
            var income = new Dictionary<string, int>();
            var slots = new List<Items>();

            Player = new Player(new Vec2(1, 1), statistics, income, slots);
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

            SpawnRandomWeapons(10);
        }
        private void SpawnRandomWeapons(int count)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = count * 10;

            while (spawned < count && attempts < maxAttempts)
            {
                int randomY = Random.Shared.Next(2, Height - 2);
                int randomX = Random.Shared.Next(2, Width - 2);

                // Only spawn on empty, walkable tiles
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

            if (next.X < 0 || next.X >= Width ||
                next.Y < 0 || next.Y >= Height)
                return false;

            if (_tiles[next.Y, next.X].IsWall)
                return false;

            Player.MoveTo(next);
            return true;
        }

        /// <summary>
        /// Pick up item at player position
        /// </summary>
        public bool TryPickUpItem()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);

            if (!tile.HasItem)
                return false;

            var item = tile.Item;

            // Try to equip in left hand first
            if (Player.EquipWeapon(item, 0))
            {
                tile.Item = null;
                return true;
            }

            // Try right hand
            if (Player.EquipWeapon(item, 1))
            {
                tile.Item = null;
                return true;
            }

            return false; // Hands full
        }

        /// <summary>
        /// Drop item from player inventory
        /// </summary>
        public bool TryDropItem(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);

            // Can't drop if there's already an item here
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
        // Dodaj te metody do klasy World:

        /// <summary>
        /// Zamienia broń między rękami gracza
        /// </summary>
        public void SwapPlayerWeapons()
        {
            Player.SwapWeapons();
        }

        /// <summary>
        /// Upuszcza broń z konkretnej ręki (0 = lewa, 1 = prawa)
        /// </summary>
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