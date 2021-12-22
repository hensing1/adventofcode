using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._08
{
    [ProblemDate(2021, 8)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var outputVals = new List<string>();

            foreach (string line in lines)
                outputVals.AddRange(GetOutputPatterns(line));

            var uniqueLengths = new[] { 2, 3, 4, 7 };
            return outputVals.Count(val => uniqueLengths.Contains(val.Length)).ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            long outputSum = 0;

            foreach (string entry in lines)
            {
                string[] signalPatterns = GetSignalPatterns(entry);
                string[] outputPatterns = GetOutputPatterns(entry);
                outputSum += SolveEntry(signalPatterns, outputPatterns);
            }

            return outputSum.ToString();
        }

        private string[] GetSignalPatterns(string entry)
        {
            return Regex.Match(entry, @".+?(?=\s\|)").Value.Split(); // "match everything until " |" (+? = lazy selector, (?=...) = positive lookahead)
        }

        private string[] GetOutputPatterns(string entry)
        {
            return Regex.Match(entry, @"(?<=\|\s).*").Value.Split(); // match everything after "| " ((?<=...) = positive lookbehind)
        }

        private int SolveEntry(string[] signalPatterns, string[] outputPatterns)
        {
            var letterMapping = new Dictionary<char, char>();

            char bSegment = FindSegments(signalPatterns, 6).Single(); // the 'b' segment should occur exactly 6 times
            letterMapping.Add(bSegment, 'b');

            char eSegment = FindSegments(signalPatterns, 4).Single(); // etc
            letterMapping.Add(eSegment, 'e');

            char fSegment = FindSegments(signalPatterns, 9).Single();
            letterMapping.Add(fSegment, 'f');


            // finding the a and c segments, both of which occur 8 times
            var one = signalPatterns.Single(pattern => pattern.Length == 2);
            var seven = signalPatterns.Single(pattern => pattern.Length == 3);

            char aSegment = seven.Single(segment => !one.Contains(segment)); // aSegment = segmentsOf(7) - segmentsOf(1)
            char cSegment = FindSegments(signalPatterns, 8).Except(new[] { aSegment }).Single(); 

            letterMapping.Add(aSegment, 'a');
            letterMapping.Add(cSegment, 'c');


            // finding the d and g segments, both of which occur 7 times
            var four = signalPatterns.Single(pattern => pattern.Length == 4);

            char[] dgSegments = FindSegments(signalPatterns, 7);
            char dSegment = dgSegments.Single(segment => four.Contains(segment));
            char gSegment = dgSegments.Except(new[] { dSegment }).Single();

            letterMapping.Add(dSegment, 'd');
            letterMapping.Add(gSegment, 'g');


            // decoding the output patterns into a four digit number
            string outputValue = string.Empty;
            foreach (string encodedPattern in outputPatterns)
            {
                string decodedPattern = string.Empty;

                foreach (char encodedSegment in encodedPattern)
                    decodedPattern += letterMapping[encodedSegment];

                decodedPattern = string.Concat(decodedPattern.OrderBy(segment => segment));
                outputValue += patternMapping[decodedPattern];
            }

            return int.Parse(outputValue);
        }

        /// <summary>
        /// Finds the letter(s) in the entry with the specified number of ocurrences 
        /// </summary>
        private char[] FindSegments(string[] entry, int numOccurrences)
        {
            return "abcdefg".Where(letter => entry.Count(pattern => pattern.Contains(letter)) == numOccurrences).ToArray();
        }

        readonly Dictionary<string, char> patternMapping = new Dictionary<string, char>()
        {
            { "abcefg", '0' },
            { "cf", '1' },
            { "acdeg", '2' },
            { "acdfg", '3' },
            { "bcdf", '4' },
            { "abdfg", '5' },
            { "abdefg", '6' },
            { "acf", '7' },
            { "abcdefg", '8' },
            { "abcdfg", '9' },
        };
    }
}
