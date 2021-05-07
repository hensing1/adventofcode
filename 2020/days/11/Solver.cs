using System;
using System.Collections.Generic;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._11
{
    [ProblemDate(11)]
    class Solver : ISolver
    {
        const char Empty = 'L';
        const char Occupied = '#';
        const char Floor = '.';
        readonly bool? BoolEmpty = false;
        readonly bool? BoolOccupied = true;
        readonly bool? BoolFloor = null;
        readonly Dictionary<(int startX, int startY), ((int x, int y) straightDir, (int x, int y) diagDir)> navDict = 
            new Dictionary<(int, int), ((int, int), (int, int))>()
            {
                { (0, 0), (( 1,  0), (-1,  1)) },
                { (1, 0), (( 0,  1), (-1, -1)) },
                { (1, 1), ((-1,  0), ( 1, -1)) },
                { (0, 1), (( 0, -1), ( 1,  1)) }
            };

        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            bool?[,] newSeats = ConvertToBoolArray(lines);
            bool?[,] oldSeats;

            do
            {
                oldSeats = newSeats;
                newSeats = Iterate_V1(oldSeats);
            } while (!oldSeats.ContentEquals(newSeats));

            return CountOccupied(newSeats).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            bool?[,] newSeats = ConvertToBoolArray(lines);
            bool?[,] oldSeats;

            do
            {
                oldSeats = newSeats;
                newSeats = Iterate_V2(oldSeats);
            } while (!oldSeats.ContentEquals(newSeats));

            return CountOccupied(newSeats).ToString();
        }

        private int CountOccupied(bool?[,] seats)
        {
            var result = 0;
            for (var y = 0; y < seats.GetLength(1); y++)
                for (var x = 0; x < seats.GetLength(0); x++)
                    if (seats[x, y] == BoolOccupied)
                        result++;
            return result;
        }

        private bool?[,] Iterate_V1(bool?[,] seats)
        {
            var result = (bool?[,])seats.Clone();
            for (var y = 0; y < seats.GetLength(1); y++)
            {
                for (var x = 0; x < seats.GetLength(0); x++)
                {
                    if (seats[x, y] == BoolFloor)
                        continue;

                    var neighbours = 0;
                    for (var dy = Math.Max(0, y - 1); dy < Math.Min(seats.GetLength(1), y + 2); dy++)
                        for (var dx = Math.Max(0, x - 1); dx < Math.Min(seats.GetLength(0), x + 2); dx++)
                            if (seats[dx, dy] == BoolOccupied && (dy != y || dx != x))
                                neighbours++;

                    if (neighbours == 0)
                        result[x, y] = BoolOccupied;
                    else if (neighbours >= 4)
                        result[x, y] = BoolEmpty;
                }
            }

            return result;
        }

        private bool?[,] Iterate_V2(bool?[,] seats)
        {
            var result = (bool?[,])seats.Clone();
            int[,] neighbours = GetNeighbours(seats);

            for (int y = 0; y < seats.GetLength(1); y++)
            {
                for (int x = 0; x < seats.GetLength(0); x++)
                {
                    if (seats[x, y] == BoolFloor)
                        continue;

                    if (neighbours[x, y] == 0)
                        result[x, y] = BoolOccupied;
                    else if (neighbours[x, y] >= 5)
                        result[x, y] = BoolEmpty;
                }
            }

            return result;
        }

        private int[,] GetNeighbours(bool?[,] seats)
        {
            int[,] neighbours = new int[seats.GetLength(0), seats.GetLength(1)];
            int numDiags = seats.GetLength(0) + seats.GetLength(1) - 1;
            var blocked = false;

            //foreach (var nav in navDict)
            //{
            //    int startX = seats.GetLength(0) * nav.Key.startX;
            //    int startY = seats.GetLength(1) * nav.Key.startY;

            //    //straight
            //    int stepDim = Math.Abs(nav.Key.startX - nav.Key.startY); // 0 if horizontal, 1 if vertical
            //    for (int row = 0; row < seats.GetLength(1 - stepDim); row++)
            //    {
            //        int rowStartX = startX + row * stepDim;
            //        int rowStartY = startY + row * (1 - stepDim);
            //        for (int step = 0; step < seats.GetLength(stepDim); step++)
            //        {
            //            int x = rowStartX + (step * nav.Value.straightDir.x);
            //            int y = rowStartY + (step * nav.Value.straightDir.y);

            //            if (seats[x, y] != BoolFloor)
            //                blocked = (bool)seats[x, y];
            //            if (blocked)
            //                neighbours[x, y]++;
            //        }
            //    }

            //    //diagonal
            //    for (int diag = 1; diag < numDiags; diag++)
            //    {
            //        int diagStartX = startX + 
            //    }
            //}
            // fuck this. I spent too much time on this. This is not worth the effort. Fuck style points. There is no style to this madness.

            // I'm doing this the stupid way now

            for (int y = 0; y < seats.GetLength(1); y++)
            {
                blocked = false;
                for (int x = 0; x < seats.GetLength(0); x++)
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                blocked = false;
                for (int x = seats.GetLength(0) - 1; x >= 0; x--)
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
            }

            for (int x = 0; x < seats.GetLength(0); x++)
            {
                blocked = false;
                for (int y = 0; y < seats.GetLength(1); y++)
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                blocked = false;
                for (int y = seats.GetLength(1) - 1; y >= 0; y--)
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
            }

            for (int diag = 1; diag < numDiags; diag++)
            {
                int startX = Math.Min(diag, seats.GetLength(0) - 1);
                int startY = Math.Max(diag - (seats.GetLength(0) - 1), 0);

                int x = startX, y = startY;
                blocked = false;
                while (x >= 0 && y < seats.GetLength(1))
                {
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                    x--;
                    y++;
                }
                x++;
                y--; // Told you. It's dumb
                blocked = false;
                while (y >= 0 && x < seats.GetLength(0))
                {
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                    x++;
                    y--;
                }
            }

            for (int diag = 1; diag < numDiags; diag++)
            {
                int startX = Math.Max(seats.GetLength(0) - 1 - diag, 0);
                int startY = Math.Max(diag - (seats.GetLength(0) - 1), 0);

                int x = startX, y = startY;
                blocked = false;
                while (x < seats.GetLength(0) && y < seats.GetLength(1))
                {
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                    x++;
                    y++;
                }
                x--;
                y--;
                blocked = false;
                while (x >= 0 && y >= 0)
                {
                    neighbours[x, y] += IsBlocked(seats[x, y], ref blocked);
                    x--;
                    y--;
                }
            }

            // this better be correct. I don't want to debug this garbage
            // edit: Wow! It worked?!

            return neighbours;
        }

        private int IsBlocked(bool? seat, ref bool blocked)  // even this function sucks ass
        {
            if (seat == BoolFloor)
                return 0;
            int val = blocked ? 1 : 0;
            blocked = (bool)seat;
            return val;
        }

        private bool?[,] ConvertToBoolArray(string[] lines)
        {
            var seats = new bool?[lines[0].Length, lines.Length];
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[0].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case Occupied:
                            seats[x, y] = BoolOccupied;
                            break;
                        case Empty:
                            seats[x, y] = BoolEmpty;
                            break;
                        case Floor:
                            seats[x, y] = BoolFloor;
                            break;
                        default:
                            throw new Exception("Invalid input file.");
                    }
                }
            }
            return seats;
        }
    }
}
