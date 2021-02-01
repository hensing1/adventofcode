using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._17
{
    [ProblemDate(17)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            const int iterations = 6;

            string[] lines = System.IO.File.ReadAllLines(input);

            // making the pocket dimension as big as it can possibly get after n iterations because I don't want it to be resized after each iteration
            bool[,,] pocketDimension = new bool[lines[0].Length + (2 * iterations), lines.Length + (2 * iterations), 1 + (2 * iterations)];

            // plopping the starting space right in the middle
            int z = iterations;
            for (int y = iterations; y < iterations + lines.Length; y++)
                for (int x = iterations; x < iterations + lines[0].Length; x++)
                    pocketDimension[x, y, z] = lines[y][x] == '#';

            for (int i = 0; i < iterations; i++)
            {

            }
            throw new NotImplementedException();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }
    }
}
