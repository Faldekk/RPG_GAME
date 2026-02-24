using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.Model
{
    public class Tile
    {
        public bool IsWall { get; }

        public Tile(bool isWall)
        {
            IsWall = isWall;
        }
    }
}
