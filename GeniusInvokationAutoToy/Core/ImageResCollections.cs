using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeniusInvokationAutoToy.Core
{
    /// <summary>
    /// 识别图片资源
    /// </summary>
    public class ImageResCollections
    {

        public static string RollPhaseDiceBitmapPath = Path.Combine(Application.StartupPath, "pic\\dice");

        // 投掷期间的骰子
        public static Dictionary<string,Bitmap> RollPhaseDiceBitmaps = new Dictionary<string, Bitmap>()
        {
            { "anemo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_anemo.png")) },
            { "geo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_geo.png")) },
            { "electro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_electro.png")) },
            { "dendro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_dendro.png")) },
            { "hydro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_hydro.png")) },
            { "pyro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_pyro.png")) },
            { "cryo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_cryo.png")) },
            { "omni",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"roll_omni.png")) },
        };

        // 主界面骰子
        public static Dictionary<string, Bitmap> ActionPhaseDiceBitmaps = new Dictionary<string, Bitmap>()
        {
            { "anemo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_anemo.png")) },
            { "geo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_geo.png")) },
            { "electro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_electro.png")) },
            { "dendro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_dendro.png")) },
            { "hydro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_hydro.png")) },
            { "pyro",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_pyro.png")) },
            { "cryo",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_cryo.png")) },
            { "omni",new Bitmap(Path.Combine(RollPhaseDiceBitmapPath,"action_omni.png")) },
        };

        public static Bitmap ConfirmButton = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\确定.png"));
        public static Bitmap RoundEndButton = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\回合结束.png"));
        public static Bitmap ElementalTuningConfirmButton = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\元素调和.png"));
        public static Bitmap ExitDuelButton = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\元素调和.png"));

        public static Bitmap InMyActionBitmap = RoundEndButton;
        public static Bitmap InOpponentActionBitmap = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\对方行动中.png"));
        public static Bitmap EndPhaseBitmap = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\回合结算阶段.png"));
        public static Bitmap ElementalDiceLackWarning = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\元素骰子不足.png"));
        public static Bitmap CharacterTakenOutBitmap = new Bitmap(Path.Combine(Application.StartupPath, "pic\\other\\角色死亡.png"));

    }
}
