using System;
using RPG_GAME.Model.Events;

namespace RPG_GAME.Model.Combat
{
    public sealed class Enemy
    {
        private readonly Random _random;

        public string Name { get; }
        public string SpeciesKey { get; }
        public int Health { get; private set; }
        public int AttackMin { get; }
        public int AttackMax { get; }
        public int Armor { get; private set; }
        public Vec2 Position { get; private set; }
        public char MapCharacter { get; }
        public bool IsAlive => Health > 0;
        public IAttackType AttackType { get; }

        // Optional species-level event publisher and reaction strategy
        public SpeciesDeathPublisher? SpeciesPublisher { get; }
        public ISpeciesDeathReaction? SpeciesReaction { get; }

        public Enemy(string speciesKey, string name, int health, int attackMin, int attackMax, int armor, Vec2 position, char mapCharacter, IAttackType attackType, SpeciesDeathPublisher? speciesPublisher = null, ISpeciesDeathReaction? speciesReaction = null)
        {
            SpeciesKey = speciesKey ?? name;
            Name = name;
            Health = Math.Max(1, health);
            AttackMin = Math.Max(1, attackMin);
            AttackMax = Math.Max(AttackMin, attackMax);
            Armor = Math.Max(0, armor);
            Position = position;
            MapCharacter = mapCharacter;
            AttackType = attackType;
            SpeciesPublisher = speciesPublisher;
            SpeciesReaction = speciesReaction;
            _random = Random.Shared;
        }

        public int RollAttackDamage()
        {
            return _random.Next(AttackMin, AttackMax + 1);
        }

        public void TakeDamage(int amount)
        {
            int dmg = Math.Max(0, amount);
            Health = Math.Max(0, Health - dmg);
        }

        public void ModifyHealth(int delta)
        {
            Health = Math.Max(0, Health + delta);
        }

        public void MoveTo(Vec2 position)
        {
            Position = position;
        }
    }
}
