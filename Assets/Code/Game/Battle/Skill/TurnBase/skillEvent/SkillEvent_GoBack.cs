using Code.Game.Battle;
using DG.Tweening;
using Game.Battle;

using ILRuntime.Runtime.Debugger;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 返回到自己位置
    /// 1.double0 ->消耗时间
    /// </summary>
    [SkillEvent("GoBack")]
    public class SkillEvent_GoBack :ISkillEvent
    {
        private SkillEvent skillEvent;
        public SkillEvent_GoBack(SkillEvent skillEvent)
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
            var pos = battle.World.GetPlayerPos(selfFSM.ID);
            selfFSM.HeroGraphic.Trans.DOMove(pos , (float)this.skillEvent.DoubleParams0 / 30f);
        }
    }
}