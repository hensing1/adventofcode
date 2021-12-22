using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._06
{
    [ProblemDate(2021, 6)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[] initialFish = Array.ConvertAll(lines[0].Split(','), f => int.Parse(f));

            const int numDays = 80;

            return initialFish.Select(fish => 1 + FishDescendantsNaive(fish, numDays)).Sum().ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int[] initialFish = Array.ConvertAll(lines[0].Split(','), f => int.Parse(f));

            const int numDays = 256;

            long totalFish = 0;
            var cache = new Dictionary<int, long>();
            foreach (int fish in initialFish)
                totalFish += 1 + FishDescendantsAdvanced(fish, numDays, cache);

            return totalFish.ToString();
        }

        private int FishDescendantsNaive(int initialCounter, int numDays)
        {
            int daysLeft = numDays - initialCounter;
            if (daysLeft <= 0)
                return 0;

            int descendants = 0;
            while (daysLeft > 0)
            {
                descendants += 1 + FishDescendantsNaive(9, daysLeft);
                daysLeft -= 7;
            }
            return descendants;
        }

        private long FishDescendantsAdvanced(int initialCounter, int numDays, Dictionary<int, long> cache)
        {
            int daysLeft = numDays - initialCounter;
            if (daysLeft <= 0)
                return 0;

            long descendants = 0;
            while (daysLeft > 0)
            {
                if (cache.ContainsKey(daysLeft))
                    descendants += 1 + cache[daysLeft];
                else
                {
                    long newDescendants = FishDescendantsAdvanced(9, daysLeft, cache);
                    descendants += 1 + newDescendants;
                    cache[daysLeft] = newDescendants;
                }
                daysLeft -= 7;
            }
            return descendants;
        }
    }
}
