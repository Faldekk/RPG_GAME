using RPG_GAME.Model.Events;
using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class SpeciesDeathListener : IGameEventListener<SpeciesDeathEvent>
    {
        private readonly Enemy _enemy;
        private readonly ISpeciesDeathReaction _reaction;

        public SpeciesDeathListener(Enemy enemy, ISpeciesDeathReaction reaction)
        {
            _enemy = enemy;
            _reaction = reaction;
        }

        public void OnEvent(SpeciesDeathEvent gameEvent)
        {
            if (!_enemy.IsAlive) return;
            if (ReferenceEquals(gameEvent.DeadEnemy, _enemy)) return;

            _reaction.ReactToAllyDeath(_enemy, gameEvent.DeadEnemy);
        }
    }
}
