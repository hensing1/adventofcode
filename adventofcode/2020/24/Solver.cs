using System;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._24
{
    [ProblemDate(2020, 24)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            bool[,] tileBitmap = ParseInput(lines);

            return CountBlackTiles(tileBitmap).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            const int ITERATIONS = 100;

            bool[,] bitmap = ConwaysTilesOfLife(ParseInput(lines), ITERATIONS);

            return CountBlackTiles(bitmap).ToString();
        }

        bool[,] ParseInput(string[] lines)
        {
            var tileCoords = new (int, int)[lines.Length];
            int xMin = int.MaxValue, xMax = int.MinValue, yMin = int.MaxValue, yMax = int.MinValue;

            for (int i = 0; i < lines.Length; i++)
            {
                int x = 0, y = 0;

                for (int j = 0; j < lines[i].Length; j++)
                {
                    switch (lines[i][j])
                    {
                        case 'e':
                            x++;
                            break;
                        case 'w':
                            x--;
                            break;
                        case 's':
                            y++;
                            if (lines[i][++j] == 'w')
                                x--;
                            break;
                        case 'n':
                            y--;
                            if (lines[i][++j] == 'e')
                                x++;
                            break;
                        default:
                            throw new InvalidInputException();
                    }
                }

                tileCoords[i] = (x, y);
                xMin = Math.Min(x, xMin);
                xMax = Math.Max(x, xMax);
                yMin = Math.Min(y, yMin);
                yMax = Math.Max(y, yMax);
            }

            var tileBitmap = new bool[xMax - xMin + 1, yMax - yMin + 1];

            foreach ((int x, int y) in tileCoords)
                tileBitmap[x - xMin, y - yMin] ^= true;

            return tileBitmap;
        }

        bool[,] ConwaysTilesOfLife(bool[,] tileBitmap, int iterations)
        {
            //Console.WriteLine($"Initial: {CountBlackTiles(tileBitmap)}");
            for (int i = 0; i < iterations; i++)
            {
                tileBitmap = Iterate(tileBitmap);
                //Console.WriteLine($"Day {i + 1}: {CountBlackTiles(tileBitmap)}");
            }

            return tileBitmap;
        }

        bool[,] Iterate(bool[,] bitmap)
        {
            short[,] neighbors = CalcNeighbors(bitmap);

            var nextIteration = new bool[neighbors.GetLength(0), neighbors.GetLength(1)];

            for (int x = 0; x < bitmap.GetLength(0); x++)
                for (int y = 0; y < bitmap.GetLength(1); y++)
                    nextIteration[x + 1, y + 1] = ApplyRules(bitmap[x, y], neighbors[x + 1, y + 1]);

            //boundaries
            var maxYIndex = neighbors.GetLength(1) - 1;
            for (int x = 0; x < neighbors.GetLength(0); x++)
            {
                nextIteration[x, 0] = ApplyRules(false, neighbors[x, 0]);
                nextIteration[x, maxYIndex] = ApplyRules(false, neighbors[x, maxYIndex]);
            }
            var maxXIndex = neighbors.GetLength(0) - 1;
            for (int y = 1; y < neighbors.GetLength(1) - 1; y++)
            {
                nextIteration[0, y] = ApplyRules(false, neighbors[0, y]);
                nextIteration[maxXIndex, y] = ApplyRules(false, neighbors[maxXIndex, y]);
            }

            // one might trim the bitmap, but I think it's just faster to assume the array gets bigger in every direction at every iteration
            // also, I can't be bothered to optimize an algorithm that takes 200ms on an i3

            return nextIteration;
        }

        short[,] CalcNeighbors(bool[,] bitmap)
        {
            var neighbors = new short[bitmap.GetLength(0) + 2, bitmap.GetLength(1) + 2];

            for (int y = 1; y < neighbors.GetLength(1) - 1; y++)
                for (int x = 1; x < neighbors.GetLength(0) - 1; x++)
                    if (bitmap[x - 1, y - 1])
                    {
                        neighbors[x, y - 1]++;
                        neighbors[x + 1, y - 1]++;
                        neighbors[x - 1, y]++;
                        neighbors[x + 1, y]++;
                        neighbors[x - 1, y + 1]++;
                        neighbors[x, y + 1]++;
                    }

            return neighbors;
        }

        bool ApplyRules(bool tile, short neighbors)
        {
            if (tile) // "black"
                return !(neighbors == 0 || neighbors > 2);
            else
                return neighbors == 2;
        }

        int CountBlackTiles(bool[,] bitmap)
        {
            int blackTileCount = 0;
            for (int x = 0; x < bitmap.GetLength(0); x++)
                for (int y = 0; y < bitmap.GetLength(1); y++)
                    blackTileCount += bitmap[x, y] ? 1 : 0;

            return blackTileCount;
        }
    }
}
