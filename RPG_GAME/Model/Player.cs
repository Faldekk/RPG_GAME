using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    
    //Character is crazy  a lot things
    public class Player
    {
        public PlayerStats Stats { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public Vec2 Pos { get; private set; }

        public bool IsAlive => Stats.IsAlive;

        public Player(Vec2 startPos, Dictionary<string, int> stats, Dictionary<string, int> income, List<Items> slots)
        {
            Pos = startPos;
            Stats = new PlayerStats();
            Inventory = new PlayerInventory();
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
        // Wojna z klonem ??? :)
        public Player(Player player)
        {
            this.Pos = player.Pos;
            this.Stats = new PlayerStats();
            this.Inventory = new PlayerInventory();
            if (player.Stats != null)
            {
                foreach (var stat in player.Stats.AllStats)
                {
                    Stats.ModifyStat(stat.Key, stat.Value - Stats.AllStats[stat.Key]);
                }
                foreach (var currency in player.Stats.AllCurrency)
                {
                    Stats.ModifyCurrency(currency.Key, currency.Value - Stats.AllCurrency[currency.Key]);
                }
            }
            if (player.Inventory != null)
            {
                if (player.Inventory.LeftHand != null)
                    Inventory.EquipItem(player.Inventory.LeftHand, 0);
                if (player.Inventory.RightHand != null)
                    Inventory.EquipItem(player.Inventory.RightHand, 1);
            }
        }
        //Self-explanatory ale interakcje w grze 
        public void MoveTo(Vec2 newPos)
        {
            Pos = newPos;
        }
        public bool EquipWeapon(Items weapon, int handIndex = 0)
        {
            return Inventory.EquipItem(weapon, handIndex);
        }
        public Items? UnequipWeapon(int handIndex)
        {
            return Inventory.UnequipItem(handIndex);
        }
        public void SwapWeapons()
        {
            var temp = Inventory.LeftHand;

            if (Inventory.LeftHand != null)
                Inventory.UnequipItem(0);

            if (Inventory.RightHand != null)
            {
                var rightWeapon = Inventory.UnequipItem(1);
                if (rightWeapon != null)
                    Inventory.EquipItem(rightWeapon, 0);
            }

            if (temp != null)
                Inventory.EquipItem(temp, 1);
        }
        public int GetAttackDamage()
        {
            int totalDamage = Stats.Strength;

            foreach (var weapon in Inventory.GetAllWeapons())
            {
                totalDamage += weapon.Damage_Heal;
            }

            return totalDamage;
        }

        public void Attack(Player target)
        {
            int damage = GetAttackDamage();
            target.Stats.TakeDamage(damage);
        }
        public void Heal(int amount)
        {
            Stats.Heal(amount);
        }

        public override string ToString()
        {
            return $"Player at {Pos} - HP: {Stats.Health}/{Stats.MaxHealth}, Coins: {Stats.Coins}";
        }
    }
}