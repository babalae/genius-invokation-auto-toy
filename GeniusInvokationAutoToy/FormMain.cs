using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Forms.Hotkey;
using GeniusInvokationAutoToy.Strategy;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace GeniusInvokationAutoToy
{
    public partial class FormMain : Form
    {
        private static NLog.Logger logger;

        private YuanShenWindow window = new YuanShenWindow();

        private BaseStrategy strategy;

        private bool isAutoPlaying = false;

        private string thisVersion;

        CancellationTokenSource cts;

        public FormMain()
        {
            InitializeComponent();
        }

        public void RtbConsoleDeleteLine()
        {
            Invoke(new Action(() =>
            {
                rtbConsole.Lines = rtbConsole.Lines.Take(rtbConsole.Lines.Length - 2).ToArray();
                rtbConsole.Text += Environment.NewLine;
            }));
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            MyLogger.formMain = this;
            // 标题加上版本号
            string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (currentVersion.Length > 3)
            {
                thisVersion = currentVersion.Substring(0, 3);
                currentVersion = " v" + thisVersion;
            }

            this.Text += currentVersion;
            //GAHelper.Instance.RequestPageView($"/main/{thisVersion}", $"进入{thisVersion}版本主界面");

            rtbConsole.Text = @"软件在Github上开源且免费 by huiyadanli

支持角色邀请、每周来客挑战、大世界NPC挑战（部分场景不支持、或者打不过/拿不满奖励）。

1、牌组必须是莫娜、砂糖、琴，顺序不能变，带什么牌无所谓
2、窗口化游戏，只支持1920x1080，游戏整个界面不能被其他窗口遮挡！
3、在游戏内进入七圣召唤对局，到初始手牌界面
4、然后直接点击开始自动打牌，双手离开键盘鼠标（快捷键F11）。
";

            YSStatus();

            try
            {
                RegisterHotKey("F11");
            }
            catch (Exception ex)
            {
                MyLogger.Warn(ex.Message);
                MessageBox.Show(ex.Message, "热键注册失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //RichTextBoxTarget target = new RichTextBoxTarget();
            //target.Layout = "${longdate} [${threadname:whenEmpty=${threadid}}] ${uppercase:${level}} ${message:withException=true} ${all-event-properties}";
            //target.TargetRichTextBox = rtbConsole;
            //target.UseDefaultRowColoringRules = true;
            //NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, NLog.LogLevel.Trace);



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

        private void timerCapture_Tick(object sender, EventArgs e)
        {
            //Bitmap pic = capture.Capture();
            //pictureBox1.Image = ImageRecognition.GetRect(pic, out rects, chkDisplayDetectForm.Checked);
            strategy.GetCharacterRects();
        }

        private async void StartGame()
        {
            if (!window.FindYSHandle())
            {
                MyLogger.Warn("未找到原神进程，请先启动原神！");
            }

            window.Focus();

            rtbConsole.Text = ""; // 清空日志

            strategy = new MonaSucroseJeanStrategy(window);


            cts = new CancellationTokenSource(); ;
            await strategy.RunAsync(cts);

        }

        private void StopGame()
        {
            cts?.Cancel();
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
            Process.Start("https://github.com/babalae/genius-invokation-auto-toy");
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