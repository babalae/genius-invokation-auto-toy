﻿using GeniusInvokationAutoToy.Strategy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Core.Model;
using OpenCvSharp;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
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
                //if (l.StartsWith("//"))
                //{
                //    continue;
                //}
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
                        continue;
                    }

                    if (line == "---" || line.StartsWith("//") || string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (stage == "角色定义:")
                    {
                        var character = ParseCharacter(line);
                        duel.Characters[character.Index] = character;
                    }
                    else if (stage == "策略定义:")
                    {
                        MyAssert(duel.Characters[3] != null, "角色未定义");

                        string[] actionParts = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        MyAssert(actionParts.Length == 3, "策略中的行动命令解析错误");
                        MyAssert(actionParts[1] == "使用", "策略中的行动命令解析错误");

                        var actionCommand = new ActionCommand();
                        var action = actionParts[1].ChineseToActionEnum();
                        actionCommand.Action = action;

                        int j = 1;
                        for (j = 1; j <= 3; j++)
                        {
                            var character = duel.Characters[j];
                            if (character != null && character.Name == actionParts[0])
                            {
                                actionCommand.Character = character;
                                break;
                            }
                        }

                        MyAssert(j <= 3, "策略中的行动命令解析错误：角色名称无法从角色定义中匹配到");

                        int skillNum = int.Parse(Regex.Replace(actionParts[2], @"[^0-9]+", ""));
                        MyAssert(skillNum < 5, "策略中的行动命令解析错误：技能编号错误");
                        actionCommand.TargetIndex = skillNum;
                        duel.ActionCommandQueue.Add(actionCommand);
                    }
                    else
                    {
                        throw new Exception($"未知的定义字段：{stage}");
                    }
                }

                MyAssert(duel.Characters[3] != null, "角色未定义，请确认策略文本格式是否为UTF-8");
            }
            catch (Exception ex)
            {
                MyLogger.Error($"解析脚本错误，行号：{i + 1}，错误信息：{ex}");
                MessageBox.Show($"解析脚本错误，行号：{i + 1}，错误信息：{ex}", "策略解析失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MyAssert(character.Index >= 1 && character.Index <= 3, "角色序号必须在1-3之间");
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
            MyAssert(skill.Index >= 1 && skill.Index <= 5, "技能序号必须在1-5之间");
            var costStr = parts[1];
            var costParts = costStr.Split('+');
            skill.SpecificElementCost = int.Parse(costParts[0].Substring(0, 1));
            skill.Type = costParts[0].Substring(1, 1).ChineseToElementalType();
            // 杂色骰子在+号右边
            if (costParts.Length > 1)
            {
                skill.AnyElementCost = int.Parse(costParts[1].Substring(0, 1));
            }

            skill.AllCost = skill.SpecificElementCost + skill.AnyElementCost;
            return skill;
        }

        private static void MyAssert(bool b, string msg)
        {
            if (!b)
            {
                throw new Exception(msg);
            }
        }
    }
}