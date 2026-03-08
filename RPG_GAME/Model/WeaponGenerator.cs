using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public static class WeaponGenerator
    {
        private static readonly Random _random = Random.Shared;
        
        private static readonly List<(string name, string type, int damage, bool twoHanded, bool isHeal)> _weaponTemplates = new()
        {
            ("Rusty Sword", "Melee", 5, false,false),
            ("Iron Axe", "Melee", 8, false, false),
            ("Wooden Staff", "Magic", 6, true, false),
            ("Great Sword", "Melee", 15, true, false),
            ("Dagger", "Melee", 3, false, false),
            ("Club", "Melee", 4, false,false),
            ("Orb of Heal", "Magic", 20, false, true),
            ("Bloody Mary", "Magic", 5, false, true),

        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            var template = _weaponTemplates[_random.Next(_weaponTemplates.Count)];
            
            return new Items(
                template.name,
                template.type,
                template.damage,
                template.twoHanded,
                template.isHeal,
                new Tuple<int, int>(position.X, position.Y)
            );
        }

        public static Items GenerateRandomWeapon(int x, int y)
        {
            return GenerateRandomWeapon(new Vec2(x, y));
        }
    }
}