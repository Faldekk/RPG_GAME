using System;
using System.Text;

namespace RPG_GAME.UI
{
    public static class ConsoleHost
    {
        public static bool IsOutputRedirected => Console.IsOutputRedirected;
        public static int WindowWidth => Console.WindowWidth;
        public static int WindowHeight => Console.WindowHeight;

        public static void Initialize(int width, int height)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            EnsureSize(width, height);
            Console.Clear();
        }

        public static void Shutdown()
        {
            Console.CursorVisible = true;
        }

        public static bool TryReadKey(out ConsoleKey key)
        {
            if (!Console.KeyAvailable)
            {
                key = default;
                return false;
            }

            key = Console.ReadKey(true).Key;
            return true;
        }

        public static void Write(string text)
        {
            Console.Write(text);
        }

        public static void WriteAt(int left, int top, string text)
        {
            TrySetCursorPosition(left, top);
            Console.Write(text);
        }

        public static void ResetCursor()
        {
            TrySetCursorPosition(0, 0);
        }

        private static void EnsureSize(int width, int height)
        {
            try
            {
                if (Console.BufferWidth < width)
                    Console.BufferWidth = width;

                if (Console.BufferHeight < height)
                    Console.BufferHeight = height;

                int windowWidth = Math.Min(width, Console.LargestWindowWidth);
                int windowHeight = Math.Min(height, Console.LargestWindowHeight);

                if (Console.WindowWidth != windowWidth || Console.WindowHeight != windowHeight)
                    Console.SetWindowSize(windowWidth, windowHeight);
            }
            catch
            {
            }
        }

        private static void TrySetCursorPosition(int left, int top)
        {
            try
            {
                Console.SetCursorPosition(left, top);
            }
            catch
            {
            }
        }
    }
}
