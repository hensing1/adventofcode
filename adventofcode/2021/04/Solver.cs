using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._04
{
    [ProblemDate(2021, 4)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            ParseInput(input, out int[] luckyNumbers, out List<int[][]> boards);
            var markSheets = MakeMarksheets(boards.Count);

            for (int i = 0; i < luckyNumbers.Length; i++)
            {
                for (int bi = 0; bi < boards.Count; bi++)
                {
                    MarkNumber(boards[bi], markSheets[bi], luckyNumbers[i]);
                    if (IsBingo(boards[bi], markSheets[bi]))
                        return (GetTally(boards[bi], markSheets[bi]) * luckyNumbers[i]).ToString();
                }
            }

            return "No Bingo found";
        }

        public string SolveSecond(string input)
        {
            ParseInput(input, out int[] luckyNumbers, out List<int[][]> boards);
            var markSheets = MakeMarksheets(boards.Count);

            for (int i = 0; i < luckyNumbers.Length; i++)
            {
                for (int bi = 0; bi < boards.Count; bi++)
                {
                    MarkNumber(boards[bi], markSheets[bi], luckyNumbers[i]);
                    if (IsBingo(boards[bi], markSheets[bi]))
                    {
                        if (boards.Count == 1)
                            return (GetTally(boards[bi], markSheets[bi]) * luckyNumbers[i]).ToString();

                        boards.RemoveAt(bi);
                        markSheets.RemoveAt(bi);
                        bi--;
                    }
                }
            }

            return "Not all boards bingoed";
        }

        private void ParseInput(string input, out int[] luckyNumbers, out List<int[][]> boards)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            luckyNumbers = lines[0].Split(',').SelectMany(num => new[] { int.Parse(num) }).ToArray();

            boards = new List<int[][]>();
            for (int i = 2; i < lines.Length; i += 6)
            {
                var board = new int[5][];
                for (int bi = 0; bi < 5; bi++)
                    board[bi] = lines[i + bi].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .SelectMany(num => new[] { int.Parse(num) }).ToArray();
                boards.Add(board);
            }
        }

        private List<bool[][]> MakeMarksheets(int count)
        {
            var sheets = new List<bool[][]>();
            for (int i = 0; i < count; i++)
            {
                var sheet = new bool[5][];
                for (int j = 0; j < 5; j++)
                    sheet[j] = new bool[5];

                sheets.Add(sheet);
            }
            return sheets;
        }

        private void MarkNumber(int[][] board, bool[][] marks, int number)
        {
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    if (board[y][x] == number)
                        marks[y][x] = true;
        }

        private bool IsBingo(int[][] board, bool[][] marks)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                    if (!marks[y][x])
                        goto NextRow;

                return true;

            NextRow:
                continue;
            }

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                    if (!marks[y][x])
                        goto NextColumn;

                return true;

            NextColumn:
                continue;
            }

            return false;
        }

        private int GetTally(int[][] board, bool[][] marks)
        {
            int tally = 0;
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    if (!marks[y][x])
                        tally += board[y][x];
            return tally;
        }
    }
}
