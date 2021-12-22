using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._12
{
    [ProblemDate(2021, 12)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            Node[] nodes = ParseInput(input);
            throw new NotImplementedException();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            throw new NotImplementedException();
        }

        private Node[] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var connections = new (string, string)[lines.Length];
            var nodes = new List<Node>();
            for (int i = 0; i < lines.Length; i++)
            {
                var endpoints = lines[i].Split('-');
                connections[i] = (endpoints[0], endpoints[1]);

                foreach (string endpoint in endpoints)
                    if (!nodes.Exists(node => node.Name == endpoint)) throw new NotImplementedException();
                        //nodes.Add(new Node
                        //{
                        //    Name = endpoint
                        //})
            }

            throw new NotImplementedException();
        }

        struct Node
        {
            public string Name { get; init; }
            public bool IsStart;
            public bool IsEnd;
            public bool IsBig;
            public string[] Neighbors;


        }
    }
}
