namespace RPG_GAME.Model
{
    // przydaje sie potem do zmiany itemow sie przyda jeszcze
    public interface IEquippable 
    {
        int Value { get; }
        bool IsTwoHanded { get; }
        int Durability { get; }
        void Use();
    }
}