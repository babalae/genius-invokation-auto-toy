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
using GeniusInvokationAutoToy.Utils;

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

            return Parse(result);
        }

        public static Duel Parse(List<string> lines)
        {
            Duel duel = new Duel();
            string stage = "";

            int i = 0;
            try
            {
                for (i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if (line.Contains(":"))
                    {
                        stage = line;
                        if (stage.StartsWith("回合"))
                        {
                            duel.RoundStrategies.Add(new RoundStrategy());
                        }

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
                        Trace.Assert(duel.Characters[3] != null, "角色未定义");

                        int roundNum = int.Parse(Regex.Replace(stage, @"[^0-9]+", ""));
                        Trace.Assert(roundNum <= 30, "你的回合数也太多了(>30)");

                        string[] actionParts = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        Trace.Assert(actionParts.Length == 2, "回合中的命令解析错误");

                        var actionCommand = new ActionCommand();
                        var action = actionParts[0].ChineseToActionEnum();
                        actionCommand.Action = action;
                        if (action == ActionEnum.ChooseFirst || action == ActionEnum.SwitchLater)
                        {
                            foreach (var character in duel.Characters)
                            {
                                if (character!= null && character.Name == actionParts[1])
                                {
                                    actionCommand.TargetIndex = character.Index;
                                    break;
                                }
                            }
                        }
                        else if (action == ActionEnum.UseSkill)
                        {
                            int skillNum = int.Parse(Regex.Replace(actionParts[1], @"[^0-9]+", ""));
                            Trace.Assert(skillNum < 5, "技能编号错误");
                            actionCommand.TargetIndex = skillNum;
                        }
                        else
                        {
                            throw new Exception($"未知的动作：{action}");
                        }

                        duel.RoundStrategies[roundNum - 1].ActionCommands.Add(actionCommand);
                        duel.RoundStrategies[roundNum - 1].RawCommandList.Add(line);
                    }
                    else
                    {
                        throw new Exception($"未知的定义字段：{stage}");
                    }
                }

                Trace.Assert(duel.Characters[3] != null, "角色未定义，请确认策略文本格式是否为UTF-8");
                Trace.Assert(duel.RoundStrategies[0].ActionCommands[0].Action == ActionEnum.ChooseFirst,
                    "回合1的首个指令必须是出战角色");
            }
            catch (Exception ex)
            {
                MyLogger.Error($"解析脚本错误，行号：{i + 1}，错误信息：{ex}");
                return null;
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

            var characterAndSkill = line.Split('{');

            var parts = characterAndSkill[0].Split('=');
            character.Index = int.Parse(Regex.Replace(parts[0], @"[^0-9]+", ""));
            Trace.Assert(character.Index >= 1 && character.Index <= 3, "角色序号必须在1-3之间");
            var nameAndElement = parts[1].Split('|');
            character.Name = nameAndElement[0];
            character.Element = nameAndElement[1].Substring(0, 1).ChineseToElementalType();

            // 技能
            string skillStr = characterAndSkill[1].Replace("}", "");
            var skillParts = skillStr.Split(',');
            var skills = new Skill[skillParts.Length + 1];
            for (int i = 0; i < skillParts.Length; i++)
            {
                var skill = ParseSkill(skillParts[i]);
                skills[skill.Index] = skill;
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