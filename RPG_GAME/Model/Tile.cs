using System;

namespace RPG_GAME.Model
{
    public class Tile
    {
        public bool IsWall { get; }
        public Items? Item { get; set; }

        public Tile(bool isWall)
        {
            IsWall = isWall;
            Item = null;
        }

        public bool HasItem => Item != null;
    }
}