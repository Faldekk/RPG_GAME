using RPG_GAME.Model;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class CreateEmptyDungeonStep : IDungeonBuildStep
    {
        public void Apply(DungeonBuildContext context)
        {
            for (int y = 0; y < context.Height; y++)
            {
                for (int x = 0; x < context.Width; x++)
                {
                    context.Tiles[y, x] = new Tile(false);
                }
            }

            context.MarkFeature("empty_dungeon");
        }

        public IEnumerable<RPG_GAME.Model.Instructions.InstructionEntry> GetInstructions()
        {
            yield break;
        }
    }
}