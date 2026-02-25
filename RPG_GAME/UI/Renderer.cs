using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            
            // map render
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {

                    char ch = world.GetTile(y, x).IsWall ? '█' : ' ';
                    
                    _buffer.PutChar(y, x, ch);
                    
                }
            }

            // player att
            var p = world.Player.Pos;
            var s = world.Player.Stats;
            var m = world.Player.Income;
            var w = world.Player.Slots;
            _buffer.PutChar(p.Y, p.X, 'X');

            // Stat panel and rules fwaAaaaEEEEEEEh
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

            if (w == null || w.Count == 0)
            {
                _buffer.PutString(itt, panelX, "No weapons yet. Grind more");
            }
            else
            {
                var left = w.Count > 0 ? w[0] : null;
                var right = w.Count > 1 ? w[1] : null;

                // jeśli lewy slot ma broń 2-ręczną
                if (left != null && left.Both_hands)
                {
                    _buffer.PutString(itt, panelX,
                        $"2-handed weapon equipped: {left.Name} Type: {left.Type} Damage: {left.Damage}");
                }
                else if (right != null && right.Both_hands)
                {
                    _buffer.PutString(itt, panelX,
                        $"2-handed weapon equipped: {right.Name} Type: {right.Type} Damage: {right.Damage}");
                }
                else
                {
                    if (left != null)
                    {
                        _buffer.PutString(itt, panelX,
                            $"Left hand: {left.Name} Type: {left.Type} Damage: {left.Damage}");
                        itt++;
                    }
                    else
                    {
                        _buffer.PutString(itt, panelX, "Left hand: (empty)");
                        itt++;
                    }

                    if (right != null)
                    {
                        _buffer.PutString(itt, panelX,
                            $"Right hand: {right.Name} Type: {right.Type} Damage: {right.Damage}");
                    }
                    else
                    {
                        _buffer.PutString(itt, panelX, "Right hand: (empty)");
                    }
                }
            }
            itt++;
            _buffer.PutString(itt++, panelX, "Q - quit");
            _buffer.Flush();
            }
    }
}