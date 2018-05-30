using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

        
        List<Transform> palyersPosTransformList =  new List<Transform>();
        List<Transform> campCenterTransformList =  new List<Transform>();
        public void Load()
        {   
            //重置pos
            this.playersPos =new List<Vector3>();
            //加载场景
            var o = GameObject.Instantiate(BResources.Load<GameObject>(this.path));
            this.Transform = o.transform;
            this.Transform.SetParent(Client.SceneRoot , false);
            var playesPosTrans = this.Transform.Find("PlayerPos");

            if (playesPosTrans != null)
            {
                foreach (Transform t in playesPosTrans)
                {
                    this.playersPos.Add(t.position);  
                    palyersPosTransformList.Add(t);
                }
            }
            else
            {          
                Debug.LogError("该场景没有指定【玩家坐标点】,请速速添加~");        
            }
            
            var campCenterTrans = this.Transform.Find("CampCenter");
            if (campCenterTrans != null)
            {
                foreach (Transform t in campCenterTrans)
                {
                    campCenterTransformList.Add(t);
                }
            }
            else
            {
                 Debug.LogError("该场景没有指定【队伍中心点】,请速速添加~");                     
            }
        }

        public Transform GetCampCenterTransform(int camp)
        {
            camp -= 1;

            if (camp < campCenterTransformList.Count)
            {
                return campCenterTransformList[camp];
            }
            return null;
        }

        public Transform GetPlayerRootTransform(int index)
        {

            index -= 1;
            if (index < palyersPosTransformList.Count)
            {
                return palyersPosTransformList[index];
            }

            return null;
        }

        public void Destroy()
        {
            
        }
    }
}