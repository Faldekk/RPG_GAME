using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class CowardlyReaction : ISpeciesDeathReaction
    {
        public void ReactToAllyDeath(Enemy survivor, Enemy deadEnemy)
        {
            // Weaken survivor: lower attack and armor
            int prevMin = survivor.AttackMin;
            int prevMax = survivor.AttackMax;
            int prevArmor = survivor.Armor;

            survivor.ModifyAttack(-2, -2);
            survivor.ModifyArmor(-1);
            survivor.ModifyHealth(-2);

            GameLog.Info($"{survivor.Name} at ({survivor.Position.X},{survivor.Position.Y}) becomes fearful after {deadEnemy.Name} died. Attack {prevMin}-{prevMax} -> {survivor.AttackMin}-{survivor.AttackMax}, Armor {prevArmor} -> {survivor.Armor}.");
        }
    }
}
