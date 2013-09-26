using System;
using System.Collections.Generic;
using UvsChess;

namespace StudentAI
{
    internal class GreedySolver
    {
        private Func<ChessState, int> heuristic;
        private Func<bool> timesUp;

        public GreedySolver(Func<ChessState, int> heuristic, Func<bool> timesUp)
        {
            this.heuristic = heuristic;
            this.timesUp = timesUp;
        }

        public ChessMove GetMove(ChessState startingState)
        {
            ChessState current = null;
            Stack<ChessState> nodes = new Stack<ChessState>();
            HashSet<string> visited = new HashSet<string>();

            nodes.Push(startingState);

            while (!timesUp() && nodes.Count > 0)
            {
                current = nodes.Pop();

                if (visited.Contains(current.PartialFenBoard))
                    continue;

                visited.Add(current.PartialFenBoard);

                List<ChessState> possible = new List<ChessState>();
                foreach (ChessMove move in current.AllPossibleMoves)
                {
                    possible.Add(current.Move(move));
                }

                possible.Sort(delegate(ChessState x, ChessState y)
                {
                    if (x == null && y == null)
                        return 0;

                    if (x == null)
                        return -1;

                    if (y == null)
                        return 1;

                    return heuristic(x).CompareTo(heuristic(y));
                });


                foreach (ChessState state in possible)
                {
                    nodes.Push(state);
                }
            }

            while (current.Depth > 1)
                current = current.PreviousState;

            return current.ActionPerformed;
        }
    }
}
