using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._06
{
    [ProblemDate(6)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input).Append(String.Empty).ToArray();

            var sum = 0;
            var currentGroup = String.Empty;

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == String.Empty)
                {
                    sum += currentGroup.Distinct().Count();
                    currentGroup = String.Empty;
                    continue;
                }

                currentGroup += lines[i];
            }

            return sum.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input).Append(String.Empty).ToArray();

            var sum = 0;
            var currentGroup = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == String.Empty)
                {
                    sum += Intersection(currentGroup.ToArray()).Length;
                    currentGroup = new List<string>();
                    continue;
                }

                currentGroup.Add(lines[i]);
            }

            return sum.ToString();
        }

        private string Intersection(string[] vals)
        {
            string result = vals[0];
            for (var i = 1; i < vals.Length; i++)
            {
                result = new String(result.Intersect(vals[i]).ToArray());
                if (result == String.Empty)
                    break;
            }
            return result;
        }
    }
}
