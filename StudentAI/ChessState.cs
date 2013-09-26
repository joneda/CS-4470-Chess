using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    internal class ChessState
    {
        private ChessBoard board;

        public ChessColor Color { get; private set; }

        public List<ChessMove> AllPossibleMoves { get; private set; }

        public ChessState PreviousState { get; private set; }

        public ChessMove ActionPerformed { get; private set; }

        public int Depth { get; private set; }

        public ChessPiece[,] RawBoard
        {
            get { return board.RawBoard; }
        }

        public string PartialFenBoard 
        { 
            get { return board.ToPartialFenBoard(); } 
        }

        public ChessState(ChessBoard board, ChessColor color)
        {
            this.board = board.Clone();
            this.Color = color;

            DetermineMoves();
        }

        private ChessState(ChessState parent, ChessMove move)
        {
            PreviousState = parent;
            Depth = parent.Depth + 1;

            board = parent.board.Clone();
            Color = parent.Color;
            board.MakeMove(move);

            ActionPerformed = move;

            DetermineMoves();
        }

        public ChessState Move(ChessMove move)
        {
            return new ChessState(this, move);
        }

        private void DetermineMoves()
        {
            AllPossibleMoves = new List<ChessMove>();

            for (int row = 0; row < ChessBoard.NumberOfRows; row++)
            {
                for (int column = 0; column < ChessBoard.NumberOfColumns; column++)
                {
                    if (board[column, row] != ChessPiece.Empty)
                        AllPossibleMoves.AddRange(MoveGenerator(board, board[column, row], column, row, Color));
                }
            }
        }

        private static List<ChessMove> MoveGenerator(ChessBoard board, ChessPiece piece, int column, int row, ChessColor color)
        {
            List<ChessMove> allMoves = new List<ChessMove>();

            if (color == ChessColor.White)
            {
                switch (piece)
                {
                    #region White Pawn Options
                    case ChessPiece.WhitePawn:
                        {
                            // if on row 6, pawn can move forward to row 5 or to row 4
                            if (row == 6)
                            {
                                if (board[column, row - 1] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                                if (board[column, row - 1] == ChessPiece.Empty && board[column, row - 2] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 2)));
                            }
                            // if less than row 6, pawn can move forward to row - 1
                            else if (row < 6)
                            {
                                if (board[column, row - 1] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                            }
                            // if there is an opponent piece on either [row - 1, column - 1] or [row - 1, column + 1], white can kill that piece
                            if (row - 1 >= 0 && column - 1 >= 0 && board[column - 1, row - 1] < ChessPiece.Empty)
                            {
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)));
                            }
                            if (row - 1 >= 0 && column + 1 < ChessBoard.NumberOfColumns && board[column + 1, row - 1] < ChessPiece.Empty)
                            {
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)));
                            }
                            // if pawn is on row 1, moving to row 0 will turn it into a queen (or any other valid piece it wants to be besides king or pawn)
                            break;
                        }
                    #endregion
                    #region White Knight Options
                    case ChessPiece.WhiteKnight:
                        {
                            // down 2 right 1
                            if (row < ChessBoard.NumberOfRows - 2 && column < ChessBoard.NumberOfColumns - 1 && !(board[column + 1, row + 2] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 2)));
                            // down 2 left 1
                            if (row < ChessBoard.NumberOfRows - 2 && column > 0 && !(board[column - 1, row + 2] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 2)));
                            // up 2 right 1
                            if (row > 1 && column < ChessBoard.NumberOfColumns - 1 && !(board[column + 1, row - 2] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 2)));
                            // up 2 left 1
                            if (row > 1 && column > 0 && !(board[column - 1, row - 2] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 2)));
                            // down 1 right 2
                            if (row < ChessBoard.NumberOfRows - 1 && column < ChessBoard.NumberOfColumns - 2 && !(board[column + 2, row + 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row + 1)));
                            // down 1 left 2
                            if (row < ChessBoard.NumberOfRows - 1 && column > 1 && !(board[column - 2, row + 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row + 1)));
                            // up 1 right 2
                            if (row > 0 && column < ChessBoard.NumberOfColumns - 2 && !(board[column + 2, row - 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row - 1)));
                            // up 1 left 2
                            if (row > 0 && column > 1 && !(board[column - 2, row - 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row - 1)));

                            break;
                        }
                    #endregion
                    #region White Bishop Options
                    case ChessPiece.WhiteBishop:
                        {
                            bool keepgoing = true;
                            // Possible bishop moves to the top right of the board
                            for (int c = column + 1, r = row - 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r >= 0); c++, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom right of the board
                            keepgoing = true;
                            for (int c = column + 1, r = row + 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r < ChessBoard.NumberOfRows); c++, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row + 1; (c >= 0) && (r < ChessBoard.NumberOfRows); c--, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the top left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region White Rook Options
                    case ChessPiece.WhiteRook:
                        {
                            bool keepgoing = true;
                            // Possible rook moves to the right of the board
                            for (int c = column + 1; keepgoing && c < ChessBoard.NumberOfColumns; c++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the left of the board
                            keepgoing = true;
                            for (int c = column - 1; keepgoing && c >= 0; c--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the bottom of the board
                            keepgoing = true;
                            for (int r = row + 1; keepgoing && r < ChessBoard.NumberOfRows; r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the top of the board
                            keepgoing = true;
                            for (int r = row - 1; keepgoing && r >= 0; r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region White Queen Options
                    case ChessPiece.WhiteQueen:
                        {
                            bool keepgoing = true;
                            // Possible bishop moves to the top right of the board
                            for (int c = column + 1, r = row - 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r >= 0); c++, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom right of the board
                            keepgoing = true;
                            for (int c = column + 1, r = row + 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r < ChessBoard.NumberOfRows); c++, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row + 1; (c >= 0) && (r < ChessBoard.NumberOfRows); c--, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the top left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }

                            // Possible rook moves to the right of the board
                            for (int c = column + 1; keepgoing && c < ChessBoard.NumberOfColumns; c++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the left of the board
                            keepgoing = true;
                            for (int c = column - 1; keepgoing && c >= 0; c--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the bottom of the board
                            keepgoing = true;
                            for (int r = row + 1; keepgoing && r < ChessBoard.NumberOfRows; r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the top of the board
                            keepgoing = true;
                            for (int r = row - 1; keepgoing && r >= 0; r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region White King Options
                    case ChessPiece.WhiteKing:
                        {
                            // Check if the king is on the edge of the board or if a piece of the same color is next to him
                            bool down = true, up = true, right = true, left = true;
                            if (!(row + 1 < ChessBoard.NumberOfRows))
                                down = false;
                            if (!(row - 1 >= 0))
                                up = false;
                            if (column + 1 >= ChessBoard.NumberOfColumns)
                                right = false;
                            if (column - 1 <= 0)
                                left = false;

                            if (down && !(board[column, row + 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                            if (up && !(board[column, row - 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                            if (right && !(board[column + 1, row] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row)));
                            if (left && !(board[column - 1, row] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row)));
                            if (right && down && !(board[column + 1, row + 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                            if (right && up && !(board[column + 1, row - 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)));
                            if (left && down && !(board[column - 1, row + 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                            if (left && up && !(board[column - 1, row - 1] > ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)));
                            break;
                        }
                    #endregion
                }
            }
            else
            {
                switch (piece)
                {
                    #region Black Pawn Options
                    case ChessPiece.BlackPawn:
                        {
                            // if on row 1, pawn can move forward to row 2 or row 3
                            if (row == 1)
                            {
                                if (board[column, row + 1] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                                if (board[column, row + 1] == ChessPiece.Empty && board[column, row + 2] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 2)));
                            }
                            // if greater than row 1, pawn can move forward to row + 1
                            else if (row > 1)
                            {
                                if (board[column, row + 1] == ChessPiece.Empty)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                            }
                            // if there is an opponent piece on either [row + 1, column - 1] or [row + 1, column + 1], black can kill that piece
                            if (column + 1 < ChessBoard.NumberOfColumns && row + 1 < ChessBoard.NumberOfRows && board[column + 1, row + 1] > ChessPiece.Empty)
                            {
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                            }
                            if (column - 1 >= 0 && row + 1 < ChessBoard.NumberOfRows && board[column - 1, row + 1] > ChessPiece.Empty)
                            {
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                            }
                            // if pawn is on row 6, moving to row 7 will turn it into a queen (or any other valid piece it wants to be besides king or pawn)
                            break;
                        }
                    #endregion
                    #region Black Knight Options
                    case ChessPiece.BlackKnight:
                        {
                            // down 2 right 1
                            if (row < ChessBoard.NumberOfRows - 2 && column < ChessBoard.NumberOfColumns - 1 && !(board[column + 1, row + 2] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 2)));
                            // down 2 left 1
                            if (row < ChessBoard.NumberOfRows - 2 && column > 0 && !(board[column - 1, row + 2] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 2)));
                            // up 2 right 1
                            if (row > 1 && column < ChessBoard.NumberOfColumns - 1 && !(board[column + 1, row - 2] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 2)));
                            // up 2 left 1
                            if (row > 1 && column > 0 && !(board[column - 1, row - 2] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 2)));
                            // down 1 right 2
                            if (row < ChessBoard.NumberOfRows - 1 && column < ChessBoard.NumberOfColumns - 2 && !(board[column + 2, row + 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row + 1)));
                            // down 1 left 2
                            if (row < ChessBoard.NumberOfRows - 1 && column > 1 && !(board[column - 2, row + 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row + 1)));
                            // up 1 right 2
                            if (row > 0 && column < ChessBoard.NumberOfColumns - 2 && !(board[column + 2, row - 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row - 1)));
                            // up 1 left 2
                            if (row > 0 && column > 1 && !(board[column - 2, row - 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row - 1)));

                            break;
                        }
                    #endregion
                    #region Black Bishop Options
                    case ChessPiece.BlackBishop:
                        {
                            bool keepgoing = true;
                            // Possible bishop moves to the top right of the board
                            for (int c = column + 1, r = row - 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r >= 0); c++, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom right of the board
                            keepgoing = true;
                            for (int c = column + 1, r = row + 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r < ChessBoard.NumberOfRows); c++, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row + 1; (c >= 0) && (r < ChessBoard.NumberOfRows); c--, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the top left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region Black Rook Options
                    case ChessPiece.BlackRook:
                        {
                            bool keepgoing = true;
                            // Possible rook moves to the right of the board
                            for (int c = column + 1; keepgoing && c < ChessBoard.NumberOfColumns; c++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the left of the board
                            keepgoing = true;
                            for (int c = column - 1; keepgoing && c >= 0; c--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the bottom of the board
                            keepgoing = true;
                            for (int r = row + 1; keepgoing && r < ChessBoard.NumberOfRows; r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the top of the board
                            keepgoing = true;
                            for (int r = row - 1; keepgoing && r >= 0; r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region Black Queen Options
                    case ChessPiece.BlackQueen:
                        {
                            bool keepgoing = true;
                            // Possible bishop moves to the top right of the board
                            for (int c = column + 1, r = row - 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r >= 0); c++, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom right of the board
                            keepgoing = true;
                            for (int c = column + 1, r = row + 1; keepgoing && (c < ChessBoard.NumberOfColumns) && (r < ChessBoard.NumberOfRows); c++, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the bottom left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row + 1; (c >= 0) && (r < ChessBoard.NumberOfRows); c--, r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible bishop moves to the top left of the board
                            keepgoing = true;
                            for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }


                            // Possible rook moves to the right of the board
                            for (int c = column + 1; keepgoing && c < ChessBoard.NumberOfColumns; c++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the left of the board
                            keepgoing = true;
                            for (int c = column - 1; keepgoing && c >= 0; c--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[c, row] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[c, row] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the bottom of the board
                            keepgoing = true;
                            for (int r = row + 1; keepgoing && r < ChessBoard.NumberOfRows; r++)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            // Possible rook moves to the top of the board
                            keepgoing = true;
                            for (int r = row - 1; keepgoing && r >= 0; r--)
                            {
                                // If the potential new location is where a piece of the same color is, we can't move there
                                if (board[column, r] < ChessPiece.Empty)
                                    keepgoing = false;

                                if (keepgoing)
                                    allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                                // If the new location was an enemy piece, we can't keep going beyond it
                                if (board[column, r] > ChessPiece.Empty)
                                    keepgoing = false;
                            }
                            break;
                        }
                    #endregion
                    #region Black King Options
                    case ChessPiece.BlackKing:
                        {
                            // Check if the king is on the edge of the board or if a piece of the same color is next to him
                            bool down = true, up = true, right = true, left = true;
                            if (!(row + 1 < ChessBoard.NumberOfRows))
                                down = false;
                            if (!(row - 1 >= 0))
                                up = false;
                            if (column + 1 >= ChessBoard.NumberOfColumns)
                                right = false;
                            if (column - 1 <= 0)
                                left = false;

                            if (down && !(board[column, row + 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                            if (up && !(board[column, row - 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                            if (right && !(board[column + 1, row] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row)));
                            if (left && !(board[column - 1, row] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row)));
                            if (right && down && !(board[column + 1, row + 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                            if (right && up && !(board[column + 1, row - 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)));
                            if (left && down && !(board[column - 1, row + 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                            if (left && up && !(board[column - 1, row - 1] < ChessPiece.Empty))
                                allMoves.Add(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)));
                            break;
                        }
                    #endregion
                }
            }

            return allMoves;
        }
        
    }
}
