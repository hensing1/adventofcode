using System;
using System.Text.RegularExpressions;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._08
{
    [ProblemDate(8)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] program = System.IO.File.ReadAllLines(input);
            Execute(program, out int accumulator);

            return accumulator.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] originalProgram = System.IO.File.ReadAllLines(input);
            string[] program;

            for (var i = 0; i < originalProgram.Length; i++)
            {
                Match instructionMatch = Regex.Match(originalProgram[i], @"^[a-z]{3}");
                if (instructionMatch.Success)
                {
                    switch (instructionMatch.Value)
                    {
                        case "nop":
                            program = (string[])originalProgram.Clone();
                            program[i] = program[i].Replace("nop", "jmp");
                            break;
                        case "jmp":
                            program = (string[])originalProgram.Clone();
                            program[i] = program[i].Replace("jmp", "nop");
                            break;
                        default:
                            continue;
                    }

                    if (Execute(program, out int accumulator))
                        return accumulator.ToString();
                }
            }

            return "No solution found";
        }

        private bool Execute(string[] program, out int accumulator)
        {
            var pointer = 0;
            var visited = new bool[program.Length];
            accumulator = 0;
            while (!visited[pointer])
            {
                visited[pointer] = true;
                Match codeMatch = Regex.Match(program[pointer], @"([a-z]{3}) ((\+|-)[0-9]+)");
                if (codeMatch.Success)
                {
                    var instruction = codeMatch.Groups[1].Value;
                    var arg = int.Parse(codeMatch.Groups[2].Value);
                    switch (instruction)
                    {
                        case "nop":
                            pointer++;
                            break;
                        case "acc":
                            accumulator += arg;
                            pointer++;
                            break;
                        case "jmp":
                            pointer += arg;
                            break;
                        default:
                            throw new Exception("Invalid argument in input file");
                    }

                    if (pointer >= program.Length)
                        return true; // Program terminates
                }
            }
            return false; // Program doesn't terminate
        }
    }
}
