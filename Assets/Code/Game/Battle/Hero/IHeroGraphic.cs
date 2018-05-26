using FB.PosePlus;
using UnityEngine;

namespace Game.Battle
{
    public interface IHeroGraphic
    {
        Transform Trans { get; }
        SkillPlayer_TBS SkillPlayer { get; }
        AniPlayer   AniPlayer { get; }
        void PlayAction(string name);
        void Init();
    }
}