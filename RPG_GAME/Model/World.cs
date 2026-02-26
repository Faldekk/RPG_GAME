using System;
using System.Collections.Generic;

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
                    bool isBorder = x == 0 || y == 0 || y == Height - 1 || x == Width - 1;
                    _tiles[y, x] = new Tile(isBorder);
                }
            }

            // Random obstacles
            for (int i = 0; i < 40; i++)
            {
                int randomY = Random.Shared.Next(2, Height - 2);
                int randomX = Random.Shared.Next(2, Width - 2);
                _tiles[randomY, randomX] = new Tile(true);
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
    }
}