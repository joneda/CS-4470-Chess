using System;
using System.Linq;
using UvsChess;

namespace StudentAI
{
    /// <summary>
    /// Determines next move by using a greedy algorithm to select the next move
    /// </summary>
    internal class GreedySolver
    {
        private Random rnd = new Random();

        /// <summary>
        /// Returns the next best move out of all possible moves. If multiple moves are tied for highest value, then
        /// the move is seleted from the tied moves at random.
        /// </summary>
        /// <param name="currentState">The current ChessState representation of the game</param>
        /// <returns>The ChessMove with the highest value</returns>
        public ChessMove GetMove(ChessState currentState)
        {
            if (currentState.AllPossibleMoves.Count < 1)
                return null;

            int max = currentState.AllPossibleMoves.Max(m => m.ValueOfMove);

            ChessMove[] bestMoves = currentState.AllPossibleMoves.Where(m => m.ValueOfMove == max).ToArray();

            ChessMove move = currentState.GetGameMove(bestMoves[rnd.Next(0, bestMoves.Length)]);

            return move;
        }
    }
}
