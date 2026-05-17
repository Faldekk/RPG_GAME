using RPG_GAME.Model.Events;
using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model.Combat
{
    public sealed class EnemySpecies
    {
        public string Name { get; }
        public SpeciesDeathPublisher DeathPublisher { get; }
        public ISpeciesDeathReaction DeathReaction { get; }
        public System.Func<Vec2, Enemy> CreateEnemy { get; }

        public EnemySpecies(string name, SpeciesDeathPublisher deathPublisher, ISpeciesDeathReaction deathReaction, System.Func<Vec2, Enemy> createEnemy)
        {
            Name = name;
            DeathPublisher = deathPublisher;
            DeathReaction = deathReaction;
            CreateEnemy = createEnemy;
        }
    }
}
