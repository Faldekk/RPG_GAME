using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddChambersStep : IDungeonBuildStep
    {
        private readonly int _roomCount;
        private readonly int _minSize;
        private readonly int _maxSize;

        public bool IsStarter => false;

        // Parametry: ile komnat, min i max rozmiar
        public AddChambersStep(int roomCount, int minSize, int maxSize)
        {
            _roomCount = roomCount;
            _minSize = minSize;
            _maxSize = maxSize;
        }

        // Wylosuj kilka komnat w losowych miejscach - nie mogą się nakładać
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int created = 0;
            int attempts = 0;
            int maxAttempts = _roomCount * 20;  // Limit prób - czasami się nie zmieści

            while (created < _roomCount && attempts < maxAttempts)
            {
                attempts++;

                // Wylosuj rozmiar komnaty
                int roomWidth = Random.Shared.Next(_minSize, _maxSize + 1);
                int roomHeight = Random.Shared.Next(_minSize, _maxSize + 1);

                // Sprawdź czy się zmieści na mapie
                if (roomWidth >= width - 2 || roomHeight >= height - 2)
                    continue;

                // Wylosuj pozycję - nie za blisko brzegów
                int roomX = Random.Shared.Next(1, width - roomWidth - 1);
                int roomY = Random.Shared.Next(1, height - roomHeight - 1);

                // Twórz obiekt komnaty
                var candidateRoom = new RectRoom(roomX, roomY, roomWidth, roomHeight);
                // Dodaj margines - nie chcemy komnat tuż obok siebie
                var roomWithMargin = new RectRoom(roomX - 1, roomY - 1, roomWidth + 2, roomHeight + 2);

                // Sprawdź czy koliduje z innymi komnatami
                bool overlaps = false;
                foreach (var existingRoom in context.Rooms)
                {
                    if (candidateRoom.Intersects(existingRoom) || roomWithMargin.Intersects(existingRoom))
                    {
                        overlaps = true;
                        break;
                    }
                }

                // Kolizja - spróbuj ponownie
                if (overlaps)
                    continue;

                // Ok, wyrywaj! Dodaj do listy
                MapUtils.CarveRoom(tiles, candidateRoom);
                context.Rooms.Add(candidateRoom);
                created++;
            }

            // Zaznacz że były komnatki
            context.AddFeature("chambers");
        }
    }
}