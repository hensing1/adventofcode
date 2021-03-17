using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
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
            int score = 0;
            while (deck.Count > 0)
            {
                score += deck.Peek() * deck.Count;
                deck.Dequeue();
            }
            return score;
        }
    }
}
