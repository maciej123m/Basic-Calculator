using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Kalkulator
{
    public static class WindowUtils
    {
        /// <summary>
        /// Extends the glass area into the client area of the window
        /// </summary>
        /// <param name="window">Window to extend the glass on.</param>
        /// <param name="thikness">Thickness of border to extend.</param>
        public static void ExtendGlass(this Window window, Thickness thikness)
        {
            try
            {
                int isGlassEnabled = 0;
                Win32.DwmIsCompositionEnabled(ref isGlassEnabled);
                if (Environment.OSVersion.Version.Major > 5 && isGlassEnabled > 0)
                {
                    // Get the window handle
                    var helper = new WindowInteropHelper(window);
                    var mainWindowSrc = HwndSource.FromHwnd(helper.Handle);

                    if (mainWindowSrc != null)
                    {
                        if (mainWindowSrc.CompositionTarget != null)
                        {
                            mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
                        }

                        // Get the dpi of the screen
                        System.Drawing.Graphics desktop =
                            System.Drawing.Graphics.FromHwnd(mainWindowSrc.Handle);

                        float dpiX = desktop.DpiX / 96;
                        float dpiY = desktop.DpiY / 96;

                        // Set Margins
                        var margins = new MARGINS
                        {
                            cxLeftWidth = (int)(thikness.Left * dpiX),
                            cxRightWidth = (int)(thikness.Right * dpiX),
                            cyBottomHeight = (int)(thikness.Bottom * dpiY),
                            cyTopHeight = (int)(thikness.Top * dpiY)
                        };

                        window.Background = Brushes.Transparent;

                        Win32.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                    }
                }
                else
                {
                    window.Background = SystemColors.WindowBrush;
                }
            }
            catch (DllNotFoundException)
            {

            }
        }
    }

    public class Win32
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int en);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);


        [DllImport("User32", EntryPoint = "ClientToScreen", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x = 0;
        public int y = 0;
    }
}
