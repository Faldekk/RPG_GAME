using RPG_GAME.Model;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class CreateFilledDungeonStep : IDungeonBuildStep
    {
        public void Apply(DungeonBuildContext context)
        {
            for (int y = 0; y < context.Height; y++)
            {
                for (int x = 0; x < context.Width; x++)
                {
                    context.Tiles[y, x] = new Tile(true);
                }
            }

            context.MarkFeature("filled_dungeon");
        }

        public IEnumerable<RPG_GAME.Model.Instructions.InstructionEntry> GetInstructions()
        {
            yield break;
        }
    }
}