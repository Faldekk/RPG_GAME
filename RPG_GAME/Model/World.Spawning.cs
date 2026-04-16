using System;
using System.Collections.Generic;
using RPG_GAME.Model.DungeonBuilding;
using RPG_GAME.Model.DungeonThemes;

namespace RPG_GAME.Model
{
    public partial class World
    {
        private void SpawnPlayer(BuildContext context)
        {
            if (context.CentralRoom.HasValue)
            {
                var centralRoom = context.CentralRoom.Value;
                Player.MoveTo(new Vec2(centralRoom.CenterX, centralRoom.CenterY));
                return;
            }

            if (context.Rooms.Count > 0)
            {
                var startRoom = context.Rooms[0];
                Player.MoveTo(new Vec2(startRoom.CenterX, startRoom.CenterY));
                return;
            }

            _tiles[1, 1].IsWall = false;
            Player.MoveTo(new Vec2(1, 1));
        }

        private List<(int y, int x)> GetAvailableFloorTiles()
        {
            var availableTiles = new List<(int y, int x)>();

            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (!_tiles[y, x].IsWall && _tiles[y, x].Item == null && _tiles[y, x].Enemy == null)
                        availableTiles.Add((y, x));
                }
            }

            return availableTiles;
        }

        private void SpawnThemeContent(BuildContext context)
        {
            var availableTiles = GetAvailableFloorTiles();
            SpawnArtifact(availableTiles);
            SpawnThemeItems(availableTiles, 6);
            SpawnThemeEnemies(availableTiles, 7);
        }

        private void SpawnArtifact(List<(int y, int x)> availableTiles)
        {
            if (_theme == null || availableTiles.Count == 0)
                return;

            int pickIndex = Random.Shared.Next(availableTiles.Count);
            var (y, x) = availableTiles[pickIndex];
            availableTiles.RemoveAt(pickIndex);
            _tiles[y, x].Item = _theme.CreateArtifact(new Vec2(x, y));
        }

        private void SpawnThemeItems(List<(int y, int x)> availableTiles, int count)
        {
            if (_itemPool == null)
                return;

            int toSpawn = Math.Min(count, availableTiles.Count);
            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);
                _tiles[y, x].Item = _itemPool.CreateRandomItem(new Vec2(x, y));
            }
        }

        private void SpawnThemeEnemies(List<(int y, int x)> availableTiles, int count)
        {
            if (_enemyFactory == null)
                return;

            int toSpawn = Math.Min(count, availableTiles.Count);
            for (int i = 0; i < toSpawn; i++)
            {
                int pickIndex = Random.Shared.Next(availableTiles.Count);
                var (y, x) = availableTiles[pickIndex];
                availableTiles.RemoveAt(pickIndex);
                _tiles[y, x].Enemy = _enemyFactory.CreateRandomEnemy(new Vec2(x, y));
            }
        }

        private void SpawnCurrencyItems(int coinCount, int goldCount)
        {
            var availableTiles = GetAvailableFloorTiles();

            void SpawnCurrency(int count, Func<Vec2, Items> factory)
            {
                int toSpawn = Math.Min(count, availableTiles.Count);

                for (int i = 0; i < toSpawn; i++)
                {
                    int pickIndex = Random.Shared.Next(availableTiles.Count);
                    var (y, x) = availableTiles[pickIndex];
                    availableTiles.RemoveAt(pickIndex);
                    _tiles[y, x].Item = factory(new Vec2(x, y));
                }
            }

            SpawnCurrency(coinCount, pos => ItemGenerator.CreateCoins(pos, Random.Shared.Next(5, 16)));
            SpawnCurrency(goldCount, pos => ItemGenerator.CreateGold(pos, Random.Shared.Next(1, 5)));
        }
    }
}