using RPG_GAME.Model.Map;
using RPG_GAME.UI;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddItemsStep : IDungeonBuildStep
    {
        private readonly int _count;

        public bool IsStarter => false;

        public AddItemsStep(int count)
        {
            _count = count;
        }
        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = _count * 20;  // Czasami się nie uda - limit prób

            while (spawned < _count && attempts < maxAttempts)
            {
                attempts++;

                int y = Random.Shared.Next(1, height - 1);
                int x = Random.Shared.Next(1, width - 1);
                if (tiles[y, x].IsWall || tiles[y, x].Item != null)
                    continue;

                tiles[y, x].Item = ItemGenerator.GenerateRandomJunk(new Vec2(x, y));
                spawned++;
            }
            if (spawned > 0)
                context.AddFeature("items");
        }
        public void RegisterInstructions(BuildContext context)
        {
            if (context.HasFeature("items"))
                context.AddInstruction(ControlsConfig.PickUp);
        }
    }
}
