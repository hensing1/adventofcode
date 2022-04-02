using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

            List<State> activeCubes = ExecuteInstructions(instructions);

            return GetTotalCubeCount(activeCubes).ToString();
        }

        public string SolveSecond(string input)
        {
            State[] instructions = ParseInput(input, false);

            List<State> activeCubes = ExecuteInstructions(instructions);

            return GetTotalCubeCount(activeCubes).ToString();
        }

        private State[] ParseInput(string input, bool ignoreBigValues = true)
        {
            var instructions = new List<State>();
            string[] lines = System.IO.File.ReadAllLines(input);

            var regex = new Regex(@"^(?<activation>on|off) x=(?<xLower>-?\d+)..(?<xUpper>-?\d+),y=(?<yLower>-?\d+)..(?<yUpper>-?\d+),z=(?<zLower>-?\d+)..(?<zUpper>-?\d+)$");
            
            foreach (string line in lines)
            {
                var match = regex.Match(line);

                var newInstruction = new State
                {
                    X = (int.Parse(match.Groups["xLower"].Value), int.Parse(match.Groups["xUpper"].Value)),
                    Y = (int.Parse(match.Groups["yLower"].Value), int.Parse(match.Groups["yUpper"].Value)),
                    Z = (int.Parse(match.Groups["zLower"].Value), int.Parse(match.Groups["zUpper"].Value)),
                    On = match.Groups["activation"].Value == "on"
                };

                if (ignoreBigValues && Math.Abs(newInstruction.X.Lower) > 50)
                    return instructions.ToArray();

                instructions.Add(newInstruction);
            }

            return instructions.ToArray();
        }

        private List<State> ExecuteInstructions(State[] instructions)
        {
            var activeCubes = new List<State>();

            for (int i = 0; i < instructions.Length; i++)
            {
                var toBeRemoved = new List<State>();
                var toBeAdded = new List<State>();
                for (int j = 0; j < activeCubes.Count; j++)
                {
                    if (IsContained(activeCubes[j], instructions[i]))
                    {
                        toBeRemoved.Add(activeCubes[j]);
                    }
                    else if (HasOverlap(instructions[i], activeCubes[j]))
                    {
                        toBeRemoved.Add(activeCubes[j]);
                        toBeAdded.AddRange(SplitCuboid(activeCubes[j], instructions[i]));
                    }
                }
                if (instructions[i].On)
                    toBeAdded.Add(instructions[i]);

                activeCubes.AddRange(toBeAdded);
                for (int j = 0; j < toBeRemoved.Count; j++)
                    activeCubes.Remove(toBeRemoved[j]);

                //Console.Write("Active Cubes: ");
                //Console.WriteLine(GetTotalCubeCount(activeCubes));
            }

            return activeCubes;
        }

        private bool IsContained(State state1, State state2)
        {
            return
                IsInRange(state1.X.Lower, state2.X) && IsInRange(state1.X.Upper, state2.X) &&
                IsInRange(state1.Y.Lower, state2.Y) && IsInRange(state1.Y.Upper, state2.Y) &&
                IsInRange(state1.Z.Lower, state2.Z) && IsInRange(state1.Z.Upper, state2.Z);
        }

        private bool HasOverlap(State state1, State state2)
        {
            return
                RangesOverlap(state1.X, state2.X) &&
                RangesOverlap(state1.Y, state2.Y) &&
                RangesOverlap(state1.Z, state2.Z);
        }

        private bool RangesOverlap((int Lower, int Upper) range1, (int Lower, int Upper) range2) =>
                IsInRange(range1.Lower, range2) || IsInRange(range1.Upper, range2) || IsInRange(range2.Upper, range1);

        private List<State> SplitCuboid(State toBeSplit, State newState)
        {
            var fragments = new List<State>();
            // split first cube along all 6 faces of the new cube
            if (IsInRange(newState.Y.Upper, toBeSplit.Y, includeUpper: false)) // upper face
            {
                fragments.Add(new State
                {
                    X = toBeSplit.X,
                    Y = (newState.Y.Upper + 1, toBeSplit.Y.Upper),
                    Z = toBeSplit.Z
                });
            }
            if (IsInRange(newState.Y.Lower, toBeSplit.Y, includeLower: false)) // lower face
            {
                fragments.Add(new State
                {
                    X = toBeSplit.X,
                    Y = (toBeSplit.Y.Lower, newState.Y.Lower - 1),
                    Z = toBeSplit.Z
                });
            }
            if (IsInRange(newState.X.Lower, toBeSplit.X, includeLower: false)) // left face
            {
                fragments.Add(new State
                {
                    X = (toBeSplit.X.Lower, newState.X.Lower - 1),
                    Y = MinRange(toBeSplit.Y, newState.Y),
                    Z = toBeSplit.Z
                });
            }
            if (IsInRange(newState.X.Upper, toBeSplit.X, includeUpper: false)) // right face
            {
                fragments.Add(new State
                {
                    X = (newState.X.Upper + 1, toBeSplit.X.Upper),
                    Y = MinRange(toBeSplit.Y, newState.Y),
                    Z = toBeSplit.Z
                });
            }
            if (IsInRange(newState.Z.Lower, toBeSplit.Z, includeLower: false)) // front face
            {
                fragments.Add(new State
                {
                    X = MinRange(toBeSplit.X, newState.X),
                    Y = MinRange(toBeSplit.Y, newState.Y),
                    Z = (toBeSplit.Z.Lower, newState.Z.Lower - 1)
                });
            }
            if (IsInRange(newState.Z.Upper, toBeSplit.Z, includeUpper: false)) // back face
            {
                fragments.Add(new State
                {
                    X = MinRange(toBeSplit.X, newState.X),
                    Y = MinRange(toBeSplit.Y, newState.Y),
                    Z = (newState.Z.Upper + 1, toBeSplit.Z.Upper)
                });
            }

            return fragments;
        }

        private long GetTotalCubeCount(List<State> activeCubes)
        {
            long total = 0;
            foreach (State cuboid in activeCubes)
                total += NumCubes(cuboid);
            return total;
        }

        private long NumCubes(State cuboid)
        {
            return
                ((long)cuboid.X.Upper + 1 - cuboid.X.Lower) *
                ((long)cuboid.Y.Upper + 1 - cuboid.Y.Lower) *
                ((long)cuboid.Z.Upper + 1 - cuboid.Z.Lower);
        }

        private bool IsInRange(int val, (int Lower, int Upper) range, bool includeLower = true, bool includeUpper = true)
            =>  (includeLower ? val >= range.Lower : val > range.Lower) &&
                (includeUpper ? val <= range.Upper : val < range.Upper);

        private (int Lower, int Upper) MinRange((int Lower, int Upper) range1, (int Lower, int Upper) range2)
            => (Math.Max(range1.Lower, range2.Lower), Math.Min(range1.Upper, range2.Upper));

        private struct State
        {
            public (int Lower, int Upper) X { get; set; }
            public (int Lower, int Upper) Y { get; set; }
            public (int Lower, int Upper) Z { get; set; }
            public bool On { get; set; }
        }
    }
}
