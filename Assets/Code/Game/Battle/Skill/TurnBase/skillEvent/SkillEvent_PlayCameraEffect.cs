using Code.Game.Battle;
using DG.Tweening;
using Game.Battle;

using ILRuntime.Runtime.Debugger;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 播放镜头特效
    /// 1.double0 ->镜头效果持续时间
    /// 2.str0    ->镜头效果名  抖动 
    /// 依次每组都是一个镜头效果
    /// </summary>
    [SkillEvent("PlayCameraEffect")]
    public class SkillEvent_PlayCameraEffect :ISkillEvent
    {
        private SkillEvent skillEvent;
        public  SkillEvent_PlayCameraEffect(SkillEvent skillEvent)
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