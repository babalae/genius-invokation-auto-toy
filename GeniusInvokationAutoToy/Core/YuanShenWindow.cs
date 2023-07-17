using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeniusInvokationAutoToy.Core
{
    public class YuanShenWindow
    {
        public static uint WM_LBUTTONDOWN = 0x201; //按下鼠标左键

        public static uint WM_LBUTTONUP = 0x202; //释放鼠标左键


        private IntPtr hWnd;
        public YuanShenWindow()
        {

        }

        public bool FindYSHandle()
        {
            var pros = Process.GetProcessesByName("YuanShen");
            if (pros.Any())
            {
                hWnd = pros[0].MainWindowHandle;
                return true;
            }
            else
            {
                pros = Process.GetProcessesByName("GenshinImpact");
                if (pros.Any())
                {
                    hWnd = pros[0].MainWindowHandle;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Focus()
        {
            Native.SendMessage(hWnd, 0x0112, (IntPtr)0xF120, (IntPtr)0);
            Native.SetForegroundWindow(hWnd);
        }

        public Rectangle GetSize()
        {
            Native.RECT rc = new Native.RECT();
            Native.GetWindowRect(hWnd, ref rc);
            return new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);
        }

        public void MouseLeftButtonDown()
        {
            IntPtr p = (IntPtr)((0 << 16) | 0);
            Native.PostMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, p);

        }

        public void MouseLeftButtonUp()
        {
            IntPtr p = (IntPtr)((0 << 16) | 0);
            Native.PostMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, p);
        }

        public void MouseClick(int x, int y)
        {
            IntPtr p = (IntPtr)((y << 16) | x);
            Native.PostMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, p);
            Thread.Sleep(100);
            Native.PostMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, p);
        }
    }
}
