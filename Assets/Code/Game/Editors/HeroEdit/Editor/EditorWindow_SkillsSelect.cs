using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;
public class EditorWindow_SkillsSelect : EditorWindow
{
    private Dictionary<int, List<Skill>> skillList = new Dictionary<int, List<Skill>>();

    private Dictionary<int, Vector2> selectPosDic = new Dictionary<int, Vector2>();

    private List<double> selectList;

    public void Awake()
    {
        Editor_TableTool.GetData_SkillData(ref skillList, "Skill");
        selectList = EditorWindows_CreateHero.handle.Skills;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(1000), GUILayout.Height(800));
        //OnGUI_EditorButton();
        OnGUI_SkillList();
        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    //private void OnGUI_EditorButton()
    //{
    //    GUILayout.BeginHorizontal(GUILayout.Width(1000));
    //    if (GUILayout.Button("保存", GUILayout.Width(100), GUILayout.Height(50)))
    //    {
    //        selectList.Sort();
    //        EditorWindows_CreateHero.handle.SkillBlocks = selectList;
    //    }
    //    GUILayout.EndHorizontal();
    //    TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
    //}

    Vector2 windowSelectPosition = Vector2.zero;

    private void OnGUI_SkillList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(1010), GUILayout.Height(800));
        {
            foreach (KeyValuePair<int, List<Skill>> kv in skillList)
            {
                OnGUI_SkillScrollView(kv);
            }
        }
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        GUILayout.EndVertical();
    }

    private void OnGUI_SkillScrollView(KeyValuePair<int, List<Skill>> kv)
    {
        GUILayout.BeginHorizontal();
        Vector2 selectPos;
        if (!selectPosDic.TryGetValue(kv.Key, out selectPos))
        {
            selectPos = Vector2.zero;
            selectPosDic.Add(kv.Key, selectPos);
        }
        selectPosDic[kv.Key] = GUILayout.BeginScrollView(selectPos, GUILayout.Width(1000), GUILayout.Height(200));
        {
            GUILayout.BeginHorizontal(GUILayout.Width(200), GUILayout.Height(200));
            for (int i = 0; i < kv.Value.Count; i++)
            {
                OnGUI_EditorSkillItem(kv.Value[i]);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
        GUILayout.EndHorizontal();
    }


    private void OnGUI_EditorSkillItem(Skill h)
    {
        GUILayout.BeginHorizontal();
        OnGUI_EditorSkillItemToggle(h);
        GUILayout.BeginVertical();
        DrawTableType.DrawDisplayWindow(h);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    private void OnGUI_EditorSkillItemToggle(Skill h)
    {
        bool isSelect = selectList.Contains(h.Id);
        isSelect = GUILayout.Toggle(isSelect, "");
        if (isSelect && !selectList.Contains(h.Id))
        {
            selectList.Add(h.Id);
        }
        if (!isSelect && selectList.Contains(h.Id))
        {
            selectList.Remove(h.Id);
        }

    }


}
