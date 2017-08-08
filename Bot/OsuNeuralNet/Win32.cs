using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

sealed class Win32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    static public Color GetPixelColor(IntPtr hdc, int x, int y)
    {
        uint pixel = GetPixel(hdc, x, y);
        Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                     (int)(pixel & 0x0000FF00) >> 8,
                     (int)(pixel & 0x00FF0000) >> 16);
        return color;
    }

    static public Color GetPixelColor(int x, int y)
    {
        IntPtr hdc = GetDC(IntPtr.Zero);
        uint pixel = GetPixel(hdc, x, y);
        ReleaseDC(IntPtr.Zero, hdc);
        Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                     (int)(pixel & 0x0000FF00) >> 8,
                     (int)(pixel & 0x00FF0000) >> 16);
        return color;
    }

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

    static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
    static public Color GetColorAt(Point location)
    {
        using (Graphics gdest = Graphics.FromImage(screenPixel))
        {
            using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hSrcDC = gsrc.GetHdc();
                IntPtr hDC = gdest.GetHdc();
                int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                gdest.ReleaseHdc();
                gsrc.ReleaseHdc();
            }
        }
        return screenPixel.GetPixel(0, 0);
    }

    static public Color GetColorAt(IntPtr dc, int x, int y)
    {
        int a = (int)GetPixel(dc, x, y);
        return Color.FromArgb((int)(a | 0xFF000000));
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    public static void PressKey(Keys key, bool up)
    {
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;
        if (up)
        {
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
        }
        else
        {
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
        }
    }

    public static List<Color> getColors(int x = 0, int y = 0, int h = 1600, int w = 900)
    {
        List<Color> c = new List<Color>();

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                c.Add(GetColorAt(new Point(j + x, i + y)));
            }
        }

        return c;
    }

    [Flags]
    private enum KeyStates
    {
        None = 0,
        Down = 1,
        Toggled = 2
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern short GetKeyState(int keyCode);

    private static KeyStates GetKeyState(Keys key)
    {
        KeyStates state = KeyStates.None;

        short retVal = GetKeyState((int)key);

        //If the high-order bit is 1, the key is down
        //otherwise, it is up.
        if ((retVal & 0x8000) == 0x8000)
            state |= KeyStates.Down;

        //If the low-order bit is 1, the key is toggled.
        if ((retVal & 1) == 1)
            state |= KeyStates.Toggled;

        return state;
    }

    public static bool IsKeyDown(Keys key)
    {
        return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
    }

    public static bool IsKeyToggled(Keys key)
    {
        return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
    }

    static uint WM_KEYDOWN = 0x0100;
    static uint WM_KEYUP = 0x0101;
    static int VK_S = 0x53;
    static int VK_D = 0x44;
    static int VK_L = 0x4C;
    static int VK_M = 0x4D;

    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

    static void SendKey(string process, int key, uint action)
    {
        Process p = Process.GetProcessesByName(process)[0];
        PostMessage(p.MainWindowHandle, action, key, 0);
    }
}
