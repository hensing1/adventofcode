using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._03
{
    [ProblemDate(2020, 3)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            return GetTreesForSlope(lines, 3).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var slopes = new [] { (1, 1), (3, 1), (5, 1), (7, 1), (1, 2) };
            long result = 1;

            foreach (var slope in slopes)
                result *= GetTreesForSlope(lines, slope.Item1, slope.Item2);

            return result.ToString();
        }

        private int GetTreesForSlope(string[] lines, int slopeX, int slopeY = 1)
        {
            var index = 0;
            var tree = '#';
            var treeCounter = 0;

            for (int i = 0; i < lines.Length; i += slopeY)
            {
                if (lines[i][index] == tree)
                    treeCounter++;
                index += slopeX;
                index %= lines[i].Length;
            }

            return treeCounter;
        }
    }
}
