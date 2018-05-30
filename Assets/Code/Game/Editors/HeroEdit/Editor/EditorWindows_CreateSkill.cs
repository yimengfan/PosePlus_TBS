using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;

public class EditorWindows_CreateSkill : EditorWindow
{

    private Dictionary<int, List<Skill>> skillList = new Dictionary<int, List<Skill>>();

    public static Skill handle;
    private static List<Buff> buffList = new List<Buff>();

    private void Awake()
    {
        Editor_TableTool.GetData_SkillData(ref skillList, "Skill");
        EditorWindows_CreateSkill.RefreshBuffData();
    }

    public static void RefreshBuffData()
    {
        buffList = Editor_TableTool.Prepare_DataByName<Buff>("Buff");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(1500), GUILayout.Height(820));
        OnGUI_SkillList();
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
        if (GUILayout.Button("一键清理所有不存在的buffid"))
        {

            Editor_TableTool.ClearUnExistBuffInSkill(ref skillList, buffList);
        }
    }

    private void OnGUI_EditorSaveButton()
    {
        if (GUILayout.Button("保存为json文件"))
        {
            if (!Editor_TableTool.CheckAllBuffExist(skillList, buffList))
            {
                EditorUtility.DisplayDialog("失败", "有不存在的buffid", "ok");
                return;
            }

            if (Editor_TableTool.IsRepeatSkillId(skillList))
            {
                EditorUtility.DisplayDialog("失败", "skillid重名", "ok");
                return;
            }
            Editor_TableTool.SaveSkillJsonFile(skillList);
//            EditorWindows_CreateHero.RefreshSkillData();
        }
    }


    private void OnGUI_EditorAddLineButton()
    {
        if (GUILayout.Button("新增加一行"))
        {
            Skill s = new Skill();
            //TODO by xiaofan
//            s.AttackRatio = new List<double>();
//            s.Buffs = new List<double>();
            List<Skill> list = new List<Skill>();
            list.Add(s);
            if (skillList.Keys.Count > 0)
            {
                List<int> keys = new List<int>(skillList.Keys);
                keys.Sort();
                skillList.Add(keys[keys.Count - 1] + 1, list);
            }
            else
            {
                skillList.Add(0, list);
            }
        }
    }

    Vector2 windowSelectPosition = Vector2.zero;
    private void OnGUI_SkillList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(1500), GUILayout.Height(800));
        {
            List<int> keyList = new List<int>(skillList.Keys);
            keyList.Sort();
            for (int i = 0; i < keyList.Count; i++)
            {
                int k = keyList[i];
                List<Skill> v;
                if (skillList.TryGetValue(k, out v))
                {
                    OnGUI_SkillScrollView(k, v);
                    TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }


    private void OnGUI_SkillScrollView(int k, List<Skill> v)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(1500));
        GUILayout.BeginHorizontal(GUILayout.Width(200));
        for (int i = 0; i < v.Count; i++)
        {
            OnGUI_EditorSkillItem(v, i, k);
            TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        }
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        OnGUI_EditorButtons(k, v);
        GUILayout.EndHorizontal();
    }

    private void OnGUI_EditorButtons(int k, List<Skill> v)
    {
        GUILayout.BeginVertical();
        this.OnGui_EditorAddOneButton(k, v);
        this.OnGui_EditorDeleteButton(k, v);
        GUILayout.EndVertical();
    }


    private void OnGui_EditorDeleteButton(int k, List<Skill> v)
    {
        if (GUILayout.Button("删除此行最后一个", GUILayout.Width(100)))
        {
            if (v.Count > 1)
            {
                v.RemoveAt(v.Count - 1);
            }
            else
            {//相当于删除一行
                skillList.Remove(k);
            }
        }
    }

    private void OnGui_EditorAddOneButton(int k, List<Skill> v)
    {
        if (GUILayout.Button("此行最后增加一个", GUILayout.Width(100)))
        {
            Skill s = new Skill();
            //TODO by xiaofan
//            s.AttackRatio = new List<double>();
//            s.Buffs = new List<double>();
            v.Add(s);
        }
    }


    private void OnGUI_EditorSkillItem(List<Skill> list, int index, int k)
    {
        Skill s = list[index];
        GUILayout.BeginVertical();
        bool isRepeat = Editor_TableTool.IsRepeatSkillId((int)s.Id, skillList);
        if (isRepeat)
        {
            GUIStyle titleStyle2 = new GUIStyle();
            titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
            GUILayout.Label("id重复", titleStyle2);
        }
        GUILayout.BeginHorizontal(GUILayout.Width(200));
        if (GUILayout.Button("add", GUILayout.Width(50)))
        {
            Skill newSkill = new Skill();
            //TODO by xiaofan
//            newSkill.Buffs = new List<double>();
//            newSkill.AttackRatio = new List<double>();
            list.Insert(index + 1, newSkill);
        }
        bool isdel = false;
        if (GUILayout.Button("del", GUILayout.Width(50)))
        {
            isdel = true;
            list.Remove(s);
            if (list.Count == 0)
            {
                skillList.Remove(k);
            }
        }
        GUILayout.EndHorizontal();
        if (!isdel)
            list[index] = (Skill)DrawTableType.DrawWindow(s);
        GUILayout.EndVertical();
    }

    public static void SwitchHandle(Skill target)
    {
        handle = target;
        TableToolMenu.OpenEditorWindow_BuffsSelect();
    }
}
