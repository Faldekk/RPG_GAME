using System;
using RPG_GAME.Model;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model.DungeonThemes
{
    public sealed class TreasuryTheme : IDungeonTheme
    {
        public string Name => "Treasury";
        public string IntroMessage => "You feel an itch in your wallet";

        public IDungeonStrategy CreateDungeonStrategy() => new TreasuryDungeonStrategy();
        public IItemPool CreateItemPool() => new TreasuryItemPool();
        public IEnemyFactory CreateEnemyFactory() => new TreasuryEnemyFactory();

        public Items CreateArtifact(Vec2 position)
        {
            return new WeaponItem(
                "Lucky Coin Pouch",
                "Artifact",
                value: 16,
                isTwoHanded: false,
                strengthBonus: 0,
                dexterityBonus: 1,
                aggressionBonus: 0,
                wisdomBonus: 1,
                luckBonus: 8,
                category: LightWeaponCategory.Instance,
                position: ToPosition(position));
        }

        private static Tuple<int, int> ToPosition(Vec2 position) => new(position.X, position.Y);
    }

    internal sealed class TreasuryDungeonStrategy : IDungeonStrategy
    {
        public BuildContext Build(Tile[,] tiles, int width, int height)
        {
            return new DungeonBuilder()
                .AddStep(new FilledDungeonStep())
                .AddStep(new AddCentralRoomStep(12, 8))
                .AddStep(new AddChambersStep(4, 4, 6))
                .AddStep(new AddPathsStep())
                .Build(tiles, width, height);
        }
    }

    internal sealed class TreasuryItemPool : IItemPool
    {
        public Items CreateRandomItem(Vec2 position)
        {
            return Random.Shared.Next(2) == 0
                ? ItemGenerator.CreateCoins(position, Random.Shared.Next(10, 31))
                : ItemGenerator.CreateGold(position, Random.Shared.Next(1, 6));
        }
    }

    internal sealed class TreasuryEnemyFactory : IEnemyFactory
    {
        private static readonly Func<Vec2, Enemy>[] Templates =
        {
            pos => new Enemy("Animated Safe", 95, 10, 16, 6, pos, 'S', new NormalAttackType()),
            pos => new Enemy("Rogue Briefcase", 85, 9, 15, 5, pos, 'B', new StealthAttackType()),
            pos => new Enemy("Vault Guardian", 125, 13, 20, 9, pos, 'V', new NormalAttackType())
        };

        public Enemy CreateRandomEnemy(Vec2 position)
        {
            return Templates[Random.Shared.Next(Templates.Length)](position);
        }
    }
}