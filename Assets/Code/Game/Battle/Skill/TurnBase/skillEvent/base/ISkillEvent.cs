using Code.Game.Battle;
using Game.Battle;

 namespace Game.Battle.Skill
{
    public interface ISkillEvent
    {
        double GetEventValue();
        void Do(IBattle battle, IHeroFSM selfFSM, IHeroFSM targetFSM);
    }
}