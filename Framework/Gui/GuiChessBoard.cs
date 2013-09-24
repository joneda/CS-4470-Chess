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
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UvsChess.Gui
{
    internal partial class GuiChessBoard : UserControl
    {
        public delegate void PieceMovedByHumanDelegate(ChessMove move);

        public PieceMovedByHumanDelegate PieceMovedByHuman = null;

        ChessBoard _board = new ChessBoard();
        List<ChessMove> _lastFewMoves = null;
        int _boardWidth;
        int _boardHeight;
        int _tileWidth;
        int _tileHeight;
        int _horizontalBorderHeight;
        int _verticalBorderWidth;
        bool _boardChanged = true;
        bool _isDraggingPiece = false;
        bool _isLocked = false;
        int _mouseX;
        int _mouseY;

        ChessLocation _pieceBeingMovedLocation;
        ChessPiece _pieceBeingMoved;

        Bitmap _boardBitmap;        

        Bitmap[] _pieceBitmaps;
        Bitmap[] _verticalBorderBitmaps = new Bitmap[ChessBoard.NumberOfRows];
        Bitmap[] _horizontalBorderBitmaps = new Bitmap[ChessBoard.NumberOfColumns];
        Bitmap _cornerBorder;
        Bitmap _darkTile;
        Bitmap _lightTile;
        Bitmap[] _tileHighlightBitmaps = new Bitmap[10];

        ComboBox cmbchessflags = null;

        public void SetChessFlagCombo(ComboBox chessflags_gui)
        {
            this.cmbchessflags = chessflags_gui;
        }

        public GuiChessBoard()
        {
            InitializeComponent();            

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            int numPieces = Enum.GetValues(typeof(ChessPiece)).Length;
            _pieceBitmaps = new Bitmap[numPieces];

            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resNames = asm.GetManifestResourceNames();

            #region Loading Image Files
            int res = 95;
            foreach (string curRes in resNames)
            {
                switch (curRes)
                {
                    case "UvsChess.Images.Chess_HighlightMove01.png":
                        _tileHighlightBitmaps[0] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[0].SetResolution(res, res);                        
                        break;

                    case "UvsChess.Images.Chess_HighlightMove02.png":
                        _tileHighlightBitmaps[1] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[1].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove03.png":
                        _tileHighlightBitmaps[2] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[2].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove04.png":
                        _tileHighlightBitmaps[3] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[3].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove05.png":
                        _tileHighlightBitmaps[4] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[4].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove06.png":
                        _tileHighlightBitmaps[5] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[5].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove07.png":
                        _tileHighlightBitmaps[6] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[6].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove08.png":
                        _tileHighlightBitmaps[7] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[7].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove09.png":
                        _tileHighlightBitmaps[8] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[8].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Chess_HighlightMove10.png":
                        _tileHighlightBitmaps[9] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _tileHighlightBitmaps[9].SetResolution(res, res);
                        break;
                        
                    case "UvsChess.Images.Chess_DarkBackground.png":
                        _darkTile = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _darkTile.SetResolution(res, res);                        
                        break;

                    case "UvsChess.Images.Chess_LightBackground.png":
                        _lightTile = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _lightTile.SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Corner_Border.png":                    
                        _cornerBorder = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _cornerBorder.SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_0.png":
                        _horizontalBorderBitmaps[0] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[0].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_1.png":
                        _horizontalBorderBitmaps[1] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[1].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_2.png":
                        _horizontalBorderBitmaps[2] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[2].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_3.png":
                        _horizontalBorderBitmaps[3] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[3].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_4.png":
                        _horizontalBorderBitmaps[4] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[4].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_5.png":
                        _horizontalBorderBitmaps[5] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[5].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_6.png":
                        _horizontalBorderBitmaps[6] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[6].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Horizontal_Border_7.png":
                        _horizontalBorderBitmaps[7] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _horizontalBorderBitmaps[7].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_0.png":
                        _verticalBorderBitmaps[0] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[0].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_1.png":
                        _verticalBorderBitmaps[1] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[1].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_2.png":
                        _verticalBorderBitmaps[2] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[2].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_3.png":
                        _verticalBorderBitmaps[3] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[3].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_4.png":
                        _verticalBorderBitmaps[4] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[4].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_5.png":
                        _verticalBorderBitmaps[5] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[5].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_6.png":
                        _verticalBorderBitmaps[6] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[6].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.Vertical_Border_7.png":
                        _verticalBorderBitmaps[7] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _verticalBorderBitmaps[7].SetResolution(res, res);
                        break; 

                    case "UvsChess.Images.ChessPiece_WhitePawn.png":
                        _pieceBitmaps[(int)ChessPiece.WhitePawn] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhitePawn].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_WhiteRook.png":
                        _pieceBitmaps[(int)ChessPiece.WhiteRook] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhiteRook].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_WhiteKnight.png":
                        _pieceBitmaps[(int)ChessPiece.WhiteKnight] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhiteKnight].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_WhiteBishop.png":
                        _pieceBitmaps[(int)ChessPiece.WhiteBishop] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhiteBishop].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_WhiteQueen.png":
                        _pieceBitmaps[(int)ChessPiece.WhiteQueen] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhiteQueen].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_WhiteKing.png":
                        _pieceBitmaps[(int)ChessPiece.WhiteKing] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.WhiteKing].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_BlackPawn.png":
                        _pieceBitmaps[(int)ChessPiece.BlackPawn] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackPawn].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_BlackRook.png":
                        _pieceBitmaps[(int)ChessPiece.BlackRook] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackRook].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_BlackKnight.png":
                        _pieceBitmaps[(int)ChessPiece.BlackKnight] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackKnight].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_BlackBishop.png":
                        _pieceBitmaps[(int)ChessPiece.BlackBishop] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackBishop].SetResolution(res, res);
                        break;
                    
                    case "UvsChess.Images.ChessPiece_BlackQueen.png":
                        _pieceBitmaps[(int)ChessPiece.BlackQueen] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackQueen].SetResolution(res, res);
                        break;

                    case "UvsChess.Images.ChessPiece_BlackKing.png":
                        _pieceBitmaps[(int)ChessPiece.BlackKing] = (Bitmap)Bitmap.FromStream(asm.GetManifestResourceStream(curRes));
                        _pieceBitmaps[(int)ChessPiece.BlackKing].SetResolution(res, res);
                        break;
                }
            }
            #endregion

            _horizontalBorderHeight = _horizontalBorderBitmaps[0].Height;
            _verticalBorderWidth = _verticalBorderBitmaps[0].Width;

            _tileWidth = _darkTile.Width;
            _tileHeight = _darkTile.Height;
            _boardHeight = (ChessBoard.NumberOfRows * _tileHeight) + _horizontalBorderHeight;
            _boardWidth = (ChessBoard.NumberOfColumns * _tileWidth) + _verticalBorderWidth;
           
            this.Paint += OnPaint;
            this.MouseDown += OnMouseDown;
            this.MouseUp += OnMouseUp;
            this.MouseMove += OnMouseMove;
            this.Resize += OnResize;


            // Validate the image sizes
            foreach (Bitmap curBitmap in _pieceBitmaps)
            {
                if (curBitmap != null)
                {
                    if (curBitmap.Height != _tileHeight)
                    {
                        throw (new Exception("Image height is outside the default height!"));
                    }

                    if (curBitmap.Width != _tileWidth)
                    {
                        throw (new Exception("Image width is outside the default width!"));
                    }
                }
            }
        }

        private ChessBoard Board
        {
            get
            {
                lock (this._board)
                {
                    return _board;
                }
            }

            set
            {
                lock (this._board)
                {
                    _board = value;
                }
            }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        private int AdjustedVerticalBorderWidth
        {
            get
            {
                return (int)(_verticalBorderWidth * ( (float)this.Width / (float)_boardWidth));
            }
        }

        private int AdjustedHorizontalBorderHeight
        {
            get
            {
                return (int)(_horizontalBorderHeight * ((float)this.Height / (float)_boardHeight));
            }
        }

        private int AdjustedTileWidth
        {
            get
            {
                return (int)(_tileWidth * ( (float)this.Width / (float)_boardWidth));
            }
        }

        private int AdjustedTileHeight
        {
            get
            {
                return (int)(_tileHeight * ((float)this.Height / (float)_boardHeight));
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (this.Width < this.Height)
            {
                this.Height = this.Width;
            }
            else
            {
                this.Width = this.Height;
            }

            Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {            
            if (_boardChanged)
            {
                _boardBitmap = GetBoardImage(Board);
                _boardChanged = false;
            }

            // This one doesn't resize the board with the control
            //e.Graphics.DrawImage(_boardBitmap, 0,0);
            e.Graphics.DrawImage(_boardBitmap, 0, 0, this.Width, this.Height);

            if (_isDraggingPiece)
            {
                e.Graphics.DrawImage(_pieceBitmaps[(int)_pieceBeingMoved], _mouseX - (_tileWidth / 2), _mouseY - (_tileHeight / 2));
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if ( (! IsLocked) &&
                 (e.Button == MouseButtons.Left) &&
                 (e.X > AdjustedVerticalBorderWidth) &&
                 (e.Y > AdjustedHorizontalBorderHeight) &&
                 (e.X < this.Width) &&
                 (e.Y < this.Height))
            {                
                _mouseX = e.X;
                _mouseY = e.Y;

                _pieceBeingMovedLocation = new ChessLocation((_mouseX - AdjustedVerticalBorderWidth) / AdjustedTileWidth,
                                                             (_mouseY - AdjustedHorizontalBorderHeight) / AdjustedTileHeight);

                _pieceBeingMoved = Board[_pieceBeingMovedLocation];

                if (_pieceBeingMoved != ChessPiece.Empty)
                {
                    // Only say the mouse is down if they clicked on a non-empty board square
                    _isDraggingPiece = true;

                    Board[_pieceBeingMovedLocation] = ChessPiece.Empty;
                    _boardChanged = true;

                    this.Invalidate();
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if ( (e.Button == MouseButtons.Left) &&
                 (_isDraggingPiece) )
            {
                _isDraggingPiece = false;
                _mouseX = e.X;
                _mouseY = e.Y;

                if ( (_mouseX > AdjustedVerticalBorderWidth) &&
                     (_mouseY > AdjustedHorizontalBorderHeight) &&
                     (_mouseX < this.Width) &&
                     (_mouseY < this.Height))
                {
                    ChessLocation newLoc = new ChessLocation((_mouseX - AdjustedVerticalBorderWidth) / AdjustedTileWidth,
                                                             (_mouseY - AdjustedHorizontalBorderHeight) / AdjustedTileHeight);

                    if (_pieceBeingMovedLocation == newLoc)
                    {
                        // They picked up a piece and put it back down
                        Board[_pieceBeingMovedLocation] = _pieceBeingMoved;
                        _boardChanged = true;
                        this.Invalidate();
                    }
                    else if (PieceMovedByHuman != null)
                    {
                        // Someone has subscribed to the event, so it's their
                        // responsibility to call my ResetBoard methods to update the board
                        // Fire the event, and forget
                        ChessMove humanMove = new ChessMove(_pieceBeingMovedLocation, newLoc);

                        //Set the chess flag from the cmbChessFlag
                        humanMove.Flag = (ChessFlag) cmbchessflags.SelectedItem;
                        cmbchessflags.SelectedIndex = 0; //reset flag to NoFlag

                        PieceMovedByHuman(humanMove);
                    }
                    else
                    {
                        // At this point, they didn't just pick up a piece and put it back down,
                        // _and_ noone is subscribed to the event, so I have to update the
                        // board myself.
                        // This should never really happen 'cause when the game is stopped,
                        // WinGui subs to this event.
                        Board[newLoc] = _pieceBeingMoved;
                        _boardChanged = true;
                        this.Invalidate();
                    }
                }
                else
                {
                     // they drug a piece off the board. Put it back to where it was.
                     Board[_pieceBeingMovedLocation] = _pieceBeingMoved;
                     _boardChanged = true;
                     this.Invalidate();
                }                
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingPiece)
            {
                _mouseX = e.X;
                _mouseY = e.Y;

                this.Invalidate();
            }
        }

        private Bitmap GetBoardImage(ChessBoard board)
        {
            Bitmap boardBitmap = new Bitmap(_boardWidth, _boardHeight);

            Graphics boardGraphics = Graphics.FromImage(boardBitmap);

            boardGraphics.DrawImage(_cornerBorder, 0, 0);

            for (int ix = 0; ix < ChessBoard.NumberOfColumns; ix++)
            {
                int curPos = (ix * _tileWidth) + _verticalBorderWidth;

                boardGraphics.DrawImage(_horizontalBorderBitmaps[ix], curPos, 0);
                boardGraphics.DrawImage(_verticalBorderBitmaps[ix], 0, curPos);
            }

            bool lightSquare = true;
            for (int y = 0; y < ChessBoard.NumberOfRows; y++)
            {
                int curY = (y * _tileHeight) + _horizontalBorderHeight;

                for (int x = 0; x < ChessBoard.NumberOfColumns; x++)
                {
                    int curX = (x * _tileWidth) + _verticalBorderWidth;

                    if (lightSquare)
                    {
                        boardGraphics.DrawImage(_lightTile, curX, curY);
                    }
                    else
                    {
                        boardGraphics.DrawImage(_darkTile, curX, curY);
                    }

                    if ((_lastFewMoves != null) && (_lastFewMoves.Count > 0))
                    {
                        ChessLocation curLoc = new ChessLocation(x, y);

                        for (int ix = 0; ix < _lastFewMoves.Count; ix++)
                        {
                            if ((_lastFewMoves[ix].From == curLoc) ||
                                (_lastFewMoves[ix].To == curLoc))
                            {
                                boardGraphics.DrawImage(_tileHighlightBitmaps[ix % _tileHighlightBitmaps.Length], curX, curY);
                            }
                        }
                    }

                    if (board[x, y] != ChessPiece.Empty)
                    {
                        boardGraphics.DrawImage(_pieceBitmaps[(int)board[x, y]], curX, curY);
                    }
                    
                    lightSquare = !lightSquare;
                }

                lightSquare = !lightSquare;
            }

            boardGraphics.Dispose();
            boardGraphics = null;

            return boardBitmap;
        }

        /// <summary>
        /// Resets the chess board to the given state highlighting the last move
        /// </summary>
        /// <param name="board"></param>
        public void ResetBoard(ChessBoard board, ChessMove lastMove)
        {
            if (lastMove == null)
            {
                ResetBoard(board, new List<ChessMove>());
            }
            else
            {
                ResetBoard(board, new List<ChessMove>() {lastMove});
            }
        }

        /// <summary>
        /// Resets the chess board to the given state highlighting the last few moves
        /// </summary>
        /// <param name="board"></param>
        public void ResetBoard(ChessBoard board, List<ChessMove> lastFewMoves)
        {
            if (Board != board)
            {
                Board = board.Clone();
                _boardChanged = true;
                _lastFewMoves = lastFewMoves;

                this.Invalidate();
            }
        }

        public Color GetMoveHighlightColor(int moveIndex)
        {
            Bitmap tile = _tileHighlightBitmaps[moveIndex % _tileHighlightBitmaps.Length];
            return tile.GetPixel(tile.Height / 2, tile.Width / 2);
        }
    }
}
