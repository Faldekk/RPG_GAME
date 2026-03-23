using System;

namespace RPG_GAME.Model
{
    public abstract class Items : IEquippable
    {
        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public int Value { get; protected set; }
        public virtual bool IsTwoHanded => false;
        public virtual bool IsHeal => false;
        public Tuple<int, int>? Position { get; set; }
        public int Count;
        public int Durability { get; protected set; }
        public virtual char MapCharacter => 'x';
        public virtual bool CanEquip => false;

        protected Items(string name, string type, int value, Tuple<int, int>? position = null)
        {
            Name = name;
            Type = type;
            Value = value;
            Position = position;
            Count = 0;
            Durability = 100;
        }


        public virtual void Use()
        {
            if (Durability > 0)
                Durability -= 5;
        }


        public virtual bool TryCollect(Player player, out string message)
        {
            message = string.Empty;
            return false;
        }

        //kiedys to dodam bo to juz jest od 4 tyg w kodzie
        public virtual bool TryUse(Player player, out string message)
        {
            message = "Item cannot be used.";
            return false;
        }


        public virtual void ApplyEquipBonuses(PlayerStats stats)
        {
        }


        public virtual void RemoveEquipBonuses(PlayerStats stats)
        {
        }
    }

    public sealed class WeaponItem : Items
    {
        private readonly bool _isTwoHanded;
        private readonly int _strengthBonus;
        private readonly int _dexterityBonus;
        private readonly int _aggressionBonus;
        private readonly int _wisdomBonus;
        private readonly int _luckBonus;

        public override bool IsTwoHanded => _isTwoHanded;
        public override bool CanEquip => true;
        public override char MapCharacter => 'X';

        public WeaponItem(
            string name,
            string type,
            int value,
            bool isTwoHanded,
            int strengthBonus,
            int dexterityBonus,
            int aggressionBonus,
            int wisdomBonus,
            int luckBonus,
            Tuple<int, int>? position = null)
            : base(name, type, value, position)
        {
            _isTwoHanded = isTwoHanded;
            _strengthBonus = strengthBonus;
            _dexterityBonus = dexterityBonus;
            _aggressionBonus = aggressionBonus;
            _wisdomBonus = wisdomBonus;
            _luckBonus = luckBonus;
        }

        public override void ApplyEquipBonuses(PlayerStats stats)
        {
            stats.ModifyStat("Strength", _strengthBonus);
            stats.ModifyStat("Dexterity", _dexterityBonus);
            stats.ModifyStat("Agression", _aggressionBonus);
            stats.ModifyStat("Wisdom", _wisdomBonus);
            stats.ModifyStat("Luck", _luckBonus);
        }

        public override void RemoveEquipBonuses(PlayerStats stats)
        {
            stats.ModifyStat("Strength", -_strengthBonus);
            stats.ModifyStat("Dexterity", -_dexterityBonus);
            stats.ModifyStat("Agression", -_aggressionBonus);
            stats.ModifyStat("Wisdom", -_wisdomBonus);
            stats.ModifyStat("Luck", -_luckBonus);
        }
    }

    public sealed class JunkItem : Items
    {
        public override char MapCharacter => 'j';

        public JunkItem(string name, int value, Tuple<int, int>? position = null)
            : base(name, "Junk", value, position)
        {
        }
    }

    public sealed class HealItem : Items
    {
        public override bool IsHeal => true;
        public override char MapCharacter => '+';

        public HealItem(string name, int healAmount, Tuple<int, int>? position = null)
            : base(name, "Heal", healAmount, position)
        {
        }

        public override bool TryUse(Player player, out string message)
        {
            player.Heal(Value);
            message = $"Used {Name}.";
            return true;
        }
    }

    public sealed class MoneyItem : Items
    {
        private readonly string _currencyName;

        public override char MapCharacter => '$';

        public MoneyItem(string currencyName, int amount, Tuple<int, int>? position = null)
            : base(currencyName, "Currency", amount, position)
        {
            _currencyName = currencyName;
        }

        public override bool TryCollect(Player player, out string message)
        {
            if (_currencyName.Equals("Coins", StringComparison.OrdinalIgnoreCase))
            {
                player.Stats.ModifyCurrency("Coins", Value);
                message = $"Collected {Value} Coins.";
                return true;
            }

            if (_currencyName.Equals("Gold", StringComparison.OrdinalIgnoreCase))
            {
                player.Stats.ModifyCurrency("Gold", Value);
                message = $"Collected {Value} Gold.";
                return true;
            }

            message = string.Empty;
            return false;
        }

        public override bool TryUse(Player player, out string message)
        {
            return TryCollect(player, out message);
        }
    }
}