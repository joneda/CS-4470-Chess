using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UvsChess.Gui
{
    internal partial class GuiDecisionTree : Form
    {
        UvsChess.DecisionTree _dt = null;
        List<ListBox> decisionListBoxes = new List<ListBox>();

        public GuiDecisionTree(DecisionTree dt)
        {
            InitializeComponent();

            _dt = dt;

            ListBox rootNodeListBox = null;

            rootNodeListBox = NewDecisionListBox(new List<DecisionTree>() { _dt });

            //rootNodeListBox.Location = new Point(-5, 0);
            decisionListBoxes.Add(rootNodeListBox);
                        
            guiChessBoard1.IsLocked = true;
            this.splitContainer1.Panel2.Controls.Add(rootNodeListBox);
            rootNodeListBox.SelectedIndex = 0;

            this.hScrollBar1.LargeChange = rootNodeListBox.Width;
        }

        public ListBox NewDecisionListBox(List<DecisionTree> dtChildren)
        {
            ListBox retVal = new ListBox();
            retVal.Font = new Font("Courier New", 8);
            retVal.ScrollAlwaysVisible = true;
            retVal.Width = 223;
            retVal.BeginUpdate();

            foreach (DecisionTree curDecision in dtChildren)
            {
                if (curDecision.Move != null)
                {
                    if ((curDecision.Parent != null) &&
                        (curDecision.Parent.BestChildMove != null) &&
                        (curDecision.Parent.BestChildMove == curDecision.Move))
                    {
                        curDecision.Move.ToStringPrefix = "-> ";
                    }
                    else
                    {
                        curDecision.Move.ToStringPrefix = "   ";
                    }
                }
                else
                {
                    retVal.Width = 140;
                }

                retVal.Items.Add(curDecision);
            }

            retVal.EndUpdate();

            retVal.Height = this.splitContainer1.Panel2.Height - this.hScrollBar1.Height;
            retVal.SelectedIndexChanged += this.OnSelectedIndexChanged;

            return retVal;
        }

        private void OnSelectedIndexChanged(object s, EventArgs e)
        {
            ListBox sender = (ListBox)s;
            DecisionTree curDt = (DecisionTree)sender.SelectedItem;
            int indexOfSelectedListBox = FindIndexOfSelectedListBox(sender);

            List<ChessMove> highlightedMoves = new List<ChessMove>();

            for (int curMoveIdx = 1; curMoveIdx <= indexOfSelectedListBox; curMoveIdx++)
            {
                highlightedMoves.Add(((DecisionTree)decisionListBoxes[curMoveIdx].SelectedItem).Move);
            }

            //guiChessBoard1.ResetBoard(curDt.Board, curDt.Move);
            guiChessBoard1.ResetBoard(curDt.Board, highlightedMoves);

            if (curDt.IsRootNode)
            {
                lblActualMoveValue.Text = "Not Set";
                lblEventualMoveValue.Text = "Not Set";
            }
            else
            {
                lblActualMoveValue.Text = curDt.ActualMoveValue;
                lblEventualMoveValue.Text = curDt.EventualMoveValue;
            }

            int ix = indexOfSelectedListBox + 1;
            while (ix < decisionListBoxes.Count)
            {
                this.splitContainer1.Panel2.Controls.Remove(decisionListBoxes[ix]);
                ix++;
            }

            if ((indexOfSelectedListBox + 1) < decisionListBoxes.Count)
            {
                decisionListBoxes.RemoveRange(indexOfSelectedListBox + 1, (decisionListBoxes.Count - indexOfSelectedListBox) - 1);
            }

            if (curDt.Children.Count > 0)
            {
                ListBox newLb = NewDecisionListBox(curDt.Children);
                newLb.Location = new Point(sender.Location.X + sender.Width, 0);
                newLb.BackColor = guiChessBoard1.GetMoveHighlightColor(decisionListBoxes.Count - 1);
                decisionListBoxes.Add(newLb);                
                this.splitContainer1.Panel2.Controls.Add(newLb);
            }

            this.hScrollBar1.Maximum = (decisionListBoxes.Count * decisionListBoxes[0].Width);
        }

        private int FindIndexOfSelectedListBox(ListBox selectedListBox)
        {
            int ix = 0;
            while (decisionListBoxes[ix] != selectedListBox)
            {
                ix++;
            }

            return ix;
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            decisionListBoxes[0].Location = new Point(0 - hScrollBar1.Value, 0);

            int ix = 1;
            while (ix < decisionListBoxes.Count)
            {
                decisionListBoxes[ix].Location = new Point(decisionListBoxes[ix - 1].Location.X + decisionListBoxes[ix - 1].Width, 0);
                ix++;
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            foreach (ListBox curLb in decisionListBoxes)
            {
                curLb.Height = this.splitContainer1.Panel2.Height - this.hScrollBar1.Height;
            }
        }
    }
}
