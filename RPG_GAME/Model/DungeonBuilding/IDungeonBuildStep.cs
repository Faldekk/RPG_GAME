using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public interface IDungeonBuildStep
    {
        void Execute(Tile[,] tiles, int width, int height, BuildContext context);
    }
}