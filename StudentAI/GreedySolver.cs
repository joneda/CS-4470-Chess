using System;
using System.Collections.Generic;
using System.Linq;
using UvsChess;

namespace StudentAI
{
    internal class GreedySolver
    {
        private Random rnd = new Random();

        public ChessMove GetMove(ChessState currentState)
        {
            if (currentState.AllPossibleMoves.Count < 1)
                return null;

            int max = currentState.AllPossibleMoves.Max(m => m.ValueOfMove);

            //ChessMove[] bestMoves = currentState.AllPossibleMoves.Where(m => m.ValueOfMove == max).ToArray();

            //ChessMove move = currentState.GetGameMove(bestMoves[rnd.Next(0, bestMoves.Length)]);

            ChessMove move = currentState.GetGameMove(currentState.AllPossibleMoves.First(m => m.ValueOfMove == max));

            return move;
        }
    }
}
