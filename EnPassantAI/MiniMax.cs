using System;
using System.Collections.Generic;
using UvsChess;

namespace EnPassantAI
{
    /// <summary>
    /// MiniMax with Alpha Beta Pruning
    /// </summary>
    internal class MiniMax
    {
        private int initialDepth;
        private Func<bool> timesUp;
        private Action<string> log;
        private Func<int[,], ChessMove, int> boardEvaluator;
        private MoveGenerator moveGenerator;

        private Queue<ChessMove> previousMoves = new Queue<ChessMove>();
        private int selectedMoveIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialDepth">The depth to start iterative deepening from</param>
        /// <param name="log">Callback method for writing log messages</param>
        /// <param name="timesUp">Callback method that returns true when we must return a move</param>
        public MiniMax(MoveGenerator moveGenerator, int initialDepth, Action<string> log, Func<bool> timesUp)
        {
            this.moveGenerator = moveGenerator;
            this.initialDepth = initialDepth;
            this.timesUp = timesUp;
            this.log = log;
        }

        /// <summary>
        /// Uses mini-max algorithm to determine the next move that should be taken
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <param name="playerColor">The color of the player to determine moves for</param>
        /// <param name="boardEvaluator">The heuristic to use in evaluating the board</param>
        /// <returns>The best move based on the evaluation</returns>
        public ChessMove MiniMaxMove(ChessBoard board, ChessColor playerColor, Func<int[,], ChessMove, int> boardEvaluator)
        {
            this.boardEvaluator = boardEvaluator;

            ChessMove selectedMove = null;
            List<ChessMove> bestMoves = null;
            int depth = initialDepth;

            int[,] state = SimpleState.GetSimpleState(board, playerColor);
            List<ChessMove> allPossibleMoves = moveGenerator.GetAllMoves(state, true, boardEvaluator);

            while (!timesUp() && allPossibleMoves.Count > 0)
            {
                if (bestMoves != null)
                    selectedMove = bestMoves[selectedMoveIndex % bestMoves.Count];

                // Reverse Sort the moves so our best moves are at the beginning of the list to improve Alpha Beta Pruning
                allPossibleMoves.Sort((x, y) => y.ValueOfMove.CompareTo(x.ValueOfMove));

                bestMoves = new List<ChessMove>();
                bestMoves.Add(allPossibleMoves[0]);

                int i = 0;
                int alpha = int.MinValue;
                int beta = int.MaxValue;

                for (i = 0; i < allPossibleMoves.Count && !timesUp(); i++)
                {
                    ChessMove move = allPossibleMoves[i];

                    move.ValueOfMove = MinValue(moveGenerator.GetStateAfterMove(state, move), move, depth - 1, alpha, beta);

                    // Keep an ordered list of best moves -- this produces better results than simply sorting the final list since
                    // It keeps the relative order the same as the original heuristic pass
                    int pos = 0;
                    while (pos < bestMoves.Count && bestMoves[pos].ValueOfMove >= move.ValueOfMove)
                        pos++;

                    bestMoves.Insert(pos, move);

                    alpha = Math.Max(alpha, move.ValueOfMove);
                }

#if DEBUG
                log(string.Format("(Depth: {5}) Processed {0} move(s) out of {1}. Move: {2} (index: {3}, value: {4})", i, allPossibleMoves.Count, SimpleState.GetGameMove(bestMoves[0], playerColor).ToString(), selectedMoveIndex, bestMoves[0].ValueOfMove, depth));
#endif
                depth++;
            }

            // In order to avoid moving back and forth endlessly, if we duplicate a recent move, then select the next best move instead of the best
            if (previousMoves.Contains(selectedMove))
                selectedMoveIndex++;
            else
                selectedMoveIndex = 0;

            previousMoves.Enqueue(selectedMove.Clone());

            while (previousMoves.Count > 10)
                previousMoves.Dequeue();

            // We shouldn't hit this, but if we haven't made it through one full search path then default to whatever the current best move is from the current incomplete search attempt
            if (selectedMove == null && bestMoves != null && bestMoves.Count > 0)
                selectedMove = bestMoves[0];

            // If we didn't find any moves, then return stalemate
            ChessMove gameMove = SimpleState.GetGameMove(selectedMove, playerColor) ?? new ChessMove(null, null) { Flag = ChessFlag.Stalemate };

            log(string.Format("Chosen Move: {0} - Value: {1}", gameMove, gameMove.ValueOfMove));
            log("Max Depth: " + (depth - 1));

            return gameMove;
        }

        private int MaxValue(int[,] state, ChessMove move, int depth, int alpha, int beta)
        {
            // If we return here, the move passed in represents the enemy's move, so we need to negate the value to obtain an evaluation of the board as it relates to us
            if (depth < 0 || move.Flag == ChessFlag.Checkmate)
                return -move.ValueOfMove;

            state = moveGenerator.GetEnemyState(state);

            int value = int.MinValue;

            List<ChessMove> allPossibleMoves = moveGenerator.GetAllMoves(state, true, boardEvaluator);

            // Sort the moves to get the most out of Alpha Beta Pruning
            allPossibleMoves.Sort((x, y) => y.ValueOfMove.CompareTo(x.ValueOfMove));

            for (int i = 0; i < allPossibleMoves.Count && !timesUp(); i++)
            {
                ChessMove currentMove = allPossibleMoves[i];

                value = Math.Max(value, MinValue(moveGenerator.GetStateAfterMove(state, currentMove), currentMove, depth - 1, alpha, beta));

                if (value >= beta)
                    return value;

                alpha = Math.Max(alpha, value);
            }

            return value;
        }

        private int MinValue(int[,] state, ChessMove move, int depth, int alpha, int beta)
        {
            if (depth < 0 || move.Flag == ChessFlag.Checkmate)
                return move.ValueOfMove;

            state = moveGenerator.GetEnemyState(state);

            int value = int.MaxValue;

            List<ChessMove> allPossibleMoves = moveGenerator.GetAllMoves(state, true, boardEvaluator);

            // Sort the moves to get the most out of Alpha Beta Pruning
            allPossibleMoves.Sort((x, y) => y.ValueOfMove.CompareTo(x.ValueOfMove));

            for (int i = 0; i < allPossibleMoves.Count && !timesUp(); i++)
            {
                ChessMove currentMove = allPossibleMoves[i];

                value = Math.Min(value, MaxValue(moveGenerator.GetStateAfterMove(state, currentMove), currentMove, depth - 1, alpha, beta));

                if (value <= alpha)
                    return value;

                beta = Math.Min(beta, value);
            }

            return value;
        }
    }
}