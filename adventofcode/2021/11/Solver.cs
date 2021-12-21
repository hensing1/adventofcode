using System.Collections.Generic;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._11
{
    [ProblemDate(2021, 11)]
    class Solver : ISolver
    {
        static readonly (int, int)[] adjacent = new[] { (-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1) };

        public string SolveFirst(string input)
        {
            int[,] octopuses = ParseInput(input);

            const int NUM_STEPS = 100;
            int totalFlashes = 0;

            for (int i = 1; i <= NUM_STEPS; i++)
                totalFlashes += SimulateStep(octopuses);

            return totalFlashes.ToString();
        }

        public string SolveSecond(string input)
        {
            int[,] octopuses = ParseInput(input);

            for (int i = 1; ; i++)
                if (SimulateStep(octopuses) == octopuses.Length)
                    return i.ToString();
        }

        private int[,] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var octopuses = new int[lines[0].Length, lines.Length];

            for (int j = 0; j < lines.Length; j++)
                for (int i = 0; i < lines[j].Length; i++)
                    octopuses[i, j] = lines[i][j] - '0';

            return octopuses;
        }

        private int SimulateStep(int[,] octopuses)
        {
            var dueToFlash = new Stack<(int, int)>();
            var alreadyFlashed = new List<(int, int)>();

            for (int i = 0; i < octopuses.GetLength(0); i++)
                for (int j = 0; j < octopuses.GetLength(1); j++)
                {
                    octopuses[i, j]++;
                    if (octopuses[i, j] == 10)
                        dueToFlash.Push((i, j));
                }

            while (dueToFlash.Count > 0)
            {
                (int i, int j) = dueToFlash.Pop();
                foreach ((int di, int dj) in adjacent)
                {
                    (int ii, int jj) = (i + di, j + dj);

                    if (ii >= 0 && ii < octopuses.GetLength(0) &&
                        jj >= 0 && jj < octopuses.GetLength(1))
                    {
                        octopuses[ii, jj]++;
                        if (octopuses[ii, jj] == 10)
                            dueToFlash.Push((ii, jj));
                    }
                }
                alreadyFlashed.Add((i, j));
            }

            foreach ((int i, int j) in alreadyFlashed)
                octopuses[i, j] = 0;

            return alreadyFlashed.Count;
        }
    }
}
