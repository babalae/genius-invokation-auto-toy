using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Utils.Extension;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Character
    {
        public short Index { get; set; }
        public string Name { get; set; }
        public ElementalType Element { get; set; }
        public Skill[] Skills { get; set; }


        /// <summary>
        /// 是否阵亡
        /// </summary>
        public bool IsTakenOut { get; set; }

        /// <summary>
        /// 充能点
        /// </summary>
        public int Energy { get; set; }


        /// <summary>
        /// 角色区域
        /// </summary>
        public Rectangle Area { get; set; }

        public void ChooseFirst()
        {
            if (Area.IsEmpty)
            {
                throw new Exception("角色区域为空");
            }

            // 双击选择角色出战
            MouseUtils.DoubleClick(MakeOffset(Area.GetCenterPoint()));
        }
    }
}