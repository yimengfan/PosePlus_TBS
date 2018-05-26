using System.Collections.Generic;
using System.Linq;
using Game.Data;
using MyJson;
using UnityEngine;
using Game.Battle.Skill;
namespace Game.Battle
{
    public class HeroLogic : IHeroLogic
    {

        public HeroLogic(Data.Hero data)
        {
            this.Data = data;
            this.State = new DataDrive_Service();
            this.TempAttribute = new DataDrive_Service();
            this.DefaultAttibute = new DataDrive_Service();
            BuffProcess = new BuffProcess(this);

            //注册所有属性
            foreach (var name in data.AttributeName)
            {
                this.DefaultAttibute.RegisterData(name);
            }
            //
            //Graphic = new HeroGraphic(this.Data);

        }

        /// <summary>
        /// id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 英雄固定属性3
        /// </summary>
        public Data.Hero Data { get; private set; }


        /// <summary>
        /// 英雄状态
        /// </summary>
        public ADataDrive State { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //默认属性赋值
            var length = Data.AttributeName.Count;
            for (int i = 0; i < length; i++)
            {
                var attr = Data.AttributeName[i];
                var value = Data.AttributeValue[i];
                this.DefaultAttibute.SetData(attr, value);
            }

            //被攻击
            this.State.RegisterData("Behurt");
        }

        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsSurvival { get; }

        /// <summary>
        /// 英雄临时属性
        /// </summary>
        protected ADataDrive TempAttribute { get; set; }

        /// <summary>
        /// 默认属性
        /// </summary>
        protected ADataDrive DefaultAttibute { get; set; }


        /// <summary>
        /// Buff 管理器
        /// </summary>
        protected BuffProcess BuffProcess { get; set; }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetAttribute(string name)
        {
            var temp = TempAttribute.GetData<double>(name);
            var main = this.DefaultAttibute.GetData<double>(name);
            return temp + main;
        }

        #region 状态

        public void ListeningState(string state, ADataDrive.CallBack callback)
        {
            this.State.RegAction(state, callback);
        }

        public void RemoveListeningState(string state, ADataDrive.CallBack callback)
        {
            this.State.RemoveAction(state, callback);
        }

        #endregion

        #region Buff & 行为

        public int AddBuff(IBuff buff)
        {
            return this.BuffProcess.AddBuffs(buff);
        }

        public bool RemoveBuff(int id)
        {
            return this.BuffProcess.RemoveBuff(id);
        }

        #endregion


        #region 攻击 技能
    

        /// <summary>
        /// 攻击
        /// </summary>
        /// <returns></returns>

        public Data.Skill Attack()
        {
            return UseSkill(0);

        }

        /// <summary>
        /// 使用技能
        /// 0 ->普攻
        /// 1 - 3 ->固定技能
        /// 4 - ~ ->特技特效
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Data.Skill UseSkill(int index)
        {
            Data.Skill skill = new Data.Skill();
            skill.Id = -1;
            if (this.Data.Skills.Count < 5)
            {
                BDebug.Log("技能数量错误! id:" + this.Data.Id);
            }
            //获取skill
             var id = this.Data.Skills[index];
             skill = SqliteHelper.DB.GetTable<Data.Skill>().Where((s) => s.Id == id).First();

            return skill;
        }


        /// <summary>
        /// 被攻击
        /// </summary>
        /// <param name="skill"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Behurt(Data.Skill skill)
        {
            this.State.SetData("Behurt" ,null);
//            foreach (var b in skill.SkillBlocks)
//            {
//                for (int i = 0; i < b.Data.Params_Formula.Count; i++)
//                {
//                    var f = b.Data.Params_Formula[i];
//
//                    f = ReplaceFormulaAttribute(f, "a.");
//                    if (f.Contains("a"))
//                    {
//                        BDebug.LogError("buff公式有错误 id:" + b.Data.Id);
//                        break;
//                    }
//
//
//                    b.Data.Params_Formula[i] = f;
//                }         
//            }
//            //一批次添加buff
//            this.BuffProcess.AddBuffs(skill.Buffs.ToArray());
        }

        

        #endregion
        
        
        /// <summary>
        /// onupdate
        /// </summary>
        public void OnNewRound()
        {
            //
            if (BuffProcess != null)
            {
                BuffProcess.OnNewRound();
            }
        }

        #region 工具方法


        /// <summary>
        /// 替换公式属性
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="replaceRrefixion"></param>
        /// <returns></returns>
        private string ReplaceFormulaAttribute( string formula , string replaceRrefixion )
        {
            List<string>  keys = new List<string>();
            foreach (var key in keys)
            {
                var s = replaceRrefixion + key;
                if (formula.Contains(s))
                {
                    formula.Replace(s, this.GetAttribute(key).ToString());
                    break;
                }
            }

            return formula;
        }

        

        #endregion
    }
}