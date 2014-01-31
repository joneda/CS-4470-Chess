using System;
using System.Collections.Generic;
using UvsChess;

namespace EnPassantAI
{
    /// <summary>
    /// Generates the list of all possible moves that can be made by the friendly pieces on a ChessState
    /// board layout
    /// </summary>
    internal class MoveGenerator
    {
        /// <summary>
        /// This must be overridden to get log messages. Default is to follow the null-object pattern (kind of)
        /// </summary>
        private Action<string> log = (message) => { };

        public MoveGenerator(Action<string> logMethod)
        {
            log = logMethod;
        }

        /// <summary>
        /// Static method that will return a list of all the possible moves.
        /// </summary>
        /// <param name="state">The current board state in the ChessState (friend/foe) board style</param>
        /// <param name="calculateCheckMate">Checkmate will only be calculated if this is true</param>
        /// <param name="boardEvaluator">The heuristc method to use in determining move value</param>
        public List<ChessMove> GetAllMoves(int[,] state, bool calculateCheckMate, Func<int[,], ChessMove, int> boardEvaluator)
        {
            List<ChessMove> AllPossibleMoves = new List<ChessMove>();
            int Columns = state.GetLength(0);
            int Rows = state.GetLength(1);
            DetermineMoves(state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
            return AllPossibleMoves;
        }

        /// <summary>
        /// Returns the state after making the specified move
        /// </summary>
        /// <param name="state">The state before making the move</param>
        /// <param name="move">The move to make</param>
        /// <returns>The state after the move</returns>
        public int[,] GetStateAfterMove(int[,] state, ChessMove move)
        {
            int[,] newState = (int[,])state.Clone();

            newState[move.To.X, move.To.Y] = newState[move.From.X, move.From.Y];
            newState[move.From.X, move.From.Y] = SimpleState.Empty;

            if (newState[move.To.X, move.To.Y] == SimpleState.Pawn && move.To.Y == state.GetLength(1) - 1)
                newState[move.To.X, move.To.Y] = SimpleState.Queen;

            return newState;
        }

        /// <summary>
        /// Flips board direction and piece values so that the foe is represented as the friend
        /// and vice versa.
        /// </summary>
        /// <param name="state">The state to flip</param>
        /// <returns>The enemy version of the state</returns>
        public int[,] GetEnemyState(int[,] state)
        {
            int Columns = state.GetLength(0);
            int Rows = state.GetLength(1);

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

        /// <summary>
        /// Determines if the specified piece is in danger of being killed by any enemy pieces.
        /// </summary>
        /// <param name="stateToCheck">The current state of the board</param>
        /// <param name="pieceLocation">The location of the piece to check check if it is in danger</param>
        /// <returns>True if the piece can be killed by any enemy pieces, otherwise false</returns>
        public bool InDanger(int[,] stateToCheck, ChessLocation pieceLocation)
        {
            int columnCount = stateToCheck.GetLength(0);
            int rowCount = stateToCheck.GetLength(1);

            bool enemy = stateToCheck[pieceLocation.X, pieceLocation.Y] < SimpleState.Empty;
            int multiplier = enemy ? -1 : 1;

            int attackingPawn = multiplier * -SimpleState.Pawn;
            int attackingKnight = multiplier * -SimpleState.Knight;
            int attackingBishop = multiplier * -SimpleState.Bishop;
            int attackingRook = multiplier * -SimpleState.Rook;
            int attackingQueen = multiplier * -SimpleState.Queen;
            int attackingKing = multiplier * -SimpleState.King;

            int col, row;

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
                col = pieceLocation.X;
                row = pieceLocation.Y;
                int up = row - distance;
                int down = row + distance;
                int left = col - distance;
                int right = col + distance;

                if (checkUp && up >= 0 && stateToCheck[col, up] != SimpleState.Empty)
                {
                    checkUp = false;
                    if (stateToCheck[col, up] == attackingQueen || stateToCheck[col, up] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, up] == attackingKing)
                        return true;
                }

                if (checkDown && down < rowCount && stateToCheck[col, down] != SimpleState.Empty)
                {
                    checkDown = false;
                    if (stateToCheck[col, down] == attackingQueen || stateToCheck[col, down] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[col, down] == attackingKing)
                        return true;
                }

                if (checkLeft && left >= 0 && stateToCheck[left, row] != SimpleState.Empty)
                {
                    checkLeft = false;
                    if (stateToCheck[left, row] == attackingQueen || stateToCheck[left, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[left, row] == attackingKing)
                        return true;
                }

                if (checkRight && right < columnCount && stateToCheck[right, row] != SimpleState.Empty)
                {
                    checkRight = false;
                    if (stateToCheck[right, row] == attackingQueen || stateToCheck[right, row] == attackingRook)
                        return true;

                    if (distance == 1 && stateToCheck[right, row] == attackingKing)
                        return true;
                }

                if (checkUpLeft && up >= 0 && left >= 0 && stateToCheck[left, up] != SimpleState.Empty)
                {
                    checkUpLeft = false;
                    if (stateToCheck[left, up] == attackingQueen || stateToCheck[left, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, up] == attackingKing || (enemy && stateToCheck[left, up] == attackingPawn)))
                        return true;
                }

                if (checkUpRight && up >= 0 && right < columnCount && stateToCheck[right, up] != SimpleState.Empty)
                {
                    checkUpRight = false;
                    if (stateToCheck[right, up] == attackingQueen || stateToCheck[right, up] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[right, up] == attackingKing || (enemy && stateToCheck[right, up] == attackingPawn)))
                        return true;
                }

                if (checkDownLeft && down < rowCount && left >= 0 && stateToCheck[left, down] != SimpleState.Empty)
                {
                    checkDownLeft = false;
                    if (stateToCheck[left, down] == attackingQueen || stateToCheck[left, down] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[left, down] == attackingKing || (!enemy && stateToCheck[left, down] == attackingPawn)))
                        return true;
                }

