using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public partial class PlayerStats
    {
        private readonly Dictionary<string, int> _stats;
        private readonly Dictionary<string, int> _currency;

        public int MaxHealth { get; private set; } = 100;
        public IReadOnlyDictionary<string, int> AllStats => _stats;
        public IReadOnlyDictionary<string, int> AllCurrency => _currency;
        public bool IsAlive => Health > 0;

        public PlayerStats()
        {
            _stats = new Dictionary<string, int>
            {
                { "Strength", 10 },
                { "Dexterity", 10 },
                { "Health", 100 },
                { "Luck", 50 },
                { "Agression", 25 },
                { "Wisdom", 0 },
                { "Armor", 0 }
            };

            _currency = new Dictionary<string, int>
            {
                { "Coins", 10 },
                { "Gold", 0 }
            };
        }

        private int GetStat(string statName)
        {
            return _stats.TryGetValue(statName, out int value) ? value : 0;
        }

        private void SetStat(string statName, int value)
        {
            _stats[statName] = value;
        }

        private int GetCurrency(string currencyName)
        {
            return _currency.TryGetValue(currencyName, out int value) ? value : 0;
        }

        private void SetCurrency(string currencyName, int value)
        {
            _currency[currencyName] = value;
        }
    }
}
