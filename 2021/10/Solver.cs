using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._10
{
    [ProblemDate(2021, 10)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int roundCount = 0;
            int squareCount = 0;
            int squigglyCount = 0;
            int pointyCount = 0;

            foreach (string line in lines)
            {
                if (IsCorrupted(line, out char incorrectBracket))
                {
                    switch(incorrectBracket)
                    {
                        case ')':
                            roundCount++;
                            break;
                        case ']':
                            squareCount++;
                            break;
                        case '}':
                            squigglyCount++;
                            break;
                        case '>':
                            pointyCount++;
                            break;
                    }
                }
            }

            int score =
                roundCount * 3 +
                squareCount * 57 +
                squigglyCount * 1197 +
                pointyCount * 25137;

            return score.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var completionStrings = new List<string>();
            foreach (string line in lines)
                if (!IsCorrupted(line, out char incorrectBracket))
                    completionStrings.Add(CompleteLine(line));

            var scores = new List<long>();
            foreach (string completionString in completionStrings)
            {
                long score = 0;
                foreach (char bracket in completionString)
                {
                    score *= 5;
                    switch(bracket)
                    {
                        case ')':
                            score += 1;
                            break;
                        case ']':
                            score += 2;
                            break;
                        case '}':
                            score += 3;
                            break;
                        case '>':
                            score += 4;
                            break;
                    }
                }
                scores.Add(score);
            }

            return scores.OrderBy(s => s).ToArray().GetValue(scores.Count / 2).ToString();
        }

        private bool IsCorrupted(string line, out char incorrectBracket)
        {
            var brackets = new Stack<char>();

            foreach (char bracket in line)
            {
                if ("([{<".Contains(bracket))
                    brackets.Push(bracket);
                else
                {
                    char lastBracket = brackets.Pop();
                    if (!IsMatch(lastBracket, bracket))
                    {
                        incorrectBracket = bracket;
                        return true;
                    }
                }
            }

            incorrectBracket = (char)0;
            return false;
        }

        private bool IsMatch(char left, char right)
        {
            return left == MatchOf(right);
        }

        private string CompleteLine(string line)
        {
            var completionString = string.Empty;
            var brackets = new Stack<char>();

            foreach (char bracket in line)
                if ("([{<".Contains(bracket))
                    brackets.Push(bracket);
                else
                    brackets.Pop();

            while (brackets.Count > 0)
                completionString += MatchOf(brackets.Pop());

            return completionString;
        }

        private char MatchOf(char bracket)
        {
            switch (bracket)
            {
                case '(':
                    return ')';
                case '[':
                    return ']';
                case '{':
                    return '}';
                case '<':
                    return '>';
                case ')':
                    return '(';
                case ']':
                    return '[';
                case '}':
                    return '{';
                case '>':
                    return '<';
                default:
                    return (char)0;
            }
        }
    }
}
