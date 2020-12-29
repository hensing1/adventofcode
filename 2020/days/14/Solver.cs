using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._14
{
    [ProblemDate(14)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] program = System.IO.File.ReadAllLines(input);
            Dictionary<long, long> memory = Execute(program, 1);

            long sum = 0;
            foreach (long value in memory.Values)
                sum += value;

            return sum.ToString();  // 1st try
        }

        public string SolveSecond(string input)
        {
            string[] program = System.IO.File.ReadAllLines(input);
            Dictionary<long, long> memory = Execute(program, 2);

            long sum = 0;
            foreach (long value in memory.Values)
                sum += value;

            return sum.ToString();
        }

        private Dictionary<long, long> Execute(string[] program, int version)
        {
            var memory = new Dictionary<long, long>();

            (long, long) mask = (0, 0);
            foreach (string line in program)
            {
                string keyword = Regex.Match(line, @"^[a-z]+").Value;
                switch (keyword)
                {
                    case "mask":
                        var maskString = Regex.Match(line, @"^.+ = (\w+)").Groups[1].Value;
                        mask = ReadMask(maskString);
                        break;
                    case "mem":
                        var index = long.Parse(Regex.Match(line, @"^[a-z]+\[(\d+)\]").Groups[1].Value);
                        var value = long.Parse(Regex.Match(line, @"^.+ = (\d+)").Groups[1].Value);

                        if (version == 1)
                            memory[index] = ApplyMask_V1(value, mask);

                        else if (version == 2)
                        {
                            var indices = ApplyMask_V2(index, mask);
                            foreach (var newIndex in indices)
                                memory[newIndex] = value;
                        }

                        break;
                    default:
                        throw new InvalidInputException();
                }
            }

            return memory;
        }

        private (long, long) ReadMask(string mask)
        {
            long ignore = 0;
            long overwrite = 0;

            foreach (char c in mask)
            {
                ignore <<= 1;
                overwrite <<= 1;
                switch (c)
                {
                    case 'X':
                        ignore += 1;
                        break;
                    case '0':
                        break;
                    case '1':
                        overwrite += 1;
                        break;
                    default:
                        throw new InvalidInputException();
                }
            }

            return (ignore, overwrite);
        }

        private long ApplyMask_V1(long value, (long, long) mask)
        {
            (long ignore, long overwrite) = mask;
            return (value & ignore) | overwrite;
        }

        private List<long> ApplyMask_V2(long value, (long, long) mask)
        {
            var indices = new LinkedList<long>();
            (long floating, long binary) = mask;
            value |= binary;
            value &= ~floating; // set all floating bits to zero
            indices.AddLast(value);

            uint iterator = 0;
            long floatingIterator = 0;
            while (floatingIterator < floating)
            {
                floatingIterator = Expand(++iterator, floating);
                indices.AddLast(value | floatingIterator);
            }

            return indices.ToList();
        }

        private long Expand(uint index, long floating)
        {
            long result = 0;
            for (int indexIterator = 0, floatingIterator = 0; (long)1 << floatingIterator <= floating; floatingIterator++)
            {
                if (((floating >> floatingIterator) & 1) == 1)
                {
                    long resultBit = (index >> indexIterator) & 1;
                    result |= resultBit << floatingIterator;
                    indexIterator++;
                }
            }
            return result;
        }
    }
}
