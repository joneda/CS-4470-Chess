using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class ValidateMove
    {
        private const int BLACK_PAWN_START = 1;
        private const int WHITE_PAWN_START = 6;
        private const int KNIGHT_MAX_MOVE_POSITIVE = 2;
        private const int KNIGHT_MAX_MOVE_NEGATIVE = -2;
        private const int KNIGHT_MIN_MOVE_POSITIVE = 1;
        private const int KNIGHT_MIN_MOVE_NEGATIVE = -1;
        private const int KING_MAX_MOVE_POSITIVE = 1;
        private const int KING_MAX_MOVE_NEGATIVE = -1;
        static public bool ValidMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                     ChessColor colorOfPlayerMoving)
        {
            if (moveToCheck.To.X >= 0 && moveToCheck.To.X < ChessBoard.NumberOfColumns && moveToCheck.To.Y >= 0 &&
                moveToCheck.To.Y < ChessBoard.NumberOfRows)
            {
                ChessPiece movedPiece = boardBeforeMove[moveToCheck.From];
                if (movedPiece == ChessPiece.BlackPawn || movedPiece == ChessPiece.WhitePawn)
                    return ValidPawnMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
                if (movedPiece == ChessPiece.BlackRook || movedPiece == ChessPiece.WhiteRook)
                    return ValidRookMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
                if (movedPiece == ChessPiece.BlackBishop || movedPiece == ChessPiece.WhiteBishop)
                    return ValidBishopMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
                if (movedPiece == ChessPiece.BlackQueen || movedPiece == ChessPiece.WhiteQueen)
                    return ValidQueenMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
                if (movedPiece == ChessPiece.BlackKnight || movedPiece == ChessPiece.WhiteKnight)
                    return ValidKnightMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
                if (movedPiece == ChessPiece.BlackKing || movedPiece == ChessPiece.WhiteKing)
                    return ValidKingMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving);
            }
            return false;
        }
        static private bool ValidPawnMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                         ChessColor colorOfPlayerMoving)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if (colorOfPlayerMoving == ChessColor.Black)
            {
                //Black pawns must move down the board by at least one (Negative differenceInY)
                if (differenceInY == -1)
                {
                    if (differenceInX == 0)
                    {
                        if (boardBeforeMove[moveToCheck.To] == ChessPiece.Empty)
                            return true;

                    }
                    else if (differenceInX == 1 || differenceInX == -1)
                    {
                        if (boardBeforeMove[moveToCheck.To] > ChessPiece.Empty)
                            return true;
                    }
                }
                else if (differenceInY == -2)
                {
                    if (moveToCheck.From.Y == BLACK_PAWN_START && boardBeforeMove[moveToCheck.To] == ChessPiece.Empty &&
                        boardBeforeMove[new ChessLocation(moveToCheck.From.X, moveToCheck.From.Y + 1)] ==
                        ChessPiece.Empty && differenceInX == 0)
                        return true;
                }
            }

            else if (colorOfPlayerMoving == ChessColor.White)
            {
                //White pawns must move up the board by at least one (Positive differenceInX)
                if (differenceInY == 1)
                {
                    if (differenceInX == 0)
                    {
                        if (boardBeforeMove[moveToCheck.To] == ChessPiece.Empty)
                            return true;

                    }
                    else if (differenceInX == 1 || differenceInX == -1)
                    {
                        if (boardBeforeMove[moveToCheck.To] < ChessPiece.Empty)
                            return true;
                    }
                }
                else if (differenceInY == 2)
                {
                    if (moveToCheck.From.Y == WHITE_PAWN_START && boardBeforeMove[moveToCheck.To] == ChessPiece.Empty &&
                        boardBeforeMove[new ChessLocation(moveToCheck.From.X, moveToCheck.From.Y - 1)] ==
                        ChessPiece.Empty && differenceInX == 0)
                        return true;
                }
            }
            return false;
        }

        static private bool ValidRookMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                         ChessColor colorOfPlayerMoving)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if (differenceInX != 0 && differenceInY == 0 && EmptyPath(boardBeforeMove, moveToCheck))
            {
                if (colorOfPlayerMoving == ChessColor.Black && boardBeforeMove[moveToCheck.To] >= ChessPiece.Empty)
                {
                    return true;
                }
                if (colorOfPlayerMoving == ChessColor.White && boardBeforeMove[moveToCheck.To] <= ChessPiece.Empty)
                {
                    return true;
                }
            }
            if (differenceInX == 0 && differenceInY != 0 && EmptyPath(boardBeforeMove, moveToCheck))
            {
                if (colorOfPlayerMoving == ChessColor.Black && boardBeforeMove[moveToCheck.To] >= ChessPiece.Empty)
                {
                    return true;
                }
                if (colorOfPlayerMoving == ChessColor.White && boardBeforeMove[moveToCheck.To] <= ChessPiece.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        static private bool ValidBishopMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                           ChessColor colorOfPlayerMoving)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if ((differenceInX == differenceInY || differenceInX == -differenceInY) && EmptyPath(boardBeforeMove, moveToCheck))
            {
                if (colorOfPlayerMoving == ChessColor.Black && boardBeforeMove[moveToCheck.To] >= ChessPiece.Empty)
                {
                    return true;
                }
                if (colorOfPlayerMoving == ChessColor.White && boardBeforeMove[moveToCheck.To] <= ChessPiece.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        static private bool ValidQueenMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                          ChessColor colorOfPlayerMoving)
        {
            if (ValidBishopMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving) || ValidRookMove(boardBeforeMove, moveToCheck, colorOfPlayerMoving))
            {
                return true;
            }
            return false;
        }

        static private bool ValidKnightMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                            ChessColor colorOfPlayerMoving)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if ((colorOfPlayerMoving == ChessColor.Black && boardBeforeMove[moveToCheck.To] >= ChessPiece.Empty) || (colorOfPlayerMoving == ChessColor.White && boardBeforeMove[moveToCheck.To] <= ChessPiece.Empty))
            {
                if (differenceInX == KNIGHT_MAX_MOVE_POSITIVE || differenceInX == KNIGHT_MAX_MOVE_NEGATIVE)
                {
                    if (differenceInY == KNIGHT_MIN_MOVE_POSITIVE || differenceInY == KNIGHT_MIN_MOVE_NEGATIVE)
                    {
                        return true;
                    }
                }
                else if (differenceInX == KNIGHT_MIN_MOVE_POSITIVE || differenceInX == KNIGHT_MIN_MOVE_NEGATIVE)
                {
                    if (differenceInY == KNIGHT_MAX_MOVE_POSITIVE || differenceInY == KNIGHT_MAX_MOVE_NEGATIVE)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        static private bool ValidKingMove(ChessBoard boardBeforeMove, ChessMove moveToCheck,
                                          ChessColor colorOfPlayerMoving)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if ((colorOfPlayerMoving == ChessColor.Black && boardBeforeMove[moveToCheck.To] >= ChessPiece.Empty) || (colorOfPlayerMoving == ChessColor.White && boardBeforeMove[moveToCheck.To] <= ChessPiece.Empty))
            {
                if (differenceInX <= KING_MAX_MOVE_POSITIVE && differenceInX >= KING_MAX_MOVE_NEGATIVE && differenceInY <= KING_MAX_MOVE_POSITIVE && differenceInY >= KING_MAX_MOVE_NEGATIVE)
                {
                    return true;
                }
            }
            return false;
        }

        static private bool EmptyPath(ChessBoard boardBeforeMove, ChessMove moveToCheck)
        {
            int differenceInX = moveToCheck.From.X - moveToCheck.To.X;
            int differenceInY = moveToCheck.From.Y - moveToCheck.To.Y;
            if (differenceInY == 0)
            {
                if (differenceInX > 0)
                {
                    for (int i = differenceInX - 1; i > 0; i--)
                    {
                        if (boardBeforeMove[moveToCheck.To.X + i, moveToCheck.To.Y] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                else if (differenceInX < 0)
                {
                    for (int i = differenceInX + 1; i < 0; i++)
                    {
                        if (boardBeforeMove[moveToCheck.To.X + i, moveToCheck.To.Y] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (differenceInX == 0)
            {
                if (differenceInY > 0)
                {
                    for (int i = differenceInY - 1; i > 0; i--)
                    {
                        if (boardBeforeMove[moveToCheck.To.X, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                else if (differenceInY < 0)
                {
                    for (int i = differenceInY + 1; i < 0; i++)
                    {
                        if (boardBeforeMove[moveToCheck.To.X, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (differenceInY == differenceInX)
            {
                if (differenceInY > 0)
                {
                    for (int i = differenceInY - 1; i > 0; i--)
                    {
                        if (boardBeforeMove[moveToCheck.To.X + i, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                else if (differenceInY < 0)
                {
                    for (int i = differenceInY + 1; i < 0; i++)
                    {
                        if (boardBeforeMove[moveToCheck.To.X + i, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (differenceInY == -differenceInX)
            {
                if (differenceInY > 0)
                {
                    for (int i = differenceInY - 1; i > 0; i--)
                    {
                        if (boardBeforeMove[moveToCheck.To.X - i, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                else if (differenceInY < 0)
                {
                    for (int i = differenceInY + 1; i < 0; i++)
                    {
                        if (boardBeforeMove[moveToCheck.To.X - i, moveToCheck.To.Y + i] != ChessPiece.Empty)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
