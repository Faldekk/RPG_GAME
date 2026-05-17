using RPG_GAME.Model;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.Events;

namespace RPG_GAME.Model.DungeonThemes
{
    public interface IEnemyFactory
    {
        Enemy CreateRandomEnemy(Vec2 position, SpeciesDeathPublisher speciesPublisher);
    }
}