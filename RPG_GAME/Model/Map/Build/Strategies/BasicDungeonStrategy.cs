using RPG_GAME.Model.DungeonBuilding;
using RPG_GAME.Model.MapBuilder;

namespace RPG_GAME.Model.MapBuilder
{
    public class BasicDungeonStrategy : IDungeonStrategy
    {
        public BuildContext Build(Tile[,] tiles, int width, int height)
        {
            var builder = new DungeonBuilder()
                .AddStep(new FilledDungeonStep())
                .AddStep(new AddCentralRoomStep(8, 6))
                .AddStep(new AddChambersStep(6, 4, 8))
                .AddStep(new AddPathsStep());

            return builder.Build(tiles, width, height);
        }
    }
}