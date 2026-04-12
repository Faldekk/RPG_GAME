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
            new("Rusty Sword", "Melee", 5, false, HeavyWeaponCategory.Instance, 2, 1, 1, 0, 0),
            new("Iron Axe", "Melee", 8, false, HeavyWeaponCategory.Instance, 3, 1, 1, 0, 0),
            new("Wooden Staff", "Magic", 6, true, MagicalWeaponCategory.Instance, 0, 0, 0, 3, 1),
            new("Great Sword", "Melee", 15, true, HeavyWeaponCategory.Instance, 5, 0, 1, 0, 0),
            new("Dagger", "Melee", 3, false, LightWeaponCategory.Instance, 0, 2, 0, 0, 2),
            new("Club", "Melee", 4, false, HeavyWeaponCategory.Instance, 2, 1, 1, 0, 0),
            new("Arcane Wand", "Magic", 7, false, MagicalWeaponCategory.Instance, 0, 1, 0, 2, 1),
            new("Crystal Tome", "Magic", 9, true, MagicalWeaponCategory.Instance, 0, 0, 0, 4, 1)
        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            var template = _weaponTemplates[_random.Next(_weaponTemplates.Count)];
            var source = BuildDataSource(template, new Tuple<int, int>(position.X, position.Y));
            var data = source.Build();

            return new WeaponItem(
                data.Name,
                data.Type,
                data.Damage,
                data.IsTwoHanded,
                data.StrengthBonus,
                data.DexterityBonus,
                data.AggressionBonus,
                data.WisdomBonus,
                data.LuckBonus,
                data.Category,
                data.Position);
        }

        public static Items GenerateRandomWeapon(int x, int y)
        {
            return GenerateRandomWeapon(new Vec2(x, y));
        }

        private static IWeaponBuildDataSource BuildDataSource(WeaponTemplate template, Tuple<int, int> position)
        {
            IWeaponBuildDataSource source = new BaseWeaponBuildDataSource(new WeaponBuildData(
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
                position));

            if (_random.NextDouble() < 0.35)
                source = new StrongModifierDecorator(source);

            if (_random.NextDouble() < 0.30)
                source = new BattleHardenedModifierDecorator(source);

            if (_random.NextDouble() < 0.20)
                source = new UnluckyModifierDecorator(source);

            return source;
        }

        private sealed class BaseWeaponBuildDataSource : IWeaponBuildDataSource
        {
            private readonly WeaponBuildData _data;

            public BaseWeaponBuildDataSource(WeaponBuildData data)
            {
                _data = data;
            }

            public WeaponBuildData Build()
            {
                return _data;
            }
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
