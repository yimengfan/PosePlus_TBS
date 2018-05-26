using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;

public class EditorWindows_CreateHero : EditorWindow
{

    private Dictionary<int, List<Hero>> heroList = new Dictionary<int, List<Hero>>();

    private static List<Skill> skillList = new List<Skill>();

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(1500), GUILayout.Height(820));
        OnGUI_MainWindow();
        OnGUI_EditorBottomButtons();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }


    private void OnGUI_EditorBottomButtons()
    {
        GUILayout.BeginHorizontal();
        OnGUI_EditorAddLineButton();
        OnGUI_EditorRepairButton();
        OnGUI_EditorSaveButton();
        GUILayout.EndHorizontal();
    }

    private void OnGUI_EditorRepairButton()
    {
        if (GUILayout.Button("一键清理所有不存在的skillid"))
        {

            Editor_TableTool.ClearUnExistSkillInHero(ref heroList, skillList);
        }
    }

    private void OnGUI_EditorSaveButton()
    {
        if (GUILayout.Button("保存为json文件"))
        {
            if (!Editor_TableTool.CheckAllSkillExist(heroList, skillList))
            {
                EditorUtility.DisplayDialog("失败", "有不存在的skillid", "ok");
                return;
            }

            if (Editor_TableTool.isRepeatHeroIdOrNullAttributeName(heroList))
            {
                EditorUtility.DisplayDialog("失败", "id重名或者属性名为空", "ok");
                return;
            }


            Editor_TableTool.SaveHeroJsonFile(heroList);
        }
    }


    private void OnGUI_EditorAddLineButton()
    {
        if (GUILayout.Button("新增加一行"))
        {
            Hero h = new Hero();
            h.AttributeName = new List<string>();
            h.AttributeValue = new List<double>();
            h.Skills = new List<double>();
            List<Hero> list = new List<Hero>();
            list.Add(h);
            if (heroList.Keys.Count > 0)
            {
                List<int> keys = new List<int>(heroList.Keys);
                keys.Sort();
                heroList.Add(keys[keys.Count - 1] + 1, list);
            }
            else
            {
                heroList.Add(0, list);
            }
        }
    }



    public void Awake()
    {
        Editor_TableTool.GetData_HeroData(ref heroList, "HeroLogic");
        EditorWindows_CreateHero.RefreshSkillData();
    }

    public static void RefreshSkillData()
    {
        skillList = Editor_TableTool.Prepare_DataByName<Skill>("Skill");
    }

    private void OnGUI_MainWindow()
    {
        OnGUI_HeroList();
    }

    Vector2 windowSelectPosition = Vector2.zero;

    private void OnGUI_HeroList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(1500), GUILayout.Height(800));
        {
            List<int> keyList = new List<int>(heroList.Keys);
            keyList.Sort();
            for (int i = 0; i < keyList.Count; i++)
            {
                int k = keyList[i];
                List<Hero> v;
                if (heroList.TryGetValue(k, out v))
                {
                    OnGUI_HeroScrollView(k, v);
                    TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OnGUI_HeroScrollView(int k, List<Hero> v)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(1500));
        GUILayout.BeginHorizontal(GUILayout.Width(200));
        for (int i = 0; i < v.Count; i++)
        {
            OnGUI_EditorHeroItem(v, i, k);
            TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        }
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        OnGUI_EditorButtons(k, v);
        GUILayout.EndHorizontal();
    }


    private void OnGUI_EditorButtons(int k, List<Hero> v)
    {
        GUILayout.BeginVertical();
        this.OnGui_EditorAddOneButton(k, v);
        this.OnGui_EditorDeleteButton(k, v);
        GUILayout.EndVertical();
    }


    private void OnGui_EditorDeleteButton(int k, List<Hero> v)
    {
        if (GUILayout.Button("删除此行最后一个", GUILayout.Width(100)))
        {
            if (v.Count > 1)
            {
                v.RemoveAt(v.Count - 1);
            }
            else
            {//相当于删除一行
                heroList.Remove(k);
            }
        }
    }

    private void OnGui_EditorAddOneButton(int k, List<Hero> v)
    {
        if (GUILayout.Button("此行最后增加一个", GUILayout.Width(100)))
        {
            Hero tp = new Hero();
            tp.AttributeValue = new List<double>();
            tp.AttributeName = new List<string>();
            tp.Skills = new List<double>();
            v.Add(tp);
        }
    }

    private void OnGUI_EditorHeroItem(List<Hero> list, int index, int k)
    {
        Hero h = list[index];
        GUILayout.BeginVertical();
        bool isRepeat = Editor_TableTool.IsRepeatHeroId((int)h.Id, heroList);
        if (isRepeat)
        {
            GUIStyle titleStyle2 = new GUIStyle();
            titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
            GUILayout.Label("id重复", titleStyle2);
        }
        GUILayout.BeginHorizontal(GUILayout.Width(200));
        if (GUILayout.Button("add", GUILayout.Width(50)))
        {
            Hero newHero = new Hero();
            newHero.AttributeName = new List<string>();
            newHero.AttributeValue = new List<double>();
            newHero.Skills = new List<double>();
            list.Insert(index + 1, newHero);
        }
        bool isdel = false;
        if (GUILayout.Button("del", GUILayout.Width(50)))
        {
            list.Remove(h);
            if (list.Count == 0)
            {
                heroList.Remove(k);
            }
            isdel = true;
        }
        GUILayout.EndHorizontal();
        if (!isdel)
            list[index] = (Hero)DrawTableType.DrawWindow(h);
        GUILayout.EndVertical();
    }

    public static void SwitchHandle(Hero target)
    {
        handle = target;
        TableToolMenu.OpenEditorWindow_SkillsSelect();
    }
    public static Hero handle;

}
