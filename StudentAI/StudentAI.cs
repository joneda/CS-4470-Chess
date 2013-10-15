using System;
using System.Collections.Generic;
using System.Linq;
using UvsChess;

namespace StudentAI
{
    public class StudentAI : IChessAI
    {
        //private Random random;

        //private GreedySolver solver;
        private MiniMax miniMax;
        public static Queue<ChessMove> previousMoves = new Queue<ChessMove>();
        public static int duplicateMoveValue = 1;
        private static int duplicateMoveMax = 10;

        public StudentAI()
        {
            //random = new Random();
            //solver = new GreedySolver();
        }
        #region IChessAI Members that are implemented by the Student

        /// <summary>
        /// The name of your AI
        /// </summary>
        public string Name
        {
#if DEBUG
            get { return "En Passant MiniMax (Debug)"; }
#else
            get { return "En Passant"; }
#endif
        }

        /// <summary>
        /// Evaluates the chess board and decided which move to make. This is the main method of the AI.
        /// The framework will call this method when it's your turn.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="yourColor">Your color</param>
        /// <returns> Returns the best chess move the player has for the given chess board</returns>
        public ChessMove GetNextMove(ChessBoard board, ChessColor myColor)
        {
#if DEBUG
            Log("Determining Next Move");
#endif


                miniMax = new MiniMax(Log, () =>
                {
                    if (IsMyTurnOver == null)
                        return false;
                    else
                        return IsMyTurnOver();
                });

                //ChessMove move = solver.GetMove(new ChessState(board, myColor, Heuristics.MoreAdvancedAddition, Log));
                ChessMove previous = null;
                ChessMove move = null;
                int depth = 1;
                
                while (!IsMyTurnOver())
                {
                    previous = move;
                    if (Heuristics.IsEndgame(board, myColor))
                    {
                        //GreedySolver g = new GreedySolver();
                       // move = g.GetMove(new ChessState(board, myColor, Heuristics.Endgame, Log));   
                        move = miniMax.MiniMaxMove(++depth, new ChessState(board, myColor, Heuristics.Endgame, Log));
                    }
                    else
                        move = miniMax.MiniMaxMove(++depth, new ChessState(board, myColor, Heuristics.MoreAdvancedAddition, Log));
                }

                if (previousMoves.Count > duplicateMoveMax)
                {
                    previousMoves.Dequeue();
                }
                ChessMove bestMove = previous ?? move ?? new ChessMove(null, null) { Flag = ChessFlag.Stalemate };
                previousMoves.Enqueue(bestMove);
                Log("Chosen Move: " + bestMove.ToString());
                Log("Max Depth: " + depth);
                return bestMove;
        }

        /// <summary>
        /// Validates a move. The framework uses this to validate the opponents move.
        /// </summary>
        /// <param name="boardBeforeMove">The board as it currently is _before_ the move.</param>
        /// <param name="moveToCheck">This is the move that needs to be checked to see if it's valid.</param>
        /// <param name="colorOfPlayerMoving">This is the color of the player who's making the move.</param>
        /// <returns>Returns true if the move was valid</returns>
        public bool IsValidMove(ChessBoard boardBeforeMove, ChessMove moveToCheck, ChessColor colorOfPlayerMoving)
        {
            //If statlemate, siganl valid move
            if (moveToCheck.Flag == ChessFlag.Stalemate)
            {
                return true;
            }

#if DEBUG
            Log("Generating Opponent's Moves");
#endif
            ChessState state = new ChessState(boardBeforeMove, colorOfPlayerMoving, null, Log);

            foreach (ChessMove move in state.AllPossibleMoves)
            {
                if (state.GetGameMove(move) == moveToCheck)
                {
                    return true;
                }
            }

            ChessMove nearMatch = state.AllPossibleMoves
                .Select(m => state.GetGameMove(m))
                .Where(m => m.From == moveToCheck.From && m.To == moveToCheck.To).FirstOrDefault();
            
            if (nearMatch != null)
            {
                Log(string.Format("Invalid Move ({0}). Expected the {1} flag to be set.", moveToCheck, nearMatch.Flag));
            }
            else
            {
                Log(string.Format("Invalid Move ({0}). The move was not in my list of possible moves.", moveToCheck));
            }

            return false;
        }


        #endregion



















        #region IChessAI Members that should be implemented as automatic properties and should NEVER be touched by students.
        /// <summary>
        /// This will return false when the framework starts running your AI. When the AI's time has run out,
        /// then this method will return true. Once this method returns true, your AI should return a 
        /// move immediately.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        public AIIsMyTurnOverCallback IsMyTurnOver { get; set; }

        /// <summary>
        /// Call this method to print out debug information. The framework subscribes to this event
        /// and will provide a log window for your debug messages.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AILoggerCallback Log { get; set; }

        /// <summary>
        /// Call this method to catch profiling information. The framework subscribes to this event
        /// and will print out the profiling stats in your log window.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="key"></param>
        public AIProfiler Profiler { get; set; }

        /// <summary>
        /// Call this method to tell the framework what decision print out debug information. The framework subscribes to this event
        /// and will provide a debug window for your decision tree.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AISetDecisionTreeCallback SetDecisionTree { get; set; }
        #endregion
    }
}
