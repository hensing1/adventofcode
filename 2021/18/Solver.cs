using System;
using System.Linq;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._18
{
    [ProblemDate(2021, 18)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            ShellfishNumber[] shellfishNumbers = ParseInput(input);
            ShellfishNumber sum = shellfishNumbers.Aggregate((subtotal, next) => AddShellfishNumbers(subtotal, next));
            return sum.Magnitude().ToString();
        }

        public string SolveSecond(string input)
        {
            ShellfishNumber[] shellfishNumbers = ParseInput(input);
            int maximum = 0;
            for (int i = 0; i < shellfishNumbers.Length; i++)
                for (int j = 0; j < shellfishNumbers.Length; j++)
                    if (i != j)
                    {
                        int newMagnitude = AddShellfishNumbers(shellfishNumbers[i].DeepCopy(), shellfishNumbers[j].DeepCopy()).Magnitude();
                        if (newMagnitude > maximum)
                            maximum = newMagnitude;
                    }
            return maximum.ToString();
        }

        private ShellfishNumber[] ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);
            var shellfishNumbers = new ShellfishNumber[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                int index = 1;
                shellfishNumbers[i] = ParseNumber(lines[i], ref index, null, null);
            }

            return shellfishNumbers;
        }

        private ShellfishNumber ParseNumber(string numString, ref int index, ShellfishNumber parent, int? parentIndex)
        {
            var shellFishNumber = new ShellfishNumber()
            {
                Parent = parent,
                ParentIndex = parentIndex
            };

            object firstNum = ParseNumber_Sub(numString, ref index, shellFishNumber, 0);
            if (firstNum is int)
                shellFishNumber.RegularNumbers[0] = (int)firstNum;
            else
                shellFishNumber.NestedPairs[0] = (ShellfishNumber)firstNum;
            index++; // skip ','

            object secondNum = ParseNumber_Sub(numString, ref index, shellFishNumber, 1);
            if (secondNum is int)
                shellFishNumber.RegularNumbers[1] = (int)secondNum;
            else
                shellFishNumber.NestedPairs[1] = (ShellfishNumber)secondNum;
            index++; // skip ']'

            return shellFishNumber;
        }

        private object ParseNumber_Sub(string numString, ref int index, ShellfishNumber parent, int? parentIndex)
        {
            var currChar = numString[index];
            index++;
            if (currChar == '[')
                return ParseNumber(numString, ref index, parent, parentIndex);
            else //currChar is number
                return currChar - '0';
        }

        private ShellfishNumber AddShellfishNumbers(ShellfishNumber first, ShellfishNumber second)
        {
            var sum = new ShellfishNumber();
            first.Parent = sum;
            first.ParentIndex = 0;
            second.Parent = sum;
            second.ParentIndex = 1;

            sum.NestedPairs[0] = first;
            sum.NestedPairs[1] = second;
            sum.Reduce();
            return sum;
        }

        private class ShellfishNumber
        {
            public ShellfishNumber[] NestedPairs { get; set; }
            public int?[] RegularNumbers { get; set; }
            public ShellfishNumber Parent { get; set; }
            public int? ParentIndex { get; set; }

            public ShellfishNumber()
            {
                NestedPairs = new ShellfishNumber[2];
                RegularNumbers = new int?[2];
                Parent = null;
            }

            public ShellfishNumber DeepCopy()
            {
                var copy = new ShellfishNumber()
                {
                    RegularNumbers = (int?[])this.RegularNumbers.Clone(),
                    ParentIndex = this.ParentIndex
                };

                for (int i = 0; i < 2; i++)
                {
                    if (this.NestedPairs[i] != null)
                    {
                        copy.NestedPairs[i] = this.NestedPairs[i].DeepCopy();
                        copy.NestedPairs[i].Parent = copy;
                    }
                }

                return copy;
            }

            public int Magnitude()
            {
                int magnitudeLeft, magnitudeRight;

                if (this.RegularNumbers[0] != null)
                    magnitudeLeft = (int)this.RegularNumbers[0];
                else
                    magnitudeLeft = this.NestedPairs[0].Magnitude();

                if (this.RegularNumbers[1] != null)
                    magnitudeRight = (int)this.RegularNumbers[1];
                else
                    magnitudeRight = this.NestedPairs[1].Magnitude();

                return 3 * magnitudeLeft + 2 * magnitudeRight;
            }

            public void Reduce()
            {
                if (TryExplode(0) || TrySplit())
                    this.Reduce();
            }

            public bool TryExplode(int level)
            {
                if (level == 4)
                {
                    this.Explode();
                    return true;
                }
                for (int i = 0; i < 2; i++)
                    if (this.NestedPairs[i] != null && this.NestedPairs[i].TryExplode(level + 1))
                        return true;
                return false;
            }

            public bool TrySplit()
            {
                for (int i = 0; i < 2; i++)
                {
                    if (this.RegularNumbers[i] != null)
                    {
                        if (this.RegularNumbers[i] >= 10)
                        {
                            this.Split(i);
                            return true;
                        }
                    }
                    else
                    {
                        if (this.NestedPairs[i].TrySplit())
                            return true;
                    }
                }
                return false;
            }

            public void Explode()
            {
                int leftNum = (int)this.RegularNumbers[0];
                int rightNum = (int)this.RegularNumbers[1];

                this.PropagateLeft(leftNum);
                this.PropagateRight(rightNum);

                this.Parent.NestedPairs[(int)this.ParentIndex] = null;
                this.Parent.RegularNumbers[(int)this.ParentIndex] = 0;
            }

            public void Split(int index)
            {
                var newNum = new ShellfishNumber
                {
                    Parent = this,
                    ParentIndex = index,
                    RegularNumbers = new int?[]
                    {
                        this.RegularNumbers[index] / 2,
                        (int)Math.Ceiling((int)this.RegularNumbers[index] / 2d)
                    }
                };
                this.RegularNumbers[index] = null;
                this.NestedPairs[index] = newNum;
            }

            private void PropagateLeft(int num)
            {
                ShellfishNumber current = this.Parent;
                int currentIndex = (int)this.ParentIndex;

                while (currentIndex == 0) //while we are coming from the left side of the branch, there can't be a number further to the left
                {
                    if (current.Parent is null)
                        return;
                    currentIndex = (int)current.ParentIndex;
                    current = current.Parent;
                }

                //currentIndex is 1, meaning we are coming from the right branch    
                if (current.RegularNumbers[0] != null) // number to the left? we're done
                {
                    current.RegularNumbers[0] += num;
                    return;
                }

                current = current.NestedPairs[0]; // snailfish number to the left? find the rightmost number
                while (current.NestedPairs[1] != null)
                    current = current.NestedPairs[1];
                current.RegularNumbers[1] += num;
            }

            private void PropagateRight(int num)
            {
                ShellfishNumber current = this.Parent;
                int currentIndex = (int)this.ParentIndex;

                while (currentIndex == 1) //while we are coming from the right side of the branch, there can't be a number further to the right
                {
                    if (current.Parent is null)
                        return;
                    currentIndex = (int)current.ParentIndex;
                    current = current.Parent;
                }

                //currentIndex is 0, meaning we are coming from the left branch    
                if (current.RegularNumbers[1] != null) // number to the right? we're done
                {
                    current.RegularNumbers[1] += num;
                    return;
                }

                current = current.NestedPairs[1]; // snailfish number to the right? find the leftmost number
                while (current.NestedPairs[0] != null)
                    current = current.NestedPairs[0];
                current.RegularNumbers[0] += num;
            }
        }
    }
}
