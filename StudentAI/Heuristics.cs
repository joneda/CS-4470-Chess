using UvsChess;

namespace StudentAI
{
    internal static class Heuristics
    {
        public static int ValueOne(ChessState state)
        {
            int result = 0;
            ChessPiece[,] rawBoard = state.RawBoard;

            for (int x = 0; x < rawBoard.GetLength(0); x++)
            {
                for (int y = 0; y < rawBoard.GetLength(1); y++)
                {
                    ChessPiece piece = rawBoard[x,y];

                    if (piece == ChessPiece.Empty)
                        continue;

                    if ((state.Color == ChessColor.Black && piece < ChessPiece.Empty)
                        || (state.Color == ChessColor.White && piece > ChessPiece.Empty))
                    {
                        result++;
                    }
                    else
                    {
                        result--;
                    }
                }
            }

            return result;
        }
    }
}
