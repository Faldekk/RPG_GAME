using System;
using System.Collections.Generic;
using RPG_GAME.Model.ItemModifiers;

namespace RPG_GAME.Model
{
    public static class WeaponGenerator
    {
        private static readonly Random _random = Random.Shared;

        private static readonly List<(string name, string type, int damage, bool twoHanded)> _weaponTemplates = new()
        {
            ("Rusty Sword", "Melee", 5, false),
            ("Iron Axe", "Melee", 8, false),
            ("Wooden Staff", "Magic", 6, true),
            ("Great Sword", "Melee", 15, true),
            ("Dagger", "Melee", 3, false),
            ("Club", "Melee", 4, false),
            ("Arcane Wand", "Magic", 7, false),
            ("Crystal Tome", "Magic", 9, true)
        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            var template = _weaponTemplates[_random.Next(_weaponTemplates.Count)];

            int strengthBonus = 0;
            int dexterityBonus = template.twoHanded ? 0 : 1;
            int aggressionBonus = 0;
            int wisdomBonus = 0;
            int luckBonus = 0;

            if (template.type.Equals("Melee", StringComparison.OrdinalIgnoreCase))
            {
                strengthBonus += Math.Max(1, template.damage / 4);
                aggressionBonus += 1;
                if (template.twoHanded)
                    strengthBonus += 2;
            }
            else
            {
                wisdomBonus += Math.Max(1, template.damage / 5);
                luckBonus += 1;
                if (template.twoHanded)
                    wisdomBonus += 2;
            }

            return BuildWeaponWithModifiers(
                template.name,
                template.type,
                template.damage,
                template.twoHanded,
                strengthBonus,
                dexterityBonus,
                aggressionBonus,
                wisdomBonus,
                luckBonus,
                new Tuple<int, int>(position.X, position.Y));
        }

        public static Items GenerateRandomWeapon(int x, int y)
        {
            return GenerateRandomWeapon(new Vec2(x, y));
        }

        private static Items BuildWeaponWithModifiers(
            string name,
            string type,
            int damage,
            bool isTwoHanded,
            int strengthBonus,
            int dexterityBonus,
            int aggressionBonus,
            int wisdomBonus,
            int luckBonus,
            Tuple<int, int> position)
        {
            IWeaponBuildDataSource source = new BaseWeaponBuildDataSource(new WeaponBuildData(
                name,
                type,
                damage,
                isTwoHanded,
                strengthBonus,
                dexterityBonus,
                aggressionBonus,
                wisdomBonus,
                luckBonus,
                position));

            source = ApplyRandomModifiers(source);
            var data = source.Build();

            return new WeaponItem(
                data.Name,
                data.Type,
                data.Damage,
                data.IsTwoHanded,
                data.StrengthBonus,
                data.DexterityBonus,
                data.AggressionBonus,
                wisdomBonus,
                luckBonus,
                position);
        }

        private static IWeaponBuildDataSource ApplyRandomModifiers(IWeaponBuildDataSource source)
        {
            if (_random.NextDouble() < 0.35)
                source = new StrongModifierDecorator(source);

            if (_random.NextDouble() < 0.30)
                source = new BattleHardenedModifierDecorator(source);

            return source;
        }

        private sealed class BaseWeaponBuildDataSource : IWeaponBuildDataSource
        {
            private readonly WeaponBuildData _data;

            public BaseWeaponBuildDataSource(WeaponBuildData data)
            {
                _data = data;
            }

            public WeaponBuildData Build() => _data;
        }
    }
}