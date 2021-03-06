﻿using System;
using System.Collections.Generic;
using UvsChess;

namespace EnPassantAI
{
    /// <summary>
    /// This class contains various Heuristic methods that can be used interchangeably with the GreedySolver
    /// </summary>
    internal class Heuristics
    {
        private MoveGenerator moveGenerator;

        private static readonly Dictionary<int, int> pieceValue = new Dictionary<int, int>
        {
            {-SimpleState.King, -10},
            {-SimpleState.Queen, -9},
            {-SimpleState.Rook, -5},
            {-SimpleState.Bishop, -3},
            {-SimpleState.Knight, -3},
            {-SimpleState.Pawn, -1},
            {SimpleState.Empty, 0 },
            {SimpleState.Pawn, 1 },
            {SimpleState.Knight, 3},
            {SimpleState.Bishop, 3},
            {SimpleState.Rook, 5},
            {SimpleState.Queen, 9},
            {SimpleState.King, 10},
        };

        public Heuristics(MoveGenerator moveGenerator)
        {
            this.moveGenerator = moveGenerator;
        }

        /// <summary>
        /// This is our general heuristic that is used during most game-play.
        /// 
        /// Uses a value system to determine the worth of each piece. 
        /// Gives precedence to check if the move doesn't put the piece in danger of being killed. 
        /// Moves pawns foward if there is a chance of queen promotion.
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public int MoreAdvancedAddition(int[,] state, ChessMove move)
        {
            int result = 0;
            bool danger = moveGenerator.InDanger(state, move.To);

            if (move.Flag == ChessFlag.Check)
                result += 1;

            if (move.Flag == ChessFlag.Checkmate)
            {
                result += 5000;
            }
            else if (!danger && shouldAttemptToQueenPawn(state, move.To))
            {
                result += 1;
            }

            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    int piece = state[x, y];

                    // Only add my piece if it's not in danger
                    if (piece <= 0 || !moveGenerator.InDanger(state, new ChessLocation(x, y)))
                        result += pieceValue[piece];
                }
            }
            
