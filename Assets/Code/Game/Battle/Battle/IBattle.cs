using System.Collections;
using System.Collections.Generic;
using Code.Game.Battle;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Battle
{
    /// <summary>
    /// 战斗
    /// </summary>
    public interface IBattle
    {
        IBattleRule Rule { get; }
        IBattleInput Input { get; set; }
        IBattleWorld World { get; }
        ADataDrive State { get; }
        void SetHero(int index, IHeroFSM heroFSM);
        void SwapHeroIndex(int a, int b);
        int[] GetHeroIndexs();
        IHeroFSM GetHero(int index);
        List<IHeroFSM> GetHeroes(int[] index);
        void RemoveHero(int id);

        void Init();
        void Start();
        void Stop();
        void OnNewRound();
        void Reset();
    }


}
