using System;

namespace RPG_GAME.Model
{
    //Tile representant XDDDD
    public class Tile
    {
        //public int x , y;
        public bool IsWall { get; set; }
        public Items? Item { get; set; }
        public Vec2 Position { get; set; }

        public Tile(bool isWall)
        {

            IsWall = isWall;
            Item = null;


        }

        public bool HasItem  { get; set; }
    }
}