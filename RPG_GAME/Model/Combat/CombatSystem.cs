using System;
using System.Collections.Generic;

namespace RPG_GAME.Model.Combat
{
    public interface IAttackType
    {
        string Name { get; }
        int ModifyHeavyDamage(int baseDamage);
        int ModifyLightDamage(int baseDamage);
        int ModifyMagicalDamage(int baseDamage);
        int ModifyOtherDamage(int baseDamage);
        int ResolveHeavyDefense(PlayerStats stats);
        int ResolveLightDefense(PlayerStats stats);
        int ResolveMagicalDefense(PlayerStats stats);
        int ResolveOtherDefense(PlayerStats stats);
    }

    public sealed class NormalAttackType : IAttackType
    {
        public string Name => "Normal";
        public int ModifyHeavyDamage(int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyLightDamage(int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyMagicalDamage(int baseDamage) => 1;
        public int ModifyOtherDamage(int baseDamage) => 0;
        public int ResolveHeavyDefense(PlayerStats stats) => stats.Strength + stats.Luck;
        public int ResolveLightDefense(PlayerStats stats) => stats.Dexterity + stats.Luck;
        public int ResolveMagicalDefense(PlayerStats stats) => stats.Dexterity + stats.Luck;
        public int ResolveOtherDefense(PlayerStats stats) => stats.Dexterity;
    }

    public sealed class StealthAttackType : IAttackType
    {
        public string Name => "Stealth";
        public int ModifyHeavyDamage(int baseDamage) => Math.Max(1, baseDamage / 2);
        public int ModifyLightDamage(int baseDamage) => Math.Max(1, baseDamage * 2);
        public int ModifyMagicalDamage(int baseDamage) => 1;
        public int ModifyOtherDamage(int baseDamage) => 0;
        public int ResolveHeavyDefense(PlayerStats stats) => stats.Strength;
        public int ResolveLightDefense(PlayerStats stats) => stats.Dexterity;
        public int ResolveMagicalDefense(PlayerStats stats) => 0;
        public int ResolveOtherDefense(PlayerStats stats) => 0;
    }

    public sealed class MagicalAttackType : IAttackType
    {
        public string Name => "Magical";
        public int ModifyHeavyDamage(int baseDamage) => 1;
        public int ModifyLightDamage(int baseDamage) => 1;
        public int ModifyMagicalDamage(int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyOtherDamage(int baseDamage) => 0;
        public int ResolveHeavyDefense(PlayerStats stats) => stats.Luck;
        public int ResolveLightDefense(PlayerStats stats) => stats.Luck;
        public int ResolveMagicalDefense(PlayerStats stats) => stats.Wisdom * 2;
        public int ResolveOtherDefense(PlayerStats stats) => stats.Luck;
    }

    public interface IWeaponCategory
    {
        string Name { get; }
        int CalculateDamage(Items item, PlayerStats stats, IAttackType attackType);
        int CalculateDefense(PlayerStats stats, IAttackType attackType);
    }

    public sealed class HeavyWeaponCategory : IWeaponCategory
    {
        public string Name => "Heavy";

        public int CalculateDamage(Items item, PlayerStats stats, IAttackType attackType)
        {
            int baseDamage = Math.Max(1, item.Value + stats.Strength + stats.Aggression);
            return attackType.ModifyHeavyDamage(baseDamage);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ResolveHeavyDefense(stats);
        }
    }

    public sealed class LightWeaponCategory : IWeaponCategory
    {
        public string Name => "Light";

        public int CalculateDamage(Items item, PlayerStats stats, IAttackType attackType)
        {
            int baseDamage = Math.Max(1, item.Value + stats.Dexterity + stats.Luck);
            return attackType.ModifyLightDamage(baseDamage);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ResolveLightDefense(stats);
        }
    }

    public sealed class MagicalWeaponCategory : IWeaponCategory
    {
        public string Name => "Magical";

        public int CalculateDamage(Items item, PlayerStats stats, IAttackType attackType)
        {
            int baseDamage = Math.Max(1, item.Value + stats.Wisdom);
            return attackType.ModifyMagicalDamage(baseDamage);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ResolveMagicalDefense(stats);
        }
    }

    public sealed class OtherWeaponCategory : IWeaponCategory
    {
        public string Name => "Other";

        public int CalculateDamage(Items item, PlayerStats stats, IAttackType attackType)
        {
            return 0;
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ResolveOtherDefense(stats);
        }
    }

    public interface IWeaponCategoryRule
    {
        bool Matches(Items? item);
        IWeaponCategory Category { get; }
    }

    public sealed class NullWeaponRule : IWeaponCategoryRule
    {
        public IWeaponCategory Category { get; } = new OtherWeaponCategory();
        public bool Matches(Items? item) => item == null;
    }

    public sealed class MagicalWeaponRule : IWeaponCategoryRule
    {
        public IWeaponCategory Category { get; } = new MagicalWeaponCategory();

        public bool Matches(Items? item)
        {
            return item != null && item.Type.Equals("Magic", StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class LightWeaponRule : IWeaponCategoryRule
    {
        public IWeaponCategory Category { get; } = new LightWeaponCategory();

        public bool Matches(Items? item)
        {
            if (item == null)
                return false;

            return item.Name.Contains("Dagger", StringComparison.OrdinalIgnoreCase)
                   || item.Name.Contains("Knife", StringComparison.OrdinalIgnoreCase)
                   || item.Name.Contains("Rapier", StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class HeavyWeaponRule : IWeaponCategoryRule
    {
        public IWeaponCategory Category { get; } = new HeavyWeaponCategory();

        public bool Matches(Items? item)
        {
            return item != null && item.Type.Equals("Melee", StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class FallbackWeaponRule : IWeaponCategoryRule
    {
        public IWeaponCategory Category { get; } = new OtherWeaponCategory();
        public bool Matches(Items? item) => true;
    }

    public sealed class WeaponCategoryResolver
    {
        private readonly IReadOnlyList<IWeaponCategoryRule> _rules;

        public WeaponCategoryResolver()
        {
            _rules = new List<IWeaponCategoryRule>
            {
                new NullWeaponRule(),
                new MagicalWeaponRule(),
                new LightWeaponRule(),
                new HeavyWeaponRule(),
                new FallbackWeaponRule()
            };
        }

        public IWeaponCategory Resolve(Items? item)
        {
            foreach (var rule in _rules)
            {
                if (rule.Matches(item))
                    return rule.Category;
            }

            return new OtherWeaponCategory();
        }
    }

    public sealed class CombatRoundResult
    {
        public bool EnemyDefeated { get; }
        public bool PlayerDefeated { get; }
        public int PlayerDamageDealt { get; }
        public int EnemyDamageDealt { get; }
        public int PlayerDefense { get; }
        public string Summary { get; }

        public CombatRoundResult(bool enemyDefeated, bool playerDefeated, int playerDamageDealt, int enemyDamageDealt, int playerDefense, string summary)
        {
            EnemyDefeated = enemyDefeated;
            PlayerDefeated = playerDefeated;
            PlayerDamageDealt = playerDamageDealt;
            EnemyDamageDealt = enemyDamageDealt;
            PlayerDefense = playerDefense;
            Summary = summary;
        }
    }

    public sealed class CombatEngine
    {
        private readonly WeaponCategoryResolver _weaponCategoryResolver;

        public CombatEngine(WeaponCategoryResolver weaponCategoryResolver)
        {
            _weaponCategoryResolver = weaponCategoryResolver;
        }

        public CombatRoundResult ExecuteRound(Player player, Enemy enemy, IAttackType playerAttackType, Items? playerWeapon)
        {
            var weaponCategory = _weaponCategoryResolver.Resolve(playerWeapon);
            int playerRawDamage = playerWeapon == null
                ? 0
                : weaponCategory.CalculateDamage(playerWeapon, player.Stats, playerAttackType);

            int enemyTaken = Math.Max(0, playerRawDamage - enemy.Armor);
            enemy.TakeDamage(enemyTaken);

            if (!enemy.IsAlive)
            {
                return new CombatRoundResult(
                    enemyDefeated: true,
                    playerDefeated: false,
                    playerDamageDealt: enemyTaken,
                    enemyDamageDealt: 0,
                    playerDefense: 0,
                    summary: $"{playerAttackType.Name} hit for {enemyTaken}. {enemy.Name} defeated.");
            }

            var enemyAttackType = enemy.AttackType;
            int playerDefense = weaponCategory.CalculateDefense(player.Stats, enemyAttackType) + player.Stats.Armor;
            int enemyRawDamage = enemy.RollAttackDamage();
            int playerTaken = Math.Max(0, enemyRawDamage - playerDefense);
            player.Stats.TakeDamage(playerTaken);

            bool playerDefeated = !player.IsAlive;
            string summary = $"{playerAttackType.Name} dealt {enemyTaken}. {enemy.Name} dealt {playerTaken} (def {playerDefense}).";

            return new CombatRoundResult(
                enemyDefeated: false,
                playerDefeated: playerDefeated,
                playerDamageDealt: enemyTaken,
                enemyDamageDealt: playerTaken,
                playerDefense: playerDefense,
                summary: summary);
        }
    }
}
