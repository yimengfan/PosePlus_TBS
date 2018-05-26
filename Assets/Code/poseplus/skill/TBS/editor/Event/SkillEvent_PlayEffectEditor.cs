using System.Collections;
using System.Collections.Generic;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

[SkillEventAttribute("PlayEffect","播放技能特效")]
public class SkillEvent_PlayEffectEditor : ISkillEventEditor {
	string[] radioName=new string[]{"特效位于目标身上 ，每个英雄身上有个挂载特效的节点","特效位于我方 或者敌方中心"};//单选按钮显示内容
	bool[] radioState=new bool[] {false,false};
	int radioInt=0;
	public SkillEvent OnGuiEditor(SkillEvent se)
	{
		GUILayout.Label("特效播放位置");
		radioInt = (int)(se.DoubleParams0 - 1);
		if (radioInt < 0) radioInt = 0;
		this.Reset();
		for (int i = 0; i < radioName.Length; i++) //用复选框实现单选按钮
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(30);
			if (radioState[i] != EditorGUILayout.Toggle(radioName[i], radioState[i]))
			{
				radioState[i]=!radioState[i];//改变选择状态
				if(radioInt!=i)
					radioState[radioInt]=false;
				radioInt=i;
			}
			GUILayout.EndHorizontal();
		}
		se.DoubleParams0 = radioInt + 1;
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("特效路径");
		GUILayout.Label(se.StrParam0);
		
		obj =  EditorGUILayout.ObjectField("选择特效",obj,typeof(Object),false);
		if (obj != null)
		{
			string path = AssetDatabase.GetAssetPath(obj);
			se.StrParam0 = path.Replace("Assets/Resource/Resources/", "").Replace(".prefab", "");
		}
		GUILayout.EndHorizontal();

		return se;
	}

	private Object obj;

	public void Reset()
	{
		for (int i = 0; i < radioState.Length; i++)
		{
			radioState[i] = false;
		}
		
		radioState[radioInt] = true;
	}
}
