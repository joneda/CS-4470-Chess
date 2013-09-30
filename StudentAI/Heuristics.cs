using UvsChess;

namespace StudentAI
{
    internal static class Heuristics
    {
        public static int SimpleAddition(int[,] state, bool check, bool checkMate)
        {
            int result = 0;

            if (check)
                result += 1;

            if (checkMate)
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
    }
}
