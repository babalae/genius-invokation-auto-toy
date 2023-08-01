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

        public static string RollPhaseDiceBitmapPath = Path.Combine(Application.StartupPath, "config\\1920x1080\\dice");
        public static string CardBitmapPath = Path.Combine(Application.StartupPath, "config\\1920x1080\\card");

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

        public static Bitmap ConfirmButton = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\确定.png"));
        public static Bitmap RoundEndButton = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\回合结束.png"));
        public static Bitmap ElementalTuningConfirmButton = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\元素调和.png"));
        public static Bitmap ExitDuelButton = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\退出挑战.png"));

        public static Bitmap InMyActionBitmap = RoundEndButton;
        public static Bitmap InOpponentActionBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\对方行动中.png"));
        public static Bitmap EndPhaseBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\回合结算阶段.png"));
        public static Bitmap ElementalDiceLackWarning = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\元素骰子不足.png"));
        public static Bitmap CharacterTakenOutBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\角色死亡.png"));
        public static Bitmap CharacterDefeatedBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\角色被打败.png"));
        public static Bitmap InCharacterPickBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\出战角色.png"));

        // 角色区域
        public static Bitmap CharacterHpUpperBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\角色血量上方.png"));
        public static Bitmap CharacterStatusFreezeBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\角色状态_冻结.png"));
        public static Bitmap CharacterStatusDizzinessBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\角色状态_水泡.png"));
        public static Bitmap CharacterEnergyOnBitmap = new Bitmap(Path.Combine(Application.StartupPath, "config\\1920x1080\\other_zh-cn\\满能量.png"));

        // 卡牌
        public static Bitmap ElementalResonanceWovenWindsCard = new Bitmap(Path.Combine(CardBitmapPath, "Elemental Resonance Woven Winds.png"));
        public static Bitmap TheBestestTravelCompanionCard = new Bitmap(Path.Combine(CardBitmapPath, "The Bestest Travel Companion.png"));
    }
}
