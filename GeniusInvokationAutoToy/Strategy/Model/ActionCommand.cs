using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class ActionCommand
    {
        /// <summary>
        ///  角色
        /// </summary>
        public Character Character { get; set; }

        public ActionEnum Action { get; set; }

        /// <summary>
        /// 目标编号（角色、技能）
        /// </summary>
        public int TargetIndex { get; set; }

        public int GetDiceUseCount()
        {
            if (Action == ActionEnum.SwitchLater)
            {
                return 1;
            }
            else if (Action == ActionEnum.UseSkill)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
