using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public static class ItemGenerator
    {
        private static readonly List<(string name, string type, int value, bool isHeal)> _nonWeaponTemplates = new()
        {
            ("Broken Spoon", "Junk", 1, false),
            ("Old Boot", "Junk", 2, false),
            ("Cracked Amulet", "Junk", 4, false),
            ("Herb Bundle", "Consumable", 15, true)
        };

        public static Items GenerateRandomWeapon(Vec2 position)
        {
            return WeaponGenerator.GenerateRandomWeapon(position);
        }

        public static Items GenerateRandomNonWeapon(Vec2 position)
        {
            int roll = Random.Shared.Next(100);
            if (roll < 30)
                return CreateCoins(position, Random.Shared.Next(5, 21));

            if (roll < 45)
                return CreateGold(position, Random.Shared.Next(1, 6));

            var template = _nonWeaponTemplates[Random.Shared.Next(_nonWeaponTemplates.Count)];
            return new Items(
                template.name,
                template.type,
                template.value,
                false,
                template.isHeal,
                new Tuple<int, int>(position.X, position.Y));
        }

        public static Items CreateCoins(Vec2 position, int amount)
        {
            return new Items("Coins", "Currency", amount, false, false, new Tuple<int, int>(position.X, position.Y));
        }

        public static Items CreateGold(Vec2 position, int amount)
        {
            return new Items("Gold", "Currency", amount, false, false, new Tuple<int, int>(position.X, position.Y));
        }
    }
}
