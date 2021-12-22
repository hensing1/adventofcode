using System;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2020._18
{
    [ProblemDate(2020, 18)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            long sum = 0;
            foreach (string line in lines)
                sum += ComputeExpression(line, LeftToRight);

            return sum.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            long sum = 0;
            foreach (string line in lines)
                sum += ComputeExpression(line, AdditionBeforeMultiplication);

            return sum.ToString();
        }

        private long ComputeExpression(string expression, Func<string, long> ruleSet)
        {
            if (long.TryParse(expression, out long num))
                return num;

            expression = RemoveParens(expression, ruleSet);

            return ruleSet(expression);
        }

        private string RemoveParens(string expression, Func<string, long> ruleSet)
        {
            while (expression.Contains('('))
            {
                int begin = -1;
                int parenCount = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '(')
                        if (begin == -1)
                            begin = i;
                        else
                            parenCount++;
                    else if (expression[i] == ')')
                        if (parenCount == 0)
                        {
                            string newExp = expression.Substring(begin + 1, i - begin - 1);
                            expression = expression.Replace('(' + newExp + ')', ComputeExpression(newExp, ruleSet).ToString());
                            break;
                        }
                        else
                            parenCount--;
                }
            }
            return expression;
        }

        private long LeftToRight(string expression)
        {
            var match = Regex.Match(expression, @"^\s*(\d+)\s*(.)\s*(\d+)");
            if (match.Success)
            {
                long newVal;
                var fstVal = long.Parse(match.Groups[1].Value);
                var op = char.Parse(match.Groups[2].Value);
                var sndVal = long.Parse(match.Groups[3].Value);

                switch (op)
                {
                    case '+':
                        newVal = fstVal + sndVal;
                        break;
                    case '*':
                        newVal = fstVal * sndVal;
                        break;
                    default:
                        throw new InvalidInputException();
                }

                // normal string.Replace() doesnt work, because it replaces every occurence
                // so doing "1 + 2 * 1 + 23".Replace("1 + 2", "3") would not result in "3 * 1 + 23" but "3 * 33"
                // therefore: https://stackoverflow.com/a/8809409
                var matchRegex = new Regex(Regex.Escape(match.Value));
                return ComputeExpression(matchRegex.Replace(expression, newVal.ToString(), 1), LeftToRight);
            }
            else throw new InvalidInputException();
        }

        private long AdditionBeforeMultiplication(string expression)
        {
            var match = Regex.Match(expression, @"(\d+)\s*\+\s*(\d+)");
            if (match.Success)
            {
                var fstVal = long.Parse(match.Groups[1].Value);
                var sndVal = long.Parse(match.Groups[2].Value);
                long newVal = fstVal + sndVal;

                var matchRegex = new Regex(Regex.Escape(match.Value));
                return ComputeExpression(matchRegex.Replace(expression, newVal.ToString(), 1), AdditionBeforeMultiplication);
            }

            match = Regex.Match(expression, @"(\d+)\s*\*\s*(\d+)");
            if (match.Success)
            {
                var fstVal = long.Parse(match.Groups[1].Value);
                var sndVal = long.Parse(match.Groups[2].Value);
                long newVal = fstVal * sndVal;

                var matchRegex = new Regex(Regex.Escape(match.Value));
                return ComputeExpression(matchRegex.Replace(expression, newVal.ToString(), 1), AdditionBeforeMultiplication);
            }
            else throw new InvalidInputException();
        }
    }
}
