using System;

namespace RPG_GAME.Model
{
    public class Items : IEquippable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public bool IsTwoHanded { get; set; }
        public Tuple<int, int>? Position;
        public int Count;
        public int Durability { get; set; }

        public Items(string name, string type, int value, bool isTwoHanded, Tuple<int, int>? position = null)
        {
            Name = name;
            Type = type;
            Value = value;
            IsTwoHanded = isTwoHanded;
            Position = position;
            Count = 0;
            Durability = 100;
        }

        public void Use()
        {
            if (Durability > 0)
            {
                Durability--;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) - DMG/HEAL: {Value}, Durability: {Durability}%";
        }
    }
}