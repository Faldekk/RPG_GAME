using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace RPG_GAME.Model
{
    public class Player 
    {
        public Vec2 Pos { get; private set; }
        public Dictionary<string, int>? Stats;
        public Dictionary<string, int>? Income;
        public List<Items> Slots;


        public Player(Vec2 startPos, Dictionary<string, int> stats, Dictionary<string, int> income, List<Items> slots)
        {
            Pos = startPos;
            Stats = new Dictionary<string, int>
        {
            { "Strength", 10 },
            { "Dexterity", 10 },
            { "Health", 100 },
            { "Luck", 50 },
            { "Agression", 25 },
            { "Wisdom", 0 }
        };
            Income = new Dictionary<string, int>
            {
                { "Coins", 10 },
                {"Gold", 0 }
            };
            List<Items> Slots = slots;
            
        }
        public Player(Player player)
        {
            this.Pos = player.Pos;
            this.Stats = player.Stats;
            Income = player.Income;
            Slots = player.Slots;
        }
        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }
        
    }
}