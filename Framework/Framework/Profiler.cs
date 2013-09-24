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
using UvsChess.Gui;

namespace UvsChess.Framework
{
    internal class Profiler
    {
        private static ChessColor _currentPlayerColor;

        internal static AIProfiler WhiteProfiler { get; set; }
        internal static AIProfiler BlackProfiler { get; set; }

        internal static bool IsEnabled
        {
            get
            {
                return CurrentProfiler.IsEnabled;
            }
        }

        private static AIProfiler CurrentProfiler
        {
            get
            {
                if (_currentPlayerColor == ChessColor.White)
                    return WhiteProfiler;
                else
                    return BlackProfiler;
            }
        }

        private static Logger.LogCallback CurrentProfilerLogger
        {
            get
            {
                if (_currentPlayerColor == ChessColor.White)
                    return Logger.AddToWhitesLog;
                else
                    return Logger.AddToBlacksLog;
            }
        }        

        public static void BeginGame()
        {
            WhiteProfiler = new AIProfiler(ChessColor.White);
            WhiteProfiler.FxKeyNames = Enum.GetNames(typeof(ProfilerMethodKey));
            BlackProfiler = new AIProfiler(ChessColor.Black);
            BlackProfiler.FxKeyNames = Enum.GetNames(typeof(ProfilerMethodKey));
        }

        public static void EndGame()
        {
            if (WhiteProfiler.IsEnabled || BlackProfiler.IsEnabled)
            {
                Logger.Log("*** Game Stats ***");
                Logger.Log("---Begin CSV---");
                Logger.Log("\"Date of Profile:\",\"" + DateTime.Now.ToString() + "\"");
                List<string> whiteOutput = null;
                List<string> blackOutput = null;
                
                if (WhiteProfiler.IsEnabled)
                {                    
                    whiteOutput = WhiteProfiler.WriteAndEndGame();

                    if (BlackProfiler.IsEnabled)
                    {
                        whiteOutput.Add(string.Empty);
                    }
                }

                if (BlackProfiler.IsEnabled)
                {
                    blackOutput = BlackProfiler.WriteAndEndGame();
                }

                if (WhiteProfiler.IsEnabled &&
                    BlackProfiler.IsEnabled &&
                    (WhiteProfiler.NodesPerSecond > 0) &&
                    (BlackProfiler.NodesPerSecond > 0) &&
                    (WhiteProfiler.AIName == BlackProfiler.AIName) )
                {
                    Logger.Log("\"Avg Nodes/Sec:\",\"" + string.Format("{0:N2}", ((WhiteProfiler.NodesPerSecond + BlackProfiler.NodesPerSecond) / 2)) + "\"");
                }

                if (whiteOutput != null)
                {
                    foreach (string curLine in whiteOutput)
                    {
                        Logger.Log(curLine);
                    }
                }

                if (blackOutput != null)
                {
                    foreach (string curLine in blackOutput)
                    {
                        Logger.Log(curLine);
                    }
                }

                Logger.Log("---End CSV---");
            }

            WhiteProfiler = null;
            BlackProfiler = null;
        }

        public static void BeginTurn(ChessColor playerColor)
        {
            _currentPlayerColor = playerColor;
        }

        public static void EndTurn(TimeSpan moveTime)
        {
            if (IsEnabled)
            {
                CurrentProfiler.EndTurn(CurrentProfilerLogger, moveTime);
            }

            if (_currentPlayerColor == ChessColor.White)
            {
                _currentPlayerColor = ChessColor.Black;
            }
            else
            {
                _currentPlayerColor = ChessColor.White;
            }
        }

        public static void AddToMainProfile(int key)
        {
            if (CurrentProfiler != null)
            {
                CurrentProfiler.AddToFxProfile(key);
            }
        }
    }
}
