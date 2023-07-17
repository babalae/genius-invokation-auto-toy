using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Utils;
using GeniusInvokationAutoToy.Utils.Extension;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog.LayoutRenderers;
using Point = System.Drawing.Point;

namespace GeniusInvokationAutoToy.Strategy
{
    public class BaseStrategy
    {
        protected ImageCapture capture = new ImageCapture();
        protected YuanShenWindow window;
        protected Rectangle windowRect;

        protected int CurrentCardCount = 0;


        public List<Rectangle> MyCharacterRects { get; set; }


        public BaseStrategy(YuanShenWindow window)
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
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// 获取我方三个角色卡牌区域
        /// </summary>
        /// <returns></returns>
        public List<Rectangle> GetCharacterRects()
        {
            Mat srcMat = capture.Capture().ToMat();

            var lowPurple = new Scalar(235, 245, 198);
            var highPurple = new Scalar(255, 255, 236);
            Mat gray = ImageRecognition.Threshold(srcMat, lowPurple, highPurple);

            // 水平投影到y轴 正常只有一个连续区域
            int[] h = ImageRecognition.HorizontalProjection(gray);

            // y轴 从上到下确认连续区域
            int y1 = 0, y2 = 0;
            int start = 0;
            bool inLine = false;
            for (int i = 0; i < h.Length; i++)
            {
                if (h[i] > h.Average() * 10)
                {
                    // 直方图
                    Cv2.Line(srcMat, 0, i, h[i], i, Scalar.Yellow);

                    if (!inLine)
                    {
                        //由空白进入字符区域了，记录标记
                        inLine = true;
                        start = i;
                    }
                }
                else if (inLine)
                {
                    //由连续区域进入空白区域了
                    inLine = false;


                    if (y1 == 0)
                    {
                        y1 = start;
                        if (ImageRecognition.IsDebug)
                        {
                            Cv2.Line(srcMat, 0, y1, srcMat.Width, y1, Scalar.Red);
                        }
                    }
                    else if (y2 == 0)
                    {
                        y2 = i;
                        if (ImageRecognition.IsDebug)
                        {
                            Cv2.Line(srcMat, 0, y2, srcMat.Width, y2, Scalar.Red);
                        }

                        break;
                    }
                }
            }

            if (y1 == 0 || y2 == 0)
            {
                MyLogger.Warn("未识别到卡牌区域（Y轴）");
                if (ImageRecognition.IsDebug)
                {
                    Cv2.ImShow("识别窗口", srcMat);
                }

                return null;
            }


            // 垂直投影
            int[] v = ImageRecognition.VerticalProjection(gray);


            inLine = false;
            start = 0;
            List<int> colLines = new List<int>();
            //开始根据投影值识别分割点
            for (int i = 0; i < v.Length; ++i)
            {
                if (v[i] > h.Average() * 10)
                {
                    if (ImageRecognition.IsDebug)
                    {
                        Cv2.Line(srcMat, i, 0, i, v[i], Scalar.Yellow);
                    }

                    if (!inLine)
                    {
                        //由空白进入字符区域了，记录标记
                        inLine = true;
                        start = i;
                    }
                }
                else if (i - start > 30 && inLine)
                {
                    //由连续区域进入空白区域了
                    inLine = false;
                    if (ImageRecognition.IsDebug)
                    {
                        Cv2.Line(srcMat, start, 0, start, srcMat.Height, Scalar.Red);
                    }

                    colLines.Add(start);
                }
            }

            if (colLines.Count != 6)
            {
                MyLogger.Warn("未识别到卡牌区域（X轴存在{}个识别点）", colLines.Count);
                if (ImageRecognition.IsDebug)
                {
                    Cv2.ImShow("识别窗口", srcMat);
                }

                return null;
            }

            List<Rectangle> rects = new List<Rectangle>();
            for (int i = 0; i < colLines.Count - 1; i++)
            {
                if (i % 2 == 0)
                {
                    var r = new Rectangle(colLines[i], y1, colLines[i + 1] - colLines[i], y2 - y1);
                    rects.Add(r);
                    Cv2.Rectangle(srcMat, r.ToCvRect(), Scalar.Red, 2);
                }
            }

            if (ImageRecognition.IsDebug)
            {
                Cv2.ImShow("识别窗口", srcMat);
            }

            return rects;
        }

