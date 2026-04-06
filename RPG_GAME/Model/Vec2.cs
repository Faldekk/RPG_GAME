using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG_GAME.Model.Map;
namespace RPG_GAME.Model
{
    public readonly struct Vec2
    {
        public int X { get; }
        public int Y { get; }

        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vec2 Add(int dx, int dy)
        {
            return new Vec2(X + dx, Y + dy);
        }
    }
}
