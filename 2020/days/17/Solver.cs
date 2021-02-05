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
        const char Alive = '#';
        const char Dead = '.';

        public string SolveFirst(string input)
        {
            const int iterations = 6;

            string[] lines = System.IO.File.ReadAllLines(input);

            // making the pocket dimension slightly bigger than it can possibly get after n iterations because I don't want it to be resized after each iteration
            bool[,,] pocketDimension = new bool[lines[0].Length + (2 * (iterations + 1)), lines.Length + (2 * (iterations + 1)), 1 + (2 * (iterations + 1))];

            // plopping the starting space right in the middle
            int z = 0;
            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[0].Length; x++)
                    pocketDimension[x + iterations + 1, y + iterations + 1, z + iterations + 1] = lines[y][x] == Alive;

            for (int i = 0; i < iterations; i++)
            {
                //PrintDimension(pocketDimension);
                short[,,] neighbours = CalcNeighbours_3D(pocketDimension);

                var nextIteration = new bool[pocketDimension.GetLength(0), pocketDimension.GetLength(1), pocketDimension.GetLength(2)];

                for (z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                    for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                        for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                            nextIteration[x, y, z] = ApplyRules(pocketDimension[x, y, z], neighbours[x, y, z]);

                pocketDimension = nextIteration;
            }

            int sum = 0;
            for (z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                    for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                        if (pocketDimension[x, y, z])
                            sum++;

            return sum.ToString();
        }

        private short[,,] CalcNeighbours_3D(bool[,,] pocketDimension)
        {
            var neighbours = new short[pocketDimension.GetLength(0), pocketDimension.GetLength(1), pocketDimension.GetLength(2)];

            // The outermost layer of the pocket universe will never be reached, therefore we can ignore it and spare ourselves all the boundary checking
            for (int z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                    for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                        if (pocketDimension[x, y, z])
                        {
                            // we don't need even more loops
                            neighbours[x-1, y-1, z-1]++;
                            neighbours[x  , y-1, z-1]++;
                            neighbours[x+1, y-1, z-1]++;
                            neighbours[x-1, y  , z-1]++;
                            neighbours[x  , y  , z-1]++;
                            neighbours[x+1, y  , z-1]++;
                            neighbours[x-1, y+1, z-1]++;
                            neighbours[x  , y+1, z-1]++;
                            neighbours[x+1, y+1, z-1]++;
                            neighbours[x-1, y-1, z  ]++;
                            neighbours[x  , y-1, z  ]++;
                            neighbours[x+1, y-1, z  ]++;
                            neighbours[x-1, y  , z  ]++;
                            neighbours[x+1, y  , z  ]++;
                            neighbours[x-1, y+1, z  ]++;
                            neighbours[x  , y+1, z  ]++;
                            neighbours[x+1, y+1, z  ]++;
                            neighbours[x-1, y-1, z+1]++;
                            neighbours[x  , y-1, z+1]++;
                            neighbours[x+1, y-1, z+1]++;
                            neighbours[x-1, y  , z+1]++;
                            neighbours[x  , y  , z+1]++;
                            neighbours[x+1, y  , z+1]++;
                            neighbours[x-1, y+1, z+1]++;
                            neighbours[x  , y+1, z+1]++;
                            neighbours[x+1, y+1, z+1]++;
                        }
            
            return neighbours;
        }

        private bool ApplyRules(bool cell, short numNeighbours)
        {
            if (cell && numNeighbours != 80)
                return numNeighbours == 2 || numNeighbours == 3;
            
            return cell ? 
                numNeighbours == 2 || numNeighbours == 3 : 
                numNeighbours == 3;
        }

        private void PrintDimension(bool[,,] dimension)
        {
            for (int z = 1; z < dimension.GetLength(2) - 1; z++)
            {
                for (int y = 1; y < dimension.GetLength(1) - 1; y++)
                {
                    for (int x = 1; x < dimension.GetLength(0) - 1; x++)
                        Console.Write(dimension[x, y, z] ? Alive : Dead);
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine("-----------------------------------");
        }

        public string SolveSecond(string input)
        {
            const int iterations = 6;

            string[] lines = System.IO.File.ReadAllLines(input);

            int pocketSize = 2 * (iterations + 1);
            var pocketDimension = new bool[lines[0].Length + pocketSize, 
                                           lines.Length + pocketSize,
                                           1 + pocketSize,
                                           1 + pocketSize];

            int w = 0;
            int z = 0;
            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[0].Length; x++)
                    pocketDimension[x + iterations + 1, y + iterations + 1, z + iterations + 1, w + iterations + 1] = lines[y][x] == Alive; // finna be a large array

            for (int i = 0; i < iterations; i++)
            {
                short[,,,] neighbours = CalcNeighbours_4D(pocketDimension);

                var nextIteration = new bool[
                                        pocketDimension.GetLength(0), 
                                        pocketDimension.GetLength(1),
                                        pocketDimension.GetLength(2),
                                        pocketDimension.GetLength(3)
                                    ];
                for (w = 1; w < pocketDimension.GetLength(3) - 1; w++)
                    for (z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                        for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                            for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                                nextIteration[x, y, z, w] = ApplyRules(pocketDimension[x, y, z, w], neighbours[x, y, z, w]);

                pocketDimension = nextIteration;
            }

            int sum = 0;
            for (w = 1; w < pocketDimension.GetLength(3) - 1; w++)
                for (z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                    for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                        for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                            if (pocketDimension[x, y, z, w])
                                sum++;

            return sum.ToString();
        }

        private short[,,,] CalcNeighbours_4D(bool[,,,] pocketDimension)
        {
            var neighbours = new short[
                                 pocketDimension.GetLength(0),
                                 pocketDimension.GetLength(1),
                                 pocketDimension.GetLength(2),
                                 pocketDimension.GetLength(3)
                             ];

            for (int w = 1; w < pocketDimension.GetLength(3) - 1; w++)
                for (int z = 1; z < pocketDimension.GetLength(2) - 1; z++)
                    for (int y = 1; y < pocketDimension.GetLength(1) - 1; y++)
                        for (int x = 1; x < pocketDimension.GetLength(0) - 1; x++)
                            if (pocketDimension[x, y, z, w])
                                for (int dw = -1; dw <= 1; dw++)
                                    for (int dz = -1; dz <= 1; dz++)
                                        for (int dy = -1; dy <= 1; dy++)
                                            for (int dx = -1; dx <= 1; dx++)
                                                if (dw != 0 || dz != 0 || dy != 0 || dx != 0)
                                                    neighbours[x + dx, y + dy, z + dz, w + dw]++;

            // children in africa could have eaten those loops

            return neighbours;
        }
    }
}
