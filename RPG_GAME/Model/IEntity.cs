namespace RPG_GAME.Model
{
    /// <summary>
    /// Interface for entities that have position and can move
    /// </summary>
    public interface IEntity
    {
        Vec2 Position { get; }
        void MoveTo(Vec2 newPosition);
    }
}