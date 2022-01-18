using System.Collections.Generic;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._15
{
    [ProblemDate(2021, 15)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            Node[,] cavern = ParseInput(input);

            int startX = 0, startY = 0;
            int goalX = cavern.GetLength(0) - 1, goalY = cavern.GetLength(1) - 1;

            int best = Dijkstra(cavern, startX, startY, goalX, goalY);
            return best.ToString();
        }

        public string SolveSecond(string input)
        {
            Node[,] cavern = ParseInput(input);

            cavern = ExpandCavern(cavern);

            int startX = 0, startY = 0;
            int goalX = cavern.GetLength(0) - 1, goalY = cavern.GetLength(1) - 1;

            int best = Dijkstra(cavern, startX, startY, goalX, goalY);
            return best.ToString();
        }

        private Node[,] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var cavern = new Node[lines[0].Length, lines.Length];

            for (int x = 0; x < lines[0].Length; x++)
                for (int y = 0; y < lines.Length; y++)
                    cavern[x, y] = new Node()
                    {
                        Coords = (x, y),
                        RiskLevel = lines[y][x] - '0',
                        Distance = int.MaxValue,
                        Discovered = false
                    };

            return cavern;
        }

        private int Dijkstra(Node[,] cavern, int startX, int startY, int goalX, int goalY)
        {
            cavern[startX, startY].Distance = 0;
            cavern[startX, startY].Discovered = true;
            var priorityQueue = new List<Node> { cavern[startX, startY] };

            while (true)
            {
                Node current = priorityQueue[0];
                priorityQueue.RemoveAt(0);

                foreach ((int x, int y) neighborCoords in GetNeighbors(current.Coords, cavern))
                {
                    Node neighbor = cavern[neighborCoords.x, neighborCoords.y];

                    if (neighborCoords == (goalX, goalY))
                        return current.Distance + neighbor.RiskLevel;

                    neighbor.Distance = current.Distance + neighbor.RiskLevel;
                    neighbor.Discovered = true;
                    InsertOdered(priorityQueue, neighbor);
                    cavern[neighborCoords.x, neighborCoords.y] = neighbor;
                }
            }
        }

        private IEnumerable<(int, int)> GetNeighbors((int, int) coords, Node[,] nodes)
        {
            (int x, int y) = coords;
            if (x > 0 && !nodes[x - 1, y].Discovered)
                yield return (x - 1, y);
            if (y > 0 && !nodes[x, y - 1].Discovered)
                yield return (x, y - 1);
            if (x < nodes.GetLength(0) - 1 && !nodes[x + 1, y].Discovered)
                yield return (x + 1, y);
            if (y < nodes.GetLength(1) - 1 && !nodes[x, y + 1].Discovered)
                yield return (x, y + 1);
        }

        private void InsertOdered(List<Node> priorityQueue, Node newNode)
        {
            for (int i = 0; i < priorityQueue.Count; i++)
            {
                if (newNode.Distance <= priorityQueue[i].Distance)
                {
                    priorityQueue.Insert(i, newNode);
                    return;
                }
            }
            priorityQueue.Add(newNode);
        }

        private struct Node
        {
            public (int x, int y) Coords { get; init; }
            public int RiskLevel { get; init; }
            public int Distance { get; set; }
            public bool Discovered { get; set; }
        }

        private Node[,] ExpandCavern(Node[,] cavern)
        {
            var expandedCavern = new Node[cavern.GetLength(0) * 5, cavern.GetLength(1) * 5];

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    for (int x = 0; x < cavern.GetLength(0); x++)
                        for (int y = 0; y < cavern.GetLength(1); y++)
                        {
                            int ex = i * cavern.GetLength(0) + x;
                            int ey = j * cavern.GetLength(1) + y;

                            int risk = ((cavern[x, y].RiskLevel + i + j - 1) % 9) + 1;

                            expandedCavern[ex, ey] = new Node()
                            {
                                Coords = (ex, ey),
                                RiskLevel = risk,
                                Discovered = false,
                                Distance = int.MaxValue
                            };
                        }

            return expandedCavern;
        }
    }
}
