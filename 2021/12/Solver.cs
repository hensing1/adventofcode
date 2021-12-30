using System.Collections.Generic;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._12
{
    [ProblemDate(2021, 12)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            MakeTree(input, out int numPaths);
            return numPaths.ToString();
        }

        public string SolveSecond(string input)
        {
            return MorePaths(input).ToString();
        }

        private Node MakeTree(string input, out int numPaths)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var connections = new Dictionary<string, List<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var endpoints = lines[i].Split('-');
                AddConnection(endpoints[0], endpoints[1], connections);
                AddConnection(endpoints[1], endpoints[0], connections);
            }

            numPaths = 0;
            var startNode = new Node() { Name = "start", Visited = new() };
            MakeSubTree(startNode, connections, ref numPaths);
            return startNode;
        }

        private void AddConnection(string a, string b, Dictionary<string, List<string>> connections)
        {
            if (b == "start")
                return;

            if (connections.TryGetValue(a, out List<string> aConns))
            {
                if (!aConns.Contains(b))
                    aConns.Add(b);
            }
            else
                connections.Add(a, new List<string>() { b });
        }

        private void MakeSubTree(Node root, Dictionary<string, List<string>> connections, ref int numPaths)
        {
            var newVisited = new List<string>( root.Visited );
            if (root.Name == root.Name.ToLower())
                newVisited.Add(root.Name);

            root.Descendants = new();
            foreach (string conn in connections[root.Name])
            {
                if (!root.Visited.Contains(conn))
                {
                    var newDescendant = new Node() { Name = conn, Visited = new(newVisited) };
                    root.Descendants.Add(newDescendant);
                    if (conn == "end")
                        numPaths++;
                    else
                        MakeSubTree(newDescendant, connections, ref numPaths);
                }
            }
        }

        struct Node
        {
            public string Name { get; init; }
            public List<Node> Descendants;

            public List<string> Visited;
        }


        #region Part Two

        private int MorePaths(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var connections = new Dictionary<string, List<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var endpoints = lines[i].Split('-');
                AddConnection(endpoints[0], endpoints[1], connections);
                AddConnection(endpoints[1], endpoints[0], connections);
            }

            var pathsUnderConsideration = new Stack<Path>(); // DFS faster than BFS because paths get removed faster

            var initialPath = new Path { CurrentCave = "start", Visited = new() };
            foreach (string cave in connections.Keys)
                if (cave == cave.ToLower() && cave != "start")
                    initialPath.Visited.Add(cave, 0);

            int pathCount = 0;
            pathsUnderConsideration.Push(initialPath);
            while (pathsUnderConsideration.Count > 0)
            {
                Path currentPath = pathsUnderConsideration.Pop();
                if (currentPath.Visited.ContainsKey(currentPath.CurrentCave))
                    if (++currentPath.Visited[currentPath.CurrentCave] == 2)
                        currentPath.SingleSmallCaveVisitedTwice = true;

                foreach (string neighbor in connections[currentPath.CurrentCave])
                {
                    if (neighbor == "end")
                        pathCount++;
                    else if (currentPath.Visited.ContainsKey(neighbor))
                    {
                        if (currentPath.Visited[neighbor] < 1 || !currentPath.SingleSmallCaveVisitedTwice)
                            pathsUnderConsideration.Push(new Path()
                            {
                                CurrentCave = neighbor,
                                Visited = new(currentPath.Visited),
                                SingleSmallCaveVisitedTwice = currentPath.SingleSmallCaveVisitedTwice
                            });
                    }
                    else
                        pathsUnderConsideration.Push(new Path()
                        {
                            CurrentCave = neighbor,
                            Visited = new(currentPath.Visited),
                            SingleSmallCaveVisitedTwice = currentPath.SingleSmallCaveVisitedTwice
                        });
                }
            }

            return pathCount;
        }

        struct Path
        {
            public Dictionary<string, int> Visited;
            public string CurrentCave;
            public bool SingleSmallCaveVisitedTwice;
        }

        #endregion
    }
}
