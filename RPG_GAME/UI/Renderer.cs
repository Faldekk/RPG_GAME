using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RPG_GAME.Model;

namespace RPG_GAME.UI
{
    public class Renderer
    {
        private readonly ConsoleBuffer _buffer;

        public Renderer()
        {
            _buffer = new ConsoleBuffer(World.Height + 2, World.Width + 30);
        }

        public void Render(World world)
        {
            _buffer.Clear();

            // mapa
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    char ch = world.GetTile(y, x).IsWall ? '#' : '.';
                    _buffer.PutChar(y, x, ch);
                }
            }

            // gracz
            var p = world.Player.Pos;
            _buffer.PutChar(p.Y, p.X, '@');

            // panel boczny
            int panelX = World.Width + 2;
            _buffer.PutString(1, panelX, "RPG GAME");
            _buffer.PutString(3, panelX, $"Pos X: {p.X}");
            _buffer.PutString(4, panelX, $"Pos Y: {p.Y}");
            _buffer.PutString(6, panelX, "WASD - move");
            _buffer.PutString(7, panelX, "Q - quit");

            _buffer.Flush();
        }
    }
}