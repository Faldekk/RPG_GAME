using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    public class PlayerInventory
    {
        private Items? _leftHand;
        private Items? _rightHand;

        public Items? LeftHand => _leftHand;
        public Items? RightHand => _rightHand;

        public bool HasTwoHandedWeapon =>
            (_leftHand?.Both_hands ?? false) || (_rightHand?.Both_hands ?? false);

        public PlayerInventory() { }

        public bool EquipItem(Items item, int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return false;

            if (item.Both_hands)
            {
                _leftHand = item;
                _rightHand = null;
                return true;
            }

            if (HasTwoHandedWeapon)
                return false;

            if (handIndex == 0)
                _leftHand = item;
            else
                _rightHand = item;

            return true;
        }

        public Items? UnequipItem(int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return null;

            Items? unequipped = handIndex == 0 ? _leftHand : _rightHand;

            if (unequipped?.Both_hands ?? false)
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

        public IEnumerable<Items> GetAllWeapons()
        {
            if (_leftHand != null) yield return _leftHand;
            if (_rightHand != null && !(_leftHand?.Both_hands ?? false))
                yield return _rightHand;
        }

        public void ClearAll()
        {
            _leftHand = null;
            _rightHand = null;
        }
    }
}