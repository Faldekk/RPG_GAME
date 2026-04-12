using System;

namespace RPG_GAME.Model.Combat
{
    public interface IWeaponCategory
    {
        int CalculateDamage(PlayerStats stats, IAttackType attackType, int baseDamage);
        int CalculateDefense(PlayerStats stats, IAttackType attackType);
    }

    public interface IAttackType
    {
        string Name { get; }

        int ModifyDamage(IWeaponCategory category, int baseDamage);
        int ModifyDefense(IWeaponCategory category, PlayerStats stats);

        int ModifyDamage(HeavyWeaponCategory category, int baseDamage);
        int ModifyDamage(LightWeaponCategory category, int baseDamage);
        int ModifyDamage(MagicalWeaponCategory category, int baseDamage);
        int ModifyDamage(NoWeaponCategory category, int baseDamage);

        int ModifyDefense(HeavyWeaponCategory category, PlayerStats stats);
        int ModifyDefense(LightWeaponCategory category, PlayerStats stats);
        int ModifyDefense(MagicalWeaponCategory category, PlayerStats stats);
        int ModifyDefense(NoWeaponCategory category, PlayerStats stats);
    }

    public sealed class HeavyWeaponCategory : IWeaponCategory
    {
        public static HeavyWeaponCategory Instance { get; } = new();

        private HeavyWeaponCategory()
        {
        }

        public int CalculateDamage(PlayerStats stats, IAttackType attackType, int baseDamage)
        {
            int scaled = Math.Max(1, baseDamage + stats.Strength + stats.Aggression);
            return attackType.ModifyDamage(this, scaled);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ModifyDefense(this, stats);
        }
    }

    public sealed class LightWeaponCategory : IWeaponCategory
    {
        public static LightWeaponCategory Instance { get; } = new();

        private LightWeaponCategory()
        {
        }

        public int CalculateDamage(PlayerStats stats, IAttackType attackType, int baseDamage)
        {
            int scaled = Math.Max(1, baseDamage + stats.Dexterity + stats.Luck);
            return attackType.ModifyDamage(this, scaled);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ModifyDefense(this, stats);
        }
    }

    public sealed class MagicalWeaponCategory : IWeaponCategory
    {
        public static MagicalWeaponCategory Instance { get; } = new();

        private MagicalWeaponCategory()
        {
        }

        public int CalculateDamage(PlayerStats stats, IAttackType attackType, int baseDamage)
        {
            int scaled = Math.Max(1, baseDamage + stats.Wisdom);
            return attackType.ModifyDamage(this, scaled);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ModifyDefense(this, stats);
        }
    }

    public sealed class NoWeaponCategory : IWeaponCategory
    {
        public static NoWeaponCategory Instance { get; } = new();

        private NoWeaponCategory()
        {
        }

        public int CalculateDamage(PlayerStats stats, IAttackType attackType, int baseDamage)
        {
            return attackType.ModifyDamage(this, 0);
        }

        public int CalculateDefense(PlayerStats stats, IAttackType attackType)
        {
            return attackType.ModifyDefense(this, stats);
        }
    }

    public sealed class NormalAttackType : IAttackType
    {
        public string Name => "Normal";

        public int ModifyDamage(IWeaponCategory category, int baseDamage)
        {
            return baseDamage;
        }

        public int ModifyDefense(IWeaponCategory category, PlayerStats stats)
        {
            return stats.Dexterity;
        }

        public int ModifyDamage(HeavyWeaponCategory category, int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyDamage(LightWeaponCategory category, int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyDamage(MagicalWeaponCategory category, int baseDamage) => 1;
        public int ModifyDamage(NoWeaponCategory category, int baseDamage) => 0;

        public int ModifyDefense(HeavyWeaponCategory category, PlayerStats stats) => stats.Strength + stats.Luck;
        public int ModifyDefense(LightWeaponCategory category, PlayerStats stats) => stats.Dexterity + stats.Luck;
        public int ModifyDefense(MagicalWeaponCategory category, PlayerStats stats) => stats.Dexterity + stats.Luck;
        public int ModifyDefense(NoWeaponCategory category, PlayerStats stats) => stats.Dexterity;
    }

    public sealed class StealthAttackType : IAttackType
    {
        public string Name => "Stealth";

        public int ModifyDamage(IWeaponCategory category, int baseDamage)
        {
            return baseDamage;
        }

        public int ModifyDefense(IWeaponCategory category, PlayerStats stats)
        {
            return 0;
        }

        public int ModifyDamage(HeavyWeaponCategory category, int baseDamage) => Math.Max(1, baseDamage / 2);
        public int ModifyDamage(LightWeaponCategory category, int baseDamage) => Math.Max(1, baseDamage * 2);
        public int ModifyDamage(MagicalWeaponCategory category, int baseDamage) => 1;
        public int ModifyDamage(NoWeaponCategory category, int baseDamage) => 0;

        public int ModifyDefense(HeavyWeaponCategory category, PlayerStats stats) => stats.Strength;
        public int ModifyDefense(LightWeaponCategory category, PlayerStats stats) => stats.Dexterity;
        public int ModifyDefense(MagicalWeaponCategory category, PlayerStats stats) => 0;
        public int ModifyDefense(NoWeaponCategory category, PlayerStats stats) => 0;
    }

    public sealed class MagicalAttackType : IAttackType
    {
        public string Name => "Magical";

        public int ModifyDamage(IWeaponCategory category, int baseDamage)
        {
            return 1;
        }

        public int ModifyDefense(IWeaponCategory category, PlayerStats stats)
        {
            return stats.Luck;
        }

        public int ModifyDamage(HeavyWeaponCategory category, int baseDamage) => 1;
        public int ModifyDamage(LightWeaponCategory category, int baseDamage) => 1;
        public int ModifyDamage(MagicalWeaponCategory category, int baseDamage) => Math.Max(1, baseDamage);
        public int ModifyDamage(NoWeaponCategory category, int baseDamage) => 0;

        public int ModifyDefense(HeavyWeaponCategory category, PlayerStats stats) => stats.Luck;
        public int ModifyDefense(LightWeaponCategory category, PlayerStats stats) => stats.Luck;
        public int ModifyDefense(MagicalWeaponCategory category, PlayerStats stats) => stats.Wisdom * 2;
        public int ModifyDefense(NoWeaponCategory category, PlayerStats stats) => stats.Luck;
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
        public CombatRoundResult ExecuteRound(Player player, Enemy enemy, IAttackType playerAttackType, Items? playerWeapon)
        {
            IWeaponCategory category = ResolveCategory(playerWeapon);
            int baseDamage = playerWeapon?.Value ?? 0;

            int playerRawDamage = category.CalculateDamage(player.Stats, playerAttackType, baseDamage);
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

            int playerDefense = category.CalculateDefense(player.Stats, playerAttackType) + player.Stats.Armor;
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

        private static IWeaponCategory ResolveCategory(Items? weapon)
        {
            if (weapon == null || !weapon.CanEquip)
                return NoWeaponCategory.Instance;

            return weapon.GetWeaponCategory();
        }
    }
}
