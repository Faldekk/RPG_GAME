using System;
using System.Text;

namespace RPG_GAME.UI
{
    public class ConsoleBuffer
    {
        private readonly int _rows;
        private readonly int _cols;
        private readonly char[,] _buffer;

        public ConsoleBuffer(int rows, int cols)
        {
            _rows = ClampRows(rows);
            _cols = ClampCols(cols);
            _buffer = new char[_rows, _cols];
        }

        public void Clear(char fill = ' ')
        {
            for (int y = 0; y < _rows; y++)
                for (int x = 0; x < _cols; x++)
                    _buffer[y, x] = fill;
        }

        public void PutChar(int row, int col, char ch)
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
                return;

            _buffer[row, col] = ch;
        }

        public void PutString(int row, int col, string text)
        {
            for (int i = 0; i < text.Length; i++)
                PutChar(row, col + i, text[i]);
        }

        public void Flush()
        {
            if (ConsoleHost.IsOutputRedirected)
            {
                FlushRedirected();
                return;
            }

            ConsoleHost.ResetCursor();

            for (int y = 0; y < _rows; y++)
                ConsoleHost.WriteAt(0, y, GetRowText(y));
        }

        private void FlushRedirected()
        {
            var sb = new StringBuilder(_rows * (_cols + Environment.NewLine.Length));

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                    sb.Append(_buffer[y, x]);

                if (y < _rows - 1)
                    sb.Append(Environment.NewLine);
            }

            ConsoleHost.Write(sb.ToString());
        }

        private string GetRowText(int row)
        {
            var chars = new char[_cols];
            for (int x = 0; x < _cols; x++)
                chars[x] = _buffer[row, x];

            return new string(chars);
        }

        private static int ClampRows(int requestedRows)
        {
            if (ConsoleHost.IsOutputRedirected)
                return Math.Max(1, requestedRows);

            int limit = Math.Max(1, ConsoleHost.WindowHeight - 1);
            return Math.Max(1, Math.Min(requestedRows, limit));
        }

        private static int ClampCols(int requestedCols)
        {
            if (ConsoleHost.IsOutputRedirected)
                return Math.Max(1, requestedCols);

            int limit = Math.Max(1, ConsoleHost.WindowWidth - 1);
            return Math.Max(1, Math.Min(requestedCols, limit));
        }
    }
}