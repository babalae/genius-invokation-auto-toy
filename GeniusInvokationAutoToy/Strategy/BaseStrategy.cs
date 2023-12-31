﻿using GeniusInvokationAutoToy.Core;
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
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Strategy.Model;
using NLog.LayoutRenderers;
using Point = System.Drawing.Point;
using GeniusInvokationAutoToy.Strategy.Model.Old;

namespace GeniusInvokationAutoToy.Strategy
{
    [Obsolete]
    public abstract class BaseStrategy
    {
        public static bool OutputImageWhenError = true;

        protected ImageCapture capture = new ImageCapture();
        protected YuanShenWindow window;
        protected Rectangle windowRect;

        protected int CurrentCardCount { get; set; }
        protected int CurrentTakenOutCharacterCount { get; set; }
        protected int CurrentActiveCharacterArrayIndex { get; set; }
        protected CurrentCharacterStatus CurrentCharacterStatus;
        protected int CurrentDiceCount = 0;


        protected CancellationTokenSource cts;


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

        protected void Init()
        {
            CurrentCardCount = 0;
            CurrentTakenOutCharacterCount = 0;
        }

        protected void Sleep(int millisecondsTimeout)
        {
            CheckTaskCancel();
            Thread.Sleep(millisecondsTimeout);
        }

        protected Bitmap Capture()
        {
            CheckTaskCancel();
            return capture.Capture();
        }

        protected void CheckTaskCancel()
        {
            if (cts != null && cts.IsCancellationRequested)
            {
                throw new TaskCanceledException("任务取消");
            }
        }

        public abstract void Run(CancellationTokenSource cts1);

        public async Task RunAsync(CancellationTokenSource cts1)
        {
            await Task.Run(() => { Run(cts1); });
        }


        protected void CommonDuelPrepare()
        {
            // 1. 选择初始手牌
            Sleep(1000);
            MyLogger.Info("开始选择初始手牌");
            while (!ClickConfirm())
            {
                // 循环等待选择卡牌画面
                Sleep(1000);
            }

            MyLogger.Info("点击确认");

            // 2. 选择出战角色
            // 此处选择第2个角色 雷神
            MyLogger.Info("等待3s动画...");
            Sleep(3000);

            // 是否是再角色出战选择界面
            Retry.Do(IsInCharacterPickRetryThrowable, TimeSpan.FromSeconds(1), 5);
            MyLogger.Info("识别到已经在角色出战界面，等待1.5s");
            Sleep(1500);
        }

