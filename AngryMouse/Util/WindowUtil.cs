using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AngryMouse.Util
{
    class WindowUtil
    {
        const int GWL_EXSTYLE = -20;

        [Flags]
        public enum ExtendedWindowStyles
        {
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_TOOLWINDOW = 0x00000080
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowStyles(Window window, ExtendedWindowStyles styles)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            extendedStyle |= (int)styles;
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle);
        }
    }
}
