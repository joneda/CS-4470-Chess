using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class MiniMax
    {
        private Func<bool> timesUp;
        private AILoggerCallback log;

        public MiniMax(AILoggerCallback log, Func<bool> timesUp)
        {
            this.timesUp = timesUp;
            this.log = log;
        }

        public ChessMove MiniMaxMove(int depth, ChessState state)
        {
            // Reverse Sort the moves
            state.AllPossibleMoves.Sort((x, y) => y.ValueOfMove.CompareTo(x.ValueOfMove));

            ChessMove bestMove = state.AllPossibleMoves[0];
            int bestValue = int.MinValue;
            int i;

            for (i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                ChessMove move = state.AllPossibleMoves[i];

                int minResult = MinValue(state.GetStateAfterMove(move, true), depth - 1, move.ValueOfMove, int.MinValue, int.MaxValue);

                if (minResult > bestValue)
                {
                    bestMove = move;
                    bestValue = minResult;
                }
            }

            log(string.Format("Processed {0} move(s) out of {1}. Move value: {2}", i, state.AllPossibleMoves.Count, bestValue));

            return state.GetGameMove(bestMove);
        }

        private int MaxValue(ChessState state, int depth, int moveValue, int alpha, int beta)
        {
            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                //This now does the terminal check to see if we even need to call MinValue or just bypass it to save time.
                alpha = Math.Max(alpha,
                                 (depth > 1 ? MinValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                 depth - 1, state.AllPossibleMoves[i].ValueOfMove, alpha, beta) : state.AllPossibleMoves[i].ValueOfMove));
                if (alpha >= beta)
                    break; ;
            }

            return alpha;
        }

        private int MinValue(ChessState state, int depth, int moveValue, int alpha, int beta)
        {
            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                //This now does the terminal check to see if we even need to call MaxValue or just bypass it to save time.
                beta = Math.Min(beta,
                                 (depth > 1 ? MaxValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                 depth - 1, -state.AllPossibleMoves[i].ValueOfMove, alpha, beta) : -state.AllPossibleMoves[i].ValueOfMove));
                if (beta <= alpha)
                    break;
            }

            return beta;
        }
    }
}
