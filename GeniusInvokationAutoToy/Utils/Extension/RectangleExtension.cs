using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Utils.Extension
{
    public static class RectangleExtension
    {
        public static OpenCvSharp.Rect ToCvRect(this Rectangle rectangle)
        {
            return new OpenCvSharp.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle ToRectangle(this OpenCvSharp.Rect rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Point GetCenterPoint(this Rectangle rectangle)
        {
            if (rectangle.IsEmpty)
            {
                throw new ArgumentException("rectangle is empty");
            }
            return new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
        }
    }
}
