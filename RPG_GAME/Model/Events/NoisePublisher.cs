using System.Collections.Generic;

namespace RPG_GAME.Model.Events
{
    public sealed class NoisePublisher : IGameEventPublisher<NoiseEvent>
    {
        private readonly List<IGameEventListener<NoiseEvent>> _listeners = new();

        public void Subscribe(IGameEventListener<NoiseEvent> listener)
        {
            if (listener == null) return;
            if (_listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        public void Unsubscribe(IGameEventListener<NoiseEvent> listener)
        {
            if (listener == null) return;
            _listeners.Remove(listener);
        }

        public void Publish(NoiseEvent gameEvent)
        {
            var snapshot = _listeners.ToArray();
            foreach (var listener in snapshot)
            {
                listener.OnEvent(gameEvent);
            }
        }
    }
}
