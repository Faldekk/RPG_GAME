using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public partial class World
    {
        public bool TryCombatRound(string attackKey)
        {
            if (ActiveEnemy == null)
            {
                AddMessage("No active enemy.");
                return false;
            }

            if (!_attackTypes.TryGetValue(attackKey, out var attackType))
            {
                AddMessage("Unknown attack style.");
                return false;
            }

            var weapon = ResolveActiveWeapon();
            var enemyBeforeRound = ActiveEnemy;
            var result = _combatEngine.ExecuteRound(Player, enemyBeforeRound, attackType, weapon);
            AddMessage(result.Summary);
            AddMessage($"Player attack dealt {result.PlayerDamageDealt} damage.");
            AddMessage($"Enemy attack dealt {result.EnemyDamageDealt} damage.");

            if (result.EnemyDefeated)
            {
                var defeatedPos = enemyBeforeRound.Position;
                RemoveEnemyFromMap(enemyBeforeRound);
                SpawnVictoryLoot(defeatedPos);
                Player.Heal(50);
                ActiveEnemy = null;
                AddMessage("Enemy removed from map.");
                AddMessage($"Enemy defeated: {enemyBeforeRound.Name}.");
            }

            if (result.PlayerDefeated)
                AddMessage("You died. Game over.");

            return true;
        }

        private Items? ResolveActiveWeapon()
        {
            if (Player.Inventory.LeftHand != null)
                return Player.Inventory.LeftHand;

            return Player.Inventory.RightHand;
        }

        private void StartCombat(Enemy enemy)
        {
            ActiveEnemy = enemy;
            AddMessage($"Encounter: {enemy.Name} | HP: {enemy.Health} | ATK: {enemy.AttackMin}-{enemy.AttackMax} | ARM: {enemy.Armor}");
        }

        private void RemoveEnemyFromMap(Enemy enemy)
        {
            var pos = enemy.Position;
            if (pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height)
                _tiles[pos.Y, pos.X].Enemy = null;
        }

        private void SpawnVictoryLoot(Vec2 center)
        {
            var candidates = new List<Vec2>();

            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    int x = center.X + dx;
                    int y = center.Y + dy;

                    if (x < 1 || x >= Width - 1 || y < 1 || y >= Height - 1)
                        continue;

                    if (_tiles[y, x].IsWall || _tiles[y, x].Enemy != null || _tiles[y, x].Item != null)
                        continue;

                    candidates.Add(new Vec2(x, y));
                }
            }

            if (candidates.Count == 0)
                return;

            if (_itemPool != null)
            {
                PlaceLoot(candidates, pos => _itemPool.CreateRandomItem(pos));
                return;
            }

            PlaceLoot(candidates, pos => ItemGenerator.CreateCoins(pos, Random.Shared.Next(6, 16)));
            PlaceLoot(candidates, pos => ItemGenerator.CreateGold(pos, Random.Shared.Next(1, 5)));
            PlaceLoot(candidates, pos => WeaponGenerator.GenerateRandomWeapon(pos));
        }

        private void PlaceLoot(List<Vec2> candidates, Func<Vec2, Items> factory)
        {
            if (candidates.Count == 0)
                return;

            int index = Random.Shared.Next(candidates.Count);
            var pos = candidates[index];
            candidates.RemoveAt(index);

            _tiles[pos.Y, pos.X].Item = factory(pos);
        }
    }
}