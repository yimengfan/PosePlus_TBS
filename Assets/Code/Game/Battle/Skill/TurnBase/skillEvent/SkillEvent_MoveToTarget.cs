using Code.Game.Battle;

using DG.Tweening;
namespace Game.Battle.Skill
{
    /// <summary>
    /// 跳跃到敌人位置 ，默认使用run动画
    /// 1.double0 ->消耗时间
    /// </summary>
    [SkillEvent("MoveToTarget")]
    public class SkillEvent_MoveToTarget :ISkillEvent
    {
        private SkillEvent skillEvent;
        public SkillEvent_MoveToTarget(SkillEvent skillEvent)
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
            selfFSM.HeroGraphic.Trans.DOMove(targetFSM.HeroGraphic.Trans.position , (float)this.skillEvent.DoubleParams0);
        }
    }
}