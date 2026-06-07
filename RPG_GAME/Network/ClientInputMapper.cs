using System;
using RPG_GAME.Network.Dto;

namespace RPG_GAME.Network
{
    public static class ClientInputMapper
    {
        public static PlayerCommandDto? ReadCommand()
        {
            var key = Console.ReadKey(intercept: true).Key;

            PlayerCommandKind? command = key switch
            {
                ConsoleKey.W => PlayerCommandKind.MoveUp,
                ConsoleKey.S => PlayerCommandKind.MoveDown,
                ConsoleKey.A => PlayerCommandKind.MoveLeft,
                ConsoleKey.D => PlayerCommandKind.MoveRight,

                ConsoleKey.E => PlayerCommandKind.PickUp,

                ConsoleKey.Q => PlayerCommandKind.Quit,

                ConsoleKey.D1 => PlayerCommandKind.CombatNormalAttack,
                ConsoleKey.D2 => PlayerCommandKind.CombatStealthAttack,
                ConsoleKey.D3 => PlayerCommandKind.CombatMagicalAttack,

                _ => null
            };

            if (command == null)
                return null;

            return new PlayerCommandDto
            {
                Command = command.Value
            };
        }
    }
}