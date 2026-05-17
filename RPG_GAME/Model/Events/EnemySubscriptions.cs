using RPG_GAME.Model.Events;
using RPG_GAME.Model.Sound;
using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model.Events
{
    public sealed class EnemySubscriptions
    {
        private readonly Enemy _enemy;
        private readonly NoisePublisher _noisePublisher;
        private readonly SpeciesDeathPublisher _speciesPublisher;
        private readonly EnemyNoiseListener _noiseListener;
        private readonly SpeciesDeathListener _speciesListener;

        public EnemySubscriptions(Enemy enemy, NoisePublisher noisePublisher, SpeciesDeathPublisher speciesPublisher, ISoundPropagation propagation, ISpeciesDeathReaction reaction)
        {
            _enemy = enemy;
            _noisePublisher = noisePublisher;
            _speciesPublisher = speciesPublisher;
            _noiseListener = new EnemyNoiseListener(enemy, propagation);
            _speciesListener = new SpeciesDeathListener(enemy, reaction);
        }

        public void Subscribe()
        {
            _noisePublisher.Subscribe(_noiseListener);
            _speciesPublisher.Subscribe(_speciesListener);
        }

        public void Unsubscribe()
        {
            _noisePublisher.Unsubscribe(_noiseListener);
            _speciesPublisher.Unsubscribe(_speciesListener);
        }

        public bool OwnedBy(Enemy enemy) => ReferenceEquals(_enemy, enemy);

        public void UnsubscribeIfOwner(Enemy enemy)
        {
            if (OwnedBy(enemy))
            {
                Unsubscribe();
            }
        }
    }
}
