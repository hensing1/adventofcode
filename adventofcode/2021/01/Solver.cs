using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._01
{
    [ProblemDate(2021, 1)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            int[] measurements = GetMeasurements(input);

            int count = 0;
            int prevMeasurement = measurements[0];

            for (int i = 1; i < measurements.Length; i++)
            {
                if (measurements[i] > prevMeasurement)
                    count++;

                prevMeasurement = measurements[i];
            }

            return count.ToString();
        }

        public string SolveSecond(string input)
        {
            int[] measurements = GetMeasurements(input);

            int count = 0;

            for (int i = 3; i < measurements.Length; i++)
            {
                int lastOfOldWindow = measurements[i - 3];
                int firstOfNewWindow = measurements[i];

                if (firstOfNewWindow > lastOfOldWindow)
                    count++;
            }

            return count.ToString();
        }

        private static int[] GetMeasurements(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var measurements = new int[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                measurements[i] = int.Parse(lines[i]);

            return measurements;
        }
    }
}
