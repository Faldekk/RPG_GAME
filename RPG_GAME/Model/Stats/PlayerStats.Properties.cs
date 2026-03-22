using System;

namespace RPG_GAME.Model
{
    public partial class PlayerStats
    {
        public int Strength
        {
            get => GetStat("Strength");
            set => SetStat("Strength", value);
        }

        public int Dexterity
        {
            get => GetStat("Dexterity");
            set => SetStat("Dexterity", value);
        }

        public int Health
        {
            get => GetStat("Health");
            set => SetStat("Health", Math.Max(0, value));
        }

        public int Luck
        {
            get => GetStat("Luck");
            set => SetStat("Luck", value);
        }

        public int Aggression
        {
            get => GetStat("Agression");
            set => SetStat("Agression", value);
        }

        public int Wisdom
        {
            get => GetStat("Wisdom");
            set => SetStat("Wisdom", value);
        }

        public int Coins
        {
            get => GetCurrency("Coins");
            set => SetCurrency("Coins", Math.Max(0, value));
        }

        public int Gold
        {
            get => GetCurrency("Gold");
            set => SetCurrency("Gold", Math.Max(0, value));
        }
    }
}
