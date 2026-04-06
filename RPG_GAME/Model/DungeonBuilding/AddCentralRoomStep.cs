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
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int clampedWidth = System.Math.Min(_roomWidth, width - 2);
            int clampedHeight = System.Math.Min(_roomHeight, height - 2);
            int roomX = (width - clampedWidth) / 2;
            int roomY = (height - clampedHeight) / 2;
            var room = new RectRoom(roomX, roomY, clampedWidth, clampedHeight);
            MapUtils.CarveRoom(tiles, room);
            context.CentralRoom = room;
            context.Rooms.Add(room);
            context.AddFeature("central_room");
        }
    }
}