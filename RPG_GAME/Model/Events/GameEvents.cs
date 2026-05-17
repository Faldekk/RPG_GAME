using RPG_GAME.Model;

namespace RPG_GAME.Model.Events
{
    public interface IGameEvent { }

    public interface IGameEventListener<TEvent>
        where TEvent : IGameEvent
    {
        void OnEvent(TEvent gameEvent);
    }

    public interface IGameEventPublisher<TEvent>
        where TEvent : IGameEvent
    {
        void Subscribe(IGameEventListener<TEvent> listener);
        void Unsubscribe(IGameEventListener<TEvent> listener);
        void Publish(TEvent gameEvent);
    }

    public sealed class NoiseEvent : IGameEvent
    {
        public Vec2 SourcePosition { get; }
        public int Range { get; }
        public string SourceDescription { get; }

        public NoiseEvent(Vec2 sourcePosition, int range, string sourceDescription)
        {
            SourcePosition = sourcePosition;
            Range = range;
            SourceDescription = sourceDescription;
        }
    }

    public sealed class SpeciesDeathEvent : IGameEvent
    {
        public RPG_GAME.Model.Combat.Enemy DeadEnemy { get; }
        public string SpeciesName { get; }

        public SpeciesDeathEvent(RPG_GAME.Model.Combat.Enemy deadEnemy, string speciesName)
        {
            DeadEnemy = deadEnemy;
            SpeciesName = speciesName;
        }
    }
}
