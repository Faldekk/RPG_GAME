using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddCentralRoomStep : IDungeonBuildStep
    {
        private readonly int _roomWidth;
        private readonly int _roomHeight;

        public bool IsStarter => false;

        public AddCentralRoomStep(int width, int height)
        {
            _roomWidth = width;
            _roomHeight = height;
        }

        // Wyryj centralną komnatę dokładnie w środku mapy - tam będzie gracz i będzie coś do roboty
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            // Upewnij się że komnata się zmieści - nie wychodź poza mapę
            int clampedWidth = System.Math.Min(_roomWidth, width - 2);
            int clampedHeight = System.Math.Min(_roomHeight, height - 2);

            // Policz gdzie ma być - dokładnie pośrodku
            int roomX = (width - clampedWidth) / 2;
            int roomY = (height - clampedHeight) / 2;

            // Stwórz komnate i wyrównaj ścianami
            var room = new RectRoom(roomX, roomY, clampedWidth, clampedHeight);
            MapUtils.CarveRoom(tiles, room);

            // Zapamiętaj dla przyszłości - tam ma się pojawić gracz
            context.CentralRoom = room;
            context.Rooms.Add(room);
            context.AddFeature("central_room");
        }
    }
}