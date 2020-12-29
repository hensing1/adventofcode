using System.Collections.Generic;
using System.Linq;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._13
{
    [ProblemDate(13)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var timestamp = int.Parse(lines[0]);
            int[] buses = lines[1].Split(',').Where(e => e != "x").Select(e => int.Parse(e)).ToArray();

            //var minbus = buses.Select(e => (int)(Math.Ceiling((double)timestamp / e) * e) - timestamp).Min();

            (int ID, int wait) minBus = (0, int.MaxValue);

            foreach (int bus in buses)
            {
                if (bus % timestamp == 0)
                    return 0.ToString();

                var wait = bus - (timestamp % bus);
                minBus = wait < minBus.wait ? (bus, wait) : minBus;
            }

            return (minBus.ID * minBus.wait).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            string[] busPlan = lines[1].Split(',');
            var busList = new List<(int planIndex, int ID)>();

            for (int i = 0; i < busPlan.Length; i++)
                if (busPlan[i] != "x")
                    busList.Add((i, int.Parse(busPlan[i])));

            var buses = busList.ToArray();

            long timestamp = 0, increment = buses[0].ID;

            for (int i = 0; i < buses.Length; i++)
            {
                timestamp = GetNextMatchup(timestamp, increment, buses[i].ID, buses[i].planIndex); // first matchup of the first i buses

                if (i != buses.Length - 1)
                    increment = GetNextMatchup(timestamp + increment, increment, buses[i].ID, buses[i].planIndex) - timestamp;
            }

            return timestamp.ToString();
        }

        private long GetNextMatchup(long start, long first, int second, int offset)
        {
            for (long i = start; ; i += first)
                if ((i + offset) % second == 0)
                    return i;
        }
    }
}
