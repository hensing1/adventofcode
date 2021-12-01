using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._09
{
    [ProblemDate(2020, 9)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var numbers = new long[lines.Length];
            for (var i = 0; i < lines.Length; i++)
                numbers[i] = long.Parse(lines[i]);

            long invalidNum = FindInvalid(numbers);

            return invalidNum == -1 ? "not found" : invalidNum.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var numbers = new long[lines.Length];
            for (var i = 0; i < lines.Length; i++)
                numbers[i] = long.Parse(lines[i]);

            long invalidNum = FindInvalid(numbers);

            var firstIndex = 0;
            var numRange = new LinkedList<long>();
            numRange.AddLast(numbers[0]);
            long sum = numbers[0];

            for (var i = 1; i < numbers.Length; )
            {
                if (sum < invalidNum)
                {
                    sum += numbers[i];
                    numRange.AddLast(numbers[i]);
                    i++;
                }
                else if (sum > invalidNum)
                {
                    sum -= numbers[firstIndex];
                    numRange.RemoveFirst();
                    firstIndex++;
                }
                else
                {
                    return (numRange.Min() + numRange.Max()).ToString();
                }
            }

            return "not found.";
        }

        private long FindInvalid(long[] numbers, int preambleLength = 25)
        {
            for (var i = 25; i < numbers.Length; i++)
                if (!IsValid(numbers, i, preambleLength))
                    return numbers[i];
            return -1;
        }

        private bool IsValid(long[] numbers, int index, int preambleLength)
        {
            if (index < preambleLength)
                return true;

            for (var i = index - preambleLength; i < index - 1; i++)
                for (var j = i + 1; j < index; j++)
                    if (numbers[i] + numbers[j] == numbers[index])
                        return true;

            return false;
        }
    }
}
