using System;
using Windows.Devices.Input;
using Windows.Foundation;

namespace GameManager
{
    internal class Form
    {
        internal Action<object, MouseEventArgs> MouseMove;
        internal Action<object, MouseEventArgs> MouseUp;
        internal Action<object, MouseEventArgs> MouseDown;
        internal Cursor Cursor;
        internal FormWindowState WindowState;
        internal bool MaximizeBox;
        internal object BackColor;
        internal Action<object, EventArgs> Resize;
        internal Size MinimumSize;
    }
}