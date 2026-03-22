using System.Collections.Generic;
using RPG_GAME.Model.Instructions;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class AddItemsStep : IDungeonBuildStep
    {
        private readonly int _count;

        public AddItemsStep(int count)
        {
            _count = count;
        }

        public void Apply(DungeonBuildContext context)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = _count * 20;

            while (spawned < _count && attempts < maxAttempts)
            {
                int y = Random.Shared.Next(1, context.Height - 1);
                int x = Random.Shared.Next(1, context.Width - 1);

                var tile = context.Tiles[y, x];
                if (!tile.IsWall && tile.Item == null)
                {
                    tile.Item = WeaponGenerator.GenerateRandomWeapon(x, y);
                    spawned++;
                }

                attempts++;
            }

            context.MarkFeature("items");
        }

        public IEnumerable<InstructionEntry> GetInstructions()
        {
            yield return new InstructionEntry("E", "Pick up item from the ground");
            yield return new InstructionEntry("G", "Drop equipped item");
        }
    }
}