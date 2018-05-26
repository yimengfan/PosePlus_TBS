using System;
using System.Collections;
using System.Collections.Generic;
using Excel.Exceptions;
using UnityEditor;

namespace Game.Battle
{
    /// <summary>
    /// 指令集
    /// </summary>
    public class Cmd
    {
        public Cmd(string name, List<object> myParams)
        {
            this.Name = name;
            this.Params = myParams;
        }

        /// <summary>
        /// 指令名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 指令附带的参数
        /// </summary>
        public List<object> Params { get; private set; }
    }

    /// <summary>
    /// 输入源
    /// </summary>
    public class BattleInput : IBattleInput
    {
        private IBattle battle;

        private List<Cmd> cmdCacheList;

        //
        public BattleInput(IBattle battle)
        {
            this.battle = battle;
            cmdCacheList = new List<Cmd>();
            InputSate = new DataDrive_Service();


            //所有状态
            this.InputSate.RegisterData("OnInput");
        }

        /// <summary>
        /// 输入的状态
        /// </summary>
        public ADataDrive InputSate { get; private set; }

        /// <summary>
        /// 压入一个指令
        /// </summary>
        /// <param name="cmd"></param>
        public void EnqueueCmd(Cmd cmd)
        {
            cmdCacheList.Add(cmd);
            // this.InputSate.SetData("OnInput" , cmd);
            var b = battle.GetHero((int) cmd.Params[0]);
            b.Input(cmd);
        }
    }
}