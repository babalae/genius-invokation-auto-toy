using GeniusInvokationAutoToy.Strategy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Core.Model;
using OpenCvSharp;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GeniusInvokationAutoToy.Strategy.Script
{
    public class ScriptParser
    {
        public static Duel Parse(string script)
        {
            var lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var result = new List<string>();
            foreach (var line in lines)
            {
                string l = line.Trim();
                if (l.StartsWith("//"))
                {
                    continue;
                }

                result.Add(l);
            }

            return null;
        }

        public static Duel Parse(List<string> lines)
        {
            Duel duel = new Duel();
            string stage = "";
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Contains(":"))
                {
                    stage = line;
                    continue;
                }

                if (line == "---")
                {
                    continue;
                }

                if (stage == "角色定义:")
                {
                    var character = ParseCharacter(line);
                    duel.Characters[character.Index] = character;
                }
                else if (stage.StartsWith("回合"))
                {
                    int roundNum = int.Parse(Regex.Replace(stage, @"[^0-9]+", ""));
                    Trace.Assert(roundNum > 30, "你的回合数也太多了(>30)");
                    duel.RoundStrategies[roundNum - 1].CommandList.Add(line);
                }
                else
                {
                    throw new Exception($"未知的定义字段：{stage}");
                }
            }

            return duel;
        }

        /// <summary>
        /// 解析示例
        /// 角色1=刻晴|雷{技能3消耗=1雷骰子+2任意,技能2消耗=3雷骰子,技能1消耗=4雷骰子}
        /// 角色2=雷神|雷{技能3消耗=1雷骰子+2任意,技能2消耗=3雷骰子,技能1消耗=4雷骰子}
        /// 角色3=甘雨|冰{技能4消耗=1冰骰子+2任意,技能3消耗=1冰骰子,技能2消耗=5冰骰子,技能1消耗=3冰骰子}
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Character ParseCharacter(string line)
        {
            var character = new Character();
            var parts = line.Split('=');
            character.Index = short.Parse(Regex.Replace(parts[0], @"[^0-9]+", ""));
            Trace.Assert(character.Index >= 1 && character.Index <= 3, "角色序号必须在1-3之间");
            var nameAndElement = parts[1].Split('|');
            character.Name = nameAndElement[0];
            character.Element = nameAndElement[1].Substring(0, 1).ChineseToElementalType();

            // 技能
            string skillStr = nameAndElement[1].Substring(1).Replace("{", "").Replace("}", "");
            var skillParts = skillStr.Split(',');
            var skills = new List<Skill>();
            for (int i = 0; i < skillParts.Length; i++)
            {
                var skill = ParseSkill(skillParts[i]);
                skills.Add(skill);
            }

            character.Skills = skills.ToArray();
            return character;
        }

        /// <summary>
        /// 技能3消耗=1雷骰子+2任意
        /// 技能2消耗=3雷骰子
        /// 技能1消耗=4雷骰子
        /// </summary>
        /// <param name="oneSkillStr"></param>
        /// <returns></returns>
        public static Skill ParseSkill(string oneSkillStr)
        {
            var skill = new Skill();
            var parts = oneSkillStr.Split('=');
            skill.Index = short.Parse(Regex.Replace(parts[0], @"[^0-9]+", ""));
            Trace.Assert(skill.Index >= 1 && skill.Index <= 5, "技能序号必须在1-5之间");
            var costStr = parts[1];
            var costParts = costStr.Split('+');
            skill.Cost = int.Parse(costParts[0].Substring(0, 1));
            skill.Type = costParts[0].Substring(1, 1).ChineseToElementalType();
            return skill;
        }
    }
}