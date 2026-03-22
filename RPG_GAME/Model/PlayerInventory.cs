using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public class PlayerInventory
    {
        private const int BackpackCapacity = 10;

        private readonly List<Items> _backpack = new();
        private Items? _leftHand;
        private Items? _rightHand;

        public Items? LeftHand => _leftHand;
        public Items? RightHand => _rightHand;
        public IReadOnlyList<Items> Backpack => _backpack;
        public int MaxBackpackSize => BackpackCapacity;

        public bool HasTwoHandedWeapon =>
            (_leftHand?.IsTwoHanded ?? false) || (_rightHand?.IsTwoHanded ?? false);

        public PlayerInventory() { }

        public bool EquipItem(Items item, int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return false;

            if (item.IsTwoHanded)
            {
                if (_leftHand != null || _rightHand != null)
                    return false;

                _leftHand = item;
                _rightHand = null;
                return true;
            }

            if (HasTwoHandedWeapon)
                return false;

            if (handIndex == 0)
            {
                if (_leftHand != null)
                    return false;

                _leftHand = item;
            }
            else
            {
                if (_rightHand != null)
                    return false;

                _rightHand = item;
            }

            return true;
        }

        public Items? UnequipItem(int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return null;

            Items? unequipped = handIndex == 0 ? _leftHand : _rightHand;

            if (unequipped?.IsTwoHanded ?? false)
            {
                _leftHand = null;
                _rightHand = null;
            }
            else
            {
                if (handIndex == 0)
                    _leftHand = null;
                else
                    _rightHand = null;
            }

            return unequipped;
        }

        public bool AddToBackpack(Items item)
        {
            if (_backpack.Count >= BackpackCapacity)
                return false;

            _backpack.Add(item);
            return true;
        }

        public bool RemoveFromBackpack(int index)
        {
            if (index < 0 || index >= _backpack.Count)
                return false;

            _backpack.RemoveAt(index);
            return true;
        }

        public Items? GetItem(int index)
        {
            if (index < 0 || index >= _backpack.Count)
                return null;

            return _backpack[index];
        }

        public int Count()
        {
            return _backpack.Count;
        }

        public Items? TakeFromBackpack(int index)
        {
            var item = GetItem(index);
            if (item == null)
                return null;

            _backpack.RemoveAt(index);
            return item;
        }

        public IEnumerable<Items> GetAllWeapons()
        {
            if (_leftHand != null) yield return _leftHand;
            if (_rightHand != null && !(_leftHand?.IsTwoHanded ?? false))
                yield return _rightHand;
        }

        public void ClearAll()
        {
            _leftHand = null;
            _rightHand = null;
            _backpack.Clear();
        }
    }
}