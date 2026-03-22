using System;

namespace RPG_GAME.Model
{
    public partial class PlayerStats
    {
        public void ModifyStat(string statName, int amount)
        {
            if (_stats.ContainsKey(statName))
                _stats[statName] += amount;
        }

        public void ModifyCurrency(string currencyName, int amount)
        {
            if (_currency.ContainsKey(currencyName))
                _currency[currencyName] = Math.Max(0, _currency[currencyName] + amount);
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
