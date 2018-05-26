using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{

    /// <summary>
    /// 战斗规则
    /// </summary>
    public interface IBattleRule
    {
        void Init(IBattle battle);
        void Start();
        void Stop();
        void OnNewRound();
    }
}