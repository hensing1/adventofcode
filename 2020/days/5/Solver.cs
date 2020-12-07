using System;
using System.Linq;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._5
{
    [ProblemDate(5)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var maxID = 0;

            foreach (string boardingPass in lines)
                maxID = Math.Max(GetID(boardingPass), maxID);

            return maxID.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var ids = new int[lines.Length];

            for (var i = 0; i < lines.Length; i++)
                ids[i] = GetID(lines[i]);

            ids = ids.OrderBy(e => e).ToArray();

            int lastID = ids[0];

            for (var i = 1; i < ids.Length; i++)
            {
                if (ids[i] == lastID + 2)
                    return (lastID + 1).ToString();
                lastID = ids[i];
            }

            return "not found";
        }

        /// <summary>
        /// Converts Binary Space Partitioning into a number.
        /// </summary>
        private int BSPtoInt(string bsp, char zero, char one)
        {
            var output = 0;

            for (var i = 0; i < bsp.Length; i++)
            {
                output <<= 1;

                if      (bsp[i] == zero) { }
                else if (bsp[i] == one)  { output++; }
                else                     { throw new ArgumentException("BSP contains illegal character"); }
            }
            return output;
        }

        private int GetID(string boardingPass)
        {
            int row = BSPtoInt(boardingPass.Substring(0, 7), 'F', 'B');
            int column = BSPtoInt(boardingPass.Substring(7), 'L', 'R');

            return row * 8 + column;
        }
    }
}
