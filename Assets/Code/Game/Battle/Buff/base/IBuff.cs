using System.Collections;
using System.Collections.Generic;
using Game.Data;
using MyJson;
using UnityEngine;

namespace Game.Battle
{

    public interface IBuff
    {
        Buff Data { get; }
        //
        void OnTrigger( IHeroLogic heroLogic);
        void OnRemove();
        //
        void OnNewRound();
    }
}