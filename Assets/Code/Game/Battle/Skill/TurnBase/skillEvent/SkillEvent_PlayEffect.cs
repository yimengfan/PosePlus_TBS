using Code.Game.Battle;
using DG.Tweening;
using Game.Battle;

using ILRuntime.Runtime.Debugger;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 播放技能特效
    /// 1.double0 ->特效播放位置
    ///       =1 特效位于目标身上 ，每个英雄身上有个挂载特效的节点
    ///       =2 特效位于我方 或者敌方中心
    /// 2.str0 ->特效路径
    /// </summary>
    [SkillEvent("PlayEffect")]
    public class SkillEvent_PlayEffect :ISkillEvent
    {
        private SkillEvent skillEvent;
        public SkillEvent_PlayEffect(SkillEvent skillEvent)
        {
            this.skillEvent = skillEvent;
        }


        public double GetEventValue()
        {
           //throw new System.NotImplementedException();
            return 0;
        }

        public void Do(IBattle battle, IHeroFSM selfFSM, IHeroFSM targetFSM)
        {

            selfFSM.HeroGraphic.Trans.DOKill();
            var pos = battle.World.GetPlayerPos(selfFSM.HeroLogic.ID);
            selfFSM.HeroGraphic.Trans.DOMove(pos , (float)this.skillEvent.DoubleParams0 / 30f);
        }
    }
}