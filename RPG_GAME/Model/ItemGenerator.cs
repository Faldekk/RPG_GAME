using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public static class ItemGenerator
    {
       
        private static readonly List<(string name, int value)> _junkTemplates = new()
        {
            ("Broken Spoon", 1),
            ("Old Boot", 2),
            ("Cracked Amulet", 4)
        };
        private static readonly List<(string name, int heal)> _healTemplates = new()
        {
            ("Herb Bundle", 15),
            ("Small Potion", 20)
        };
        public static Items GenerateRandomWeapon(Vec2 position)
        {
            return WeaponGenerator.GenerateRandomWeapon(position);
        }  
        public static Items GenerateRandomJunk(Vec2 position)
        {
            var junk = _junkTemplates[Random.Shared.Next(_junkTemplates.Count)];
            return new JunkItem(junk.name, junk.value, new Tuple<int, int>(position.X, position.Y));
        }
        public static Items GenerateRandomNonWeapon(Vec2 position)
        {
            int roll = Random.Shared.Next(100);
            
            if (roll < 30)
                return CreateCoins(position, Random.Shared.Next(5, 21));

            
            if (roll < 45)
                return CreateGold(position, Random.Shared.Next(1, 6));

           
            if (roll < 70)
            {
                var heal = _healTemplates[Random.Shared.Next(_healTemplates.Count)];
                return new HealItem(heal.name, heal.heal, new Tuple<int, int>(position.X, position.Y));
            }

            
            return GenerateRandomJunk(position);
        }

       
        public static Items CreateCoins(Vec2 position, int amount)
        {
            return new MoneyItem("Coins", amount, new Tuple<int, int>(position.X, position.Y));
        }

        public static Items CreateGold(Vec2 position, int amount)
        {
            return new MoneyItem("Gold", amount, new Tuple<int, int>(position.X, position.Y));
        }
    }
}
