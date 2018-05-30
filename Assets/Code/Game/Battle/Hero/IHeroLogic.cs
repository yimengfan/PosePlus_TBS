using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using MyJson;
using UnityEngine;
namespace Game.Battle
{
    public interface IHeroLogic
    {
        /// <summary>
        /// 英雄数据
        /// </summary>
        Data.Hero  Data { get; }
        
        ADataDrive State { get; }

        void Init();
        /// <summary>
        /// 是否生存
        /// </summary>
        bool IsSurvival { get; }
        /// <summary>
        /// 获取玩家属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetAttribute(string name);

       
        /// <summary>
        /// 监听状态
        /// </summary>
        void ListeningState(string state, ADataDrive.CallBack callback);
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="state"></param>
        /// <param name="callback"></param>
        void RemoveListeningState(string state, ADataDrive.CallBack callback);

        /// <summary>
        /// 获取buff
        /// </summary>
        /// <param name="buff"></param>
        int AddBuff(IBuff buff);
        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="id"></param>
        bool RemoveBuff(int id);

        /// <summary>
        /// 攻击
        /// </summary>
        /// <returns></returns>
        Data.Skill Attack();

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Data.Skill UseSkill(int index);

        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="skill"></param>
        void Behurt(Data.Skill skill);
        
        
        void OnNewRound();
    }
}