using System.Collections.Generic;

namespace RPG_GAME.Model.Map.Build.Steps
{
    public class AddChambersStep : IDungeonBuildStep
    {
        private readonly int _count;
        private readonly int _minSize;
        private readonly int _maxSize;

        public AddChambersStep(int count, int minSize, int maxSize)
        {
            _count = count;
            _minSize = minSize;
            _maxSize = maxSize;
        }

        public void Apply(DungeonBuildContext context)
        {
            for (int i = 0; i < _count; i++)
            {
                int w = Random.Shared.Next(_minSize, _maxSize + 1);
                int h = Random.Shared.Next(_minSize, _maxSize + 1);
                int x = Random.Shared.Next(1, context.Width - w - 1);
                int y = Random.Shared.Next(1, context.Height - h - 1);

                var room = new RectRoom(x, y, w, h);

                bool overlaps = false;
                foreach (var existing in context.Rooms)
                {
                    if (room.Intersects(existing))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (overlaps)
                    continue;

                MapUtils.CarveRoom(context.Tiles, room);
                context.Rooms.Add(room);
            }

            context.MarkFeature("chambers");
        }

        public IEnumerable<RPG_GAME.Model.Instructions.InstructionEntry> GetInstructions()
        {
            yield break;
        }
    }
}