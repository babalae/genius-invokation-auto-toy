using GeniusInvokationAutoToy.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public enum ActionEnum
    {
        ChooseFirst, SwitchLater, UseSkill
    }

    public static class ActionEnumExtension
    {
        public static ActionEnum ChineseToActionEnum(this string type)
        {
            type = type.ToLower();
            switch (type)
            {
                case "出战":
                    return ActionEnum.ChooseFirst;
                case "切换":
                    return ActionEnum.SwitchLater;
                case "使用":
                    return ActionEnum.UseSkill;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
