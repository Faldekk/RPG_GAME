using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public interface IDungeonBuildStep
    {
        bool IsStarter { get; }

        void Execute(Tile[,] tiles, int width, int height, BuildContext context);

        void RegisterInstructions(BuildContext context) { }
    }
}