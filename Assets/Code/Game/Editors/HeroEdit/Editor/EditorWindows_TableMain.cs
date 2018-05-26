using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;

public class EditorWindows_TableMain : EditorWindow
{
    

    private void Awake()
    {
        Editor_TableTool.GetData_TableConfigData();
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        OnGUI_WindowBtn();
        GUILayout.EndVertical();
    }

    private void OnGUI_WindowBtn()
    {
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("Hero表编辑", GUILayout.Width(150), GUILayout.Height(30)))
        {
            TableToolMenu.OpenEditorWindow_CreateHero();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("Skill表编辑", GUILayout.Width(150), GUILayout.Height(30)))
        {
            TableToolMenu.OpenEditorWindow_CreateSkill();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("Buff表编辑", GUILayout.Width(150), GUILayout.Height(30)))
        {
            TableToolMenu.OpenEditorWindow_CreateBuff();
        }
        GUILayout.EndHorizontal();
    }
}