                if (checkDownRight && down < rowCount && right < columnCount && stateToCheck[right, down] != SimpleState.Empty)
                {
                    checkDownRight = false;
                    if (stateToCheck[right, down] == attackingQueen || stateToCheck[right, down] == attackingBishop)
                        return true;

                    if (distance == 1 && (stateToCheck[right, down] == attackingKing || (!enemy && stateToCheck[right, down] == attackingPawn)))
                        return true;
                }

                done = !(checkUp || checkDown || checkLeft || checkRight || checkUpLeft || checkUpRight || checkDownLeft || checkDownRight);
                done = done || (up < 0 && left < 0 && down >= rowCount && right >= columnCount);
                distance++;
            }

            // Check knight left up
            col = pieceLocation.X - 2;
            row = pieceLocation.Y - 1;

            if (col >= 0 && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight left down
            col = pieceLocation.X - 2;
            row = pieceLocation.Y + 1;

            if (col >= 0 && row < rowCount && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight up left
            col = pieceLocation.X - 1;
            row = pieceLocation.Y - 2;

            if (col >= 0 && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight up right
            col = pieceLocation.X + 1;
            row = pieceLocation.Y - 2;

            if (col < columnCount && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight right up
            col = pieceLocation.X + 2;
            row = pieceLocation.Y - 1;

            if (col < columnCount && row >= 0 && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight right down
            col = pieceLocation.X + 2;
            row = pieceLocation.Y + 1;

            if (col < columnCount && row < rowCount && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight down left
            col = pieceLocation.X - 1;
            row = pieceLocation.Y + 2;

            if (col >= 0 && row < rowCount && stateToCheck[col, row] == attackingKnight)
                return true;

            // Check knight down right
            col = pieceLocation.X + 1;
            row = pieceLocation.Y + 2;

            if (col < columnCount && row < rowCount && stateToCheck[col, row] == attackingKnight)
                return true;

            return false;
        }

        private void DetermineMoves(int[,] state, bool calculateCheckMate, Func<int[,], ChessMove, int> boardEvaluator, int Columns, int Rows, List<ChessMove> AllPossibleMoves)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (state[column, row] > 0)
                        GenerateMoves(state[column, row], column, row, state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                }
            }
        }        

        private void AddMove(ChessMove move, int[,] state, bool calculateCheckMate, Func<int[,], ChessMove, int> boardEvaluator, int Columns, int Rows, List<ChessMove> AllPossibleMoves)
        {
            int[,] stateAfterMove = GetStateAfterMove(state, move);

            // If we're in check, then the move is illegal
            if (InCheck(stateAfterMove, false, Columns, Rows))
                return;

            if (InCheck(stateAfterMove, true, Columns, Rows))
            {
                move.Flag = ChessFlag.Check;

                if (calculateCheckMate)
                {
                    int[,] enemyStateAfterMove = GetEnemyState(stateAfterMove);

                    // if there are no possible moves for the enemy, they are in checkmate
                    if (GetAllMoves(enemyStateAfterMove, false, null).Count < 1)
                        move.Flag = ChessFlag.Checkmate;
                }
            }

            if (boardEvaluator != null)
                move.ValueOfMove = boardEvaluator(stateAfterMove, move);

            AllPossibleMoves.Add(move);
        }

        private void GenerateMoves(int statePiece, int column, int row, int[,] state, bool calculateCheckMate, Func<int[,], ChessMove, int> boardEvaluator, int Columns, int Rows, List<ChessMove> AllPossibleMoves)
        {
            switch (statePiece)
            {
                #region Pawn Options
                case SimpleState.Pawn:
                    {
                        // if on row 1, pawn can move forward to row 2 or row 3
                        if (row == 1)
                        {
                            if (state[column, row + 1] == SimpleState.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                            if (state[column, row + 1] == SimpleState.Empty && state[column, row + 2] == SimpleState.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 2)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        }
                        // if greater than row 1, pawn can move forward to row + 1
                        else if (row > 1)
                        {
                            if (state[column, row + 1] == SimpleState.Empty)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        }
                        // if there is an opponent piece on either [row + 1, column - 1] or [row + 1, column + 1], black can kill that piece
                        if (column + 1 < Columns && row + 1 < Rows && state[column + 1, row + 1] < SimpleState.Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        }
                        if (column - 1 >= 0 && row + 1 < Rows && state[column - 1, row + 1] < SimpleState.Empty)
                        {
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        }
                        // if pawn is on row 6, moving to row 7 will turn it into a queen (or any other valid piece it wants to be besides king or pawn)
                        break;
                    }
                #endregion
                #region Knight Options
                case SimpleState.Knight:
                    {
                        // down 2 right 1
                        if (row < Rows - 2 && column < Columns - 1 && !(state[column + 1, row + 2] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 2)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // down 2 left 1
                        if (row < Rows - 2 && column > 0 && !(state[column - 1, row + 2] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 2)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // up 2 right 1
                        if (row > 1 && column < Columns - 1 && !(state[column + 1, row - 2] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 2)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // up 2 left 1
                        if (row > 1 && column > 0 && !(state[column - 1, row - 2] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 2)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // down 1 right 2
                        if (row < Rows - 1 && column < Columns - 2 && !(state[column + 2, row + 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // down 1 left 2
                        if (row < Rows - 1 && column > 1 && !(state[column - 2, row + 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // up 1 right 2
                        if (row > 0 && column < Columns - 2 && !(state[column + 2, row - 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 2, row - 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        // up 1 left 2
                        if (row > 0 && column > 1 && !(state[column - 2, row - 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 2, row - 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                        break;
                    }
                #endregion
                #region Bishop Options
                case SimpleState.Bishop:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Rook Options
                case SimpleState.Rook:
                    {
                        bool keepgoing = true;
                        // Possible rook moves to the right of the board
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region Queen Options
                case SimpleState.Queen:
                    {
                        bool keepgoing = true;
                        // Possible bishop moves to the top right of the board
                        for (int c = column + 1, r = row - 1; keepgoing && (c < Columns) && (r >= 0); c++, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom right of the board
                        keepgoing = true;
                        for (int c = column + 1, r = row + 1; keepgoing && (c < Columns) && (r < Rows); c++, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the bottom left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row + 1; (c >= 0) && (r < Rows); c--, r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible bishop moves to the top left of the board
                        keepgoing = true;
                        for (int c = column - 1, r = row - 1; (c >= 0) && (r >= 0); c--, r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, r] < SimpleState.Empty)
                                keepgoing = false;
                        }


                        // Possible rook moves to the right of the board
                        keepgoing = true;
                        for (int c = column + 1; keepgoing && c < Columns; c++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the left of the board
                        keepgoing = true;
                        for (int c = column - 1; keepgoing && c >= 0; c--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[c, row] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(c, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[c, row] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the bottom of the board
                        keepgoing = true;
                        for (int r = row + 1; keepgoing && r < Rows; r++)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        // Possible rook moves to the top of the board
                        keepgoing = true;
                        for (int r = row - 1; keepgoing && r >= 0; r--)
                        {
                            // If the potential new location is where a piece of the same color is, we can't move there
                            if (state[column, r] > SimpleState.Empty)
                                keepgoing = false;

                            if (keepgoing)
                                AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, r)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);

                            // If the new location was an enemy piece, we can't keep going beyond it
                            if (state[column, r] < SimpleState.Empty)
                                keepgoing = false;
                        }
                        break;
                    }
                #endregion
                #region King Options
                case SimpleState.King:
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

                        if (down && !(state[column, row + 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (up && !(state[column, row - 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column, row - 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (right && !(state[column + 1, row] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (left && !(state[column - 1, row] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (right && down && !(state[column + 1, row + 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (right && up && !(state[column + 1, row - 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column + 1, row - 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (left && down && !(state[column - 1, row + 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row + 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        if (left && up && !(state[column - 1, row - 1] > SimpleState.Empty))
                            AddMove(new ChessMove(new ChessLocation(column, row), new ChessLocation(column - 1, row - 1)), state, calculateCheckMate, boardEvaluator, Columns, Rows, AllPossibleMoves);
                        break;
                    }
                #endregion
            }
        }

        private ChessLocation LocatePiece(int[,] stateToCheck, int piece, int Columns, int Rows)
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

        private bool InCheck(int[,] stateToCheck, bool enemy, int Columns, int Rows)
        {
            int kingToCheck = enemy ? -SimpleState.King : SimpleState.King;

            ChessLocation kingLocation = LocatePiece(stateToCheck, kingToCheck, Columns, Rows);

            if (kingLocation == null)
            {
                log("kingLocation is null");
                return true;
            }

            return InDanger(stateToCheck, kingLocation);
        }
    }
}
