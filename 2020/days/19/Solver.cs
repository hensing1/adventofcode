using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._19
{
    [ProblemDate(19)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            Dictionary<int, Node> nodes = ReadNodes(lines);

            string[] messages = lines.SkipWhile(e => !(e.StartsWith("a") || e.StartsWith("b"))).ToArray();
            int sum = 0;
            foreach (var message in messages)
                if (IsValid(message, nodes))
                    sum++;

            return sum.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            Dictionary<int, Node> nodes = ReadNodes(lines);

            var newNode8 = new Node
            {
                FstAlt = new[] { 42 },
                SndAlt = new[] { 42, 8 },
                IsA = false,
                IsB = false
            };
            //nodes[8] = newNode8;
            var newNode11 = new Node
            {
                FstAlt = new[] { 42, 31 },
                SndAlt = new[] { 42, 11, 31 },
                IsA = false,
                IsB = false
            };
            nodes[11] = newNode11;

            string[] messages = lines.SkipWhile(e => !(e.StartsWith("a") || e.StartsWith("b"))).ToArray();
            int sum = 0;
            foreach (var message in messages)
                if (IsValid_2(message, nodes))
                    sum++;

            return sum.ToString();
        }

        private Dictionary<int, Node> ReadNodes(string[] lines)
        {
            var nodes = new Dictionary<int, Node>();

            foreach (string line in lines)
            {
                Match nodeIndexMatch = Regex.Match(line, @"^(\d+): ");
                if (nodeIndexMatch.Success)
                {
                    Node n = new Node
                    {
                        FstAlt = new int[0],
                        SndAlt = new int[0],
                        IsA = false,
                        IsB = false
                    };
                    var fstAltMatch = Regex.Match(line, @":((?: \d+)+)");
                    if (fstAltMatch.Success)
                    {
                        var numMatches = Regex.Matches(fstAltMatch.Value, @"\d+");
                        n.FstAlt = new int[numMatches.Count];
                        for (int i = 0; i < numMatches.Count; i++)
                            n.FstAlt[i] = int.Parse(numMatches[i].Value);

                        var sndAltMatch = Regex.Match(line, @"\|((?: \d+)+)");
                        if (sndAltMatch.Success)
                        {
                            numMatches = Regex.Matches(sndAltMatch.Value, @"\d+");
                            n.SndAlt = new int[numMatches.Count];
                            for (int i = 0; i < numMatches.Count; i++)
                                n.SndAlt[i] = int.Parse(numMatches[i].Value);
                        }
                    }
                    else
                    {
                        Match letterMatch = Regex.Match(line, "(\"a\"|\"b\")");
                        if (letterMatch.Success)
                        {
                            n.IsA = letterMatch.Value == "\"a\"";
                            n.IsB = !n.IsA;
                        }
                        else throw new InvalidInputException();
                    }
                    nodes.Add(int.Parse(nodeIndexMatch.Groups[1].Value), n);
                }
                else break;
            }

            return nodes;
        }

        private bool IsValid(string message, Dictionary<int, Node> nodes)
        {
            int nodeIndex = 0, msgIndex = 0;
            if (IsValid_Rec(message, nodes, nodeIndex, ref msgIndex))
                return msgIndex == message.Length - 1;
            return false;
        }

        private bool IsValid_2(string message, Dictionary<int, Node> nodes)
        {
            int nodeIndex = 0, msgIndex = 0;
            if (IsValid_Rec_2(message, nodes, nodeIndex, ref msgIndex))
                return msgIndex == message.Length - 1;
            return false;
        }

        private bool IsValid_Rec(string message, Dictionary<int, Node> nodes, int nodeIndex, ref int msgIndex)
        {
            if (nodes[nodeIndex].IsA)
                return message[msgIndex] == 'a';
            if (nodes[nodeIndex].IsB)
                return message[msgIndex] == 'b';

            int msgIndexBegin = msgIndex;
            bool fitsFstAlt;
            for (int i = 0; ; i++)
            {
                fitsFstAlt = IsValid_Rec(message, nodes, nodes[nodeIndex].FstAlt[i], ref msgIndex);
                if (fitsFstAlt && i < nodes[nodeIndex].FstAlt.Length - 1)
                    msgIndex++;
                else
                    break;
            }

            if (fitsFstAlt || nodes[nodeIndex].SndAlt.Length == 0)
                return fitsFstAlt;

            int msgIndexAfterFstAlt = msgIndex;
            msgIndex = msgIndexBegin;
            bool fitsSndAlt;
            for (int i = 0; ; i++)
            {
                fitsSndAlt = IsValid_Rec(message, nodes, nodes[nodeIndex].SndAlt[i], ref msgIndex);
                if (fitsSndAlt && i < nodes[nodeIndex].SndAlt.Length - 1)
                    msgIndex++;
                else
                    break;
            }
            if (!fitsSndAlt)
                msgIndex = msgIndexAfterFstAlt;

            return fitsSndAlt;
        }

        private bool IsValid_Rec_2(string message, Dictionary<int, Node> nodes, int nodeIndex, ref int msgIndex)
        {
            if (msgIndex >= message.Length)
                return false;
            if (nodes[nodeIndex].IsA)
                return message[msgIndex] == 'a';
            if (nodes[nodeIndex].IsB)
                return message[msgIndex] == 'b';

            int msgIndexBegin = msgIndex;
            bool fitsFstAlt;
            for (int i = 0; ; i++)
            {
                fitsFstAlt = IsValid_Rec_2(message, nodes, nodes[nodeIndex].FstAlt[i], ref msgIndex);
                if (fitsFstAlt && i < nodes[nodeIndex].FstAlt.Length - 1)
                    msgIndex++;
                else
                    break;
            }

            if (nodeIndex == 8) // apparently generalized solutions are very hard, so we're just gonna go with this dodgy approach
            {
                if (!fitsFstAlt)
                {
                    msgIndex = msgIndexBegin;
                    return false;
                }
                int lastValidMsgIndex = msgIndex;
                while (IsValid_Rec_2(message, nodes, 42, ref msgIndex))
                {
                    lastValidMsgIndex = msgIndex;
                    msgIndex++;
                }
                msgIndex = lastValidMsgIndex;
                return true;
            }

            if (fitsFstAlt || nodes[nodeIndex].SndAlt.Length == 0)
                return fitsFstAlt;

            if (nodeIndex == 11)
            {
                int rule42Count = 0;
                int lastValidMsgIndex = msgIndex;
                while (IsValid_Rec_2(message, nodes, 42, ref msgIndex))
                {
                    rule42Count++;
                    lastValidMsgIndex = msgIndex;
                    msgIndex++;
                }
                if (rule42Count == 0)
                {
                    msgIndex = msgIndexBegin;
                    return false;
                }
                msgIndex = ++lastValidMsgIndex;
                for (int i = 0; i < rule42Count; i++)
                {
                    if (!IsValid_Rec_2(message, nodes, 31, ref msgIndex))
                    {
                        msgIndex = msgIndexBegin;
                        return false;
                    }
                    msgIndex++;
                }
                msgIndex--;
                return true;
            }

            int msgIndexAfterFstAlt = msgIndex;
            msgIndex = msgIndexBegin;
            bool fitsSndAlt;
            for (int i = 0; ; i++)
            {
                fitsSndAlt = IsValid_Rec_2(message, nodes, nodes[nodeIndex].SndAlt[i], ref msgIndex);
                if (fitsSndAlt && i < nodes[nodeIndex].SndAlt.Length - 1)
                    msgIndex++;
                else
                    break;
            }
            if (!fitsSndAlt)
                msgIndex = msgIndexAfterFstAlt;

            return fitsSndAlt;
        }
    }

    struct Node
    {
        public int[] FstAlt, SndAlt;
        public bool IsA, IsB;
    }
}
