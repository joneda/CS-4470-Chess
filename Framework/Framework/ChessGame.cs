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
using System.Threading;

namespace UvsChess.Framework
{
    internal class ChessGame
    {
        public delegate void UpdatedStateDelegate(ChessState state);
        public delegate void SetGuiChessBoard_IsLockedDelegate(bool isLocked);
        public delegate void DeclareResultsDelegate(string results);
        public delegate void SetDecisionTreeDelegate(DecisionTree dt);

        string _results = string.Empty;

        DecisionTree _tmpDecisionTree = null;
        ChessPlayer _whitePlayer = null;
        ChessPlayer _blackPlayer = null;
        Thread _chessGameThread = null;
        ChessState _mainChessState = null;

        public ChessGame(ChessState state, string whitePlayerName, string blackPlayerName)
        {
            IsGameRunning = false;

            _mainChessState = state;
            _whitePlayer = new ChessPlayer(ChessColor.White);
            _blackPlayer = new ChessPlayer(ChessColor.Black);

            _whitePlayer.AIName = whitePlayerName;
            _blackPlayer.AIName = blackPlayerName;

            //Load the AI if it isn't loaded already
            LoadAI(_whitePlayer);
            LoadAI(_blackPlayer);

            Profiler.BeginGame();

            // Hook up the AI Log methods to the GUI
            if (_whitePlayer.IsComputer)
            {
                _whitePlayer.AI.Log += Logger.AddToWhitesLog;
                _whitePlayer.AI.IsMyTurnOver += _whitePlayer.IsTurnOver;                
                _whitePlayer.AI.SetDecisionTree += SetTmpLastDecisionTree;
                _whitePlayer.AI.Profiler = Profiler.WhiteProfiler;
                Profiler.WhiteProfiler.AIName = _whitePlayer.AI.Name;
            }

            if (_blackPlayer.IsComputer)
            {
                _blackPlayer.AI.Log += Logger.AddToBlacksLog;
                _blackPlayer.AI.IsMyTurnOver += _blackPlayer.IsTurnOver;                
                _blackPlayer.AI.SetDecisionTree += SetTmpLastDecisionTree;
                _blackPlayer.AI.Profiler = Profiler.BlackProfiler;
                Profiler.BlackProfiler.AIName = _blackPlayer.AI.Name;
            }
        }

        public ChessGame(string fen, string whitePlayerName, string blackPlayerName)
            : this(new ChessState(fen), whitePlayerName, blackPlayerName)
        {
        }

        public bool IsGameRunning { get; set; }
        public SetGuiChessBoard_IsLockedDelegate SetGuiChessBoard_IsLocked { get; set; }
        public SetDecisionTreeDelegate SetDecisionTree { get; set; }
        public DeclareResultsDelegate DeclareResults { get; set; }
        public UpdatedStateDelegate UpdatedState { get; set; }

        public void SetTmpLastDecisionTree(DecisionTree dt)
        {
            _tmpDecisionTree = dt;
        }

        public void StartGame()
        {
            _chessGameThread = new Thread(PlayInThread);
            _chessGameThread.Start();
        }

        public void StopGameEarly()
        {
            IsGameRunning = false;            

            _whitePlayer.EndTurnEarly();
            _blackPlayer.EndTurnEarly();

            _chessGameThread.Join();


            // Explicitly get rid of the AI and Player objects
            _whitePlayer.AI = null;
            _whitePlayer = null;
            _blackPlayer.AI = null;
            _blackPlayer = null;           
        }

        public void WhitePlayer_HumanMovedPieceEvent(ChessMove move)
        {
            _whitePlayer.HumanMovedPieceEvent(move);
        }

        public void BlackPlayer_HumanMovedPieceEvent(ChessMove move)
        {
            _blackPlayer.HumanMovedPieceEvent(move);
        }

