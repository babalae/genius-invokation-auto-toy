﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Utils;
using GeniusInvokationAutoToy.Utils.Extension;
using NLog;

namespace GeniusInvokationAutoToy.Strategy
{
    /// <summary>
    ///【原神】七圣召唤-史上最无脑PVE牌组！连牌都不用出，暴打NPC！https://www.bilibili.com/video/BV1ZP41197Ws
    /// 虽然颠勺队伍早就有了，但是第一次知道PVE也好用
    /// </summary>
    public class MonaSucroseJeanStrategy : BaseStrategy
    {
        /// <summary>
        /// 莫娜充能
        /// </summary>
        protected int MonaEnergyNum;

        public MonaSucroseJeanStrategy(YuanShenWindow window) : base(window)
        {
        }


        public override void Run(CancellationTokenSource cts1)
        {
            cts = cts1;
            MonaSucroseJeanInit();
            try
            {
                MyLogger.Info("========================================");
                MyLogger.Info("对局启动！");
                // 对局准备（选择初始手牌、出战角色）
                DuelPrepare();

                // 执行回合
                MyLogger.Info("--------------第1回合--------------");
                Round1();
                MyLogger.Info("--------------第2回合--------------");
                Round2();


                for (int i = 0; i < 3; i++)
                {
                    MyLogger.Info($"--------------第{3 + i}回合--------------");
                    if (CurrentTakenOutCharacterCount >= 2)
                    {
                        RoundMoreForMona();
                    }
                    else
                    {
                        RoundMore();
                    }
                }

                // end
                capture.Stop();
                MyLogger.Info("没活了，结束自动打牌");
            }
            catch (TaskCanceledException ex)
            {
                MyLogger.Info(ex.Message);
            }
            catch (DuelEndException ex)
            {
                MyLogger.Info(ex.Message);

            }
            catch (Exception ex)
            {
                MyLogger.Error(ex.ToString());
            }
            finally
            {
                MyLogger.Info("========================================");
            }
        }

        public void MonaSucroseJeanInit()
        {
            Init(); // 初始化对局变量
            MonaEnergyNum = 0;
        }

        private void DuelPrepare()
        {
            // 1. 选择初始手牌
            Sleep(1000);
            MyLogger.Info("开始选择初始手牌");
            while (!ClickConfirm())
            {
                // 循环等待选择卡牌画面
                Sleep(1000);
            }

            MyLogger.Info("点击确认");

            // 2. 选择出战角色
            // 此处选择第一个角色 莫娜
            MyLogger.Info("等待3s动画...");
            Sleep(3000);

            // 是否是再角色出战选择界面
            Retry.Do(IsInCharacterPickRetryThrowable, TimeSpan.FromSeconds(1), 5);
            MyLogger.Info("识别到已经在角色出战界面，等待1.5s");
            Sleep(1500);

            // 识别角色所在区域
            Retry.Do(() => ChooseCharacterFirst(1), TimeSpan.FromSeconds(1), 5);

            MyLogger.Info("出战莫娜");

            // 初始化手牌
            CurrentCardCount = 5;
        }

        /// <summary>
        /// 第一回合
        /// </summary>
        public void Round1()
        {
            // 0 投骰子
            ReRollDice(ElementalType.Anemo, ElementalType.Hydro, ElementalType.Omni);

            // 等待到我的回合 // 投骰子动画时间是不确定的
            WaitForMyTurn(1000);

            // 1 回合1 行动1 莫娜使用1次二技能
            MyLogger.Info("回合1 行动1 莫娜使用1次二技能");
            bool useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Hydro, 8);
            if (!useSkillRes)
            {
                MyLogger.Warn("没有足够的手牌或元素骰子释放技能，停止自动打牌");
                return;
            }

            MonaEnergyNum++;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 2 回合1 行动2 切换砂糖
            MyLogger.Info("回合1 行动2 切换砂糖");
            SwitchCharacterLater(2);

            // 快速切换无需等待对方 但是有动画，需要延迟一会
            WaitForMyTurn(3000);

            // 3 回合1 行动2 砂糖使用1次二技能
            MyLogger.Info("回合1 行动3 砂糖使用1次二技能");
            ActionPhaseAutoUseSkill(2, 3, ElementalType.Anemo, 4);


            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合1 结束 剩下1个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }


        /// <summary>
        /// 第二回合
        /// </summary>
        public void Round2()
        {
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Anemo, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            // 1 回合2 行动1 砂糖使用1次二技能
            MyLogger.Info("回合2 行动1 砂糖使用1次二技能");
            bool useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Anemo, 8);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 2 回合2 行动2 砂糖充能已满，使用1次三技能
            MyLogger.Info("回合2 行动2 砂糖充能已满，使用1次三技能");
            useSkillRes = ActionPhaseAutoUseSkill(1, 3, ElementalType.Anemo, 5);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合2 结束 剩下2个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }


        /// <summary>
        /// 第三/N回合 出战为砂糖琴
        /// 对局可能会胜利
        /// </summary>
        public void RoundMore()
        {
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Anemo, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            // 1 回合2 行动1 使用1次二技能
            MyLogger.Info("行动1 使用1次二技能");
            bool useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Anemo, 8);
            if (!useSkillRes)
            {
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 2 回合2 行动2 使用1次二技能
            MyLogger.Info("行动2 使用1次二技能");
            useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Anemo, 5);
            if (!useSkillRes)
            {
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合2 结束 剩下2个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }

        /// <summary>
        /// 第三/N回合 出战为莫娜
        /// 对局可能会胜利
        /// </summary>
        public void RoundMoreForMona()
        {
            bool useSkillRes;
            int roundDiceCount = 8;
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Hydro, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            if (MonaEnergyNum == 3)
            {
                roundDiceCount = MonaUse3Skill(roundDiceCount);
            }

            // 1 使用1次二技能
            MyLogger.Info("行动1 使用1次二技能");
            useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Hydro, roundDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            roundDiceCount -= 3;
            MonaEnergyNum++;


            if (MonaEnergyNum == 3 && roundDiceCount >= 3)
            {
                roundDiceCount = MonaUse3Skill(roundDiceCount);
            }

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 2 使用1次一技能
            if (roundDiceCount >= 3)
            {
                MyLogger.Info("行动2 使用1次一技能");
                useSkillRes = ActionPhaseAutoUseSkill(3, 1, ElementalType.Hydro, roundDiceCount);
                if (!useSkillRes)
                {
                    MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                    RoundEnd();
                }

                roundDiceCount -= 3;
                MonaEnergyNum++;
            }


            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合2 结束 剩下2个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }

        private int MonaUse3Skill(int roundDiceCount)
        {
            bool useSkillRes;
            MyLogger.Info("使用1次三技能");
            useSkillRes = ActionPhaseAutoUseSkill(1, 3, ElementalType.Hydro, roundDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                RoundEnd();
            }

            roundDiceCount -= 3;
            MonaEnergyNum = 0;
            return roundDiceCount;
        }
    }
}