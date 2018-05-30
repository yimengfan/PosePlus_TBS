using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

public class Window_SelectSkillEvent : EditorWindow
{
    private void OnGUI()
    {
        if (this._types == null || this._types.Length == 0)
        {
            Debug.LogError("没有找到继承ISkillEventEditor类");
            return;
        }

        GUILayout.BeginVertical();
        this.ShowEventList();
        GUILayout.EndVertical();
    }

    private void ShowEventList()
    {
        foreach (Type t in this._types)
        {
            SkillEventAttribute attribute =
                (SkillEventAttribute) t.GetCustomAttribute(typeof(SkillEventAttribute));
            if (GUILayout.Button("EventName:" + attribute.Name+" "+attribute.Des))
            {
                SkillEvent se = new SkillEvent();
                se.EventName = attribute.Name;
                se.FrameId = this._curFrame;
                this._curBlock.Events.Add(se);
                this._curSkillEventList.Add(se);
                this.Close();
            }
        }
    }

    private int _curFrame;
    private SkillBlock _curBlock;
    private List<SkillEvent> _curSkillEventList;

    public void Show(int frame, SkillBlock sb,List<SkillEvent> curSkillEventList)
    {
        this._curBlock = sb;
        this._curFrame = frame;
        this._curSkillEventList = curSkillEventList;
        this.Show();
    }

    private Type[] _types;

    private void Awake()
    {
        this._types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISkillEventEditor))))
            .ToArray();
    }
}