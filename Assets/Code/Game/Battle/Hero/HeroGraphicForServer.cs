using FB.PosePlus;
using Game.Data;
using UnityEditor;
using UnityEngine;

namespace Game.Battle
{
    public class HeroGraphicForServer : IHeroGraphic
    {
        public Transform Trans { get; }
        public SkillPlayer_TBS SkillPlayer { get; }
        public AniPlayer AniPlayer { get; }

        public void PlayAction(string name)
        {
            
        }

        public void Init()
        {
            
        }
    }
}