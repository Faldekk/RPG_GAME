using System.Collections.Generic;
using RPG_GAME.Model.Instructions;

namespace RPG_GAME.Model.Map.Build
{
    public interface IInstructionAwareBuildStep
    {
        void AppendInstructions(List<InstructionEntry> entries);
    }
}