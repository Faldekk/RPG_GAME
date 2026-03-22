using System.Collections.Generic;
using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class DungeonBuilder
    {
        private readonly List<IDungeonBuildStep> _steps = new();

        public DungeonBuilder AddStep(IDungeonBuildStep step)
        {
            _steps.Add(step);
            return this;
        }

        public BuildContext Build(Tile[,] tiles, int width, int height)
        {
            var context = new BuildContext();

            foreach (var step in _steps)
            {
                step.Execute(tiles, width, height, context);
            }

            return context;
        }
    }
}