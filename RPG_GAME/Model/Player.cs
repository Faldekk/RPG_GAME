using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.Model
{
    public class Player
    {
        public Vec2 Pos { get; private set; }
        public Dictionary<string, int>? Stats;
        public Dictionary<string, int>? Income;

        public Player(Vec2 startPos, Dictionary<string,int> stats, Dictionary<string, int> income)
        {
            Pos = startPos;
            Stats = stats;
            Income = income;
        }

        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }
    }
}