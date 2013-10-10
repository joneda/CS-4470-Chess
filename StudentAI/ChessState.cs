using System;
using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    /// <summary>
    /// Represents game board as friend vs. foe where friend always starts at 0,0 side of the board.
    /// Determines all possible moves that the friendly pieces can make.
    /// </summary>
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
        private Func<int[,], ChessMove, int> boardEvaluator;
        private AILoggerCallback log;
        public List<ChessMove> AllPossibleMoves;

        public override string ToString()
        {
            string output = string.Empty;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                    output += state[col, row].ToString();

                output += "\r\n";
            }

            return output;
        }

        private ChessState()
        {
            
        }

        /// <summary>
        /// Constructor. Converts ChessBoard to ChessState by ensuring that all pieces are represented as either friend or foe, with friend being
        /// positive values, and foe being negative values.
        /// </summary>
        /// <param name="board">The original ChessBoard to generate this state from</param>
        /// <param name="color">The color to be treated as friend</param>
        /// <param name="boardEvaluator">Heuristic method to use in determining move value</param>
        /// <param name="log">Callback to use for logging</param>
        public ChessState(ChessBoard board, ChessColor color, Func<int[,], ChessMove, int> boardEvaluator, AILoggerCallback log)
        {
#if DEBUG
            log(board.ToPartialFenBoard());
#endif
            this.color = color;
            this.boardEvaluator = boardEvaluator;
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

            AllPossibleMoves = MoveGenerator.GetAllMoves(state, true, boardEvaluator, log);
        }

        /// <summary>
        /// Converts a state based move to a ChessBoard move
        /// </summary>
        /// <param name="stateMove"></param>
        /// <returns></returns>
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

        public ChessState GetStateAfterMove(ChessMove move, bool swappBoardAndColor)
        {
            ChessState newState = new ChessState
            {
                Columns = Columns,
                Rows = Rows,
                state = (int[,])state.Clone(),
                color = color,
				boardEvaluator = boardEvaluator,
                log = log,
            };

            newState.InternalMove(move);

            if (swappBoardAndColor)
                newState.ChangeSides();

            newState.AllPossibleMoves = MoveGenerator.GetAllMoves(newState.state, true, boardEvaluator, newState.log);

            return newState;
        }

        private void ChangeSides()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows / 2; y++)
                {
                    int opCol = Columns - x - 1;
                    int opRow = Rows - y - 1;
                    int temp = state[x, y];
                    state[x, y] = -state[opCol, opRow];
                    state[opCol, opRow] = -temp;
                }
            }

            color = color == ChessColor.White ? ChessColor.Black : ChessColor.White;
        }

        private void InternalMove(ChessMove move)
        {
            state[move.To.X, move.To.Y] = state[move.From.X, move.From.Y];
            state[move.From.X, move.From.Y] = Piece.Empty;

            if (state[move.To.X, move.To.Y] == Piece.Pawn && move.To.Y == Columns - 1)
                state[move.To.X, move.To.Y] = Piece.Queen;
        }
    }
}
