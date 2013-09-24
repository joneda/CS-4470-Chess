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

namespace UvsChess
{
    /// <summary>
    /// This is the interface that your AI must implement in order to be used by the chess framework.
    /// The framework will call these methods to signal your AI to do work.
    /// </summary>
    public interface IChessAI
    {
        /// <summary>
        /// The name of your AI
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Evaluates the chess board and decided which move to make. This is the main method of the AI.
        /// The framework will call this method when it's your turn.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="yourColor">Your color</param>
        /// <returns> Returns the best chess move the player has for the given chess board</returns>
        ChessMove GetNextMove(ChessBoard board, ChessColor yourColor);

        /// <summary>
        /// Validates a move. The framework uses this to validate the opponents move.
        /// </summary>
        /// <param name="boardBeforeMove">The board as it currently is _before_ the move.</param>
        /// <param name="moveToCheck">This is the move that needs to be checked to see if it's valid.</param>
        /// <param name="colorOfPlayerMoving">This is the color of the player who's making the move.</param>
        /// <returns>Returns true if the move was valid</returns>
        bool IsValidMove(ChessBoard boardBeforeMove, ChessMove moveToCheck, ChessColor colorOfPlayerMoving);

        #region These should be implemented as automatic properties and should NEVER be touched by students.
        /// <summary>
        /// This will return true when the framework starts running your AI. When the AI's time has run out,
        /// the this method will return false. Once this method returns false, your AI should return a 
        /// move immediately.
        /// 
        /// You should NEVER EVER set this property!
        /// IsRunning should be defined as an Automatic Property.
        /// IsRunning SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        AIIsMyTurnOverCallback IsMyTurnOver
        {
            get;
            set;
        }

        /// <summary>
        /// Call this method to print out debug information. The framework subscribes to this event
        /// and will provide a log window for your debug messages.
        /// 
        /// You should NEVER EVER set this property!
        /// Log should be defined as an Automatic Property.
        /// Log SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        AILoggerCallback Log
        {
            get;
            set;
        }

        /// <summary>
        /// Call this method to catch profiling information. The framework subscribes to this event
        /// and will print out the profiling stats in your log window.
        /// 
        /// You should NEVER EVER set this property!
        /// Profile should be defined as an Automatic Property.
        /// Profile SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="key"></param>
        AIProfiler Profiler
        {
            get;
            set;
        }

        /// <summary>
        /// Call this method to tell the framework what decision print out debug information. The framework subscribes to this event
        /// and will provide a debug window for your decision tree.
        /// 
        /// You should NEVER EVER set this property!
        /// SetDecisionTree should be defined as an Automatic Property.
        /// SetDecisionTree SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        AISetDecisionTreeCallback SetDecisionTree
        {
            get;
            set;
        }
        #endregion
    }

    #region Delegates for the interface
    /// <summary>
    /// This is the delegate that defines the Log callback. You should not need to use this delegate. This is
    /// here solely for the Log property to use as it's return value.
    /// </summary>
    /// <param name="text">This is a string parameter that the Log callback uses to pass the message to the framework.</param>
    public delegate void AILoggerCallback(string message);

    /// <summary>
    /// This is the delegate that defines the IsMyTurnOver callback. You should not need to use this delegate. This is
    /// here solely for the IsMyTurnOver property to use as it's return value.
    /// </summary>
    public delegate bool AIIsMyTurnOverCallback();

    /// <summary>
    /// This is the delegate that defines the SetDecisionTree callback. You should not need to use this delegate. This is
    /// here solely for the SetDecisionTree property to use as it's return value.
    /// </summary>
    /// <param name="dt"></param>
    public delegate void AISetDecisionTreeCallback(UvsChess.DecisionTree dt);
    #endregion
}
