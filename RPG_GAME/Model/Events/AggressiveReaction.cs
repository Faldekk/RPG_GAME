using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class AggressiveReaction : ISpeciesDeathReaction
    {
        public void ReactToAllyDeath(Enemy survivor, Enemy deadEnemy)
        {
            // Aggressive reaction: heal survivor a bit to simulate enragement
            survivor.TakeDamage(-10); // negative damage = heal
            GameLog.Info($"{survivor.Name} at ({survivor.Position.X},{survivor.Position.Y}) becomes enraged after {deadEnemy.Name} died. Heals 10 HP.");
        }
    }
}
