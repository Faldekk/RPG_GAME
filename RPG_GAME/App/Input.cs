using System;
using RPG_GAME.Model;
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
        public InputCommand ReadCommand(GameMode mode)
        {
            if (!ConsoleHost.TryReadKey(out var key))
                return InputCommand.None;

            if (mode == GameMode.Inventory)
            {
                return key switch
                {
                    ConsoleKey.W or ConsoleKey.UpArrow => InputCommand.InventoryUp,
                    ConsoleKey.S or ConsoleKey.DownArrow => InputCommand.InventoryDown,
                    ConsoleKey.E => InputCommand.InventoryEquip,
                    ConsoleKey.D => InputCommand.InventoryDrop,
                    ConsoleKey.U => InputCommand.InventoryUse,
                    ConsoleKey.Escape => InputCommand.CloseInventory,
                    _ => InputCommand.Unknown
                };
            }

            return key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => InputCommand.Up,
                ConsoleKey.S or ConsoleKey.DownArrow => InputCommand.Down,
                ConsoleKey.A or ConsoleKey.LeftArrow => InputCommand.Left,
                ConsoleKey.D or ConsoleKey.RightArrow => InputCommand.Right,
                ConsoleKey.E => InputCommand.Pickup,
                ConsoleKey.X => InputCommand.SwapWeapons,
                ConsoleKey.B => InputCommand.OpenInventory,
                ConsoleKey.D1 or ConsoleKey.NumPad1 => InputCommand.DropLeftHand,
                ConsoleKey.D2 or ConsoleKey.NumPad2 => InputCommand.DropRightHand,
                ConsoleKey.Q or ConsoleKey.Escape => InputCommand.Quit,
                _ => InputCommand.Unknown
            };
        }
    }
}