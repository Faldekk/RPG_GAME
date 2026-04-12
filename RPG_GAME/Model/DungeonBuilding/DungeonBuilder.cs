using System;
using System.Collections.Generic;
using RPG_GAME.Model.Map;
using RPG_GAME.UI;

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
            if (_steps.Count == 0)
                throw new InvalidOperationException("No dungeon build steps configured.");

            if (!_steps[0].IsStarter)
                throw new InvalidOperationException("First step must be a starter step (filled or empty dungeon).");

            
            for (int i = 1; i < _steps.Count; i++)
            {
                if (_steps[i].IsStarter)
                    throw new InvalidOperationException("Starter steps can only be used as the first step.");
            }
            var context = new BuildContext();
            context.AddInstruction(ControlsConfig.Movement);
            context.AddInstruction(ControlsConfig.Quit);

        
            foreach (var step in _steps)
            {
                step.Execute(tiles, width, height, context);
                step.RegisterInstructions(context);
            }

            return context;
        }
    }
}