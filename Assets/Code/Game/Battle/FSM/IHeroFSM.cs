using Game.Battle;

namespace Code.Game.Battle
{
    public interface IHeroFSM
    {
        int ID { get; set; }
        int Camp { get; }
        IHeroLogic HeroLogic { get; }
        IHeroGraphic HeroGraphic { get; }
        void Init();
        int CurSkillBlockState {get;}
        int CurSkillId { get; }
        void Input(Cmd cmd);
        
        void OnNewRound();
    }
}