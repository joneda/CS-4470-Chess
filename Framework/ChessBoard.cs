/******************************************************************************
* The MIT License
* Copyright (c) 2008 Rusty Howell, Thomas Wiest
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

// Authors:
// 		Thomas Wiest  twiest@users.sourceforge.net
//		Rusty Howell  rhowell@users.sourceforge.net

using System;
using System.Text;
using UvsChess.Framework;

namespace UvsChess
{
    /// <summary>
    /// Represents a chess board.
    /// Pieces can be accessed using x,y coordinates.
    /// Tiles are either a white piece, black piece, or are empty.
    /// </summary>
    public class ChessBoard
    {
        #region Members
        public const int NumberOfRows = 8;
        public const int NumberOfColumns = 8;
        #endregion

        #region private automatic properties
        private ChessPiece[,] Board { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// The main constructor that defaults the ChessBoard to the normal starting chess board state.
        /// </summary>
        public ChessBoard():this(ChessState.FenStartState)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_ctor);
        }

        /// <summary>
        /// Helper constructor that creates a new chess board based off of a Fen string.
        /// </summary>
        /// <param name="fenBoard">The Fen string that represents the board.</param>
        public ChessBoard(string fenBoard)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_ctor_string);
            Board = new ChessPiece[NumberOfRows, NumberOfColumns];

            FromFenBoard(fenBoard);
        }

        /// <summary>
        /// Helper constructor that creates a new chess board based off of a ChessPiece array board.
        /// </summary>
        /// <param name="board">The ChessPiece array that represents the board.</param>
        public ChessBoard(ChessPiece[,] board)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_ctor_ChessPieceArray);
            Board = CloneBoard(board);
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the piece on the board in a specified location; 0,0 is the upper left hand corner of the board.
        /// </summary>
        /// <param name="location">Location of the desired piece</param>
        /// <returns>ChessPiece</returns>
        public ChessPiece this[ChessLocation location]
        {
            get 
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_get_ChessLocationIndexer);
                return this[location.X, location.Y]; 
            }
            set 
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_set_ChessLocationIndexer);
                this[location.X, location.Y] = value; 
            }
        }

        /// <summary>
        /// Gets or sets the piece on the board in a specified location; 0,0 is the upper left hand corner of the board.
        /// </summary>
        /// <param name="x">Column of the desired piece location</param>
        /// <param name="y">Row of the desired piece location</param>
        /// <returns>ChessPiece</returns>
        public ChessPiece this[int x, int y]
        {
            get 
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_get_Indexer_int_int);
                return Board[x, y]; 
            }
            set 
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_set_Indexer_int_int);
                Board[x, y] = value; 
            }
        }

        /// <summary>
        /// Helper property that returns a copy of the raw chess board as a ChessPiece array.
        /// Any modifications to the raw board are _not_ reflected in this ChessBoard object.
        /// </summary>
        public ChessPiece[,] RawBoard
        {
            get 
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_get_RawBoard);
                return this.Clone().Board; 
            }
        }
        #endregion

        #region Methods and Operators
        private ChessPiece[,] CloneBoard(ChessPiece[,] board)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_CloneBoard_ChessPieceArray);
            ChessPiece[,] retBoard = new ChessPiece[NumberOfRows, NumberOfColumns];
            for (int Y = 0; Y < NumberOfRows; Y++)
            {
                for (int X = 0; X < NumberOfColumns; X++)
                {
                    retBoard[X, Y] = board[X, Y];
                }
            }

            return retBoard;
        }

        /// <summary>
        /// Creates a complete copy of this object and returns it.
        /// </summary>
        /// <returns>The copy ChessBoard</returns>
        public ChessBoard Clone()
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_Clone);
            return new ChessBoard(this.Board);
        }

        /// <summary>
        /// Executes a move on the ChessBoard.
        /// </summary>
        /// <param name="move">The move to execute.</param>
        public void MakeMove(ChessMove move)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_MakeMove_ChessMove);
            if (move.IsBasicallyValid)
            {
                // Handle Queening
                if ((this[move.From] == ChessPiece.WhitePawn) && (move.From.Y == 1) && (move.To.Y == 0))
                {
                    this[move.To] = ChessPiece.WhiteQueen;
                }
                else if ((this[move.From] == ChessPiece.BlackPawn) && (move.From.Y == 6) && (move.To.Y == 7))
                {
                    this[move.To] = ChessPiece.BlackQueen;
                }
                else
                {
                    this[move.To] = this[move.From];                    
                }

                this[move.From] = ChessPiece.Empty;
            }
        }

        /// <summary>
        /// Accepts a full Fen board and sets the ChessBoard object to that state.
        /// </summary>
        /// <param name="fenBoard">The Fen string</param>
        public void FromFenBoard(string fenBoard)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_FromFenBoard_string);
            string[] lines = fenBoard.Split(' ')[0].Split('/');
            int spaces = 0;

            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    this[x, y] = ChessPiece.Empty;
                }
            }
            ChessPiece piece = ChessPiece.Empty;
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0, boardCol = 0; x < lines[y].Length; ++x, ++boardCol)
                {
                    if (Char.IsDigit(lines[y][x]))
                    {
                        spaces = Convert.ToInt32(lines[y][x]) - 48;
                        boardCol += spaces - 1;
                    }
                    else
                    {
                        switch (lines[y][x])
                        {
                            case 'r':
                                piece = ChessPiece.BlackRook;
                                break;
                            case 'n':
                                piece = ChessPiece.BlackKnight;
                                break;
                            case 'b':
                                piece = ChessPiece.BlackBishop;
                                break;
                            case 'q':
                                piece = ChessPiece.BlackQueen;
                                break;
                            case 'k':
                                piece = ChessPiece.BlackKing;
                                break;
                            case 'p':
                                piece = ChessPiece.BlackPawn;
                                break;
                            case 'K':
                                piece = ChessPiece.WhiteKing;
                                break;
                            case 'Q':
                                piece = ChessPiece.WhiteQueen;
                                break;
                            case 'R':
                                piece = ChessPiece.WhiteRook;
                                break;
                            case 'B':
                                piece = ChessPiece.WhiteBishop;
                                break;
                            case 'N':
                                piece = ChessPiece.WhiteKnight;
                                break;
                            case 'P':
                                piece = ChessPiece.WhitePawn;
                                break;
                            default:
                                throw new Exception("Invalid FEN board");

                        }
                        this[boardCol, y] = piece;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a partial Fen string from the ChessBoard.
        /// It is only partial however, since Fen strings contain more data than just the board.
        /// </summary>
        /// <returns>The partial Fen string.</returns>
        public string ToPartialFenBoard()
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_ToPartialFenBoard);
            StringBuilder strBuild = new StringBuilder();
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                int spaces = 0;
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    char c = 'x';
                    switch (this[x, y])
                    {
                        case ChessPiece.WhitePawn:
                            c = 'P';
                            break;
                        case ChessPiece.WhiteRook:
                            c = 'R';
                            break;
                        case ChessPiece.WhiteKnight:
                            c = 'N';
                            break;
                        case ChessPiece.WhiteBishop:
                            c = 'B';
                            break;
                        case ChessPiece.WhiteQueen:
                            c = 'Q';
                            break;
                        case ChessPiece.WhiteKing:
                            c = 'K';
                            break;
                        case ChessPiece.BlackPawn:
                            c = 'p';
                            break;
                        case ChessPiece.BlackRook:
                            c = 'r';
                            break;
                        case ChessPiece.BlackKnight:
                            c = 'n';
                            break;
                        case ChessPiece.BlackBishop:
                            c = 'b';
                            break;
                        case ChessPiece.BlackQueen:
                            c = 'q';
                            break;
                        case ChessPiece.BlackKing:
                            c = 'k';
                            break;
                        case ChessPiece.Empty:
                            ++spaces;
                            continue;
                        default:
                            throw new Exception("Invalid chess piece");
                    }
                    if ((c != 'x') && (spaces > 0))
                    {
                        strBuild.Append(spaces.ToString());
                        spaces = 0;
                    }
                    strBuild.Append(c);
                }

                if (spaces > 0)
                {
                    strBuild.Append(spaces.ToString());
                    spaces = 0;
                }
                if (y < 7)
                {
                    strBuild.Append('/');
                }
            }
            return strBuild.ToString();
        }

        /// <summary>
        /// Returns the hashcode for this object
        /// </summary>
        /// <returns>the hash code.</returns>
        public override int GetHashCode()
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessBoard_GetHashCode);
            return this.ToPartialFenBoard().GetHashCode();
        }
        #endregion
    }
}
