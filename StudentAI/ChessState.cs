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
        private AILoggerCallback log;
        public List<ChessMove> AllPossibleMoves;

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

        static public ChessState GetStateAfterMove(ChessState currentState, ChessMove move, Func<int[,], ChessMove, int> boardEvaluator, bool swappBoardAndColor)
        {
            ChessState newState = new ChessState();
            newState.Columns = currentState.Columns;
            newState.Rows = currentState.Rows;
            newState.log = currentState.log;
            newState.state = MakeMove(currentState.state, move);
            if (swappBoardAndColor)
            {
                if (currentState.color == ChessColor.Black)
                {
                    newState.color = ChessColor.White;
                }
                else
                {
                    newState.color = ChessColor.Black;
                }

                //Swapp all the peices on the board to reverse the friend vs foe
                int columnSwapp = ChessBoard.NumberOfColumns;
                int rowSwapp = ChessBoard.NumberOfRows;
                for (int i = 0; i < columnSwapp; i++)
                {
                    for (int j = 0; j < rowSwapp / 2; j++)
                    {
                        int temp = newState.state[i, j];
                        newState.state[i, j] = newState.state[columnSwapp - 1 - i, rowSwapp - 1 - j];
                        newState.state[columnSwapp - 1 - i, rowSwapp - 1 - j] = temp;
                    }
                }
            }
            else
            {
                newState.color = currentState.color;
            }
            newState.AllPossibleMoves = new MoveGenerator(newState.state, true, boardEvaluator, newState.log).AllPossibleMoves;
            return newState;
        }

        static public int[,] MakeMove(int[,] currentState, ChessMove move)
        {
            int[,] newState = (int[,])currentState.Clone();

            newState[move.To.X, move.To.Y] = newState[move.From.X, move.From.Y];
            newState[move.From.X, move.From.Y] = Piece.Empty;
            if (newState[move.To.X, move.To.Y] == Piece.Pawn && move.To.Y == ChessBoard.NumberOfColumns - 1)
                newState[move.To.X, move.To.Y] = Piece.Queen;

            return newState;
        }
    }
}
