using System.Collections;
using System.Collections.Generic;
using BDFramework.Editor;
using Code.Game.Battle;
using Game.Battle;
using Game.Data;
using LitJson;
using SQLite4Unity3d;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class BattleTest : MonoBehaviour
{
    public bool isByEditor = false;

    private static bool _isByEditor = false;
    public static bool IsByEditor
    {
        get { return _isByEditor; }
        
    }
    private void Start()
    {
        var d= SqliteHelper.DB;
        if (battle == null)
        {

            CreateBattle();
        }
    }

    private string myId = "1";
    private string enemyId = "1";
    private bool isStartGame = false;
    private void OnGUI()
    {
        _isByEditor = isByEditor;
        
        GUI.skin.button.fontSize = 40;
        GUI.skin.textField.fontSize = 40;
        GUI.skin.label.fontSize = 40;
      
        GUILayout.BeginVertical();
        {

            if (myHero == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("英雄:");
                myId = GUILayout.TextField(myId );

                GUILayout.EndHorizontal();

                if (GUILayout.Button("创建测试英雄", GUILayout.Width(300), GUILayout.Height(80)))
                {
                    int _id = -1;
                    if (int.TryParse(myId, out _id))
                    {
                        var hero = SqliteHelper.DB.GetTable<Hero>().Where((s)=>s.Id ==_id).First();
                        if (hero.Id != 0)
                        {
                            var logic = new HeroLogic(hero);
                            var  graphic = new HeroGraphic(hero);
                            myHero = new HeroFSM_TBS(1,logic,graphic , battle);  
                            myHero.Init();
                            
                            battle.SetHero(1 , myHero);
                            myHero.HeroGraphic.PlayAction("idle");

                            Debug.Log("hero:" + myHero.HeroGraphic.Trans.position);
                            Debug.Log("pos:" +  battle.World.GetPlayerPos(1));
                            
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("提示", "请输入正确的id哦", "确定");
                        }
                                             
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "请输入正确的id哦", "确定");
                    }
                }
                // guiLayoutUtility = new GUILayoutUtility();
            }
            else
            {
                GUILayout.Label("英雄创建成功: " + myId);
            }
            
            
            if (enemy == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("活靶子:");
                enemyId = GUILayout.TextField(enemyId );

                GUILayout.EndHorizontal();
                if (GUILayout.Button("创建活靶子", GUILayout.Width(300), GUILayout.Height(80)))
                {
                    int _id = -1;
                    if (int.TryParse(enemyId, out _id))
                    {
                        var hero = SqliteHelper.DB.GetTable<Hero>().Where((s)=>s.Id ==_id).First();
                        if (hero.Id != 0)
                        {
                            var logic = new HeroLogic(hero);
                            var  graphic = new HeroGraphic(hero);
                            enemy = new HeroFSM_TBS(2,logic,graphic , battle);  
                            enemy.Init();
                            
                            battle.SetHero(6 , enemy);
                            enemy.HeroGraphic.PlayAction("idle");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("提示", "请输入正确的id哦", "确定");
                        }
                                             
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("提示", "请输入正确的id哦", "确定");
                    }
                }
                // guiLayoutUtility = new GUILayoutUtility();
            }
           // if()
        }

//        if (myHero != null && enemy != null && isStartGame ==false)
//        {
//            if (GUILayout.Button("开始", GUILayout.Width(300), GUILayout.Height(80)))
//            {
//                battle.Start();
//            }
//        }
        
        if (myHero != null && enemy != null)
        {
            if (GUILayout.Button("[技]被动", GUILayout.Width(300), GUILayout.Height(80)))
            {
                battle.Input.EnqueueCmd(new Cmd("UseSkill" ,  new List<object>(){1,6,0}));
            }
            if (GUILayout.Button("[技]普攻", GUILayout.Width(300), GUILayout.Height(80)))
            {
                battle.Input.EnqueueCmd(new Cmd("UseSkill" ,  new List<object>(){1,6,1}));
            }
            if (GUILayout.Button("[技]技能 1", GUILayout.Width(300), GUILayout.Height(80)))
            {
                battle.Input.EnqueueCmd(new Cmd("UseSkill" ,  new List<object>(){1,6,2}));
            }
            if (GUILayout.Button("[技]技能 2", GUILayout.Width(300), GUILayout.Height(80)))
            {
                battle.Input.EnqueueCmd(new Cmd("UseSkill" ,  new List<object>(){1,6,3}));
            }
            if (GUILayout.Button("[技]技能 3", GUILayout.Width(300), GUILayout.Height(80)))
            {
                battle.Input.EnqueueCmd(new Cmd("UseSkill" ,  new List<object>(){1,6,4}));
            }
        }
        GUILayout.EndVertical();
    }


    private IBattle battle = null;
    private void CreateBattle()
    {
        var id = BattleFactory.CreateBattle(battleWorldId: 1);
        battle = BattleFactory.GetBattle(id);
        var input  = new BattleInput(battle);
        battle.Input = input;
    }


    private IHeroFSM myHero = null;
    private IHeroFSM enemy = null;

}
