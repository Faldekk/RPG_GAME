using System.Collections.Generic;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Map
{
    public static class DungeonGenerator
    {
        public static List<RectRoom> Generate(Tile[,] tiles, int width, int height)
        {
            var rooms = new List<RectRoom>();

            int roomCount = 7;
            int minSize = 4;
            int maxSize = 8;

            for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
            {
                int roomWidth = Random.Shared.Next(minSize, maxSize + 1);
                int roomHeight = Random.Shared.Next(minSize, maxSize + 1);
                int roomX = Random.Shared.Next(1, width - roomWidth - 1);
                int roomY = Random.Shared.Next(1, height - roomHeight - 1);

                var candidateRoom = new RectRoom(roomX, roomY, roomWidth, roomHeight);

                bool hasOverlap = false;
                foreach (var existingRoom in rooms)
                {
                    if (candidateRoom.Intersects(existingRoom))
                    {
                        hasOverlap = true;
                        break;
                    }
                }

                if (hasOverlap)
                    continue;

                MapUtils.CarveRoom(tiles, candidateRoom);

                if (rooms.Count > 0)
                {
                    var previousRoom = rooms[^1];

                    if (Random.Shared.Next(2) == 0)
                    {
                        MapUtils.CarveHorizontalTunnel(tiles, previousRoom.CenterX, candidateRoom.CenterX, previousRoom.CenterY);
                        MapUtils.CarveVerticalTunnel(tiles, previousRoom.CenterY, candidateRoom.CenterY, candidateRoom.CenterX);
                    }
                    else
                    {
                        MapUtils.CarveVerticalTunnel(tiles, previousRoom.CenterY, candidateRoom.CenterY, previousRoom.CenterX);
                        MapUtils.CarveHorizontalTunnel(tiles, previousRoom.CenterX, candidateRoom.CenterX, candidateRoom.CenterY);
                    }
                }
                rooms.Add(candidateRoom);
            }

            return rooms;
        }
    }
}