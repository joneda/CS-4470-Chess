/******************************************************************************
* The MIT License
* Copyright (c) 2008 Rusty Howell, Thomas Wiest
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

// Authors:
// 		Thomas Wiest  twiest@users.sourceforge.net
//		Rusty Howell  rhowell@users.sourceforge.net

using System;
using System.Collections.Generic;
using System.Text;
using UvsChess;

namespace ExampleAI
{
    public class ExampleAI : IChessAI
    {
        #region IChessAI Members that are implemented by the Student

        /// <summary>
        /// The name of your AI
        /// </summary>
        public string Name
        {
#if DEBUG
            get { return "ExampleAI (Debug)"; }
#else
            get { return "ExampleAI"; }
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
            //For more information about using the profiler, visit http://code.google.com/p/uvschess/wiki/ProfilerHowTo
            // This is how you setup the profiler. This should be done in GetNextMove.
            Profiler.TagNames = Enum.GetNames(typeof(ExampleAIProfilerTags));

            // In order for the profiler to calculate the AI's nodes/sec value,
            // I need to tell the profiler which method key's are for MiniMax.
            // In this case our mini and max methods are the same,
            // but usually they are not.
            Profiler.MinisProfilerTag = (int)ExampleAIProfilerTags.GetAllMoves;
            Profiler.MaxsProfilerTag = (int)ExampleAIProfilerTags.GetAllMoves;

            // This increments the method call count by 1 for GetNextMove in the profiler
            Profiler.IncrementTagCount((int)ExampleAIProfilerTags.GetNextMove);
#endif

            ChessMove myNextMove = null;

            while (! IsMyTurnOver())
            {
                if (myNextMove == null)
                {
                    myNextMove = MoveAPawn(board, myColor);
                    this.Log(myColor.ToString() + " (" + this.Name + ") just moved.");
                    this.Log(string.Empty);

                    // Since I have a move, break out of loop
                    break;
                }
            }

            Profiler.SetDepthReachedDuringThisTurn(2);
            return myNextMove;
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
#if DEBUG
            Profiler.IncrementTagCount((int)ExampleAIProfilerTags.IsValidMove);
#endif
            // This AI isn't sophisticated enough to validate moves, therefore
            // just tell UvsChess that all moves are valid.
            return true;
        }

        #endregion

        #region My AI Logic
        /// <summary>
        /// This method generates a ChessMove to move a pawn.
        /// </summary>
        /// <param name="currentBoard">This is the current board to generate the move for.</param>
        /// <param name="myColor">This is the color of the player that should generate the move.</param>
        /// <returns>A chess move.</returns>
        ChessMove MoveAPawn(ChessBoard currentBoard, ChessColor myColor)
        {
#if DEBUG
            Profiler.IncrementTagCount((int)ExampleAIProfilerTags.MoveAPawn);
#endif
            List<ChessMove> allMyMoves = GetAllMoves(currentBoard, myColor);
            ChessMove myChosenMove = null;
            Random random = new Random();
            int myChosenMoveNumber = random.Next(allMyMoves.Count);

            if (allMyMoves.Count == 0)
            {
                // If I couldn't find a valid move easily, 
                // I'll just create an empty move and flag a stalemate.
                myChosenMove = new ChessMove(null, null);
                myChosenMove.Flag = ChessFlag.Stalemate;
            }
            else
            {
                myChosenMove = allMyMoves[myChosenMoveNumber];               

                AddAllPossibleMovesToDecisionTree(allMyMoves, myChosenMove, currentBoard.Clone(), myColor);
            }

            return myChosenMove;
        }

        /// <summary>
        /// This method generates all valid moves for myColor based on the currentBoard
        /// </summary>
        /// <param name="currentBoard">This is the current board to generate the moves for.</param>
        /// <param name="myColor">This is the color of the player to generate the moves for.</param>
        /// <returns>List of ChessMoves</returns>
        List<ChessMove> GetAllMoves(ChessBoard currentBoard, ChessColor myColor)
        {
#if DEBUG
            Profiler.IncrementTagCount((int)ExampleAIProfilerTags.GetAllMoves);
#endif
            // This method only generates moves for pawns to move one space forward.
            // It does not generate moves for any other pieces.
            List<ChessMove> allMoves = new List<ChessMove>();

            // Got through the entire board one tile at a time looking for pawns I can move
            for (int Y = 1; Y < ChessBoard.NumberOfRows - 1; Y++)
            {
                for (int X = 0; X < ChessBoard.NumberOfColumns; X++)
                {
                    if (myColor == ChessColor.White)
                    {                        
                        if ((currentBoard[X, Y-1] == ChessPiece.Empty) &&
                            (currentBoard[X, Y] == ChessPiece.WhitePawn))
                        {
                            // Generate a move to move my pawn 1 tile forward
                            allMoves.Add(new ChessMove(new ChessLocation(X, Y), new ChessLocation(X, Y - 1)));
                        }
                    }
                    else // myColor is black
                    {
                        if ((currentBoard[X, Y+1] == ChessPiece.Empty) &&
                            (currentBoard[X, Y] == ChessPiece.BlackPawn))
                        {
                            // Generate a move to move my pawn 1 tile forward
                            allMoves.Add(new ChessMove(new ChessLocation(X, Y), new ChessLocation(X, Y + 1)));
                        }
                    }
                }
            }

            return allMoves;
        }

        public void AddAllPossibleMovesToDecisionTree(List<ChessMove> allMyMoves, ChessMove myChosenMove, 
                                                      ChessBoard currentBoard, ChessColor myColor)
        {
#if DEBUG
            Profiler.IncrementTagCount((int)ExampleAIProfilerTags.AddAllPossibleMovesToDecisionTree);
#endif
            Random random = new Random();

            // Create the decision tree object
            DecisionTree dt = new DecisionTree(currentBoard);

            // Tell UvsChess about the decision tree object
            SetDecisionTree(dt);
            dt.BestChildMove = myChosenMove;

            // Go through all of my moves, add them to the decision tree
            // Then go through each of these moves and generate all of my
            // opponents moves and add those to the decision tree as well.
            for (int ix = 0; ix < allMyMoves.Count; ix++)
            {
                ChessMove myCurMove = allMyMoves[ix];
                ChessBoard boardAfterMyCurMove = currentBoard.Clone();
                boardAfterMyCurMove.MakeMove(myCurMove);

                // Add the new move and board to the decision tree
                dt.AddChild(boardAfterMyCurMove, myCurMove);

                // Descend the decision tree to the last child added so we can 
                // add all of the opponents response moves to our move.
                dt = dt.LastChild;

                // Get all of the opponents response moves to my move
                ChessColor oppColor = (myColor == ChessColor.White ? ChessColor.Black : ChessColor.White);
                List<ChessMove> allOppMoves = GetAllMoves(boardAfterMyCurMove, oppColor);

                // Go through all of my opponent moves and add them to the decision tree
                foreach (ChessMove oppCurMove in allOppMoves)
                {
                    ChessBoard boardAfterOppCurMove = boardAfterMyCurMove.Clone();
                    boardAfterOppCurMove.MakeMove(oppCurMove);
                    dt.AddChild(boardAfterOppCurMove, oppCurMove);

                    // Setting all of the opponents eventual move values to 0 (see below).
                    dt.LastChild.EventualMoveValue = "0";
                }

                if (allOppMoves.Count > 0)
                {
                    // Tell the decision tree which move we think our opponent will choose.
                    int chosenOppMoveNumber = random.Next(allOppMoves.Count);
                    dt.BestChildMove = allOppMoves[chosenOppMoveNumber];
                }

                // Tell the decision tree what this moves eventual value will be.
                // Since this AI can't evaulate anything, I'm just going to set this
                // value to 0.
                dt.EventualMoveValue = "0";

                // All of the opponents response moves have been added to this childs move, 
                // so return to the parent so we can do the loop again for our next move.
                dt = dt.Parent;
            }
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

#if DEBUG
        private enum ExampleAIProfilerTags
        {
            AddAllPossibleMovesToDecisionTree,
            GetAllMoves,
            GetNextMove,
            IsValidMove,
            MoveAPawn
        }
#endif
    }
}
