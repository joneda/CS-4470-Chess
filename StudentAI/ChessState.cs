﻿using System;
using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    internal class ChessState
    {
        public const int Empty = 0;
        public const int Pawn = 1;
        public const int Knight = 2;
        public const int Bishop = 3;
        public const int Rook = 4;
        public const int Queen = 5;
        public const int King = 6;

        private int Columns;
        private int Rows;

        private int[,] state;

        private static readonly Dictionary<ChessPiece, int> GamePieceToStatePiece = new Dictionary<ChessPiece, int>
        {
            {ChessPiece.BlackPawn, Pawn},
            {ChessPiece.BlackKnight, Knight},
            {ChessPiece.BlackBishop, Bishop},
            {ChessPiece.BlackRook, Rook},
            {ChessPiece.BlackQueen, Queen},
            {ChessPiece.BlackKing, King},
            {ChessPiece.Empty, Empty},
            {ChessPiece.WhitePawn, Pawn},
            {ChessPiece.WhiteKnight, Knight},
            {ChessPiece.WhiteBishop, Bishop},
            {ChessPiece.WhiteRook, Rook},
            {ChessPiece.WhiteQueen, Queen},
            {ChessPiece.WhiteKing, King},
        };

        private ChessColor color;
        private ChessBoard board;
        private Func<int[,], int> boardEvaluator;

        public List<ChessMove> AllPossibleMoves = new List<ChessMove>();

        public ChessState(ChessBoard board, ChessColor color, Func<int[,], int> boardEvaluator)
        {
            this.board = board.Clone();
            this.boardEvaluator = boardEvaluator;
            this.color = color;

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

            DetermineMoves();
        }

        private void DetermineMoves()
        {
            AllPossibleMoves.Clear();

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (state[column, row] > 0)
                        GenerateMoves(state[column, row], column, row);
                }
            }
        }

        private bool InCheck(int[,] stateToCheck, bool enemy)
        {
            int multiplier = enemy ? -1 : 1;

            int kingToCheck = multiplier * King;
            int attackingPawn = multiplier * -Pawn;
            int attackingKnight = multiplier * -Knight;
            int attackingBishop = multiplier * -Bishop;
            int attackingRook = multiplier * -Rook;
            int attackingQueen = multiplier * -Queen;
            int attackingKing = multiplier * -King;

            int col, row;
            ChessLocation kingLocation = LocatePiece(stateToCheck, kingToCheck);

            bool checkUp = true;
            bool checkDown = true;
            bool checkUpLeft = true;
            bool checkUpRight = true;
            bool checkLeft = true;
            bool checkRight = true;
            bool checkDownLeft = true;
            bool checkDownRight = true;
            bool done = false;
            int distance = 1;

            while (!done)
            {
                col = kingLocation.X;
                row = kingLocation.Y;
                int up = row - distance;
                int down = row + distance;
                int left = col - distance;
                int right = col + distance;

                if (checkUp && up >= 0 && stateToCheck[col, up] != Empty)
                {
                    checkUp = false;
                    if (stateToCheck[col, up] == attackingQueen || stateToCheck[col, up] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, up] == attackingKing)
                        return true;
                }

                if (checkDown && down < Rows && stateToCheck[col, down] != Empty)
                {
                    checkDown = false;
                    if (stateToCheck[col, down] == attackingQueen || stateToCheck[col, down] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, down] == attackingKing)
                        return true;
                }

                if (checkLeft && left >= 0 && stateToCheck[left, row] != Empty)
                {
                    checkLeft = false;
                    if (stateToCheck[left, row] == attackingQueen || stateToCheck[left, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[left, row] == attackingKing)
                        return true;
                }

                if (checkRight && right < Columns && stateToCheck[right, row] != Empty)
                {
                    checkRight = false;
                    if (stateToCheck[right, row] == attackingQueen || stateToCheck[right, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[right, row] == attackingKing)
                        return true;
                }

                if (checkUpLeft && up >= 0 && left >= 0 && stateToCheck[left, up] != Empty)
                {
                    checkUpLeft = false;
                    if (stateToCheck[left, up] == attackingQueen || stateToCheck[left, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, up] == attackingKing || (enemy && stateToCheck[left, up] == attackingPawn)))
                        return true;
                }

                if (checkUpRight && up >= 0 && right < Columns && stateToCheck[right, up] != Empty)
                {
                    checkUpRight = false;
                    if (stateToCheck[right, up] == attackingQueen || stateToCheck[right, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[right, up] == attackingKing || (enemy && stateToCheck[right, up] == attackingPawn)))
                        return true;
                }

                if (checkDownLeft && down < Rows && left >= 0 && stateToCheck[left, down] != Empty)
                {
                    checkDownLeft = false;
                    if (stateToCheck[left, down] == attackingQueen || stateToCheck[left, down] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, down] == attackingKing || (!enemy && stateToCheck[left, down] == attackingPawn)))
                        return true;
                }

                if (checkDownRight && down < Rows && right < Columns && stateToCheck[right, down] != Empty)
                {
                    checkDownRight = false;
                    if (stateToCheck[right, down] == attackingQueen || stateToCheck[right, down] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[right, down] == attackingKing || (!enemy && stateToCheck[right, down] == attackingPawn)))
                        return true;
                }

                done = !(checkUp || checkDown || checkLeft || checkRight || checkUpLeft || checkUpRight || checkDownLeft || checkDownRight);
                done = done || (up < 0 && left < 0 && down >= Rows && right >= Columns);
                distance++;
            }

            // Check knight left up
            col = kingLocation.X - 2;
            row = kingLocation.Y - 1;

            if (col >= 0 && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight left down
            col = kingLocation.X - 2;
            row = kingLocation.Y + 1;

            if (col >= 0 && row < Rows && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight up left
            col = kingLocation.X - 1;
            row = kingLocation.Y - 2;

            if (col >= 0 && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight up right
            col = kingLocation.X + 1;
            row = kingLocation.Y - 2;

            if (col < Columns && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight right up
            col = kingLocation.X + 2;
            row = kingLocation.Y - 1;

            if (col < Columns && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight right down
            col = kingLocation.X + 2;
            row = kingLocation.Y + 1;

            if (col < Columns && row < Rows && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight down left
            col = kingLocation.X - 1;
            row = kingLocation.Y + 2;

            if (col >= 0 && row < Rows && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight down right
            col = kingLocation.X + 1;
            row = kingLocation.Y + 2;

            if (col < Columns && row < Rows && stateToCheck[col, row] == attackingKnight)
                return true;

            return false;
        }

        private ChessLocation LocatePiece(int[,] stateToCheck, int piece)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (stateToCheck[column, row] == piece)
                        return new ChessLocation(column, row);
                }
            }

            return null;
        }

        private void AddMove(ChessMove move)
        {
            int[,] stateAfterMove = GetStateAfterMove(move);

            // If we're in check, then the move is illegal
            if (InCheck(stateAfterMove, false))
                return;

            move.ValueOfMove = boardEvaluator(stateAfterMove);

            if (InCheck(stateAfterMove, true))
            {
                move.Flag = ChessFlag.Check;
           
                // TEST FOR CHECKMATE, THIS CODE DOESN'T SEEM TO WORK RIGHT NOW BUT I'M NOT SURE WHY
                // we know its check, now lets see if its checkmate
                // make a copy of the current board and execute the move that will put the enemy into check
                ChessBoard temp = board.Clone();
                temp.MakeMove(move);
                // Determine the enemy color
                ChessColor enemyColor = (color == ChessColor.White) ? ChessColor.Black : ChessColor.White;
                // create a new state for the enemy so we can get all their possible response moves
                ChessState stateToCheck = new ChessState(temp, enemyColor, boardEvaluator);
                // if there are no possible moves for the enemy, they are in checkmate
                if (stateToCheck.AllPossibleMoves.Count < 1)
                    move.Flag = ChessFlag.Checkmate;
            }

            AllPossibleMoves.Add(move);
        }

        public int[,] GetStateAfterMove(ChessMove move)
        {
            int[,] newState = (int[,])state.Clone();

            newState[move.To.X, move.To.Y] = newState[move.From.X, move.From.Y];
            newState[move.From.X, move.From.Y] = Empty;

            if (newState[move.To.X, move.To.Y] == Pawn && move.To.Y == Columns - 1)
                newState[move.To.X, move.To.Y] = Queen;

            return newState;
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

        private int GameColumn(int stateColumn)
        {
            return color == ChessColor.Black ? stateColumn : Columns - stateColumn;
        }

        private int GameRow(int stateRow)
        {
            return color == ChessColor.Black ? stateRow : Rows - stateRow;
        }

        private void GenerateMoves(int statePiece, int column, int row)
        {
            switch (statePiece)
            {
                #region Pawn Options
                case Pawn:
                    {
                        // if on row 1, pawn can move forward to row 2 or row 3
                        if (row == 1)
                        {
                            if (state[column, row + 1] == Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                            if (state[column, row + 1] == Empty && state[column, row + 2] == Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 2)));
                        }
                        // if greater than row 1, pawn can move forward to row + 1
                        else if (row > 1)
                        {
                            if (state[column, row + 1] == Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                        }
                        // if there is an opponent piece on either [row + 1, column - 1] or [row + 1, column + 1], black can kill that piece
                        if (column + 1 < Columns && row + 1 < Rows && state[column + 1, row + 1] < Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                        }
                        if (column - 1 >= 0 && row + 1 < Rows && state[column - 1, row + 1] < Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                        }
                        // if pawn is on row 6, moving to row 7 will turn it into a queen (or any other valid piece it wants to be besides king or pawn)
                        break;
                    }
                #endregion
                #region Knight Options
                case Knight:
                    {
                        // down 2 right 1
                        if (row < Rows - 2 && column < Columns - 1 && !(state[column + 1, row + 2] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 2)));
                        // down 2 left 1
                        if (row < Rows - 2 && column > 0 && !(state[column - 1, row + 2] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 2)));
                        // up 2 right 1
                        if (row > 1 && column < Columns - 1 && !(state[column + 1, row - 2] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 2)));
                        // up 2 left 1
                        if (row > 1 && column > 0 && !(state[column - 1, row - 2] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 2)));
                        // down 1 right 2
                        if (row < Rows - 1 && column < Columns - 2 && !(state[column + 2, row + 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row + 1)));
                        // down 1 left 2
                        if (row < Rows - 1 && column > 1 && !(state[column - 2, row + 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row + 1)));
                        // up 1 right 2
                        if (row > 0 && column < Columns - 2 && !(state[column + 2, row - 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row - 1)));
                        // up 1 left 2
                        if (row > 0 && column > 1 && !(state[column - 2, row - 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row - 1)));

                        break;
                    }
                #endregion
                #region Bishop Options
                case Bishop:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Rook Options
                case Rook:
                    {
                        bool keepgoing = true;
                        // Possible rook moves to the right of the board
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Queen Options
                case Queen:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Empty)
                                keepgoing = false;
                        }


                        // Possible rook moves to the right of the board
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region King Options
                case King:
                    {
                        // Check if the king is on the edge of the board or if a piece of the same color is next to him
                        bool down = true, up = true, right = true, left = true;
                        if (!(row + 1 < Rows))
                            down = false;
                        if (!(row - 1 >= 0))
                            up = false;
                        if (column + 1 >= Columns)
                            right = false;
                        if (column - 1 <= 0)
                            left = false;

                        if (down && !(state[column, row + 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                        if (up && !(state[column, row - 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                        if (right && !(state[column + 1, row] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row)));
                        if (left && !(state[column - 1, row] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row)));
                        if (right && down && !(state[column + 1, row + 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                        if (right && up && !(state[column + 1, row - 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)));
                        if (left && down && !(state[column - 1, row + 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                        if (left && up && !(state[column - 1, row - 1] > Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)));
                        break;
                    }
                #endregion
            }
        }
    }
}