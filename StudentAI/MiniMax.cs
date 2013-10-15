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
            int i = 0;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                ChessMove move = state.AllPossibleMoves[i];

                int result = MinValue(state.GetStateAfterMove(move, true, false), depth - 1, move.ValueOfMove, alpha, beta);

                if (result > bestValue)
                {
                    bestMove = move;
                    bestValue = result;
                }
                alpha = Math.Max(alpha, result);
            }

            //log(string.Format("Processed {0} move(s) out of {1}. Move: {2} value: {3}", i, state.AllPossibleMoves.Count, state.GetGameMove(bestMove).ToString(), bestValue) );

            return state.GetGameMove(bestMove);
        }

        private int MaxValue(ChessState state, int depth, int moveValue, int alpha, int beta)
        {
            int value = int.MinValue;
            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                //This now does the terminal check to see if we even need to call MinValue or just bypass it to save time.
                value = Math.Max(value,
                                 (depth > 1 ? MinValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true, true),
                                 depth - 1, state.AllPossibleMoves[i].ValueOfMove, alpha, beta) : state.AllPossibleMoves[i].ValueOfMove));
                if (value >= beta)
                    return value;
                alpha = Math.Max(alpha, value);
            }

            return value;
        }

        private int MinValue(ChessState state, int depth, int moveValue, int alpha, int beta)
        {
            int value = int.MaxValue;
            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                //This now does the terminal check to see if we even need to call MaxValue or just bypass it to save time.
                value = Math.Min(value,
                                 (depth > 1 ? MaxValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true, false),
                                 depth - 1, -state.AllPossibleMoves[i].ValueOfMove, alpha, beta) : -state.AllPossibleMoves[i].ValueOfMove));
                if (value <= alpha)
                    return value;
                beta = Math.Min(beta, value);
            }

            return value;
        }
    }
}
