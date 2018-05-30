using Code.Game.Battle;
using DG.Tweening;
using Game.Battle;

using ILRuntime.Runtime.Debugger;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 播放飞行道具
    /// 1.double0 ->飞行持续时间
    /// 2.str0    ->飞行道具路径名
    /// 3.double1 ->触发事件 时间
    /// 4.str1    ->触发事件名
    /// </summary>
    [SkillEvent("PlayFlyItem")]
    public class SkillEvent_PlayflyItem :ISkillEvent
    {
        private SkillEvent skillEvent;
        public SkillEvent_PlayflyItem(SkillEvent skillEvent)
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