            return result;
        }

        /// <summary>
        /// Returns true if its just our king, their king, and we have a single rook or queen
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <param name="myColor">The player's color</param>
        /// <returns>True if we're in an end-game state otherwise false</returns>
        public bool IsEndgame(ChessBoard board, ChessColor myColor)
        {
            int numberOfPieces = 0;

            for (int column = 0; column < ChessBoard.NumberOfColumns; column++)
            {
                for (int row = 0; row < ChessBoard.NumberOfRows; row++)
                {
                    ChessPiece piece = board[column, row];
                    if (piece != ChessPiece.Empty && piece != ChessPiece.WhiteQueen && piece != ChessPiece.WhiteRook &&
                        piece != ChessPiece.BlackQueen && piece != ChessPiece.BlackRook &&
                        piece != ChessPiece.WhiteKing && piece != ChessPiece.BlackKing)
                        return false;
                    if (piece != ChessPiece.Empty)
                        numberOfPieces++;
                }
            }

            if (numberOfPieces == 3)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This is an end-game heuristic used only when we're down to only a few select pieces
        /// </summary>
        /// <param name="state">The state to generate the heursitic for</param>
        /// <param name="move">The state move that was made to create this new state</param>
        /// <returns>Numeric value indicating a value for this state</returns>
        public int Endgame(int[,] state, ChessMove move)
        {

            int result = 0;
            // ek = enemyKing, k = our king, r = rook/queen
            int ekRow = -1, ekCol = -1, kRow = -1, kCol = -1, rRow = -1, rCol = -1;

            bool isQueen = true;

            for (int column = 0; column < ChessBoard.NumberOfColumns; column++)
            {
                for (int row = 0; row < ChessBoard.NumberOfRows; row++)
                {
                    switch (state[column, row])
                    {
                        case -SimpleState.King:
                            ekCol = column;
                            ekRow = row;
                            break;
                        case SimpleState.King:
                            kCol = column;
                            kRow = row;
                            break;
                        case SimpleState.Rook:
                            rCol = column;
                            rRow = row;
                            isQueen = false;
                            break;
                        case SimpleState.Queen:
                            rCol = column;
                            rRow = row;
                            break;
                    }
                }
            }
            // only evaluate if there is a rook/queen
            if (rCol != -1)
            {
                // if rook is by our king, add 64 points
                if ((rCol == kCol && rRow == kRow + 1) || (rCol == kCol && rRow == kRow - 1) || (rRow == kRow && rCol == kCol + 1) ||
                    (rRow == kRow && rCol == kCol - 1) || (rCol == kCol + 1 && rRow == kRow + 1) || (rCol == kCol + 1 && rRow == kRow - 1) ||
                    (rCol == kCol - 1 && rRow == kRow + 1) || (rCol == kCol - 1 && rRow == kRow - 1))
                {
                    result += 1000;

                    // if rook is by our king and between our king and his king, add 64 more points
                    if ((!((ekCol < rCol && kCol < rCol) || (ekCol > rCol && kCol > rCol))) ||
                        (!((ekRow < rRow && kRow < rRow) || (ekRow > rRow && kRow > rRow))))
                    {
                        result += 1000;
                    }
                }
                else
                {
                    if (moveGenerator.InDanger(state, new ChessLocation(rCol, rRow)))
                        result = int.MinValue + 1;
                    // If our king is nearing their king and our Rook/Queen is not by our king, so move it closer to our king
                    else if (Math.Abs(ekCol - kCol) == 2 || Math.Abs(ekRow - kRow) == 2 && ((Math.Abs(kCol - rCol) == 1) || (Math.Abs(kRow - rRow) == 1)))
                        result += 1500;
                    // Move our king closer to thier king
                    else
                        result += ((700 - Math.Abs(ekCol - kCol) * 100) + (700 - Math.Abs(ekRow - kRow) * 100));
                }

                // find the area his king is restricted to.  Add 64 - area so that the smaller the area, the better the move
                int area = 0;
                if (rCol > ekCol && rRow < ekRow) // ek is to right and above rook
                {
                    area = (rCol) * (ChessBoard.NumberOfRows - 1 - rRow);
                    result += (64 - area);

                }
                else if (rCol > ekCol && rRow > ekRow)// ek is to the right and below rook
                {
                    area = rCol * rRow;
                    result += (64 - area);
                }
                else if (rCol < ekCol && rRow < ekRow) // ek is to the left and above rook
                {
                    area = (ChessBoard.NumberOfColumns - 1 - rCol) * (ChessBoard.NumberOfRows - 1 - rRow);
                    result += (64 - area);
                }
                else if (rCol < ekCol && rRow > ekRow) // ek is to the left and below rook
                {
                    area = (ChessBoard.NumberOfColumns - 1 - rCol) * rRow;
                    result += (64 - area);
                }
                #region Top Right Checkmate
                // checkmate king in top right corner
                if ((ekCol == 0 || ekCol == 1) && (ekRow == 6 || ekRow == 7))
                {
                    if (isQueen)
                    {
                        if (kCol == 0 || kRow == 7)
                            result = 0;
                        else if (ekRow == 7 && ekCol == 0 && rRow == 5 && rCol == 1)
                            result = 0;
                        else if (ekRow == 7 && ekCol == 0 && rRow == 6 && rCol == 2)
                            result = 0;
                        else if (ekCol == 0 && ekRow == 6 && kCol == 2 && kRow == 6 && rCol == 2 && (rRow == 7 || rRow == 5))
                            result = 0;
                        else if (ekCol == 1 && ekRow == 7 && kCol == 1 && kRow == 5 && (rCol == 2 || rCol == 0) && rRow == 5)
                            result = 0;
                        else if (ekCol == 0 && (ekRow == 6 || ekRow == 7) && kCol == 2 && (kRow == 5 || kRow == 6) &&
                            rCol == 1 && rRow != 7) // mate in one
                            result += 1000;
                        else if ((ekCol == 0 || ekCol == 1) && ekRow == 7 && (kCol == 1 || kCol == 2) && kRow == 5 &&
                            rRow == 6)
                            result += 1000;
                    }


                    if (ekRow == 7 && ekCol == 0 && kRow == 5 && kCol == 0)
                        result = 0;
                    else if (!isQueen && ekRow == 7 && ekCol == 0 && rRow == 6 && rCol == 1)
                        result = 0;
                    else if ((ekRow == 7 || ekRow == 6) && ekCol == 0 && kRow == 5 && kCol == 2 && rRow == 5 && rCol == 1)
                        result += 500;
                    else if (ekRow == 7 && ekCol == 0 && kRow == 5 && kCol == 2 && rRow == 5 && rCol == 1)
                        result += 1000;
                    else if (ekRow == 7 && (ekCol == 0 || ekCol == 1) && kRow == 5 && kCol == 2 && rRow == 6 && rCol == 2)
                        result += 500;
                    else if (ekRow == 7 && ekCol == 1 && rRow == 6 && rCol == 2 && kRow == 5 && kCol == 1) // mate in one
                        result += 1000;
                    else if (ekRow == 7 && ekCol == 0 && rRow == 6 && rCol == 2 && kRow == 4 && kCol == 1) // mate in two
                        result += 4000;
                    else if (ekRow == 7 && ekCol == 0 && kRow == 6 && kCol == 3 && rRow == 5 && rCol == 1) // mate in two
                        result += 4000;
                    else if (ekRow == 6 && ekCol == 0 && kRow == 6 && kCol == 2 && rRow == 5 && rCol == 1) // mate in one
                        result += 1000;
                }
                #endregion

                #region Top Left Checkmate
                // checkmate king in top left corner
                else if ((ekCol == 7 || ekCol == 6) && (ekRow == 6 || ekRow == 7))
                {
                    if (isQueen)
                    {
                        if (kCol == 7 || kRow == 7)
                            result = 0;
                        else if (ekRow == 7 && ekCol == 7 && rRow == 5 && rCol == 6)
                            result = 0;
                        else if (ekCol == 7 && ekRow == 6 && kCol == 5 && kRow == 6 && rCol == 5 && (rRow == 7 || rRow == 5))
                            result = 0;
                        else if (ekCol == 6 && ekRow == 7 && kCol == 6 && kRow == 5 && (rCol == 5 || rCol == 7) && rRow == 5)
                            result = 0;
                        else if (ekRow == 7 && ekCol == 7 && rRow == 6 && rCol == 5)
                            result = 0;
                        else if (ekCol == 7 && (ekRow == 6 || ekRow == 7) && kCol == 5 && (kRow == 5 || kRow == 6) &&
                            rCol == 6 && rRow != 7) // mate in one
                            result += 1000;
                        else if ((ekCol == 7 || ekCol == 6) && ekRow == 7 && (kCol == 6 || kCol == 5) && kRow == 5 &&
                            rRow == 6)
                            result += 1000;
                    }
                    if (ekRow == 7 && ekCol == 7 && kRow == 5 && kCol == 7 && rCol == 6)
                        result = 0;
                    else if (!isQueen && ekRow == 7 && ekCol == 7 && rRow == 6 && rCol == 6)
                        result = 0;
                    else if ((ekRow == 7 || ekRow == 6) && ekCol == 7 && kRow == 5 && kCol == 5 && rRow == 5 && rCol == 6)
                        result += 500;
                    else if (ekRow == 7 && ekCol == 7 && kRow == 5 && kCol == 5 && rRow == 5 && rCol == 6)
                        result += 1000;
                    else if (ekRow == 7 && (ekCol == 7 || ekCol == 6) && kRow == 5 && kCol == 5 && rRow == 6 && rCol == 5)
                        result += 500;
                    else if (ekRow == 7 && ekCol == 6 && rRow == 6 && rCol == 5 && kRow == 5 && kCol == 6)
                        result += 1000;
                    else if (ekRow == 7 && ekCol == 7 && rRow == 6 && rCol == 5 && kRow == 4 && kCol == 6)
                        result += 4000;
                    else if (ekRow == 6 && ekCol == 7 && kRow == 6 && kCol == 5 && rRow == 5 && rCol == 6)
                        result += 1000;
                    else if (ekRow == 7 && ekCol == 7 && kRow == 6 && kCol == 4 && rRow == 5 && rCol == 6)
                        result += 4000;
                }
                #endregion

                #region Bottom Left checkmate

                // checkmate king in bottom left corner
                else if ((ekCol == 7 || ekCol == 6) && (ekRow == 1 || ekRow == 0))
                {
                    if (isQueen)
                    {
                        if (kCol == 7 || kRow == 0)
                            result = 0;
                        else if (ekRow == 0 && ekCol == 7 && rRow == 2 && rCol == 6)
                            result = 0;
                        else if (ekCol == 7 && ekRow == 1 && kCol == 5 && kRow == 1 && rCol == 5 && (rRow == 0 || rRow == 2))
                            result = 0;
                        else if (ekCol == 6 && ekRow == 0 && kCol == 6 && kRow == 2 && (rCol == 5 || rCol == 7) && rRow == 2)
                            result = 0;
                        else if (ekRow == 0 && ekCol == 7 && rRow == 1 && rCol == 5)
                            result = 0;
                        else if (ekCol == 7 && (ekRow == 1 || ekRow == 0) && kCol == 5 && (kRow == 2 || kRow == 1) &&
                            rCol == 6 && rRow != 0) // mate in one
                            result += 1000;
                        else if ((ekCol == 7 || ekCol == 6) && ekRow == 0 && (kCol == 6 || kCol == 5) && kRow == 2 &&
                            rRow == 1)
                            result += 1000;
                    }
                    if (ekRow == 0 && ekCol == 7 && kRow == 2 && kCol == 7)
                        result = 0;
                    else if (!isQueen && ekRow == 0 && ekCol == 7 && rRow == 1 && rCol == 6)
                        result = 0;
                    else if ((ekRow == 0 || ekRow == 1) && ekCol == 7 && kRow == 2 && kCol == 5 && rRow == 2 && rCol == 6)
                        result += 500;
                    else if (ekRow == 0 && ekCol == 7 && kRow == 2 && kCol == 5 && rRow == 2 && rCol == 6)
                        result += 1000;
                    else if (ekRow == 0 && (ekCol == 7 || ekCol == 6) && kRow == 2 && kCol == 5 && rRow == 1 && rCol == 5)
                        result += 500;
                    else if (ekRow == 0 && ekCol == 6 && rRow == 1 && rCol == 5 && kRow == 2 && kCol == 6)
                        result += 1000;
                    else if (ekRow == 0 && ekCol == 7 && rRow == 1 && rCol == 5 && kRow == 3 && kCol == 6)
                        result += 4000;
                    else if (ekRow == 1 && ekCol == 7 && kRow == 1 && kCol == 5 && rRow == 2 && rCol == 6)
                        result += 1000;
                    else if (ekRow == 0 && ekCol == 7 && kRow == 1 && kCol == 4 && rRow == 2 && rCol == 6)
                        result += 4000;
                }
                #endregion

                #region Bottom Right Checkmate
                // checkmate king in bottom right corner
                else if ((ekCol == 0 || ekCol == 1) && (ekRow == 1 || ekRow == 0))
                {
                    if (isQueen)
                    {
                        if (kCol == 0 || kRow == 0)
                            result = 0;
                        else if (ekRow == 0 && ekCol == 0 && rRow == 2 && rCol == 1)
                            result = 0;
                        else if (ekCol == 0 && ekRow == 1 && kCol == 2 && kRow == 1 && rCol == 2 && (rRow == 0 || rRow == 2))
                            result = 0;
                        else if (ekCol == 1 && ekRow == 0 && kCol == 1 && kRow == 2 && (rCol == 2 || rCol == 0) && rRow == 2)
                            result = 0;
                        else if (ekRow == 0 && ekCol == 0 && rRow == 1 && rCol == 2)
                            result = 0;
                        else if (ekCol == 0 && (ekRow == 1 || ekRow == 0) && kCol == 2 && (kRow == 2 || kRow == 1) &&
                            rCol == 1 && rRow != 0) // mate in one
                            result += 1000;
                        else if ((ekCol == 0 || ekCol == 1) && ekRow == 0 && (kCol == 1 || kCol == 2) && kRow == 2 &&
                            rRow == 1)
                            result += 1000;
                    }
                    if (ekRow == 0 && ekCol == 0 && kRow == 2 && kCol == 0)
                        result = 0;
                    else if (!isQueen && ekRow == 0 && ekCol == 0 && rRow == 1 && rCol == 1)
                        result = 0;
                    else if ((ekRow == 0 || ekRow == 1) && ekCol == 0 && kRow == 2 && kCol == 2 && rRow == 2 && rCol == 1)
                        result += 500;
                    else if (ekRow == 0 && ekCol == 0 && kRow == 2 && kCol == 2 && rRow == 2 && rCol == 1)
                        result += 1000;
                    else if (ekRow == 0 && (ekCol == 0 || ekCol == 1) && kRow == 2 && kCol == 2 && rRow == 1 && rCol == 2)
                        result += 500;
                    else if (ekRow == 0 && ekCol == 1 && rRow == 1 && rCol == 2 && kRow == 2 && kCol == 1)
                        result += 1000;
                    else if (ekRow == 0 && ekCol == 0 && rRow == 1 && rCol == 2 && kRow == 3 && kCol == 1)
                        result += 4000;
                    else if (ekRow == 1 && ekCol == 0 && kRow == 1 && kCol == 2 && rRow == 2 && rCol == 1)
                        result += 1000;
                    else if (ekRow == 0 && ekCol == 0 && kRow == 1 && kCol == 3 && rRow == 2 && rCol == 1)
                        result += 4000;
                }

                #endregion
            }
            else
                result = 0;

            if (move.Flag == ChessFlag.Checkmate)
            {
                result += 5000;
            }

            return result;
        }

        private bool shouldAttemptToQueenPawn(int[,] state, ChessLocation pawnLocation)
        {
            if (state[pawnLocation.X, pawnLocation.Y] == SimpleState.Pawn)
                return false;

            for (int row = pawnLocation.Y + 1; row < state.GetLength(1); row++)
            {
                if (state[pawnLocation.X, row] != SimpleState.Empty)
                    return false;
            }

            return true;
        }
    }
}
