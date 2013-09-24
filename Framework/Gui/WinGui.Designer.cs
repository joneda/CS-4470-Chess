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

namespace UvsChess.Gui
{
    internal partial class WinGui
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinGui));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDecisionTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutUvsChessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbChessFlags = new System.Windows.Forms.ComboBox();
            this.numHalfMoves = new System.Windows.Forms.NumericUpDown();
            this.numFullMoves = new System.Windows.Forms.NumericUpDown();
            this.cmbBlack = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radBlack = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.radWhite = new System.Windows.Forms.RadioButton();
            this.cmbWhite = new System.Windows.Forms.ComboBox();
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.chessBoardControl = new UvsChess.Gui.GuiChessBoard();
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tabMainLog = new System.Windows.Forms.TabPage();
            this.btnSaveMainLog = new System.Windows.Forms.Button();
            this.btnClearMainLog = new System.Windows.Forms.Button();
            this.chkBxAutoScrollMainLog = new System.Windows.Forms.CheckBox();
            this.lstMainLog = new System.Windows.Forms.ListBox();
            this.tabWhitesLog = new System.Windows.Forms.TabPage();
            this.btnClearWhitesLog = new System.Windows.Forms.Button();
            this.btnSaveWhitesLog = new System.Windows.Forms.Button();
            this.chkBxAutoScrollWhitesLog = new System.Windows.Forms.CheckBox();
            this.lstWhitesLog = new System.Windows.Forms.ListBox();
            this.tabBlacksLog = new System.Windows.Forms.TabPage();
            this.btnClearBlacksLog = new System.Windows.Forms.Button();
            this.btnSaveBlacksLog = new System.Windows.Forms.Button();
            this.chkBxAutoScrollBlacksLog = new System.Windows.Forms.CheckBox();
            this.lstBlacksLog = new System.Windows.Forms.ListBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHalfMoves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFullMoves)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.tabMainLog.SuspendLayout();
            this.tabWhitesLog.SuspendLayout();
            this.tabBlacksLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.gameToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1064, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.saveToolStripMenuItem.Text = "Save Selected State";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.clearHistoryToolStripMenuItem,
            this.viewDecisionTreeToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.startToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // clearHistoryToolStripMenuItem
            // 
            this.clearHistoryToolStripMenuItem.Name = "clearHistoryToolStripMenuItem";
            this.clearHistoryToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.clearHistoryToolStripMenuItem.Text = "Clear History";
            this.clearHistoryToolStripMenuItem.Click += new System.EventHandler(this.clearHistoryToolStripMenuItem_Click);
            // 
            // viewDecisionTreeToolStripMenuItem
            // 
            this.viewDecisionTreeToolStripMenuItem.Name = "viewDecisionTreeToolStripMenuItem";
            this.viewDecisionTreeToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.viewDecisionTreeToolStripMenuItem.Text = "View Decision Tree";
            this.viewDecisionTreeToolStripMenuItem.Click += new System.EventHandler(this.viewDecisionTreeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem,
            this.aboutUvsChessToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.helpToolStripMenuItem.Text = "Information";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // aboutUvsChessToolStripMenuItem
            // 
            this.aboutUvsChessToolStripMenuItem.Name = "aboutUvsChessToolStripMenuItem";
            this.aboutUvsChessToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.aboutUvsChessToolStripMenuItem.Text = "About UvsChess";
            this.aboutUvsChessToolStripMenuItem.Click += new System.EventHandler(this.aboutUvsChessToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.cmbChessFlags);
            this.splitContainer1.Panel1.Controls.Add(this.numHalfMoves);
            this.splitContainer1.Panel1.Controls.Add(this.numFullMoves);
            this.splitContainer1.Panel1.Controls.Add(this.cmbBlack);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.radBlack);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.radWhite);
            this.splitContainer1.Panel1.Controls.Add(this.cmbWhite);
            this.splitContainer1.Panel1.Controls.Add(this.lstHistory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1064, 726);
            this.splitContainer1.SplitterDistance = 262;
            this.splitContainer1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Chess Flag";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // cmbChessFlags
            // 
            this.cmbChessFlags.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChessFlags.FormattingEnabled = true;
            this.cmbChessFlags.Location = new System.Drawing.Point(79, 104);
            this.cmbChessFlags.Name = "cmbChessFlags";
            this.cmbChessFlags.Size = new System.Drawing.Size(94, 21);
            this.cmbChessFlags.TabIndex = 7;
            // 
            // numHalfMoves
            // 
            this.numHalfMoves.Location = new System.Drawing.Point(79, 67);
            this.numHalfMoves.Name = "numHalfMoves";
            this.numHalfMoves.Size = new System.Drawing.Size(51, 20);
            this.numHalfMoves.TabIndex = 4;
            // 
            // numFullMoves
            // 
            this.numFullMoves.Location = new System.Drawing.Point(179, 67);
            this.numFullMoves.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numFullMoves.Name = "numFullMoves";
            this.numFullMoves.Size = new System.Drawing.Size(52, 20);
            this.numFullMoves.TabIndex = 5;
            // 
            // cmbBlack
            // 
            this.cmbBlack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbBlack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBlack.FormattingEnabled = true;
            this.cmbBlack.Location = new System.Drawing.Point(71, 33);
            this.cmbBlack.Name = "cmbBlack";
            this.cmbBlack.Size = new System.Drawing.Size(188, 21);
            this.cmbBlack.TabIndex = 3;
            this.cmbBlack.SelectedIndexChanged += new System.EventHandler(this.cmbBlack_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(136, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Move:";
            // 
            // radBlack
            // 
            this.radBlack.AutoSize = true;
            this.radBlack.Location = new System.Drawing.Point(12, 29);
            this.radBlack.Name = "radBlack";
            this.radBlack.Size = new System.Drawing.Size(52, 17);
            this.radBlack.TabIndex = 2;
            this.radBlack.Text = "Black";
            this.radBlack.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Half Moves";
            // 
            // radWhite
            // 
            this.radWhite.AutoSize = true;
            this.radWhite.Checked = true;
            this.radWhite.Location = new System.Drawing.Point(12, 6);
            this.radWhite.Name = "radWhite";
            this.radWhite.Size = new System.Drawing.Size(53, 17);
            this.radWhite.TabIndex = 0;
            this.radWhite.TabStop = true;
            this.radWhite.Text = "White";
            this.radWhite.UseVisualStyleBackColor = true;
            // 
            // cmbWhite
            // 
            this.cmbWhite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWhite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhite.FormattingEnabled = true;
            this.cmbWhite.Location = new System.Drawing.Point(71, 6);
            this.cmbWhite.Name = "cmbWhite";
            this.cmbWhite.Size = new System.Drawing.Size(188, 21);
            this.cmbWhite.TabIndex = 1;
            this.cmbWhite.SelectedIndexChanged += new System.EventHandler(this.cmbWhite_SelectedIndexChanged);
            // 
            // lstHistory
            // 
            this.lstHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.Location = new System.Drawing.Point(3, 143);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new System.Drawing.Size(256, 576);
            this.lstHistory.TabIndex = 6;
            this.lstHistory.SelectedIndexChanged += new System.EventHandler(this.lstHistory_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel1.Controls.Add(this.chessBoardControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabLogs);
            this.splitContainer2.Size = new System.Drawing.Size(798, 726);
            this.splitContainer2.SplitterDistance = 333;
            this.splitContainer2.TabIndex = 0;
            // 
            // chessBoardControl
            // 
            this.chessBoardControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chessBoardControl.IsLocked = false;
            this.chessBoardControl.Location = new System.Drawing.Point(0, 0);
            this.chessBoardControl.Name = "chessBoardControl";
            this.chessBoardControl.Size = new System.Drawing.Size(333, 333);
            this.chessBoardControl.TabIndex = 0;
            this.chessBoardControl.TabStop = false;
            // 
            // tabLogs
            // 
            this.tabLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabLogs.Controls.Add(this.tabMainLog);
            this.tabLogs.Controls.Add(this.tabWhitesLog);
            this.tabLogs.Controls.Add(this.tabBlacksLog);
            this.tabLogs.Location = new System.Drawing.Point(0, 3);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.SelectedIndex = 0;
            this.tabLogs.Size = new System.Drawing.Size(798, 386);
            this.tabLogs.TabIndex = 8;
            // 
            // tabMainLog
            // 
            this.tabMainLog.Controls.Add(this.btnSaveMainLog);
            this.tabMainLog.Controls.Add(this.btnClearMainLog);
            this.tabMainLog.Controls.Add(this.chkBxAutoScrollMainLog);
            this.tabMainLog.Controls.Add(this.lstMainLog);
            this.tabMainLog.Location = new System.Drawing.Point(4, 22);
            this.tabMainLog.Name = "tabMainLog";
            this.tabMainLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabMainLog.Size = new System.Drawing.Size(790, 360);
            this.tabMainLog.TabIndex = 0;
            this.tabMainLog.Text = "Main";
            this.tabMainLog.UseVisualStyleBackColor = true;
            // 
            // btnSaveMainLog
            // 
            this.btnSaveMainLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveMainLog.Location = new System.Drawing.Point(628, 3);
            this.btnSaveMainLog.Name = "btnSaveMainLog";
            this.btnSaveMainLog.Size = new System.Drawing.Size(75, 23);
            this.btnSaveMainLog.TabIndex = 10;
            this.btnSaveMainLog.Text = "Save...";
            this.btnSaveMainLog.UseVisualStyleBackColor = true;
            this.btnSaveMainLog.Click += new System.EventHandler(this.btnSaveMainLog_Click);
            // 
            // btnClearMainLog
            // 
            this.btnClearMainLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearMainLog.Location = new System.Drawing.Point(709, 3);
            this.btnClearMainLog.Name = "btnClearMainLog";
            this.btnClearMainLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearMainLog.TabIndex = 11;
            this.btnClearMainLog.Text = "Clear";
            this.btnClearMainLog.UseVisualStyleBackColor = true;
            this.btnClearMainLog.Click += new System.EventHandler(this.btnClearMainLog_Click);
            // 
            // chkBxAutoScrollMainLog
            // 
            this.chkBxAutoScrollMainLog.AutoSize = true;
            this.chkBxAutoScrollMainLog.Checked = true;
            this.chkBxAutoScrollMainLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxAutoScrollMainLog.Location = new System.Drawing.Point(3, 5);
            this.chkBxAutoScrollMainLog.Name = "chkBxAutoScrollMainLog";
            this.chkBxAutoScrollMainLog.Size = new System.Drawing.Size(77, 17);
            this.chkBxAutoScrollMainLog.TabIndex = 9;
            this.chkBxAutoScrollMainLog.Text = "Auto Scroll";
            this.chkBxAutoScrollMainLog.UseVisualStyleBackColor = true;
            // 
            // lstMainLog
            // 
            this.lstMainLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMainLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstMainLog.FormattingEnabled = true;
            this.lstMainLog.ItemHeight = 14;
            this.lstMainLog.Location = new System.Drawing.Point(0, 28);
            this.lstMainLog.Name = "lstMainLog";
            this.lstMainLog.Size = new System.Drawing.Size(787, 312);
            this.lstMainLog.TabIndex = 12;
            // 
            // tabWhitesLog
            // 
            this.tabWhitesLog.Controls.Add(this.btnClearWhitesLog);
            this.tabWhitesLog.Controls.Add(this.btnSaveWhitesLog);
            this.tabWhitesLog.Controls.Add(this.chkBxAutoScrollWhitesLog);
            this.tabWhitesLog.Controls.Add(this.lstWhitesLog);
            this.tabWhitesLog.Location = new System.Drawing.Point(4, 22);
            this.tabWhitesLog.Name = "tabWhitesLog";
            this.tabWhitesLog.Size = new System.Drawing.Size(790, 360);
            this.tabWhitesLog.TabIndex = 2;
            this.tabWhitesLog.Text = "White\'s log";
            this.tabWhitesLog.UseVisualStyleBackColor = true;
            // 
            // btnClearWhitesLog
            // 
            this.btnClearWhitesLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearWhitesLog.Location = new System.Drawing.Point(709, 3);
            this.btnClearWhitesLog.Name = "btnClearWhitesLog";
            this.btnClearWhitesLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearWhitesLog.TabIndex = 2;
            this.btnClearWhitesLog.Text = "Clear";
            this.btnClearWhitesLog.UseVisualStyleBackColor = true;
            this.btnClearWhitesLog.Click += new System.EventHandler(this.btnClearWhitesLog_Click);
            // 
            // btnSaveWhitesLog
            // 
            this.btnSaveWhitesLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveWhitesLog.Location = new System.Drawing.Point(628, 3);
            this.btnSaveWhitesLog.Name = "btnSaveWhitesLog";
            this.btnSaveWhitesLog.Size = new System.Drawing.Size(75, 23);
            this.btnSaveWhitesLog.TabIndex = 3;
            this.btnSaveWhitesLog.Text = "Save...";
            this.btnSaveWhitesLog.UseVisualStyleBackColor = true;
            this.btnSaveWhitesLog.Click += new System.EventHandler(this.btnSaveWhitesLog_Click);
            // 
            // chkBxAutoScrollWhitesLog
            // 
            this.chkBxAutoScrollWhitesLog.AutoSize = true;
            this.chkBxAutoScrollWhitesLog.Checked = true;
            this.chkBxAutoScrollWhitesLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxAutoScrollWhitesLog.Location = new System.Drawing.Point(3, 5);
            this.chkBxAutoScrollWhitesLog.Name = "chkBxAutoScrollWhitesLog";
            this.chkBxAutoScrollWhitesLog.Size = new System.Drawing.Size(77, 17);
            this.chkBxAutoScrollWhitesLog.TabIndex = 1;
            this.chkBxAutoScrollWhitesLog.Text = "Auto Scroll";
            this.chkBxAutoScrollWhitesLog.UseVisualStyleBackColor = true;
            // 
            // lstWhitesLog
            // 
            this.lstWhitesLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstWhitesLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstWhitesLog.FormattingEnabled = true;
            this.lstWhitesLog.ItemHeight = 14;
            this.lstWhitesLog.Location = new System.Drawing.Point(0, 28);
            this.lstWhitesLog.Name = "lstWhitesLog";
            this.lstWhitesLog.Size = new System.Drawing.Size(787, 312);
            this.lstWhitesLog.TabIndex = 0;
            // 
            // tabBlacksLog
            // 
            this.tabBlacksLog.Controls.Add(this.btnClearBlacksLog);
            this.tabBlacksLog.Controls.Add(this.btnSaveBlacksLog);
            this.tabBlacksLog.Controls.Add(this.chkBxAutoScrollBlacksLog);
            this.tabBlacksLog.Controls.Add(this.lstBlacksLog);
            this.tabBlacksLog.Location = new System.Drawing.Point(4, 22);
            this.tabBlacksLog.Name = "tabBlacksLog";
            this.tabBlacksLog.Size = new System.Drawing.Size(790, 360);
            this.tabBlacksLog.TabIndex = 1;
            this.tabBlacksLog.Text = "Black\'s log";
            this.tabBlacksLog.UseVisualStyleBackColor = true;
            // 
            // btnClearBlacksLog
            // 
            this.btnClearBlacksLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearBlacksLog.Location = new System.Drawing.Point(709, 3);
            this.btnClearBlacksLog.Name = "btnClearBlacksLog";
            this.btnClearBlacksLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearBlacksLog.TabIndex = 2;
            this.btnClearBlacksLog.Text = "Clear";
            this.btnClearBlacksLog.UseVisualStyleBackColor = true;
            this.btnClearBlacksLog.Click += new System.EventHandler(this.btnClearBlacksLog_Click);
            // 
            // btnSaveBlacksLog
            // 
            this.btnSaveBlacksLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveBlacksLog.Location = new System.Drawing.Point(628, 3);
            this.btnSaveBlacksLog.Name = "btnSaveBlacksLog";
            this.btnSaveBlacksLog.Size = new System.Drawing.Size(75, 23);
            this.btnSaveBlacksLog.TabIndex = 3;
            this.btnSaveBlacksLog.Text = "Save...";
            this.btnSaveBlacksLog.UseVisualStyleBackColor = true;
            this.btnSaveBlacksLog.Click += new System.EventHandler(this.btnSaveBlacksLog_Click);
            // 
            // chkBxAutoScrollBlacksLog
            // 
            this.chkBxAutoScrollBlacksLog.AutoSize = true;
            this.chkBxAutoScrollBlacksLog.Checked = true;
            this.chkBxAutoScrollBlacksLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxAutoScrollBlacksLog.Location = new System.Drawing.Point(3, 5);
            this.chkBxAutoScrollBlacksLog.Name = "chkBxAutoScrollBlacksLog";
            this.chkBxAutoScrollBlacksLog.Size = new System.Drawing.Size(77, 17);
            this.chkBxAutoScrollBlacksLog.TabIndex = 1;
            this.chkBxAutoScrollBlacksLog.Text = "Auto Scroll";
            this.chkBxAutoScrollBlacksLog.UseVisualStyleBackColor = true;
            // 
            // lstBlacksLog
            // 
            this.lstBlacksLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBlacksLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstBlacksLog.FormattingEnabled = true;
            this.lstBlacksLog.ItemHeight = 14;
            this.lstBlacksLog.Location = new System.Drawing.Point(0, 28);
            this.lstBlacksLog.Name = "lstBlacksLog";
            this.lstBlacksLog.Size = new System.Drawing.Size(787, 312);
            this.lstBlacksLog.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // WinGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 750);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "WinGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UvsChess.googlecode.com - © Rusty Howell and Thomas Wiest";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.WinGui_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinGui_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numHalfMoves)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFullMoves)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.tabMainLog.ResumeLayout(false);
            this.tabMainLog.PerformLayout();
            this.tabWhitesLog.ResumeLayout(false);
            this.tabWhitesLog.PerformLayout();
            this.tabBlacksLog.ResumeLayout(false);
            this.tabBlacksLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        public  System.Windows.Forms.ComboBox cmbBlack;
        public  System.Windows.Forms.RadioButton radBlack;
        public  System.Windows.Forms.RadioButton radWhite;
        public  System.Windows.Forms.ComboBox cmbWhite;
        public  System.Windows.Forms.ListBox lstHistory;
        public  System.Windows.Forms.OpenFileDialog openFileDialog1;
        public  System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        internal GuiChessBoard chessBoardControl;
        private System.Windows.Forms.TabControl tabLogs;
        private System.Windows.Forms.TabPage tabMainLog;
        public  System.Windows.Forms.ListBox lstMainLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutUvsChessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        public  System.Windows.Forms.ToolStripMenuItem clearHistoryToolStripMenuItem;
        private System.Windows.Forms.TabPage tabBlacksLog;
        private System.Windows.Forms.TabPage tabWhitesLog;
        public  System.Windows.Forms.ListBox lstWhitesLog;
        public  System.Windows.Forms.ListBox lstBlacksLog;
        public  System.Windows.Forms.CheckBox chkBxAutoScrollBlacksLog;
        public  System.Windows.Forms.CheckBox chkBxAutoScrollMainLog;
        public  System.Windows.Forms.CheckBox chkBxAutoScrollWhitesLog;
        private System.Windows.Forms.Button btnSaveMainLog;
        private System.Windows.Forms.Button btnClearMainLog;
        private System.Windows.Forms.Button btnClearWhitesLog;
        private System.Windows.Forms.Button btnSaveWhitesLog;
        private System.Windows.Forms.Button btnClearBlacksLog;
        private System.Windows.Forms.Button btnSaveBlacksLog;
        public  System.Windows.Forms.NumericUpDown numHalfMoves;
        public  System.Windows.Forms.NumericUpDown numFullMoves;
        private System.Windows.Forms.ToolStripMenuItem viewDecisionTreeToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbChessFlags;
    }
}
