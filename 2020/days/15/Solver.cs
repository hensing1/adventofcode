using System.Collections.Generic;
using System.Linq;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._15
{
    [ProblemDate(15)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            return CalcUntil(2020, input).ToString();
        }

        public string SolveSecond(string input)
        {
            return CalcUntil(30000000, input).ToString();
        }

        private int CalcUntil(int target, string input)
        {
            string line = System.IO.File.ReadAllLines(input)[0];
            int[] startingNumbers = line.Split(',').Select(e => int.Parse(e)).ToArray();

            var numDict = new Dictionary<int, int>(); // number -> last time spoken

            for (int i = 0; i < startingNumbers.Length; i++)
                numDict.Add(startingNumbers[i], i + 1);

            int lastNum = startingNumbers.Last();
            bool wasNovel = true;

            for (int turn = startingNumbers.Length + 1; turn <= target; turn++)
            {
                int newNum;
                if (wasNovel)
                    newNum = 0;
                else
                {
                    newNum = (turn - 1) - numDict[lastNum];
                    numDict[lastNum] = turn - 1;
                }

                wasNovel = !numDict.ContainsKey(newNum);
                if (wasNovel)
                    numDict.Add(newNum, turn);

                lastNum = newNum;
            }

            return lastNum;
        }
    }
}
