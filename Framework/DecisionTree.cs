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
using UvsChess.Framework;

namespace UvsChess
{
    /// <summary>
    /// Represents the AI's decision making process while running through MiniMax.
    /// </summary>
    public class DecisionTree
    {
        private ChessMove _move = null;
        private ChessBoard _board = null;
        private string _eventualMoveValue = null;

        /// <summary>
        /// Gets or Sets which child move the AI has determined is the best.
        /// </summary>
        public ChessMove BestChildMove { get; set; }

        /// <summary>
        /// Constructor that creates a decision tree off of a chess board.
        /// </summary>
        /// <param name="board">The chess board that the decision tree starts from.</param>
        public DecisionTree(ChessBoard board)
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_ctor_ChessBoard);
            Children = new List<DecisionTree>();
            Board = board;
            BestChildMove = null;
        }

        private DecisionTree(DecisionTree parent, ChessBoard board, ChessMove move)
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_ctor_DecisionTree_ChessBoard_ChessMove);
            Children = new List<DecisionTree>();
            Parent = parent;
            Board = board;
            Move = move;
            BestChildMove = null;
        }

        /// <summary>
        /// The last child decsion added to the decision tree.
        /// </summary>
        public DecisionTree LastChild
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_LastChild);
                return this.Children[this.Children.Count - 1];
            }                
        }

        /// <summary>
        /// The parent decision of this part of the decision tree.
        /// </summary>
        public DecisionTree Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns this decision's actual move value.
        /// </summary>
        public string ActualMoveValue
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_ActualMoveValue);
                return Move.ValueOfMove.ToString();
            }
        }

        /// <summary>
        /// Returns this decisions eventual move value.
        /// </summary>
        public string EventualMoveValue
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_EventualMoveValue);
                if (_eventualMoveValue == null)
                {
                    return "Not Set";
                }

                return _eventualMoveValue.ToString();
            }

            set
            {
                _eventualMoveValue = value;
            }
        }

        /// <summary>
        /// Create a DecisionTree exactly like this one.
        /// </summary>
        /// <param name="parent">The parent decision</param>
        /// <returns>The cloned decision tree</returns>
        public DecisionTree Clone(DecisionTree parent)
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_Clone_DecisionTree);
            DecisionTree retVal = null;

            if (parent == null)
            {
                retVal = new DecisionTree(this.Board);
            }
            else
            {
                retVal = new DecisionTree(parent, this.Board, this.Move);
            }

            retVal.EventualMoveValue = this.EventualMoveValue;
            retVal.BestChildMove = this.BestChildMove;

            foreach (DecisionTree curChild in this.Children)
            {
                retVal.Children.Add(curChild.Clone(this));
            }

            return retVal;
        }

        /// <summary>
        /// Adds a child decision to the decision tree.
        /// </summary>
        /// <param name="board">The board the decision was made from</param>
        /// <param name="move">The move decided.</param>
        public void AddChild(ChessBoard board, ChessMove move)
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_AddChild_ChessBoard_ChessMove);
            this.Children.Add(new DecisionTree(this, board, move));
        }

        /// <summary>
        /// Creates a string representation of the DecisionTree at this point.
        /// </summary>
        /// <returns>the string representation</returns>
        public override string ToString()
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_ToString);
            if (IsRootNode)
            {
                return "Starting Board";
            }
            else
            {
                return Move.ToString();
            }
        }

        #region Internal Properties

        internal List<DecisionTree> Children
        {
            get;
            set;
        }

        internal bool IsRootNode
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_IsRootNode);
                return (this.Parent == null);
            }
        }

        internal ChessBoard Board
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_Board);
                return _board;
            }

            set
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_set_Board);
                _board = value.Clone();
            }
        }

        internal ChessMove Move
        {
            get
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_get_Move);
                return _move;
            }

            set
            {
                UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_set_Move);
                _move = value.Clone();
            }
        }

        internal DecisionTree Clone()
        {
            UvsChess.Framework.Profiler.AddToMainProfile((int)ProfilerMethodKey.DecisionTree_Clone);
            return this.Clone(null);
        }

        #endregion
    }
}
