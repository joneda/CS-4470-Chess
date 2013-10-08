using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    /// <summary>
    /// This class contains various Heuristic methods that can be used interchangeably with the GreedySolver
    /// </summary>
    internal static class Heuristics
    {
        private static readonly Dictionary<int, int> pieceValue = new Dictionary<int, int>
        {
            {-Piece.King, -10},
            {-Piece.Queen, -9},
            {-Piece.Rook, -5},
            {-Piece.Bishop, -3},
            {-Piece.Knight, -3},
            {-Piece.Pawn, -1},
            {Piece.Empty, 0 },
            {Piece.Pawn, 1 },
            {Piece.Knight, 3},
            {Piece.Bishop, 3},
            {Piece.Rook, 5},
            {Piece.Queen, 9},
            {Piece.King, 10},
        };

        /// <summary>
        /// Uses a value system to determine the worth of each piece. 
        /// Gives precedence to check if the move doesn't put the piece in danger of being killed. 
        /// Moves pawns foward if there is a chance of queen promotion.
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public static int MoreAdvancedAddition(int[,] state, ChessMove move)
        {
            int result = 0;
            bool danger = MoveGenerator.InDanger(state, move.To);

            if (move.Flag == ChessFlag.Check)
                result += 1;

            if (move.Flag == ChessFlag.Checkmate)
            {
                result += 5000;
            }
            else if (!danger && Heuristics.shouldAttemptToQueenPawn(state, move.To))
            {
                result += 1;
            }

            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    int piece = state[x, y];

                    // Only add my piece if it's not in danger
                    if (piece <= 0 || !MoveGenerator.InDanger(state, new ChessLocation(x, y)))
                        result += pieceValue[piece];
                }
            }

            return result;
        }

        #region Heuristics not currently in use

        /// <summary>
        /// Very basic Heuristic that just adds up the numeric state values of each piece. Some allowance is made for check and checkmate as well
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public static int SimpleAddition(int[,] state, ChessMove move)
        {
            int result = 0;

            //if (move.Flag == ChessFlag.Check)
            //    result += 1;

            //if (move.Flag == ChessFlag.Checkmate)
            //    result += 5000;
            
            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    result += state[x,y];
                }
            }

            return result;
        }

        /// <summary>
        /// Adds on to the SimpleAddition Heuristic by checking if the piece that is being moved is in danger of being killed. If it
        /// is, then the state value of the piece is removed from the result.
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public static int DontGoToDanger(int[,] state, ChessMove move)
        {
            int result = SimpleAddition(state, move);

            if (MoveGenerator.InDanger(state, move.To))
                result -= state[move.To.X, move.To.Y];

            return result;
        }

        /// <summary>
        /// Only counts friendly pieces that are not in danger of being killed
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public static int Cautious(int[,] state, ChessMove move)
        {
            int result = 0;

            if (move.Flag == ChessFlag.Check)
                result += 1;

            if (move.Flag == ChessFlag.Checkmate)
                result += 5000;

            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    int piece = state[x, y];

                    if (piece <= 0 || !MoveGenerator.InDanger(state, new ChessLocation(x, y)))
                        result += piece;
                }
            }

            return result;
        }

        #endregion Heuristics not currently in use

        private static bool shouldAttemptToQueenPawn(int[,] state, ChessLocation pawnLocation)
        {
            if (state[pawnLocation.X, pawnLocation.Y] == Piece.Pawn)
                return false;

            for (int row = pawnLocation.Y + 1; row < state.GetLength(1); row++)
            {
                if (state[pawnLocation.X, row] != Piece.Empty)
                    return false;
            }

            return true;
        }
    }
}
