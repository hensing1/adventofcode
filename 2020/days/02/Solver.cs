using _2020.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using static _2020.Utility.Attributes;

namespace _2020.days._02
{
    [ProblemDate(2)]
    class Solver : ISolver
    {
        const string passwordRegex = @"^(\d+)-(\d+) ([a-z]): ([a-z]+)$";
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var validityCount = 0;
            foreach (string line in lines)
            {
                Match match = Regex.Match(line, passwordRegex);

                if (match.Success)
                {
                    var min = int.Parse(match.Groups[1].Value);
                    var max = int.Parse(match.Groups[2].Value);
                    var letter = char.Parse(match.Groups[3].Value);
                    string password = match.Groups[4].Value;

                    var letterCount = password.Count(c => c == letter);

                    if (letterCount >= min && letterCount <= max)
                        validityCount++;
                }
            }
            return validityCount.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var validityCount = 0;
            foreach (string line in lines)
            {
                Match match = Regex.Match(line, passwordRegex);

                if (match.Success)
                {
                    var firstIndex = int.Parse(match.Groups[1].Value);
                    var secondIndex = int.Parse(match.Groups[2].Value);
                    var letter = char.Parse(match.Groups[3].Value);
                    string password = match.Groups[4].Value;

                    firstIndex--;
                    secondIndex--;  // Toboggan Corporate Policies have no concept of "index zero"

                    if (password[firstIndex] == letter ^ password[secondIndex] == letter)
                        validityCount++;
                }
            }
            return validityCount.ToString();
        }
    }
}
