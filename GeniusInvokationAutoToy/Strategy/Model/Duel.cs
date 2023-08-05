using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Strategy.Model.Old;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusInvokationAutoToy.Strategy.Model
{
    public class Duel
    {
        public Character CurrentCharacter { get; set; }
        public Character[] Characters { get; set; } = new Character[4];

        /// <summary>
        /// 行动指令队列
        /// </summary>
        public List<ActionCommand> ActionCommandQueue { get; set; } = new List<ActionCommand>();

        /// <summary>
        /// 当前回合数
        /// </summary>
        public int RoundNum { get; set; } = 1;

        /// <summary>
        /// 角色牌位置
        /// </summary>
        public List<Rectangle> CharacterCardRects { get; set; }

        /// <summary>
        /// 手牌数量
        /// </summary>
        public int CurrentCardCount { get; set; } = 0;

        /// <summary>
        /// 骰子数量
        /// </summary>
        public int CurrentDiceCount { get; set; } = 0;


        public CancellationTokenSource Cts { get; set; }


        public async Task CustomStrategyRunAsync(CancellationTokenSource cts1)
        {
            await Task.Run(() => { CustomStrategyRun(cts1); });
        }

        public void CustomStrategyRun(CancellationTokenSource cts1)
        {
            Cts = cts1;
            try
            {
                MyLogger.Info("========================================");
                MyLogger.Info("对局启动！");

                GameControl.GetInstance().Init(Cts);
                GameControl.GetInstance().FocusGameWindow();
                Retry.Do(() => GameControl.GetInstance().IsGameFocus(), TimeSpan.FromSeconds(1), 10);
                // 对局准备 选择初始手牌
                GameControl.GetInstance().CommonDuelPrepare();


                // 获取角色区域
                CharacterCardRects = Retry.Do(() => GameControl.GetInstance().GetCharacterRects(),
                    TimeSpan.FromSeconds(1.5), 20);
                if (CharacterCardRects == null || CharacterCardRects.Count != 3)
                {
                    throw new DuelEndException("未成功获取到角色区域");
                }

                for (var i = 1; i < 4; i++)
                {
                    Characters[i].Area = CharacterCardRects[i - 1];
                }

                // 出战角色
                CurrentCharacter = ActionCommandQueue[0].Character;
                CurrentCharacter.ChooseFirst();

                // 开始执行回合
                while (true)
                {
                    MyLogger.Info($"--------------第{RoundNum}回合--------------");
                    ClearCharacterStatus(); // 清空单回合的异常状态
                    if (RoundNum == 1)
                    {
                        CurrentCardCount = 5;
                    }
                    else
                    {
                        CurrentCardCount += 2;
                    }

                    CurrentDiceCount = 8;

                    // 预计算本回合内的所有可能的元素
                    HashSet<ElementalType> elementSet = PredictionDiceType();

                    // 0 投骰子
                    GameControl.GetInstance().ReRollDice(elementSet.ToArray());

                    // 等待到我的回合 // 投骰子动画时间是不确定的  // 可能是对方先手
                    GameControl.GetInstance().WaitForMyTurn(this, 1000);

                    // 开始执行行动
                    while (true)
                    {
                        // 没骰子了就结束行动
                        MyLogger.Info($"行动开始,当前骰子数[{CurrentDiceCount}],当前手牌数[{CurrentCardCount}]");
                        if (CurrentDiceCount <= 0)
                        {
                            MyLogger.Info("骰子已经用完");
                            GameControl.GetInstance().Sleep(3000);
                            break;
                        }

                        // 每次行动前都要检查当前角色
                        CurrentCharacter = GameControl.GetInstance().WhichCharacterActiveWithRetry(this);

                        List<int> alreadyExecutedActionIndex = new List<int>();
                        List<ActionCommand> alreadyExecutedActionCommand = new List<ActionCommand>();
                        var i = 0;
                        for (i = 0; i < ActionCommandQueue.Count; i++)
                        {
                            var actionCommand = ActionCommandQueue[i];
                            // 指令中的角色未被打败、角色有异常状态 跳过指令
                            if (actionCommand.Character.IsDefeated || actionCommand.Character.StatusList?.Count > 0)
                            {
                                continue;
                            }

                            // 当前出战角色身上存在异常状态的情况下不执行本角色的指令
                            if (CurrentCharacter.StatusList?.Count > 0 &&
                                actionCommand.Character.Index == CurrentCharacter.Index)
                            {
                                continue;
                            }


                            // 1. 判断切人
                            if (CurrentCharacter.Index != actionCommand.Character.Index)
                            {
                                if (CurrentDiceCount >= 1)
                                {
                                    actionCommand.SwitchLater();
                                    CurrentDiceCount--;
                                    alreadyExecutedActionIndex.Add(-actionCommand.Character.Index); // 标记为已执行
                                    var switchAction = new ActionCommand
                                    {
                                        Character = CurrentCharacter,
                                        Action = ActionEnum.SwitchLater,
                                        TargetIndex = actionCommand.Character.Index
                                    };
                                    alreadyExecutedActionCommand.Add(switchAction);
                                    MyLogger.Info("→指令执行完成：" + switchAction);
                                    break;
                                }
                                else
                                {
                                    MyLogger.Info("骰子不足以进行下一步：切换角色" + actionCommand.Character.Index);
                                    break;
                                }
                            }

                            // 2. 判断使用技能
                            if (actionCommand.GetAllDiceUseCount() > CurrentDiceCount)
                            {
                                MyLogger.Info("骰子不足以进行下一步：" + actionCommand);
                                break;
                            }
                            else
                            {
                                bool useSkillRes = actionCommand.UseSkill(this);
                                if (useSkillRes)
                                {
                                    CurrentDiceCount -= actionCommand.GetAllDiceUseCount();
                                    alreadyExecutedActionIndex.Add(i);
                                    alreadyExecutedActionCommand.Add(actionCommand);
                                    MyLogger.Info("→指令执行完成：" + actionCommand);
                                }
                                else
                                {
                                    MyLogger.Warn("→指令执行失败(可能是手牌不够)：" + actionCommand);
                                    GameControl.GetInstance().Sleep(1000);
                                    GameControl.GetInstance().ClickGameWindowCenter();
                                }

                                break;
                            }
                        }



                        if (alreadyExecutedActionIndex.Count != 0)
                        {
                            foreach (var index in alreadyExecutedActionIndex)
                            {
                                if (index >= 0)
                                {
                                    ActionCommandQueue.RemoveAt(index);
                                }
                            }

                            alreadyExecutedActionIndex.Clear();
                            // 等待对方行动完成 （开大的时候等待时间久一点）
                            int sleepTime = ComputeWaitForMyTurnTime(alreadyExecutedActionCommand);
                            GameControl.GetInstance().WaitForMyTurn(this, sleepTime);
                            alreadyExecutedActionCommand.Clear();
                        }
                        else
                        {
                            // 如果没有任何指令可以执行 则跳出循环
                            // TODO 也有可能是角色死亡/所有角色被冻结导致没有指令可以执行
                            //if (i >= ActionCommandQueue.Count)
                            //{
                            //    throw new DuelEndException("策略中所有指令已经执行完毕，结束自动打牌");
                            //}
                            GameControl.GetInstance().Sleep(3000);
                            break;
                        }

                        if (ActionCommandQueue.Count == 0)
                        {
                            throw new DuelEndException("策略中所有指令已经执行完毕，结束自动打牌");
                        }
                    }

                    // 回合结束
                    GameControl.GetInstance().Sleep(1000);
                    MyLogger.Info("我方点击回合结束");
                    GameControl.GetInstance().RoundEnd();

                    // 等待对方行动+回合结算
                    GameControl.GetInstance().WaitOpponentAction(this);
                    RoundNum++;
                }
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

        private HashSet<ElementalType> PredictionDiceType()
        {
            int actionUseDiceSum = 0;
            HashSet<ElementalType> elementSet = new HashSet<ElementalType>
            {
                ElementalType.Omni
            };
            for (var i = 0; i < ActionCommandQueue.Count; i++)
            {
                var actionCommand = ActionCommandQueue[i];

                // 角色未被打败的情况下才能执行
                if (actionCommand.Character.IsDefeated)
                {
                    continue;
                }

                // 通过骰子数量判断是否可以执行

                // 1. 判断切人
                if (i > 0 && actionCommand.Character.Index != ActionCommandQueue[i - 1].Character.Index)
                {
                    actionUseDiceSum++;
                    if (actionUseDiceSum > CurrentDiceCount)
                    {
                        break;
                    }
                    else
                    {
                        elementSet.Add(actionCommand.GetDiceUseElementType());
                        //executeActionIndex.Add(-actionCommand.Character.Index);
                    }
                }

                // 2. 判断使用技能
                actionUseDiceSum += actionCommand.GetAllDiceUseCount();
                if (actionUseDiceSum > CurrentDiceCount)
                {
                    break;
                }
                else
                {
                    elementSet.Add(actionCommand.GetDiceUseElementType());
                    //executeActionIndex.Add(i);
                }
            }

            return elementSet;
        }

        public void ClearCharacterStatus()
        {
            foreach (var character in Characters)
            {
                character?.StatusList?.Clear();
            }
        }

        /// <summary>
        /// 根据前面执行的命令计算等待时间
        /// 大招等待15秒
        /// 快速切换等待3秒
        /// </summary>
        /// <param name="alreadyExecutedActionCommand"></param>
        /// <returns></returns>
        private int ComputeWaitForMyTurnTime(List<ActionCommand> alreadyExecutedActionCommand)
        {
            foreach (var command in alreadyExecutedActionCommand)
            {
                if (command.Action == ActionEnum.UseSkill && command.TargetIndex == 1)
                {
                    return 15000;
                }

                // 莫娜切换等待3秒
                if (command.Character.Name == "莫娜" && command.Action == ActionEnum.SwitchLater)
                {
                    return 3000;
                }
            }

            return 10000;
        }

        /// <summary>
        /// 获取角色切换顺序
        /// </summary>
        /// <returns></returns>
        public List<int> GetCharacterSwitchOrder()
        {
            List<int> orderList = new List<int>();
            for (var i = 0; i < ActionCommandQueue.Count; i++)
            {
                if (!orderList.Contains(ActionCommandQueue[i].Character.Index))
                {
                    orderList.Add(ActionCommandQueue[i].Character.Index);
                }
            }

            return orderList;
        }

        /// <summary>
        /// 获取角色存活数量
        /// </summary>
        /// <returns></returns>
        public int GetCharacterAliveNum()
        {
            int num = 0;
            foreach (var character in Characters)
            {
                if (character != null && !character.IsDefeated)
                {
                    num++;
                }
            }

            return num;
        }
    }
}