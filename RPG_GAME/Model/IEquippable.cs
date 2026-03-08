namespace RPG_GAME.Model
{

    public interface IEquippable 
    {
        int Value { get; }
        bool IsTwoHanded { get; }
        int Durability { get; }
        void Use();
    }
}