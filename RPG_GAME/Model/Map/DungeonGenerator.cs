using System.Collections.Generic;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Map
{
    public static class DungeonGenerator
    {
        public static List<RectRoom> Generate(Tile[,] tiles, int width, int height)
        {
            var rooms = new List<RectRoom>();

            int roomCount = 8;
            int minSize = 4;
            int maxSize = 8;

            for (int i = 0; i < roomCount; i++)
            {
                int w = Random.Shared.Next(minSize, maxSize + 1);
                int h = Random.Shared.Next(minSize, maxSize + 1);
                int x = Random.Shared.Next(1, width - w - 1);
                int y = Random.Shared.Next(1, height - h - 1);

                var newRoom = new RectRoom(x, y, w, h);

                bool overlaps = false;
                foreach (var room in rooms)
                {
                    if (newRoom.Intersects(room))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps)
                    continue;

                MapUtils.CarveRoom(tiles, newRoom);

                if (rooms.Count > 0)
                {
                    var prev = rooms[^1];

                    if (Random.Shared.Next(2) == 0)
                    {
                        MapUtils.CarveHorizontalTunnel(tiles, prev.CenterX, newRoom.CenterX, prev.CenterY);
                        MapUtils.CarveVerticalTunnel(tiles, prev.CenterY, newRoom.CenterY, newRoom.CenterX);
                    }
                    else
                    {
                        MapUtils.CarveVerticalTunnel(tiles, prev.CenterY, newRoom.CenterY, prev.CenterX);
                        MapUtils.CarveHorizontalTunnel(tiles, prev.CenterX, newRoom.CenterX, newRoom.CenterY);
                    }
                }

                rooms.Add(newRoom);
            }

            return rooms;
        }
    }
}