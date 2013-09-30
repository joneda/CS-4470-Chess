using System;
using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    internal class ChessState
    {
        private int Columns;
        private int Rows;

        private int[,] state;

        private static readonly Dictionary<ChessPiece, int> GamePieceToStatePiece = new Dictionary<ChessPiece, int>
        {
            {ChessPiece.BlackPawn, Piece.Pawn},
            {ChessPiece.BlackKnight, Piece.Knight},
            {ChessPiece.BlackBishop, Piece.Bishop},
            {ChessPiece.BlackRook, Piece.Rook},
            {ChessPiece.BlackQueen, Piece.Queen},
            {ChessPiece.BlackKing, Piece.King},
            {ChessPiece.Empty, Piece.Empty},
            {ChessPiece.WhitePawn, Piece.Pawn},
            {ChessPiece.WhiteKnight, Piece.Knight},
            {ChessPiece.WhiteBishop, Piece.Bishop},
            {ChessPiece.WhiteRook, Piece.Rook},
            {ChessPiece.WhiteQueen, Piece.Queen},
            {ChessPiece.WhiteKing, Piece.King},
        };

        private ChessColor color;
        private AILoggerCallback log;
        public List<ChessMove> AllPossibleMoves;

        public ChessState(ChessBoard board, ChessColor color, Func<int[,], bool, bool, int> boardEvaluator, AILoggerCallback log)
        {
            log(board.ToPartialFenBoard());
            this.color = color;
            this.log = log;

            Columns = ChessBoard.NumberOfColumns;
            Rows = ChessBoard.NumberOfRows;

            state = new int[Columns, Rows];

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

            MoveGenerator generator = new MoveGenerator(state, true, boardEvaluator, log);
            AllPossibleMoves = generator.AllPossibleMoves;
        }

        public ChessMove GetGameMove(ChessMove stateMove)
        {
            ChessMove gameMove = stateMove.Clone();
            if (color == ChessColor.White)
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
