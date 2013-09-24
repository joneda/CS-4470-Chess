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
using UvsChess.Framework;

namespace UvsChess
{
    /// <summary>
    /// Represents a location on the chess board.
    /// </summary>
    public class ChessLocation
    {
        #region Constructors
        /// <summary>
        /// Creates a ChessLocation object
        /// </summary>
        /// <param name="x">Column number. Ranges 0 - 7</param>
        /// <param name="y">Row number. Ranges from 0 - 7</param>
        public ChessLocation(int x, int y)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_ctor_int_int);
            X = x;
            Y = y;
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Column number. Ranges from 0 - 7
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Row number. Ranges from 0 - 7
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Returns true if the row and column numbers are each between 0 and 7
        /// </summary>
        internal bool IsValid
        {
            get
            {
                Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_get_IsValid);
                return ((X >= 0) && (X < ChessBoard.NumberOfColumns) &&
                         (Y >= 0) && (Y < ChessBoard.NumberOfRows));
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Creates a complete copy of this object and returns it.
        /// </summary>
        /// <returns>The ChessLocation copy.</returns>
        public ChessLocation Clone()
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_Clone);
            return new ChessLocation(this.X, this.Y);
        }
        #endregion

        #region operator and hashcode overloads
        public override bool Equals(object obj)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_Equals);
            if (obj == null)
                return false;

            if (!(obj is ChessLocation))
                return false;

            ChessLocation tmpLoc = (ChessLocation)obj;

            if ((this.X == tmpLoc.X) &&
                 (this.Y == tmpLoc.Y))
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(ChessLocation lhs, ChessLocation rhs)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_NE);
            return !(lhs == rhs);
        }

        public static bool operator== (ChessLocation lhs, ChessLocation rhs)
        {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_EQ);
            if ( ((object) lhs == null) &&
                 ((object) rhs == null) )
            {
                return true;
            }

            if ( ((object) lhs == null) ||
                 ((object) rhs == null) )
            {
                return false;
            }

            return lhs.Equals(rhs);                 
        }
	
	    public override int GetHashCode()
	    {
            Profiler.AddToMainProfile((int)ProfilerMethodKey.ChessLocation_GetHashCode);
            return X * 10 + Y;
	    }
        #endregion
    }
}
