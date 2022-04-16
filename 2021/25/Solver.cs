using System.Text;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._25
{
    [ProblemDate(2021, 25)]
    class Solver : ISolver
    {
        const char FLOOR = '.';
        const char EAST_FACING = '>';
        const char SOUTH_FACING = 'v';
        public string SolveFirst(string input)
        {
            char[,] seaFloor = ParseInput(input);

            bool cucumbersHaveMoved;
            int count = 0;
            do
            {
                MoveCucumbers(seaFloor, out cucumbersHaveMoved);
                count++;
            } while (cucumbersHaveMoved);

            return count.ToString();
        }

        public string SolveSecond(string input)
        {
            char[,] seaFloor = ParseInput(input);

            bool cucumbersHaveMoved;
            int count = 0;
            do
            {
                MoveCucumbers(seaFloor, out cucumbersHaveMoved);
                count++;
            } while (cucumbersHaveMoved);

            return ToString(seaFloor);
        }

        private char[,] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var seaFloor = new char[lines[0].Length, lines.Length];

            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[0].Length; x++)
                    seaFloor[x, y] = lines[y][x];

            return seaFloor;
        }

        private void MoveCucumbers(char[,] seaFloor, out bool cucumbersHaveMoved)
        {
            cucumbersHaveMoved = false;
            MoveEast(seaFloor, ref cucumbersHaveMoved);
            MoveSouth(seaFloor, ref cucumbersHaveMoved);
        }

        private void MoveEast(char[,] seaFloor, ref bool cucumbersHaveMoved)
        {
            int xLength = seaFloor.GetLength(0);
            int yLength = seaFloor.GetLength(1);
            var originalSeaFloor = (char[,])seaFloor.Clone();

            for (int y = 0; y < yLength; y++)
                for (int x = 0; x < xLength; x++)
                    if (originalSeaFloor[x, y] == EAST_FACING)
                    {
                        int targetX = (x + 1) % xLength;
                        if (originalSeaFloor[targetX, y] == FLOOR)
                        {
                            seaFloor[x, y] = FLOOR;
                            seaFloor[targetX, y] = EAST_FACING;
                            cucumbersHaveMoved = true;
                        }
                    }
        }

        private void MoveSouth(char[,] seaFloor, ref bool cucumbersHaveMoved)
        {
            int xLength = seaFloor.GetLength(0);
            int yLength = seaFloor.GetLength(1);
            var originalSeaFloor = (char[,])seaFloor.Clone();

            for (int y = 0; y < yLength; y++)
                for (int x = 0; x < xLength; x++)
                    if (originalSeaFloor[x, y] == SOUTH_FACING)
                    {
                        int targetY = (y + 1) % yLength;
                        if (originalSeaFloor[x, targetY] == FLOOR)
                        {
                            seaFloor[x, y] = FLOOR;
                            seaFloor[x, targetY] = SOUTH_FACING;
                            cucumbersHaveMoved = true;
                        }
                    }
        }

        private string ToString(char[,] seaFloor)
        {
            var merryChristmas = "*MERRY*CHRISTMAS";
            var floor = new StringBuilder("\n");
            for (int y = 0; y < seaFloor.GetLength(1); y++)
            {
                for (int x = 0; x < seaFloor.GetLength(0); x++)
                {
                    if (seaFloor[x, y] == FLOOR)
                    {
                        int merryIndex = x % merryChristmas.Length;
                        floor.Append(merryChristmas[merryIndex]);
                        continue;
                    }
                    floor.Append(seaFloor[x, y]);
                }
                floor.Append('\n');
            }
            return floor.ToString();
        }
    }
}