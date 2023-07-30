
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.chkTopMost = new System.Windows.Forms.CheckBox();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblYSStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboGameResolution = new System.Windows.Forms.ComboBox();
            this.cboStrategy = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // rtbConsole
            // 
            this.rtbConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbConsole.Location = new System.Drawing.Point(14, 140);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.Size = new System.Drawing.Size(578, 583);
            this.rtbConsole.TabIndex = 7;
            this.rtbConsole.Text = "";
            this.rtbConsole.TextChanged += new System.EventHandler(this.rtbConsole_TextChanged);
            // 
            // chkTopMost
            // 
            this.chkTopMost.AutoSize = true;
            this.chkTopMost.Location = new System.Drawing.Point(213, 21);
            this.chkTopMost.Name = "chkTopMost";
            this.chkTopMost.Size = new System.Drawing.Size(99, 22);
            this.chkTopMost.TabIndex = 9;
            this.chkTopMost.Text = "置顶界面";
            this.chkTopMost.UseVisualStyleBackColor = true;
            this.chkTopMost.CheckedChanged += new System.EventHandler(this.chkTopMost_CheckedChanged);
            // 
            // btnSwitch
            // 
            this.btnSwitch.Location = new System.Drawing.Point(399, 57);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(195, 76);
            this.btnSwitch.TabIndex = 10;
            this.btnSwitch.Text = "开始自动打牌(F11)";
            this.btnSwitch.UseVisualStyleBackColor = true;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(328, 22);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(170, 18);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "软件主页(免费开源)";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lblYSStatus
            // 
            this.lblYSStatus.AutoSize = true;
            this.lblYSStatus.ForeColor = System.Drawing.Color.Red;
            this.lblYSStatus.Location = new System.Drawing.Point(120, 22);
            this.lblYSStatus.Name = "lblYSStatus";
            this.lblYSStatus.Size = new System.Drawing.Size(62, 18);
            this.lblYSStatus.TabIndex = 19;
            this.lblYSStatus.Text = "未启动";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 18;
            this.label1.Text = "原神状态：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 22;
            this.label2.Text = "卡牌策略：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 18);
            this.label3.TabIndex = 23;
            this.label3.Text = "游戏分辨率：";
            // 
            // cboGameResolution
            // 
            this.cboGameResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGameResolution.Enabled = false;
            this.cboGameResolution.FormattingEnabled = true;
            this.cboGameResolution.Items.AddRange(new object[] {
            "1920x1080"});
            this.cboGameResolution.Location = new System.Drawing.Point(148, 104);
            this.cboGameResolution.Name = "cboGameResolution";
            this.cboGameResolution.Size = new System.Drawing.Size(222, 26);
            this.cboGameResolution.TabIndex = 21;
            // 
            // cboStrategy
            // 
            this.cboStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStrategy.FormattingEnabled = true;
            this.cboStrategy.Items.AddRange(new object[] {
            "莫娜砂糖琴",
            "刻晴雷电将军甘雨"});
            this.cboStrategy.Location = new System.Drawing.Point(148, 62);
            this.cboStrategy.Name = "cboStrategy";
            this.cboStrategy.Size = new System.Drawing.Size(222, 26);
            this.cboStrategy.TabIndex = 20;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 741);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboGameResolution);
            this.Controls.Add(this.cboStrategy);
            this.Controls.Add(this.lblYSStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.chkTopMost);
            this.Controls.Add(this.rtbConsole);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "原神自动打牌（自动七胜召唤对局）";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.CheckBox chkTopMost;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label lblYSStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboGameResolution;
        private System.Windows.Forms.ComboBox cboStrategy;
    }
}

