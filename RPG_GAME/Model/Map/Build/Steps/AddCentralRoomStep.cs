using System.Collections.Generic;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class AddCentralRoomStep : IDungeonBuildStep
    {
        private readonly int _roomWidth;
        private readonly int _roomHeight;

        public AddCentralRoomStep(int roomWidth, int roomHeight)
        {
            _roomWidth = roomWidth;
            _roomHeight = roomHeight;
        }

        public void Apply(DungeonBuildContext context)
        {
            int startX = (context.Width - _roomWidth) / 2;
            int startY = (context.Height - _roomHeight) / 2;

            var room = new RectRoom(startX, startY, _roomWidth, _roomHeight);

            MapUtils.CarveRoom(context.Tiles, room);
            context.Rooms.Add(room);
            context.MarkFeature("central_room");
        }

        public IEnumerable<RPG_GAME.Model.Instructions.InstructionEntry> GetInstructions()
        {
            yield break;
        }
    }
}