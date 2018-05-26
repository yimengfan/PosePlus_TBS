using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗世界
    /// </summary>
    public class BattleWorld : IBattleWorld
    {
        public Transform Transform { get; private set; }

        private List<Vector3> playersPos;
        private string path;
        public BattleWorld(int worldId)
        {
            this.path  = string.Format("Scene/0{0}", worldId);
       
        }

        /// <summary>
        /// 获取玩家pos
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetPlayerPos(int index)
        {
            index = index - 1;
            if (index < this.playersPos.Count)
            {
                return this.playersPos[index];
            }
            return Vector3.zero;
        }

        public void Load()
        {   
            //重置pos
            this.playersPos =new List<Vector3>();
            //加载场景
            var o = GameObject.Instantiate(BResources.Load<GameObject>(this.path));
            this.Transform = o.transform;
            this.Transform.SetParent(Client.SceneRoot , false);
            var playesPosTrans = this.Transform.Find("PlayerPos");

            foreach (Transform p in playesPosTrans)
            {
              this.playersPos.Add(p.position);   
            }
        }

        public void Destroy()
        {
            
        }
    }
}