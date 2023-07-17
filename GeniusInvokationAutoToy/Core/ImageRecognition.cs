using GeniusInvokationAutoToy.Utils;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using OpenCvSharp.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static GeniusInvokationAutoToy.Utils.Native;
using Point = System.Drawing.Point;

namespace GeniusInvokationAutoToy.Core
{
    class ImageRecognition
    {
        public static bool IsDebug = false;

        public static Point FindSingleTarget(Bitmap imgSrc, Bitmap imgSub, double threshold = 0.8)
        {
            Mat srcMat = null;
            Mat dstMat = null;
            try
            {
                srcMat = imgSrc.ToMat();
                dstMat = imgSub.ToMat();
                return FindSingleTarget(srcMat, dstMat, threshold);
            }
            catch (Exception ex)
            {
                return new Point();
            }
            finally
            {
                srcMat?.Dispose();
                dstMat?.Dispose();
            }
        }

        public static Point FindSingleTarget(Mat srcMat, Mat dstMat, double threshold = 0.8)
        {
            Point p = new Point();
            
            OutputArray outArray = null;
            try
            {
                outArray = OutputArray.Create(srcMat);
                Cv2.MatchTemplate(srcMat, dstMat, outArray, TemplateMatchModes.CCoeffNormed);
                double minValue, maxValue;
                OpenCvSharp.Point location, point;
                Cv2.MinMaxLoc(InputArray.Create(outArray.GetMat()), out minValue, out maxValue,
                    out location, out point);

                if (maxValue >= threshold)
                {
                    p = new Point(point.X + dstMat.Width / 2, point.Y + dstMat.Height / 2);
                    if (IsDebug)
                    {
                        var imgTar = srcMat.Clone();
                        Cv2.Rectangle(imgTar, point, new OpenCvSharp.Point(point.X + dstMat.Width, point.Y + dstMat.Height),
                            Scalar.Red, 2);
                        Cv2.PutText(imgTar, maxValue.ToString("0.00"), new OpenCvSharp.Point(point.X, point.Y - 10),
                            HersheyFonts.HersheySimplex, 0.5, Scalar.Red);

                        Cv2.ImShow("识别窗口", imgTar);
                    }

                }

                return p;
            }
            catch (Exception ex)
            {
                MyLogger.Error(ex.ToString());
                return p;
            }
            finally
            {
                outArray?.Dispose();
            }
        }

        //public static List<Point> FindMultiTarget(Bitmap imgSrc, Bitmap imgSub, out Mat resMat, double threshold = 0.9,
        //    int findTargetCount = 8)
        //{
        //    Mat srcMat = null;
        //    Mat dstMat = null;
        //    try
        //    {
        //        srcMat = imgSrc.ToMat();
        //        dstMat = imgSub.ToMat();
        //        return FindMultiTarget(srcMat, dstMat, resMat, threshold, findTargetCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<Point>();
        //    }
        //    finally
        //    {
        //        srcMat?.Dispose();
        //        dstMat?.Dispose();
        //    }
        //}

