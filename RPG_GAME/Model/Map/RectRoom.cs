namespace RPG_GAME.Model.Map
{
    public struct RectRoom
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public RectRoom(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int CenterX => X + Width / 2;
        public int CenterY => Y + Height / 2;

        public bool Intersects(RectRoom other)
        {
            return X <= other.X + other.Width &&
                   X + Width >= other.X &&
                   Y <= other.Y + other.Height &&
                   Y + Height >= other.Y;
        }
    }
}