using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using _2020.Utility;
using HenrysDevLib.Extensions;
using static _2020.Utility.Attributes;

namespace _2020.days._04
{
    [ProblemDate(4)]
    class Solver : ISolver
    {
        const string passportFieldRegex = @"(byr|iyr|eyr|hgt|hcl|ecl|pid|cid):(\S+)";
        readonly string[] requiredFields = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

        public string SolveFirst(string input)
        {
            var validPassportCount = 0;
            string[] lines = System.IO.File.ReadAllLines(input);
            Dictionary<string, string>[] passports = GetPassportsFromInput(lines);

            foreach (Dictionary<string, string> passport in passports)
                if (HasAllRequiredFields(passport))
                    validPassportCount++;

            return validPassportCount.ToString();
        }

        public string SolveSecond(string input)
        {
            var validPassportCount = 0;
            string[] lines = System.IO.File.ReadAllLines(input);
            Dictionary<string, string>[] passports = GetPassportsFromInput(lines);

            foreach (Dictionary<string, string> passport in passports)
            {
                bool valid =
                    HasAllRequiredFields(passport) &&
                    int.Parse(passport["byr"]).IsInRangeOf(1920, 2002) &&
                    int.Parse(passport["iyr"]).IsInRangeOf(2010, 2020) &&
                    int.Parse(passport["eyr"]).IsInRangeOf(2020, 2030) &&
                    IsValidHeight(passport["hgt"]) &&
                    Regex.IsMatch(passport["hcl"], @"^#([0-9]|[a-f]){6}$") &&
                    Regex.IsMatch(passport["ecl"], @"^(amb|blu|brn|gry|grn|hzl|oth)$") &&
                    Regex.IsMatch(passport["pid"], @"^[0-9]{9}$");  // all this shit worked first try and now I'm a bit proud

                if (valid)
                    validPassportCount++;
            }

            return validPassportCount.ToString();
        }

        private Dictionary<string, string>[] GetPassportsFromInput(string[] lines)
        {
            var passportList = new List<Dictionary<string, string>>();
            var currentPassport = new Dictionary<string, string>();

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == String.Empty)
                {
                    passportList.Add(currentPassport);
                    currentPassport = new Dictionary<string, string>();
                    continue;
                }

                MatchCollection passportFieldMatches = Regex.Matches(lines[i], passportFieldRegex);
                foreach (Match match in passportFieldMatches)
                {
                    currentPassport.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }

            return passportList.ToArray();
        }

        private bool HasAllRequiredFields(Dictionary<string, string> passport)
        {
            foreach (string field in requiredFields)
                if (!passport.ContainsKey(field))
                    return false;

            return true;
        }

        private bool IsValidHeight(string height)
        {
            Match match = Regex.Match(height, @"([0-9]+)(cm|in)");
            if (match.Success)
            {
                if (match.Groups[2].Value == "cm")
                    return int.Parse(match.Groups[1].Value).IsInRangeOf(150, 193);
                else
                    return int.Parse(match.Groups[1].Value).IsInRangeOf(59, 76);
            }
            return false;
        }
    }
}
