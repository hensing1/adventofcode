using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._13
{
    [ProblemDate(2021, 13)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            bool[,] sheet = GetSheet(input);
            (int foldIndex, bool foldVertically) = GetFolds(input).First();

            sheet = FoldSheet(sheet, foldIndex, foldVertically);
            int count = 0;
            for (int x = 0; x < sheet.GetLength(0); x++)
                for (int y = 0; y < sheet.GetLength(1); y++)
                    if (sheet[x, y])
                        count++;

            return count.ToString();
        }

        public string SolveSecond(string input)
        {
            bool[,] sheet = GetSheet(input);
            (int, bool)[] folds = GetFolds(input);

            foreach ((int foldIndex, bool foldVertically) in folds)
                sheet = FoldSheet(sheet, foldIndex, foldVertically);

            return "\n" + ToPrettyString(sheet);
        }

        private bool[,] GetSheet(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var coords = new List<(int, int)>();

            for (int i = 0; lines[i] != string.Empty; i++)
            {
                string[] coordString = lines[i].Split(',');
                coords.Add((int.Parse(coordString[0]), int.Parse(coordString[1])));
            }

            var maxX = coords.Max(tuple => tuple.Item1);
            var maxY = coords.Max(tuple => tuple.Item2);
            var sheet = new bool[maxX + 1, maxY + 1];

            foreach ((int x, int y) in coords)
                sheet[x, y] = true;

            return sheet;
        }

        private (int, bool)[] GetFolds(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            string[] foldStrings = lines.SkipWhile(line => !line.StartsWith("fold along")).ToArray();
            var folds = new (int, bool)[foldStrings.Length];
            for (int i = 0; i < foldStrings.Length; i++)
            {
                Match match = Regex.Match(foldStrings[i], @"(?<dim>x|y)=(?<index>\d+)");
                int index = int.Parse(match.Groups["index"].Value);
                bool foldVertically = match.Groups["dim"].Value == "x";

                folds[i] = (index, foldVertically);
            }

            return folds;
        }

        private bool[,] FoldSheet(bool[,] sheet, int foldIndex, bool foldVertically)
        {
            if (foldVertically)
                return FoldVertically(sheet, foldIndex);
            else
                return FoldHorizontally(sheet, foldIndex);
        }

        private bool[,] FoldVertically(bool[,] sheet, int foldIndex)
        {
            var result = new bool[foldIndex, sheet.GetLength(1)];

            // copy rows unaffected by the fold
            int foldSize = sheet.GetLength(0) - (foldIndex + 1);
            int numUnaffectedRows = sheet.GetLength(0) - ((2 * foldSize) + 1);
            for (int x = 0; x < numUnaffectedRows; x++)
                for (int y = 0; y < sheet.GetLength(1); y++)
                    result[x, y] = sheet[x, y];

            // fold sheet
            for (int offset = 1; offset <= foldSize; offset++)
                for (int y = 0; y < sheet.GetLength(1); y++)
                    result[foldIndex - offset, y] = sheet[foldIndex - offset, y] | sheet[foldIndex + offset, y];

            return result;
        }

        private bool[,] FoldHorizontally(bool[,] sheet, int foldIndex)
        {
            var result = new bool[sheet.GetLength(0), foldIndex];

            // copy rows unaffected by the fold
            int foldSize = sheet.GetLength(1) - (foldIndex + 1);
            int numUnaffectedRows = sheet.GetLength(1) - ((2 * foldSize) + 1);
            for (int y = 0; y < numUnaffectedRows; y++)
                for (int x = 0; x < sheet.GetLength(0); x++)
                    result[x, y] = sheet[x, y];

            // fold sheet
            for (int offset = 1; offset <= foldSize; offset++)
                for (int x = 0; x < sheet.GetLength(0); x++)
                    result[x, foldIndex - offset] = sheet[x, foldIndex - offset] | sheet[x, foldIndex + offset];

            return result;
        }

        private static string ToPrettyString(bool[,] sheet)
        {
            string result = string.Empty;
            for (int y = 0; y < sheet.GetLength(1); y++)
            {
                for (int x = 0; x < sheet.GetLength(0); x++)
                    result += sheet[x, y] ? '#' : '.';

                result += "\n";
            }

            return result;
        }
    }
}
