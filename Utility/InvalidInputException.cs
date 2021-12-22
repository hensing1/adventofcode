using System;

namespace adventofcode.Utility
{
    class InvalidInputException : Exception
    {
        public InvalidInputException() : base("Invalid input file.")
        {
        }
    }
}
