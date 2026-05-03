using System;
using RPG_GAME.Model;
using RPG_GAME.Model.Combat;
using RPG_GAME.Model.DungeonBuilding;

namespace RPG_GAME.Model.DungeonThemes
{
    public sealed class ArcaneTheme : IDungeonTheme
    {
        public string Name => "Arcane";
        public string IntroMessage => "The smell of old books fills the air.";

        public IDungeonStrategy CreateDungeonStrategy() => new ArcaneDungeonStrategy();
        public IItemPool CreateItemPool() => new ArcaneItemPool();
        public IEnemyFactory CreateEnemyFactory() => new ArcaneEnemyFactory();

        public Items CreateArtifact(Vec2 position)
        {
            return new WeaponItem(
                "Black Wand",
                "Artifact",
                value: 18,
                isTwoHanded: false,
                strengthBonus: 0,
                dexterityBonus: 1,
                aggressionBonus: 0,
                wisdomBonus: 8,
                luckBonus: 2,
                category: MagicalWeaponCategory.Instance,
                position: ToPosition(position));
        }

        private static Tuple<int, int> ToPosition(Vec2 position) => new(position.X, position.Y);
    }

    internal sealed class ArcaneDungeonStrategy : IDungeonStrategy
    {
        public BuildContext Build(Tile[,] tiles, int width, int height)
        {
            return new DungeonBuilder()
                .AddStep(new FilledDungeonStep())
                .AddStep(new AddChambersStep(7, 3, 5))
                .AddStep(new AddPathsStep())
                .Build(tiles, width, height);
        }
    }

    internal sealed class ArcaneItemPool : IItemPool
    {
        private static readonly (string Name, int Value, int Wisdom, int Dexterity)[] Books =
        {
            ("Spellbook", 6, 3, 0),
            ("Ancient Tome", 8, 4, 0),
            ("Tattered Grimoire", 5, 2, 1),
            ("Scroll of Insight", 7, 5, 0)
        };

        public Items CreateRandomItem(Vec2 position)
        {
            var template = Books[Random.Shared.Next(Books.Length)];
            return new WeaponItem(
                template.Name,
                "Book",
                template.Value,
                isTwoHanded: false,
                strengthBonus: 0,
                dexterityBonus: template.Dexterity,
                aggressionBonus: 0,
                wisdomBonus: template.Wisdom,
                luckBonus: 1,
                category: MagicalWeaponCategory.Instance,
                position: ToPosition(position));
        }

        private static Tuple<int, int> ToPosition(Vec2 position) => new(position.X, position.Y);
    }

    internal sealed class ArcaneEnemyFactory : IEnemyFactory
    {
        private static readonly Func<Vec2, Enemy>[] Templates =
        {
            pos => new Enemy("Apprentice Mage", 70, 8, 14, 4, pos, 'M', new MagicalAttackType()),
            pos => new Enemy("Runic Sage", 90, 10, 16, 5, pos, 'S', new MagicalAttackType()),
            pos => new Enemy("Archmage", 110, 12, 18, 6, pos, 'A', new MagicalAttackType())
        };

        public Enemy CreateRandomEnemy(Vec2 position)
        {
            return Templates[Random.Shared.Next(Templates.Length)](position);
        }
    }
}