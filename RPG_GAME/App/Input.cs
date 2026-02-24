using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace RPG_GAME.App
{
    public enum InputCommand
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Quit
    }

    public class Input
    {
        public InputCommand ReadCommand()
        {
            var key = Console.ReadKey(true);
            System.Console.Title = $"Last key: {key.Key}";
            return key.Key switch
            {

                ConsoleKey.W => InputCommand.Up,
                ConsoleKey.S => InputCommand.Down,
                ConsoleKey.A => InputCommand.Left,
                ConsoleKey.D => InputCommand.Right,
                ConsoleKey.Q => InputCommand.Quit,
                _ => InputCommand.None
            };
        }
    }
}