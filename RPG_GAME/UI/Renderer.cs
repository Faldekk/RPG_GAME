using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG_GAME.App;

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
                    char ch = world.GetTile(y, x).IsWall ? '█' : ' ';
                    _buffer.PutChar(y, x, ch);
                }
            }

            // gracz
            var p = world.Player.Pos;
            var s = world.Player.Stats;
            var m = world.Player.Income;
            _buffer.PutChar(p.Y, p.X, 'X');

            // panel boczny
            int panelX = World.Width + 2;
            _buffer.PutString(0, panelX, "Welcome to Dungeons :)");
            _buffer.PutString(1, panelX, "created by Faldekk");
            _buffer.PutString(3, panelX, "WASD - move");
            _buffer.PutString(5, panelX, "E - pick up the item");
            _buffer.PutString(6, panelX, "G - drop the item");
            int itt = 9;
            _buffer.PutString(9, panelX, "Money:");
            foreach (var money in m)
            {
                _buffer.PutString(itt, panelX, $"{money.Key}: {money.Value}");
                itt++;
            }
            itt++;
            _buffer.PutString(13, panelX, $"Stats: ");
            foreach (var stat in s){
                _buffer.PutString(itt, panelX, $"{stat.Key}: {stat.Value}");
                itt++;
            }
            itt++;
            //_buffer.PutString(14, panelX, $"Strength: {s["Strength"]}");
            //_buffer.PutString(15, panelX, $"Dexterity: {s["Dexterity"]}");
            //_buffer.PutString(16, panelX, $"Health: {s["Health"]}");
            //_buffer.PutString(17, panelX, $"Luck: {s["Luck"]}");
            //_buffer.PutString(18, panelX, $"Aggression: {s["Aggression"]}");
            //_buffer.PutString(19, panelX, $"Wisdom: {s["Wisdom"]}");




            _buffer.PutString(20, panelX, "Q - quit");

            _buffer.Flush();
        }
    }
}