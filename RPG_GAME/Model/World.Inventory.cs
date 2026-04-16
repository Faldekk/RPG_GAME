using System;

namespace RPG_GAME.Model
{
    public partial class World
    {
        public bool TryMovePlayer(int dx, int dy)
        {
            var next = Player.Pos.Add(dx, dy);

            if (next.X < 0 || next.X >= Width || next.Y < 0 || next.Y >= Height)
            {
                AddMessage("Walking into a wall.");
                return false;
            }

            var targetTile = _tiles[next.Y, next.X];
            if (targetTile.IsWall)
            {
                AddMessage("Walking into a wall.");
                return false;
            }

            if (targetTile.Enemy != null)
            {
                StartCombat(targetTile.Enemy);
                return true;
            }

            Player.MoveTo(next);
            return true;
        }

        public bool TryPickUpItem()
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            var item = tile.Item;

            if (item == null)
            {
                AddMessage("Cannot pick up: no item here.");
                return false;
            }

            if (item.TryCollect(Player, out var collectMessage))
            {
                tile.Item = null;
                AddMessage(collectMessage);
                AddMessage($"Picking up item: {item.Name}.");
                return true;
            }

            if (!item.CanEquip)
                return TryStoreFromTile(tile, item, $"Picked up {item.Name}.");

            if (TryEquipWeapon(item))
            {
                ApplyWeaponBonuses(item);
                tile.Item = null;
                AddMessage($"Equipped {item.Name}.");
                return true;
            }

            return TryStoreFromTile(tile, item, $"Stored {item.Name} in backpack.");
        }

        private bool TryStoreFromTile(Tile tile, Items item, string successMessage)
        {
            if (!Player.Inventory.AddToBackpack(item))
            {
                AddMessage("Backpack full.");
                return false;
            }

            tile.Item = null;
            AddMessage(successMessage);
            return true;
        }

        private bool TryEquipWeapon(Items item)
        {
            if (!item.CanEquip)
                return false;

            if (item.IsTwoHanded)
            {
                if (Player.Inventory.LeftHand != null || Player.Inventory.RightHand != null)
                    return false;

                return Player.Inventory.EquipItem(item, 0);
            }

            if (Player.Inventory.HasTwoHandedWeapon)
                return false;

            if (Player.Inventory.LeftHand == null)
                return Player.Inventory.EquipItem(item, 0);

            if (Player.Inventory.RightHand == null)
                return Player.Inventory.EquipItem(item, 1);

            return false;
        }

        public bool TryBackpackAction()
        {
            int count = Player.Inventory.Count();
            if (count == 0)
            {
                AddMessage("Backpack is empty.");
                return false;
            }

            return DropFromBackpack(count - 1);
        }

