using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy
{
    /// <summary>
    /// 用于操控游戏
    /// </summary>
    public class GameControl
    {
        public static ImageCapture capture = new ImageCapture();
        public static YuanShenWindow window = new YuanShenWindow();
        public static Rectangle windowRect;
        public static void Init()
        {
            if (!window.FindYSHandle())
            {
                throw new Exception("未找到原神进程，请先启动原神！");
            }
            windowRect = GetWindowRealRect(window);
            capture.Start(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height);
        }

        public static void FocusGameWindow()
        {
            window.Focus();
        }


        public static Rectangle GetWindowRealRect(YuanShenWindow yuanShenWindow)
        {
            Rectangle rc = yuanShenWindow.GetSize();
            int x = (int)Math.Ceiling(rc.Location.X * PrimaryScreen.ScaleX);
            int y = (int)Math.Ceiling(rc.Location.Y * PrimaryScreen.ScaleY);
            int w = (int)Math.Ceiling(rc.Width * PrimaryScreen.ScaleX);
            int h = (int)Math.Ceiling(rc.Height * PrimaryScreen.ScaleY);
            MyLogger.Debug($"原神窗口大小：{rc.Width} x {rc.Height}");
            MyLogger.Debug($"原神窗口大小(计算DPI缩放后)：{rc.Width * PrimaryScreen.ScaleX} x {rc.Height * PrimaryScreen.ScaleY}");

            System.Drawing.Size size = new System.Drawing.Size(1920, 1080);
            if (w >= 1920 && w < 2000 && h >= 1080 && h < 1150)
            {
                size = new System.Drawing.Size(1920, 1080);
                ImageRecognition.WidthScale = 1;
                //ImageRecognition.HeightScale = 1;
            }
            else if (w >= 1600 && w < 2000 && h >= 900 && h < 950)
            {
                size = new System.Drawing.Size(1600, 900);
                ImageRecognition.WidthScale = 1600 * 1.0 / 1920;
                //ImageRecognition.HeightScale = 900 * 1.0 / 1080;
            }
            else
            {
                ImageRecognition.WidthScale = w * 1.0 / 1929; // 1929是计算了边缘
                //ImageRecognition.HeightScale = h * 1.0 / 1109; // 1109是计算了窗口标题栏
            }

            MyLogger.Debug($"匹配图片缩放比率：{ImageRecognition.WidthScale}");
            return new Rectangle(x, y, w, h);
        }
    }
}
