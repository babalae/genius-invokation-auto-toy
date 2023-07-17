using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Utils
{
    public class MouseUtils
    {
        public static void Move(int x, int y)
        {
            Native.mouse_event(Native.MouseEventFlag.Absolute | Native.MouseEventFlag.Move,
                x * 65536 / PrimaryScreen.DESKTOP.Width, y * 65536 / PrimaryScreen.DESKTOP.Height,
                0, 0);
        }

        public static void Move(Point point)
        {
            Move(point.X, point.Y);
        }

        public static void LeftDown()
        {
            Native.mouse_event(Native.MouseEventFlag.LeftDown, 0, 0, 0, 0);
        }

        public static void LeftUp()
        {
            Native.mouse_event(Native.MouseEventFlag.LeftUp, 0, 0, 0, 0);
        }

        public static bool Click(int x, int y)
        {
            if (x == 0 && y==0)
            {
                return false;
            }
            //MyLogger.Info($"Click {x},{y}");
            Move(x, y);
            LeftDown();
            Thread.Sleep(20);
            LeftUp();
            return true;
        }

        public static bool Click(Point point)
        {
            return Click(point.X, point.Y);
        }

        public static bool DoubleClick(Point point)
        {
            Click(point.X, point.Y);
            Thread.Sleep(200);
            return Click(point.X, point.Y);
        }
    }
}