        void PlayInThread()
        {
            //This method run in its own thread.          

            IsGameRunning = true;
            while (IsGameRunning)
            {
                if (_mainChessState.CurrentPlayerColor == ChessColor.White)
                {
                    DoNextMove(_whitePlayer, _blackPlayer);
                }
                else
                {
                    DoNextMove(_blackPlayer, _whitePlayer);
                }
                
                //Logger.Log("New chess state: " + mainChessState.ToFenBoard());
            }

            if ((DeclareResults != null) && (_results != string.Empty))
            {
                DeclareResults(_results);
            }
            Profiler.EndGame();

                        

            // Remove the AI Log methods from the GUI
            if (_whitePlayer.IsComputer)
            {
                _whitePlayer.AI.Log -= Logger.AddToWhitesLog;
                _whitePlayer.AI.IsMyTurnOver -= _whitePlayer.IsTurnOver;                
                _whitePlayer.AI.SetDecisionTree -= SetTmpLastDecisionTree;
                _whitePlayer.AI.Profiler = null;
            }

            if (_blackPlayer.IsComputer)
            {
                _blackPlayer.AI.Log -= Logger.AddToBlacksLog;
                _blackPlayer.AI.IsMyTurnOver -= _blackPlayer.IsTurnOver;                
                _blackPlayer.AI.SetDecisionTree -= SetTmpLastDecisionTree;
                _blackPlayer.AI.Profiler = null;
            }            
          
            Logger.Log("Game Over");
            //StopGame();
        }

        void DoNextMove(ChessPlayer player, ChessPlayer opponent)
        {
            ChessMove nextMove = null;
            //DateTime start = DateTime.Now;
            bool? isValidMove = false;
            ChessState newstate = null;

            if (player.IsComputer)
            {
                // Clear out the decision tree
                _tmpDecisionTree = null;

                Profiler.BeginTurn(player.Color);
                nextMove = player.GetNextMove(_mainChessState.CurrentBoard);
                Profiler.EndTurn(player.TimeOfLastMove);
                Logger.Log("Time Of " + player.ColorAndName + "'s last move: " + player.TimeOfLastMove);

                SetDecisionTree(_tmpDecisionTree);

                if (!this.IsGameRunning)
                {
                    // if we're no longer running, leave the method
                    return;
                }

                if ( (nextMove == null) || (!nextMove.IsBasicallyValid) )
                {
                    IsGameRunning = false;
                    _results = "The framework caught " + player.ColorAndName + " returning a completely invalid move, therefore " +
                              player.ColorAndName + " loses!";

                    if (nextMove == null)
                    {
                        Logger.Log(player.ColorAndName + " returned a null move object.");
                    }
                    else
                    {
                        Logger.Log(player.ColorAndName + "'s invalid move was: " + nextMove.ToString());
                    }

                    return;
                }

                if (nextMove.Flag == ChessFlag.AIWentOverTime)
                {
                    // the AI went over it's time limit.
                    IsGameRunning = false;
                    _results = player.ColorAndName + " went over the time limit to move and grace period. Total move time was: " + player.TimeOfLastMove.ToString();
                    return;
                }

                if (nextMove.Flag != ChessFlag.Stalemate)
                {
                    // The move is not a stale mate, and it's basically valid,
                    // so, let's see if the move is actually valid.
                    newstate = _mainChessState.Clone();
                    newstate.MakeMove(nextMove);

                    isValidMove = opponent.IsValidMove(newstate.PreviousBoard, newstate.PreviousMove);
                    if (!opponent.IsHuman)
                    {
                        Logger.Log("Time Of " + opponent.ColorAndName + "'s validate move: " + opponent.TimeOfLastMove);
                    }
                }
            }
            else //player is human
            {
                this.SetGuiChessBoard_IsLocked(false);

                nextMove = player.GetNextMove(_mainChessState.CurrentBoard);                

                if (!IsGameRunning)
                {
                    // if we're no longer running, leave the method
                    return;
                }

                newstate = _mainChessState.Clone();
                newstate.MakeMove(nextMove);

                isValidMove = opponent.IsValidMove(newstate.PreviousBoard, newstate.PreviousMove);
                if (!opponent.IsHuman)
                {
                    Logger.Log("Time Of " + opponent.ColorAndName + "'s validate move: " + opponent.TimeOfLastMove);
                }

                this.SetGuiChessBoard_IsLocked(true);
            }//end if player == human
                        
            if ((UpdatedState != null) && (newstate != null))
            {
                // If someone is sub'd to our update delegate, 
                // update them with the new state.
                UpdatedState(newstate);
            }

            if (isValidMove == null)
            {
                // the AI went over it's time limit.
                IsGameRunning = false;
                _results = opponent.ColorAndName + " went over the time limit to validate a move and grace period. Total move time was: " + opponent.TimeOfLastMove.ToString();
                return;
            }

            if (isValidMove == true)
            {
                _mainChessState = newstate;

                if (player.Color == ChessColor.Black)
                {
                    _mainChessState.FullMoves++;//Increment fullmoves after black's turn
                }

                //Determine if a pawn was moved or a kill was made.
                if (ResetHalfMove())
                {
                    _mainChessState.HalfMoves = 0;
                }
                else
                {
                    if (_mainChessState.HalfMoves < 50)
                    {
                        _mainChessState.HalfMoves++;
                    }
                    else
                    {
                        //end of game: 50 move rule
                        IsGameRunning = false;
                        _results = "Game is a stalemate. 50 moves were made without a kill or a pawn advancement.";
                    }
                }

                if (nextMove.Flag == ChessFlag.Check)
                {
                    Logger.Log(player.ColorAndName + " has put " + opponent.ColorAndName + " in Check!");
                }

                if (nextMove.Flag == ChessFlag.Checkmate)
                {
                    // Checkmate on a valid move has been signaled.
                    IsGameRunning = false;

                    _results = player.ColorAndName + " has signaled that the game is a checkmate _and_ " +
                              opponent.ColorAndName + " said the last move was valid.";
                }
            }
            else
            {
                // It is either a stalemate or an invalid move. Either way, we're done running.
                IsGameRunning = false;

                if (nextMove.Flag == ChessFlag.Stalemate)
                {
                    // A stalemate has occurred. Since stalemates can occur because the AI can't
                    // make a move, we don't have the other AI check their move (because it would
                    // probably just be an empty move).
                    _results = player.ColorAndName + " has signaled that the game is a stalemate.";
                }
                else
                {
                    _results = opponent.ColorAndName + " has signaled that " + player.ColorAndName + 
                              " returned an invalid move!";

                    Logger.Log(player.ColorAndName + "'s invalid move was: " + nextMove.ToString());
                }
            }
        }

