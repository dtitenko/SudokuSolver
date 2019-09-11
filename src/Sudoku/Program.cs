using System;
using System.Collections.Generic;
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
            board.Fill(print: true);
            board.Print(leaveInTheEnd: true);
        }
    }

    public class Board
    {
        private static readonly Dictionary<int, int[]> Blocks = new Dictionary<int, int[]>
        {
            { 0, new[] { 0, 1, 2 } },
            { 1, new[] { 0, 1, 2 } },
            { 2, new[] { 0, 1, 2 } },
            { 3, new[] { 3, 4, 5 } },
            { 4, new[] { 3, 4, 5 } },
            { 5, new[] { 3, 4, 5 } },
            { 6, new[] { 6, 7, 8 } },
            { 7, new[] { 6, 7, 8 } },
            { 8, new[] { 6, 7, 8 } }
        };

        private const string AllNumbers = "123456789";

        private readonly char[][] _board;

        public Board(char[][] board)
        {
            _board = board;
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
                if (CheckColumn(number, column) && CheckRow(number, row) && CheckBlock(number, row, column))
                {
                    _board[row][column] = number;
                    if (print) Print();
                    if (DFS(index + 1, print)) return true;
                }
            }
            
            _board[row][column] = '.';
            return false;
        }

        private bool CheckRow(char number, int i) =>
            _board[i].All(n => n == '.' || n != number);

        private bool CheckColumn(char number, int j) =>
            Enumerable.Range(0, 9).All(i => _board[i][j] == '.' || _board[i][j] != number);

        private bool CheckBlock(char number, int i, int j) =>
            !Blocks[i].Any(row => Blocks[j].Any(column => _board[row][column] != '.' && _board[row][column] == number));
        
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