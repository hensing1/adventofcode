using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._23
{
    [ProblemDate(2021, 23)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            State initialState = ParseInput(lines);

            var goalState = new State
            {
                Hallway = new char?[7],
                Rooms = new[]
                {
                    new char?[] { 'A', 'A' },
                    new char?[] { 'B', 'B' },
                    new char?[] { 'C', 'C' },
                    new char?[] { 'D', 'D' }
                }
            };

            State[] path = AStarSearch(initialState, goalState, out int cost);

            for (int i = 0; i < path.Length; i++)
                Console.WriteLine(path[i].ToString() + '\n');

            return cost.ToString();
        }

        public string SolveSecond(string input)
        {
            List<string> lines = System.IO.File.ReadAllLines(input).ToList();

            lines.InsertRange(3, new[]
            {
                "  #D#C#B#A#",
                "  #D#B#A#C#"
            });

            State initialState = ParseInput(lines.ToArray());

            var goalState = new State
            {
                Hallway = new char?[7],
                Rooms = new[]
                {
                    new char?[] { 'A', 'A', 'A', 'A' },
                    new char?[] { 'B', 'B', 'B', 'B' },
                    new char?[] { 'C', 'C', 'C', 'C' },
                    new char?[] { 'D', 'D', 'D', 'D' }
                }
            };

            State[] path = AStarSearch(initialState, goalState, out int cost);

            for (int i = 0; i < path.Length; i++)
                Console.WriteLine(path[i].ToString() + '\n');

            return cost.ToString();
        }

        private State ParseInput(string[] lines)
        {
            var roomRows = new List<char?[]>();

            for (int i = 2; i < lines.Length - 1; i++)
                roomRows.Add(new char?[] { lines[i][3], lines[i][5], lines[i][7], lines[i][9] });

            var rooms = new char?[4][];

            for (int room = 0; room < 4; room++)
            {
                rooms[room] = new char?[roomRows.Count];
                for (int row = 0; row < roomRows.Count; row++)
                    rooms[room][row] = roomRows[row][room];
            }

            return new State
            {
                Hallway = new char?[7],
                Rooms = rooms
            };
        }

        private State[] AStarSearch(State initialState, State goalState, out int fullPathCost)
        {
            var ancestorDict = new Dictionary<State, State>();
            var costDict = new Dictionary<State, int>() { { initialState, 0 } };

            var frontier = new PriorityQueue<State>();
            frontier.Enqueue(initialState, 0);

            while (frontier.Count > 0)
            {
                State currentState = frontier.Dequeue();

                if (currentState == goalState)
                {
                    fullPathCost = costDict[goalState];
                    return Backtrack(goalState, ancestorDict);
                }

                foreach ((State nextState, int deltaCost) in GetAvailableStates(currentState))
                {
                    int newCost = costDict[currentState] + deltaCost;

                    if (costDict.TryGetValue(nextState, out int oldCost))
                        if (oldCost <= newCost)
                            continue;

                    int heuristic = newCost + Terribleness(nextState);
                    frontier.Enqueue(nextState, heuristic);
                    costDict[nextState] = newCost;
                    ancestorDict[nextState] = currentState;
                }
            }

            throw new ArgumentException("No solution possible");
        }

        private State[] Backtrack(State goalState, Dictionary<State, State> ancestors)
        {
            var path = new List<State>() { goalState };
            var currentState = goalState;

            while(ancestors.ContainsKey(currentState))
            {
                currentState = ancestors[currentState];
                path.Insert(0, currentState);
            }

            return path.ToArray();
        }

        private int Terribleness(State state)
        {
            int terribleness = 0;
            var indexOfLowestUnordered = new int[4];
            for (int room = 0; room < 4; room++)
                indexOfLowestUnordered[room] = Array.FindLastIndex(state.Rooms[room], e => e is null || !IsAtHome((char)e, room));

            var numStillToBeOrdered = new int[4];
            indexOfLowestUnordered.CopyTo(numStillToBeOrdered, 0);

            for (int room = 0; room < 4; room++)
            {
                for (int i = indexOfLowestUnordered[room]; i >= 0; i--)
                {
                    if (state.Rooms[room][i] is null)
                        break;
                    var amphipod = (char)state.Rooms[room][i];
                    var targetRoom = amphipod - 'A';

                    var stepsToTake = 0;

                    // steps out of room
                    stepsToTake += i + 1;
                    // steps between rooms
                    stepsToTake += Math.Abs(room - targetRoom) * 2;
                    // steps into target room
                    stepsToTake += numStillToBeOrdered[targetRoom] + 1;

                    // 2 steps extra cause you need to step aside for the amphipod below you
                    if (room == targetRoom)
                        stepsToTake += 2;

                    terribleness += stepsToTake * stepCost[amphipod];

                    numStillToBeOrdered[targetRoom]--;
                }
            }

            for (int hallwayIndex = 0; hallwayIndex < 7; hallwayIndex++)
            {
                if (state.Hallway[hallwayIndex] is null)
                    continue;
                var amphipod = (char)state.Hallway[hallwayIndex];
                var targetRoom = amphipod - 'A';

                var stepsToTake = 0;

                stepsToTake += distance[targetRoom][hallwayIndex];
                stepsToTake += numStillToBeOrdered[targetRoom] + 1;

                terribleness += stepsToTake * stepCost[amphipod];

                numStillToBeOrdered[targetRoom]--;
            }

            return terribleness;
        }

        private IEnumerable<(State state, int cost)> GetAvailableStates(State baseState)
        {
            foreach (var state in GetStatesIntoRooms(baseState))
                yield return state;

            foreach (var state in GetStatesOutOfRooms(baseState))
                yield return state;
        }

        private IEnumerable<(State state, int cost)> GetStatesOutOfRooms(State baseState)
        {
            for (int room = 0; room < 4; room++)
            {
                int upperAmphipodIndex = Array.FindIndex(baseState.Rooms[room], e => e != null);
                if (upperAmphipodIndex == -1)
                    continue;

                char upperAmphipod = (char)baseState.Rooms[room][upperAmphipodIndex];

                bool needsToMove =
                    !IsAtHome(upperAmphipod, room) ||
                    (upperAmphipodIndex < baseState.Rooms[room].Length - 1 && 
                    baseState.Rooms[room].Skip(upperAmphipodIndex + 1).Any(e => !IsAtHome((char)e, room)));

                if (needsToMove)
                {
                    foreach ((int targetHallwayIndex, int steps) in GetMovesOutOfRoom(room, baseState.Hallway))
                    {
                        var newState = baseState.Copy();
                        newState.Hallway[targetHallwayIndex] = upperAmphipod;
                        newState.Rooms[room][upperAmphipodIndex] = null;

                        yield return (newState, (steps + upperAmphipodIndex) * stepCost[upperAmphipod]);
                    }
                }
            }
        }

        private bool IsAtHome(char amphipod, int room)
            => amphipod - 'A' == room;

        private IEnumerable<(int hallwayIndex, int steps)> GetMovesOutOfRoom(int roomIndex, char?[] hallway)
        {
            // check to the left
            for (int hallwayIndex = roomIndex + 1; hallwayIndex >= 0 && hallway[hallwayIndex] == null; hallwayIndex--)
                yield return (hallwayIndex, distance[roomIndex][hallwayIndex]);

            // check to the right
            for (int hallwayIndex = roomIndex + 2; hallwayIndex < hallway.Length && hallway[hallwayIndex] == null; hallwayIndex++)
                yield return (hallwayIndex, distance[roomIndex][hallwayIndex]);
        }

        private IEnumerable<(State state, int cost)> GetStatesIntoRooms(State baseState)
        {
            for (int hallwayIndex = 0; hallwayIndex < baseState.Hallway.Length; hallwayIndex++)
            {
                var hallwaySpot = baseState.Hallway[hallwayIndex];
                if (hallwaySpot is null)
                    continue;
                
                var amphipod = (char)hallwaySpot;
                int targetRoomIndex = amphipod - 'A';
                float targetHallwaySpot = targetRoomIndex + 1.5f;

                if (IsPathBlocked(baseState.Hallway, hallwayIndex, targetHallwaySpot))
                    continue;

                if (!CanEnterRoom(baseState.Rooms[targetRoomIndex], amphipod))
                    continue;

                int lowestFreeSpotInRoom = Array.LastIndexOf(baseState.Rooms[targetRoomIndex], null);

                var newState = baseState.Copy();
                newState.Rooms[targetRoomIndex][lowestFreeSpotInRoom] = amphipod;
                newState.Hallway[hallwayIndex] = null;
                yield return (newState, (distance[targetRoomIndex][hallwayIndex] + lowestFreeSpotInRoom) * stepCost[amphipod]);
            }
        }

        private bool IsPathBlocked(char?[] hallway, int hallwayIndex, float targetHallwaySpot)
        {
            if (hallwayIndex < targetHallwaySpot)
            {
                for (int i = hallwayIndex + 1; i < targetHallwaySpot; i++)
                    if (hallway[i] != null)
                        return true;
            }
            else
            {
                for (int i = hallwayIndex - 1; i > targetHallwaySpot; i--)
                    if (hallway[i] != null)
                        return true;
            }
            return false;
        }

        private bool CanEnterRoom(char?[] room, char amphipod)
            => !room.Any(e => e != null && e != amphipod);

        private static readonly int[][] distance = new[]
        {
            new[] { 3, 2, 2, 4, 6, 8, 9 },
            new[] { 5, 4, 2, 2, 4, 6, 7 },
            new[] { 7, 6, 4, 2, 2, 4, 5 },
            new[] { 9, 8, 6, 4, 2, 2, 3 }
        };

        private static readonly Dictionary<char, int> stepCost = new()
        {
            { 'A', 1 },
            { 'B', 10 },
            { 'C', 100 },
            { 'D', 1000 }
        };

        private class PriorityQueue<T>
        {
            private readonly List<(T item, int priority)> elements = new();

            public int Count
            {
                get { return elements.Count; }
            }

            public void Enqueue(T item, int priority)
            {
                elements.Add((item, priority));
            }

            public T Dequeue()
            {
                int bestIndex = 0;

                for (int i = 0; i < elements.Count; i++)
                    if (elements[i].priority < elements[bestIndex].priority)
                        bestIndex = i;

                T bestItem = elements[bestIndex].item;
                elements.RemoveAt(bestIndex);
                return bestItem;
            }
        }

        private struct State
        {
            public char?[] Hallway { get; init; }
            public char?[][] Rooms { get; init; }

            public override string ToString()
            {
                var hallway = string.Empty;
                for (int i = 0; i < this.Hallway.Length; i++)
                {
                    hallway += MakePrintable(this.Hallway[i]);
                    if (1 <= i && i <= 4)
                        hallway += '.';
                }

                var roomRows = new List<string>();
                for (int rowIndex = 0; rowIndex < this.Rooms[0].Length; rowIndex++)
                {
                    string row = "  #";
                    for (int room = 0; room < 4; room++)
                        row += MakePrintable(this.Rooms[room][rowIndex]) + "#";
                    roomRows.Add(row);
                }
                roomRows[0] = $"##{ roomRows[0].Trim() }##";

                var roomString = string.Join("\n", roomRows);

                return
                    "#############\n" +
                    $"#{hallway}#\n" +
                    roomString + "\n" +
                    "  #########";
            }

            public State Copy()
            {
                var copy = new State
                {
                    Hallway = (char?[])this.Hallway.Clone(),
                    Rooms = new char?[4][]
                };

                for (int i = 0; i < 4; i++)
                    copy.Rooms[i] = (char?[])this.Rooms[i].Clone();

                return copy;
            }

            private static char MakePrintable(char? c)
                => c is null ? '.' : (char)c;

            public override bool Equals(object obj)
            {
                return this.GetHashCode() == obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                int hashCode = 0;
                for (int i = 0; i < this.Hallway.Length; i++)
                    hashCode = HashCode.Combine(hashCode, this.Hallway[i]);

                for (int room = 0; room < 4; room++)
                    for (int i = 0; i < this.Rooms[0].Length; i++)
                        hashCode = HashCode.Combine(hashCode, this.Rooms[room][i]);

                return hashCode;
            }

            public static bool operator ==(State state1, State state2) => state1.Equals(state2);
            public static bool operator !=(State state1, State state2) => !state1.Equals(state2);
        }
    }
}
