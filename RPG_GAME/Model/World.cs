using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var Statistics = new Dictionary<string, int>();
            Statistics.Add("Strength", 10);
            Statistics.Add("Dexterity", 10);
            Statistics.Add("Health", 100);
            Statistics.Add("Luck", 50);
            Statistics.Add("Aggression", 25);
            Statistics.Add("Wisdom", 0);
            var Income = new Dictionary<string, int>();
            Income.Add("Coins", 50);
            Income.Add("Gold", 0);
            Player = new Player(new Vec2(1, 1),Statistics, Income);
        }

        private void InitializeTiles()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool isBorder =
                        y == 0 || y == Height - 1 ||
                        x == 0 || x == Width - 1;

                    _tiles[y, x] = new Tile(isBorder);
                }
            }

            // przykładowa ściana w środku
            for (int x = 10; x < 30; x++)
            {
                _tiles[8, x] = new Tile(true);
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