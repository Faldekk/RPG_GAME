namespace RPG_GAME.Model
{
    /// <summary>
    /// Interface for items that can be equipped
    /// </summary>
    public interface IEquippable 
    {
        int Value { get; }
        bool IsTwoHanded { get; }
        int Durability { get; }
        void Use();
    }
}