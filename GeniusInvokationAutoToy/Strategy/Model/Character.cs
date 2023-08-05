using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Utils.Extension;
using GeniusInvokationAutoToy.Core.MyException;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Character
    {
        /// <summary>
        /// 1-3 所在数组下标一致
        /// </summary>
        public int Index { get; set; }
        public string Name { get; set; }
        public ElementalType Element { get; set; }
        public Skill[] Skills { get; set; }


        /// <summary>
        /// 是否被打败
        /// </summary>
        public bool IsDefeated { get; set; }

        /// <summary>
        /// 充能点
        /// </summary>
        public int Energy { get; set; }


        /// <summary>
        /// 充能点来自于图像识别
        /// </summary>
        public int EnergyByRecognition { get; set; }

        /// <summary>
        /// 角色身上的负面状态
        /// </summary>
        public List<CharacterStatusEnum> StatusList { get; set; } = new List<CharacterStatusEnum>();

        /// <summary>
        /// 角色区域
        /// </summary>
        public Rectangle Area { get; set; }
        /// <summary>
        /// 血量上方区域，用于判断是否出战
        /// </summary>
        public Rectangle HpUpperArea { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"角色{Index}，");
            sb.Append($"充能={EnergyByRecognition}，");
            if (StatusList?.Count > 0)
            {
                sb.Append($"状态：{string.Join(",", StatusList)}");
            }
            return sb.ToString();
        }

        public bool ChooseFirst()
        {
            return MouseUtils.DoubleClick(GameControl.GetInstance().MakeOffset(Area.GetCenterPoint()));
        }

        public bool SwitchLater()
        {
            Point p = GameControl.GetInstance().MakeOffset(Area.GetCenterPoint());
            // 选择角色
            MouseUtils.Click(p);

            // 点击切人按钮
            GameControl.GetInstance().ActionPhasePressSwitchButton();
            return true;
        }

        /// <summary>
        /// 角色死亡的时候双击角色牌重新出战
        /// </summary>
        /// <returns></returns>
        public bool SwitchWhenTakenOut()
        {
            Point p = GameControl.GetInstance().MakeOffset(Area.GetCenterPoint());
            // 选择角色
            MouseUtils.Click(p);
            // 双击切人
            GameControl.GetInstance().Sleep(500);
            MouseUtils.Click(p);
            GameControl.GetInstance().Sleep(300);
            return true;
        }

        public bool UseSkill(int skillIndex, Duel duel)
        {
            bool res = GameControl.GetInstance()
                .ActionPhaseAutoUseSkill(skillIndex, Skills[skillIndex].SpecificElementCost, Skills[skillIndex].Type, duel);
            if (res)
            {
                return true;
            }
            else
            {
                MyLogger.Warn("没有足够的手牌或元素骰子释放技能");
                return false;
            }
        }
    }
}