        /// <summary>
        /// 选择角色牌（首次）
        /// </summary>
        /// <param name="characterIndex">从左到右第几个角色，从1开始计数</param>
        /// <returns></returns>
        public bool ChooseCharacterFirst(int characterIndex)
        {
            // 首次执行获取角色区域
            if (MyCharacterRects == null || MyCharacterRects.Count == 0)
            {
                MyCharacterRects = GetCharacterRects();
                if (MyCharacterRects == null || MyCharacterRects.Count != 3)
                {
                    return false;
                }
            }


            // 双击选择角色出战
            MouseUtils.DoubleClick(MakeOffset(MyCharacterRects[characterIndex - 1].GetCenterPoint()));
            return true;
        }

        ///// <summary>
        ///// 切换角色牌（历次）
        ///// </summary>
        ///// <param name="characterIndex">从左到右第几个角色，从1开始计数</param>
        ///// <returns></returns>
        //public bool SwitchCharacterWhenActiveCharacterTakenOut(int characterIndex)
        //{
        //    if (MyCharacterRects == null || MyCharacterRects.Count != 3)
        //    {
        //        return false;
        //    }

        //    // 选择角色
        //    MouseUtils.Click(MakeOffset(MyCharacterRects[characterIndex - 1].GetCenterPoint()));
        //    // 点击切人按钮
        //    ActionPhasePressSwitchButton();
        //    return true;
        //}


        /// <summary>
        /// 切换角色牌（历次）
        /// </summary>
        /// <param name="characterIndex">从左到右第几个角色，从1开始计数</param>
        /// <returns></returns>
        public bool SwitchCharacterLater(int characterIndex)
        {
            if (MyCharacterRects == null || MyCharacterRects.Count != 3)
            {
                return false;
            }

            Point p = MakeOffset(MyCharacterRects[characterIndex - 1].GetCenterPoint());
            // 选择角色
            MouseUtils.Click(p);
            // // 双击切人
            // Thread.Sleep(1500);
            // MouseUtils.Click(p);

            // 点击切人按钮
            ActionPhasePressSwitchButton();
            return true;
        }

        /// <summary>
        /// 角色死亡的时候双击角色牌重新出战
        /// </summary>
        /// <param name="characterIndex">从左到右第几个角色，从1开始计数</param>
        /// <returns></returns>
        public bool SwitchCharacterWhenTakenOut(int characterIndex)
        {
            if (MyCharacterRects == null || MyCharacterRects.Count != 3)
            {
                return false;
            }

            Point p = MakeOffset(MyCharacterRects[characterIndex - 1].GetCenterPoint());
            // 选择角色
            MouseUtils.Click(p);
            // // 双击切人
            Thread.Sleep(500);
            MouseUtils.Click(p);

            return true;
        }

        /// <summary>
        ///  点击游戏屏幕中心点
        /// </summary>
        public void ClickGameWindowCenter()
        {
            MouseUtils.Click(windowRect.GetCenterPoint());
        }


        ///// <summary>
        ///// 重投骰子
        ///// </summary>
        ///// <param name="holdElementalTypes">保留的元素类型</param>
        //public void RollPhaseReRoll2(params ElementalType[] holdElementalTypes)
        //{
        //    Bitmap gameSnapshot = capture.Capture();

        //    Mat srcMat = gameSnapshot.ToMat();
        //    Mat resMat = null;
        //    foreach (KeyValuePair<string, Bitmap> kvp in ImageResCollections.RollPhaseDiceBitmaps)
        //    {
        //        // 跳过保留的元素类型
        //        if (holdElementalTypes.Contains(kvp.Key.ToElementalType()))
        //        {
        //            continue;
        //        }

        //        // 识别
        //        List<Point> points =
        //            ImageRecognition.FindMultiTarget(srcMat, kvp.Value.ToMat(), kvp.Key, out resMat, 0.83);
        //        srcMat = resMat.Clone();
        //        MyLogger.Info($"{kvp.Key} 识别完成,找到{points.Count}个骰子");

        //        // 选中重投
        //        foreach (var point in points)
        //        {
        //            MouseUtils.Click(point.X + windowRect.X, point.Y + windowRect.Y);
        //            Thread.Sleep(100);
        //        }
        //    }

        //    Thread.Sleep(1000);
        //    //Cv2.ImShow("识别窗口", resMat);
        //}

        /// <summary>
        /// 重投骰子
        /// </summary>
        /// <param name="holdElementalTypes">保留的元素类型</param>
        public bool RollPhaseReRoll(params ElementalType[] holdElementalTypes)
        {
            Bitmap gameSnapshot = capture.Capture();


            Dictionary<string, List<Point>> dictionary =
                ImageRecognition.FindPicFromImage(gameSnapshot, ImageResCollections.RollPhaseDiceBitmaps, 0.75);

            int count = 0;
            foreach (KeyValuePair<string, List<Point>> kvp in dictionary)
            {
                count += kvp.Value.Count;
            }

            MyLogger.Info($"骰子界面识别到了{count}个骰子");
            if (count != 8)
            {
                return false;
            }

            foreach (KeyValuePair<string, List<Point>> kvp in dictionary)
            {
                // 跳过保留的元素类型
                if (holdElementalTypes.Contains(kvp.Key.ToElementalType()))
                {
                    continue;
                }

                // 选中重投
                foreach (var point in kvp.Value)
                {
                    MouseUtils.Click(point.X + windowRect.X, point.Y + windowRect.Y);
                    Thread.Sleep(100);
                }
            }

            return true;
        }

