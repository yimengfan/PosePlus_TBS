using System.Collections;
using System.Collections.Generic;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

[SkillEventAttribute("PlayFlyItem","播放飞行道具")]
public class SkillEvent_PlayFlyItemEditor : ISkillEventEditor {
	public SkillEvent OnGuiEditor(SkillEvent se)
	{
		se.DoubleParams0 = EditorGUILayout.DoubleField("飞行持续时间", se.DoubleParams0);
		GUILayout.BeginHorizontal();
		GUILayout.Label("特效路径");
		GUILayout.Label(se.StrParam0);
		
		obj =  EditorGUILayout.ObjectField("选择飞行道具",obj,typeof(Object),false);
		if (obj != null)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			se.StrParam0 = path.Replace("Assets/Resource/Resources/", "").Replace(".prefab", "");
		}
		GUILayout.EndHorizontal();
		se.DoubleParams1 = EditorGUILayout.DoubleField("触发事件 时间", se.DoubleParams1);
		se.StrParam1 = EditorGUILayout.TextField("触发事件名", se.StrParam1);
		return se;
	}

	private Object obj;
}
