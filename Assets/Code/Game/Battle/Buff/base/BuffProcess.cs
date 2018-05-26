using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Data;
using Newtonsoft.Json.Converters;

namespace Game.Battle
{
    public class BuffProcess
    {
     
        /// <summary>
        /// buff影响的对象
        /// </summary>
        private IHeroLogic _heroLogic;

        private Dictionary<int, List<IBuff>> buffMap;
        
        public BuffProcess(IHeroLogic heroLogic)
        {
            this._heroLogic = heroLogic;
            buffMap = new Dictionary<int, List<IBuff>>();
        }

        private int idCount = 1;
        /// <summary>
        /// 添加buff,如同时有多个buff，则视为同一批次buff
        /// </summary>
        /// <param name="buff"></param>
        public int AddBuffs(params IBuff[] buffs)
        {
            var list =  new List<IBuff>();
            foreach (var b in buffs)
            {
                b.OnTrigger(this._heroLogic);
                list.Add(b);
            }

            //
            this.buffMap[idCount] = list;
            return idCount++;
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="id"></param>
        public bool RemoveBuff(int id)
        {

            foreach (var value in this.buffMap.Values)
            {
                foreach (var v in value)
                {
                    v.OnRemove();
                }
            }
            return false;
        }


        public void  OnNewRound()
        {
            foreach (var value in this.buffMap.Values)
            {
                foreach (var v in value)
                {
                    v.OnNewRound();
                }
            }
        }
    }
}