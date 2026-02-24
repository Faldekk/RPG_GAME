using System;
using RPG_GAME.App;

namespace RPG_GAME
{
    internal static class Program
    {
        private static void Main()
        {
            Console.CursorVisible = false;
            try
            {
                
                const int width = 80;
                const int height = 30;

                if (Console.BufferWidth < width) Console.BufferWidth = width;
                if (Console.WindowWidth < width) Console.WindowWidth = width;

                if (Console.BufferHeight < height) Console.BufferHeight = height;
                if (Console.WindowHeight < height) Console.WindowHeight = height;
            }
            catch
            {
            }

            Console.Clear();

            var game = new Game();
            game.Run();

       
            Console.CursorVisible = true;
            Console.ResetColor();
            Console.SetCursorPosition(0, Math.Min(Console.CursorTop, Console.BufferHeight - 1));
            Console.WriteLine();
        }
    }
}