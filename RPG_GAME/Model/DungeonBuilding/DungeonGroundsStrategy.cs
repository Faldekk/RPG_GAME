namespace RPG_GAME.Model.DungeonBuilding
{
    public class DungeonGroundsStrategy : IDungeonStrategy
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