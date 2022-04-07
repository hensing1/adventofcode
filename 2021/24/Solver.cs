using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._24
{
    [ProblemDate(2021, 24)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }

        private enum Operation
        {
            Inp, Add, Mul, Div, Mod, Eql
        }
    }
}