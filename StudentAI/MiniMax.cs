using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class MiniMax
    {
        private int MaxDepth;
        private AIIsMyTurnOverCallback timesUp;
        private AILoggerCallback log;

        public MiniMax(int depth, AILoggerCallback log, AIIsMyTurnOverCallback timesUp)
        {
            this.timesUp = timesUp;
            this.log = log;
            MaxDepth = depth;
        }

        public ChessMove MiniMaxMove(ChessState state)
        {
			// Reverse Sort the moves
			state.AllPossibleMoves.Sort((x, y) => y.ValueOfMove.CompareTo(x.ValueOfMove));

            ChessMove bestMove = state.AllPossibleMoves[0];
            int bestValue = int.MinValue;
            int i;

            // alpha beta for alpha-beta pruning
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                ChessMove move = state.AllPossibleMoves[i];

                int minResult = MinValue(state.GetStateAfterMove(move, true), 1, move.ValueOfMove, alpha, beta);

                if (minResult > bestValue)
                {
                    bestMove = move;
                    bestValue = minResult;
                }
            }

            log(string.Format("Processed {0} move(s) out of {1}. Move value: {2}", i, state.AllPossibleMoves.Count, bestValue));

            return state.GetGameMove(bestMove);
        }

        private int MaxValue(ChessState state, int currentDepth, int moveValue, int alpha, int beta)
        {
            if (currentDepth >= MaxDepth)
            {
                return moveValue;
            }

            int value = int.MinValue;

            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                value = Math.Max(value,
                                 MinValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                     currentDepth + 1, state.AllPossibleMoves[i].ValueOfMove, alpha, beta));
                if (value >= beta)
                    return value;
                alpha = Math.Max(alpha, value);
            }

			return value == int.MinValue ? moveValue : value;
        }

        private int MinValue(ChessState state, int currentDepth, int moveValue, int alpha, int beta)
        {
            if (currentDepth >= MaxDepth)
            {
                return moveValue;
            }

            int value = int.MaxValue;

            for (int i = 0; i < state.AllPossibleMoves.Count && !timesUp(); i++)
            {
                value = Math.Min(value,
                                 MaxValue(state.GetStateAfterMove(state.AllPossibleMoves[i], true),
                                     currentDepth + 1, -state.AllPossibleMoves[i].ValueOfMove, alpha, beta));
                if (value <= alpha)
                    return value;
                beta = Math.Min(beta, value);
            }

			return value == int.MinValue ? moveValue : value;
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