        /// <summary>
        /// Loads the AI for the chess player. If the AI is already loaded, it will just return.
        /// </summary>
        /// <param name="player"></param>
        void LoadAI(ChessPlayer player)
        {
            if (player.IsHuman)
            {
                // Player is a human, so we don't need to load an AI.
                return;
            }
            else if (player.AI != null)
            {
                // AI has already been loaded, so return
                return;
            }

            AI tmpAI = null;

            foreach (AI t in DllLoader.AvailableAIs)
            {
                if (t.ShortName == player.AIName)
                {
                    tmpAI = t;
                    break;
                }
            }
            System.Reflection.Assembly assem = System.Reflection.Assembly.LoadFile(tmpAI.FileName);
            IChessAI ai = (IChessAI)assem.CreateInstance(tmpAI.FullName);
            player.AI = ai;

        }

        //This method checks if a pawn was moved or a kill was made.
        private bool ResetHalfMove()
        {
            ChessMove move = _mainChessState.PreviousMove;
            //Check for a pawn move
            ChessPiece piece = _mainChessState.PreviousBoard[move.From.X, move.From.Y];
            if ((piece == ChessPiece.WhitePawn) || (piece == ChessPiece.BlackPawn))
            {
                return true;
            }

            //Check for a kill
            piece = _mainChessState.PreviousBoard[move.To.X, move.To.Y];
            if (piece != ChessPiece.Empty)
            {
                return true;
            }

            return false;
        }
    }
}
