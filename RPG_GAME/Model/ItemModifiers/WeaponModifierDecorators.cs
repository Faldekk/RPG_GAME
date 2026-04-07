using System;
using RPG_GAME.Model.Combat;

namespace RPG_GAME.Model.ItemModifiers
{
    public abstract class ItemModifierDecorator : Items
    {
        protected Items Inner { get; }
        private Tuple<int, int>? _positionShadow;

        protected ItemModifierDecorator(Items inner)
            : base(inner.Name, inner.Type, inner.Value, inner.Position)
        {
            Inner = inner;
            _positionShadow = inner.Position;
        }

        public override string Name => Inner.Name;
        public override string Type => Inner.Type;
        public override int Value => Inner.Value;
        public override bool IsTwoHanded => Inner.IsTwoHanded;
        public override bool IsHeal => Inner.IsHeal;
        public override Tuple<int, int>? Position
        {
            get => Inner is null ? _positionShadow : Inner.Position;
            set
            {
                if (Inner is null)
                    _positionShadow = value;
                else
                    Inner.Position = value;
            }
        }
        public override int Durability
        {
            get => Inner.Durability;
            protected set { }
        }
        public override char MapCharacter => Inner.MapCharacter;
        public override bool CanEquip => Inner.CanEquip;

        public override void Use() => Inner.Use();
        public override bool TryCollect(Player player, out string message) => Inner.TryCollect(player, out message);
        public override bool TryUse(Player player, out string message) => Inner.TryUse(player, out message);
        public override void ApplyEquipBonuses(PlayerStats stats) => Inner.ApplyEquipBonuses(stats);
        public override void RemoveEquipBonuses(PlayerStats stats) => Inner.RemoveEquipBonuses(stats);
        public override IWeaponCategory GetWeaponCategory() => Inner.GetWeaponCategory();
    }

    public sealed class StrongModifierDecorator : ItemModifierDecorator
    {
        public StrongModifierDecorator(Items inner) : base(inner) { }

        public override string Name => $"{Inner.Name} (Strong)";
        public override int Value => Inner.Value + 5;
    }

    public sealed class BattleHardenedModifierDecorator : ItemModifierDecorator
    {
        public BattleHardenedModifierDecorator(Items inner) : base(inner) { }

        public override string Name => $"{Inner.Name} (BattleHardened)";

        public override void ApplyEquipBonuses(PlayerStats stats)
        {
            Inner.ApplyEquipBonuses(stats);
            stats.ModifyStat("Agression", 3);
        }

        public override void RemoveEquipBonuses(PlayerStats stats)
        {
            Inner.RemoveEquipBonuses(stats);
            stats.ModifyStat("Agression", -3);
        }
    }

    public sealed class UnluckyModifierDecorator : ItemModifierDecorator
    {
        public UnluckyModifierDecorator(Items inner) : base(inner) { }

        public override string Name => $"{Inner.Name} (Unlucky)";

        public override void ApplyEquipBonuses(PlayerStats stats)
        {
            Inner.ApplyEquipBonuses(stats);
            stats.ModifyStat("Luck", -4);
        }

        public override void RemoveEquipBonuses(PlayerStats stats)
        {
            Inner.RemoveEquipBonuses(stats);
            stats.ModifyStat("Luck", 4);
        }
    }
}
