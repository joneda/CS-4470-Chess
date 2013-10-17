using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    /// <summary>
    /// Simplified values for chess peices for use in ChessState
    /// </summary>
    internal static class SimpleState
    {
        public const int Empty = 0;
        public const int Pawn = 1;
        public const int Knight = 2;
        public const int Bishop = 3;
        public const int Rook = 4;
        public const int Queen = 5;
        public const int King = 6;

        private static readonly int Columns = ChessBoard.NumberOfColumns;
        private static readonly int Rows = ChessBoard.NumberOfRows;

        private static readonly Dictionary<ChessPiece, int> GamePieceToStatePiece = new Dictionary<ChessPiece, int>
        {
            {ChessPiece.BlackPawn, SimpleState.Pawn},
            {ChessPiece.BlackKnight, SimpleState.Knight},
            {ChessPiece.BlackBishop, SimpleState.Bishop},
            {ChessPiece.BlackRook, SimpleState.Rook},
            {ChessPiece.BlackQueen, SimpleState.Queen},
            {ChessPiece.BlackKing, SimpleState.King},
            {ChessPiece.Empty, SimpleState.Empty},
            {ChessPiece.WhitePawn, SimpleState.Pawn},
            {ChessPiece.WhiteKnight, SimpleState.Knight},
            {ChessPiece.WhiteBishop, SimpleState.Bishop},
            {ChessPiece.WhiteRook, SimpleState.Rook},
            {ChessPiece.WhiteQueen, SimpleState.Queen},
            {ChessPiece.WhiteKing, SimpleState.King},
        };

        /// <summary>
        /// Takes a board and color, and returns a simplified state where friend pieces are represented
        /// by positive integers and always start on the same side of the board (0,0). Foe pieces are 
        /// represented as negative values
        /// </summary>
        /// <param name="board">The board to convert</param>
        /// <param name="color">The color of the player (friend)</param>
        /// <returns>A simplified representation of the current board</returns>
        public static int[,] GetSimpleState(ChessBoard board, ChessColor color)
        {
            int[,] state = new int[Columns, Rows];

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    int stateRow = row;
                    int stateCol = col;
                    if (color == ChessColor.White)
                    {
                        stateRow = Rows - row - 1;
                        stateCol = Columns - col - 1;
                    }

                    ChessPiece piece = board[col, row];

                    int multiplier = 1;
                    if (color == ChessColor.White && piece < ChessPiece.Empty
                        || color == ChessColor.Black && piece > ChessPiece.Empty)
                        multiplier = -1;

                    state[stateCol, stateRow] = multiplier * GamePieceToStatePiece[piece];
                }
            }
            return state;
        }

        /// <summary>
        /// Converts a state based move to a ChessBoard move
        /// </summary>
        /// <param name="stateMove">The move the convert</param>
        /// <param name="playerColor">The color of hte player</param>
        /// <returns>The move corrected to work with ChessBoard</returns>
        public static ChessMove GetGameMove(ChessMove stateMove, ChessColor playerColor)
        {
            if (stateMove == null)
                return null;

            ChessMove gameMove = stateMove.Clone();

            if (playerColor == ChessColor.White)
            {
                gameMove.From.X = Columns - gameMove.From.X - 1;
                gameMove.From.Y = Rows - gameMove.From.Y - 1;
                gameMove.To.X = Columns - gameMove.To.X - 1;
                gameMove.To.Y = Rows - gameMove.To.Y - 1;
            }

            return gameMove;
        }
    }
}
