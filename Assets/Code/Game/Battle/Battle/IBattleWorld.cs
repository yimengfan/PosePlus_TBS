using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗世界
    /// </summary>
    public interface IBattleWorld
    {
       Transform Transform { get; }
       Vector3 GetPlayerPos(int index);
       void Load();

       Transform GetCampCenterTransform(int camp);
        Transform GetPlayerRootTransform(int index);
       void Destroy();
    }
}