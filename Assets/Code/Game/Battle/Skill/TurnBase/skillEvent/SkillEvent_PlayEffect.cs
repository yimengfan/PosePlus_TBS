using Code.Game.Battle;
using DG.Tweening;
using Game.Battle;

using ILRuntime.Runtime.Debugger;
using UnityEngine;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 播放技能特效
    /// 1.double0 ->特效播放位置
    ///       =1 "单体特效",
    ///       =2 "群体特效"
    ///       =3 "链接特效(单)"
    /// 2.str0 ->特效路径
    /// 3.double1 ->播放时间
    /// </summary>
    [SkillEvent("PlayEffect")]
    public class SkillEvent_PlayEffect :ISkillEvent
    {
        private SkillEvent se;
        public SkillEvent_PlayEffect(SkillEvent se)
        {
            this.se = se;
        }


        public double GetEventValue()
        {
           //throw new System.NotImplementedException();
            return 0;
        }


        private Transform effect;

        public void Do(IBattle battle, IHeroFSM selfFSM, IHeroFSM targetFSM)
        {

             var o = BResources.Load<GameObject>(this.se.StrParam0);
             effect = GameObject.Instantiate(o).transform;
            //在敌方阵营的特效 需要旋转
          
      
            //父节点
            Transform parent = null;
            if (se.DoubleParams0 == 1) //单体
            {
                if (targetFSM.Camp == 2)
                    effect.eulerAngles = new Vector3(0,180,0);
                parent = battle.World.GetPlayerRootTransform(targetFSM.ID);              
            }
            else if (se.DoubleParams0 == 2) 
            {
                parent = battle.World.GetPlayerRootTransform(targetFSM.Camp);
            }
            else if (se.DoubleParams0 == 3) //连接
            {
                parent = battle.World.GetPlayerRootTransform(selfFSM.Camp);
                var mypos = battle.World.GetPlayerPos(selfFSM.ID);
                var tarpos = battle.World.GetPlayerPos(targetFSM.ID);

                //TODO 求两个向量夹角
                var v1=  new Vector2(mypos.x , mypos.z);
                var v2 = new Vector2(tarpos.x , tarpos.z);
                var dir = (v2 - v1).normalized;
                
                effect.LookAt(dir);
//              float angle = Vector3.Angle(mypos, tarpos);          
//                effect.eulerAngles += new Vector3(0,angle,0);
            }

            effect.SetParent(parent, false);
            
            
            if (se.DoubleParams1 > 0)
            {
                IEnumeratorTool.WaitingForExec((float)se.DoubleParams1, () =>
                {
                    BResources.Destroy(effect);
                });
            }
        }
    }
}