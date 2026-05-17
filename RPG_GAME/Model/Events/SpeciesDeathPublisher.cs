using System.Collections.Generic;

namespace RPG_GAME.Model.Events
{
    public sealed class SpeciesDeathPublisher : IGameEventPublisher<SpeciesDeathEvent>
    {
        private readonly List<IGameEventListener<SpeciesDeathEvent>> _listeners = new();

        public void Subscribe(IGameEventListener<SpeciesDeathEvent> listener)
        {
            if (listener == null) return;
            if (_listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        public void Unsubscribe(IGameEventListener<SpeciesDeathEvent> listener)
        {
            if (listener == null) return;
            _listeners.Remove(listener);
        }

        public void Publish(SpeciesDeathEvent gameEvent)
        {
            var snapshot = _listeners.ToArray();
            foreach (var listener in snapshot)
            {
                listener.OnEvent(gameEvent);
            }
        }
    }
}
