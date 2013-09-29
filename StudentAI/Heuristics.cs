using UvsChess;

namespace StudentAI
{
    internal static class Heuristics
    {
        public static int SimpleAddition(int[,] state)
        {
            int result = 0;
            
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
