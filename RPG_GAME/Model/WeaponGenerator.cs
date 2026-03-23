using System;
using System.Collections.Generic;

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

            return new WeaponItem(
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
    }
}