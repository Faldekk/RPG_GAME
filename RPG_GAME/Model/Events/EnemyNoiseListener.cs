using System;
using RPG_GAME.Model.Events;
using RPG_GAME.Model.Sound;
using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class EnemyNoiseListener : IGameEventListener<NoiseEvent>
    {
        private readonly Enemy _enemy;
        private readonly ISoundPropagation _propagation;

        public EnemyNoiseListener(Enemy enemy, ISoundPropagation propagation)
        {
            _enemy = enemy;
            _propagation = propagation;
        }

        public void OnEvent(NoiseEvent gameEvent)
        {
            if (!_enemy.IsAlive)
                return;

            var enemyPos = _enemy.Position;
            var distance = _propagation.GetDistanceIfReachable(gameEvent.SourcePosition, enemyPos, gameEvent.Range, WorldAccessor.Tiles);
            if (distance.HasValue)
            {
                GameLog.Info($"{_enemy.Name} at ({enemyPos.X},{enemyPos.Y}) heard noise '{gameEvent.SourceDescription}' from ({gameEvent.SourcePosition.X},{gameEvent.SourcePosition.Y}) at distance {distance.Value}.");
            }
        }
    }
}
