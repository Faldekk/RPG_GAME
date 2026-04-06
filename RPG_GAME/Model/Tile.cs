using System;
using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model
{
    public class Tile
    {
        public bool IsWall { get; set; }
        public Items? Item { get; set; }
        public Enemy? Enemy { get; set; }
        public bool IsCraftingStation { get; set; }
        public bool HasItem => Item != null;  
        public bool HasEnemy => Enemy != null;
        public Vec2 Position { get; set; }

        public Tile(bool isWall)
        {
            IsWall = isWall;
            Item = null;
            Enemy = null;
            IsCraftingStation = false;
        }
    }
}