using System;

namespace GameManager
{
    public class Cursor
    {
        private IntPtr intPtr;

        public Cursor(IntPtr intPtr)
        {
            this.intPtr = intPtr;
        }
    }
}