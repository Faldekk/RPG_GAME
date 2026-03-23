namespace RPG_GAME.Model.Map
{
    public struct RectRoom
    {
        // Struktura do trzymania info o prostokątnym pokoju
        public int X;      // Pozycja X (lewy górny róg)
        public int Y;      // Pozycja Y (lewy górny róg)
        public int Width;  // Szerokość
        public int Height; // Wysokość

        public RectRoom(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // Środek komnaty - tu będą korytarze połączone
        public int CenterX => X + Width / 2;
        public int CenterY => Y + Height / 2;

        // Sprawdzaj czy dwa pokoje się przenikają - wypadałoby nie
        public bool Intersects(RectRoom other)
        {
            return X <= other.X + other.Width &&
                   X + Width >= other.X &&
                   Y <= other.Y + other.Height &&
                   Y + Height >= other.Y;
        }
    }
}