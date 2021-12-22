using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._05
{
    [ProblemDate(2021, 05)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            List<int[]> ventLines = ParseInput(input, false);

            int[,] floor = MakeInitialFloor(ventLines);

            foreach (int[] line in ventLines)
                AddLineToFloor(line, floor);

            return CountOverlaps(floor).ToString();
        }

        public string SolveSecond(string input)
        {
            List<int[]> ventLines = ParseInput(input, true);

            int[,] floor = MakeInitialFloor(ventLines);

            foreach (int[] line in ventLines)
                AddLineToFloor(line, floor);

            return CountOverlaps(floor).ToString();
        }

        private List<int[]> ParseInput(string input, bool diagonals)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var ventLines = new List<int[]>();

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, @"(?<x1>\d+),(?<y1>\d+) -> (?<x2>\d+),(?<y2>\d+)");

                var x1 = int.Parse(match.Groups["x1"].Value);
                var y1 = int.Parse(match.Groups["y1"].Value);
                var x2 = int.Parse(match.Groups["x2"].Value);
                var y2 = int.Parse(match.Groups["y2"].Value);

                if (diagonals || x1 == x2 || y1 == y2)
                {
                    var ventLine = new[] { x1, y1, x2, y2 };
                    ventLines.Add(ventLine);
                }
            }

            return ventLines;
        }

        private int[,] MakeInitialFloor(List<int[]> ventLines)
        {
            int maxX = ventLines.Max(line => Math.Max(line[0], line[2]));
            int maxY = ventLines.Max(line => Math.Max(line[1], line[3]));

            return new int[maxX + 1, maxY + 1];
        }

        private void AddLineToFloor(int[] line, int[,] floor)
        {
            int xDir,  yDir;
            int startX = line[0], startY = line[1], endX = line[2], endY = line[3];

            if (startX < endX)
                xDir = 1;
            else if (startX == endX)
                xDir = 0;
            else
                xDir = -1;
            
            if (startY < endY)
                yDir = 1;
            else if (startY == endY)
                yDir = 0;
            else
                yDir = -1;

            int x = startX, y = startY;
            floor[x, y]++;

            while (x != endX || y != endY)
            {
                x += xDir;
                y += yDir;
                floor[x, y]++;
            } 
        }

        private int CountOverlaps(int[,] floor)
        {
            //var floor1D = floor.Cast<int>().ToArray();            // works, but is slow as BALLS
            //return floor1D.Count(pointValue => pointValue >= 2);  // total time with this: ~400-450 ms; time with code below: ~25 ms
                                                                    // so yeah, don't do this

            int count = 0;
            for (int x = 0; x < floor.GetLength(0); x++)
                for (int y = 0; y < floor.GetLength(1); y++)
                    if (floor[x, y] >= 2)
                        count++;

            return count;
        }
    }
}
