using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Battle;

namespace Code.Game.Battle
{
    static  public class BattleFactory
    {
        static BattleFactory()
        {
            battleMap = new Dictionary<Int64, IBattle>();
        }
        private  static Dictionary<Int64,IBattle>  battleMap ;
        private static Int64 counter = 0;

        /// <summary>
        /// 创建战场
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="battleWorldId"></param>
        /// <returns></returns>
        static public Int64 CreateBattle(int ruleId = 0, int battleWorldId =-1)
        {
            //判断是否越界
            if (counter >= 1 << 63 - 1 )
            {
                counter = 0;
            }
            counter++;
            IBattleRule rule = null;

            if (ruleId == 0)
            {
                rule  =  new BattleRule_TBS();
            }
            var world = new BattleWorld(battleWorldId);

          
            var battle = new global::Game.Battle.Battle(rule, world);

            //
            battleMap[counter] = battle;
            battle.Init();
            return counter;
        }

        

        /// <summary>
        /// 获取一个战场
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public IBattle GetBattle(Int64 id)
        {
            IBattle battle = null;
            battleMap.TryGetValue(id, out battle);
            return battle;
        }

        #region for server

        //
        static public string SetBattleOpration(string str)
        {

            return "";
        }

        #endregion
        
    }
}