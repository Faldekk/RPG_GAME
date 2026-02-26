using System;
using System.Collections.Generic;

namespace RPG_GAME.Model
{
    /// <summary>
    /// Manages player's equipped weapons (left and right hand only)
    /// </summary>
    public class PlayerInventory
    {
        private Items? _leftHand;
        private Items? _rightHand;

        public Items? LeftHand => _leftHand;
        public Items? RightHand => _rightHand;

        public bool HasTwoHandedWeapon =>
            (_leftHand?.Both_hands ?? false) || (_rightHand?.Both_hands ?? false);

        public IReadOnlyList<Items?> EquippedItems =>
            new List<Items?> { _leftHand, _rightHand };

        public PlayerInventory()
        {
            _leftHand = null;
            _rightHand = null;
        }

        /// <summary>
        /// Equips item to specified hand (0 = left, 1 = right)
        /// </summary>
        public bool EquipItem(Items item, int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return false;

            // If item is two-handed, clear both hands and equip in left
            if (item.Both_hands)
            {
                _leftHand = item;
                _rightHand = null;
                return true;
            }

            // Can't equip if there's already a two-handed weapon
            if (HasTwoHandedWeapon)
                return false;

            if (handIndex == 0)
                _leftHand = item;
            else
                _rightHand = item;

            return true;
        }

        /// <summary>
        /// Unequips weapon from specified hand
        /// </summary>
        public Items? UnequipItem(int handIndex)
        {
            if (handIndex < 0 || handIndex > 1)
                return null;

            Items? unequipped = handIndex == 0 ? _leftHand : _rightHand;

            // If it was two-handed, clear both hands
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

        /// <summary>
        /// Gets both equipped weapons
        /// </summary>
        public IEnumerable<Items> GetAllWeapons()
        {
            if (_leftHand != null) yield return _leftHand;
            if (_rightHand != null && !(_leftHand?.Both_hands ?? false))
                yield return _rightHand;
        }

        /// <summary>
        /// Clears all equipped items
        /// </summary>
        public void ClearAll()
        {
            _leftHand = null;
            _rightHand = null;
        }
    }
}