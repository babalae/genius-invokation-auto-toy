﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Core.Model;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Skill
    {
        /// <summary>
        /// 1-4 和数组下标一致，游戏中是从右往左开始数的！
        /// </summary>
        public short Index { get; set; }
        public ElementalType Type { get; set; }
        /// <summary>
        /// 消耗指定元素骰子数量
        /// </summary>
        public int SpecificElementCost { get; set; }

        /// <summary>
        /// 消耗杂色骰子数量
        /// </summary>
        public int AnyElementCost { get; set; } = 0;
        /// <summary>
        /// 消耗指定元素骰子数量 + 消耗杂色骰子数量 = 消耗总骰子数量
        /// </summary>
        public int AllCost { get; set; }
    }
}
