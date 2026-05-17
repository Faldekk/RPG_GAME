using System.Collections.Generic;
using System;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Sound
{
    public interface ISoundPropagation
    {
        int? GetDistanceIfReachable(Vec2 source, Vec2 target, int maxRange, Tile[,] tiles);
    }

    public sealed class DungeonSoundPropagation : ISoundPropagation
    {
        public int? GetDistanceIfReachable(Vec2 source, Vec2 target, int maxRange, Tile[,] tiles)
        {
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            var visited = new bool[height, width];
            var queue = new Queue<(int y, int x, int d)>();

            if (source.X < 0 || source.X >= width || source.Y < 0 || source.Y >= height)
                return null;

            if (target.X < 0 || target.X >= width || target.Y < 0 || target.Y >= height)
                return null;

            queue.Enqueue((source.Y, source.X, 0));
            visited[source.Y, source.X] = true;

            var dirs = new (int dy, int dx)[] { (-1,0),(1,0),(0,-1),(0,1) };

            while (queue.Count > 0)
            {
                var (y,x,d) = queue.Dequeue();
                if (d > maxRange) continue;
                if (y == target.Y && x == target.X) return d;

                foreach (var (dy,dx) in dirs)
                {
                    int ny = y + dy;
                    int nx = x + dx;

                    if (ny < 0 || ny >= height || nx < 0 || nx >= width) continue;
                    if (visited[ny, nx]) continue;
                    if (tiles[ny, nx].IsWall) continue;

                    visited[ny, nx] = true;
                    queue.Enqueue((ny, nx, d + 1));
                }
            }

            return null;
        }
    }
}
