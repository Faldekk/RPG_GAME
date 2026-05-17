using RPG_GAME.Model.Combat;
using RPG_GAME.App.Logging;

namespace RPG_GAME.Model.Events
{
    public sealed class AggressiveReaction : ISpeciesDeathReaction
    {
        public void ReactToAllyDeath(Enemy survivor, Enemy deadEnemy)
        {
            int prevMin = survivor.AttackMin;
            int prevMax = survivor.AttackMax;
            int prevArmor = survivor.Armor;
            int prevHealth = survivor.Health;

            survivor.ModifyAttack(2, 2);
            survivor.ModifyArmor(1);
            survivor.ModifyHealth(5);

            GameLog.Info($"{survivor.Name} at ({survivor.Position.X},{survivor.Position.Y}) becomes enraged after {deadEnemy.Name} died. Attack {prevMin}-{prevMax} -> {survivor.AttackMin}-{survivor.AttackMax}, Armor {prevArmor} -> {survivor.Armor}, HP {prevHealth} -> {survivor.Health}.");
        }
    }
}
