using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UvsChess;

namespace EnPassantAI
{
    public class EnPassantAI_Greedy : IChessAI
    {
        private Random random = new Random();
         private MoveGenerator moveGenerator;
         private Heuristics heuristics;

        public EnPassantAI_Greedy()
        {
            Action<string> logAction = (message) =>
                {
                    if (Log != null)
                        Log(message);
                };

            Func<bool> timesUpCheck = () =>
                {
                    if (IsMyTurnOver == null)
                        return false;
                    else
                        return IsMyTurnOver();
                };

            moveGenerator = new MoveGenerator(logAction);
            heuristics = new Heuristics(moveGenerator);
        }
        #region IChessAI Members that are implemented by the Student

        /// <summary>
        /// The name of your AI
        /// </summary>
        public string Name
        {
#if DEBUG
            get { return "En Passant - Greedy (Debug)"; }
#else
            get { return "En Passant - Greedy"; }
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
            int[,] state = SimpleState.GetSimpleState(board, myColor);

            List<ChessMove> moves = moveGenerator.GetAllMoves(state, true, heuristics.MoreAdvancedAddition);

            if (moves.Count < 1)
                return new ChessMove(null, null) { Flag = ChessFlag.Stalemate };

            int max = moves.Max(m => m.ValueOfMove);

            ChessMove[] bestMoves = moves.Where(m => m.ValueOfMove == max).ToArray();

            return SimpleState.GetGameMove(bestMoves[random.Next(bestMoves.Length)], myColor);
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

            int[,] state = SimpleState.GetSimpleState(boardBeforeMove, colorOfPlayerMoving);
            List<ChessMove> allPossibleMoves = moveGenerator.GetAllMoves(state, true, null);

            foreach (ChessMove move in allPossibleMoves)
            {
                if (SimpleState.GetGameMove(move, colorOfPlayerMoving) == moveToCheck)
                {
                    return true;
                }
            }

            ChessMove nearMatch = allPossibleMoves
                .Select(m => SimpleState.GetGameMove(m, colorOfPlayerMoving))
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
