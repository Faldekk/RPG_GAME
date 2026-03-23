using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class FilledDungeonStep : IDungeonBuildStep
    {
        public bool IsStarter => true;
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Każde pole to ściana
                    tiles[y, x] = new Tile(true);
                }
            }
            context.AddFeature("filled_dungeon");
        }
    }
}