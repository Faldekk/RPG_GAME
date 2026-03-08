using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    
    public class PlayerStats
    {
        private readonly Dictionary<string, int> _stats;
        private readonly Dictionary<string, int> _currency;
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

        public int MaxHealth { get; private set; } = 100;

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
                { "Wisdom", 0 }
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

        public void ModifyStat(string statName, int amount)
        {
            if (_stats.ContainsKey(statName))
            {
                _stats[statName] += amount;
            }
        }

        public void ModifyCurrency(string currencyName, int amount)
        {
            if (_currency.ContainsKey(currencyName))
            {
                _currency[currencyName] = Math.Max(0, _currency[currencyName] + amount);
            }
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }
    }
}