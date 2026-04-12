using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddPathsStep : IDungeonBuildStep
    {
        public bool IsStarter => false;
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            for (int index = 1; index < context.Rooms.Count; index++)
            {
                var previousRoom = context.Rooms[index - 1];
                var currentRoom = context.Rooms[index];

                MapUtils.CarveCorridor(
                    tiles,
                    previousRoom.CenterX,
                    previousRoom.CenterY,
                    currentRoom.CenterX,
                    currentRoom.CenterY);
            }
            context.AddFeature("paths");
        }
    }
}