        /// <summary>
        /// 获取我方三个角色卡牌区域
        /// </summary>
        /// <returns></returns>
        public List<Rectangle> GetCharacterRects()
        {
            Mat srcMat = Capture().ToMat();

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
                        if (ImageRecognition.IsDebug || OutputImageWhenError)
                        {
                            Cv2.Line(srcMat, 0, y1, srcMat.Width, y1, Scalar.Red);
                        }
                    }
                    else if (y2 == 0)
                    {
                        y2 = i;
                        if (ImageRecognition.IsDebug || OutputImageWhenError)
                        {
                            Cv2.Line(srcMat, 0, y2, srcMat.Width, y2, Scalar.Red);
                        }

                        break;
                    }
                }
            }

            if (y1 == 0 || y2 == 0)
            {
                MyLogger.Warn("未识别到角色卡牌区域（Y轴）");
                if (ImageRecognition.IsDebug)
                {
                    Cv2.ImShow("识别窗口", srcMat);
                }

                if (OutputImageWhenError)
                {
                    Cv2.ImWrite("logs\\character_card_error.jpg", srcMat);
                }

                return null;
            }

            if (y1 < windowRect.Height / 2 || y2 < windowRect.Height / 2)
            {
                MyLogger.Warn("识别的角色卡牌区域（Y轴）错误：y1:{} y2:{}", y1, y2);
                if (ImageRecognition.IsDebug)
                {
                    Cv2.ImShow("识别窗口", srcMat);
                }

                if (OutputImageWhenError)
                {
                    Cv2.ImWrite("logs\\character_card_error.jpg", srcMat);
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
                    if (ImageRecognition.IsDebug || OutputImageWhenError)
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
                    if (ImageRecognition.IsDebug || OutputImageWhenError)
                    {
                        Cv2.Line(srcMat, start, 0, start, srcMat.Height, Scalar.Red);
                    }

                    colLines.Add(start);
                }
            }

            if (colLines.Count != 6)
            {
                MyLogger.Warn("未识别到角色卡牌区域（X轴存在{}个识别点）", colLines.Count);
                if (ImageRecognition.IsDebug)
                {
                    Cv2.ImShow("识别窗口", srcMat);
                }

                if (OutputImageWhenError)
                {
                    Cv2.ImWrite("logs\\character_card_error.jpg", srcMat);
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
        public void ChooseCharacterFirst(int characterIndex)
        {
            // 首次执行获取角色区域
            if (MyCharacterRects == null || MyCharacterRects.Count == 0)
            {
                MyCharacterRects = GetCharacterRects();
                if (MyCharacterRects == null || MyCharacterRects.Count != 3)
                {
                    throw new RetryException("未获取到角色区域");
                }
            }


            // 双击选择角色出战
            MouseUtils.DoubleClick(MakeOffset(MyCharacterRects[characterIndex - 1].GetCenterPoint()));
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
            // Sleep(1500);
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
            // 双击切人
            Sleep(500);
            MouseUtils.Click(p);
            Sleep(300);
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
        //    Bitmap gameSnapshot = Capture();

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
        //            Sleep(100);
        //        }
        //    }

        //    Sleep(1000);
        //    //Cv2.ImShow("识别窗口", resMat);
        //}

        /// <summary>
        /// 重投骰子
        /// </summary>
        /// <param name="holdElementalTypes">保留的元素类型</param>
        public bool RollPhaseReRoll(params ElementalType[] holdElementalTypes)
        {
            Bitmap gameSnapshot = Capture();


            Dictionary<string, List<Point>> dictionary =
                ImageRecognition.FindPicFromImage(gameSnapshot, ImageResCollections.RollPhaseDiceBitmaps, 0.73);

            int count = 0;
            foreach (KeyValuePair<string, List<Point>> kvp in dictionary)
            {
                count += kvp.Value.Count;
            }


            if (count != 8)
            {
                MyLogger.Warn($"骰子界面识别到了{count}个骰子");
                return false;
            }
            else
            {
                MyLogger.Info($"骰子界面识别到了{count}个骰子");
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
                    Sleep(100);
                }
            }

            return true;
        }

        /// <summary>
        ///  选择手牌/重投骰子 确认
        /// </summary>
        public bool ClickConfirm()
        {
            Point p = ImageRecognition.FindSingleTarget(Capture().ToMat(),
                ImageResCollections.ConfirmButton.ToMat());
            if (p.IsEmpty)
            {
                return false;
            }

            return MouseUtils.Click(MakeOffset(p));
        }

        public void ReRollDice(params ElementalType[] holdElementalTypes)
        {
            // 3.重投骰子
            MyLogger.Info("等待5s投骰动画...");
            Sleep(5000);
            int retryCount = 0;
            // 保留 风、水、万能 骰子
            while (!RollPhaseReRoll(holdElementalTypes))
            {
                retryCount++;

                if (IsDuelEnd())
                {
                    throw new DuelEndException("对战已结束,停止自动打牌！");
                }

                MyLogger.Warn("识别骰子数量不正确,第{}次重试中...", retryCount);
                Sleep(1000);
                if (retryCount > 20)
                {
                    throw new Exception("识别骰子数量不正确,重试超时,停止自动打牌！");
                }
            }

            ClickConfirm();
            MyLogger.Info("选择需要重投的骰子后点击确认完毕");

            Sleep(1000);
            // 鼠标移动到中心
            MouseUtils.Move(windowRect.GetCenterPoint());

            MyLogger.Info("等待10s对方重投");
            Sleep(10000);
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
            Mat srcMat = Capture().ToMat();
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
            Sleep(1500);
            MouseUtils.LeftDown();
            Sleep(100);
            MouseUtils.Move(windowRect.X + windowRect.Width - 50, windowRect.Y + windowRect.Height / 2);
            Sleep(100);
            MouseUtils.LeftUp();
        }

        /// <summary>
        ///  烧牌确认（元素调和按钮）
        /// </summary>
        public void ActionPhaseElementalTuningConfirm()
        {
            Point p = ImageRecognition.FindSingleTarget(Capture().ToMat(),
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
            Sleep(800); // 等待动画彻底弹出

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
            Sleep(600); // 等待动画彻底弹出

            Mat srcMat = Capture().ToMat();
            Point p = ImageRecognition.FindSingleTarget(CutRight(srcMat, srcMat.Width / 2),
                ImageResCollections.ElementalDiceLackWarning.ToMat());
            if (p.IsEmpty)
            {
                // 多点几次保证点击到
                Sleep(500);
                MyLogger.Info($"使用技能{skillIndex}");
                MouseUtils.Click(x, y);
                Sleep(200);
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
            int dice9RetryCount = 0;
            int retryCount = 0;
            Dictionary<string, int> diceStatus = ActionPhaseDice();
            while (true)
            {
                int dCount = diceStatus.Sum(x => x.Value);
                if (dCount != expectDiceCount)
                {

                    if (retryCount > 20)
                    {
                        throw new Exception("骰子数量与预期不符，重试次数过多，可能出现了未知错误！");
                    }

                    if (dCount == 9 && expectDiceCount == 8 && diceStatus[ElementalType.Omni.ToLowerString()] > 0)
                    {
                        dice9RetryCount++;
                        if (dice9RetryCount > 5)
                        {
                            // 支援区存在 鲸井小弟 情况下骰子数量增加导致识别出错的问题 #1
                            // 5次重试后仍然是9个骰子并且至少有一个万能骰子，出现多识别的情况是很稀少的，此时可以基本认为 支援区存在 鲸井小弟
                            // TODO : 但是这个方法并不是100%准确，后续需要添加支援区判断
                            MyLogger.Info("期望的骰子数量8，应为开局期望，重试多次后累计实际识别9个骰子的情况为5次", dCount, expectDiceCount);
                            CurrentDiceCount = 9; // 修正当前骰子数量
                            break;
                        }
                    }


                    MyLogger.Info("当前骰子数量{}与期望的骰子数量{}不相等，重试", dCount, expectDiceCount);
                    diceStatus = ActionPhaseDice();
                    retryCount++;
                    Sleep(1000);
                }
                else
                {
                    break;
                }
            }


            int needSpecifyElementDiceCount = diceCost - diceStatus[ElementalType.Omni.ToLowerString()] -
                                              diceStatus[elementalType.ToLowerString()];
            if (needSpecifyElementDiceCount > 0)
            {
                if (CurrentCardCount < needSpecifyElementDiceCount)
                {
                    MyLogger.Info("当前手牌数{}小于需要烧牌数量{}，无法释放技能", CurrentCardCount, needSpecifyElementDiceCount);
                    return false;
                }

                MyLogger.Info("当前需要的元素骰子数量不足{}个，还缺{}个，当前手牌数{}，烧牌", diceCost, needSpecifyElementDiceCount,
                    CurrentCardCount);

                for (int i = 0; i < needSpecifyElementDiceCount; i++)
                {
                    CurrentCardCount--;
                    MyLogger.Info("- {} 烧牌", i + 1);
                    ActionPhaseElementalTuning();
                    Sleep(100);
                    ActionPhaseElementalTuningConfirm();
                    Sleep(1000); // 烧牌动画
                    ClickGameWindowCenter(); // 复位
                    Sleep(500);
                }
            }

            return ActionPhaseUseSkill(skillIndex);
        }


        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundEnd()
        {
            Mat srcMat = Capture().ToMat();
            // 切割图片后再识别 加快速度 左上切割 不影响坐标
            Point p = ImageRecognition.FindSingleTarget(CutLeft(srcMat, srcMat.Width / 5),
                ImageResCollections.RoundEndButton.ToMat());
            MouseUtils.Click(MakeOffset(p));
            Sleep(1000); // 有弹出动画 
            MouseUtils.Click(MakeOffset(p));
            Sleep(300);
            ClickGameWindowCenter(); // 复位
        }

        /// <summary>
        /// 是否是再角色出战选择界面
        /// 可重试方法
        /// </summary>
        public void IsInCharacterPickRetryThrowable()
        {
            if (!IsInCharacterPick())
            {
                throw new RetryException("当前不在角色出战选择界面");
            }
        }

        /// <summary>
        /// 是否是再角色出战选择界面
        /// </summary>
        /// <returns></returns>
        public bool IsInCharacterPick()
        {
            Mat srcMat = Capture().ToMat();
            // 切割右下
            srcMat = new Mat(srcMat,
                new Rect(srcMat.Width / 2, srcMat.Height / 2, srcMat.Width - srcMat.Width / 2,
                    srcMat.Height - srcMat.Height / 2));
            Point p = ImageRecognition.FindSingleTarget(srcMat, ImageResCollections.InCharacterPickBitmap.ToMat());
            return !p.IsEmpty;
        }

        /// <summary>
        /// 是否是我的回合
        /// </summary>
        /// <returns></returns>
        public bool IsInMyAction()
        {
            Mat srcMat = Capture().ToMat();
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
            Mat srcMat = Capture().ToMat();
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
            Mat srcMat = Capture().ToMat();
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
            Point p = ImageRecognition.FindSingleTarget(Capture().ToMat(),
                ImageResCollections.CharacterTakenOutBitmap.ToMat());
            return !p.IsEmpty;
        }

        /// <summary>
        /// 出战角色是否被打倒2
        /// </summary>
        /// <returns>true 是已经被打倒</returns>
        public bool[] WhatCharacterDefeated()
        {
            if (MyCharacterRects == null || MyCharacterRects.Count != 3)
            {
                throw new Exception("未能获取到我方角色卡位置");
            }

            Mat resMat;
            List<Point> pList = ImageRecognition.FindMultiTarget(Capture().ToMat(),
                ImageResCollections.CharacterDefeatedBitmap.ToMat(), "defeated", out resMat);

            bool[] res = new bool[3];
            foreach (var p in pList)
            {
                if (!p.IsEmpty)
                {
                    for (int i = 0; i < MyCharacterRects.Count; i++)
                    {
                        if (isOverlap(MyCharacterRects[i],
                                new Rectangle(p.X, p.Y, ImageResCollections.CharacterDefeatedBitmap.Width,
                                    ImageResCollections.CharacterDefeatedBitmap.Height)))
                        {
                            res[i] = true;
                        }
                    }
                }
            }


            return res;
        }

        /// <summary>
        /// 判断矩形是否重叠
        /// </summary>
        /// <param name="rc1"></param>
        /// <param name="rc2"></param>
        /// <returns></returns>
        public bool isOverlap(Rectangle rc1, Rectangle rc2)
        {
            if (rc1.X + rc1.Width > rc2.X &&
                rc2.X + rc2.Width > rc1.X &&
                rc1.Y + rc1.Height > rc2.Y &&
                rc2.Y + rc2.Height > rc1.Y
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否对局完全结束
        /// </summary>
        /// <returns></returns>
        public bool IsDuelEnd()
        {
            Mat srcMat = Capture().ToMat();
            // 切割左下
            srcMat = new Mat(srcMat,
                new Rect(0, srcMat.Height / 2, srcMat.Width / 2, srcMat.Height - srcMat.Height / 2));
            Point p = ImageRecognition.FindSingleTarget(srcMat,
                ImageResCollections.ExitDuelButton.ToMat());
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

        /// <summary>
        /// 等待我的回合
        /// 我方角色可能在此期间阵亡
        /// </summary>
        public void WaitForMyTurn(int waitTime = 0)
        {
            if (waitTime > 0)
            {
                MyLogger.Info($"等待对方行动{waitTime / 1000}s");
                Sleep(waitTime);
            }

            // 判断对方行动是否已经结束
            int retryCount = 0;
            int inOpponentActionCount = 0;
            int inMyActionCount = 0;
            while (true)
            {
                if (IsInMyAction())
                {
                    if (IsActiveCharacterTakenOut())
                    {
                        CurrentTakenOutCharacterCount++;
                        bool[] defeatedArray = WhatCharacterDefeated();

                        int i;
                        for (i = defeatedArray.Length - 1; i >= 0; i--)
                        {
                            if (!defeatedArray[i])
                            {
                                MyLogger.Info($"我方角色已阵亡，选择新的出战角色{i + 1}");
                                SwitchCharacterWhenTakenOut(i + 1);
                                CurrentActiveCharacterArrayIndex = i;
                                break;
                            }
                        }

                        if (i == -1)
                        {
                            MyLogger.Error("我方角色已阵亡，但是无法确定是哪个角色，使用备用方案");
                            SwitchCharacterWhenTakenOut(3);
                            SwitchCharacterWhenTakenOut(2); // 防止超载切角色，导致切换失败
                            SwitchCharacterWhenTakenOut(1); // 不知道最后死的是谁，所以切换3次
                            CurrentActiveCharacterArrayIndex = -1;
                        }

                        ClickGameWindowCenter();
                        Sleep(2000); // 切人动画
                        inOpponentActionCount = 0;
                    }
                    else
                    {
                        inMyActionCount++;
                        // 至少存在一次在对方行动中
                        //if (inOpponentActionCount > 0)
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    if (inMyActionCount < 3)
                        //    {
                        //        MyLogger.Debug("无法确定对方是否已经行动过，额外等待...");
                        //    }
                        //    else
                        //    {
                        //        MyLogger.Info("认定当前在我方回合,继续操作");
                        //        break;
                        //    }
                        //}
                    }
                }
                else if (IsInOpponentAction())
                {
                    inOpponentActionCount++;
                }
                else if (IsDuelEnd())
                {
                    throw new DuelEndException("对战已结束,停止自动打牌！");
                }

                retryCount++;
                if (retryCount >= 60)
                {
                    throw new Exception("等待对方行动超时,停止自动打牌！");
                }

                MyLogger.Info("对方仍在行动中,继续等待(次数{})...", retryCount);
                Sleep(800);
            }
        }

        /// <summary>
        /// 等待对方回合 和 回合结束阶段
        /// 我方角色可能在此期间阵亡
        /// </summary>
        public void WaitOpponentAction()
        {
            Random rd = new Random();
            Sleep(3000 + rd.Next(1, 1000));
            // 判断对方行动是否已经结束
            int retryCount = 0;
            while (true)
            {
                if (IsInOpponentAction())
                {
                    MyLogger.Info("对方仍在行动中,继续等待(次数{})...", retryCount);
                }
                else if (IsEndPhase())
                {
                    MyLogger.Info("正在回合结束阶段,继续等待(次数{})...", retryCount);
                }
                else if (IsInMyAction())
                {
                    if (IsActiveCharacterTakenOut())
                    {
                        CurrentTakenOutCharacterCount++;
                        MyLogger.Info("我方角色已阵亡，选择新的出战角色");
                        SwitchCharacterWhenTakenOut(3);
                        SwitchCharacterWhenTakenOut(2); // 防止超载切角色，导致切换失败
                        SwitchCharacterWhenTakenOut(1); // 不知道最后死的是谁，所以切换3次
                        ClickGameWindowCenter();
                        MyLogger.Info("依次切换新角色完成，等待2s");
                        Sleep(2000); // 切人动画
                    }
                }
                else if (IsDuelEnd())
                {
                    throw new DuelEndException("对战已结束,停止自动打牌！");
                }
                else
                {
                    // 至少走三次判断才能确定对方行动结束
                    if (retryCount > 2)
                    {
                        break;
                    }
                    else
                    {
                        MyLogger.Error("等待对方回合 和 回合结束阶段 时程序未识别到有效内容(次数{})...", retryCount);
                    }
                }

                retryCount++;
                if (retryCount >= 30)
                {
                    throw new Exception("等待对方行动超时,停止自动打牌！");
                }


                Sleep(1000 + rd.Next(1, 500));
            }
        }

        /// <summary>
        /// 哪个角色处于出战状态
        /// </summary>
        /// <returns></returns>
        public CurrentCharacterStatus WhichCharacterActive()
        {
            if (MyCharacterRects == null || MyCharacterRects.Count != 3)
            {
                throw new Exception("未能获取到我方角色卡位置");
            }
            Mat srcMat = Capture().ToMat();

            // 切割下半部分
            int halfHeight = srcMat.Height / 2;
            Mat bottomMat = new Mat(srcMat, new Rect(0, halfHeight, srcMat.Width, srcMat.Height - halfHeight));
            Mat resMat;
            List<Point> pList = ImageRecognition.FindMultiTarget(bottomMat,
                ImageResCollections.CharacterHpUpperBitmap.ToMat(), "HpUpper", out resMat, 0.7);
            if (pList.Count != 3)
            {
                if (OutputImageWhenError)
                {
                    var outMat = srcMat.Clone();
                    foreach (var point in pList)
                    {
                        Cv2.Rectangle(outMat,
                            new Rect(point.X, point.Y + halfHeight, ImageResCollections.CharacterHpUpperBitmap.Width,
                                ImageResCollections.CharacterHpUpperBitmap.Height), Scalar.Red, 2);
                    }

                    Cv2.ImWrite("logs\\active_character_error.jpg", outMat);
                }

                throw new RetryException("角色Hp区块未识别到");
            }

            // 按照X轴升序
            pList = pList.OrderBy(point => point.X).ToList();

            // 获取出战角色序号
            var currentCharacterStatus = new CurrentCharacterStatus();
            int cnt = 0;
            for (int i = 0; i < pList.Count; i++)
            {
                if (halfHeight + pList[i].Y < MyCharacterRects[i].Y)
                {
                    cnt++;
                    currentCharacterStatus.ArrayIndex = i;
                    CurrentActiveCharacterArrayIndex = i;
                }
            }

            if (cnt != 1)
            {
                if (OutputImageWhenError)
                {
                    var outMat = srcMat.Clone();
                    foreach (var point in pList)
                    {
                        Cv2.Rectangle(outMat,
                            new Rect(point.X, point.Y + halfHeight, ImageResCollections.CharacterHpUpperBitmap.Width,
                                ImageResCollections.CharacterHpUpperBitmap.Height), Scalar.Red, 2);
                    }
                    foreach (var rc in MyCharacterRects)
                    {
                        Cv2.Rectangle(outMat,
                            rc.ToCvRect(), Scalar.Green, 2);
                    }
                    Cv2.ImWrite("logs\\active_character_error.jpg", outMat);
                }
                throw new RetryException($"识别到{cnt}个出战角色");
            }

            // 截取出战角色区域扩展
            Mat characterMat = new Mat(srcMat, new Rect(pList[currentCharacterStatus.ArrayIndex].X,
                pList[currentCharacterStatus.ArrayIndex].Y,
                MyCharacterRects[currentCharacterStatus.ArrayIndex].Width + 40,
                MyCharacterRects[currentCharacterStatus.ArrayIndex].Height +
                MyCharacterRects[currentCharacterStatus.ArrayIndex].Y - pList[currentCharacterStatus.ArrayIndex].Y));
            // 识别角色异常状态
            Point pCharacterStatusFreeze = ImageRecognition.FindSingleTarget(characterMat.Clone(),
                ImageResCollections.CharacterStatusFreezeBitmap.ToMat(), 0.7);
            if (!pCharacterStatusFreeze.IsEmpty)
            {
                currentCharacterStatus.NegativeStatusList.Add(CharacterStatusEnum.Frozen);
            }

            Point pCharacterStatusDizziness = ImageRecognition.FindSingleTarget(characterMat.Clone(),
                ImageResCollections.CharacterStatusDizzinessBitmap.ToMat(), 0.7);
            if (!pCharacterStatusDizziness.IsEmpty)
            {
                currentCharacterStatus.NegativeStatusList.Add(CharacterStatusEnum.Dizziness);
            }

            // 识别角色能量
            List<Point> energyPointList = ImageRecognition.FindMultiTarget(characterMat.Clone(),
                ImageResCollections.CharacterEnergyOnBitmap.ToMat(), "e", out resMat);
            currentCharacterStatus.EnergyNum = energyPointList.Count;

            MyLogger.Info(currentCharacterStatus.ToString());
            return currentCharacterStatus;
        }

        public CurrentCharacterStatus WhichCharacterActiveWithRetry()
        {
            return Retry.Do(WhichCharacterActive, TimeSpan.FromSeconds(0.5), 8);
        }
    }
}