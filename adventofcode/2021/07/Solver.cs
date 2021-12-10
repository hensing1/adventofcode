using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._07
{
    [ProblemDate(2021, 7)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int[] numbers = Array.ConvertAll(lines[0].Split(','), e => int.Parse(e));
            numbers = numbers.OrderBy(num => num).ToArray(); // num num num
            var optimum = numbers[numbers.Length / 2]; // the median is the optimum; except if numbers.Length is even, we don't even need the median, because
                                                       // both entries on either side of the median will give exactly the same result (as well as every number
                                                       // in between these entries)
            return numbers.Sum(num => Math.Abs(num - optimum)).ToString();
        }

        public string SolveSecond(string input)
        {
            Func<int, int> fuelCostForDistance = (d) => d * (d + 1) / 2; // sum([1, 2, ..., n]) = n * (n+1) / 2
            Func<int, int, int> distance = (a, b) => Math.Abs(a - b);
            Func<int, int, int> fuelCost = (a, b) => fuelCostForDistance(distance(a, b));
            Func<IEnumerable<int>, int, int> totalFuelCost = (nums, position) => nums.Sum(num => fuelCost(num, position)); // what is this, Haskell?

            string[] lines = System.IO.File.ReadAllLines(input);

            int[] numbers = Array.ConvertAll(lines[0].Split(','), e => int.Parse(e));
            numbers = numbers.OrderBy(num => num).ToArray();


            // I was going to implement a classical hill climb algorithm (because I can't think of a purely mathematical solution),
            // but the problem is that it might get stuck on a wrong answer if two adjacent numbers are the same (also, if we start
            // in the middle, we might not know in which direction to go for the same reason).
            // So instead, I guess I'm still doing a hill climb (hill descent?) search, except I'm starting at the very beginning of
            // the list so I definitely know in which direction to go. It's still in O(n), so I guess I'll be fine.

            int guessIndex = 0;
            
            while (true)
            {
                int guess = numbers[guessIndex];
                int guessCost = totalFuelCost(numbers, guess);

                if (totalFuelCost(numbers, numbers[++guessIndex]) > guessCost) // we now know that the optimum is somewhere
                                                                               // between numbers[guessIndex] and numbers[guessIndex - 2]
                {
                    guess = numbers[guessIndex - 2];
                    do do do // do you want me to
                             // spend some time sitting next to you
                                guessCost = totalFuelCost(numbers, guess);
                            while (totalFuelCost(numbers, ++guess) <= guessCost); while (false); while (false);

                    return guessCost.ToString();
                }
            }
            // overoptimized? yes
            // un-elegant solution? yep
            // fun exercise? yea i guess
            // hotel? no, we stay at home. there's a pandemic outside
        }
    }
}
