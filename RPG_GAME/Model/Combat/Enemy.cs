using System;

namespace RPG_GAME.Model.Combat
{
    public sealed class Enemy
    {
        private readonly Random _random;

        public string Name { get; }
        public int Health { get; private set; }
        public int AttackMin { get; }
        public int AttackMax { get; }
        public int Armor { get; }
        public Vec2 Position { get; private set; }
        public char MapCharacter { get; }
        public bool IsAlive => Health > 0;
        public IAttackType AttackType { get; }

        public Enemy(string name, int health, int attackMin, int attackMax, int armor, Vec2 position, char mapCharacter, IAttackType attackType)
        {
            Name = name;
            Health = Math.Max(1, health);
            AttackMin = Math.Max(1, attackMin);
            AttackMax = Math.Max(AttackMin, attackMax);
            Armor = Math.Max(0, armor);
            Position = position;
            MapCharacter = mapCharacter;
            AttackType = attackType;
            _random = Random.Shared;
        }

        public int RollAttackDamage()
        {
            return _random.Next(AttackMin, AttackMax + 1);
        }

        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - Math.Max(0, amount));
        }

        public void MoveTo(Vec2 position)
        {
            Position = position;
        }
    }
}
