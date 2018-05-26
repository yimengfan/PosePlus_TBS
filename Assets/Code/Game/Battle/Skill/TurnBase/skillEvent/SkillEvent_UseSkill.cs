using System;
using System.Collections;
using Code.Game.Battle;
using Game.Battle;
using UnityEngine;

namespace Game.Battle.Skill
{
    /// <summary>
    /// 使用技能攻击
    /// 1.无参数
    /// </summary>
    [SkillEvent("UseSkill")]
    public class SkillEvent_UseSkill :ISkillEvent
    {
        private SkillEvent skillEvent;
        public SkillEvent_UseSkill(SkillEvent skillEvent)
        {
            this.skillEvent = skillEvent;
        }


        public double GetEventValue()
        {
           //throw new System.NotImplementedException();
            return 0;
        }

        public void Do(IBattle battle, IHeroFSM selfFSM, IHeroFSM targetFSM)
        {
            targetFSM.HeroGraphic.PlayAction("behurt");
            IEnumeratorTool.StartCoroutine(CheckAniEnd(targetFSM, "behurt", () =>
            {
                targetFSM.HeroGraphic.PlayAction("idle");
            }));
        }


        /// <summary>
        /// 检查动画结束
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="aniName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator CheckAniEnd(IHeroFSM fsm,  string aniName , Action callback)
        {
            //设置个最长80帧的检测
            int i = 80;
            while (i>0)
            {
                var aniPlayer = fsm.HeroGraphic.AniPlayer;
                var ani =  aniPlayer.CurClip.name.Replace(".FBAni", "");
                //判断动画播放完
                if (ani == aniName && aniPlayer.CurAniFrame == aniPlayer.CurClip.aniFrameCount- 1)
                {
                    if (callback != null)
                    {
                        callback();
                    }
                    i = 0;
                }

                i--;
                yield return null;
            }
            yield break;
        }
        
        
        /// <summary>
        /// skillblock的公式修改
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="replaceRrefixion"></param>
        /// <returns></returns>
        private string SkillBlock_FormulaProcess( string formula , string replaceRrefixion, IHeroLogic a = null, IHeroLogic b = null)
        {
            formula = formula.Trim();
            if (formula.Contains("a.") || formula.Contains("b.")) //属性类
            {
                var str = formula.Split('+', '-', '*', '/');
                foreach (var s in str)
                {
                    if (s.Contains("a."))
                    {
                        var attrName = s.Replace("a.", "");
                        var attr = a.GetAttribute(attrName);
                        formula = formula.Replace(s, attr.ToString());
                    }
                    else if(s.Contains("b."))
                    {
                        var attrName = s.Replace("b.", "");
                        var attr = b.GetAttribute(attrName);
                        formula = formula.Replace(s, attr.ToString());
                    }
                }
            }
            else if (formula.Contains("vref.")) //值引用类
            {
                
            }


            return formula;
        }
    }
}