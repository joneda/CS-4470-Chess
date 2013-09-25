using System;
using System.Collections.Generic;
using System.Text;
using UvsChess;

namespace StudentAI
{
    public class StudentAI : IChessAI
    {
        private Random random;
        public StudentAI()
        {
            random = new Random();
        }
        #region IChessAI Members that are implemented by the Student

        /// <summary>
        /// The name of your AI
        /// </summary>
        public string Name
        {
#if DEBUG
            get { return "En Passant (Debug)"; }
#else
            get { return "En Passant"; }
#endif
        }

        /// <summary>
        /// Evaluates the chess board and decided which move to make. This is the main method of the AI.
        /// The framework will call this method when it's your turn.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="yourColor">Your color</param>
        /// <returns> Returns the best chess move the player has for the given chess board</returns>
        public ChessMove GetNextMove(ChessBoard board, ChessColor myColor)
        {
            ChessMove myNextMove = null;
            List<ChessMove> allPossibleMoves = new List<ChessMove>();
            while (!IsMyTurnOver())
            {
                for (int row = 0; row < ChessBoard.NumberOfRows; row++)
                {
                    for (int column = 0; column < ChessBoard.NumberOfColumns; column++)
                    {
                        if (board[column, row] != ChessPiece.Empty)
                            allPossibleMoves.AddRange(MoveGenerator(board, board[column, row], column, row, myColor));
                    }
                }
                break;
            }
            int myChosenMoveNumber = random.Next(allPossibleMoves.Count);
            myNextMove = allPossibleMoves[myChosenMoveNumber];
            return myNextMove;
            //throw (new NotImplementedException());
        }

        /// <summary>
        /// Validates a move. The framework uses this to validate the opponents move.
        /// </summary>
        /// <param name="boardBeforeMove">The board as it currently is _before_ the move.</param>
        /// <param name="moveToCheck">This is the move that needs to be checked to see if it's valid.</param>
        /// <param name="colorOfPlayerMoving">This is the color of the player who's making the move.</param>
        /// <returns>Returns true if the move was valid</returns>
        public bool IsValidMove(ChessBoard boardBeforeMove, ChessMove moveToCheck, ChessColor colorOfPlayerMoving)
        {
            //If statlemate, siganl valid move
            if (moveToCheck.Flag == ChessFlag.Stalemate)
            {
                return true;
            }

            //Check to see if they moved a peice of their color then pass the validation on to the ValidMove class
            ChessPiece movedPiece = boardBeforeMove[moveToCheck.From];
            if (colorOfPlayerMoving == ChessColor.Black && movedPiece < ChessPiece.Empty)
            {
                return ValidateMove.ValidMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
            }
            if (colorOfPlayerMoving == ChessColor.White && movedPiece > ChessPiece.Empty)
            {
                return ValidateMove.ValidMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
            }
            return false;
        }

        public List<ChessMove> MoveGenerator(ChessBoard board, ChessPiece piece, int column, int row, ChessColor color)
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

        #endregion
















        #region IChessAI Members that should be implemented as automatic properties and should NEVER be touched by students.
        /// <summary>
        /// This will return false when the framework starts running your AI. When the AI's time has run out,
        /// then this method will return true. Once this method returns true, your AI should return a 
        /// move immediately.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        public AIIsMyTurnOverCallback IsMyTurnOver { get; set; }

        /// <summary>
        /// Call this method to print out debug information. The framework subscribes to this event
        /// and will provide a log window for your debug messages.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AILoggerCallback Log { get; set; }

        /// <summary>
        /// Call this method to catch profiling information. The framework subscribes to this event
        /// and will print out the profiling stats in your log window.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="key"></param>
        public AIProfiler Profiler { get; set; }

        /// <summary>
        /// Call this method to tell the framework what decision print out debug information. The framework subscribes to this event
        /// and will provide a debug window for your decision tree.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AISetDecisionTreeCallback SetDecisionTree { get; set; }
        #endregion
    }
}
