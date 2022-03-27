using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._22
{
    [ProblemDate(2021, 22)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            State[] instructions = ParseInput(input);
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }

        private State[] ParseInput(string input, bool ignoreBigValues = true)
        {
            var instructions = new List<State>();
            string[] lines = System.IO.File.ReadAllLines(input);

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, 
                    @"^(?<activation>on|off) x=(?<xLower>-?\d+)..(?<xUpper>-?\d+),y=(?<yLower>-?\d+)..(?<yUpper>-?\d+),z=(?<zLower>-?\d+)..(?<zUpper>-?\d+)$");

                var newInstruction = new State
                {
                    X = (int.Parse(match.Groups["xLower"].Value), int.Parse(match.Groups["xUpper"].Value)),
                    Y = (int.Parse(match.Groups["yLower"].Value), int.Parse(match.Groups["yUpper"].Value)),
                    Z = (int.Parse(match.Groups["zLower"].Value), int.Parse(match.Groups["zUpper"].Value)),
                    On = match.Groups["activation"].Value == "on"
                };

                if (ignoreBigValues && Math.Abs(newInstruction.X.Lower) > 50)
                    return instructions.ToArray();
            }

            return instructions.ToArray();
        }

        private struct State
        {
            public (int Upper, int Lower) X { get; set; }
            public (int Upper, int Lower) Y { get; set; }
            public (int Upper, int Lower) Z { get; set; }
            public bool On { get; set; }
        }
    }
}
