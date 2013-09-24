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

namespace UvsChess.Framework
{
    internal class ChessState
    {
        #region Members
        public const string FenStartState = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        //public const string FenStartState = "rnbqkbnr/1ppppppp/8/p7/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 2";
        private ChessBoard _currentBoard;
        private ChessBoard _previousBoard;
        private ChessMove _previousMove;
        private ChessColor _yourColor;
        private int _fullMoves = 0;
        private int _halfMoves = 0;
        private ChessLocation _enPassant = null;
		private bool _canWhiteCastleKingSide = false;
		private bool _canWhiteCastleQueenSide = false;
		private bool _canBlackCastleKingSide = false;
		private bool _canBlackCastleQueenSide = false;
		
        //private string _castling = "-";
        #endregion

        #region Constructors
        public ChessState()
            : this(FenStartState)
        {

        }

        /// <summary>
        /// See: http://en.wikipedia.org/wiki/Forsyth-Edwards_Notation
        /// </summary>
        /// <param name="fenBoard"></param>
        public ChessState(string fenBoard)
        {
            if ((fenBoard == null) || (fenBoard == string.Empty))
            {
                fenBoard = FenStartState;
            }
			FromFenBoard(fenBoard);
        }
        #endregion

        #region Properties and Indexers

        public ChessBoard CurrentBoard
        {
            get { return _currentBoard; }
            set { _currentBoard = value; }
        }       

        public ChessBoard PreviousBoard
        {
            get { return _previousBoard; }
            set { _previousBoard = value; }
        }       

        public ChessMove PreviousMove
        {
            get { return _previousMove; }
            set { _previousMove = value; }
        }       

        public ChessColor CurrentPlayerColor
        {
            get { return _yourColor; }
            set { _yourColor = value; }
        }

        /// <summary>
        /// Fullmove number: The number of the full move. It starts at 1, and is incremented after Black's move.
        /// </summary>
        public int FullMoves
        {
            get { return _fullMoves; }
            set 
            { 
                _fullMoves = value; 
                //Program.Log("FullMoves: " + _fullMoves);
            }
        }

        /// <summary>
        /// Halfmove clock: This is the number of halfmoves since the last pawn advance or capture. 
        /// This is used to determine if a draw can be claimed under the fifty move rule.
        /// See: http://en.wikipedia.org/wiki/Fifty_move_rule
        /// </summary>
        public int  HalfMoves
        {
            get { return _halfMoves; }
            set { _halfMoves = value; }
        }
        /// <summary>
        /// Returns the ChessLocation of the available En passant move. If no En passant move is available,
        /// null is returned.
        /// See: http://en.wikipedia.org/wiki/En_passant
        /// </summary>
        public ChessLocation EnPassant
        {
            get { return _enPassant; }
            set { _enPassant = value; }
        }

        public bool CanWhiteCastleKingSide 
        { 
            get { return _canWhiteCastleKingSide; } 
            set { _canWhiteCastleKingSide = value; }
        }

        public bool CanWhiteCastleQueenSide
        {
            get { return _canWhiteCastleQueenSide; }
            set { _canWhiteCastleQueenSide = value; }
        }

        public bool CanBlackCastleKingSide
        {
            get { return _canBlackCastleKingSide; }
            set { _canBlackCastleKingSide = value; }
        }

        public bool CanBlackCastleQueenSide
        {
            get { return _canBlackCastleQueenSide; }
            set { _canBlackCastleQueenSide = value; }
        }

        
        #endregion

        #region Methods and Operators

        public void MakeMove(ChessMove move)
        {
            PreviousMove = move;
            PreviousBoard = CurrentBoard.Clone();
            CurrentBoard.MakeMove(move);

            if (CurrentPlayerColor == ChessColor.White)
            {
                CurrentPlayerColor = ChessColor.Black;
            }
            else
            {
                CurrentPlayerColor = ChessColor.White;
            }
        }

        /// <summary>
        /// Creates a deep copy of ChessState
        /// </summary>
        /// <returns></returns>
        public ChessState Clone()
        {
            ChessState newState = new ChessState();

            if (this.CurrentBoard == null)
                newState.CurrentBoard = null;                
            else
                newState.CurrentBoard = this.CurrentBoard.Clone();                

            if (this.PreviousBoard == null)
                newState.PreviousBoard = null;
            else
                newState.PreviousBoard = this.PreviousBoard.Clone();                

            if (this.PreviousMove == null)
                newState.PreviousMove = null;                
            else
                newState.PreviousMove = this.PreviousMove.Clone();

            newState.CurrentPlayerColor = this.CurrentPlayerColor;

            newState.CanWhiteCastleKingSide = this.CanWhiteCastleKingSide;
            newState.CanWhiteCastleQueenSide = this.CanWhiteCastleQueenSide;
            newState.CanBlackCastleKingSide = this.CanBlackCastleKingSide;
            newState.CanBlackCastleQueenSide = this.CanBlackCastleQueenSide;

            if (this.EnPassant != null)
                newState.EnPassant = this.EnPassant.Clone();
            newState.HalfMoves = this.HalfMoves;
            newState.FullMoves = this.FullMoves;

            return newState;
        }

