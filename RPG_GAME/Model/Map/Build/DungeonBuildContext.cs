using System.Collections.Generic;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Map.Build
{
    public class DungeonBuildContext
    {
        public int Width { get; }
        public int Height { get; }
        public Tile[,] Tiles { get; }
        public List<RectRoom> Rooms { get; }
        public List<string> Features { get; }

        public DungeonBuildContext(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[height, width];
            Rooms = new List<RectRoom>();
            Features = new List<string>();
        }

        public void MarkFeature(string feature)
        {
            if (!Features.Contains(feature))
                Features.Add(feature);
        }
    }
}