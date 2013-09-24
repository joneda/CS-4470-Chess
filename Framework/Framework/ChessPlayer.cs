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


// TODO: Go through and make sure that only the namespaces used are the ones referenced.
using System;
using System.Collections.Generic;
using System.Threading;

namespace UvsChess.Framework
{
    // Students don't need to see this class
    internal class ChessPlayer
    {
        public ChessColor Color;
        public string AIName;
        public IChessAI AI;
        public TimeSpan TimeOfLastMove = TimeSpan.MinValue;

        private bool _isValidMove = false;
        private bool _isGetNextMoveCall = false;
        private bool _isTurnOver = false;
        private bool _hasAIEndedTurn = false;
        private bool _forceAIToEndTurnEarly = false;
        private DateTime _startTime;
        private DateTime _endTime;
        private Thread _aiThread;
        private ChessMove _moveToCheck = null;
        private ChessBoard _currentBoard = null;
        private ChessMove _moveToReturn;
        private ManualResetEvent _waitForPlayerEvent = new ManualResetEvent(true);
        private ManualResetEvent _waitForTurnToEnd = new ManualResetEvent(true);
        private int Interval = 100;
        private Timer _pollAITimer;

        public string ColorAndName
        {
            get
            {
                return this.Color.ToString() + " (" + this.AIName + ")";
            }
        }

        public ChessPlayer(ChessColor color)
        {
            Color = color;
        }

        public bool IsTurnOver()
        {
            return _isTurnOver;
        }

        public bool IsHuman
        {
            get { return ((this.AIName == null) || (this.AIName == "Human")); }
        }

        public bool IsComputer
        {
            get { return !IsHuman; }
        }

        public void StartAiInTimedThread(int maxTimeToLetThreadRun)
        {
            _aiThread = new Thread(RunAiInThread);

            // NO LOGGING ALLOWED between here
            _startTime = DateTime.Now;
            _endTime = _startTime.AddMilliseconds(maxTimeToLetThreadRun);
            _aiThread.Start();
            // AND HERE because it would count against the AI's time.

            this.PollAIOnce();

            _waitForPlayerEvent.Reset();
            _waitForPlayerEvent.WaitOne();

            _aiThread = null;

            // Clean up the heap for the next player.
            // This doesn't cost the AI time 
            //(it's done after the AI's turn time has been calculated).
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public ChessMove GetNextMove(ChessBoard currentBoard)
        {
            _isGetNextMoveCall = true;

            _currentBoard = currentBoard.Clone();

            if (this.IsHuman)
            {
                _waitForPlayerEvent.Reset();
                _waitForPlayerEvent.WaitOne();
            }
            else
            {
                StartAiInTimedThread(UvsChess.Gui.Preferences.Time);
            }

            return _moveToReturn;
        }

        /// <summary>
        /// Validates a move. The framework uses this to validate the opponents move.
        /// </summary>
        /// <param name="currentBoard">The board as it currently is.</param>
        /// <param name="moveToCheck">This is the move that needs to be checked to see if it's valid.</param>
        /// <param name="colorOfPlayerMoving">This is the color of the player who's making the move.</param>
        /// <returns>Returns true if the move was valid</returns>
        public bool? IsValidMove(ChessBoard currentBoard, ChessMove moveToCheck)
        {
            _moveToReturn = null;
            _isGetNextMoveCall = false;
            _isValidMove = false;

            if (this.IsHuman)
            {
                // Humans don't check the AI's moves, so just always return true
                return true;
            }

            _currentBoard = currentBoard.Clone();
            _moveToCheck = moveToCheck.Clone();
            StartAiInTimedThread(UvsChess.Gui.Preferences.CheckMoveTimeout);
            if ((_moveToReturn != null) && (_moveToReturn.Flag == ChessFlag.AIWentOverTime))
            {
                // The AI Went over time while validating the move. Signal ChessGame of this.
                return null;
            }
                
            return _isValidMove;
        }

        public void EndTurnEarly()
        {
            if (this.IsHuman)
            {
                _waitForPlayerEvent.Set();
            }
            else if ((_aiThread != null) && (_aiThread.IsAlive))
            {
                _forceAIToEndTurnEarly = true;
                _waitForTurnToEnd.Reset();
                _waitForTurnToEnd.WaitOne();
            }
        }

        private void PollAIOnce()
        {
            _pollAITimer = new Timer(this.PollAI, null, Interval, System.Threading.Timeout.Infinite);
        }

        private void PollAI(object state)
        {
            // NO LOGGING ALLOWED between here
            if ((_forceAIToEndTurnEarly) || (_hasAIEndedTurn) || (DateTime.Now > _endTime))
            {
                this._isTurnOver = true;

                if (! _hasAIEndedTurn)
                {
                    int gracePeriod = Gui.Preferences.GracePeriod;

                    // Start the grace period timer
                    _pollAITimer = new Timer(this.GracePeriodTimer, null, gracePeriod, System.Threading.Timeout.Infinite);

                    _aiThread.Join();

                    // Turn off the grace period timer
                    _pollAITimer = new Timer(this.GracePeriodTimer, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                    _hasAIEndedTurn = true;
                }

                TimeOfLastMove = DateTime.Now.Subtract(_startTime);
                // AND HERE because it would count against the AI's time.

                _waitForPlayerEvent.Set();
                _waitForTurnToEnd.Set();
            }
            else
            {
                // poll again
                PollAIOnce();
            }
        }

        private void GracePeriodTimer(object state)
        {
            if (! _hasAIEndedTurn)
            {
                // The AI is still running, even after the grace period.
                // They've now lost!
                _moveToReturn = new ChessMove(null, null);
                _moveToReturn.Flag = ChessFlag.AIWentOverTime;
                _aiThread.Abort();
            }
        }

        public void HumanMovedPieceEvent(ChessMove move)
        {
            if (this.IsHuman)
            {
                _moveToReturn = move;
                _waitForPlayerEvent.Set();
            }
        }

        private void RunAiInThread()
        {
            // This is the only place that IsRunning should be set to true.
            this._isTurnOver = false;
            _hasAIEndedTurn = false;

            if (_isGetNextMoveCall)
            {
                _moveToReturn = this.AI.GetNextMove(_currentBoard, this.Color);
            }
            else
            {
                _isValidMove = this.AI.IsValidMove(_currentBoard, _moveToCheck, 
                    (this.Color == ChessColor.White ? ChessColor.Black : ChessColor.White));
            }

            _hasAIEndedTurn = true;
        }
    }

}
