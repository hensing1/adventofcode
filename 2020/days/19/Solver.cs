using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._19
{
    [ProblemDate(19)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            Dictionary<int, Node> nodes = ReadNodes(lines);

            return String.Empty;
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }

        private Dictionary<int, Node> ReadNodes(string[] lines)
        {
            var nodes = new Dictionary<int, Node>();

            foreach (string line in lines)
            {
                Match nodeIndexMatch = Regex.Match(line, @"^(\d+): ");
                if (nodeIndexMatch.Success)
                {
                    Node n = new Node
                    {
                        A1 = -1,
                        A2 = -1,
                        B1 = -1,
                        B2 = -1,
                        IsA = false,
                        IsB = false
                    };
                    Match numsMatch = Regex.Match(line, @": (\d+)( \d+)?( \| (\d+) (\d+))?");
                    if (numsMatch.Success)
                    {
                        n.A1 = int.Parse(numsMatch.Groups[1].Value);
                        if (int.TryParse(numsMatch.Groups[2].Value.Trim(), out int a2))
                            n.A2 = a2;
                        if (int.TryParse(numsMatch.Groups[4].Value, out int b1))
                            n.B1 = b1;
                        if (int.TryParse(numsMatch.Groups[5].Value, out int b2))
                            n.B2 = b2;
                    }
                    else
                    {
                        Match letterMatch = Regex.Match(line, "(\"a\"|\"b\")");
                        if (letterMatch.Success)
                        {
                            n.IsA = letterMatch.Value == "\"a\"";
                            n.IsB = !n.IsA;
                        }
                        else throw new InvalidInputException();
                    }
                    nodes.Add(int.Parse(nodeIndexMatch.Groups[1].Value), n);
                }
                else break;
            }

            return nodes;
        }
    }

    struct Node
    {
        public int A1, A2, B1, B2;
        public bool IsA, IsB;
    }
}
