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
        Quit
    }

    public class Input
    {
        public InputCommand ReadCommand()
        {
            if (!ConsoleHost.TryReadKey(out var key))
                return InputCommand.None;

            return key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => InputCommand.Up,
                ConsoleKey.S or ConsoleKey.DownArrow => InputCommand.Down,
                ConsoleKey.A or ConsoleKey.LeftArrow => InputCommand.Left,
                ConsoleKey.D or ConsoleKey.RightArrow => InputCommand.Right,
                ConsoleKey.E => InputCommand.Pickup,
                ConsoleKey.G => InputCommand.BackpackAction,
                ConsoleKey.X => InputCommand.SwapWeapons,
                ConsoleKey.D1 or ConsoleKey.NumPad1 => InputCommand.DropLeftHand,
                ConsoleKey.D2 or ConsoleKey.NumPad2 => InputCommand.DropRightHand,
                ConsoleKey.Q or ConsoleKey.Escape => InputCommand.Quit,
                _ => InputCommand.Unknown
            };
        }
    }
}