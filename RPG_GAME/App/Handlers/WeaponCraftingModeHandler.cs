using RPG_GAME.Model;
using System.Collections.Generic;

namespace RPG_GAME.App
{
    public class WeaponCraftingModeHandler : IGameModeHandler
    {
        private const string CraftingHelpMessage = "Crafting: W/S to select, E to combine, ESC to leave.";

        public void Handle(InputCommand command, World world, GameState state)
        {
            var weapons = GetWeaponsFromInventory(world);

            switch (command)
            {
                case InputCommand.CraftingSelectFirst:
                    MoveWeaponSelection(state, weapons.Count, 1);
                    world.AddMessage($"Selected weapon {state.CraftingFirstSelection + 1}");
                    break;

                case InputCommand.CraftingSelectSecond:
                    MoveWeaponSelection(state, weapons.Count, -1);
                    world.AddMessage($"Selected weapon {state.CraftingFirstSelection + 1}");
                    break;

                case InputCommand.CraftingCombine:
                    HandleWeaponCombine(world, state, weapons);
                    break;

                case InputCommand.CraftingCancel:
                    state.CurrentMode = GameMode.Normal;
                    state.CraftingFirstSelection = -1;
                    world.AddMessage("Crafting station left.");
                    break;

                default:
                    world.AddMessage(CraftingHelpMessage);
                    break;
            }
        }

        private List<WeaponItem> GetWeaponsFromInventory(World world)
        {
            var weapons = new List<WeaponItem>();
            for (int i = 0; i < world.Player.Inventory.Count(); i++)
            {
                var item = world.Player.Inventory.GetItem(i);
                if (item is WeaponItem weapon)
                    weapons.Add(weapon);
            }
            return weapons;
        }

        private void MoveWeaponSelection(GameState state, int weaponCount, int direction)
        {
            if (weaponCount == 0)
                return;

            state.CraftingFirstSelection += direction;
            if (state.CraftingFirstSelection < 0)
                state.CraftingFirstSelection = weaponCount - 1;
            else if (state.CraftingFirstSelection >= weaponCount)
                state.CraftingFirstSelection = 0;
        }

        private void HandleWeaponCombine(World world, GameState state, List<WeaponItem> weapons)
        {
            if (weapons.Count < 2)
            {
                world.AddMessage("Need 2 weapons to combine.");
                return;
            }

            var weapon1 = weapons[state.CraftingFirstSelection];
            var weapon2 = weapons[(state.CraftingFirstSelection + 1) % weapons.Count];

            var combined = world.CombineWeapons(weapon1, weapon2);
            if (combined != null)
            {
                RemoveWeaponFromBackpack(world, weapon1);
                RemoveWeaponFromBackpack(world, weapon2);
                world.Player.Inventory.AddToBackpack(combined);
                world.AddMessage("Weapons combined and added to backpack!");
            }
        }

        private void RemoveWeaponFromBackpack(World world, Items weapon)
        {
            for (int i = 0; i < world.Player.Inventory.Count(); i++)
            {
                var item = world.Player.Inventory.GetItem(i);
                if (ReferenceEquals(item, weapon))
                {
                    world.Player.Inventory.RemoveFromBackpack(i);
                    return;
                }
            }
        }
    }
}
