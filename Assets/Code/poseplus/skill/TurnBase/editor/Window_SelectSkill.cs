using System;
using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using UnityEditor;

public class Window_SelectSkill : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("选择创建技能id");
        this.ShowSkillList();
        GUILayout.EndVertical();
    }

    private Vector2 pos = Vector2.zero;

    private Game.Battle.Skill.Skills _skill;
    private int _index;

    private void ShowSkillList()
    {
        if (this._skills == null)
        {
            Debug.LogError("DB不存在skill数据");
            return;
        }

        GUILayout.BeginScrollView(pos, GUILayout.Width(100), GUILayout.Height(800));
        foreach (Skill skill in _skills)
        {
            if (GUILayout.Button(skill.Id + ""))
            {
                Game.Battle.Skill.Skill sk = new Game.Battle.Skill.Skill();
                sk.Id = Convert.ToInt32(skill.Id);
                this._skill.SkillList.Insert(this._index,sk);
                this.Close();
            }
        }

        GUILayout.EndScrollView();
    }

    private IEnumerable<Skill> _skills;

    private void Awake()
    {
        this._skills = SqliteHelper.DB.GetTable<Skill>();
    }

    public void Show(Game.Battle.Skill.Skills skill,int index)
    {
        this._skill = skill;
        this._index = index;
        base.Show();
    }
}