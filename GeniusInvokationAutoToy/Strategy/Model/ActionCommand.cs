﻿using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Utils;
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
        /// 目标编号（技能编号，从右往左）
        /// </summary>
        public int TargetIndex { get; set; }

        public override string ToString()
        {
            if (Action == ActionEnum.UseSkill)
            {
                return $"【{Character.Name}】使用【技能{TargetIndex}】";
            }
            else if (Action == ActionEnum.SwitchLater)
            {
                return $"【{Character.Name}】切换至【角色{TargetIndex}】";
            }
            else
            {
                return base.ToString();
            }
        }


        public int GetSpecificElementDiceUseCount()
        {
            if (Action == ActionEnum.UseSkill)
            {
                return Character.Skills[TargetIndex].SpecificElementCost;
            }
            else
            {
                throw new ArgumentException("未知行动");
            }
        }

        public int GetAllDiceUseCount()
        {
            if (Action == ActionEnum.UseSkill)
            {
                return Character.Skills[TargetIndex].AllCost;
            }
            else
            {
                throw new ArgumentException("未知行动");
            }
        }

        public ElementalType GetDiceUseElementType()
        {
            if (Action == ActionEnum.UseSkill)
            {
                return Character.Element;
            }
            else
            {
                throw new ArgumentException("未知行动");
            }
        }

        public bool SwitchLater()
        {
            return Character.SwitchLater();
        }

        public bool UseSkill(Duel duel)
        {
            return Character.UseSkill(TargetIndex, duel);
        }
    }
}