using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using Game.Data;
using Mono.CompilerServices.SymbolWriter;
using UnityEditor.Graphs;

public class EditorWindow_SkillsSelect : EditorWindow
{
    private List<object> skillList;

    private List<double> selectList;

    private Type _target;
    private object _aimObj;
    private TableConfig _cfg;


    public void Show<T>(object aimObj,TableConfig cfg)
    {
        this._aimObj = aimObj;
        this._cfg = cfg;
        Type t = aimObj.GetType();
        PropertyInfo pi = t.GetProperty(cfg.Name);
        selectList = (List<double>) pi.GetValue(aimObj);
        skillList = Editor_TableTool.GetData<T>();
        base.Show();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(1000), GUILayout.Height(800));
        OnGUI_SkillList();
        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    Vector2 windowSelectPosition = Vector2.zero;

    private void OnGUI_SkillList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(1010), GUILayout.Height(800));
        {
            foreach (object obj in skillList)
            {
                OnGUI_EditorSkillItem(obj);
            }
        }
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        GUILayout.EndVertical();
    }

    private void OnGUI_EditorSkillItem(object o)
    {
        GUILayout.BeginHorizontal();
        OnGUI_EditorSkillItemToggle(o);
        GUILayout.BeginVertical();
        DrawTableType.DrawDisplayWindow(o);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    private void OnGUI_EditorSkillItemToggle(object o)
    {
        if ((string.IsNullOrEmpty(this._cfg.ForeignKey))) return;
        string match = this._cfg.ForeignKey.Substring(this._cfg.ForeignKey.LastIndexOf(".")+1);
        PropertyInfo pi = o.GetType().GetProperty(match);
        double matchId = (double) pi.GetValue(o);
        bool isSelect = selectList.Contains(matchId);
        isSelect = GUILayout.Toggle(isSelect, "");
        if (isSelect && !selectList.Contains(matchId))
        {
            selectList.Add(matchId);
        }
        if (!isSelect && selectList.Contains(matchId))
        {
            selectList.Remove(matchId);
        }

    }

}
