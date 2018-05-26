#if UNITY_2017_1_OR_NEWER
using FB.PosePlus;
using Game.Data;
using UnityEditor;
using UnityEngine;
namespace Game.Battle
{
    public class HeroGraphic : IHeroGraphic
    {
        public Transform Trans { get; private set; }
        public SkillPlayer_TBS SkillPlayer { get; private set; }
        public AniPlayer   AniPlayer { get; private set; }
        private Data.Hero data;
        public HeroGraphic(Data.Hero data)
        {
            this.data = data;
           
        }
        
        public void Init()
        {
            var o =  BResources.Load<GameObject>(data.ResurcePath);
            this.Trans = GameObject.Instantiate(o).transform;
            this.Trans.SetParent(Client.SceneRoot,false);
            //
            this.AniPlayer = this.Trans.GetComponent<AniPlayer>();
            this.SkillPlayer = this.Trans.GetComponent<SkillPlayer_TBS>();

        }
        
        public void PlayAction(string name)
        {
            //ani.Play(name);
            this.AniPlayer.Play(name);
        }


    }
}
#endif