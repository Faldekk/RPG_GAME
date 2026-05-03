using System;
using RPG_GAME.Model;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model.DungeonThemes
{
    public sealed class IndustrialTheme : IDungeonTheme
    {
        public string Name => "Industrial";
        public string IntroMessage => "The clang of metal echoes off the walls.";

        public IDungeonStrategy CreateDungeonStrategy() => new IndustrialDungeonStrategy();
        public IItemPool CreateItemPool() => new IndustrialItemPool();
        public IEnemyFactory CreateEnemyFactory() => new IndustrialEnemyFactory();

        public Items CreateArtifact(Vec2 position)
        {
            return new WeaponItem(
                "Blaster",
                "Artifact",
                value: 24,
                isTwoHanded: true,
                strengthBonus: 4,
                dexterityBonus: 1,
                aggressionBonus: 3,
                wisdomBonus: 0,
                luckBonus: 0,
                category: HeavyWeaponCategory.Instance,
                position: ToPosition(position));
        }

        private static Tuple<int, int> ToPosition(Vec2 position) => new(position.X, position.Y);
    }

    internal sealed class IndustrialDungeonStrategy : IDungeonStrategy
    {
        public BuildContext Build(Tile[,] tiles, int width, int height)
        {
            return new DungeonBuilder()
                .AddStep(new FilledDungeonStep())
                .AddStep(new AddChambersStep(12, 3, 6))
                .AddStep(new AddPathsStep())
                .Build(tiles, width, height);
        }
    }

    internal sealed class IndustrialItemPool : IItemPool
    {
        private static readonly (string Name, int Value)[] Parts =
        {
            ("Metal Scrap", 2),
            ("Bent Gear", 4),
            ("Steel Plate", 5),
            ("Circuit Board", 7),
            ("Hydraulic Bolt", 6)
        };

        public Items CreateRandomItem(Vec2 position)
        {
            var template = Parts[Random.Shared.Next(Parts.Length)];
            return new JunkItem(template.Name, template.Value, ToPosition(position));
        }

        private static Tuple<int, int> ToPosition(Vec2 position) => new(position.X, position.Y);
    }

    internal sealed class IndustrialEnemyFactory : IEnemyFactory
    {
        private static readonly Func<Vec2, Enemy>[] Templates =
        {
            pos => new Enemy("Maintenance Drone", 80, 9, 15, 5, pos, 'D', new NormalAttackType()),
            pos => new Enemy("Steel Robot", 100, 11, 17, 7, pos, 'R', new NormalAttackType()),
            pos => new Enemy("Automaton Guard", 120, 13, 19, 8, pos, 'G', new NormalAttackType())
        };

        public Enemy CreateRandomEnemy(Vec2 position)
        {
            return Templates[Random.Shared.Next(Templates.Length)](position);
        }
    }
}