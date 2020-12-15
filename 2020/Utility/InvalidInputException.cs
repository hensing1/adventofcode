using System;

namespace _2020.Utility
{
    class InvalidInputException : Exception
    {
        public InvalidInputException() : base("Invalid input file.")
        {
        }
    }
}
