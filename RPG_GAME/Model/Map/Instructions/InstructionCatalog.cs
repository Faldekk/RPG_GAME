using System.Collections.Generic;
using RPG_GAME.Model.Map.Build;

namespace RPG_GAME.Model.Instructions
{
    public class InstructionCatalog
    {
        private readonly List<InstructionEntry> _entries = new();

        public IReadOnlyList<InstructionEntry> Entries => _entries;

        public void Add(string keyHint, string description)
        {
            _entries.Add(new InstructionEntry(keyHint, description));
        }

        public void AddRange(IEnumerable<InstructionEntry> entries)
        {
            _entries.AddRange(entries);
        }

        public static InstructionCatalog FromSteps(IEnumerable<IDungeonBuildStep> steps)
        {
            var catalog = new InstructionCatalog();

            catalog.Add("WASD / Arrows", "Move around the dungeon");
            catalog.Add("Q / Esc", "Quit the game");

            foreach (var step in steps)
            {
                catalog.AddRange(step.GetInstructions());
            }

            return catalog;
        }
    }
}