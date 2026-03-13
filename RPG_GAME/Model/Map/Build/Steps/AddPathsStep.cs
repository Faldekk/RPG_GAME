using System.Collections.Generic;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class AddPathsStep : IDungeonBuildStep
    {
        public void Apply(DungeonBuildContext context)
        {
            for (int i = 1; i < context.Rooms.Count; i++)
            {
                var prev = context.Rooms[i - 1];
                var next = context.Rooms[i];

                if (Random.Shared.Next(2) == 0)
                {
                    MapUtils.CarveHorizontalTunnel(context.Tiles, prev.CenterX, next.CenterX, prev.CenterY);
                    MapUtils.CarveVerticalTunnel(context.Tiles, prev.CenterY, next.CenterY, next.CenterX);
                }
                else
                {
                    MapUtils.CarveVerticalTunnel(context.Tiles, prev.CenterY, next.CenterY, prev.CenterX);
                    MapUtils.CarveHorizontalTunnel(context.Tiles, prev.CenterX, next.CenterX, next.CenterY);
                }
            }

            context.MarkFeature("paths");
        }

        public IEnumerable<RPG_GAME.Model.Instructions.InstructionEntry> GetInstructions()
        {
            yield break;
        }
    }
}