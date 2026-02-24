using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.Model
{
     class Items
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Damage { get; set; }
        public bool Both_hands { get; set; }
        public Tuple<int, int> Position;

        public Items(string name, string type, int damage, bool both_hands, Tuple<int,int> position) {
        Name = name;
        Type = type;
        Damage = Damage;
        Both_hands = both_hands;
        Position = position;
        }
        
    }
}
