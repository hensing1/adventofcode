using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._7
{
    [ProblemDate(7)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            Dictionary<string, List<(int, string)>> bags = ParseLines(lines);
            List<string> containingBags = GetContainingBags("shiny gold", bags);

            return containingBags.Count.ToString();
        }

        public string SolveSecond(string input)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, List<(int, string)>> ParseLines(string[] lines)
        {
            var bags = new Dictionary<string, List<(int, string)>>();

            foreach (string line in lines)
            {
                MatchCollection bagMatches = Regex.Matches(line, @"(^|\d+ )(\w+ \w+) bags?");
                if (bagMatches.Count > 0)
                {
                    string newBag = bagMatches[0].Groups[2].Value;
                    bags.Add(newBag, new List<(int, string)>());
                    for (var i = 1; i < bagMatches.Count; i++)
                    {
                        var bagCount = int.Parse(bagMatches[i].Groups[1].Value);
                        var bagColor = bagMatches[i].Groups[2].Value;
                        bags[newBag].Add((bagCount, bagColor));
                    }
                }
            }

            return bags;
        }

        private List<string> GetContainingBags(string containedBag, in Dictionary<string, List<(int, string)>> bags)
        {
            var containingBags = new List<string>();

            foreach (string listBag in bags.Keys)
                if (bags[listBag].Any(e => e.Item2 == containedBag))
                    containingBags.Add(listBag);

            var originalBags = new List<string>(containingBags);
            foreach (string containingBag in originalBags)
                containingBags = containingBags.Union(GetContainingBags(containingBag, bags)).ToList();

            return containingBags;
        }
    }
}
