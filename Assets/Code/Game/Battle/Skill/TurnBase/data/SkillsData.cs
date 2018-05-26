using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle.Skill
{
    //[Serializable]
    public class Skills : ScriptableObject
    {
        [SerializeField]
        public List<Skill> SkillList =  new List<Skill>();
    }



    /// <summary>
    /// 单个技能
    /// </summary>
    [Serializable]
    public class Skill
    {
        [SerializeField]
        public int Id = -1;
        [SerializeField]
        public List<SkillBlock> Blocks = new List<SkillBlock>();
    }


    /// <summary>
    /// 每段攻击的属性
    /// </summary>
    [Serializable]
    public class SkillBlock
    {
        [SerializeField]
        public string AniName  = "null" ;

        [SerializeField] 
        public List<SkillEvent> Events  = new List<SkillEvent>();
    }


    [Serializable]
    public class SkillEvent
    {
        [SerializeField]
        public int FrameId = -1;
        //
        [SerializeField]
        public string EventName = "null";

        //三个字符串类型传参,不够再加
        [SerializeField]
        public string StrParam0 = "";
        [SerializeField]
        public string StrParam1 = "";
        [SerializeField]
        public string StrParam2 = "";
        //三个浮点数类型传参,不够再加
        [SerializeField]
        public double DoubleParams0 = 0;
        [SerializeField]
        public double DoubleParams1 = 0;
        [SerializeField]
        public double DoubleParams2 = 0;
        //2个vec类型传参
        [SerializeField]
        public Vector4 Vec4Params0;
        [SerializeField]
        public Vector4 Vec4Params1;
    }

}

