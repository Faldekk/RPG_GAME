using System;

namespace RPG_GAME.Model
{
    public class Items
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Damage_Heal { get; set; }
        public bool Both_hands { get; set; }
        public Tuple<int, int>? Position;
        public int Count;
        public int Lifespan;

        // Convenient properties
        public int Value => Damage_Heal;
        public bool IsTwoHanded => Both_hands;
        public int Durability => Lifespan;
        public bool CanPickUp => Lifespan > 0;

        public Items(string name, string type, int damage, bool both_hands, Tuple<int, int>? position = null)
        {
            Name = name;
            Type = type;
            Damage_Heal = damage; // FIXED: by³o Damage_Heal = Damage_Heal
            Both_hands = both_hands;
            Position = position;
            Count = 0;
            Lifespan = 100;
        }

        public void Use()
        {
            if (Lifespan > 0)
            {
                Lifespan--;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) - DMG/HEAL: {Damage_Heal}, Durability: {Lifespan}%";
        }
    }
}