using System.Collections.Generic;
using RPG_GAME.Model.Instructions;

namespace RPG_GAME.Model.Map.Build
{
    public class DungeonBuilder
    {
        private readonly List<IDungeonBuildStep> _steps = new();

        public IReadOnlyList<IDungeonBuildStep> Steps => _steps;

        public DungeonBuilder AddStep(IDungeonBuildStep step)
        {
            _steps.Add(step);
            return this;
        }

        public DungeonBuildContext Build(int width, int height)
        {
            var context = new DungeonBuildContext(width, height);

            foreach (var step in _steps)
                step.Apply(context);

            return context;
        }

        public InstructionCatalog BuildInstructions()
        {
            var catalog = new InstructionCatalog();
            catalog.Add("WASD / Arrows", "Move around the dungeon");
            catalog.Add("Q / Esc", "Quit the game");

            foreach (var step in _steps)
                catalog.AddRange(step.GetInstructions());

            return catalog;
        }
    }
}