using GeniusInvokationAutoToy.Core;
using GeniusInvokationAutoToy.Core.Model;
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public List<RoundStrategy> RoundStrategies { get; set; } = new List<RoundStrategy>();

        public int RoundNum { get; set; } = 1;

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
                CurrentCardCount = 5;

                // 获取角色区域
                var rects = Retry.Do(() => GameControl.GetInstance().GetCharacterRects(), TimeSpan.FromSeconds(1.5), 20);
                if (rects == null || rects.Count != 3)
                {
                    throw new DuelEndException("未成功获取到角色区域");
                }
                for (var i = 1; i < 4; i++)
                {
                    Characters[i].Area = rects[i-1];
                }

                // 出战角色
                CurrentCharacter = Characters[RoundStrategies[0].ActionCommands[0].TargetIndex];
                Characters[RoundStrategies[0].ActionCommands[0].TargetIndex].ChooseFirst();

                // 开始回合循环
                for (int i = 0; i < RoundStrategies.Count; i++)
                {
                    MyLogger.Info($"--------------第{i + 1}回合--------------");
                    CurrentCardCount += 2;

                    // 0 投骰子
                    List<ElementalType> list = RoundStrategies[i].MaybeNeedElement(this);
                    list.Add(ElementalType.Omni);
                    list.Add(CurrentCharacter.Element);
                    GameControl.GetInstance().ReRollDice(list.ToArray());

                    // 等待到我的回合 // 投骰子动画时间是不确定的  // 可能是对方先手
                    GameControl.GetInstance().WaitForMyTurn(1000);

                    CurrentDiceCount = 8;
                    for (int j = 0; j < RoundStrategies[i].ActionCommands.Count; j++)
                    {
                        MyLogger.Info($"第{i + 1}回合执行：{RoundStrategies[i].RawCommandList[j]}");
                        var actionCommand = RoundStrategies[i].ActionCommands[j];
                        if (actionCommand.Action == ActionEnum.ChooseFirst)
                        {
                            continue;
                        }
                        else if (actionCommand.Action == ActionEnum.SwitchLater)
                        {
                            CurrentCharacter = Characters[actionCommand.TargetIndex];
                            Characters[actionCommand.TargetIndex].SwitchLater();
                            CurrentDiceCount--;
                        }
                        else if (actionCommand.Action == ActionEnum.UseSkill)
                        {
                            CurrentCharacter.UseSkill(actionCommand.TargetIndex, this);
                            CurrentDiceCount -= CurrentCharacter.Skills[actionCommand.TargetIndex].Cost;
                        }
                        else
                        {
                            throw new Exception("未知的Action");
                        }

                        // 等待对方行动完成
                        GameControl.GetInstance().WaitForMyTurn(10000);
                    }

                    // 回合结束
                    MyLogger.Info("我方点击回合结束");
                    GameControl.GetInstance().RoundEnd();

                    // 等待对方行动+回合结算
                    GameControl.GetInstance().WaitOpponentAction();
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
    }
}