        public bool EquipFromBackpack(int index)
        {
            var selected = Player.Inventory.GetItem(index);
            if (selected == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            if (!selected.CanEquip)
            {
                AddMessage("Selected item cannot be equipped.");
                return false;
            }

            return selected.IsTwoHanded
                ? EquipTwoHandedFromBackpack(index)
                : EquipOneHandedFromBackpack(index);
        }

        private bool EquipTwoHandedFromBackpack(int index)
        {
            var left = Player.Inventory.LeftHand;
            var right = Player.Inventory.RightHand;

            int requiredSlots = (left != null ? 1 : 0) + (right != null ? 1 : 0);
            if (Player.Inventory.Count() - 1 + requiredSlots > Player.Inventory.MaxBackpackSize)
            {
                AddMessage("Backpack full.");
                return false;
            }

            var backpackItem = Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            var unequippedLeft = Player.Inventory.UnequipItem(0);
            var unequippedRight = Player.Inventory.UnequipItem(1);

            if (unequippedLeft != null)
                RemoveWeaponBonuses(unequippedLeft);
            if (unequippedRight != null)
                RemoveWeaponBonuses(unequippedRight);

            if (unequippedLeft != null)
                Player.Inventory.AddToBackpack(unequippedLeft);
            if (unequippedRight != null)
                Player.Inventory.AddToBackpack(unequippedRight);

            Player.Inventory.EquipItem(backpackItem, 0);
            ApplyWeaponBonuses(backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private bool EquipOneHandedFromBackpack(int index)
        {
            var backpackItem = Player.Inventory.TakeFromBackpack(index);
            if (backpackItem == null)
            {
                AddMessage("Cannot equip selected item.");
                return false;
            }

            int handIndex = GetOneHandEquipSlot();
            var displaced = Player.Inventory.UnequipItem(handIndex);

            if (displaced != null)
            {
                RemoveWeaponBonuses(displaced);

                if (!Player.Inventory.AddToBackpack(displaced))
                {
                    ApplyWeaponBonuses(displaced);
                    Player.Inventory.EquipItem(displaced, handIndex);
                    Player.Inventory.AddToBackpack(backpackItem);
                    AddMessage("Backpack full.");
                    return false;
                }
            }

            if (!Player.Inventory.EquipItem(backpackItem, handIndex))
            {
                var rollback = Player.Inventory.TakeFromBackpack(Player.Inventory.Count() - 1);
                if (rollback != null)
                {
                    ApplyWeaponBonuses(rollback);
                    Player.Inventory.EquipItem(rollback, handIndex);
                }

                Player.Inventory.AddToBackpack(backpackItem);
                AddMessage("Cannot equip selected item.");
                return false;
            }

            ApplyWeaponBonuses(backpackItem);
            AddMessage($"Equipped {backpackItem.Name}.");
            return true;
        }

        private int GetOneHandEquipSlot()
        {
            if (Player.Inventory.HasTwoHandedWeapon)
                return 0;

            if (Player.Inventory.LeftHand == null)
                return 0;

            if (Player.Inventory.RightHand == null)
                return 1;

            return 0;
        }

        public bool DropFromBackpack(int index)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.Item != null)
            {
                AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            if (tile.Enemy != null)
            {
                AddMessage("Cannot drop during enemy occupation.");
                return false;
            }

            var item = Player.Inventory.TakeFromBackpack(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        public bool UseFromBackpack(int index)
        {
            var item = Player.Inventory.GetItem(index);
            if (item == null)
            {
                AddMessage("No item selected.");
                return false;
            }

            if (!item.TryUse(Player, out var useMessage))
            {
                AddMessage(useMessage);
                return false;
            }

            Player.Inventory.RemoveFromBackpack(index);
            AddMessage(useMessage);
            return true;
        }

        public bool TryDropItem(int handIndex)
        {
            var tile = GetTile(Player.Pos.Y, Player.Pos.X);
            if (tile.Item != null)
            {
                AddMessage("Cannot drop: tile already has an item.");
                return false;
            }

            if (tile.Enemy != null)
            {
                AddMessage("Cannot drop during enemy occupation.");
                return false;
            }

            var item = Player.Inventory.UnequipItem(handIndex);
            if (item == null)
            {
                AddMessage("Cannot drop: selected hand is empty.");
                return false;
            }

            RemoveWeaponBonuses(item);

            item.Position = new Tuple<int, int>(Player.Pos.X, Player.Pos.Y);
            tile.Item = item;
            AddMessage($"Dropped {item.Name}.");
            return true;
        }

        private void ApplyWeaponBonuses(Items item) => item.ApplyEquipBonuses(Player.Stats);

        private void RemoveWeaponBonuses(Items item) => item.RemoveEquipBonuses(Player.Stats);

        public bool CraftArmorFromJunk()
        {
            var junkIndexes = new List<int>();
            for (int i = 0; i < Player.Inventory.Count(); i++)
            {
                var item = Player.Inventory.GetItem(i);
                if (item != null && item.Type.Equals("Junk", StringComparison.OrdinalIgnoreCase))
                    junkIndexes.Add(i);
            }

            if (junkIndexes.Count < 2)
            {
                AddMessage("Need 2 junk items to craft armor.");
                return false;
            }

            junkIndexes.Sort((a, b) => b.CompareTo(a));
            Player.Inventory.RemoveFromBackpack(junkIndexes[0]);
            Player.Inventory.RemoveFromBackpack(junkIndexes[1]);
            Player.Stats.Armor += 2;
            AddMessage($"Crafted armor from junk. Armor is now {Player.Stats.Armor}.");
            return true;
        }
    }
}