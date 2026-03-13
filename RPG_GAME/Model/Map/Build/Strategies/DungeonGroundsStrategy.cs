using RPG_GAME.Model.Map.Build.Steps;

namespace RPG_GAME.Model.Map.Build.Strategies
{
    public class DungeonGroundsStrategy : IDungeonBuildStrategy
    {
        public DungeonBuilder CreateBuilder()
        {
            return new DungeonBuilder()
                .AddStep(new CreateFilledDungeonStep())
                .AddStep(new AddCentralRoomStep(8, 6))
                .AddStep(new AddChambersStep(6, 4, 8))
                .AddStep(new AddPathsStep())
                .AddStep(new AddWeaponsStep(10));
        }
    }
}