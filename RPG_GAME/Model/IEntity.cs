namespace RPG_GAME.Model
{
    public interface IEntity
    {
        Vec2 Position { get; }
        void MoveTo(Vec2 newPosition);
    }
}