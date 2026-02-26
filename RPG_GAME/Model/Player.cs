using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    /// <summary>
    /// Player character with stats, inventory and position
    /// </summary>
    public class Player
    {
        public PlayerStats Stats { get; private set; }
        public PlayerInventory Inventory { get; private set; }

        // Position
        public Vec2 Pos { get; private set; }

        // Legacy compatibility - for Renderer
        public Dictionary<string, int>? Income => ConvertToDictionary(Stats?.AllCurrency);
        public List<Items>? Slots => ConvertToItemsList();

        public Player(Vec2 startPos, Dictionary<string, int> stats, Dictionary<string, int> income, List<Items> slots)
        {
            Pos = startPos;
            Stats = new PlayerStats();
            Inventory = new PlayerInventory();

            // Initialize with existing slots if provided
            if (slots != null && slots.Count > 0)
            {
                if (slots.Count > 0 && slots[0] != null)
                    Inventory.EquipItem(slots[0], 0);
                if (slots.Count > 1 && slots[1] != null)
                    Inventory.EquipItem(slots[1], 1);
            }
        }

        public Player(Vec2 startPosition)
        {
            Pos = startPosition;
            Stats = new PlayerStats();
            Inventory = new PlayerInventory();
        }

        // Copy constructor
        public Player(Player player)
        {
            this.Pos = player.Pos;
            this.Stats = new PlayerStats();
            this.Inventory = new PlayerInventory();

            // Copy stats
            if (player.Stats != null)
            {
                foreach (var stat in player.Stats.AllStats)
                {
                    Stats.ModifyStat(stat.Key, stat.Value - Stats.AllStats[stat.Key]);
                }

                // Copy currency
                foreach (var currency in player.Stats.AllCurrency)
                {
                    Stats.ModifyCurrency(currency.Key, currency.Value - Stats.AllCurrency[currency.Key]);
                }
            }

            // Copy equipment
            if (player.Inventory != null)
            {
                if (player.Inventory.LeftHand != null)
                    Inventory.EquipItem(player.Inventory.LeftHand, 0);
                if (player.Inventory.RightHand != null)
                    Inventory.EquipItem(player.Inventory.RightHand, 1);
            }
        }

        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }

        /// <summary>
        /// Equips weapon to left (0) or right (1) hand
        /// </summary>
        public bool EquipWeapon(Items weapon, int handIndex = 0)
        {
            return Inventory.EquipItem(weapon, handIndex);
        }

        /// <summary>
        /// Unequips weapon from specified hand
        /// </summary>
        public Items? UnequipWeapon(int handIndex)
        {
            return Inventory.UnequipItem(handIndex);
        }

        /// <summary>
        /// Calculates total attack damage from strength + weapons
        /// </summary>
        public int GetAttackDamage()
        {
            int totalDamage = Stats.Strength;

            foreach (var weapon in Inventory.GetAllWeapons())
            {
                totalDamage += weapon.Damage_Heal;
            }

            return totalDamage;
        }

        /// <summary>
        /// Attacks another player
        /// </summary>
        public void Attack(Player target)
        {
            int damage = GetAttackDamage();
            target.Stats.TakeDamage(damage);
        }

        /// <summary>
        /// Heals the player
        /// </summary>
        public void Heal(int amount)
        {
            Stats.Heal(amount);
        }

        /// <summary>
        /// Checks if player is alive
        /// </summary>
        public bool IsAlive => Stats.IsAlive;

        // Helper methods for legacy compatibility
        private Dictionary<string, int>? ConvertToDictionary(IReadOnlyDictionary<string, int>? readOnlyDict)
        {
            if (readOnlyDict == null) return null;
            return new Dictionary<string, int>(readOnlyDict);
        }

        private List<Items>? ConvertToItemsList()
        {
            var list = new List<Items?>();

            if (Inventory.LeftHand != null)
                list.Add(Inventory.LeftHand);
            else
                list.Add(null);

            if (Inventory.RightHand != null)
                list.Add(Inventory.RightHand);
            else
                list.Add(null);

            return list;
        }

        public override string ToString()
        {
            return $"Player at {Pos} - HP: {Stats.Health}/{Stats.MaxHealth}, Coins: {Stats.Coins}";
        }
    }
}