using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddChambersStep : IDungeonBuildStep
    {
        private readonly int _roomCount;
        private readonly int _minSize;
        private readonly int _maxSize;

        public bool IsStarter => false;
        public AddChambersStep(int roomCount, int minSize, int maxSize)
        {
            _roomCount = roomCount;
            _minSize = minSize;
            _maxSize = maxSize;
        }
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int created = 0;
            int attempts = 0;
            int maxAttempts = _roomCount * 20;  // Limit prób - czasami się nie zmieści

            while (created < _roomCount && attempts < maxAttempts)
            {
                attempts++;
                int roomWidth = Random.Shared.Next(_minSize, _maxSize + 1);
                int roomHeight = Random.Shared.Next(_minSize, _maxSize + 1);
                if (roomWidth >= width - 2 || roomHeight >= height - 2)
                    continue;
                int roomX = Random.Shared.Next(1, width - roomWidth - 1);
                int roomY = Random.Shared.Next(1, height - roomHeight - 1);
                var candidateRoom = new RectRoom(roomX, roomY, roomWidth, roomHeight);
                var roomWithMargin = new RectRoom(roomX - 1, roomY - 1, roomWidth + 2, roomHeight + 2);
                bool overlaps = false;
                foreach (var existingRoom in context.Rooms)
                {
                    if (candidateRoom.Intersects(existingRoom) || roomWithMargin.Intersects(existingRoom))
                    {
                        overlaps = true;
                        break;
                    }
                }
                if (overlaps)
                    continue;

               
                MapUtils.CarveRoom(tiles, candidateRoom);
                context.Rooms.Add(candidateRoom);
                created++;
            }
            context.AddFeature("chambers");
        }
    }
}