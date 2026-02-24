using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.Model
{
    public class Player
    {
        public Vec2 Pos { get; private set; }

        public Player(Vec2 startPos)
        {
            Pos = startPos;
        }

        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }
    }
}