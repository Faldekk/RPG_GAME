using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using RPG_GAME.Model;
using RPG_GAME.UI;

namespace RPG_GAME.App
{
    public enum InputCommand
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Quit,
        Pickup,
        Drop,
        FirstWeapon,
        SecondWeapon,
        Shoot
    }

    public class Input
    {
        public InputCommand ReadCommand()
        {
            var key = Console.ReadKey(true);
            //System.Console.Title = $"Last key: {key.Key}";
            return key.Key switch
            {

                ConsoleKey.W => InputCommand.Up,
                ConsoleKey.S => InputCommand.Down,
                ConsoleKey.A => InputCommand.Left,
                ConsoleKey.D => InputCommand.Right,
                ConsoleKey.Q => InputCommand.Quit,
                ConsoleKey.E => InputCommand.Pickup,
                ConsoleKey.G => InputCommand.Drop,
                ConsoleKey.D1 => InputCommand.FirstWeapon,
                ConsoleKey.D2 => InputCommand.SecondWeapon,
                ConsoleKey.R => InputCommand.Shoot,
                _ => InputCommand.None
            };

        }
    }
}