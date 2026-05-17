using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class CowardlyReaction : ISpeciesDeathReaction
    {
        public void ReactToAllyDeath(Enemy survivor, Enemy deadEnemy)
        {
            // Reduce survivor's attack and armor modestly
            // As Enemy fields are immutable except Health/Position, we can't change AttackMin/Max/Armor directly
            // So we simulate by reducing Health slightly and logging the effect
            survivor.TakeDamage(5);
            GameLog.Info($"{survivor.Name} at ({survivor.Position.X},{survivor.Position.Y}) becomes fearful after {deadEnemy.Name} died. Health reduced by 5.");
        }
    }
}
