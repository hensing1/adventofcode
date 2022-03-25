using System;
using System.Collections.Generic;
using adventofcode.Utility;
using static adventofcode.Utility.Attributes;

namespace adventofcode._2021._21
{
    [ProblemDate(2021, 21)]
    class Solver : ISolver
    {
        private (int Player1, int Player2) ParseInput(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            int player1 = lines[0][^1] - '0';
            int player2 = lines[1][^1] - '0';

            return (player1, player2);
        }

        private int Move(int pos, int spaces) => (pos + spaces - 1) % 10 + 1;

        #region Part1

        public string SolveFirst(string input)
        {
            (int player1Pos, int player2Pos) = ParseInput(input);
            int nextRoll = 1;
            int rollCount = 0;
            int player1Score = 0, player2Score = 0;
            bool nextPlayerIsPlayer1 = true;

            while (player1Score < 1000 && player2Score < 1000)
            {
                if (nextPlayerIsPlayer1)
                {
                    player1Pos = MakeDeterministicMove(player1Pos, ref nextRoll);
                    player1Score += player1Pos;
                }
                else
                {
                    player2Pos = MakeDeterministicMove(player2Pos, ref nextRoll);
                    player2Score += player2Pos;
                }
                rollCount += 3;
                nextPlayerIsPlayer1 ^= true;
            }

            return (Math.Min(player1Score, player2Score) * rollCount).ToString();
        }

        private int MakeDeterministicMove(int pos, ref int nextRoll)
        {
            int spacesMoved = nextRoll;
            nextRoll = nextRoll % 100 + 1;
            spacesMoved += nextRoll;
            nextRoll = nextRoll % 100 + 1;
            spacesMoved += nextRoll;
            nextRoll = nextRoll % 100 + 1;

            return Move(pos, spacesMoved);
        }

        #endregion

        #region Part2

        private Dictionary<(int, int, int, int, bool), (long, long)> diracDict;
        private int[] allPossibleTurns;

        public string SolveSecond(string input)
        {
            (int player1Pos, int player2Pos) = ParseInput(input);
            diracDict = new();
            allPossibleTurns = GenerateAllPossibleTurns();

            (long player1WinCount, long player2WinCount) = PlayDiracGame(player1Pos, player2Pos, 0, 0, true);
            return Math.Max(player1WinCount, player2WinCount).ToString();
        }

        private int[] GenerateAllPossibleTurns()
        {
            var turns = new List<int>();
            for (int roll1 = 1; roll1 <= 3; roll1++)
                for (int roll2 = 1; roll2 <= 3; roll2++)
                    for (int roll3 = 1; roll3 <= 3; roll3++)
                        turns.Add(roll1 + roll2 + roll3);

            return turns.ToArray();
        }

        private (long, long) PlayDiracGame(int player1Pos, int player2Pos, int player1Score, int player2Score, bool nextPlayerIsPlayer1)
        {
            if (diracDict.TryGetValue((player1Pos, player2Pos, player1Score, player2Score, nextPlayerIsPlayer1), 
                                       out (long, long) result))
                return result;

            long player1WinCount = 0, player2WinCount = 0;
            foreach (int spacesMoved in allPossibleTurns)
            {
                int newPlayer1Pos = player1Pos, newPlayer2Pos = player2Pos;
                int newPlayer1Score = player1Score, newPlayer2Score = player2Score;
                if (nextPlayerIsPlayer1)
                {
                    newPlayer1Pos = Move(player1Pos, spacesMoved);
                    newPlayer1Score += newPlayer1Pos;
                    if (newPlayer1Score >= 21)
                    {
                        player1WinCount++;
                        continue;
                    }
                }
                else
                {
                    newPlayer2Pos = Move(player2Pos, spacesMoved);
                    newPlayer2Score += newPlayer2Pos;
                    if (newPlayer2Score >= 21)
                    {
                        player2WinCount++;
                        continue;
                    }
                }

                (long sub_player1WinCount, long sub_player2WinCount) =
                    PlayDiracGame(newPlayer1Pos, newPlayer2Pos, newPlayer1Score, newPlayer2Score, !nextPlayerIsPlayer1);

                player1WinCount += sub_player1WinCount;
                player2WinCount += sub_player2WinCount;
            }

            diracDict.Add((player1Pos, player2Pos, player1Score, player2Score, nextPlayerIsPlayer1), (player1WinCount, player2WinCount));
            return (player1WinCount, player2WinCount);
        }

        #endregion
    }
}
