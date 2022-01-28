using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._16
{
    [ProblemDate(2021, 16)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string binary = ParseInput(input);

            int index = 0;
            Packet rootPacket = ParsePacket(binary, ref index);

            return AddUpVersionNumbers(rootPacket).ToString();
        }

        public string SolveSecond(string input)
        {
            string binary = ParseInput(input);

            int index = 0;
            Packet rootPacket = ParsePacket(binary, ref index);

            return ComputeValue(rootPacket).ToString();
        }

        private string ParseInput(string input)
        {
            string hex = System.IO.File.ReadAllLines(input)[0];
            IEnumerable<string> nibbles = ConvertAllHexDigits(hex);
            return string.Join("", nibbles);
        }

        private IEnumerable<string> ConvertAllHexDigits(string hex)
        {
            foreach (char digit in hex)
            {
                int val = Convert.ToInt32(digit.ToString(), 16);
                yield return Convert.ToString(val, 2).PadLeft(4, '0');
            }
        }

        private Packet ParsePacket(string binary, ref int index)
        {
            int version = Convert.ToInt32(binary.Substring(index, 3), 2);
            int typeID = Convert.ToInt32(binary.Substring(index + 3, 3), 2);
            index += 6;

            Packet packet;

            if (typeID == 4)
                packet = ParseLiteralPacket(binary, version, ref index);
            else
                packet = ParseOperatorPacket(binary, version, typeID, ref index);
            return packet;
        }

        private LiteralPacket ParseLiteralPacket(string binary, int version, ref int index)
        {
            string binaryVal = string.Empty;
            bool finalPackage = false;
            while (!finalPackage)
            {
                if (binary[index] == '0')
                    finalPackage = true;
                binaryVal += binary.Substring(index + 1, 4);
                index += 5;
            }
            return new LiteralPacket()
            {
                Version = version,
                TypeID = 4,
                LiteralValue = Convert.ToInt64(binaryVal, 2)
            };
        }

        private OperatorPacket ParseOperatorPacket(string binary, int version, int typeID, ref int index)
        {
            int lengthTypeID = binary[index] - '0';
            index++;

            var subPackets = new List<Packet>();
            if (lengthTypeID == 0)
            {
                int lengthOfSubPackets = Convert.ToInt32(binary.Substring(index, 15), 2);
                index += 15;
                int targetIndex = index + lengthOfSubPackets;

                while (index < targetIndex)
                    subPackets.Add(ParsePacket(binary, ref index));
            }
            else
            {
                int numSubPackets = Convert.ToInt32(binary.Substring(index, 11), 2);
                index += 11;

                for (int i = 0; i < numSubPackets; i++)
                    subPackets.Add(ParsePacket(binary, ref index));
            }

            return new OperatorPacket()
            {
                Version = version,
                TypeID = typeID,
                ContainedPackets = subPackets.ToArray()
            };
        }

        private int AddUpVersionNumbers(Packet packet)
        {
            int value = packet.Version;

            if (packet is OperatorPacket op)
                foreach (Packet subPacket in op.ContainedPackets)
                    value += AddUpVersionNumbers(subPacket);

            return value;
        }

        private long ComputeValue(Packet packet)
        {
            if (packet is LiteralPacket lp)
                return lp.LiteralValue;

            Packet[] subPackets = ((OperatorPacket)packet).ContainedPackets;

            switch (packet.TypeID)
            {
                case 0:
                    return subPackets.Sum(packet => ComputeValue(packet));
                case 1:
                    long product = 1;
                    foreach (Packet subPacket in subPackets)
                        product *= ComputeValue(subPacket);
                    return product;
                case 2:
                    return subPackets.Min(packet => ComputeValue(packet));
                case 3:
                    return subPackets.Max(packet => ComputeValue(packet));
                case 5:
                    return ComputeValue(subPackets[0]) > ComputeValue(subPackets[1]) ? 1 : 0;
                case 6:
                    return ComputeValue(subPackets[0]) < ComputeValue(subPackets[1]) ? 1 : 0;
                case 7:
                    return ComputeValue(subPackets[0]) == ComputeValue(subPackets[1]) ? 1 : 0;
                default:
                    throw new ArgumentException("There is something profoundly wrong because a three digit binary number was somehow greater than 7");
            }
        }

        private class Packet
        {
            public int Version { get; init; }
            public int TypeID { get; init; }
        }

        private class LiteralPacket : Packet
        {
            public long LiteralValue { get; init; }
        }
        
        private class OperatorPacket : Packet
        {
            public Packet[] ContainedPackets { get; init; }
        }
    }
}
