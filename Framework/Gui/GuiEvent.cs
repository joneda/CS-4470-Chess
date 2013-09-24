using System;
using System.Collections.Generic;
using System.Text;

namespace UvsChess.Gui
{
    internal class GuiEvent
    {
        public GuiEventLoop.ObjectParameterCallback EventCallback { get; set; }
        public object[] EventArgs { get; set; }

        public GuiEvent(GuiEventLoop.ObjectParameterCallback eventCallback, params object[] eventArgs)
        {
            EventCallback = eventCallback;
            EventArgs = eventArgs;
        }
    }
}
