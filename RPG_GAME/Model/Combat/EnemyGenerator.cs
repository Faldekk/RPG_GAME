using System;
using System.Collections.Generic;

namespace RPG_GAME.Model.Combat
{
    public static class EnemyGenerator
    {
        private static readonly Random _random = Random.Shared;

        private static readonly List<Func<Vec2, Enemy>> _templates = new()
        {
            pos => new Enemy("Goblin", health: 80, attackMin: 10, attackMax: 16, armor: 7, position: pos, mapCharacter: 'G', attackType: new NormalAttackType()),
            pos => new Enemy("Bandit", health: 100, attackMin: 12, attackMax: 18, armor: 8, position: pos, mapCharacter: 'B', attackType: new StealthAttackType()),
            pos => new Enemy("Cultist", health: 95, attackMin: 11, attackMax: 17, armor: 6, position: pos, mapCharacter: 'C', attackType: new MagicalAttackType())
        };

        public static Enemy Generate(Vec2 position)
        {
            var template = _templates[_random.Next(_templates.Count)];
            return template(position);
        }
    }
}
