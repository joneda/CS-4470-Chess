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
using System.Windows.Forms;
using UvsChess.Framework;

namespace UvsChess.Gui
{
    internal partial class WinGui : Form
    {
        List<AI> AvailableAIs = new List<AI>();
        public ChessFlag HumanFlag
        {
            get { return (ChessFlag) cmbChessFlags.SelectedItem; }
        }

        #region Constructors
        public WinGui()
        {
            InitializeComponent();

            MessageBox.MainForm = this;
            GuiEventLoop.MainForm = this;

            GuiEventLoop.ResetHistory(new ChessState());

            GuiEventLoop.PollGuiOnce();

            chessBoardControl.PieceMovedByHuman += PieceMovedByHuman_Changed;
            chessBoardControl.SetChessFlagCombo(this.cmbChessFlags);
        }

        private void WinGui_Load(object sender, EventArgs e)
        {
            DllLoader.SearchForAIs();

            foreach (AI ai in DllLoader.AvailableAIs)
            {
                cmbBlack.Items.Add(ai.ShortName);
                cmbWhite.Items.Add(ai.ShortName);
            }
            cmbBlack.SelectedIndex = 0;
            cmbWhite.SelectedIndex = 0;

            // Set values in Chess Flag drop down
            foreach(ChessFlag flag in Enum.GetValues(typeof(ChessFlag)))
            {
                if (flag != ChessFlag.AIWentOverTime)
                {
                    cmbChessFlags.Items.Add(flag);
                }
            }
            cmbChessFlags.SelectedIndex = 0;

        }
        #endregion    
        
        public void PieceMovedByHuman_Changed(ChessMove move)
        {
            GuiEventLoop.UpdateBoardBasedOnMove((ChessState)this.lstHistory.SelectedItem, move, this.radWhite.Checked);
        }

        #region File menu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.OpenStateFromDisk();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.SaveSelectedStateToDisk();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Game menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ResetHistory(new ChessState());
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.StartGame();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.StopGame();
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ResetHistory(new ChessState());
        }
        #endregion

        #region Information menu
        private void aboutUvsChessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUvsChess about = new AboutUvsChess();
            about.ShowDialog();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences prefs = new Preferences();
            prefs.ShowDialog();
        }
        #endregion

        #region AISelector controls
        private void cmbWhite_SelectedIndexChanged(object sender, EventArgs e)
        {
            GuiEventLoop.UpdateWhitesName(this.cmbWhite.SelectedItem.ToString());
        }

        private void cmbBlack_SelectedIndexChanged(object sender, EventArgs e)
        {
            GuiEventLoop.UpdateBlacksName(this.cmbBlack.SelectedItem.ToString());
        }
        #endregion

        #region Update ChessState Info
        private void lstHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            GuiEventLoop.UpdateBoardBasedOnLstHistory((ChessState)this.lstHistory.SelectedItem);
        }
        #endregion

        #region Log Tabs
        private void btnSaveMainLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.SaveMainLogToDisk();
        }

        private void btnSaveWhitesLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.SaveWhitesLogToDisk();
        }

        private void btnSaveBlacksLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.SaveBlacksLogToDisk();
        }

        private void btnClearMainLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ClearMainLog();
        }

        private void btnClearWhitesLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ClearWhitesLog();
        }

        private void btnClearBlacksLog_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ClearBlacksLog();
        }
        #endregion

        private void WinGui_FormClosing(object sender, FormClosingEventArgs e)
        {            
            GuiEventLoop.ShutdownGuiEventLoop();
        }

        private void viewDecisionTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuiEventLoop.ShowDecisionTree();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
