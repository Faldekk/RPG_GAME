using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model.Events
{
    public interface ISpeciesDeathReaction
    {
        void ReactToAllyDeath(Enemy survivor, Enemy deadEnemy);
    }
}
