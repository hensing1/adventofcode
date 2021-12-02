using System;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._02
{
    [ProblemDate(2021, 2)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int horizontalDist = 0;
            int depth = 0;

            foreach (string line in lines)
            {
                string[] command = line.Split(' ');
                string direction = command[0];
                int value = int.Parse(command[1]);

                switch(direction)
                {
                    case "forward":
                        horizontalDist += value;
                        break;
                    case "down":
                        depth += value;
                        break;
                    case "up":
                        depth -= value;
                        break;
                    default:
                        throw new InvalidInputException();
                }
            }

            int result = horizontalDist * depth;
            return result.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int horizontalDist = 0;
            int depth = 0;
            int aim = 0;

            foreach (string line in lines)
            {
                string[] command = line.Split(' ');
                string direction = command[0];
                int value = int.Parse(command[1]);

                switch (direction)
                {
                    case "forward":
                        horizontalDist += value;
                        depth += aim * value;
                        break;
                    case "down":
                        aim += value;
                        break;
                    case "up":
                        aim -= value;
                        break;
                    default:
                        throw new InvalidInputException();
                }
            }

            int result = horizontalDist * depth;
            return result.ToString();
        }
    }
}
