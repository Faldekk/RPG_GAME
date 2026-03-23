using RPG_GAME.Model.Map;

namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddWeaponsStep : IDungeonBuildStep
    {
        private readonly int _count;

        public bool IsStarter => false;

        public AddWeaponsStep(int count)
        {
            _count = count;
        }

        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = _count * 20;  

            while (spawned < _count && attempts < maxAttempts)
            {
                attempts++;

                int y = Random.Shared.Next(1, height - 1);
                int x = Random.Shared.Next(1, width - 1);
                if (tiles[y, x].IsWall || tiles[y, x].Item != null)
                    continue;

                tiles[y, x].Item = WeaponGenerator.GenerateRandomWeapon(x, y);
                spawned++;
            }
            if (spawned > 0)
                context.AddFeature("weapons");
        }
        public void RegisterInstructions(BuildContext context)
        {
            if (!context.HasFeature("weapons"))
                return;

            context.AddInstruction(ControlsConfig.PickUp);
            context.AddInstruction(ControlsConfig.SwapWeapons);
            context.AddInstruction(ControlsConfig.DropLeft);
            context.AddInstruction(ControlsConfig.DropRight);
        }
    }
}
