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
using System.IO;

namespace UvsChess.Gui
{
    internal static class GuiEventLoop
    {
        public static WinGui MainForm = null;

        private static string WhitePlayerName = string.Empty;
        private static string BlackPlayerName = string.Empty;
        
        private static int Interval = 50;
        private static object _updateGuiDataLockObject = new object();
        private static object _updateGuiLockObject = new object();
        private static object _pollGuiLockObject = new object();
        private static UvsChess.Framework.ChessGame _mainGame = null;

        private static bool _isInGameMode = false;

        private static List<GuiEvent> _guiEvents = new List<GuiEvent>();
        private static List<GuiEvent> _tmpGuiEvents = null;

        private static bool _isShuttingDown = false;
        private static UvsChess.DecisionTree _lastDecisionTree = null;

        private static bool _wasMainLogUpdated = false;
        private static bool _tmpWasMainLogUpdated = false;

        private static bool _wasWhitesLogUpdated = false;
        private static bool _tmpWasWhitesLogUpdated = false;

        private static bool _wasBlacksLogUpdated = false;
        private static bool _tmpWasBlacksLogUpdated = false; 

        private static bool _wasHistoryUpdated = false;
        private static bool _tmpWasHistoryUpdated = false;        

        private static Timer _pollGuiTimer = null;

        delegate void NoParameterCallback();
        delegate UvsChess.Framework.ChessState NoParameterChessStateReturnCallback();        
        public delegate void ObjectParameterCallback(params object[] eventArgs);

        public static void PollGuiOnce()
        {
            lock (_pollGuiLockObject)
            {
                // only poll if we're not shutting down
                if (!_isShuttingDown)
                {
                    // Run UpdateGui in <interval> ms, exactly one time. 
                    // In UpdateGui, I'll tell it to run me again, exactly once.
                    _pollGuiTimer = new Timer(UpdateGui, null, Interval, System.Threading.Timeout.Infinite);
                }
            }
        }

        public static void ShutdownGuiEventLoop()
        {
            lock (_pollGuiLockObject)
            {
                _isShuttingDown = true;

                if (_pollGuiTimer != null)
                {
                    // if there's a timer out there ready to go off, shut it down.
                    _pollGuiTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _pollGuiTimer = null;
                }

                Actually_StopGame();
            }
        }

        private static void UpdateGui(object state)
        {
            if (! _isShuttingDown)
            {
                // This should guarantee that we won't lose any data.
                lock (_updateGuiDataLockObject)
                {
                    if (_guiEvents.Count > 0)
                    {
                        _tmpGuiEvents =_guiEvents;
                        _guiEvents = new List<GuiEvent>();
                    }
                    else
                    {
                        _tmpGuiEvents = null;
                    }

                    if (_wasMainLogUpdated)
                    {
                        _tmpWasMainLogUpdated = true;
                        _wasMainLogUpdated = false;
                    }
                    else
                    {
                        _tmpWasMainLogUpdated = false;
                    }

                    if (_wasWhitesLogUpdated)
                    {
                        _tmpWasWhitesLogUpdated = true;
                        _wasWhitesLogUpdated = false;
                    }
                    else
                    {
                        _tmpWasWhitesLogUpdated = false;
                    }

                    if (_wasBlacksLogUpdated)
                    {
                        _tmpWasBlacksLogUpdated = true;
                        _wasBlacksLogUpdated = false;
                    }
                    else
                    {
                        _tmpWasBlacksLogUpdated = false;
                    }

                    if (_wasHistoryUpdated)
                    {
                        _tmpWasHistoryUpdated = true;
                        _wasHistoryUpdated = false;
                    }
                    else
                    {
                        _tmpWasHistoryUpdated = false;
                    }
                }

                // This is to make sure that if two UpdateGui timers fire at aroun the
                // same time, only 1 of them can be updating the Gui at a time.
                // This, of course, should NEVER happen.
                lock (_updateGuiLockObject)
                {
                    try
                    {
                        RunThroughAllGuiEvents();
                    }
                    catch (Exception e)
                    {
                        // this is to catch any errant exceptions that might 
                        // be thrown when we shut down (if the form is closing 
                        // and we're trying to update the gui)
                        UvsChess.Framework.Logger.Log("  Exception: " + e.Message);
                    }

                    // Setup to Poll Again in <interval> ms
                    PollGuiOnce();
                }
            }
        }

