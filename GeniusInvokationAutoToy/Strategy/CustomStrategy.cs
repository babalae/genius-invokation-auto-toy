using GeniusInvokationAutoToy.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeniusInvokationAutoToy.Strategy.Model;
using GeniusInvokationAutoToy.Core.MyException;
using GeniusInvokationAutoToy.Utils;

namespace GeniusInvokationAutoToy.Strategy
{
    /// <summary>
    /// 暂时没用
    /// </summary>
    public class CustomStrategy : BaseStrategy
    {

        public Duel CustomDuel { get; set; }

        protected new int CurrentCardCount
        {
            get => CustomDuel.CurrentCardCount;
            set => CustomDuel.CurrentCardCount = value;
        }

        public CustomStrategy(YuanShenWindow window) : base(window)
        {
        }

        public override void Run(CancellationTokenSource cts1)
        {
            cts = cts1;
            if (CustomDuel == null)
            {
                throw new Exception("CustomDuel is null");
            }

            try
            {
                MyLogger.Info("========================================");
                MyLogger.Info("对局启动！");

                // 对局准备 选择初始手牌
                CommonDuelPrepare();


                for (int i = 0; i < CustomDuel.RoundStrategies.Count; i++)
                {
                    MyLogger.Info($"--------------第{i + 1}回合--------------");
                    List<string> list = CustomDuel.RoundStrategies[i].RawCommandList;
                }



                // 识别角色所在区域出战角色
                Retry.Do(() => ChooseCharacterFirst(2), TimeSpan.FromSeconds(1), 5);

                MyLogger.Info("出战雷电将军");

                // 初始化手牌
                CurrentCardCount = 5;




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
    }
}
