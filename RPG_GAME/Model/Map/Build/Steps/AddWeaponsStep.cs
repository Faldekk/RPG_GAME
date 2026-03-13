using System.Collections.Generic;
using RPG_GAME.Model.Instructions;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class AddWeaponsStep : IDungeonBuildStep
    {
        private readonly int _count;

        public AddWeaponsStep(int count)
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
                if (!tile.IsWall && !tile.HasItem)
                {
                    tile.Item = WeaponGenerator.GenerateRandomWeapon(x, y);
                    spawned++;
                }

                attempts++;
            }

            context.MarkFeature("weapons");
        }

        public IEnumerable<InstructionEntry> GetInstructions()
        {
            yield return new InstructionEntry("E", "Pick up a weapon from the ground");
            yield return new InstructionEntry("G", "Drop equipped weapon");
            yield return new InstructionEntry("X", "Swap left and right hand weapons");
            yield return new InstructionEntry("1", "Drop left-hand weapon");
            yield return new InstructionEntry("2", "Drop right-hand weapon");
        }
    }
}