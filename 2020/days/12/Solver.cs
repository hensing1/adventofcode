using System;
using System.Text.RegularExpressions;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._12
{
    [ProblemDate(12)]
    class Solver : ISolver
    {
        enum Directions { N, E, S, W }
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int posX = 0;
            int posY = 0;
            var direction = Directions.E;
            foreach (string line in lines)
            {
                var actionMatch = Regex.Match(line, @"^([NESWLRF]{1})([0-9]+)$");
                if (actionMatch.Success)
                {
                    var action = char.Parse(actionMatch.Groups[1].Value);
                    var value  =  int.Parse(actionMatch.Groups[2].Value);

                    switch(action)
                    {
                        case 'N':
                            (posX, posY) = Move(Directions.N, value, posX, posY);
                            break;
                        case 'E':
                            (posX, posY) = Move(Directions.E, value, posX, posY);
                            break;
                        case 'S':
                            (posX, posY) = Move(Directions.S, value, posX, posY);
                            break;
                        case 'W':
                            (posX, posY) = Move(Directions.W, value, posX, posY);
                            break;
                        case 'L':
                            //if (value % 90 != 0)
                            //    throw new InvalidInputException();
                            for (int i = 0; i < value / 90; i++)
                                direction = TurnLeft(direction);
                            break;
                        case 'R':
                            //if (value % 90 != 0)
                            //    throw new InvalidInputException();
                            for (int i = 0; i < value / 90; i++)
                                direction = TurnRight(direction);
                            break;
                        case 'F':
                            (posX, posY) = Move(direction, value, posX, posY);
                            break;
                    }
                }
                else
                {
                    throw new InvalidInputException();
                }
            }
            return (Math.Abs(posX) + Math.Abs(posY)).ToString();
        }

        private (int, int) Move(Directions direction, int amount, int posX, int posY)
        {
            switch (direction)
            {
                case Directions.N:
                    return (posX, posY - amount);
                case Directions.E:
                    return (posX + amount, posY);
                case Directions.S:
                    return (posX, posY + amount);
                case Directions.W:
                    return (posX - amount, posY);
                default:
                    return (0, 0);
            }
        }

        /// <param name="initialD">Deja vu!</param>
        private Directions TurnLeft(Directions initialD)
        {
            switch (initialD)
            {
                case Directions.N:
                    return Directions.W;
                case Directions.E:
                case Directions.S:
                case Directions.W:
                    return --initialD;
            }
            throw new ArgumentException();
        }

        private Directions TurnRight(Directions initialD)
        {
            switch (initialD)
            {
                case Directions.N:
                case Directions.E:
                case Directions.S:
                    return ++initialD;
                case Directions.W:
                    return Directions.N;
            }
            throw new ArgumentException();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int shipX = 0;
            int shipY = 0;
            int waypointX = 10;
            int waypointY = -1;
            foreach (string line in lines)
            {
                var actionMatch = Regex.Match(line, @"^([NESWLRF]{1})([0-9]+)$");
                if (actionMatch.Success)
                {
                    var action = char.Parse(actionMatch.Groups[1].Value);
                    var value = int.Parse(actionMatch.Groups[2].Value);

                    switch (action)
                    {
                        case 'N':
                            (waypointX, waypointY) = Move(Directions.N, value, waypointX, waypointY);
                            break;
                        case 'E':
                            (waypointX, waypointY) = Move(Directions.E, value, waypointX, waypointY);
                            break;
                        case 'S':
                            (waypointX, waypointY) = Move(Directions.S, value, waypointX, waypointY);
                            break;
                        case 'W':
                            (waypointX, waypointY) = Move(Directions.W, value, waypointX, waypointY);
                            break;
                        case 'L':
                            for (int i = 0; i < value / 90; i++)
                                (waypointX, waypointY) = TurnWaypointLeft(waypointX, waypointY);
                            break;
                        case 'R':
                            for (int i = 0; i < value / 90; i++)
                                (waypointX, waypointY) = TurnWaypointRight(waypointX, waypointY);
                            break;
                        case 'F':
                            for (int i = 0; i < value; i++)
                            {
                                shipX += waypointX;
                                shipY += waypointY;
                            }
                            break;
                    }
                }
                else
                {
                    throw new InvalidInputException();
                }
            }
            return (Math.Abs(shipX) + Math.Abs(shipY)).ToString();
        }

        private (int, int) TurnWaypointLeft(int waypointX, int waypointY)
        {
            return (waypointY, -waypointX);
        }

        private (int, int) TurnWaypointRight(int waypointX, int waypointY)
        {
            return (-waypointY, waypointX);
        }
    }
}
