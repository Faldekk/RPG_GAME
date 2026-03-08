namespace RPG_GAME.Model
{ 
    //Nie uzywam tego (w przyszlosci moze uzyje)
    public interface IEntity
    {
        Vec2 Position { get; }
        void MoveTo(Vec2 newPosition);
    }
}