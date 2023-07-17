
namespace GeniusInvokationAutoToy
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerCapture = new System.Windows.Forms.Timer(this.components);
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.chkTopMost = new System.Windows.Forms.CheckBox();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkDisplayDetectForm = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lblYSStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(10, 432);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(294, 174);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // timerCapture
            // 
            this.timerCapture.Tick += new System.EventHandler(this.timerCapture_Tick);
            // 
            // rtbConsole
            // 
            this.rtbConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbConsole.Location = new System.Drawing.Point(10, 124);
            this.rtbConsole.Margin = new System.Windows.Forms.Padding(2);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.Size = new System.Drawing.Size(558, 304);
            this.rtbConsole.TabIndex = 7;
            this.rtbConsole.Text = "";
            this.rtbConsole.TextChanged += new System.EventHandler(this.rtbConsole_TextChanged);
            // 
            // chkTopMost
            // 
            this.chkTopMost.AutoSize = true;
            this.chkTopMost.Location = new System.Drawing.Point(10, 62);
            this.chkTopMost.Margin = new System.Windows.Forms.Padding(2);
            this.chkTopMost.Name = "chkTopMost";
            this.chkTopMost.Size = new System.Drawing.Size(72, 16);
            this.chkTopMost.TabIndex = 9;
            this.chkTopMost.Text = "置顶界面";
            this.chkTopMost.UseVisualStyleBackColor = true;
            this.chkTopMost.CheckedChanged += new System.EventHandler(this.chkTopMost_CheckedChanged);
            // 
            // btnSwitch
            // 
            this.btnSwitch.Location = new System.Drawing.Point(173, 23);
            this.btnSwitch.Margin = new System.Windows.Forms.Padding(2);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(151, 30);
            this.btnSwitch.TabIndex = 10;
            this.btnSwitch.Text = "开始自动打牌(F11)";
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(312, 9);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(179, 12);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "开源地址-遇到问题也在这边提问";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkDisplayDetectForm
            // 
            this.chkDisplayDetectForm.AutoSize = true;
            this.chkDisplayDetectForm.Location = new System.Drawing.Point(106, 80);
            this.chkDisplayDetectForm.Margin = new System.Windows.Forms.Padding(2);
            this.chkDisplayDetectForm.Name = "chkDisplayDetectForm";
            this.chkDisplayDetectForm.Size = new System.Drawing.Size(138, 16);
            this.chkDisplayDetectForm.TabIndex = 12;
            this.chkDisplayDetectForm.Text = "显示识别窗口(Debug)";
            this.chkDisplayDetectForm.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(386, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "投掷时保留有用骰子";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(330, 80);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 16;
            this.button3.Text = "烧牌测试";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(411, 80);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(170, 23);
            this.button4.TabIndex = 17;
            this.button4.Text = "计算当前有多少元素骰子";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblYSStatus
            // 
            this.lblYSStatus.AutoSize = true;
            this.lblYSStatus.ForeColor = System.Drawing.Color.Red;
            this.lblYSStatus.Location = new System.Drawing.Point(80, 15);
            this.lblYSStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblYSStatus.Name = "lblYSStatus";
            this.lblYSStatus.Size = new System.Drawing.Size(41, 12);
            this.lblYSStatus.TabIndex = 19;
            this.lblYSStatus.Text = "未启动";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "原神状态：";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 620);
            this.Controls.Add(this.lblYSStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkDisplayDetectForm);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.chkTopMost);
            this.Controls.Add(this.rtbConsole);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.Text = "原神自动打牌（自动七胜召唤对局）";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerCapture;
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.CheckBox chkTopMost;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkDisplayDetectForm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lblYSStatus;
        private System.Windows.Forms.Label label1;
    }
}

