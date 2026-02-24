using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.Model
{
    public class Tile
    {
        public bool IsWall { get; }
        private bool IsWeapon { get; }

        public Tile(bool isWall)
        {
            IsWall = isWall;
        }
        public bool Weapon(bool IsWeapon)
        {
            return IsWeapon;
        }
    }
}
