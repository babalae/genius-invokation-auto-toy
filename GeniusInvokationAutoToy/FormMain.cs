using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Forms.Hotkey;
using GeniusInvokationAutoToy.Strategy;
using GeniusInvokationAutoToy.Utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeniusInvokationAutoToy.Utils.Extension;
using MathNet.Numerics.Statistics;
using System.Text.RegularExpressions;


namespace GeniusInvokationAutoToy
{
    public partial class FormMain : Form
    {
        private static NLog.Logger logger;

        private FormMask maskForm;

        private YuanShenWindow window = new YuanShenWindow();


        private MonaSucroseJeanStrategy strategy;

        private bool isAutoPlaying = false;

        private string thisVersion;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            // 标题加上版本号
            string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (currentVersion.Length > 3)
            {
                thisVersion = currentVersion.Substring(0, 3);
                currentVersion = " v" + thisVersion;
            }

            this.Text += currentVersion;
            //GAHelper.Instance.RequestPageView($"/main/{thisVersion}", $"进入{thisVersion}版本主界面");


            YSStatus();

            strategy = new MonaSucroseJeanStrategy(window);

            try
            {
                RegisterHotKey("F11");
            }
            catch (Exception ex)
            {
                MyLogger.Warn(ex.Message);
                MessageBox.Show(ex.Message, "热键注册失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool YSStatus()
        {
            if (window.FindYSHandle())
            {
                lblYSStatus.ForeColor = Color.Green;
                lblYSStatus.Text = "已启动";
                return true;
            }
            else
            {
                lblYSStatus.ForeColor = Color.Red;
                lblYSStatus.Text = "未启动";
                return false;
            }
        }

        private void InitMaskWindows(int x, int y, int w, int h)
        {
            // 钓鱼条捕获窗口
            maskForm = new FormMask(x, y, w, h);
            //maskForm.FormMainInstance = this;
            maskForm.Show();
        }

        private void TestScreen()
        {
            //PrintMsg($"获取真实设置的桌面分辨率大小 {PrimaryScreen.DESKTOP.Width} x {PrimaryScreen.DESKTOP.Height}");
            //PrintMsg($"获取屏幕分辨率当前物理大小 {PrimaryScreen.WorkingArea.Width} x {PrimaryScreen.WorkingArea.Height}");

            //PrintMsg($"获取缩放百分比 {PrimaryScreen.ScaleX} x {PrimaryScreen.ScaleY}");
            //PrintMsg($"当前系统DPI {PrimaryScreen.DpiX} x {PrimaryScreen.DpiY}");

            //if (!window.GetHWND())
            //{
            //    PrintMsg("未找到原神进程，请先启动原神！");
            //}

            //Rectangle rc = window.GetSize();
            //PrintMsg($"原神窗口 {rc.Width} x {rc.Height}");
            //PrintMsg($"原神窗口 {rc.Width * PrimaryScreen.ScaleX} x {rc.Height * PrimaryScreen.ScaleY}");
            //strainBarArea.Location = new System.Drawing.Point((int)((rc.X + 300) * PrimaryScreen.ScaleX), (int)(rc.Y * PrimaryScreen.ScaleY + 16));
        }


        private void timerCapture_Tick(object sender, EventArgs e)
        {
            //Bitmap pic = capture.Capture();
            //pictureBox1.Image = ImageRecognition.GetRect(pic, out rects, chkDisplayDetectForm.Checked);
            strategy.GetCharacterRects();
        }

        //private void PrintMsg(string msg)
        //{
        //    msg = DateTime.Now + " " + msg;
        //    Console.WriteLine(msg);
        //    rtbConsole.Text += msg + Environment.NewLine;
        //    this.rtbConsole.SelectionStart = rtbConsole.TextLength;
        //    this.rtbConsole.ScrollToCaret();
        //}

        private async void StartGame()
        {
            if (!window.FindYSHandle())
            {
                MyLogger.Warn("未找到原神进程，请先启动原神！");
            }


            window.Focus();

            // strategy.Run();
            await strategy.RunAsync();


            //Rectangle rc = window.GetSize();
            //PrintMsg($"原神窗口 {rc.Width} x {rc.Height}");
            //PrintMsg(
            //    $"原神窗口 位置 {rc.Location.X * PrimaryScreen.ScaleX},{rc.Location.Y * PrimaryScreen.ScaleY} 大小 {rc.Width * PrimaryScreen.ScaleX} x {rc.Height * PrimaryScreen.ScaleY}");

            //int x = (int)Math.Ceiling(rc.Location.X * PrimaryScreen.ScaleX);
            //int y = (int)Math.Ceiling(rc.Location.Y * PrimaryScreen.ScaleY);
            //int w = (int)Math.Ceiling(rc.Width * PrimaryScreen.ScaleX);
            //int h = (int)Math.Ceiling(rc.Height * PrimaryScreen.ScaleY);

            ////InitMaskWindows(x, y, w, h);

            ////capture.Start(x, y, w, h);

            //timerCapture.Interval = 3000;
            //timerCapture.Start();
        }

        private void StopGame()
        {
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (!isAutoPlaying)
            {
                StartGame();
                isAutoPlaying = true;
                btnSwitch.Text = "关闭自动打牌(F11)";
            }
            else
            {
                StopGame();
                isAutoPlaying = false;
                btnSwitch.Text = "开始自动打牌(F11)";
            }
        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkTopMost.Checked;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Properties.Settings.Default.FrameRate = nudFrameRate.Value;
            //Properties.Settings.Default.AutoPullUpChecked = chkAutoPullUp.Checked;
            //Properties.Settings.Default.TopMostChecked = chkTopMost.Checked;
            //Properties.Settings.Default.DisplayDetectChecked = chkDisplayDetectForm.Checked;
            //Properties.Settings.Default.AlwaysHideAreaChecked = chkAlwaysHideArea.Checked;

            //Properties.Settings.Default.FormMainLocation = Location;
            //Properties.Settings.Default.StrainBarAreaLocation = strainBarArea.Location;
            //Properties.Settings.Default.StrainBarAreaSize = strainBarArea.Size;
            //Properties.Settings.Default.PullUpRodAreaLocation = pullUpRodArea.Location;
            //Properties.Settings.Default.PullUpRodAreaSize = pullUpRodArea.Size;
            //Properties.Settings.Default.Save();
            //strainBarArea.Close();
            //pullUpRodArea.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/babalae/genshin-fishing-toy");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //strategy.RollPhaseReRoll(ElementalType.Hydro);

            //ImageRecognition.FindSingleTarget(new Bitmap("E:\\HuiTask\\原神七圣召唤\\素材\\Clip_20230715_182700.png").ToMat(), ImageResCollections.ConfirmBitmap.ToMat());

            //List<Rect> rects = new List<Rect>();

            strategy.WaitOpponentAction();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            strategy.ActionPhaseElementalTuning();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ImageRecognition.IsDebug = true;
            //Mat srcMat = new Bitmap("E:\\HuiTask\\原神七圣召唤\\素材\\Clip_20230715_182733.png").ToMat();
            //// 切割图片后再识别 加快速度 位置没啥用，所以切割后比较方便
            //int cutLength = srcMat.Width / 5 * 4;
            //srcMat = new Mat(srcMat, new Rect(cutLength, 0, srcMat.Width - cutLength, srcMat.Height));
            //Dictionary<string, List<System.Drawing.Point>> dictionary = ImageRecognition.FindPicFromImage(srcMat, ImageResCollections.ActionPhaseDiceBitmaps, 0.7);


            Dictionary<string, int> res = strategy.ActionPhaseDice();
            foreach (var item in res)
            {
                logger.Info($"{item.Key} {item.Value}");
            }
        }

        #region Hotkey

        private Hotkey hotkey;
        private HotkeyHook hotkeyHook;

        private void rtbConsole_TextChanged(object sender, EventArgs e)
        {
            rtbConsole.SelectionStart = rtbConsole.Text.Length;
            rtbConsole.ScrollToCaret();
        }

        public void RegisterHotKey(string hotkeyStr)
        {
            if (string.IsNullOrEmpty(hotkeyStr))
            {
                UnregisterHotKey();
                return;
            }

            hotkey = new Hotkey(hotkeyStr);

            if (hotkeyHook != null)
            {
                hotkeyHook.Dispose();
            }

            hotkeyHook = new HotkeyHook();
            // register the event that is fired after the key press.
            hotkeyHook.KeyPressed += new EventHandler<KeyPressedEventArgs>(btnSwitch_Click);
            hotkeyHook.RegisterHotKey(hotkey.ModifierKey, hotkey.Key);
        }

        public void UnregisterHotKey()
        {
            if (hotkeyHook != null)
            {
                hotkeyHook.UnregisterHotKey();
                hotkeyHook.Dispose();
            }
        }

        #endregion
    }
}