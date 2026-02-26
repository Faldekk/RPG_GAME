using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_GAME.UI
{
    public class ConsoleBuffer
    {
        private readonly int _rows;
        private readonly int _cols;
        private readonly char[,] _buffer;

        public ConsoleBuffer(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _buffer = new char[rows, cols];
        }

        public void Clear(char fill = ' ')
        {
            for (int y = 0; y < _rows; y++)
                for (int x = 0; x < _cols; x++)
                    _buffer[y, x] = fill;
        }

        public void PutChar(int row, int col, char ch)

        {
            Console.OutputEncoding = Encoding.Unicode;
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                return;

            _buffer[row, col] = ch;
        }

        public void PutString(int row, int col, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                PutChar(row, col + i, text[i]);
            }
        }

        public void Flush()
        {
            var sb = new StringBuilder(_rows * (_cols + 1));

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                    sb.Append(_buffer[y, x]);

                sb.AppendLine();
            }

            System.Console.SetCursorPosition(0, 0);
            System.Console.Write(sb.ToString());
        }
    }
}