using System;
using System.Collections.Generic;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.ItemModifiers;

namespace RPG_GAME.Model
{
    public static class WeaponGenerator
    {
        private static readonly Random _random = Random.Shared;

        private static readonly List<WeaponTemplate> _weaponTemplates = new()
        {
            new("Rusty Sword", "Melee", 5, false, HeavyWeaponCategory.Instance, StrengthBonus: 2, DexterityBonus: 1, AggressionBonus: 1, WisdomBonus: 0, LuckBonus: 0),
            new("Iron Axe", "Melee", 8, false, HeavyWeaponCategory.Instance, StrengthBonus: 3, DexterityBonus: 1, AggressionBonus: 1, WisdomBonus: 0, LuckBonus: 0),
            new("Wooden Staff", "Magic", 6, true, MagicalWeaponCategory.Instance, StrengthBonus: 0, DexterityBonus: 0, AggressionBonus: 0, WisdomBonus: 3, LuckBonus: 1),
            new("Great Sword", "Melee", 15, true, HeavyWeaponCategory.Instance, StrengthBonus: 5, DexterityBonus: 0, AggressionBonus: 1, WisdomBonus: 0, LuckBonus: 0),
            new("Dagger", "Melee", 3, false, LightWeaponCategory.Instance, StrengthBonus: 0, DexterityBonus: 2, AggressionBonus: 0, WisdomBonus: 0, LuckBonus: 2),
            new("Club", "Melee", 4, false, HeavyWeaponCategory.Instance, StrengthBonus: 2, DexterityBonus: 1, AggressionBonus: 1, WisdomBonus: 0, LuckBonus: 0),
            new("Arcane Wand", "Magic", 7, false, MagicalWeaponCategory.Instance, StrengthBonus: 0, DexterityBonus: 1, AggressionBonus: 0, WisdomBonus: 2, LuckBonus: 1),
            new("Crystal Tome", "Magic", 9, true, MagicalWeaponCategory.Instance, StrengthBonus: 0, DexterityBonus: 0, AggressionBonus: 0, WisdomBonus: 4, LuckBonus: 1)
        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            var template = _weaponTemplates[_random.Next(_weaponTemplates.Count)];
            var positionTuple = new Tuple<int, int>(position.X, position.Y);

            Items weapon = new WeaponItem(
                template.Name,
                template.DisplayType,
                template.Damage,
                template.IsTwoHanded,
                template.StrengthBonus,
                template.DexterityBonus,
                template.AggressionBonus,
                template.WisdomBonus,
                template.LuckBonus,
                template.Category,
                positionTuple);

            return ApplyRandomModifiers(weapon);
        }

        public static Items GenerateRandomWeapon(int x, int y)
        {
            return GenerateRandomWeapon(new Vec2(x, y));
        }

        private static Items ApplyRandomModifiers(Items weapon)
        {
            Items current = weapon;

            if (_random.NextDouble() < 0.35)
                current = new StrongModifierDecorator(current);

            if (_random.NextDouble() < 0.30)
                current = new BattleHardenedModifierDecorator(current);

            if (_random.NextDouble() < 0.20)
                current = new UnluckyModifierDecorator(current);

            return current;
        }

        private sealed record WeaponTemplate(
            string Name,
            string DisplayType,
            int Damage,
            bool IsTwoHanded,
            IWeaponCategory Category,
            int StrengthBonus,
            int DexterityBonus,
            int AggressionBonus,
            int WisdomBonus,
            int LuckBonus);
    }
}