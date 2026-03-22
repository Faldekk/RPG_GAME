using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model
{
    public class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;

        public Player Player { get; }

        public World()
            : this(new DungeonGroundsStrategy())
        {
        }

        public World(IDungeonStrategy strategy)
        {
            _tiles = new Tile[Height, Width];
            Player = new Player(new Vec2(1, 1));

            var context = strategy.Build(_tiles, Width, Height);
            SpawnPlayer(context);
            SpawnRandomWeapons(10);
        }

        private void SpawnPlayer(BuildContext context)
        {
            if (context.CentralRoom.HasValue)
            {
                var centralRoom = context.CentralRoom.Value;
                Player.MoveTo(new Vec2(centralRoom.CenterX, centralRoom.CenterY));
                return;
            }

            if (context.Rooms.Count > 0)
            {
                var startRoom = context.Rooms[0];
                Player.MoveTo(new Vec2(startRoom.CenterX, startRoom.CenterY));
                return;
            }

            _tiles[1, 1].IsWall = false;
            Player.MoveTo(new Vec2(1, 1));
        }

        private void SpawnRandomWeapons(int count)
        {
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = count * 20;

            while (spawned < count && attempts < maxAttempts)
            {
                int randomY = Random.Shared.Next(1, Height - 1);
                int randomX = Random.Shared.Next(1, Width - 1);

                if (!_tiles[randomY, randomX].IsWall && !_tiles[randomY, randomX].HasItem)
                {
                    _tiles[randomY, randomX].Item = WeaponGenerator.GenerateRandomWeapon(randomX, randomY);
                    spawned++;
                }

                attempts++;
            }
        }

        public Tile GetTile(int y, int x) => _tiles[y, x];

        public bool TryMovePlayer(int dx, int dy)
        {
            var next = Player.Pos.Add(dx, dy);

            if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
                return false;

            if (_tiles[next.Y, next.X].IsWall)
                return false;

            Player.MoveTo(next);
            return true;
        }

        public bool TryPickUpItem()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            var item = tile.Item;
            if (item == null) return false;

            if (item.IsTwoHanded && Player.Inventory.LeftHand != null && Player.Inventory.RightHand != null)
                return false;

            int hand = item.IsTwoHanded
                ? (Player.Inventory.LeftHand != null ? 0 : Player.Inventory.RightHand != null ? 1 : 0)
                : (Player.Inventory.LeftHand == null ? 0 : Player.Inventory.RightHand == null ? 1 : 0);

            var displaced = Player.Inventory.UnequipItem(hand);

            if (!Player.Inventory.EquipItem(item, hand))
            {
                if (displaced != null)
                    Player.Inventory.EquipItem(displaced, hand);
                return false;
            }

            tile.Item = displaced;

            if (displaced != null)
                displaced.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);

            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.HasItem) return false;

            var item = Player.Inventory.UnequipItem(handIndex);
            if (item == null) return false;

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            return true;
        }
    }
}