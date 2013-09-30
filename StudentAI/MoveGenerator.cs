using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    internal class MoveGenerator
    {
        private int[,] state;
        private int Columns;
        private int Rows;
        private bool calculateCheckMate;
        private Func<int[,], bool, bool, int> boardEvaluator;
        private AILoggerCallback log;

        public List<ChessMove> AllPossibleMoves = new List<ChessMove>();

        public MoveGenerator(int[,] state, bool calculateCheckMate, Func<int[,], bool, bool, int> boardEvaluator, AILoggerCallback log)
        {
            log("Move Generator: " + calculateCheckMate.ToString());
            this.state = state;
            this.calculateCheckMate = calculateCheckMate;
            this.boardEvaluator = boardEvaluator;
            this.log = log;

            Columns = state.GetLength(0);
            Rows = state.GetLength(1);

            DetermineMoves();
            log("End Move Generator: " + calculateCheckMate.ToString());
        }

        private void DetermineMoves()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (state[column, row] > 0)
                        GenerateMoves(state[column, row], column, row);
                }
            }
        }

        public int[,] GetStateAfterMove(ChessMove move)
        {
            int[,] newState = (int[,])state.Clone();

            newState[move.To.X, move.To.Y] = newState[move.From.X, move.From.Y];
            newState[move.From.X, move.From.Y] = Piece.Empty;

            if (newState[move.To.X, move.To.Y] == Piece.Pawn && move.To.Y == Columns - 1)
                newState[move.To.X, move.To.Y] = Piece.Queen;

            return newState;
        }

        private void AddMove(ChessMove move)
        {
            int[,] stateAfterMove = GetStateAfterMove(move);

            // If we're in check, then the move is illegal
            if (InCheck(stateAfterMove, false))
                return;

            log(move.ToString());
            if (InCheck(stateAfterMove, true))
            {
                log("In Check " + move.ToString());
                move.Flag = ChessFlag.Check;

                if (calculateCheckMate)
                {
                    int[,] enemyStateAfterMove = GetEnemyState(stateAfterMove);

                    MoveGenerator enemyMoveGenerator = new MoveGenerator(enemyStateAfterMove, false, null, log);

                    // if there are no possible moves for the enemy, they are in checkmate
                    if (enemyMoveGenerator.AllPossibleMoves.Count < 1)
                    {
                        log("enemy moves: " + enemyMoveGenerator.AllPossibleMoves.Count);
                        move.Flag = ChessFlag.Checkmate;
                    }
                }
            }

            if (boardEvaluator != null)
                move.ValueOfMove = boardEvaluator(stateAfterMove, move.Flag == ChessFlag.Check, move.Flag == ChessFlag.Checkmate);

            AllPossibleMoves.Add(move);
        }

        private int[,] GetEnemyState(int[,] state)
        {
            int[,] enemyState = new int[Columns, Rows];

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    enemyState[x, y] = -1 * state[Columns - x - 1, Rows - y - 1];
                }
            }

            return enemyState;
        }

        private void GenerateMoves(int statePiece, int column, int row)
        {
            switch (statePiece)
            {
                #region Pawn Options
                case Piece.Pawn:
                    {
                        // if on row 1, pawn can move forward to row 2 or row 3
                        if (row == 1)
                        {
                            if (state[column, row + 1] == Piece.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                            if (state[column, row + 1] == Piece.Empty && state[column, row + 2] == Piece.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 2)));
                        }
                        // if greater than row 1, pawn can move forward to row + 1
                        else if (row > 1)
                        {
                            if (state[column, row + 1] == Piece.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                        }
                        // if there is an opponent piece on either [row + 1, column - 1] or [row + 1, column + 1], black can kill that piece
                        if (column + 1 < Columns && row + 1 < Rows && state[column + 1, row + 1] < Piece.Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                        }
                        if (column - 1 >= 0 && row + 1 < Rows && state[column - 1, row + 1] < Piece.Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                        }
                        // if pawn is on row 6, moving to row 7 will turn it into a queen (or any other valid piece it wants to be besides king or pawn)
                        break;
                    }
                #endregion
                #region Knight Options
                case Piece.Knight:
                    {
                        // down 2 right 1
                        if (row < Rows - 2 && column < Columns - 1 && !(state[column + 1, row + 2] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 2)));
                        // down 2 left 1
                        if (row < Rows - 2 && column > 0 && !(state[column - 1, row + 2] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 2)));
                        // up 2 right 1
                        if (row > 1 && column < Columns - 1 && !(state[column + 1, row - 2] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 2)));
                        // up 2 left 1
                        if (row > 1 && column > 0 && !(state[column - 1, row - 2] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 2)));
                        // down 1 right 2
                        if (row < Rows - 1 && column < Columns - 2 && !(state[column + 2, row + 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row + 1)));
                        // down 1 left 2
                        if (row < Rows - 1 && column > 1 && !(state[column - 2, row + 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row + 1)));
                        // up 1 right 2
                        if (row > 0 && column < Columns - 2 && !(state[column + 2, row - 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row - 1)));
                        // up 1 left 2
                        if (row > 0 && column > 1 && !(state[column - 2, row - 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row - 1)));

                        break;
                    }
                #endregion
                #region Bishop Options
                case Piece.Bishop:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Rook Options
                case Piece.Rook:
                    {
                        bool keepgoing = true;
                        // Possible rook moves to the right of the board
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Queen Options
                case Piece.Queen:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < Piece.Empty)
                                keepgoing = false;
                        }


                        // Possible rook moves to the right of the board
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > Piece.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)));

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < Piece.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region King Options
                case Piece.King:
                    {
                        // Check if the king is on the edge of the board or if a piece of the same color is next to him
                        bool down = true, up = true, right = true, left = true;
                        if (row + 1 >= Rows)
                            down = false;
                        if (row - 1 < 0)
                            up = false;
                        if (column + 1 >= Columns)
                            right = false;
                        if (column - 1 < 0)
                            left = false;

                        if (down && !(state[column, row + 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)));
                        if (up && !(state[column, row - 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)));
                        if (right && !(state[column + 1, row] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row)));
                        if (left && !(state[column - 1, row] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row)));
                        if (right && down && !(state[column + 1, row + 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)));
                        if (right && up && !(state[column + 1, row - 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)));
                        if (left && down && !(state[column - 1, row + 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)));
                        if (left && up && !(state[column - 1, row - 1] > Piece.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)));
                        break;
                    }
                #endregion
            }
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

        private bool InCheck(int[,] stateToCheck, bool enemy)
        {
            int multiplier = enemy ? -1 : 1;

            int kingToCheck = multiplier * Piece.King;
            int attackingPawn = multiplier * -Piece.Pawn;
            int attackingKnight = multiplier * -Piece.Knight;
            int attackingBishop = multiplier * -Piece.Bishop;
            int attackingRook = multiplier * -Piece.Rook;
            int attackingQueen = multiplier * -Piece.Queen;
            int attackingKing = multiplier * -Piece.King;

            int col, row;
            ChessLocation kingLocation = LocatePiece(stateToCheck, kingToCheck);

            if (kingLocation == null)
            {
                log("kingLocation is null");
                return true;
            }

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

                if (checkUp && up >= 0 && stateToCheck[col, up] != Piece.Empty)
                {
                    checkUp = false;
                    if (stateToCheck[col, up] == attackingQueen || stateToCheck[col, up] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, up] == attackingKing)
                        return true;
                }

                if (checkDown && down < Rows && stateToCheck[col, down] != Piece.Empty)
                {
                    checkDown = false;
                    if (stateToCheck[col, down] == attackingQueen || stateToCheck[col, down] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, down] == attackingKing)
                        return true;
                }

                if (checkLeft && left >= 0 && stateToCheck[left, row] != Piece.Empty)
                {
                    checkLeft = false;
                    if (stateToCheck[left, row] == attackingQueen || stateToCheck[left, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[left, row] == attackingKing)
                        return true;
                }

                if (checkRight && right < Columns && stateToCheck[right, row] != Piece.Empty)
                {
                    checkRight = false;
                    if (stateToCheck[right, row] == attackingQueen || stateToCheck[right, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[right, row] == attackingKing)
                        return true;
                }

                if (checkUpLeft && up >= 0 && left >= 0 && stateToCheck[left, up] != Piece.Empty)
                {
                    checkUpLeft = false;
                    if (stateToCheck[left, up] == attackingQueen || stateToCheck[left, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, up] == attackingKing || (enemy && stateToCheck[left, up] == attackingPawn)))
                        return true;
                }

                if (checkUpRight && up >= 0 && right < Columns && stateToCheck[right, up] != Piece.Empty)
                {
                    checkUpRight = false;
                    if (stateToCheck[right, up] == attackingQueen || stateToCheck[right, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[right, up] == attackingKing || (enemy && stateToCheck[right, up] == attackingPawn)))
                        return true;
                }

                if (checkDownLeft && down < Rows && left >= 0 && stateToCheck[left, down] != Piece.Empty)
                {
                    checkDownLeft = false;
                    if (stateToCheck[left, down] == attackingQueen || stateToCheck[left, down] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, down] == attackingKing || (!enemy && stateToCheck[left, down] == attackingPawn)))
                        return true;
                }

                if (checkDownRight && down < Rows && right < Columns && stateToCheck[right, down] != Piece.Empty)
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
    }
}
