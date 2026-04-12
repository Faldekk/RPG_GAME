using RPG_GAME.Model.Combat;
using RPG_GAME.Model.Map;
using RPG_GAME.UI;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddEnemiesStep : IDungeonBuildStep
    {
        private readonly int _count;

        public bool IsStarter => false;

        public AddEnemiesStep(int count)
        {
            _count = count;
        }

        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = _count * 20;
            RectRoom? spawnRoom = GetSpawnRoom(context);

            while (spawned < _count && attempts < maxAttempts)
            {
                attempts++;

                int y = Random.Shared.Next(1, height - 1);
                int x = Random.Shared.Next(1, width - 1);

                if (spawnRoom.HasValue && IsInsideRoom(x, y, spawnRoom.Value))
                    continue;

                if (tiles[y, x].IsWall || tiles[y, x].Item != null || tiles[y, x].Enemy != null)
                    continue;

                tiles[y, x].Enemy = EnemyGenerator.Generate(new Vec2(x, y));
                spawned++;
            }

            if (spawned > 0)
                context.AddFeature("enemies");
        }

        public void RegisterInstructions(BuildContext context)
        {
            if (!context.HasFeature("enemies"))
                return;

            context.AddInstruction(ControlsConfig.CombatNormal);
            context.AddInstruction(ControlsConfig.CombatStealth);
            context.AddInstruction(ControlsConfig.CombatMagical);
        }

        private static RectRoom? GetSpawnRoom(BuildContext context)
        {
            if (context.CentralRoom.HasValue)
                return context.CentralRoom.Value;

            if (context.Rooms.Count > 0)
                return context.Rooms[0];

            return null;
        }

        private static bool IsInsideRoom(int x, int y, RectRoom room)
        {
            return x >= room.X &&
                   x < room.X + room.Width &&
                   y >= room.Y &&
                   y < room.Y + room.Height;
        }
    }
}
