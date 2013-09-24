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
using UvsChess.Gui;

namespace UvsChess.Framework
{
    internal class Logger
    {
        public delegate void LogCallback(string msg);

        public static void Log(string msg)
        {
            //Console.WriteLine(msg);
            //UpdateWinGuiOnTimer.AddToMainLog("[" + System.AppDomain.GetCurrentThreadId().ToString() + "] " + 
            //                                    Environment.TickCount.ToString() + ": " + msg); 
            GuiEventLoop.AddToMainLog(msg); 
        }

        public static void AddToWhitesLog(string msg)
        {
            GuiEventLoop.AddToWhitesLog(msg);
        }

        public static void AddToBlacksLog(string msg)
        {
            GuiEventLoop.AddToBlacksLog(msg);
        }
    }
}
