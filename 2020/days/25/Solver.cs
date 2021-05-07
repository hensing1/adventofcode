using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._25
{
    [ProblemDate(25)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            int cardKey = int.Parse(lines[0]);
            int doorKey = int.Parse(lines[1]);

            int cardLoopSize = DetermineLoopSize(cardKey);

            int value = 1;
            for (int i = 0; i < cardLoopSize; i++)
                value = Transform(value, doorKey);

            return value.ToString();
        }

        int DetermineLoopSize(int publicKey)
        {
            const int subjectNumber = 7;
            int value = 1;
            int loopSize = 0;

            while (value != publicKey)
            {
                value = Transform(value, subjectNumber);
                loopSize++;
            }

            return loopSize;
        }

        int Transform(int value, int subjectNumber)
        {
            long lVal = value;
            lVal *= subjectNumber;
            lVal %= 20201227;

            return (int)lVal;
        }

        public string SolveSecond(string input)
        {
            return "\n" +
                " **********************\n" +
                "*** Merry Christmas! ***\n" +
                " **********************";
        }
    }
}
