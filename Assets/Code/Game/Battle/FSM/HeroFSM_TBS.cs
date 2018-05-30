using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BDFramework.Http;
using Game.Battle;
using Game.Battle.Skill;
using Game.Data;
using UnityEditor;
using UnityEngine;

namespace Code.Game.Battle
{
    /// <summary>
    /// 单个英雄的状态机，对外开放英雄的一系列处理
    /// </summary>
    public class HeroFSM_TBS : IHeroFSM
    {
        //总战场管理
        IBattle battle = null;
       
        /// <summary>
        /// 输入管理
        /// </summary>
        private Dictionary<string, Action<List<object>>> inputProcessMap;
        public HeroFSM_TBS(int camp, HeroLogic logic, HeroGraphic graphic , IBattle battle)
        {
            this.Camp = camp;
            this.HeroLogic = logic;
            this.HeroGraphic = graphic;
            this.battle = battle;
            
            inputProcessMap = new Dictionary<string, Action<List<object>>>();
            inputProcessMap["UseSkill"] = OnUseSkill;
        }

        public int ID { get; set; }

        /// <summary>
        /// 阵营
        /// </summary>
        public int Camp { get; private set; }
        public IHeroLogic HeroLogic { get; private set; }
        public IHeroGraphic HeroGraphic { get; private set; }

       

        /// <summary>
        /// 处理指令
        /// </summary>
        /// <param name="cmd"></param>
        public void Input(Cmd cmd)
        {
            Action<List<object>> action = null;
            if (this.inputProcessMap.TryGetValue(cmd.Name, out action))
            {
                //处理指令
                action(cmd.Params);
            }
            else
            {
                BDebug.LogError("没有实现指令:" +  cmd.Name);
            }
        }


        /// <summary>
        /// 当前操作的block
        /// </summary>
        private List<Skillblock> curPlaySkillBlocks;
        private int targetPlayer = -1;
        public void Init()
        {
            HeroGraphic.Init();
            HeroLogic.Init();            
        }
        #region 技能攻击处理
        /// <summary>
        /// 当前技能block的阶段
        /// </summary>
        public int CurSkillBlockState { get; private set; }
        /// <summary>
        /// 当前技能id
        /// </summary>
        public int CurSkillId { get; private set; }
        
        /// <summary>
        /// 当前是否播放技能
        /// </summary>
        public bool IsPlaySkill { get; private set; }

        private int skillTargetId;
        /// <summary>
        /// 技能攻击
        /// </summary>
        /// <param name="_params"></param>
        private void OnUseSkill(List<object> _params)
        {
            if (IsPlaySkill)
            {
                BDebug.LogError("正在播放技能,操作无效!");
                return;
            }
            IsPlaySkill = true;
            //0 : 攻击者下标
            //1 : 作用者下标
            //2 : 技能id
            skillTargetId = (int) _params[1];
            CurSkillId = (int) _params[2];
            BDebug.LogFormat("{0}对{1}释放技能{2}" ,  (int) _params[0] ,  (int) _params[1] , (int) _params[2]);
            var skill = this.HeroLogic.UseSkill(CurSkillId);

            CurSkillBlockState = -1;
            
            curPlaySkillBlocks = SqliteHelper.DB.GetTable<Skillblock>().Where((sb) => skill.SkillBlocks.Contains(sb.Id)).ToList();

            if (curPlaySkillBlocks.Count == 0)
            {
                BDebug.LogError("当前skill无block , skill id:" + CurSkillId);
            }
            /******注册渲染层事件*****/
            //渲染层通知：播放block
            this.HeroGraphic.SkillPlayer.State.RegAction("DoBlockEvent" , o =>
            {
                BDebug.Log("执行事件");
                DoBlockEvent((List<SkillEvent> )o);
            });
            //渲染层通知：
            this.HeroGraphic.SkillPlayer.State.RegAction("CurBlockEnd" , o =>
            {
                BDebug.Log(string.Format("播放block:{0} 结束" , CurSkillBlockState));
                DoPlayNextBlock();
            });
            //
            this.HeroGraphic.SkillPlayer.State.RegAction("AllBlockEnd" , o =>
            {
                BDebug.Log("当前所有block播放完毕~");
                AllBlockEnd();
            });

            DoPlayNextBlock();
        }


        /// <summary>
        /// 播放下一个block
        /// </summary>
        private void DoPlayNextBlock()
        {
            CurSkillBlockState++;
            var blocks = curPlaySkillBlocks.Where((b) => b.BlockIndex == CurSkillBlockState).ToList();

#if UNITY_EDITOR
            if (BattleTest.IsByEditor)
            {
                
                BDebug.Log(string.Format("[编辑器] 播放block:{0}" , CurSkillBlockState));
                this.HeroGraphic.SkillPlayer.PlaySkill(CurSkillId ,  CurSkillBlockState);
                return;
            }
#endif
            if (blocks.Count != 0)
            {
                var b = blocks[0];

                if (SkillBlock_CheckCondition(b.Condition))
                {
                    BDebug.Log(string.Format("播放block:{0}" , CurSkillBlockState));
                    this.HeroGraphic.SkillPlayer.PlaySkill(CurSkillId ,  CurSkillBlockState);
                }
                else //继续下一个block
                {
                   DoPlayNextBlock();   
                }

            }
            else 
            {

                BDebug.LogError("当前没有播放的block,请检查配置！！");
               
            }
        }



        /// <summary>
        /// 开始执行SkillBlock
        /// </summary>
        private void DoBlockEvent(List<SkillEvent> events)
        {

            foreach (var e in events)
            {
                SkillEventFactory.Inst.DoEvent(e, this.battle, this, battle.GetHero(skillTargetId));
            }         
        }


        /// <summary>
        /// 当前释放技能完毕
        /// </summary>
        private void AllBlockEnd()
        {
            IsPlaySkill = false;
            this.HeroGraphic.PlayAction("idle");
        }

        #endregion

        /// <summary>
        /// 新回合
        /// </summary>
        public void OnNewRound()
        {
            this.HeroLogic.OnNewRound();
            
        }


        #region skillblock  处理



        /// <summary>
        /// 每个skill block的进入条件 
        /// </summary>
        /// <returns></returns>
        private bool SkillBlock_CheckCondition(List<string> Params)
        {
            if (Params.Count == 0) return true;
            
            //
            return false;
        }

        #endregion
    }
}