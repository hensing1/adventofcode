using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._14
{
    [ProblemDate(2021, 14)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            const int numSteps = 10;
            string polymer = lines[0];

            Dictionary<string, string> rules = ParseRules(lines);

            for (int s = 0; s < numSteps; s++)
                polymer = ApplyStep(polymer, rules);

            int mostCommon = 0;
            int leastCommon = int.MaxValue;
            foreach (char element in polymer.Distinct())
            {
                int count = polymer.Count(c => c == element);
                if (count > mostCommon)
                    mostCommon = count;
                if (count < leastCommon)
                    leastCommon = count;
            }

            return (mostCommon - leastCommon).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            const int numSteps = 40;
            string template = lines[0];

            Dictionary<(char, char), char> rules = ParseRulesBetter(lines);

            long[] tally = DoTally(template, rules, numSteps);

            long result = tally.Max() - tally.Where(e => e != 0).Min();
            return result.ToString();
        }

        #region Part 1

        private Dictionary<string, string> ParseRules(string[] lines)
        {
            var rules = new Dictionary<string, string>();
            for (int i = 2; i < lines.Length; i++)
            {
                string pair = lines[i].Substring(0, 2);
                string result = string.Join("", new[] { lines[i][6], lines[i][1] }); //omitting the first letter so that I can just append the result
                rules.Add(pair, result);
            }
            return rules;
        }

        private string ApplyStep(string polymer, Dictionary<string, string> rules)
        {
            string newPolymer = polymer[0].ToString();
            for (int i = 0; i < polymer.Length - 1; i++)
            {
                string pair = polymer.Substring(i, 2);
                newPolymer += rules[pair];
            }
            return newPolymer;
        }

        #endregion

        #region Part 2

        private Dictionary<(char, char), char> ParseRulesBetter(string[] lines)
        {
            var rules = new Dictionary<(char, char), char>();
            for (int i = 2; i < lines.Length; i++)
            {
                (char, char) pair = (lines[i][0], lines[i][1]); //char char real smooth
                char result = lines[i][6];
                rules.Add(pair, result);
            }
            return rules;
        }

        private long[] DoTally(string template, Dictionary<(char, char), char> rules, int numSteps)
        {
            var memoizationDict = new Dictionary<(char, char, int), long[]>();
            var tally = new long[26];
            for (int i = 0; i < template.Length - 1; i++)
            {
                char first = template[i];
                char second = template[i + 1];

                tally[ToInt(first)]++;

                tally = MergeArray(tally, DoTallyRec(first, second, rules, numSteps, memoizationDict));
            }
            tally[ToInt(template[^1])]++; //count last character
            return tally;
        }

        private long[] DoTallyRec(char first, char second, Dictionary<(char, char), char> rules, int numSteps,
            Dictionary<(char, char, int), long[]> memoizationDict)
        {
            if (memoizationDict.TryGetValue((first, second, numSteps), out long[] result))
                return result;

            long[] tally;
            char inserted = rules[(first, second)];
            if (numSteps > 1)
            {
                tally = MergeArray(
                    DoTallyRec(first, inserted, rules, numSteps - 1, memoizationDict),
                    DoTallyRec(inserted, second, rules, numSteps - 1, memoizationDict));
            }
            else
                tally = new long[26];

            tally[ToInt(inserted)]++;
            memoizationDict.Add((first, second, numSteps), (long[])tally.Clone());
            return tally;
        }

        private int ToInt(char element)
        {
            return element - 'A';
        }

        private long[] MergeArray(long[] a, long[] b)
        {
            var merged = new long[a.Length];
            for (int i = 0; i < a.Length; i++)
                merged[i] = a[i] + b[i];
            return merged;
        }

        #endregion
    }
}
