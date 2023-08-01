using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy.Model.Old
{
    public class CurrentCharacterStatus
    {
        /// <summary>
        /// 0-2 和所在数组下标一致
        /// </summary>
        public int ArrayIndex { get; set; }
        public int EnergyNum { get; set; }

        public List<CharacterStatusEnum> NegativeStatusList { get; set; } = new List<CharacterStatusEnum>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"当前出战角色{ArrayIndex+1}，");
            sb.Append($"充能={EnergyNum}，");
            if (NegativeStatusList.Count > 0)
            {
                sb.Append($"负面状态：{string.Join(",", NegativeStatusList)}");
            }
            return sb.ToString();
        }   
    }
}