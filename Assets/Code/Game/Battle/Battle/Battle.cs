using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BDFramework.ResourceMgr;
using Code.Game.Battle;
using UnityEngine;

namespace Game.Battle
{
    public class Battle : IBattle
    {
        public IBattleRule Rule { get; private set; }
        public IBattleInput Input { get;  set; }
        public IBattleWorld World { get; private set; }
        public ADataDrive State { get;private set; }

        private bool isStartGame = false;
        public Battle(IBattleRule rule , IBattleWorld world)
        {
            this.Rule = rule;
            this.World = world;
            this.heroMap =  new Dictionary<int, IHeroFSM>();
            this.State = new DataDrive_Service();
            for (int i = 0; i < 10; i++)
            {
                heroMap[i] = null;
            }
            //
            State.RegisterData("OnNewRound");     //回合
            State.RegisterData("OnCanOperation"); //可以操作的下标
            State.RegisterData("OnBattleEnd");    //战斗结束
            
            //每一个新回合
            State.RegAction("OnNewRound", o =>
            {
                OnNewRound();
            });
        }

        #region 英雄管理

        /// <summary>
        /// 英雄列表
        /// 0-4 自己
        /// 5-9 敌人
        /// </summary>
        private Dictionary<int,IHeroFSM>  heroMap;


        /// <summary>
        /// 设置英雄位置
        /// 0-5 自己
        /// 6-11 敌人
        /// </summary>
        /// <param name="index"></param>
        /// <param name="heroFSM"></param>
        public void SetHero(int index, IHeroFSM heroFSM)
        {
            
            heroMap[index] = heroFSM;
            var pos = World.GetPlayerPos(index);
            heroFSM.HeroGraphic.Trans.position = pos;
            heroFSM.ID = index;
            //阵营划分
            if ( heroFSM.Camp == 1)
            {
                heroFSM.HeroGraphic.Trans.eulerAngles = new Vector3(0, 0, 0); 
            }
            else if(heroFSM.Camp == 2)
            {
                heroFSM.HeroGraphic.Trans.eulerAngles = new Vector3(0, 180, 0); 
            }
        }

        /// <summary>
        /// 交换英雄位置
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void SwapHeroIndex(int a, int b)
        {
            var heroA = heroMap[a];
            heroMap[a] = heroMap[b];
            heroMap[b] = heroA;

            heroMap[a].HeroGraphic.Trans.position = World.GetPlayerPos(a);
            heroMap[b].HeroGraphic.Trans.position = World.GetPlayerPos(b);
        }

        /// <summary>
        /// 获取英雄的所有下标
        /// </summary>
        /// <returns></returns>
        public int[] GetHeroIndexs()
        {
            return this.heroMap.Keys.ToArray();
        }

        /// <summary>
        /// 获取一个英雄
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref=" "></exception>
        public IHeroFSM GetHero(int index)
        {
            IHeroFSM  heroFsm = null;
            this.heroMap.TryGetValue(index, out heroFsm);
            return heroFsm;
        }

        /// <summary>
        /// 获取一组英雄
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        public List<IHeroFSM> GetHeroes(int[] indexs)
        {
            List<IHeroFSM> heroes = new List<IHeroFSM>();
            foreach (var i in indexs)
            {
                IHeroFSM  hf = null;
                this.heroMap.TryGetValue(i, out hf);
                heroes.Add(hf);
            }

            return heroes;
        }

        /// <summary>
        /// 移除英雄
        /// </summary>
        /// <param name="index"></param>
        public void RemoveHero(int index)
        {
            heroMap.Remove(index);
        }

        #endregion

        #region  生命周期
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //加载场景地图
            this.World.Load();
        }

        
        
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            this.isStartGame = true;
            //设置好player index
            foreach (var h in this.heroMap)
            {
                h.Value.ID = h.Key;
            }
            Rule.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
          
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            
        }

        /// <summary>
        /// 新回合
        /// </summary>
        public void OnNewRound()
        {
            foreach (var value in this.heroMap.Values)
            {
                value.OnNewRound();
            }
        }
        #endregion

    }
}