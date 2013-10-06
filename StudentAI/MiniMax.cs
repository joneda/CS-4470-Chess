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
        private Func<int[,], ChessMove, int> boardEvaluator;
        public MiniMax(int depth, Func<int[,], ChessMove, int> BoardEvaluator)
        {
            MaxDepth = depth;
            boardEvaluator = BoardEvaluator;
        }
        public ChessMove MaxValue(ChessState state, int currentDepth, ChessMove previousMove)
        {
            ChessMove minMove = previousMove;
            ChessMove bestMove = new ChessMove(null, null);
            bestMove.ValueOfMove = int.MinValue;
            ChessMove move;
            for (int i = 0; i < state.AllPossibleMoves.Count; i++)
            {
                move = state.AllPossibleMoves[i];
                if (currentDepth < MaxDepth)
                {
                    minMove = MinValue(ChessState.GetStateAfterMove(state, move, boardEvaluator, true), currentDepth + 1, move);
                }
                if (move.ValueOfMove > minMove.ValueOfMove)
                {
                    bestMove = move;
                }
                else
                {
                    //I think this would set the move to be the enemies move
                    //bestMove = minMove;

                    bestMove = state.AllPossibleMoves[0]; // I know this isn't right, I'm just not sure what else to set it to yet
                }
            }
            // I think this just wasn't converting the move back to the ChessBoard standard notation which is another reason 
            // it was moving an enemy piece sometimes
            //return bestMove;

            return state.GetGameMove(bestMove); // flip the move back to the ChessBoard grid notation
        }
        public ChessMove MinValue(ChessState state, int currentDepth, ChessMove previousMove)
        {
            ChessMove maxMove = previousMove;
            ChessMove bestMove = new ChessMove(null, null);
            bestMove.ValueOfMove = int.MaxValue;
            ChessMove move;
            for (int i = 0; i < state.AllPossibleMoves.Count; i++)
            {
                move = state.AllPossibleMoves[i];
                if (currentDepth < MaxDepth) maxMove = MaxValue(ChessState.GetStateAfterMove(state, move, boardEvaluator, true), currentDepth + 1, move);
                if (move.ValueOfMove < maxMove.ValueOfMove)
                {
                    bestMove = move;
                }
                else
                {
                    //I think this would set the move to be the enemies move
                    //bestMove = minMove;

                    bestMove = state.AllPossibleMoves[0]; // I know this isn't right, I'm just not sure what else to set it to yet
                }
            }
            // I think this just wasn't converting the move back to the ChessBoard standard notation which is another reason 
            // it was moving an enemy piece sometimes
            //return bestMove;

            return state.GetGameMove(bestMove); // flip the move back to the ChessBoard grid notation
        }
    }
}
