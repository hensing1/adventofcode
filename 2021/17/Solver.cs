using System;
using System.Text.RegularExpressions;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._17
{
    [ProblemDate(2021, 17)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            (_, _, int yMin, _) = ParseInput(input);
            return (yMin * (yMin + 1) / 2).ToString(); // when initial y velocity is abs(yMin)-1 upwards, it will reach yMin in one step when it is back on the
                                                       // same height as it started. (since the trajectory is symmetrical)
                                                       // the maximum height is vInitial * (vInitial + 1) / 2 = (-yMin - 1) * -yMin / 2 = yMin * (yMin + 1) / 2
        }

        public string SolveSecond(string input)
        {
            (int xMin, int xMax, int yMin, int yMax) target = ParseInput(input);
            int vxMin = (int)Math.Ceiling(Math.Sqrt(2 * target.xMin + 1) - 1); // vxMin * (vxMin + 1) / 2 = xMin, solved for vxMin, rounded up
            int vxMax = target.xMax;
            int vyMin = target.yMin;
            int vyMax = (-target.yMin) - 1; // see part 1

            int total = 0;
            for (int vx = vxMin; vx <= vxMax; vx++)
                for (int vy = vyMin; vy <= vyMax; vy++)
                    if (HitsTarget(vx, vy, target))
                        total++;

            return total.ToString();
        }

        private (int, int, int, int) ParseInput(string input)
        {
            string text = System.IO.File.ReadAllLines(input)[0];

            Match m = Regex.Match(text, @"^target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)");
            int xMin = int.Parse(m.Groups[1].Value);
            int xMax = int.Parse(m.Groups[2].Value);
            int yMin = int.Parse(m.Groups[3].Value);
            int yMax = int.Parse(m.Groups[4].Value);

            return (xMin, xMax, yMin, yMax); // happy xMax
        }

        private bool HitsTarget(int vx, int vy, (int, int, int, int) target)
        {
            int x = 0, y = 0;
            (int xMin, int xMax, int yMin, int yMax) = target;

            while (x <= xMax && y >= yMin)
            {
                MakeStep(ref x, ref y, ref vx, ref vy);
                if (xMin <= x && x <= xMax && yMin <= y && y <= yMax)
                    return true;
            }
            return false;
        }

        private void MakeStep(ref int x, ref int y, ref int vx, ref int vy)
        {
            x += vx;
            y += vy;
            if (vx > 0)
                vx--;
            vy--;
        }
    }
}
