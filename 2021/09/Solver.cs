using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._09
{
    [ProblemDate(2021, 9)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[][] vals = ParseInput(lines);

            var minima = LocalMinima(vals);
            return (minima.Sum() + minima.Count()).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[][] vals = ParseInput(lines);
            
            var basinSizes = new List<int>();
            for (int i = 0; i < vals.Length; i++)
                for (int j = 0; j < vals[0].Length; j++)
                    if (vals[i][j] != 9)
                    {
                        basinSizes.Add(DoFloodfill(vals, i, j));
                    }

            return basinSizes.OrderByDescending(s => s).Take(3).Aggregate((curr, next) => curr * next).ToString();
        }

        private static int[][] ParseInput(string[] lines)
        {
            var vals = new int[lines.Length][];

            for (int i = 0; i < lines.Length; i++)
            {
                vals[i] = new int[lines[i].Length];
                for (int j = 0; j < lines[i].Length; j++)
                    vals[i][j] = lines[i][j] - '0';
            }

            return vals;
        }

        private IEnumerable<int> LocalMinima(int[][] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                for (int j = 0; j < vals[0].Length; j++)
                {
                    if (i > 0 && vals[i - 1][j] <= vals[i][j])
                        goto NextCell;

                    if (j > 0 && vals[i][j - 1] <= vals[i][j])
                        goto NextCell;

                    if (i < vals.Length - 1 && vals[i + 1][j] <= vals[i][j])
                        goto NextCell;

                    if (j < vals[0].Length - 1 && vals[i][j + 1] <= vals[i][j])
                        goto NextCell;

                    yield return vals[i][j];
                        
                NextCell:
                    continue;
                }
            }
        }

        private int DoFloodfill(int[][] vals, int i, int j)
        {
            int basinSize = 0;
            var cellsToVisit = new Queue<(int, int)>();
            cellsToVisit.Enqueue((i, j));
            vals[i][j] = 9;

            while (cellsToVisit.Count > 0)
            {
                (i, j) = cellsToVisit.Dequeue();

                if (i > 0 && vals[i - 1][j] != 9)
                {
                    cellsToVisit.Enqueue((i - 1, j));
                    vals[i - 1][j] = 9;
                }

                if (j > 0 && vals[i][j - 1] != 9)
                {
                    cellsToVisit.Enqueue((i, j - 1));
                    vals[i][j - 1] = 9;
                }

                if (i < vals.Length - 1 && vals[i + 1][j] != 9)
                {
                    cellsToVisit.Enqueue((i + 1, j));
                    vals[i + 1][j] = 9;
                }

                if (j < vals[0].Length - 1 && vals[i][j + 1] != 9)
                {
                    cellsToVisit.Enqueue((i, j + 1));
                    vals[i][j + 1] = 9;
                }

                basinSize++;
            }

            return basinSize;
        }
    }
}