        private static void RunThroughAllGuiEvents()
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.Invoke(new NoParameterCallback(RunThroughAllGuiEvents));
            }
            else
            {
                LstBoxes_BeginUpdate();

                while ((!_isShuttingDown) && (_tmpGuiEvents != null) && (_tmpGuiEvents.Count > 0))
                {
                    GuiEvent curEvent = _tmpGuiEvents[0];

                    //if (curEvent.EventCallback == Actually_DeclareResults)
                    //{
                    //    _tmpGuiEvents.Add(curEvent);
                    //    _tmpGuiEvents.RemoveAt(0);
                    //    curEvent = _tmpGuiEvents[0];
                    //}

                    try
                    {
                        // Process the GUI Events one event at a time,
                        // _and_ in the same order that they were received.
                        curEvent.EventCallback(curEvent.EventArgs);
                    }
                    catch (Exception e)
                    {
                        // this is to catch exceptions from GuiEvents, 
                        // close enough that I can get a good stack trace.
                        UvsChess.Framework.Logger.Log("  Exception: " + e.Message);
                    }

                    _tmpGuiEvents.RemoveAt(0);
                }

                if (!_isShuttingDown)
                {
                    LstBoxes_EndUpdate();
                }
            }
        }

        private static void LstBoxes_BeginUpdate()
        {
            if (_tmpWasHistoryUpdated)
            {
                MainForm.lstHistory.BeginUpdate();
            }

            if (_tmpWasMainLogUpdated)
            {
                MainForm.lstMainLog.BeginUpdate();
            }

            if (_tmpWasWhitesLogUpdated)
            {
                MainForm.lstWhitesLog.BeginUpdate();
            }

            if (_tmpWasBlacksLogUpdated)
            {
                MainForm.lstBlacksLog.BeginUpdate();
            }
        }

        private static void LstBoxes_EndUpdate()
        {
            if (_tmpWasHistoryUpdated)
            {
                MainForm.lstHistory.EndUpdate();
            }

            if (_tmpWasMainLogUpdated)
            {
                MainForm.lstMainLog.EndUpdate();
            }

            if (_tmpWasWhitesLogUpdated)
            {
                MainForm.lstWhitesLog.EndUpdate();
            }

            if (_tmpWasBlacksLogUpdated)
            {
                MainForm.lstBlacksLog.EndUpdate();
            }
        }

        public static void SetDecisionTree(UvsChess.DecisionTree dt)
        {
            lock (_updateGuiDataLockObject)
            {
                _lastDecisionTree = dt;
            }
        }

        public static void ShowDecisionTree()
        {
            lock (_updateGuiDataLockObject)
            {
                if (GuiEventLoop._lastDecisionTree == null)
                {
                    _guiEvents.Add(new GuiEvent(Actually_ShowDecisionTree, null));
                }
                else
                {
                    _guiEvents.Add(new GuiEvent(Actually_ShowDecisionTree, GuiEventLoop._lastDecisionTree.Clone()));
                }
            }
        }

        private static void Actually_ShowDecisionTree(params object[] eventArgs)
        {
            if ((_isInGameMode) && 
                (MainForm.cmbWhite.SelectedItem.ToString() != "Human") && 
                (MainForm.cmbBlack.SelectedItem.ToString() != "Human"))
            {
                MessageBox.Show("To view the Decision Tree, at least 1 player must be human.");
            }
            else if ((eventArgs == null) || (eventArgs.Length == 0))
            {
                MessageBox.Show("The DecisionTree object is set to Null");
            }            
            else
            {
                GuiDecisionTree gdt = new GuiDecisionTree((UvsChess.DecisionTree)eventArgs[0]);
                gdt.ShowDialog(MainForm);
            }
        }

        public static void ResetHistory(UvsChess.Framework.ChessState newState)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_ResetHistory, newState.Clone()));
            }
        }

        private static void Actually_ResetHistory(params object[] eventArgs)
        {
            UvsChess.Framework.ChessState newState = (UvsChess.Framework.ChessState)eventArgs[0];

            MainForm.lstHistory.Items.Clear();
            Actually_AddToHistory(new object[] {newState});
        }

        public static void AddToHistory(UvsChess.Framework.ChessState state)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_AddToHistory, state.Clone()));
                _wasHistoryUpdated = true;
            }
        }

        private static void Actually_AddToHistory(params object[] eventArgs)
        {
            UvsChess.Framework.ChessState state = (UvsChess.Framework.ChessState)eventArgs[0];

            MainForm.lstHistory.Items.Add(state);
            MainForm.lstHistory.SelectedIndex = MainForm.lstHistory.Items.Count - 1;
        }

        public static void SetGuiChessBoard_IsLocked(bool isLocked)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_SetGuiChessBoard_IsLocked, isLocked));
            }
        }

        private static void Actually_SetGuiChessBoard_IsLocked(params object[] eventArgs)
        {
            bool isLocked = (bool)eventArgs[0];
            MainForm.chessBoardControl.IsLocked = isLocked;
        }

        public static void DeclareResults(string results)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_DeclareResults, results));
            }
        }

        private static void Actually_DeclareResults(params object[] eventArgs)
        {
            string result = (string)eventArgs[0];
            Actually_StopGame();
            MessageBox.Show(result);
            Actually_AddToMainLog(new string[] {result});
        }

        public static void AddToMainLog(string message)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_AddToMainLog, message));
                _wasMainLogUpdated = true;
            }
        }

        private static void Actually_AddToMainLog(params object[] eventArgs)
        {
            string message = (string)eventArgs[0];
            MainForm.lstMainLog.Items.Add(message);

            // It's ok to call MainLog.Checked here since this is an
            // "up to the minute" operation
            if (MainForm.chkBxAutoScrollMainLog.Checked)
            {
                MainForm.lstMainLog.SelectedIndex = MainForm.lstMainLog.Items.Count - 1;
                MainForm.lstMainLog.ClearSelected();
            }
        }

        public static void AddToWhitesLog(string message)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_AddToWhitesLog, message));
                _wasWhitesLogUpdated = true;
            }
        }

        private static void Actually_AddToWhitesLog(params object[] eventArgs)
        {
            string message = (string)eventArgs[0];
            MainForm.lstWhitesLog.Items.Add(message);

            // It's ok to call MainLog.Checked here since this is an
            // "up to the minute" operation
            if (MainForm.chkBxAutoScrollWhitesLog.Checked)
            {
                MainForm.lstWhitesLog.SelectedIndex = MainForm.lstWhitesLog.Items.Count - 1;
                MainForm.lstWhitesLog.ClearSelected();
            }
        }

        public static void AddToBlacksLog(string message)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_AddToBlacksLog, message));
                _wasBlacksLogUpdated = true;
            }
        }

        private static void Actually_AddToBlacksLog(params object[] eventArgs)
        {
            string message = (string)eventArgs[0];
            MainForm.lstBlacksLog.Items.Add(message);

            // It's ok to call MainLog.Checked here since this is an
            // "up to the minute" operation
            if (MainForm.chkBxAutoScrollBlacksLog.Checked)
            {
                MainForm.lstBlacksLog.SelectedIndex = MainForm.lstBlacksLog.Items.Count - 1;
                MainForm.lstBlacksLog.ClearSelected();
            }
        }

        public static void StartGame()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_StartGame));
            }
        }

        private static void Actually_StartGame(params object[] eventArgs)
        {
            if (! _isInGameMode)
            {
                _isInGameMode = true;

                UvsChess.Framework.ChessState item = Actually_GetUpdated_LstHistory_SelectedItem();                

                _mainGame = new UvsChess.Framework.ChessGame(item, GuiEventLoop.WhitePlayerName, GuiEventLoop.BlackPlayerName);

                // Remove WinGui from the GuiChessBoard updates
                MainForm.chessBoardControl.PieceMovedByHuman -= MainForm.PieceMovedByHuman_Changed;

                // Add the ChessGame to the GuiChessBoard updates
                MainForm.chessBoardControl.PieceMovedByHuman += _mainGame.WhitePlayer_HumanMovedPieceEvent;
                MainForm.chessBoardControl.PieceMovedByHuman += _mainGame.BlackPlayer_HumanMovedPieceEvent;

                // Add WinGui to the ChessGame updates event
                _mainGame.UpdatedState += GuiEventLoop.AddToHistory;

                _mainGame.SetGuiChessBoard_IsLocked += GuiEventLoop.SetGuiChessBoard_IsLocked;

                // Add WinGui to the DeclareResults event
                _mainGame.DeclareResults += GuiEventLoop.DeclareResults;

                _mainGame.SetDecisionTree += GuiEventLoop.SetDecisionTree;

                Actually_RemoveHistoryAfterSelected();

                Actually_DisableMenuItemsDuringPlay();
                Actually_DisableRadioBtnsAndComboBoxes();
                Actually_DisableHistoryWindowClicking();
                GuiEventLoop.Actually_SetGuiChessBoard_IsLocked(new object[] { true });

                _mainGame.StartGame();
            }
        }

        public static void StopGame()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_StopGame));
            }
        }

        private static void Actually_StopGame(params object[] eventArgs)
        {
            if (_isInGameMode)
            {
                _isInGameMode = false;

                _mainGame.StopGameEarly();

                // Remove the ChessGame from the GuiChessBoard updates
                MainForm.chessBoardControl.PieceMovedByHuman -= _mainGame.WhitePlayer_HumanMovedPieceEvent;
                MainForm.chessBoardControl.PieceMovedByHuman -= _mainGame.BlackPlayer_HumanMovedPieceEvent;

                // Remove WinGui from the ChessGame updates event
                //_mainGame.Updated -= OnChessGameUpdated;
                _mainGame.UpdatedState -= GuiEventLoop.AddToHistory;

                _mainGame.SetGuiChessBoard_IsLocked -= GuiEventLoop.SetGuiChessBoard_IsLocked;

                // Remove WinGui from the DeclareResults event
                _mainGame.DeclareResults -= GuiEventLoop.DeclareResults;

                _mainGame.SetDecisionTree -= GuiEventLoop.SetDecisionTree;

                // Add WinGui to the GuiChessBoard updates
                MainForm.chessBoardControl.PieceMovedByHuman += MainForm.PieceMovedByHuman_Changed;

                Actually_EnableMenuItemsAfterPlay();
                Actually_EnableRadioBtnsAndComboBoxes();
                Actually_EnableHistoryWindowClicking();
                GuiEventLoop.Actually_SetGuiChessBoard_IsLocked(new object[] { false });

                _mainGame = null;                
            }
        }

        private static void Actually_RemoveHistoryAfterSelected()
        {
            int sel = MainForm.lstHistory.SelectedIndex;

            MainForm.lstHistory.BeginUpdate();
            while (MainForm.lstHistory.Items.Count > sel + 1)
            {
                MainForm.lstHistory.Items.RemoveAt(MainForm.lstHistory.Items.Count - 1);
            }
            MainForm.lstHistory.EndUpdate();
        }

        private static void Actually_DisableMenuItemsDuringPlay()
        {
            MainForm.startToolStripMenuItem.Enabled = false;
            MainForm.clearHistoryToolStripMenuItem.Enabled = false;
            MainForm.newToolStripMenuItem.Enabled = false;

            MainForm.openToolStripMenuItem.Enabled = false;
            MainForm.saveToolStripMenuItem.Enabled = false;

            MainForm.stopToolStripMenuItem.Enabled = true;
        }

        private static void Actually_DisableRadioBtnsAndComboBoxes()
        {
            MainForm.radBlack.Enabled = false;
            MainForm.radWhite.Enabled = false;
            MainForm.cmbBlack.Enabled = false;
            MainForm.cmbWhite.Enabled = false;
            MainForm.numFullMoves.Enabled = false;
            MainForm.numHalfMoves.Enabled = false;
        }

        private static void Actually_DisableHistoryWindowClicking()
        {
            MainForm.lstHistory.Enabled = false;
        }

        private static void Actually_EnableMenuItemsAfterPlay()
        {
            MainForm.startToolStripMenuItem.Enabled = true;
            MainForm.clearHistoryToolStripMenuItem.Enabled = true;
            MainForm.newToolStripMenuItem.Enabled = true;

            MainForm.openToolStripMenuItem.Enabled = true;
            MainForm.saveToolStripMenuItem.Enabled = true;

            MainForm.stopToolStripMenuItem.Enabled = false;
        }

        private static void Actually_EnableRadioBtnsAndComboBoxes()
        {
            MainForm.radBlack.Enabled = true;
            MainForm.radWhite.Enabled = true;
            MainForm.cmbBlack.Enabled = true;
            MainForm.cmbWhite.Enabled = true;
            MainForm.numFullMoves.Enabled = true;
            MainForm.numHalfMoves.Enabled = true;
        }

        private static void Actually_EnableHistoryWindowClicking()
        {
            MainForm.lstHistory.Enabled = true;
        }

        private static void SaveLogToDisk(string message, System.Windows.Forms.ListBox lstBox)
        {
            MainForm.saveFileDialog1.Filter = "Text Files (*.txt) | *.txt";
            System.Windows.Forms.DialogResult result = MainForm.saveFileDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(MainForm.saveFileDialog1.FileName);

                AddToMainLog(message + MainForm.saveFileDialog1.FileName);

                foreach (string curLine in lstBox.Items)
                {
                    writer.WriteLine(curLine);
                }

                writer.Close();
            }
        }

        public static void SaveMainLogToDisk()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_SaveMainLogToDisk));
            }
        }

        private static void Actually_SaveMainLogToDisk(params object[] eventArgs)
        {
            SaveLogToDisk("Saving Main Log to: ", MainForm.lstMainLog);
        }

        public static void SaveWhitesLogToDisk()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_SaveWhitesLogToDisk));
            }
        }

        private static void Actually_SaveWhitesLogToDisk(params object[] eventArgs)
        {
            SaveLogToDisk("Saving White AI's Log to: ", MainForm.lstWhitesLog);
        }

        public static void SaveBlacksLogToDisk()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_SaveBlacksLogToDisk));
            }
        }

        private static void Actually_SaveBlacksLogToDisk(params object[] eventArgs)
        {
            SaveLogToDisk("Saving Black AI's Log to: ", MainForm.lstBlacksLog);
        }

        public static void OpenStateFromDisk()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_OpenStateFromDisk));
            }
        }

        private static void Actually_OpenStateFromDisk(params object[] eventArgs)
        {
            MainForm.openFileDialog1.Filter = "FEN files (*.fen)|*.fen|All files (*.*)| *.*";
            System.Windows.Forms.DialogResult result = MainForm.openFileDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                AddToMainLog("Resetting chess board");
                StreamReader reader = new StreamReader(MainForm.openFileDialog1.FileName);
                string line = reader.ReadLine();

                ResetHistory(new UvsChess.Framework.ChessState(line));

                reader.Close();
            }
        }

        public static void SaveSelectedStateToDisk()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_SaveSelectedStateToDisk));
            }
        }

        private static void Actually_SaveSelectedStateToDisk(params object[] eventArgs)
        {
            MainForm.saveFileDialog1.Filter = "FEN files (*.fen) | *.fen";
            System.Windows.Forms.DialogResult result = MainForm.saveFileDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(MainForm.saveFileDialog1.FileName);

                AddToMainLog("Saving board to: " + MainForm.saveFileDialog1.FileName);


                UvsChess.Framework.ChessState item = Actually_GetUpdated_LstHistory_SelectedItem();
                string fenboard = item.ToFenBoard();

                writer.WriteLine(fenboard);
                writer.Close();
            }
        }

        public static void ClearMainLog()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_ClearMainLog));
            }
        }

        private static void Actually_ClearMainLog(params object[] eventArgs)
        {
            MainForm.lstMainLog.Items.Clear();
        }

        public static void ClearWhitesLog()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_ClearWhitesLog));
            }
        }

        private static void Actually_ClearWhitesLog(params object[] eventArgs)
        {
            MainForm.lstWhitesLog.Items.Clear();
        }

        public static void ClearBlacksLog()
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_ClearBlacksLog));
            }
        }

        private static void Actually_ClearBlacksLog(params object[] eventArgs)
        {
            MainForm.lstBlacksLog.Items.Clear();
        }

        private static UvsChess.Framework.ChessState Actually_GetUpdated_LstHistory_SelectedItem()
        {
            UvsChess.Framework.ChessState selectedState = (UvsChess.Framework.ChessState)MainForm.lstHistory.SelectedItem;
            if (MainForm.radWhite.Checked)
            {
                selectedState.CurrentPlayerColor = ChessColor.White;
            }
            else
            {
                selectedState.CurrentPlayerColor = ChessColor.Black;
            }

            selectedState.HalfMoves = Convert.ToInt32(MainForm.numHalfMoves.Value);
            selectedState.FullMoves = Convert.ToInt32(MainForm.numFullMoves.Value);

            return selectedState;
        }

        public static void UpdateBoardBasedOnLstHistory(UvsChess.Framework.ChessState selectedItem)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_UpdateBoardBasedOnLstHistory, selectedItem));
            }
        }

        private static void Actually_UpdateBoardBasedOnLstHistory(params object[] eventArgs)
        {
            UvsChess.Framework.ChessState selectedItem = (UvsChess.Framework.ChessState)eventArgs[0];

            if (selectedItem.CurrentPlayerColor == ChessColor.White)
            {
                MainForm.radWhite.Checked = true;
            }
            else
            {
                MainForm.radBlack.Checked = true;
            }

            MainForm.numFullMoves.Value = selectedItem.FullMoves;
            MainForm.numHalfMoves.Value = selectedItem.HalfMoves;

            MainForm.chessBoardControl.ResetBoard(selectedItem.CurrentBoard, selectedItem.PreviousMove);
        }

        public static void UpdateBoardBasedOnMove(UvsChess.Framework.ChessState selectedItem, ChessMove move, bool isWhiteChecked)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_UpdateBoardBasedOnMove, selectedItem, move, isWhiteChecked));
            }
        }

        private static void Actually_UpdateBoardBasedOnMove(params object[] eventArgs)
        {
            UvsChess.Framework.ChessState selectedItem = (UvsChess.Framework.ChessState)eventArgs[0];
            ChessMove move = (ChessMove)eventArgs[1];
            bool isWhiteChecked = (bool)eventArgs[2];
            
            //ChessState tmpState = new ChessState(item.fenboard);
            selectedItem.MakeMove(move);
            selectedItem.PreviousMove = null;
            selectedItem.PreviousBoard = null;

            if (isWhiteChecked)
            {
                selectedItem.CurrentPlayerColor = ChessColor.White;
            }
            else
            {
                selectedItem.CurrentPlayerColor = ChessColor.Black;
            }

            //item.fenboard = tmpState.ToFenBoard();

            // This causes the "selected index changed event"
            //lstHistory.Items[lstHistory.SelectedIndex] = tmpState;
            MainForm.lstHistory.SelectedItem = selectedItem;

            // Force update the lstHistory and the GuiChessBoard
            Actually_UpdateBoardBasedOnLstHistory(selectedItem);
        }

        public static void UpdateWhitesName(string name)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_UpdateWhitesName, name));
            }
        }

        private static void Actually_UpdateWhitesName(params object[] eventArgs)
        {            
            WhitePlayerName = (string)eventArgs[0];
        }

        public static void UpdateBlacksName(string name)
        {
            lock (_updateGuiDataLockObject)
            {
                _guiEvents.Add(new GuiEvent(Actually_UpdateBlacksName, name));
            }
        }

        private static void Actually_UpdateBlacksName(params object[] eventArgs)
        {
            BlackPlayerName = (string)eventArgs[0];
        }
    }
}