        /// <summary>
        ///  选择手牌/重投骰子 确认
        /// </summary>
        public bool ClickConfirm()
        {
            Point p = ImageRecognition.FindSingleTarget(capture.Capture().ToMat(),
                ImageResCollections.ConfirmButton.ToMat());
            return MouseUtils.Click(MakeOffset(p));
        }


        public Point MakeOffset(Point p)
        {
            return new Point(p.X + windowRect.X, p.Y + windowRect.Y);
        }

        /// <summary>
        /// 计算当前有那些骰子
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> ActionPhaseDice()
        {
            Mat srcMat = capture.Capture().ToMat();
            // 切割图片后再识别 加快速度 位置没啥用，所以切割后比较方便
            Dictionary<string, List<Point>> dictionary =
                ImageRecognition.FindPicFromImage(CutRight(srcMat, srcMat.Width / 5),
                    ImageResCollections.ActionPhaseDiceBitmaps, 0.7);

            string msg = "";
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (KeyValuePair<string, List<Point>> kvp in dictionary)
            {
                result.Add(kvp.Key, kvp.Value.Count);
                msg += $"{kvp.Key.ToElementalType().ToChinese()} {kvp.Value.Count}| ";
            }

            MyLogger.Info($"当前骰子状态：{msg.Remove(msg.Length - 2, 1)}");
            return result;
        }


        /// <summary>
        ///  烧牌
        /// </summary>
        public void ActionPhaseElementalTuning()
        {
            MouseUtils.Click(windowRect.X + windowRect.Width / 2, windowRect.Y + windowRect.Height - 50);
            Thread.Sleep(1500);
            MouseUtils.LeftDown();
            Thread.Sleep(100);
            MouseUtils.Move(windowRect.X + windowRect.Width - 50, windowRect.Y + windowRect.Height / 2);
            Thread.Sleep(100);
            MouseUtils.LeftUp();
        }

        /// <summary>
        ///  烧牌确认（元素调和按钮）
        /// </summary>
        public void ActionPhaseElementalTuningConfirm()
        {
            Point p = ImageRecognition.FindSingleTarget(capture.Capture().ToMat(),
                ImageResCollections.ElementalTuningConfirmButton.ToMat());
            MouseUtils.Click(MakeOffset(p));
        }

        /// <summary>
        /// 点击切人按钮
        /// </summary>
        /// <returns></returns>
        public bool ActionPhasePressSwitchButton()
        {
            int x = windowRect.X + windowRect.Width - 100;
            int y = windowRect.Y + windowRect.Height - 120;
            MouseUtils.Click(x, y);
            Thread.Sleep(800); // 等待动画彻底弹出

            return MouseUtils.Click(x, y);
        }


        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skillIndex">技能编号,从右往左数,从1开始</param>
        /// <returns>元素骰子是否充足</returns>
        public bool ActionPhaseUseSkill(int skillIndex)
        {
            // 技能坐标写死 (w - 100 * n, h - 120)
            int x = windowRect.X + windowRect.Width - 100 * skillIndex;
            int y = windowRect.Y + windowRect.Height - 120;
            MouseUtils.Click(x, y);
            Thread.Sleep(600); // 等待动画彻底弹出

            Mat srcMat = capture.Capture().ToMat();
            Point p = ImageRecognition.FindSingleTarget(CutRight(srcMat, srcMat.Width / 2),
                ImageResCollections.ElementalDiceLackWarning.ToMat());
            if (p.IsEmpty)
            {
                // 多点几次保证点击到
                Thread.Sleep(500);
                MyLogger.Info($"使用技能{skillIndex}");
                MouseUtils.Click(x, y);
                Thread.Sleep(200);
                return MouseUtils.Click(x, y);
            }

            return false;
        }