        #region FEN board
        /// <summary>
        /// Converts the ChessState to a FEN board
        /// </summary>
        /// <returns>FEN board</returns>
        public string ToFenBoard()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append(CurrentBoard.ToPartialFenBoard());

            if (CurrentPlayerColor == ChessColor.White)
            {
                strBuild.Append(" w");
            }
            else
            {
                strBuild.Append(" b");
            }

            //place holder for castling (not currently supported)
            strBuild.Append(" " + CastlingToFen());

            //place holder for en passant (not currently supported)
            strBuild.Append(EnPassantToFen());
            
            //half and full moves
            strBuild.Append(" " + HalfMoves.ToString() + " " + FullMoves.ToString());

            
            return strBuild.ToString();
        }

        /// <summary>
        /// Sets the chess state as described in the FEN board. 
        /// See: http://en.wikipedia.org/wiki/Forsyth-Edwards_Notation
        /// </summary>
        /// <param name="fenBoard"></param>
        public void FromFenBoard(string fenBoard)
        {  
            string[] lines = fenBoard.Split(' ');

            CurrentBoard = new ChessBoard(fenBoard);

            if (lines[1] == "w")
            {
                CurrentPlayerColor = ChessColor.White;
            }
            else if (lines[1] == "b")
            {
                CurrentPlayerColor = ChessColor.Black;
            }
            else
            {
                throw new Exception("Missing active color in FEN board");
            }
			
			//casting is lines[2]
			CastlingFromFen(lines[2]);

            //en passant is lines[3]
            EnPassant = EnPassantFromFen(lines[3].ToLower());            
            
            HalfMoves = Convert.ToInt32(lines[4]);
            FullMoves = Convert.ToInt32(lines[5]);
        }
		
		private void CastlingFromFen(string castling)
		{
			_canBlackCastleKingSide = false;
			_canBlackCastleQueenSide = false;
			_canWhiteCastleKingSide = false;
			_canWhiteCastleQueenSide = false;
			
			if (castling == "-")
			{
				return;
			}
			foreach (char c in castling)
			{
				switch(c)
				{
				case 'K':
					_canWhiteCastleKingSide = true;
					break;			
				case 'Q':
					_canWhiteCastleQueenSide = true;
					break;
				case 'k':
					_canBlackCastleKingSide = true;
					break;
				case 'q':
					_canBlackCastleQueenSide = true;
					break;
				default:
					throw new Exception(string.Format("Invalid castling values '{0}' in fen board", castling));
					
				}
			}
		}
		
		private string CastlingToFen()
		{
			string castling = string.Empty;
			
			castling += (_canWhiteCastleKingSide)?"K":"";
			castling += (_canWhiteCastleQueenSide)?"Q":"";
			castling += (_canBlackCastleKingSide)?"k":"";
			castling += (_canBlackCastleQueenSide)?"q":"";
			
			castling = (castling == string.Empty)?"-":castling;
			return castling;
		}

        private ChessLocation EnPassantFromFen(string fen)
        {
            if (fen == "-")
            {
                return null;
            }

            int row = Convert.ToInt32(fen[0]) - 97;
            int col = Convert.ToInt32(fen[1]) - 48;
            return new ChessLocation(row, col);            
        }

        private string EnPassantToFen()
        {
            if (EnPassant == null)
            {
                return " -"; //preceding space is important
            }

            char c = Convert.ToChar(EnPassant.Y + 97);
            string enp = " " + c + EnPassant.X.ToString();

            return enp;           
        }

        #endregion


        public override string ToString()
        {
            if (this.PreviousMove == null)
            {
                return "Start of Game";
            }
            else
            {
                ChessColor color = ChessColor.White;
                if (CurrentPlayerColor == ChessColor.White)
                {
                    color = ChessColor.Black;
                }
                return string.Format("{0}: {1}",color,PreviousMove.ToString());
            }
        }
        #endregion
    }
}
