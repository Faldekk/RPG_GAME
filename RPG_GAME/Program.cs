using System;
using RPG_GAME.App;

namespace RPG_GAME
{
    internal static class Program
    {
        private const int RequiredWidth = 80;
        private const int RequiredHeight = 30;

        private static void Main()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorVisible = false;

            TryPrepareConsole();

            if (Console.WindowWidth < RequiredWidth ||
                Console.WindowHeight < RequiredHeight ||
                Console.BufferWidth < RequiredWidth ||
                Console.BufferHeight < RequiredHeight)
            {
                Console.Clear();
                Console.WriteLine($"Console too small. Required: {RequiredWidth}x{RequiredHeight}");
                Console.WriteLine($"Current window: {Console.WindowWidth}x{Console.WindowHeight}");
                Console.WriteLine($"Current buffer: {Console.BufferWidth}x{Console.BufferHeight}");
                Console.CursorVisible = true;
                return;
            }

            Console.Clear();

            var game = new Game();
            game.Run();

            Console.CursorVisible = true;
            Console.SetCursorPosition(0, Math.Min(Console.CursorTop, Console.BufferHeight - 1));
            Console.WriteLine();
        }

        private static void TryPrepareConsole()
        {
            try
            {
                if (Console.BufferWidth < RequiredWidth)
                    Console.BufferWidth = RequiredWidth;

                if (Console.BufferHeight < RequiredHeight)
                    Console.BufferHeight = RequiredHeight;

                if (Console.WindowWidth < RequiredWidth)
                    Console.WindowWidth = RequiredWidth;

                if (Console.WindowHeight < RequiredHeight)
                    Console.WindowHeight = RequiredHeight;
            }
            catch
            {
            }
        }
    }
}