        /// <summary>
        /// 使用技能（元素骰子不够的情况下，自动烧牌）
        /// </summary>
        /// <param name="skillIndex">技能编号,从右往左数,从1开始</param>
        /// <param name="diceCost">技能消耗骰子数</param>
        /// <param name="elementalType">消耗骰子元素类型</param>
        /// <param name="expectDiceCount">期望的骰子数量</param>
        /// <returns>手牌或者元素骰子是否充足</returns>
        public bool ActionPhaseAutoUseSkill(int skillIndex, int diceCost, ElementalType elementalType,
            int expectDiceCount)
        {
            int retryCount = 0;
            Dictionary<string, int> diceStatus = ActionPhaseDice();
            while (true)
            {
                if (diceStatus.Sum(x => x.Value) != expectDiceCount)
                {
                    if (retryCount > 20)
                    {
                        throw new Exception("骰子数量与预期不符，重试次数过多，可能出现了未知错误！");
                        break;
                    }

                    MyLogger.Info("当前骰子数量{}与期望的骰子数量{}不相等，重试", diceStatus.Sum(x => x.Value), expectDiceCount);
                    diceStatus = ActionPhaseDice();
                    retryCount++;
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }


            int needHydroDiceCount = diceCost - diceStatus[ElementalType.Omni.ToLowerString()] -
                                     diceStatus[elementalType.ToLowerString()];
            if (needHydroDiceCount > 0)
            {
                if (CurrentCardCount < needHydroDiceCount)
                {
                    MyLogger.Info("当前手牌数{}小于需要烧牌数量{}，无法释放技能", CurrentCardCount, needHydroDiceCount);
                    return false;
                }

                MyLogger.Info("当前需要的元素骰子数量不足3个，还缺{}个，当前手牌数{}，烧牌", needHydroDiceCount, CurrentCardCount);

                for (int i = 0; i < needHydroDiceCount; i++)
                {
                    CurrentCardCount--;
                    MyLogger.Info("- {} 烧牌", i + 1);
                    ActionPhaseElementalTuning();
                    Thread.Sleep(100);
                    ActionPhaseElementalTuningConfirm();
                    Thread.Sleep(1000); // 烧牌动画
                    ClickGameWindowCenter(); // 复位
                    Thread.Sleep(500);
                }
            }

            return ActionPhaseUseSkill(skillIndex);
        }


        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundEnd()
        {
            Mat srcMat = capture.Capture().ToMat();
            // 切割图片后再识别 加快速度 左上切割 不影响坐标
            Point p = ImageRecognition.FindSingleTarget(CutLeft(srcMat, srcMat.Width / 5),
                ImageResCollections.RoundEndButton.ToMat());
            MouseUtils.Click(MakeOffset(p));
            Thread.Sleep(1000); // 有弹出动画 
            MouseUtils.Click(MakeOffset(p));
        }

        /// <summary>
        /// 是否是我的回合
        /// </summary>
        /// <returns></returns>
        public bool IsInMyAction()
        {
            Mat srcMat = capture.Capture().ToMat();
            Point p = ImageRecognition.FindSingleTarget(CutLeft(srcMat, srcMat.Width / 5),
                ImageResCollections.InMyActionBitmap.ToMat());
            return !p.IsEmpty;
        }

        /// <summary>
        /// 是否是对方的回合
        /// </summary>
        /// <returns></returns>
        public bool IsInOpponentAction()
        {
            Mat srcMat = capture.Capture().ToMat();
            Point p = ImageRecognition.FindSingleTarget(CutLeft(srcMat, srcMat.Width / 5),
                ImageResCollections.InOpponentActionBitmap.ToMat());
            return !p.IsEmpty;
        }

        /// <summary>
        /// 是否是回合结算阶段
        /// </summary>
        /// <returns></returns>
        public bool IsEndPhase()
        {
            Mat srcMat = capture.Capture().ToMat();
            Point p = ImageRecognition.FindSingleTarget(CutLeft(srcMat, srcMat.Width / 5),
                ImageResCollections.EndPhaseBitmap.ToMat());
            return !p.IsEmpty;
        }


        /// <summary>
        /// 出战角色是否被打倒
        /// </summary>
        /// <returns></returns>
        public bool IsActiveCharacterTakenOut()
        {
            Point p = ImageRecognition.FindSingleTarget(capture.Capture().ToMat(),
                ImageResCollections.CharacterTakenOutBitmap.ToMat());
            return !p.IsEmpty;
        }


        public Mat CutRight(Mat srcMat, int saveRightWidth)
        {
            srcMat = new Mat(srcMat, new Rect(srcMat.Width - saveRightWidth, 0, saveRightWidth, srcMat.Height));
            return srcMat;
        }

        public Mat CutLeft(Mat srcMat, int saveLeftWidth)
        {
            srcMat = new Mat(srcMat, new Rect(0, 0, saveLeftWidth, srcMat.Height));
            return srcMat;
        }
    }
}