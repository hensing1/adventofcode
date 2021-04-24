using System;
using System.Collections.Generic;
using System.Linq;
using _2020.Utility;
using static _2020.Utility.Attributes;

namespace _2020.days._22
{
    [ProblemDate(22)]
    class Solver : ISolver
    {
        public string SolveFirst(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            ReadInput(lines, out Queue<int> player1Deck, out Queue<int> player2Deck);

            while (Play(player1Deck, player2Deck)) ;

            int result;
            if (player1Deck.Count > 0)
                result = Tally(player1Deck);
            else
                result = Tally(player2Deck);

            return result.ToString();
        }

        public string SolveSecond(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(input);

            ReadInput(lines, out Queue<int> player1Deck, out Queue<int> player2Deck);

            PlayGame(player1Deck, player2Deck, out int winner);

            int result =
                winner == 1 ?
                    Tally(player1Deck) :
                    Tally(player2Deck);

            return result.ToString();
        }

        private void ReadInput(string[] lines, out Queue<int> player1Deck, out Queue<int> player2Deck)
        {
            player1Deck = new Queue<int>();
            int i = Array.FindIndex(lines, line => line == "Player 1:");
            if (i == -1)
                throw new InvalidInputException();
            i++;
            while (lines[i].Trim() != String.Empty)
            {
                player1Deck.Enqueue(int.Parse(lines[i]));
                i++;
            }

            player2Deck = new Queue<int>();
            i = Array.FindIndex(lines, line => line == "Player 2:");
            if (i == -1)
                throw new InvalidInputException();
            i++;
            while (i < lines.Length && lines[i].Trim() != String.Empty)
            {
                player2Deck.Enqueue(int.Parse(lines[i]));
                i++;
            }
        }

        private bool Play(Queue<int> player1Deck, Queue<int> player2Deck)
        {
            var player1Card = player1Deck.Dequeue();
            var player2Card = player2Deck.Dequeue();

            if (player1Card > player2Card)
            {
                player1Deck.Enqueue(player1Card);
                player1Deck.Enqueue(player2Card);
            }
            else
            {
                player2Deck.Enqueue(player2Card);
                player2Deck.Enqueue(player1Card);
            }

            if (player1Deck.Count == 0 || player2Deck.Count == 0)
                return false;
            return true;
        }

        private int Tally(Queue<int> deck)
        {
            var deckClone = new Queue<int>(deck);
            int score = 0;
            while (deckClone.Count > 0)
            {
                score += deckClone.Peek() * deckClone.Count;
                deckClone.Dequeue();
            }
            return score;
        }

        #region -- Recursive Area --

        private void PlayGame(Queue<int> player1Deck, Queue<int> player2Deck, out int winner)
        {
            // keeping track of both deck histories in a single list, using longs to store two ints at once, is ridiculously slow for some reason
            var previousP1Decks = new List<int>();
            var previousP2Decks = new List<int>();

            do
            {
                int p1Hash = Tally(player1Deck);
                int p2Hash = Tally(player2Deck);
                if (previousP1Decks.Contains(p1Hash) && previousP2Decks.Contains(p2Hash))
                {
                    winner = 1;
                    return;
                }
                previousP1Decks.Add(p1Hash);
                previousP2Decks.Add(p2Hash);
            } while (PlayRound(player1Deck, player2Deck, out winner));
        }

        private bool PlayRound(Queue<int> player1Deck, Queue<int> player2Deck, out int winner)
        {
            int player1Card = player1Deck.Dequeue();
            int player2Card = player2Deck.Dequeue();

            if (player1Deck.Count >= player1Card && player2Deck.Count >= player2Card)
            {
                var newP1Deck = new Queue<int>(player1Deck.Take(player1Card));
                var newP2Deck = new Queue<int>(player2Deck.Take(player2Card));

                PlayGame(newP1Deck, newP2Deck, out winner);
            }
            else
            {
                if (player1Card > player2Card)
                    winner = 1;
                else
                    winner = 2;
            }

            if (winner == 1)
            {
                player1Deck.Enqueue(player1Card);
                player1Deck.Enqueue(player2Card);
            }
            else
            {
                player2Deck.Enqueue(player2Card);
                player2Deck.Enqueue(player1Card);
            }

            if (player1Deck.Count == 0 || player2Deck.Count == 0)
                return false;
            return true;
        }

        #endregion
    }
}
