using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._24
{
    /// <summary>
    /// This problem can be solved by hand.
    /// 
    /// The MONAD is divided into 14 sections, one for each digit of the model number. Each block is almost identical, with only three variables:
    /// - Z either gets divided by 1 or 26
    /// - There's a mistery Number m1 that gets applied to x
    /// - There's a mistery Number m2 that gets applied to y
    /// 
    /// It's important to note that only z carries over to the next block, all other variables are reset.
    /// 
    /// A block, if you break it down, executes the following logic:
    /// 
    /// x = z % 26 + m1;
    /// z /= oneOrTwentysix;
    /// if (x != digit)
    /// {
    ///     z *= 26;
    ///     z += digit + m2;
    /// }
    /// 
    /// Breaking it down further, there are two types of blocks:
    /// - "Increase" blocks, where:
    ///     - m1 is always >= 10, therefore x can never be equal to the digit (digit <= 9)
    ///     - z always gets div'd by 1
    ///     
    /// - "Decrease" blocks, where:
    ///     - m1 is always <= 0
    ///     - z always gets div'd by 26
    /// 
    /// Broken down even further:
    /// - Inc blocks take z = n * 26 + a and turn it into (n * 26 + a) * 26 + digit + m2 = m * 26 + b
    /// - Dec blocks take z = n * 26 + a and turn it into n, IF AND ONLY IF a + m1 == digit.
    /// 
    /// In summary, inc and dec blocks cancel each other out.
    /// 
    /// Now, here's the clou: there are exactly 7 inc and 7 dec blocks in MONAD. So, in order to have z be zero at the end,
    /// the condition in the dec block to decrease z has to be true in EVERY dec block.
    /// 
    /// Since z behaves sort of like a stack, with inc blocks pushing and dec blocks popping, each inc block must match with
    /// exactly one dec block. This allows us to apply constraints on pairs of digits (since each digit is processed by one 
    /// block) and you can run through the whole program by hand (see attached image).
    /// 
    /// </summary>
    [ProblemDate(2021, 24)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            var constraints = ParseInput(input);
            return FindMax(constraints);
        }

        public string SolveSecond(string input)
        {
            var constraints = ParseInput(input);
            return FindMin(constraints);
        }

        private (int index1, int index2, int delta)[] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var constraints = new List<(int index1, int index2, int delta)>(); // digit at index1 + delta = digit at index2
            var stack = new Stack<(int index, int misteryNumber)>();

            for (int i = 0; i < 14; i++)
            {
                var oneOrTwentysix = int.Parse(lines[i * 18 + 4].Split(' ')[2]);

                if (oneOrTwentysix == 1) // inc block
                {
                    var misteryNumber = int.Parse(lines[i * 18 + 15].Split(' ')[2]);
                    stack.Push((i, misteryNumber));
                }
                else // dec block
                {
                    var misteryNumber = int.Parse(lines[i * 18 + 5].Split(' ')[2]);
                    var topOfStack = stack.Pop();
                    int delta = topOfStack.misteryNumber + misteryNumber;

                    if (delta >= 0)
                        constraints.Add((topOfStack.index, i, delta));
                    else
                        constraints.Add((i, topOfStack.index, Math.Abs(delta)));
                }
            }

            return constraints.ToArray();
        }

        private string FindMax((int index1, int index2, int delta)[] constraints)
        {
            var modelNumArray = new int[14];

            for (int i = 0; i < 7; i++)
            {
                modelNumArray[constraints[i].index2] = 9;
                modelNumArray[constraints[i].index1] = 9 - constraints[i].delta;
            }

            return String.Join("", modelNumArray);
        }

        private string FindMin((int index1, int index2, int delta)[] constraints)
        {
            var modelNumArray = new int[14];

            for (int i = 0; i < 7; i++)
            {
                modelNumArray[constraints[i].index1] = 1;
                modelNumArray[constraints[i].index2] = 1 + constraints[i].delta;
            }

            return String.Join("", modelNumArray);
        }
    }
}