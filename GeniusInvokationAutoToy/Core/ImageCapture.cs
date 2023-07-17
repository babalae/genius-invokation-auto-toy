using GeniusInvokationAutoToy.Utils;
using System;
using System.Drawing;

namespace GeniusInvokationAutoToy.Core
{
    public class ImageCapture
    {
        IntPtr hwnd;
        IntPtr hdc;

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public void Start(int x, int y, int w, int h)
        {
            hwnd = Native.GetDesktopWindow();
            hdc = Native.GetDC(hwnd);

            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
        }

        public Bitmap Capture()
        {
            Bitmap bmp = new Bitmap(W, H);
            Graphics bmpGraphic = Graphics.FromImage(bmp);
            //get handle to source graphic
            IntPtr bmpHdc = bmpGraphic.GetHdc();

            //copy it
            bool res = Native.StretchBlt(bmpHdc, 0, 0, W, H,
                hdc, X, Y, W, H, Native.CopyPixelOperation.SourceCopy);
            bmpGraphic.ReleaseHdc();
            return bmp;
        }

        public void Stop()
        {
            Native.ReleaseDC(hwnd, hdc);
        }
    }
}
