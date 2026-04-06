namespace RPG_GAME.Model.DungeonBuilding
{
    public class AddCraftingStationsStep : IDungeonBuildStep
    {
        private readonly int _count;

        public bool IsStarter => false;

        public AddCraftingStationsStep(int count = 2)
        {
            _count = count;
        }

        public void Execute(Tile[,] tiles, int width, int height, BuildContext context)
        {
            int placed = 0;
            int attempts = 0;
            int maxAttempts = _count * 20;

            while (placed < _count && attempts < maxAttempts)
            {
                attempts++;

                int y = Random.Shared.Next(1, height - 1);
                int x = Random.Shared.Next(1, width - 1);

                if (tiles[y, x].IsWall || tiles[y, x].Item != null || tiles[y, x].Enemy != null || tiles[y, x].IsCraftingStation)
                    continue;

                tiles[y, x].IsCraftingStation = true;
                placed++;
            }
        }
    }
}