        public static List<Point> FindMultiTarget(Mat srcMat, Mat dstMat, string title, out Mat resMat,
            double threshold = 0.8,
            int findTargetCount = 8)
        {
            List<Point> pointList = new List<Point>();
            resMat = srcMat.Clone();
            try
            {
                Mat matchResult = new Mat();
                Cv2.MatchTemplate(srcMat, dstMat, matchResult, TemplateMatchModes.CCoeffNormed);

                double minValue = 0;
                double maxValue = 0;
                OpenCvSharp.Point minLoc = new OpenCvSharp.Point();

                //寻找最几个最值的位置
                Mat mask = new Mat(matchResult.Height, matchResult.Width, MatType.CV_8UC1, Scalar.White);
                Mat maskSub = new Mat(matchResult.Height, matchResult.Width, MatType.CV_8UC1, Scalar.Black);
                var point = new OpenCvSharp.Point(0, 0);
                for (int i = 0; i < findTargetCount; i++)
                {
                    Cv2.MinMaxLoc(matchResult, out minValue, out maxValue, out minLoc, out point, mask);
                    Rect maskRect = new Rect(point.X - dstMat.Width / 2, point.Y - dstMat.Height / 2, dstMat.Width,
                        dstMat.Height);
                    maskSub.Rectangle(maskRect, Scalar.White, -1);
                    mask -= maskSub;
                    if (maxValue >= threshold)
                    {
                        pointList.Add(new Point(point.X + dstMat.Width / 2, point.Y + dstMat.Height / 2));

                        if (IsDebug)
                        {
                            MyLogger.Info(title + " " + maxValue.ToString("0.000") + " " + point);
                            Cv2.Rectangle(resMat, point,
                                new OpenCvSharp.Point(point.X + dstMat.Width, point.Y + dstMat.Height),
                                Scalar.Red, 2);
                            Cv2.PutText(resMat, title + " " + maxValue.ToString("0.00"),
                                new OpenCvSharp.Point(point.X, point.Y - 10),
                                HersheyFonts.HersheySimplex, 0.5, Scalar.Red);
                        }

                    }
                    else
                    {
                        break;
                    }
                }

                if (IsDebug)
                {
                    Cv2.ImShow("识别窗口", resMat);
                }

                return pointList;
            }
            catch (Exception ex)
            {
                MyLogger.Error(ex.ToString());
                return pointList;
            }
            finally
            {
                srcMat?.Dispose();
                dstMat?.Dispose();
            }
        }


        public static Dictionary<string, List<Point>> FindPicFromImage(Bitmap imgSrc,
            Dictionary<string, Bitmap> imgSubDictionary, double threshold = 0.8)
        {
            Dictionary<string, List<Point>> dictionary = new Dictionary<string, List<Point>>();
            Mat srcMat = imgSrc.ToMat();
            Mat resMat;

            foreach (KeyValuePair<string, Bitmap> kvp in imgSubDictionary)
            {
                dictionary.Add(kvp.Key, FindMultiTarget(srcMat, kvp.Value.ToMat(), kvp.Key, out resMat, threshold));
                srcMat = resMat.Clone();
                if (IsDebug)
                {
                    MyLogger.Info($"{kvp.Key} 识别完成");
                }
            }

            return dictionary;
        }

        public static Dictionary<string, List<Point>> FindPicFromImage(Mat srcMat,
            Dictionary<string, Bitmap> imgSubDictionary, double threshold = 0.8)
        {
            Dictionary<string, List<Point>> dictionary = new Dictionary<string, List<Point>>();
            Mat resMat;
            foreach (KeyValuePair<string, Bitmap> kvp in imgSubDictionary)
            {
                dictionary.Add(kvp.Key, FindMultiTarget(srcMat, kvp.Value.ToMat(), kvp.Key, out resMat, threshold));
                srcMat = resMat.Clone();
                if (IsDebug)
                {
                    MyLogger.Info($"{kvp.Key} 识别完成");
                }
            }

            return dictionary;
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Mat Threshold(Mat src, Scalar lowPurple, Scalar highPurple)
        {
            Mat mask = new Mat();
            using (Mat rgbMat = new Mat())
            {
                Cv2.CvtColor(src, rgbMat, ColorConversionCodes.BGR2RGB);
                Cv2.InRange(rgbMat, lowPurple, highPurple, mask);
                Cv2.Threshold(mask, mask, 0, 255, ThresholdTypes.Binary); //二值化
                //Cv2.ImShow("识别窗口", mask);
                return mask;
            }
        }

        public static int[] HorizontalProjection(Mat gray)
        {
            var projection = new int[gray.Height];
            //对每一行计算投影值
            for (int y = 0; y < gray.Height; ++y)
            {
                //遍历这一行的每一个像素，如果是有效的，累加投影值
                for (int x = 0; x < gray.Width; ++x)
                {
                    var s = gray.Get<Vec2b>(y, x);
                    if (s.Item0 == 255)
                    {
                        projection[y]++;
                    }
                }
            }

            return projection;
        }

        public static int[] VerticalProjection(Mat gray)
        {
            var projection = new int[gray.Width];
            //遍历每一列计算投影值
            for (int x = 0; x < gray.Width; ++x)
            {
                for (int y = 0; y < gray.Height; ++y)
                {
                    var s = gray.Get<Vec2b>(y, x);
                    if (s.Item0 == 255)
                    {
                        projection[x]++;
                    }
                }
            }

            return projection;
        }

    }
}