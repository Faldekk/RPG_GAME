using System;
using System.Text;
using RPG_GAME.App;

namespace RPG_GAME
{
    internal static class Program
    {
        
        private static void Main()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
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
            Console.SetCursorPosition(0, Math.Min(Console.CursorTop, Console.BufferHeight - 1));
            Console.WriteLine();
            
        }
    }
}