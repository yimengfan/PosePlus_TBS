using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Game.Data;

public class EditorWindows_CreateHero : EditorWindow
{
    private List<object> heroList;

//    private static List<Skill> skillList = new List<Skill>();
    private Type _target;

    public void Show<T>()
    {
        this._target = typeof(T);
        heroList = Editor_TableTool.GetData<T>();
    }

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
//        OnGUI_EditorRepairButton();
        OnGUI_EditorSaveButton();
        GUILayout.EndHorizontal();
    }

    private void OnGUI_EditorRepairButton()
    {
        if (GUILayout.Button("一键清理所有不存在的skillid"))
        {
//            Editor_TableTool.ClearUnExistSkillInHero(ref heroList, skillList);
        }
    }

    private void OnGUI_EditorSaveButton()
    {
        if (GUILayout.Button("保存为json文件"))
        {
            if (!Editor_TableTool.CheckForeignKey(heroList, _target))
            {
                EditorUtility.DisplayDialog("失败", "有不存在的skillid", "ok");
                return;
            }

            if (Editor_TableTool.IsRepeatId(heroList,_target))
            {
                EditorUtility.DisplayDialog("失败", "id重名或者属性名为空", "ok");
                return;
            }


            Editor_TableTool.SaveJsonFile(heroList, this._target);
        }
    }


    private void OnGUI_EditorAddLineButton()
    {
        if (GUILayout.Button("新增加一行"))
        {
            object obj = Editor_TableTool.CreateInstance(this._target);
            List<object> list = new List<object>();
            list.Add(obj);
            int maxId = Editor_TableTool.GetMaxId(heroList);
            PropertyInfo idProp = this._target.GetProperty("Id");
            idProp.SetValue(obj, maxId+1);
            heroList.Add(obj);
        }
    }

    private void OnGUI_MainWindow()
    {
        GUILayout.BeginVertical();
        windowSelectPosition =
            GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(1500), GUILayout.Height(800));
        {
            heroList = DrawTableType.DrawWindowWithSort(heroList, 200, 800, 1, "NextLevel");
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    Vector2 windowSelectPosition = Vector2.zero;

    public static void SwitchHandle(object target,TableConfig cfg)
    {
//        handle = target;
        TableToolMenu.OpenEditorWindow_SkillsSelect(target,cfg);
    }

//    public static object handle;
}