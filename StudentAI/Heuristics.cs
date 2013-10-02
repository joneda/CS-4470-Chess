using UvsChess;

namespace StudentAI
{
    internal static class Heuristics
    {
        public static int SimpleAddition(int[,] state, ChessMove move)
        {
            int result = 0;

            if (move.Flag == ChessFlag.Check)
                result += 1;

            if (move.Flag == ChessFlag.Checkmate)
                result += 5000;
            
            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    result += state[x,y];
                }
            }

            return result;
        }

        public static int MoreAdvancedAddition(int[,] state, ChessMove move)
        {
            int pawn = 1, knight = 3, bishop = 3, rook = 5, queen = 9, king = 10;
            int result = 0;

            if (move.Flag == ChessFlag.Check)
            {
                if (isByKing(state))
                    result += 1;
                else
                    result += 10;
            }

            if (move.Flag == ChessFlag.Checkmate)
            {
                result += 5000;
            }
            else if (state[move.To.X, move.To.Y] == Piece.Pawn && Heuristics.shouldAttemptToQueenPawn(state))
            {
                // increment by a large number, but less than checkmate so if a checkmate move exists we don't pass it over for an attempt to queen a pawn
                result += 2000;
            }

            for (int x = 0; x < state.GetLength(0); x++)
            {
                for (int y = 0; y < state.GetLength(1); y++)
                {
                    switch (state[x, y])
                    {
                        case Piece.Pawn:
                            result += pawn;
                            break;
                        case Piece.Knight:
                            result += knight;
                            break;
                        case Piece.Bishop:
                            result += bishop;
                            break;
                        case Piece.Rook:
                            result += rook;
                            break;
                        case Piece.Queen:
                            result += queen;
                            break;
                        case Piece.King:
                            result += king;
                            break;
                        default:
                            result += state[x, y];
                            break;
                    }
                }
            }

            return result;
        }

        public static bool isByKing(int[,] state)
        {
            int row = -1, column = -1;
            for (int i = 0; i < ChessBoard.NumberOfRows; i++)
            {
                for (int j = 0; j < ChessBoard.NumberOfColumns; j++)
                {
                    if (state[j, i] == -Piece.King)
                    {
                        row = i; column = j;
                    }
                }
            }

            bool up = row + 1 < ChessBoard.NumberOfRows;
            bool down = row - 1 >= 0;
            bool right = column + 1 < ChessBoard.NumberOfColumns;
            bool left = column - 1 >= 0;


            // up
            if (up)
            {
                if (state[column, row + 1] == Piece.Rook || state[column, row + 1] == Piece.Queen)
                    return true;
            }
            // down
            if (down)
            {
                if (state[column, row - 1] == Piece.Rook || state[column, row - 1] == Piece.Queen)
                    return true;
            }
            // right
            if (right)
            {
                if (state[column + 1, row] == Piece.Rook || state[column + 1, row] == Piece.Queen)
                    return true;
            }
            // left
            if (left)
            {
                if (state[column - 1, row] == Piece.Rook || state[column - 1, row] == Piece.Queen)
                    return true;
            }

            // up right
            if (up && right)
            {
                if (state[column + 1, row + 1] == Piece.Bishop || state[column + 1, row + 1] == Piece.Queen || state[column + 1, row + 1] == Piece.Pawn)
                    return true;
            }
            // up left
            if (up && left)
            {
                if (state[column - 1, row + 1] == Piece.Bishop || state[column - 1, row + 1] == Piece.Queen || state[column - 1, row + 1] == Piece.Pawn)
                    return true;
            }
            // down right
            if (down && right)
            {
                if (state[column + 1, row - 1] == Piece.Bishop || state[column + 1, row - 1] == Piece.Queen)
                    return true;
            }
            // down left
            if (down && left)
            {
                if (state[column - 1, row - 1] == Piece.Bishop || state[column - 1, row - 1] == Piece.Queen)
                    return true;
            }

            return false;
        }

        public static bool shouldAttemptToQueenPawn(int[,] state)
        {
            int numberOfQueensAndRooks = 0;
            foreach (int p in state)
                if (p == Piece.Queen || p == Piece.Rook)
                    numberOfQueensAndRooks++;
            if (numberOfQueensAndRooks < 2)
                return true;

            return false;
        }
    }
}
