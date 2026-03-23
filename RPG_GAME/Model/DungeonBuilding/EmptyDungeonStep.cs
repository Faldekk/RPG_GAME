using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class EmptyDungeonStep : IDungeonBuildStep
    {
       
        public bool IsStarter => true;

      
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[y, x] = new Tile(false);
                }
            }
            context.AddFeature("empty_dungeon");
        }
    }
}