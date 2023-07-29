using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Core.Model;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Skill
    {
        public short Index { get; set; }
        public ElementalType Type { get; set; }
        /// <summary>
        /// 消耗指定元素骰子数量
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// 消耗杂色骰子数量
        /// </summary>
        public int CostAny { get; set; } = 0;
    }
}
