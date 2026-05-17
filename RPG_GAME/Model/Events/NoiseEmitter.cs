using RPG_GAME.Model.Sound;
using RPG_GAME.Model.Events;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Events
{
    public interface INoiseEmitter
    {
        void EmitNoise(Vec2 sourcePosition, int range, string description);
    }

    public sealed class NoiseEmitter : INoiseEmitter
    {
        private readonly NoisePublisher _publisher;

        public NoiseEmitter(NoisePublisher publisher)
        {
            _publisher = publisher;
        }

        public void EmitNoise(Vec2 sourcePosition, int range, string description)
        {
            var ev = new NoiseEvent(sourcePosition, range, description);
            _publisher.Publish(ev);
        }
    }
}
