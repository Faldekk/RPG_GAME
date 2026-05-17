using RPG_GAME.Model;

namespace RPG_GAME.Model.Movement
{
    public interface IEnemyMovementStrategy
    {
        Vec2 ChooseNextPosition(RPG_GAME.Model.Combat.Enemy enemy, World world);
    }
}
