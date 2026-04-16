using RPG_GAME.Model;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model.DungeonThemes
{
    public interface IDungeonTheme
    {
        string Name { get; }
        string IntroMessage { get; }

        IDungeonStrategy CreateDungeonStrategy();
        IItemPool CreateItemPool();
        IEnemyFactory CreateEnemyFactory();
        Items CreateArtifact(Vec2 position);
    }
}