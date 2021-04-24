using System;
using System.Collections.Generic;
using System.Linq;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._23
{
    [ProblemDate(23)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            const int NUM_MOVES = 100;

            int[] aCups = ParseInput(input);

            var cupCircle = new CircularList<int>(aCups);

            for (int i = 0; i < NUM_MOVES; i++)
                MakeMove(cupCircle);

            return cupCircle.ToString(1);
        }

        public string SolveSecond(string input)
        {
            const int NUM_MOVES = 10000000;
            const int NUM_CUPS = 1000000;

            int[] aFirstCups = ParseInput(input);

            var aAllCups = new int[NUM_CUPS];

            aFirstCups.CopyTo(aAllCups, 0);
            for (int i = aFirstCups.Length; i < NUM_CUPS; i++)
                aAllCups[i] = i + 1;

            var cupCircle = new CircularList<int>(aAllCups);

            for (int i = 0; i < NUM_MOVES; i++)
                MakeMove(cupCircle);

            cupCircle.Select(1);
            long result = (long)cupCircle.Current.Next.Value * cupCircle.Current.Next.Next.Value;
            return result.ToString();
        }

        int[] ParseInput(string input)
        {
            string sCups = System.IO.File.ReadAllLines(input)[0];

            var output = new int[sCups.Length];
            for (int i = 0; i < sCups.Length; i++)
                output[i] = (int)char.GetNumericValue(sCups[i]);

            return output;
        }

        void MakeMove(CircularList<int> cupCircle)
        {
            Node<int> currentCup = cupCircle.Current;
            Node<int> grabbed = cupCircle.Take(3, 1);

            var grabbedValues = new int[] { grabbed.Value, grabbed.Next.Value, grabbed.Next.Next.Value };

            int targetLabel = SubtractWrapped(currentCup.Value, cupCircle.Min, cupCircle.Max);
            while (grabbedValues.Contains(targetLabel))
                targetLabel = SubtractWrapped(targetLabel, cupCircle.Min, cupCircle.Max);

            cupCircle.Select(targetLabel);

            cupCircle.Insert(grabbed);

            cupCircle.Current = currentCup.Next;
        }

        static int SubtractWrapped(int num, int min, int max)
        {
            int count = 1 + max - min;

            return ((num - 1 - min + count) % count) + min;
        }
    }
    class Node<T>
    {
        public Node<T> Previous;
        public Node<T> Next;
        public T Value;

        public Node(T val)
        {
            this.Value = val;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    class CircularList<T>
        //: ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IEnumerable, IDeserializationCallback, ISerializable
        // this would be the proper way to do it, but it's kind of overkill
    {

        public Node<T> Current;
        public T Min { get; private set; }
        public T Max { get; private set; }

        private Dictionary<T, Node<T>> dic;

        public CircularList(T[] values, int fillTo = 0)
        {
            int count = values.Length;

            // create and link all the nodes
            Current = new Node<T>(values[0]);

            var previousNode = Current;
            for (int i = 1; i < values.Length; i++)
            {
                var newNode = new Node<T>(values[i]);
                newNode.Previous = previousNode;
                previousNode.Next = newNode;

                previousNode = newNode;
            }

            Current.Previous = previousNode;
            previousNode.Next = Current;

            Min = values.Min();
            Max = values.Max();

            // create dictionary for O(1) lookup times instead of O(n)
            dic = new Dictionary<T, Node<T>>(count);

            var firstNode = Current;

            do
            {
                dic.Add(Current.Value, Current);
                MoveClockwise();
            } while (Current != firstNode);
        }

        public Node<T> Select(T value)
        {
            Current = dic[value];
            return Current;
        }

        public Node<T> MoveClockwise()
        {
            Current = Current.Next;
            return Current;
        }

        public Node<T> MoveCounterClockwise()
        {
            Current = Current.Previous;
            return Current;
        }

        public Node<T> Take(int count, int offset)
        {
            var startNode = Current;
            while (offset > 0)
            {
                startNode = startNode.Next;
                offset--;
            }

            var endNode = startNode;
            while (count > 1)
            {
                endNode = endNode.Next;
                count--;
            }

            startNode.Previous.Next = endNode.Next;
            endNode.Next.Previous = startNode.Previous;
            startNode.Previous = null;
            endNode.Next = null;

            return startNode;
        }

        public void Insert(Node<T> startNode)
        {
            var nextNode = Current.Next;
            Current.Next = startNode;
            startNode.Previous = Current;

            var endNode = startNode;
            while (endNode.Next != null)
                endNode = endNode.Next;

            endNode.Next = nextNode;
            nextNode.Previous = endNode;
        }

        public string ToString(T start)
        {
            while (!Current.Value.Equals(start))
                MoveClockwise();

            Node<T> first = Current;
            MoveClockwise();
            string output = String.Empty;
            do
            {
                output += Current.ToString();
                MoveClockwise();
            } while (Current != first);

            return output;
        }
    }
}
