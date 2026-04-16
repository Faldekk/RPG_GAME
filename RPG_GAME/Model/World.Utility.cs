namespace RPG_GAME.Model
{
    public partial class World
    {
        public Items? CombineWeapons(Items weapon1, Items weapon2)
        {
            if (!weapon1.CanEquip || !weapon2.CanEquip)
            {
                AddMessage("Can only combine two weapons.");
                return null;
            }

            var combinedName = $"{weapon1.Name} & {weapon2.Name}";
            var combinedValue = weapon1.Value + weapon2.Value;

            var strengthBonus = GetStatBonus(weapon1, "Strength") + GetStatBonus(weapon2, "Strength");
            var dexterityBonus = GetStatBonus(weapon1, "Dexterity") + GetStatBonus(weapon2, "Dexterity");
            var aggressionBonus = GetStatBonus(weapon1, "Agression") + GetStatBonus(weapon2, "Agression");
            var wisdomBonus = GetStatBonus(weapon1, "Wisdom") + GetStatBonus(weapon2, "Wisdom");
            var luckBonus = GetStatBonus(weapon1, "Luck") + GetStatBonus(weapon2, "Luck");

            var combined = new WeaponItem(
                combinedName,
                "Combined",
                combinedValue,
                false,
                strengthBonus,
                dexterityBonus,
                aggressionBonus,
                wisdomBonus,
                luckBonus,
                weapon1.GetWeaponCategory()
            );

            AddMessage($"Combined into: {combined.Name} (DMG: {combined.Value})");
            return combined;
        }

        private int GetStatBonus(Items weapon, string statName)
        {
            var stats = new PlayerStats();
            weapon.ApplyEquipBonuses(stats);

            return statName switch
            {
                "Strength" => stats.Strength - 10,
                "Dexterity" => stats.Dexterity - 10,
                "Agression" => stats.Aggression - 25,
                "Wisdom" => stats.Wisdom,
                "Luck" => stats.Luck - 50,
                _ => 0
            };
        }

        public bool IsAtCraftingStation()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            return tile.IsCraftingStation;
        }

        public void Stop()
        {
            IsExitRequested = true;
        }

        public void Respawn()
        {
            Initialize();
        }
    }
}