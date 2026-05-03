using System;
using System.Collections.Generic;
using RPG_GAME.UI;

namespace RPG_GAME.App
{
    public enum InputCommand
    {
        None,
        Unknown,
        Up,
        Down,
        Left,
        Right,
        Pickup,
        Drop,
        BackpackAction,
        SwapWeapons,
        DropLeftHand,
        DropRightHand,
        OpenInventory,
        OpenJournal,
        InventoryUp,
        InventoryDown,
        InventoryEquip,
        InventoryDrop,
        InventoryUse,
        InventoryCraftArmor,
        CraftingSelectFirst,
        CraftingSelectSecond,
        CraftingCombine,
        CraftingCancel,
        CombatNormalAttack,
        CombatStealthAttack,
        CombatMagicalAttack,
        DeathRespawn,
        CloseInventory,
        Quit
    }

    public class Input
    {
        private static readonly Dictionary<ConsoleKey, InputCommand> _normalBindings = new()
        {
            [ConsoleKey.W] = InputCommand.Up,
            [ConsoleKey.UpArrow] = InputCommand.Up,
            [ConsoleKey.S] = InputCommand.Down,
            [ConsoleKey.DownArrow] = InputCommand.Down,
            [ConsoleKey.A] = InputCommand.Left,
            [ConsoleKey.LeftArrow] = InputCommand.Left,
            [ConsoleKey.D] = InputCommand.Right,
            [ConsoleKey.RightArrow] = InputCommand.Right,
            [ConsoleKey.E] = InputCommand.Pickup,
            [ConsoleKey.X] = InputCommand.SwapWeapons,
            [ConsoleKey.B] = InputCommand.OpenInventory,
            [ConsoleKey.J] = InputCommand.OpenJournal,
            [ConsoleKey.D1] = InputCommand.DropLeftHand,
            [ConsoleKey.NumPad1] = InputCommand.DropLeftHand,
            [ConsoleKey.D2] = InputCommand.DropRightHand,
            [ConsoleKey.NumPad2] = InputCommand.DropRightHand,
            [ConsoleKey.Q] = InputCommand.Quit,
            [ConsoleKey.Escape] = InputCommand.Quit
        };

        private static readonly Dictionary<ConsoleKey, InputCommand> _inventoryBindings = new()
        {
            [ConsoleKey.W] = InputCommand.InventoryUp,
            [ConsoleKey.UpArrow] = InputCommand.InventoryUp,
            [ConsoleKey.S] = InputCommand.InventoryDown,
            [ConsoleKey.DownArrow] = InputCommand.InventoryDown,
            [ConsoleKey.E] = InputCommand.InventoryEquip,
            [ConsoleKey.D] = InputCommand.InventoryDrop,
            [ConsoleKey.U] = InputCommand.InventoryUse,
            [ConsoleKey.C] = InputCommand.InventoryCraftArmor,
            [ConsoleKey.Escape] = InputCommand.CloseInventory,
            [ConsoleKey.J] = InputCommand.OpenJournal
        };

        private static readonly Dictionary<ConsoleKey, InputCommand> _combatBindings = new()
        {
            [ConsoleKey.D1] = InputCommand.CombatNormalAttack,
            [ConsoleKey.NumPad1] = InputCommand.CombatNormalAttack,
            [ConsoleKey.D2] = InputCommand.CombatStealthAttack,
            [ConsoleKey.NumPad2] = InputCommand.CombatStealthAttack,
            [ConsoleKey.D3] = InputCommand.CombatMagicalAttack,
            [ConsoleKey.NumPad3] = InputCommand.CombatMagicalAttack,
            [ConsoleKey.J] = InputCommand.OpenJournal,
            [ConsoleKey.Escape] = InputCommand.Quit
        };

        private static readonly Dictionary<ConsoleKey, InputCommand> _deathBindings = new()
        {
            [ConsoleKey.R] = InputCommand.DeathRespawn,
            [ConsoleKey.Q] = InputCommand.Quit,
            [ConsoleKey.J] = InputCommand.OpenJournal
        };

        private static readonly Dictionary<ConsoleKey, InputCommand> _craftingBindings = new()
        {
            [ConsoleKey.W] = InputCommand.CraftingSelectFirst,
            [ConsoleKey.UpArrow] = InputCommand.CraftingSelectFirst,
            [ConsoleKey.S] = InputCommand.CraftingSelectSecond,
            [ConsoleKey.DownArrow] = InputCommand.CraftingSelectSecond,
            [ConsoleKey.E] = InputCommand.CraftingCombine,
            [ConsoleKey.J] = InputCommand.OpenJournal,
            [ConsoleKey.Escape] = InputCommand.CraftingCancel
        };

        public InputCommand ReadCommand(GameMode mode)
        {
            if (!ConsoleHost.TryReadKey(out var key))
                return InputCommand.None;

            var bindings = ResolveBindings(mode);
            return bindings.TryGetValue(key, out var command) ? command : InputCommand.Unknown;
        }

        private static Dictionary<ConsoleKey, InputCommand> ResolveBindings(GameMode mode)
        {
            if (mode == GameMode.Inventory)
                return _inventoryBindings;

            if (mode == GameMode.Combat)
                return _combatBindings;

            if (mode == GameMode.Death)
                return _deathBindings;

            if (mode == GameMode.WeaponCrafting)
                return _craftingBindings;

            return _normalBindings;
        }
    }
}