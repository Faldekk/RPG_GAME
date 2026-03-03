using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    /// <summary>
    /// Simple weapon generator - easy to refactor later
    /// </summary>
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
            ("War Hammer", "Melee", 12, true),
            ("Short Bow", "Ranged", 7, true),
            ("Club", "Melee", 4, false)
        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            var template = _weaponTemplates[_random.Next(_weaponTemplates.Count)];
            
            return new Items(
                template.name,
                template.type,
                template.damage,
                template.twoHanded,
                new Tuple<int, int>(position.X, position.Y)
            );
        }

        public static Items GenerateRandomWeapon(int x, int y)
        {
            return GenerateRandomWeapon(new Vec2(x, y));
        }
    }
}