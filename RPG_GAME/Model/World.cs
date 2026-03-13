using RPG_GAME.Model.Instructions;
using RPG_GAME.Model.Map.Build;
using RPG_GAME.Model.Map.Build.Strategies;

namespace RPG_GAME.Model
{
    public class World
    {
        public const int Height = 20;
        public const int Width = 40;

        private readonly Tile[,] _tiles;

        public Player Player { get; }
        public InstructionCatalog Instructions { get; }

        public World()
            : this(new DungeonGroundsStrategy())
        {
        }

        public World(IDungeonBuildStrategy strategy)
        {
            Player = new Player(new Vec2(1, 1));

            var builder = strategy.CreateBuilder();
            var context = builder.Build(Width, Height);

            _tiles = context.Tiles;
            Instructions = builder.BuildInstructions();

            SpawnPlayer(context);
        }

        private void SpawnPlayer(DungeonBuildContext context)
        {
            if (context.Rooms.Count > 0)
            {
                var start = context.Rooms[0];
                Player.MoveTo(new Vec2(start.CenterX, start.CenterY));
                return;
            }

            _tiles[1, 1].IsWall = false;
            Player.MoveTo(new Vec2(1, 1));
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