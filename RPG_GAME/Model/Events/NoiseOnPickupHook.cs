using RPG_GAME.Model.Events;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model;

namespace RPG_GAME.Model.Events
{
    public static class NoiseOnPickupHook
    {
        public static void OnItemPickedUp(World world, Items item, INoiseEmitter emitter)
        {
            if (item == null || emitter == null) return;
            int range = 0;

            if (item.CanEquip)
            {
                var category = item.GetWeaponCategory();
                range = category.GetNoiseRange();
            }

            if (range > 0)
            {
                emitter.EmitNoise(world.Player.Pos, range, $"Player picked up {item.Name}");
            }
        }
    }
}
