using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy
{
    [Obsolete]
    public class KeqingRaidenGanyuStrategy : BaseStrategy
    {
        /// <summary>
        /// 刻晴充能
        /// </summary>
        protected int KeqingEnergyNum;

        /// <summary>
        /// 莫娜充能
        /// </summary>
        protected int RaidenEnergyNum;

        /// <summary>
        /// 甘雨充能
        /// </summary>
        protected int GanyuEnergyNum;

        public KeqingRaidenGanyuStrategy(YuanShenWindow window) : base(window)
        {
        }


        public override void Run(CancellationTokenSource cts1)
        {
            cts = cts1;
            KeqingRaidenGanyuInit();
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
                MyLogger.Info("--------------第3回合--------------");
                Round3();
                MyLogger.Info("--------------第4回合--------------");
                Round4();


                // end
                capture.Stop();
                MyLogger.Info("没活了，结束自动打牌");
                MyLogger.Info("对手还或者的话，麻烦手动终结他吧");
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

        public void KeqingRaidenGanyuInit()
        {
            Init(); // 初始化对局变量
            KeqingEnergyNum = 0;
            RaidenEnergyNum = 0;
            GanyuEnergyNum = 0;
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
            // 此处选择第2个角色 雷神
            MyLogger.Info("等待3s动画...");
            Sleep(3000);

            // 是否是再角色出战选择界面
            Retry.Do(IsInCharacterPickRetryThrowable, TimeSpan.FromSeconds(1), 5);
            MyLogger.Info("识别到已经在角色出战界面，等待1.5s");
            Sleep(1500);
            // 识别角色所在区域
            Retry.Do(() => ChooseCharacterFirst(2), TimeSpan.FromSeconds(1), 5);

            MyLogger.Info("出战雷电将军");

            // 初始化手牌
            CurrentCardCount = 5;
        }

        /// <summary>
        /// 第一回合
        /// </summary>
        public void Round1()
        {
            CurrentDiceCount = 8;
            // 0 投骰子
            ReRollDice(ElementalType.Electro, ElementalType.Omni);

            // 等待到我的回合 // 投骰子动画时间是不确定的
            WaitForMyTurn(1000);

            // 1 回合1 行动1 雷电将军使用1次二技能
            MyLogger.Info("回合1 行动1 雷电将军使用1次二技能");
            bool useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Warn("没有足够的手牌或元素骰子释放技能，停止自动打牌");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }
            CurrentDiceCount -=3;

            RaidenEnergyNum++;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 2 回合1 行动2 雷电将军使用1次一技能
            MyLogger.Info("回合1 行动2 雷电将军使用1次一技能");
            useSkillRes = ActionPhaseAutoUseSkill(3, 1, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Warn("没有足够的手牌或元素骰子释放技能，停止自动打牌");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }
            CurrentDiceCount -= 3;
            RaidenEnergyNum++;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合1 结束 剩下2个骰子
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
            CurrentDiceCount = 8;
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Electro, ElementalType.Cryo, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            MyLogger.Info("回合2 行动1 雷电将军开大");
            bool useSkillRes = ActionPhaseAutoUseSkill(1, 4, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }
            CurrentDiceCount -= 4;

            KeqingEnergyNum += 2;
            RaidenEnergyNum = 0;
            GanyuEnergyNum += 2;

            // 等待对方行动完成
            WaitForMyTurn(15000);

            MyLogger.Info("回合2 行动2 切换甘雨");
            SwitchCharacterLater(3);
            CurrentDiceCount -= 1;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            MyLogger.Info("回合2 行动3 甘雨使用1次二技能");
            useSkillRes = ActionPhaseAutoUseSkill(3, 3, ElementalType.Cryo, CurrentDiceCount);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }
            CurrentDiceCount -= 3;
            GanyuEnergyNum++;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合2 结束 剩下0个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }


        /// <summary>
        /// 第三回合
        /// 对局可能会胜利
        /// </summary>
        public void Round3()
        {
            CurrentDiceCount = 8;
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Electro, ElementalType.Cryo, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            MyLogger.Info("回合3 行动1 甘雨开大");
            bool useSkillRes = ActionPhaseAutoUseSkill(1, 3, ElementalType.Cryo, CurrentDiceCount);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }

            CurrentDiceCount -= 3;
            GanyuEnergyNum =0;

            // 等待对方行动完成 // 甘雨大招动画比较长
            WaitForMyTurn(15000);

            MyLogger.Info("回合3 行动2 切换刻晴");
            SwitchCharacterLater(1);
            CurrentDiceCount -= 1;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            MyLogger.Info("回合3 行动3 刻晴使用1次二技能");
            useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Info("回合3 行动3 二技能无法使用，刻晴尝试使用一技能");
                useSkillRes = ActionPhaseAutoUseSkill(3, 1, ElementalType.Electro, CurrentDiceCount);
                if (!useSkillRes)
                {
                    // 运气太差直接结束
                    MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                    throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
                }
                else
                {
                    CurrentDiceCount -= 3;
                }
            }
            else
            {
                CurrentDiceCount -= 3;
            }
            KeqingEnergyNum++;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            // 4 回合2 结束 剩下0个骰子
            MyLogger.Info("我方点击回合结束");
            RoundEnd();

            // 5 等待对方行动+回合结算
            WaitOpponentAction();
        }

        /// <summary>
        /// 第四回合
        /// 对局可能会胜利
        /// </summary>
        public void Round4()
        {
            CurrentDiceCount = 8;
            CurrentCardCount += 2;
            // 0 投骰子
            ReRollDice(ElementalType.Electro, ElementalType.Omni);

            // 等待对方行动完成 // 可能是对方先手
            WaitForMyTurn(1000);

            MyLogger.Info("回合4 行动1 刻晴开大");
            bool useSkillRes = ActionPhaseAutoUseSkill(1, 4, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                // 运气太差直接结束
                MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
            }
            CurrentDiceCount -= 4;
            KeqingEnergyNum = 0;

            // 等待对方行动完成
            WaitForMyTurn(10000);

            MyLogger.Info("回合4 行动2 刻晴使用1次二技能");
            useSkillRes = ActionPhaseAutoUseSkill(2, 3, ElementalType.Electro, CurrentDiceCount);
            if (!useSkillRes)
            {
                MyLogger.Info("回合4 行动3 二技能无法使用，刻晴尝试使用一技能");
                useSkillRes = ActionPhaseAutoUseSkill(3, 1, ElementalType.Electro, CurrentDiceCount);
                if (!useSkillRes)
                {
                    // 运气太差直接结束
                    MyLogger.Info("没有足够的手牌或元素骰子释放技能，回合结束");
                    throw new DuelEndException("没有足够的手牌或元素骰子释放技能，停止自动打牌");
                }
                else
                {
                    CurrentDiceCount -= 3;
                }
            }
            else
            {
                CurrentDiceCount -= 3;
            }
            KeqingEnergyNum++;
        }
    }
}