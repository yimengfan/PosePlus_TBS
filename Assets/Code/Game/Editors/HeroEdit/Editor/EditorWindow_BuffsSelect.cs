using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;

public class EditorWindow_BuffsSelect : EditorWindow
{
    private List<Buff> buffList = new List<Buff>();

    private List<double> selectList;

    public void Awake()
    {
        Editor_TableTool.GetData_BuffData(ref buffList, "Buff");
        selectList = EditorWindows_CreateSkill.handle.SkillBlocks;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(600), GUILayout.Height(800));
        //OnGUI_EditorButton();
        OnGUI_BuffList();
        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    Vector2 windowSelectPosition = Vector2.zero;

    private void OnGUI_BuffList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(610), GUILayout.Height(800));
        {
            foreach (Buff b in buffList)
            {
                OnGUI_EditorBuffItem(b);
            }
        }
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        GUILayout.EndVertical();
    }

    private void OnGUI_EditorBuffItem(Buff b)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(610));
        OnGUI_EditorSkillItemToggle(b);
        GUILayout.BeginVertical();
        DrawTableType.DrawDisplayWindow(b);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
    }

    private void OnGUI_EditorSkillItemToggle(Buff b)
    {
        bool isSelect = selectList.Contains(b.Id);
        isSelect = GUILayout.Toggle(isSelect, "");
        if (isSelect && !selectList.Contains(b.Id))
        {
            selectList.Add(b.Id);
        }
        if (!isSelect && selectList.Contains(b.Id))
        {
            selectList.Remove(b.Id);
        }

    }

}
