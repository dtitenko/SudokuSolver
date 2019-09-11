using System;
using System.Linq;
using System.Threading;

namespace Sudoku
{
    class Program
    {
        static void Main()
        {
            char[][] board = @"
53..7....
6..195...
.98....6.
8...6...3
4..8.3..1
7...2...6
.6....28.
...419..5
....8..79"
                .Split(Environment.NewLine)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Select(x => x).ToArray()).ToArray();

            var solution = new Solution();
            solution.SolveSudoku(board);
        }
    }

    public class Solution
    {
        public void SolveSudoku(char[][] source)
        {
            var board = new Board(source);
            board.Fill(print: false);
            board.Print(leaveInTheEnd: true);
        }
    }

    public class Board
    {
        private const string AllNumbers = "123456789";

        private readonly char[][] _board;
        private readonly bool[][] _rows = new bool[9][];
        private readonly bool[][] _columns = new bool[9][];
        private readonly bool[][] _blocks = new bool[9][];

        public Board(char[][] board)
        {
            _board = board;

            for (int i = 0; i < 9; i++)
            {
                _rows[i] = new bool[9];
                _columns[i] = new bool[9];
                _blocks[i] = new bool[9];
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var c = board[i][j];
                    if (c == '.') continue;
                    _rows[i][c - 49] = true;
                    _columns[j][c - 49] = true;
                    _blocks[GetBlockIndex(i, j)][c - 49] = true;
                }
            }
        }

        public void Fill(bool print) => DFS(0, print);

        private bool DFS(int index, bool print)
        {
            if (index == 81) return true;
            var row = index / 9;
            var column = index - row * 9;
            if (_board[row][column] != '.')
            {
                return DFS(index + 1, print);
            }

            foreach (var number in AllNumbers)
            {
                if (!IsValid(number, row, column)) continue;
                SetNumber(number, row, column);
                if (print) Print();
                if (DFS(index + 1, print)) return true;
            }

            SetNumber('.', row, column);
            return false;
        }

        private void SetNumber(char number, int row, int column)
        {
            var existing = _board[row][column];
            if (number == existing) return;
            if (existing != '.')
            {
                _columns[column][existing - 49] = false;
                _rows[row][existing - 49] = false;
                _blocks[GetBlockIndex(row, column)][existing - 49] = false;
            }

            if (number != '.')
            {
                _columns[column][number - 49] = true;
                _rows[row][number - 49] = true;
                _blocks[GetBlockIndex(row, column)][number - 49] = true;
            }

            _board[row][column] = number;
        }

        private bool IsValid(char number, int i, int j) =>
            !_rows[i][number - 49]
            && !_columns[j][number - 49]
            && !_blocks[GetBlockIndex(i, j)][number - 49];

        private int GetBlockIndex(int row, int column) => row / 3 * 3 + column / 3;

        public void Print(bool leaveInTheEnd = false)
        {
            var board = _board;
            Console.CursorVisible = false;
            var top = Console.CursorTop;
            var left = Console.CursorLeft;
            for (int i = 0; i < board.Length; i++)
            {
                if (i % 3 == 0 && i > 0) Console.WriteLine(new string('-', 9 * 3 + 2 * 4));
                var row = board[i];
                for (int j = 0; j < row.Length; j++)
                {
                    var symbol = row[j];
                    if (symbol == '.') symbol = ' ';
                    string splitter = j % 3 == 0 && j > 0 ? "  |" : "";
                    Console.Write($"{splitter}  {symbol}");
                }

                Console.WriteLine();
            }

            if (!leaveInTheEnd)
            {
                Console.CursorTop = top;
                Console.CursorLeft = left;
            }

            Console.CursorVisible = true;
        }
    }
}