using System;
using RPG_GAME.Model.Events;

namespace RPG_GAME.Model.Combat
{
    public sealed class EnemySpecies
    {
        public string Name { get; }
        public SpeciesDeathPublisher DeathPublisher { get; }
        public ISpeciesDeathReaction DeathReaction { get; }

        public EnemySpecies(string name, SpeciesDeathPublisher deathPublisher, ISpeciesDeathReaction deathReaction)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DeathPublisher = deathPublisher ?? throw new ArgumentNullException(nameof(deathPublisher));
            DeathReaction = deathReaction ?? throw new ArgumentNullException(nameof(deathReaction));
        }
    }

    public sealed class EnemySpeciesSpawnPlan
    {
        private readonly Func<Vec2, Enemy> _createEnemy;

        public EnemySpecies Species { get; }
        public int Count { get; }

        public EnemySpeciesSpawnPlan(EnemySpecies species, int count, Func<Vec2, Enemy> createEnemy)
        {
            if (count < 2)
                throw new ArgumentOutOfRangeException(nameof(count), "A species group must contain at least two enemies.");

            Species = species ?? throw new ArgumentNullException(nameof(species));
            Count = count;
            _createEnemy = createEnemy ?? throw new ArgumentNullException(nameof(createEnemy));
        }

        public Enemy CreateEnemy(Vec2 position)
        {
            return _createEnemy(position);
        }
    }
}
