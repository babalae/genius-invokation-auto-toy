using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Duel
    {
        public Character CurrentCharacter { get; set; }
        public Character[] Characters { get; set; } = new Character[3];

        public List<RoundStrategy> RoundStrategies { get; set; } = new List<RoundStrategy>();

        public int RoundNum { get; set; } = 1;


        protected ImageCapture capture = new ImageCapture();
        protected YuanShenWindow window;
        protected Rectangle windowRect;

        protected int CurrentCardCount;
        protected int CurrentTakenOutCharacterCount;

        protected CancellationTokenSource cts;


        public Duel(YuanShenWindow window)
        {
            this.window = window;
            windowRect = GetWindowRealRect(window);
            capture.Start(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height);
        }

        public Rectangle GetWindowRealRect(YuanShenWindow yuanShenWindow)
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
