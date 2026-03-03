using System;
using RPG_GAME.Model;

namespace RPG_GAME.App
{
    public enum InputCommand
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Pickup,
        Drop,
        SwapWeapons,      
        DropLeftHand,     
        DropRightHand,    
        Quit
    }

    public class Input
    {
        public InputCommand ReadCommand()
        {
            if (!Console.KeyAvailable)
                return InputCommand.None;

            var key = Console.ReadKey(true).Key;

            return key switch
            {
                ConsoleKey.W or ConsoleKey.UpArrow => InputCommand.Up,
                ConsoleKey.S or ConsoleKey.DownArrow => InputCommand.Down,
                ConsoleKey.A or ConsoleKey.LeftArrow => InputCommand.Left,
                ConsoleKey.D or ConsoleKey.RightArrow => InputCommand.Right,
                ConsoleKey.E => InputCommand.Pickup,
                ConsoleKey.G => InputCommand.Drop,
                ConsoleKey.X => InputCommand.SwapWeapons,      
                ConsoleKey.D1 or ConsoleKey.NumPad1 => InputCommand.DropLeftHand,  // 1 - upuść lewą
                ConsoleKey.D2 or ConsoleKey.NumPad2 => InputCommand.DropRightHand, // 2 - upuść prawą
                ConsoleKey.Q or ConsoleKey.Escape => InputCommand.Quit,
                _ => InputCommand.None
            };
        }
    }
}