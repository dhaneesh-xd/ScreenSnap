using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class MouseHook
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;

    private static LowLevelMouseProc mouseProc = HookCallback;
    private static IntPtr hookID = IntPtr.Zero;

    private static Stopwatch stopwatch = new Stopwatch();

    public static event EventHandler RightButtonHold;

    public static void Start()
    {
        hookID = SetHook(mouseProc);
    }

    public static void Stop()
    {
        UnhookWindowsHookEx(hookID);
    }

    private static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONDOWN)
        {
            stopwatch.Restart();
        }
        else if (nCode >= 0 && wParam == (IntPtr)WM_RBUTTONUP && stopwatch.IsRunning)
        {
            stopwatch.Stop();
            if (stopwatch.Elapsed.TotalSeconds >= 2)
            {

            }
            RightButtonHold?.Invoke(null, new RightButtonHoldEventArgs(stopwatch.Elapsed.TotalSeconds));
        }
        return CallNextHookEx(hookID, nCode, wParam, lParam);
    }
    private static Rectangle GetVirtualScreenBounds()
    {
        Rectangle bounds = Rectangle.Empty;
        foreach (var screen in Screen.AllScreens)
        {
            bounds = Rectangle.Union(bounds, screen.WorkingArea);
        }
        if (bounds.Width == 1536) //scale 125%
        {
            bounds.Width = (int)(bounds.Width * 1.25);
            bounds.Height = (int)(bounds.Height * 1.25);
        }
        if (bounds.Width == 1280) //scale 150%
        {
            bounds.Width = (int)(bounds.Width * 1.5);
            bounds.Height = (int)(bounds.Height * 1.5);
        }
        return bounds;
    }
    public static void CaptureScreen()
    {
        try
        {
            string fileName = $"screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), fileName);

            // Get the screen bounds
            Rectangle bounds = GetVirtualScreenBounds();
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Capture the entire desktop area to the bitmap
                    g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                }

                // Save the bitmap as a JPEG image
                bmp.Save(filePath, ImageFormat.Jpeg);
            }

            //MessageBox.Show("Screenshot saved to: " + filePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving screenshot: " + ex.Message);
        }
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}

public class RightButtonHoldEventArgs : EventArgs
{
    public double HoldDurationSeconds { get; }

    public RightButtonHoldEventArgs(double holdDurationSeconds)
    {
        HoldDurationSeconds = holdDurationSeconds;
    }
}
