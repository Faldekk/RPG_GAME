using RPG_GAME.Model;

namespace RPG_GAME.Model.Map
{
    public static class MapUtils
    {
        public static void CarveRoom(Tile[,] tiles, RectRoom room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    tiles[y, x].IsWall = false;
                }
            }
        }
        public static void CarveHorizontalTunnel(Tile[,] tiles, int x1, int x2, int y)
        {
            for (int x = System.Math.Min(x1, x2); x <= System.Math.Max(x1, x2); x++)
            {
                tiles[y, x].IsWall = false;
            }
        }
        public static void CarveVerticalTunnel(Tile[,] tiles, int y1, int y2, int x)
        {
            for (int y = System.Math.Min(y1, y2); y <= System.Math.Max(y1, y2); y++)
            {
                tiles[y, x].IsWall = false;
            }
        }
        public static void CarveCorridor(Tile[,] tiles, int fromX, int fromY, int toX, int toY)
        {
            if (Random.Shared.Next(2) == 0)
            {
               
                CarveHorizontalTunnel(tiles, fromX, toX, fromY);
                CarveVerticalTunnel(tiles, fromY, toY, toX);
            }
            else
            {
               
                CarveVerticalTunnel(tiles, fromY, toY, fromX);
                CarveHorizontalTunnel(tiles, fromX, toX, toY);
            }
        }
    }
}