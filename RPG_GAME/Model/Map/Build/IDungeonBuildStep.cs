using System.Collections.Generic;
using RPG_GAME.Model.Instructions;

namespace RPG_GAME.Model.Map.Build
{
    public interface IDungeonBuildStep
    {
        void Apply(DungeonBuildContext context);
        IEnumerable<InstructionEntry> GetInstructions();
    }
}