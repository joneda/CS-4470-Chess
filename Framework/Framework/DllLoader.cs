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
using System.IO;

namespace UvsChess.Framework
{
    internal class DllLoader
    {
        private static List<AI> _availableais = new List<AI>();
        public static List<AI> AvailableAIs
        {
            get { return _availableais; }
        }

        #region Loading DLLs
        public static void SearchForAIs()
        {
            _availableais = new List<AI>();
            _availableais.Add(new AI("Human"));
            //AvailableAIs.Add("Human");
            string appFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string[] dlls = Directory.GetFiles(appFolder, "*.dll");

            foreach (string dll in dlls)
            {
                LoadAIsFromFile(dll);
            }
        }

        static void LoadAIsFromFile(string filename)
        {
            try
            {
                System.Reflection.Assembly assem = System.Reflection.Assembly.LoadFile(filename);
                System.Type[] types = assem.GetTypes();

                foreach (System.Type type in types)
                {
                    System.Type[] interfaces = type.GetInterfaces();

                    foreach (System.Type inter in interfaces)
                    {
                        if (inter == typeof(UvsChess.IChessAI))
                        {
                            IChessAI ai = (IChessAI)assem.CreateInstance(type.FullName);
                            AI tmp = new AI(ai.Name);
                            tmp.FileName = filename;
                            tmp.FullName = type.FullName;
                            _availableais.Add(tmp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Chess->MainForm->LoadAI: " + ex.Message);
            }
        }

        #endregion

    }
}
