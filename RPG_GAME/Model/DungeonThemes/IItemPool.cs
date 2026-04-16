using RPG_GAME.Model;

namespace RPG_GAME.Model.DungeonThemes
{
    public interface IItemPool
    {
        Items CreateRandomItem(Vec2 position);
    }
}