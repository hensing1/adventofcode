using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventofcode.Utility
{
    public interface ISolver
    {
        string SolveFirst(string input);
        string SolveSecond(string input);
    }
}
