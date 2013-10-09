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
            if (depth < 1)
            {
                return moveValue;
            }

            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                alpha = Math.Max(alpha,
                                 MinValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                     depth - 1, state.AllPossibleMoves[i].ValueOfMove, alpha, beta));
                if (alpha >= beta)
                    break; ;
            }

            return alpha;
        }

        private int MinValue(ChessState state, int depth, int moveValue, int alpha, int beta)
        {
            if (depth < 1)
            {
                return moveValue;
            }

            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                beta = Math.Min(beta,
                                 MaxValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                     depth - 1, -state.AllPossibleMoves[i].ValueOfMove, alpha, beta));
                if (beta <= alpha)
                    break;
            }

            return beta;
        }



        //public ChessMove MaxValue(ChessState state, int currentDepth)
        //{
        //    ChessMove bestMove = null;
        //    ChessMove minMove = new ChessMove(null, null);
        //    minMove.ValueOfMove = int.MinValue;
        //    ChessMove move;
        //    for (int i = 0; i < state.AllPossibleMoves.Count; i++)
        //    {
        //        move = state.AllPossibleMoves[i];
        //        if (currentDepth < MaxDepth)
        //        {
        //            minMove = MinValue(ChessState.GetStateAfterMove(state, move, boardEvaluator, true), currentDepth + 1);
        //        }
        //        if (move.ValueOfMove > minMove.ValueOfMove)
        //        {
        //            bestMove = move;
        //        }
        //        else
        //        {
        //            //I think this would set the move to be the enemies move
        //            bestMove = minMove;

        //            //bestMove = state.AllPossibleMoves[0]; // I know this isn't right, I'm just not sure what else to set it to yet
        //        }
        //    }
        //    // I think this just wasn't converting the move back to the ChessBoard standard notation which is another reason 
        //    // it was moving an enemy piece sometimes
        //    //return bestMove;

        //    return state.GetGameMove(bestMove); // flip the move back to the ChessBoard grid notation
        //}
        //public ChessMove MinValue(ChessState state, int currentDepth)
        //{
        //    ChessMove bestMove = null;
        //    ChessMove maxMove = new ChessMove(null, null);
        //    maxMove.ValueOfMove = int.MaxValue;
        //    ChessMove move;
        //    for (int i = 0; i < state.AllPossibleMoves.Count; i++)
        //    {
        //        move = state.AllPossibleMoves[i];
        //        if (currentDepth < MaxDepth) maxMove = MaxValue(ChessState.GetStateAfterMove(state, move, boardEvaluator, true), currentDepth + 1);
        //        if (move.ValueOfMove < maxMove.ValueOfMove)
        //        {
        //            bestMove = move;
        //        }
        //        else
        //        {
        //            //I think this would set the move to be the enemies move
        //            bestMove = maxMove;

        //            //bestMove = state.AllPossibleMoves[0]; // I know this isn't right, I'm just not sure what else to set it to yet
        //        }
        //    }
        //    // I think this just wasn't converting the move back to the ChessBoard standard notation which is another reason 
        //    // it was moving an enemy piece sometimes
        //    //return bestMove;

        //    return state.GetGameMove(bestMove); // flip the move back to the ChessBoard grid notation
        //}
    }
}
