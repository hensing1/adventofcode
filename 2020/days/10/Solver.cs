using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._10
{
    [ProblemDate(10)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var numbers = new int[lines.Length];
            for (var i = 0; i < lines.Length; i++)
                numbers[i] = int.Parse(lines[i]);
            numbers = 
                numbers
                    .OrderBy(e => e)
                    .Prepend(0)
                    .Append(numbers.Max() + 3)
                    .ToArray();

            var oneSteps = 0;
            var threeSteps = 0;

            for (var i = 1; i < numbers.Length; i++)
            {
                var stepSize = numbers[i] - numbers[i - 1];
                if (stepSize == 1)
                    oneSteps++;
                else if (stepSize == 3)
                    threeSteps++;
                else if (stepSize != 2)
                    throw new Exception("No joltage adapter conversion possible");
            }

            return (oneSteps * threeSteps).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var numbers = new int[lines.Length];
            for (var i = 0; i < lines.Length; i++)
                numbers[i] = int.Parse(lines[i]);
            numbers =
                numbers
                    .OrderBy(e => e)
                    .Prepend(0)
                    .Append(numbers.Max() + 3)
                    .ToArray();

            var routesTo = new long[numbers.Length];
            routesTo[0] = 1;
            routesTo[1] = 1;
            routesTo[2] = numbers[2] - numbers[1] == 1 && numbers[1] - numbers[0] == 1 ? 2 : 1;

            for (var i = 3; i < numbers.Length; i++)
            {
                routesTo[i] = routesTo[i - 1];
                if (numbers[i] - numbers[i - 2] <= 3)
                {
                    routesTo[i] += routesTo[i - 2];
                    if (numbers[i] - numbers[i - 3] == 3)
                        routesTo[i] += routesTo[i - 3];
                }
            }
            return routesTo.Last().ToString();
        }
    }
}
