using System;

namespace RPG_GAME.Model.ItemModifiers
{
    public sealed record WeaponBuildData(
        string Name,
        string Type,
        int Damage,
        bool IsTwoHanded,
        int StrengthBonus,
        int DexterityBonus,
        int AggressionBonus,
        int WisdomBonus,
        int LuckBonus,
        Tuple<int, int>? Position);

    public interface IWeaponBuildDataSource
    {
        WeaponBuildData Build();
    }

    public abstract class WeaponBuildDataSourceDecorator : IWeaponBuildDataSource
    {
        private readonly IWeaponBuildDataSource _inner;

        protected WeaponBuildDataSourceDecorator(IWeaponBuildDataSource inner)
        {
            _inner = inner;
        }

        public WeaponBuildData Build()
        {
            return Transform(_inner.Build());
        }

        protected abstract WeaponBuildData Transform(WeaponBuildData data);
    }

    public sealed class StrongModifierDecorator : WeaponBuildDataSourceDecorator
    {
        public StrongModifierDecorator(IWeaponBuildDataSource inner) : base(inner)
        {
        }

        protected override WeaponBuildData Transform(WeaponBuildData data)
        {
            return data with
            {
                Name = $"{data.Name} (Strong)",
                Damage = data.Damage + 5
            };
        }
    }

    public sealed class BattleHardenedModifierDecorator : WeaponBuildDataSourceDecorator
    {
        public BattleHardenedModifierDecorator(IWeaponBuildDataSource inner) : base(inner)
        {
        }

        protected override WeaponBuildData Transform(WeaponBuildData data)
        {
            return data with
            {
                Name = $"{data.Name} (BattleHardened)",
                AggressionBonus = data.AggressionBonus + 3
            };
        }
    }
}
