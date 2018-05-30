using System.Collections;
using System.Collections.Generic;

namespace Game.Battle
{
    public class BattleRule_TBS : IBattleRule
    {
        private IBattle battle;


        public void Init(IBattle battle)
        {
            
        }

        public void Start()
        {
            OnDoNextOperation();
        }

        public void Stop()
        {
            
        }

        public void OnNewRound()
        {
           battle.OnNewRound();
        }

        private List<int> operationList = null;
        /// <summary>
        /// 每一个操作
        /// </summary>
        private void OnDoNextOperation()
        {
            if (operationList == null)
            {
                operationList = new List<int>(battle.GetHeroIndexs());
            }
            var players = battle.GetHeroes(operationList.ToArray());
            players.Sort((a, b) => { return a.HeroLogic.GetAttribute("speed") > b.HeroLogic.GetAttribute("speed") ? 1 : -1; });

            battle.State.SetData("CanOperation", players[0].ID);


            operationList.Remove(players[0].ID);
            if (operationList.Count == 0)
            {
                operationList = null;
            }

            //本回合还没操作完
            if (operationList != null)
            {
                players[0].HeroLogic.State.RegAction("OnAttackEnd", o =>
                {
                    if (!CheckBattleEnd())
                    {
                        OnDoNextOperation();
                    }
                   
                });
            }
            //本回合结束
            else  
            {
                players[0].HeroLogic.State.RegAction("OnAttackEnd", o =>
                {
                    if (!CheckBattleEnd())
                    { //开启下一回合
                        OnNewRound();
                        OnDoNextOperation();
                    }
                });
            }

        }



        /// <summary>
        /// 检测是否结束战斗
        /// </summary>
        /// <returns></returns>
        private bool CheckBattleEnd()
        {

            for (int i = 0; i < 6; i++)
            {
                var v = battle.GetHero(i);
                if (v != null)
                {
                    if (v.HeroLogic.IsSurvival)
                    {
                        break;
                    }
                }
                
                if(i == 5)
                    return true;
            }
            
            for (int i = 6; i < 12; i++)
            {
                var v = battle.GetHero(i);
                if (v != null)
                {
                    if (v.HeroLogic.IsSurvival)
                    {
                        break;
                    }
                }
                
                if(i == 11)
                    return true;
            }
            return false;
        }
    }
}