using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using HenrysDevLib.Extensions;
using static _2020.Utility.Attributes;

namespace _2020.days._16
{
    [ProblemDate(16)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            List<(int Lower, int Upper)> ranges = GetCombinedRanges(lines);
            //int[] myTicket;
            List<int[]> nearbyTickets = GetNearbyTickets(lines);
            int ticketScanningErrorRate = 0;

            foreach (var ticket in nearbyTickets)
                foreach (var value in ticket)
                    if (!IsInRanges(value, ranges))
                        ticketScanningErrorRate += value;

            return ticketScanningErrorRate.ToString();
        }

        private List<(int Lower, int Upper)> GetCombinedRanges(string[] lines)
        {
            var ranges = new List<(int, int)>();
            const string rangePattern = @".+: ([0-9]+)-([0-9]+) or ([0-9]+)-([0-9]+)";

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, rangePattern);
                if (match.Success)
                {
                    AddRange(ranges, (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
                    AddRange(ranges, (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value)));
                }
                else break;
            }

            return ranges;
        }

        /**
         * Six scenraios:
         *      1. New range is not contained
         *      2. New range is contained within existing range
         *      3. New range contains one or more existing ranges
         *      4. New range joins two or more existing ranges into a single range
         *      5. New range extends lower bound of existing range beyond zero or more other existing ranges
         *      6. New range extends upper bound of existing range beyond zero or more other existing ranges
         *  
         *  ranges must be sorted and not overlap, else no worky
         *  
         *  edit: NO NO NO DUMB VERY DUMB
         *  YOU DUMB IDIOT
         */
        private void AddRange(List<(int Lower, int Upper)> ranges, (int Lower, int Upper) newRange)
        {
            /**
            bool lowerContained = ranges.Any(r => newRange.Lower.IsInRangeOf(r.Lower, r.Upper));
            bool upperContained = ranges.Any(r => newRange.Upper.IsInRangeOf(r.Lower, r.Upper));

            if (!lowerContained) // scenarios 1, 3, 5
            {
                int iNearestRange = ranges.FindIndex(r => r.Lower > newRange.Lower);

                if (!upperContained) // scenarios 1, 3
                {
                    if (iNearestRange == -1) // scenario 1
                        ranges.Append(newRange);
                    else if (newRange.Upper < ranges[iNearestRange].Lower) // also scenario 1
                        ranges.Insert(iNearestRange, newRange);
                    else // scenario 3
                    {
                        int iLastContainedRange = ranges.FindLastIndex(r => r.Upper < newRange.Upper);

                        ranges.RemoveRange(iNearestRange, iLastContainedRange - iNearestRange);
                        ranges[iNearestRange] = newRange;
                    }
                }
                else // scenario 5
                {
                    int iContainingRange = ranges.FindIndex(r => r.Lower < newRange.Upper);
                    newRange.Upper = ranges[iContainingRange].Upper;

                    ranges.RemoveRange(iNearestRange, iContainingRange - iNearestRange);
                    ranges[iNearestRange] = newRange;
                }
            }
            else // scenarios 2, 4, 6
            {
                int iContainingRangeLower = ranges.FindIndex(r => r.Upper >= newRange.Lower);

                if (!upperContained) // scenario 6
                {
                    int iLastContainedRange = ranges.FindLastIndex(r => r.Upper < newRange.Upper);

                    newRange.Lower = ranges[iContainingRangeLower].Lower;
                    ranges.RemoveRange(iContainingRangeLower, iLastContainedRange - iContainingRangeLower);
                    ranges[iContainingRangeLower] = newRange;
                }
                else // scenario 2 (noop), 4
                {
                    int iContainingRangeUpper = ranges.FindIndex(r => r.Lower <= newRange.Upper);
                    if (iContainingRangeLower != iContainingRangeUpper) // scenario 4
                    {
                        newRange.Lower = ranges[iContainingRangeLower].Lower;
                        newRange.Upper = ranges[iContainingRangeUpper].Upper;
                        ranges.RemoveRange(iContainingRangeLower, iContainingRangeUpper - iContainingRangeLower);
                        ranges[iContainingRangeLower] = newRange;
                    }
                }
            }
            
            // this entire approach astronomically stupid, let's try this again
             */

            var iDeleteFrom = ranges.Count;

            for (int i = 0; i < ranges.Count; i++) // determine lower bound
            {
                if (newRange.Lower <= ranges[i].Upper + 1)
                {
                    newRange.Lower = Math.Min(ranges[i].Lower, newRange.Lower);
                    iDeleteFrom = i;
                    break;
                }
            }

            if (iDeleteFrom == ranges.Count) // new lower > maximum?
            {
                ranges.Add(newRange);
                return;
            }

            for (int i = ranges.Count - 1; i >= 0; i--) // determine upper bound and replace range(s)
            {
                if (newRange.Upper >= ranges[i].Lower - 1)
                {
                    newRange.Upper = Math.Max(ranges[i].Upper, newRange.Upper);

                    ranges.RemoveRange(iDeleteFrom, (i - iDeleteFrom) + 1);
                    ranges.Insert(iDeleteFrom, newRange);
                    return;
                }
            }

            ranges.Insert(0, newRange); // new upper < minimum

            // much better
        }

        private List<int[]> GetNearbyTickets(string[] lines)
        {
            var nearbyTickets = new List<int[]>();
            bool bNearbyTickets = false;
            foreach (var line in lines)
            {
                if (!bNearbyTickets)
                {
                    if (line == "nearby tickets:")
                        bNearbyTickets = true;
                    continue;
                }

                nearbyTickets.Add(line.Split(',').Select(int.Parse).ToArray());
            }

            return nearbyTickets;
        }

        private bool IsInRanges(int value, List<(int Lower, int Upper)> ranges)
        {
            foreach ((int lower, int upper) in ranges)
            {
                if (value > upper)
                    return false;
                if (value >= lower)
                    return true;
            }
            return false;
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            var combinedRanges = GetCombinedRanges(lines);
            var ranges = GetRanges(lines);
            int[] myTicket = GetMyTicket(lines);
            int ticketLength = myTicket.Length;
            List<int[]> nearbyTickets = GetNearbyTickets(lines);
            var validTickets = new List<int[]>(nearbyTickets);
            var fittingFields = new Dictionary<string, bool[]>();

            // remove invalid tickets
            foreach (var ticket in nearbyTickets)
                foreach (var value in ticket)
                    if (!IsInRanges(value, combinedRanges))
                    {
                        validTickets.Remove(ticket);
                        break;
                    }

            // determine which fields fit which ranges
            foreach (var range in ranges)
            {
                fittingFields.Add(range.Key, new bool[ticketLength]);
                for (int field = 0; field < ticketLength; field++)
                {
                    for (int ticketNum = 0; ticketNum < validTickets.Count; ticketNum++)
                    {
                        if (validTickets[ticketNum][field].IsNotInRangeOf(range.Value.Item1.Lower, range.Value.Item1.Upper) &&
                            validTickets[ticketNum][field].IsNotInRangeOf(range.Value.Item2.Lower, range.Value.Item2.Upper))
                            goto skipField; //sorry
                    }
                    fittingFields[range.Key][field] = true;
                skipField:
                    continue;
                }
            }

            // do some sudoku
            var fields = FindAllFieldsByExclusion(fittingFields);

            // compute result
            long result = 1;
            foreach ((string fieldName, int fieldNum) in fields)
            {
                if (fieldName.StartsWith("departure"))
                    result *= myTicket[fieldNum];
            }

            return result.ToString();
        }

        private Dictionary<string, ((int Lower, int Upper), (int Lower, int Upper))> GetRanges(string[] lines)
        {
            var ranges = new Dictionary<string, ((int, int), (int, int))>();
            const string rangePattern = @"(.+): ([0-9]+)-([0-9]+) or ([0-9]+)-([0-9]+)";

            foreach (string line in lines)
            {
                Match match = Regex.Match(line, rangePattern);
                if (match.Success)
                {
                    var first  = (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                    var second = (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));
                    ranges.Add(match.Groups[1].Value, (first, second));
                }
                else break;
            }

            return ranges;
        }

        private int[] GetMyTicket(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
                if (lines[i] == "your ticket:")
                    return lines[++i].Split(',').Select(int.Parse).ToArray();
            throw new ArgumentException("No ticket provided");
        }

        private bool HasSingleMatch(bool[] matches, out int index)
        {
            index = -1;
            for (int i = 0; i < matches.Length; i++)
            {
                if (matches[i])
                {
                    if (index == -1)
                        index = i;
                    else
                        return false;
                }
            }
            return index != -1;
        }

        private List<(string, int)> FindAllFieldsByExclusion(Dictionary<string, bool[]> fittingFields)
        {
            var fields = new List<(string, int)>();

            Rec_FindAllFieldsByExclusion(fittingFields, fields);

            return fields;
        }

        private void Rec_FindAllFieldsByExclusion(Dictionary<string, bool[]> fittingFields, List<(string, int)> fields)
        {
            var fittingFieldsCopy = new Dictionary<string, bool[]>(fittingFields);

            foreach (var field in fittingFields)
            {
                string fieldName = field.Key;
                bool[] matches = field.Value;

                if (HasSingleMatch(matches, out int index))
                {
                    fields.Add((fieldName, index));

                    fittingFieldsCopy.Remove(fieldName);
                    if (fittingFieldsCopy.Count > 0)
                    {
                        foreach (var f in fittingFieldsCopy.Keys)
                            fittingFieldsCopy[f][index] = false;
                        Rec_FindAllFieldsByExclusion(fittingFieldsCopy, fields);
                    }
                    return;
                }
            }
        }
    }
}
