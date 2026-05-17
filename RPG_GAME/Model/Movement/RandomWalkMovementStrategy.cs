using System;
using System.Collections.Generic;
using RPG_GAME.Model;
using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model.Movement
{
    public sealed class RandomWalkMovementStrategy : IEnemyMovementStrategy
    {
        public Vec2 ChooseNextPosition(Enemy enemy, World world)
        {
            var pos = enemy.Position;
            var tiles = WorldAccessor.Tiles;
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            var dirs = new (int dy, int dx)[] { (-1,0),(1,0),(0,-1),(0,1) };
            var candidates = new List<Vec2>();

            foreach (var (dy,dx) in dirs)
            {
                int ny = pos.Y + dy;
                int nx = pos.X + dx;

                if (ny < 0 || ny >= height || nx < 0 || nx >= width) continue;
                var tile = tiles[ny, nx];
                if (tile.IsWall) continue;
                if (tile.Enemy != null) continue;

                candidates.Add(new Vec2(nx, ny));
            }

            if (candidates.Count == 0) return pos;

            return candidates[Random.Shared.Next(candidates.Count)];
        }
    }
}
