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
        InventoryUp,
        InventoryDown,
        InventoryEquip,
        InventoryDrop,
        InventoryUse,
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
            [ConsoleKey.Escape] = InputCommand.CloseInventory
        };

        public InputCommand ReadCommand(GameMode mode)
        {
            if (!ConsoleHost.TryReadKey(out var key))
                return InputCommand.None;

            var bindings = ReferenceEquals(mode, GameMode.Inventory) ? _inventoryBindings : _normalBindings;
            return bindings.TryGetValue(key, out var command) ? command : InputCommand.Unknown;
        }
    }
}